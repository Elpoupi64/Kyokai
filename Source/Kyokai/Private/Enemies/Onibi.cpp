// Copyright Epic Games, Inc. All Rights Reserved.

#include "Enemies/Onibi.h"

#include "Characters/KyokaiCharacter.h"
#include "Components/BoxComponent.h"
#include "Components/PointLightComponent.h"
#include "Components/StaticMeshComponent.h"
#include "Engine/StaticMesh.h"
#include "Materials/MaterialInstanceDynamic.h"
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

	static ConstructorHelpers::FObjectFinder<UMaterialInterface> TelegraphMatFinder(TEXT("/Game/Materials/M_HazardTelegraph.M_HazardTelegraph"));
	if (TelegraphMatFinder.Succeeded())
	{
		BodyMesh->SetMaterial(0, TelegraphMatFinder.Object);
	}

	GlowLight = CreateDefaultSubobject<UPointLightComponent>(TEXT("GlowLight"));
	GlowLight->SetupAttachment(BodyMesh);
	GlowLight->SetLightColor(FLinearColor(0.6f, 0.75f, 0.9f));
	GlowLight->SetIntensity(800.0f);
	GlowLight->SetAttenuationRadius(400.0f);
	GlowLight->SetCastShadows(false);

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

	if (BodyMesh && BodyMesh->GetMaterial(0))
	{
		BodyMID = UMaterialInstanceDynamic::Create(BodyMesh->GetMaterial(0), this);
		BodyMesh->SetMaterial(0, BodyMID);
	}

	EnterState(EOnibiState::Patrol);
}

void AOnibi::EnterState(const EOnibiState NewState)
{
	State = NewState;
	StateTimer = 0.0f;
	bIsTelegraphingCharge = (NewState == EOnibiState::Telegraphing);
	bIsCharging = (NewState == EOnibiState::Charging);
}

void AOnibi::UpdateGlow(const float DeltaTime)
{
	FlickerTime += DeltaTime;

	// Base color/intensity by state, then a flicker (two mismatched sine
	// frequencies, reads less mechanical than a single one) layered on top
	// - a will-o-wisp shouldn't glow perfectly steady.
	FLinearColor BaseColor(0.6f, 0.75f, 0.9f);
	float BaseIntensity = 800.0f;
	if (State == EOnibiState::Telegraphing)
	{
		BaseColor = FLinearColor(1.0f, 0.9f, 0.4f);
		BaseIntensity = 1400.0f;
	}
	else if (State == EOnibiState::Charging)
	{
		BaseColor = FLinearColor(1.0f, 0.6f, 0.2f);
		BaseIntensity = 2200.0f;
	}

	const float Flicker = 0.85f + 0.15f * (FMath::Sin(FlickerTime * 11.0f) * 0.6f + FMath::Sin(FlickerTime * 23.0f) * 0.4f);

	if (GlowLight)
	{
		GlowLight->SetLightColor(BaseColor);
		GlowLight->SetIntensity(BaseIntensity * Flicker);
	}
	if (BodyMID)
	{
		BodyMID->SetVectorParameterValue(TEXT("TintColor"), BaseColor * Flicker);
	}
}

void AOnibi::Tick(const float DeltaTime)
{
	Super::Tick(DeltaTime);

	StateTimer += DeltaTime;
	BobTime += DeltaTime;
	UpdateGlow(DeltaTime);

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
