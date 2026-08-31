// Copyright Epic Games, Inc. All Rights Reserved.

#include "Characters/KyokaiCharacter.h"

#include "Camera/CameraComponent.h"
#include "Characters/KyokaiMovementComponent.h"
#include "Components/CapsuleComponent.h"
#include "Components/StaticMeshComponent.h"
#include "Engine/Engine.h"
#include "Engine/StaticMesh.h"
#include "UObject/ConstructorHelpers.h"
#include "Engine/LocalPlayer.h"
#include "EnhancedInputComponent.h"
#include "EnhancedInputSubsystems.h"
#include "GameFramework/PlayerController.h"
#include "GameFramework/SpringArmComponent.h"
#include "InputAction.h"
#include "InputMappingContext.h"
#include "TimerManager.h"

AKyokaiCharacter::AKyokaiCharacter(const FObjectInitializer& ObjectInitializer)
	: Super(ObjectInitializer.SetDefaultSubobjectClass<UKyokaiMovementComponent>(ACharacter::CharacterMovementComponentName))
{
	PrimaryActorTick.bCanEverTick = true;

	GetCapsuleComponent()->InitCapsuleSize(42.0f, 96.0f);

	BodyMesh = CreateDefaultSubobject<UStaticMeshComponent>(TEXT("BodyMesh"));
	BodyMesh->SetupAttachment(RootComponent);
	BodyMesh->SetCollisionEnabled(ECollisionEnabled::NoCollision);
	BodyMesh->SetCastShadow(true);
	// Engine cube is 100cm per side, centered on its pivot like the capsule
	// is centered on its own root - scale it to match the capsule's 84cm
	// diameter x 192cm height so the placeholder reads as "the character".
	BodyMesh->SetRelativeScale3D(FVector(0.84f, 0.84f, 1.92f));
	static ConstructorHelpers::FObjectFinder<UStaticMesh> PlaceholderCubeFinder(TEXT("/Engine/BasicShapes/Cube.Cube"));
	if (PlaceholderCubeFinder.Succeeded())
	{
		BodyMesh->SetStaticMesh(PlaceholderCubeFinder.Object);
	}

	bUseControllerRotationPitch = false;
	bUseControllerRotationYaw = false;
	bUseControllerRotationRoll = false;
	JumpMaxCount = 1;
	JumpMaxHoldTime = 0.18f;

	CameraBoom = CreateDefaultSubobject<USpringArmComponent>(TEXT("CameraBoom"));
	CameraBoom->SetupAttachment(RootComponent);
	CameraBoom->TargetArmLength = 1200.0f;
	CameraBoom->SetRelativeRotation(FRotator(0.0f, -90.0f, 0.0f));
	CameraBoom->TargetOffset = FVector(0.0f, 0.0f, 140.0f);
	CameraBoom->bDoCollisionTest = false;
	CameraBoom->bUsePawnControlRotation = false;
	CameraBoom->bInheritPitch = false;
	CameraBoom->bInheritYaw = false;
	CameraBoom->bInheritRoll = false;
	CameraBoom->bEnableCameraLag = true;
	CameraBoom->CameraLagSpeed = 14.0f;

	SideViewCamera = CreateDefaultSubobject<UCameraComponent>(TEXT("SideViewCamera"));
	SideViewCamera->SetupAttachment(CameraBoom, USpringArmComponent::SocketName);
	SideViewCamera->bUsePawnControlRotation = false;
	SideViewCamera->FieldOfView = 50.0f;
}

void AKyokaiCharacter::BeginPlay()
{
	Super::BeginPlay();

	LastGroundedTime = GetWorld()->GetTimeSeconds();

	if (GameplayMappingContext)
	{
		if (const APlayerController* PlayerController = Cast<APlayerController>(Controller))
		{
			if (UEnhancedInputLocalPlayerSubsystem* InputSubsystem =
				ULocalPlayer::GetSubsystem<UEnhancedInputLocalPlayerSubsystem>(PlayerController->GetLocalPlayer()))
			{
				InputSubsystem->AddMappingContext(GameplayMappingContext, 0);
			}
		}
	}
}

void AKyokaiCharacter::Tick(const float DeltaSeconds)
{
	Super::Tick(DeltaSeconds);

	const UKyokaiMovementComponent* Movement = GetKyokaiMovement();
	if (!Movement)
	{
		return;
	}

	if (Movement->IsMovingOnGround())
	{
		LastGroundedTime = GetWorld()->GetTimeSeconds();
		bAirDashAvailable = true;
	}
	else if (bIsSliding)
	{
		OnSlideReleased();
	}

	TryConsumeJumpBuffer();
	UpdateCameraLookAhead(DeltaSeconds);

	if (bShowPrototypeDebug)
	{
		DrawPrototypeDebug();
	}
}

