// Copyright Epic Games, Inc. All Rights Reserved.

#include "Enemies/Onibi.h"

#include "Characters/KyokaiCharacter.h"
#include "Components/BoxComponent.h"
#include "Components/StaticMeshComponent.h"
#include "Engine/StaticMesh.h"
#include "UObject/ConstructorHelpers.h"

AOnibi::AOnibi()
{
	PrimaryActorTick.bCanEverTick = true;

	BodyMesh = CreateDefaultSubobject<UStaticMeshComponent>(TEXT("BodyMesh"));
	RootComponent = BodyMesh;
	BodyMesh->SetCollisionEnabled(ECollisionEnabled::NoCollision);
	BodyMesh->SetMobility(EComponentMobility::Movable);
	BodyMesh->SetWorldScale3D(FVector(0.7f));

	static ConstructorHelpers::FObjectFinder<UStaticMesh> SphereFinder(TEXT("/Engine/BasicShapes/Sphere.Sphere"));
	if (SphereFinder.Succeeded())
	{
		BodyMesh->SetStaticMesh(SphereFinder.Object);
	}

	HitBox = CreateDefaultSubobject<UBoxComponent>(TEXT("HitBox"));
	HitBox->SetupAttachment(BodyMesh);
	HitBox->SetBoxExtent(FVector(50.0f, 50.0f, 50.0f));
	HitBox->SetCollisionEnabled(ECollisionEnabled::QueryOnly);
	HitBox->SetCollisionResponseToAllChannels(ECR_Ignore);
	HitBox->SetCollisionResponseToChannel(ECC_Pawn, ECR_Overlap);
	HitBox->SetGenerateOverlapEvents(true);
	HitBox->OnComponentBeginOverlap.AddDynamic(this, &AOnibi::OnHitBoxOverlap);
}

void AOnibi::BeginPlay()
{
	Super::BeginPlay();
	HomeLocation = GetActorLocation();
	EnterState(EOnibiState::Patrol);
}

void AOnibi::EnterState(const EOnibiState NewState)
{
	State = NewState;
	StateTimer = 0.0f;
	bIsTelegraphingCharge = (NewState == EOnibiState::Telegraphing);
	bIsCharging = (NewState == EOnibiState::Charging);
}

void AOnibi::Tick(const float DeltaTime)
{
	Super::Tick(DeltaTime);

	StateTimer += DeltaTime;
	BobTime += DeltaTime;

	const FVector CurrentLocation = GetActorLocation();
	const float Bob = FMath::Sin(BobTime * BobSpeed) * BobAmplitude;

	switch (State)
	{
	case EOnibiState::Patrol:
		{
			SetActorLocation(FVector(HomeLocation.X, HomeLocation.Y, HomeLocation.Z + Bob));

			if (const AKyokaiCharacter* Player = FindPlayerCharacter())
			{
				const FVector PlayerLoc = Player->GetActorLocation();
				const bool bWithinX = FMath::Abs(PlayerLoc.X - HomeLocation.X) <= DetectionRadiusX;
				const bool bWithinZ = FMath::Abs(PlayerLoc.Z - HomeLocation.Z) <= DetectionRangeZ;
				if (bWithinX && bWithinZ)
				{
					ChargeDirectionSign = (PlayerLoc.X >= HomeLocation.X) ? 1.0f : -1.0f;
					EnterState(EOnibiState::Telegraphing);
				}
			}
		}
		break;

	case EOnibiState::Telegraphing:
		{
			// Faster bob while winding up - a cheap graybox telegraph cue
			// (a real one is an art-pass concern; bIsTelegraphingCharge is
			// exposed for that later).
			const float FastBob = FMath::Sin(BobTime * BobSpeed * 4.0f) * (BobAmplitude * 0.5f);
			SetActorLocation(FVector(HomeLocation.X, HomeLocation.Y, HomeLocation.Z + FastBob));

			if (StateTimer >= TelegraphDuration)
			{
				EnterState(EOnibiState::Charging);
			}
		}
		break;

	case EOnibiState::Charging:
		{
			const FVector NewLocation = CurrentLocation + FVector(ChargeDirectionSign * ChargeSpeed * DeltaTime, 0.0f, 0.0f);
			SetActorLocation(NewLocation);

			if (StateTimer >= ChargeDuration)
			{
				EnterState(EOnibiState::Cooldown);
			}
		}
		break;

	case EOnibiState::Cooldown:
		{
			// Drift back toward home so the next charge starts from a
			// consistent position rather than wherever the last one ended.
			const FVector Target(HomeLocation.X, HomeLocation.Y, HomeLocation.Z + Bob);
			SetActorLocation(FMath::VInterpTo(CurrentLocation, Target, DeltaTime, 2.0f));

			if (StateTimer >= AttackCooldown)
			{
				EnterState(EOnibiState::Patrol);
			}
		}
		break;
	}
}

void AOnibi::OnHitBoxOverlap(UPrimitiveComponent* OverlappedComponent, AActor* OtherActor,
	UPrimitiveComponent* OtherComp, int32 OtherBodyIndex, bool bFromSweep, const FHitResult& SweepResult)
{
	if (State != EOnibiState::Charging)
	{
		return;
	}

	AKyokaiCharacter* Character = Cast<AKyokaiCharacter>(OtherActor);
	if (!Character)
	{
		return;
	}

	ApplyContactConsequence(Character, TEXT("onibi"));
	EnterState(EOnibiState::Cooldown);
}
