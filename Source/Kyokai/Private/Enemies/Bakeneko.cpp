// Copyright Epic Games, Inc. All Rights Reserved.

#include "Enemies/Bakeneko.h"

#include "Characters/KyokaiCharacter.h"
#include "Components/BoxComponent.h"
#include "Components/StaticMeshComponent.h"
#include "Engine/StaticMesh.h"
#include "Materials/MaterialInstanceDynamic.h"
#include "UObject/ConstructorHelpers.h"

ABakeneko::ABakeneko()
{
	PrimaryActorTick.bCanEverTick = true;

	BodyMesh = CreateDefaultSubobject<UStaticMeshComponent>(TEXT("BodyMesh"));
	RootComponent = BodyMesh;
	BodyMesh->SetCollisionEnabled(ECollisionEnabled::NoCollision);
	BodyMesh->SetMobility(EComponentMobility::Movable);
	BodyMesh->SetWorldScale3D(FVector(0.8f, 0.6f, 0.5f));

	static ConstructorHelpers::FObjectFinder<UStaticMesh> CubeFinder(TEXT("/Engine/BasicShapes/Cube.Cube"));
	static ConstructorHelpers::FObjectFinder<UStaticMesh> SphereFinder(TEXT("/Engine/BasicShapes/Sphere.Sphere"));
	if (CubeFinder.Succeeded())
	{
		BodyMesh->SetStaticMesh(CubeFinder.Object);
	}

	// Absolute scale/rotation on the head+eyes so they don't compound with
	// BodyMesh's own (0.8,0.6,0.5) squash - want a readable head-sized
	// silhouette, not a shape scaled by the body's own proportions.
	HeadMesh = CreateDefaultSubobject<UStaticMeshComponent>(TEXT("HeadMesh"));
	HeadMesh->SetupAttachment(BodyMesh);
	HeadMesh->SetUsingAbsoluteScale(true);
	HeadMesh->SetCollisionEnabled(ECollisionEnabled::NoCollision);
	HeadMesh->SetMobility(EComponentMobility::Movable);
	HeadMesh->SetRelativeLocation(FVector(62.0f, 0.0f, 40.0f));
	HeadMesh->SetRelativeScale3D(FVector(0.32f));
	if (CubeFinder.Succeeded())
	{
		HeadMesh->SetStaticMesh(CubeFinder.Object);
	}

	EyeLeft = CreateDefaultSubobject<UStaticMeshComponent>(TEXT("EyeLeft"));
	EyeLeft->SetupAttachment(HeadMesh);
	EyeLeft->SetUsingAbsoluteScale(true);
	EyeLeft->SetCollisionEnabled(ECollisionEnabled::NoCollision);
	EyeLeft->SetMobility(EComponentMobility::Movable);
	EyeLeft->SetRelativeLocation(FVector(30.0f, 20.0f, 5.0f));
	EyeLeft->SetRelativeScale3D(FVector(0.08f));

	EyeRight = CreateDefaultSubobject<UStaticMeshComponent>(TEXT("EyeRight"));
	EyeRight->SetupAttachment(HeadMesh);
	EyeRight->SetUsingAbsoluteScale(true);
	EyeRight->SetCollisionEnabled(ECollisionEnabled::NoCollision);
	EyeRight->SetMobility(EComponentMobility::Movable);
	EyeRight->SetRelativeLocation(FVector(30.0f, -20.0f, 5.0f));
	EyeRight->SetRelativeScale3D(FVector(0.08f));

	static ConstructorHelpers::FObjectFinder<UMaterialInterface> TelegraphMatFinder(TEXT("/Game/Materials/M_HazardTelegraph.M_HazardTelegraph"));
	if (SphereFinder.Succeeded() && TelegraphMatFinder.Succeeded())
	{
		EyeLeft->SetStaticMesh(SphereFinder.Object);
		EyeRight->SetStaticMesh(SphereFinder.Object);
		EyeLeft->SetMaterial(0, TelegraphMatFinder.Object);
		EyeRight->SetMaterial(0, TelegraphMatFinder.Object);
	}

	HitBox = CreateDefaultSubobject<UBoxComponent>(TEXT("HitBox"));
	HitBox->SetupAttachment(BodyMesh);
	HitBox->SetBoxExtent(FVector(60.0f, 60.0f, 60.0f));
	HitBox->SetCollisionEnabled(ECollisionEnabled::QueryOnly);
	HitBox->SetCollisionResponseToAllChannels(ECR_Ignore);
	HitBox->SetCollisionResponseToChannel(ECC_Pawn, ECR_Overlap);
	HitBox->SetGenerateOverlapEvents(true);
	HitBox->OnComponentBeginOverlap.AddDynamic(this, &ABakeneko::OnHitBoxOverlap);
}

void ABakeneko::BeginPlay()
{
	Super::BeginPlay();
	HomeLocation = GetActorLocation();

	if (EyeLeft && EyeLeft->GetMaterial(0))
	{
		EyeMID = UMaterialInstanceDynamic::Create(EyeLeft->GetMaterial(0), this);
		EyeLeft->SetMaterial(0, EyeMID);
		EyeRight->SetMaterial(0, EyeMID);
	}

	EnterState(EBakenekoState::Idle);
	UpdateEyeGlow();
}