void AKyokaiCharacter::SetupPlayerInputComponent(UInputComponent* PlayerInputComponent)
{
	Super::SetupPlayerInputComponent(PlayerInputComponent);

	const bool bHasEnhancedBindings = GameplayMappingContext && MoveAction && JumpAction && SlideAction && DashAction;
	if (bHasEnhancedBindings)
	{
		if (UEnhancedInputComponent* EnhancedInput = Cast<UEnhancedInputComponent>(PlayerInputComponent))
		{
			EnhancedInput->BindAction(MoveAction, ETriggerEvent::Triggered, this, &AKyokaiCharacter::MoveEnhanced);
			EnhancedInput->BindAction(MoveAction, ETriggerEvent::Completed, this, &AKyokaiCharacter::MoveEnhanced);
			EnhancedInput->BindAction(JumpAction, ETriggerEvent::Started, this, &AKyokaiCharacter::OnJumpPressed);
			EnhancedInput->BindAction(JumpAction, ETriggerEvent::Completed, this, &AKyokaiCharacter::OnJumpReleased);
			EnhancedInput->BindAction(SlideAction, ETriggerEvent::Started, this, &AKyokaiCharacter::OnSlidePressed);
			EnhancedInput->BindAction(SlideAction, ETriggerEvent::Completed, this, &AKyokaiCharacter::OnSlideReleased);
			EnhancedInput->BindAction(DashAction, ETriggerEvent::Started, this, &AKyokaiCharacter::OnDashPressed);
		}
	}
	else
	{
		PlayerInputComponent->BindAxis(TEXT("MoveHorizontal"), this, &AKyokaiCharacter::MoveLegacy);
		PlayerInputComponent->BindAction(TEXT("Jump"), IE_Pressed, this, &AKyokaiCharacter::OnJumpPressed);
		PlayerInputComponent->BindAction(TEXT("Jump"), IE_Released, this, &AKyokaiCharacter::OnJumpReleased);
		PlayerInputComponent->BindAction(TEXT("Slide"), IE_Pressed, this, &AKyokaiCharacter::OnSlidePressed);
		PlayerInputComponent->BindAction(TEXT("Slide"), IE_Released, this, &AKyokaiCharacter::OnSlideReleased);
		PlayerInputComponent->BindAction(TEXT("Dash"), IE_Pressed, this, &AKyokaiCharacter::OnDashPressed);
	}

	PlayerInputComponent->BindAction(TEXT("TogglePrototypeDebug"), IE_Pressed, this, &AKyokaiCharacter::TogglePrototypeDebug);
}

void AKyokaiCharacter::Landed(const FHitResult& Hit)
{
	Super::Landed(Hit);
	LastGroundedTime = GetWorld()->GetTimeSeconds();
	bAirDashAvailable = true;
	TryConsumeJumpBuffer();
}

bool AKyokaiCharacter::CanJumpInternal_Implementation() const
{
	if (Super::CanJumpInternal_Implementation())
	{
		return true;
	}

	const UKyokaiMovementComponent* Movement = GetKyokaiMovement();
	if (!Movement || !Movement->IsFalling() || JumpCurrentCount >= JumpMaxCount)
	{
		return false;
	}

	return GetWorld()->GetTimeSeconds() - LastGroundedTime <= CoyoteTime;
}

UKyokaiMovementComponent* AKyokaiCharacter::GetKyokaiMovement() const
{
	return Cast<UKyokaiMovementComponent>(GetCharacterMovement());
}

void AKyokaiCharacter::MoveEnhanced(const FInputActionValue& Value)
{
	ApplyMoveInput(Value.Get<float>());
}

void AKyokaiCharacter::MoveLegacy(const float Value)
{
	ApplyMoveInput(Value);
}

void AKyokaiCharacter::ApplyMoveInput(const float Value)
{
	MoveInput = FMath::Clamp(Value, -1.0f, 1.0f);
	if (FMath::Abs(MoveInput) <= KINDA_SMALL_NUMBER || bIsDashing)
	{
		return;
	}

	FacingDirection = FMath::Sign(MoveInput);
	SetActorRotation(FRotator(0.0f, FacingDirection > 0.0f ? 0.0f : 180.0f, 0.0f));
	AddMovementInput(FVector::XAxisVector, MoveInput);
}

