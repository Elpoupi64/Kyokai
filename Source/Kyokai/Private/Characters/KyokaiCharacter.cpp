// Copyright Epic Games, Inc. All Rights Reserved.

#include "Characters/KyokaiCharacter.h"

#include "Camera/CameraComponent.h"
#include "Characters/KyokaiMovementComponent.h"
#include "CollisionQueryParams.h"
#include "Components/CapsuleComponent.h"
#include "Components/StaticMeshComponent.h"
#include "Engine/Engine.h"
#include "Engine/StaticMesh.h"
#include "UObject/ConstructorHelpers.h"
#include "Engine/LocalPlayer.h"
#include "EnhancedInputComponent.h"
#include "EnhancedInputSubsystems.h"
#include "Game/KyokaiGameMode.h"
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
	CameraBoom->TargetOffset = FVector(0.0f, 0.0f, CameraGroundedVerticalOffset);
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
	LastSafeGroundLocation = GetActorLocation();
	CurrentIntegritySegments = MaxIntegritySegments;
	CurrentPression = MaxPressionCharges;

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

void AKyokaiCharacter::RespawnAtCheckpoint(const FString& Cause)
{
	if (AKyokaiGameMode* GameMode = GetWorld()->GetAuthGameMode<AKyokaiGameMode>())
	{
		GameMode->NotifyPlayerDeath(Cause, GetActorLocation());
		SetActorLocation(GameMode->GetRespawnLocation(), false, nullptr, ETeleportType::TeleportPhysics);
		if (UKyokaiMovementComponent* Movement = GetKyokaiMovement())
		{
			Movement->StopMovementImmediately();
		}
		// "instant respawn at 0 HP... no lives system" - a full checkpoint
		// reset always comes with a full integrity refill, whether this
		// call came from ApplyHazardHit() reaching 0 or a direct call
		// (there are none left as of this system's introduction, but
		// RespawnAtCheckpoint() stays public/callable on its own).
		CurrentIntegritySegments = MaxIntegritySegments;
	}
}

bool AKyokaiCharacter::ApplyHazardHit(const FString& Cause)
{
	if (IFrameTimer > 0.0f)
	{
		return false;
	}
	IFrameTimer = IFrameDuration;
	--CurrentIntegritySegments;

	if (AKyokaiGameMode* GameMode = GetWorld()->GetAuthGameMode<AKyokaiGameMode>())
	{
		GameMode->NotifyIntegrityLost(Cause, CurrentIntegritySegments, GetActorLocation());
	}

	if (CurrentIntegritySegments <= 0)
	{
		RespawnAtCheckpoint(Cause);
		return false;
	}
	return true;
}