void ABakeneko::EnterState(const EBakenekoState NewState)
{
	State = NewState;
	StateTimer = 0.0f;
	bIsChasing = (NewState == EBakenekoState::Chasing);
	bIsTelegraphingPounce = (NewState == EBakenekoState::Telegraphing);
	bIsPouncing = (NewState == EBakenekoState::Pouncing);
	UpdateEyeGlow();
}

void ABakeneko::UpdateEyeGlow()
{
	if (!EyeMID)
	{
		return;
	}

	// Dim amber at idle/chase - just enough to read as eyes in the dark -
	// bright yellow-white through the telegraph, the actual warning cue.
	const FLinearColor Tint = (State == EBakenekoState::Telegraphing || State == EBakenekoState::Pouncing)
		? FLinearColor(3.0f, 2.6f, 0.8f)
		: FLinearColor(0.8f, 0.65f, 0.1f);
	EyeMID->SetVectorParameterValue(TEXT("TintColor"), Tint);
}

void ABakeneko::UpdateFacing(const float MoveSignX)
{
	if (FMath::IsNearlyZero(MoveSignX))
	{
		return;
	}
	SetActorRotation(FRotator(0.0f, MoveSignX > 0.0f ? 0.0f : 180.0f, 0.0f));
}

void ABakeneko::Tick(const float DeltaTime)
{
	Super::Tick(DeltaTime);

	StateTimer += DeltaTime;

	const FVector CurrentLocation = GetActorLocation();
	const AKyokaiCharacter* Player = FindPlayerCharacter();
	const float PlayerX = Player ? Player->GetActorLocation().X : CurrentLocation.X;

	switch (State)
	{
	case EBakenekoState::Idle:
		{
			if (Player && FMath::Abs(PlayerX - HomeLocation.X) <= DetectionRangeX)
			{
				EnterState(EBakenekoState::Chasing);
			}
		}
		break;

	case EBakenekoState::Chasing:
		{
			if (!Player || FMath::Abs(PlayerX - HomeLocation.X) > LeashRangeX)
			{
				EnterState(EBakenekoState::Cooldown);
				break;
			}

			const float DistanceToPlayer = PlayerX - CurrentLocation.X;
			if (FMath::Abs(DistanceToPlayer) <= PounceRangeX)
			{
				EnterState(EBakenekoState::Telegraphing);
				break;
			}

			const float MoveSign = FMath::Sign(DistanceToPlayer);
			UpdateFacing(MoveSign);
			SetActorLocation(CurrentLocation + FVector(MoveSign * ChaseSpeed * DeltaTime, 0.0f, 0.0f));
		}
		break;

	case EBakenekoState::Telegraphing:
		if (StateTimer >= TelegraphDuration)
		{
			// Aim at the player's CURRENT position, not where they were
			// when the telegraph started - Bakeneko holds still for the
			// whole telegraph while a running player covers real ground
			// (up to ~425cm at 850cm/s over a 0.5s telegraph), so locking
			// the direction in early sent pounces at a stale, often-passed
			// position (found by testing: velocity_x never dipped crossing
			// this zone, meaning every pounce this way had been missing).
			const float AimDistance = PlayerX - CurrentLocation.X;
			PounceDirectionSign = (AimDistance >= 0.0f) ? 1.0f : -1.0f;
			UpdateFacing(PounceDirectionSign);
			EnterState(EBakenekoState::Pouncing);
		}
		break;

	case EBakenekoState::Pouncing:
		{
			const FVector NewLocation = CurrentLocation + FVector(PounceDirectionSign * PounceSpeed * DeltaTime, 0.0f, 0.0f);
			SetActorLocation(NewLocation);

			if (StateTimer >= PounceDuration)
			{
				EnterState(EBakenekoState::Cooldown);
			}
		}
		break;

	case EBakenekoState::Cooldown:
		{
			// Walk back toward home at chase pace rather than teleporting,
			// so re-engaging reads as continuous rather than a reset.
			const float MoveSign = FMath::Sign(HomeLocation.X - CurrentLocation.X);
			if (FMath::Abs(HomeLocation.X - CurrentLocation.X) > 5.0f)
			{
				UpdateFacing(MoveSign);
				SetActorLocation(CurrentLocation + FVector(MoveSign * ChaseSpeed * DeltaTime, 0.0f, 0.0f));
			}

			if (StateTimer >= PounceCooldown)
			{
				EnterState(EBakenekoState::Idle);
			}
		}
		break;
	}
}

void ABakeneko::OnHitBoxOverlap(UPrimitiveComponent* OverlappedComponent, AActor* OtherActor,
	UPrimitiveComponent* OtherComp, int32 OtherBodyIndex, bool bFromSweep, const FHitResult& SweepResult)
{
	if (State != EBakenekoState::Pouncing)
	{
		return;
	}

	AKyokaiCharacter* Character = Cast<AKyokaiCharacter>(OtherActor);
	if (!Character)
	{
		return;
	}

	ApplyContactConsequence(Character, TEXT("bakeneko"));
	EnterState(EBakenekoState::Cooldown);
}