void AKyokaiCharacter::OnJumpPressed()
{
	LastJumpPressedTime = GetWorld()->GetTimeSeconds();
	bJumpBuffered = true;

	if (bIsSliding)
	{
		OnSlideReleased();
	}

	TryConsumeJumpBuffer();
}

void AKyokaiCharacter::OnJumpReleased()
{
	StopJumping();
}

void AKyokaiCharacter::TryConsumeJumpBuffer()
{
	if (!bJumpBuffered || bIsDashing)
	{
		return;
	}

	const float TimeSinceInput = GetWorld()->GetTimeSeconds() - LastJumpPressedTime;
	if (TimeSinceInput > JumpBufferTime)
	{
		bJumpBuffered = false;
		return;
	}

	if (CanJump())
	{
		Jump();
		bJumpBuffered = false;
	}
}

void AKyokaiCharacter::OnSlidePressed()
{
	UKyokaiMovementComponent* Movement = GetKyokaiMovement();
	if (!Movement || !Movement->IsMovingOnGround() || bIsDashing || GetVelocity().Size2D() < MinimumSlideSpeed)
	{
		return;
	}

	bIsSliding = true;
	Movement->SetSliding(true);
	Crouch();
	Movement->Velocity.X = FacingDirection * FMath::Max(FMath::Abs(Movement->Velocity.X), SlideEntrySpeed);
}

void AKyokaiCharacter::OnSlideReleased()
{
	if (!bIsSliding)
	{
		return;
	}

	bIsSliding = false;
	if (UKyokaiMovementComponent* Movement = GetKyokaiMovement())
	{
		Movement->SetSliding(false);
	}
	UnCrouch();
}

void AKyokaiCharacter::OnDashPressed()
{
	UKyokaiMovementComponent* Movement = GetKyokaiMovement();
	if (!Movement || bIsDashing)
	{
		return;
	}

	const float Now = GetWorld()->GetTimeSeconds();
	const bool bCanAirDash = Movement->IsMovingOnGround() || bAirDashAvailable;
	if (!bCanAirDash || Now - LastDashTime < DashCooldown)
	{
		return;
	}

	OnSlideReleased();
	bIsDashing = true;
	LastDashTime = Now;
	if (!Movement->IsMovingOnGround())
	{
		bAirDashAvailable = false;
	}

	Movement->SetMovementMode(MOVE_Flying);
	Movement->Velocity = FVector(FacingDirection * DashSpeed, 0.0f, 0.0f);
	GetWorldTimerManager().SetTimer(DashTimerHandle, this, &AKyokaiCharacter::StopDash, DashDuration, false);
}

void AKyokaiCharacter::StopDash()
{
	if (!bIsDashing)
	{
		return;
	}

	bIsDashing = false;
	if (UKyokaiMovementComponent* Movement = GetKyokaiMovement())
	{
		Movement->SetMovementMode(MOVE_Falling);
		Movement->Velocity.X *= 0.75f;
	}
}

void AKyokaiCharacter::TogglePrototypeDebug()
{
	bShowPrototypeDebug = !bShowPrototypeDebug;
}

void AKyokaiCharacter::UpdateCameraLookAhead(const float DeltaSeconds)
{
	if (!CameraBoom)
	{
		return;
	}

	const float DesiredOffset = FacingDirection * CameraLookAheadDistance;
	FVector TargetOffset = CameraBoom->TargetOffset;
	TargetOffset.X = FMath::FInterpTo(TargetOffset.X, DesiredOffset, DeltaSeconds, CameraLookAheadSpeed);
	CameraBoom->TargetOffset = TargetOffset;
}

void AKyokaiCharacter::DrawPrototypeDebug() const
{
	if (!GEngine)
	{
		return;
	}

	const UKyokaiMovementComponent* Movement = GetKyokaiMovement();
	const FString MovementState = bIsDashing ? TEXT("DASH") : bIsSliding ? TEXT("SLIDE") :
		Movement && Movement->IsMovingOnGround() ? TEXT("GROUND") : TEXT("AIR");
	const FString DebugText = FString::Printf(
		TEXT("KYOKAI CONTROLLER | State: %s | Speed: %.0f | Z: %.0f | Input: %.2f | Air dash: %s"),
		*MovementState,
		GetVelocity().Size2D(),
		GetVelocity().Z,
		MoveInput,
		bAirDashAvailable ? TEXT("YES") : TEXT("NO"));

	GEngine->AddOnScreenDebugMessage(7318, 0.0f, FColor::Cyan, DebugText);
}