void AKyokaiCharacter::AddPression(const float Amount)
{
	CurrentPression = FMath::Clamp(CurrentPression + Amount, 0.0f, MaxPressionCharges);
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
		LastSafeGroundLocation = GetActorLocation();
	}
	else if (bIsSliding)
	{
		OnSlideReleased();
	}

	if (IFrameTimer > 0.0f)
	{
		IFrameTimer = FMath::Max(0.0f, IFrameTimer - DeltaSeconds);
	}

	// Pression: "continuous movement" fills it (deadzone avoids tiny
	// residual velocities - e.g. right as a dash decays - counting as
	// real movement), otherwise the idle timer runs and, past the
	// decay delay, drains it. bIsDashing is excluded from the moving
	// check on purpose: the dash consumes its own Pression cost up
	// front, so it shouldn't also refill from its own motion.
	const bool bPressionIsMoving = !bIsDashing && FMath::Abs(GetVelocity().X) > 50.0f;
	if (bPressionIsMoving)
	{
		PressionIdleTimer = 0.0f;
		CurrentPression = FMath::Min(MaxPressionCharges, CurrentPression + PressionMoveFillRate * DeltaSeconds);
	}
	else
	{
		PressionIdleTimer += DeltaSeconds;
		if (PressionIdleTimer > PressionIdleDecayDelay)
		{
			CurrentPression = FMath::Max(0.0f, CurrentPression - PressionIdleDecayRate * DeltaSeconds);
		}
	}

	TryConsumeJumpBuffer();
	UpdateCameraLookAhead(DeltaSeconds);
	UpdateWallDetection();

	// Fall-catch: -600 sits well below the lowest real platform in Level 02
	// (top=0, the lowest of any Segment 1/2 platform - confirmed by
	// querying every StaticMeshActor's Z), so this only fires once the
	// player has genuinely fallen off the world, not during a legitimate
	// designed drop. Goes through ApplyHazardHit() like every other hazard
	// now - a fall costs 1 integrity segment and recovers to the last
	// grounded spot (GDD: "falls send you back to the last safe ground,
	// not a full restart") rather than always teleporting to the
	// checkpoint; only reaching 0 segments does that (handled inside
	// ApplyHazardHit() itself, which is why the false-return case here
	// needs no further action).
	if (GetActorLocation().Z < -600.0f)
	{
		if (ApplyHazardHit(TEXT("fall")))
		{
			SetActorLocation(LastSafeGroundLocation, false, nullptr, ETeleportType::TeleportPhysics);
			if (UKyokaiMovementComponent* FallMovement = GetKyokaiMovement())
			{
				FallMovement->StopMovementImmediately();
			}
		}
	}

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
	const UKyokaiMovementComponent* Movement = GetKyokaiMovement();
	if (Movement && Movement->IsMovingOnGround())
	{
		return Super::CanJumpInternal_Implementation();
	}

	if (!Movement || !Movement->IsFalling())
	{
		return false;
	}

	// Deliberately NOT going through Super() (or checking JumpCurrentCount)
	// here: ACharacter's own "first jump while already falling, with
	// JumpCurrentCount==0 < JumpMaxCount" special case looks like exactly
	// what coyote time wants, but it isn't reliable - UCharacterMovementComponent
	// re-checks CanJump() a second time inside DoJump() itself, by which
	// point CheckJumpInput() has already bumped JumpCurrentCount to 1, so a
	// JumpCurrentCount-gated check here flips to false between the two
	// calls: the jump attempt gets "consumed" (counter incremented) but
	// DoJump() bails before ever touching Velocity.Z, so nothing actually
	// happens. Gating purely on elapsed time since grounded sidesteps this
	// entirely, since that stays true across both calls within the same
	// frame.
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
	else if (bIsTouchingWall)
	{
		PerformWallJump();
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
	if (!bCanAirDash || Now - LastDashTime < DashCooldown || CurrentPression < PressionDashCost)
	{
		return;
	}

	CurrentPression -= PressionDashCost;
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

void AKyokaiCharacter::UpdateWallDetection()
{
	bIsTouchingWall = false;
	WallPushDirection = 0.0f;

	const UKyokaiMovementComponent* Movement = GetKyokaiMovement();
	if (!Movement || Movement->IsMovingOnGround() || bIsDashing)
	{
		return;
	}

	const FVector Origin = GetActorLocation();
	const float TraceLength = GetCapsuleComponent()->GetScaledCapsuleRadius() + WallCheckDistance;
	FCollisionQueryParams QueryParams(SCENE_QUERY_STAT(WallJumpCheck), false, this);

	for (const float Direction : {-1.0f, 1.0f})
	{
		FHitResult Hit;
		const FVector End = Origin + FVector(Direction * TraceLength, 0.0f, 0.0f);
		if (GetWorld()->LineTraceSingleByChannel(Hit, Origin, End, ECC_Visibility, QueryParams))
		{
			bIsTouchingWall = true;
			WallPushDirection = -Direction; // launch away from whichever side the wall is on
			break;
		}
	}
}

void AKyokaiCharacter::PerformWallJump()
{
	UKyokaiMovementComponent* Movement = GetKyokaiMovement();
	if (!Movement || FMath::IsNearlyZero(WallPushDirection))
	{
		return;
	}

	FacingDirection = WallPushDirection;
	SetActorRotation(FRotator(0.0f, FacingDirection > 0.0f ? 0.0f : 180.0f, 0.0f));

	Movement->SetMovementMode(MOVE_Falling);
	Movement->Velocity = FVector(WallPushDirection * WallJumpHorizontalVelocity, 0.0f, WallJumpZVelocity);

	// Detach immediately so the very next tick's trace (still close to the
	// wall we just pushed off) doesn't re-trigger the same wall jump.
	bIsTouchingWall = false;
	WallPushDirection = 0.0f;
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

	// Drop the camera's vertical focus while airborne so platforms below
	// stay in frame during a jump instead of scrolling off the bottom -
	// see CameraAirborneVerticalDrop's own comment for why this isn't
	// just polish.
	const UKyokaiMovementComponent* Movement = GetKyokaiMovement();
	const bool bAirborne = Movement && !Movement->IsMovingOnGround();
	const float DesiredOffsetZ = bAirborne
		? CameraGroundedVerticalOffset - CameraAirborneVerticalDrop
		: CameraGroundedVerticalOffset;
	TargetOffset.Z = FMath::FInterpTo(TargetOffset.Z, DesiredOffsetZ, DeltaSeconds, CameraVerticalInterpSpeed);

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
		bIsTouchingWall ? TEXT("WALL") :
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
