// Copyright Epic Games, Inc. All Rights Reserved.

#include "Gameplay/LightningStrike.h"

#include "Characters/KyokaiCharacter.h"
#include "Components/BoxComponent.h"
#include "Components/StaticMeshComponent.h"
#include "Engine/StaticMesh.h"
#include "Materials/MaterialInstanceDynamic.h"
#include "UObject/ConstructorHelpers.h"

ALightningStrike::ALightningStrike()
{
	PrimaryActorTick.bCanEverTick = true;

	StrikeVolume = CreateDefaultSubobject<UBoxComponent>(TEXT("StrikeVolume"));
	RootComponent = StrikeVolume;
	StrikeVolume->SetBoxExtent(FVector(150.0f, 200.0f, 200.0f));
	StrikeVolume->SetCollisionEnabled(ECollisionEnabled::QueryOnly);
	StrikeVolume->SetCollisionResponseToAllChannels(ECR_Ignore);
	StrikeVolume->SetCollisionResponseToChannel(ECC_Pawn, ECR_Overlap);
	StrikeVolume->SetGenerateOverlapEvents(true);

	VisualMesh = CreateDefaultSubobject<UStaticMeshComponent>(TEXT("VisualMesh"));
	VisualMesh->SetupAttachment(StrikeVolume);
	VisualMesh->SetCollisionEnabled(ECollisionEnabled::NoCollision);
	VisualMesh->SetMobility(EComponentMobility::Movable);
	// Thin vertical column standing in for a bolt/rod marker - a fraction
	// of the strike volume's footprint, not filling it.
	VisualMesh->SetRelativeScale3D(FVector(0.3f, 0.3f, 4.0f));

	static ConstructorHelpers::FObjectFinder<UStaticMesh> CylinderFinder(TEXT("/Engine/BasicShapes/Cylinder.Cylinder"));
	if (CylinderFinder.Succeeded())
	{
		VisualMesh->SetStaticMesh(CylinderFinder.Object);
	}

	// See AWindGust's constructor comment - FObjectFinder only works
	// during CDO construction, so the material is assigned here and the
	// dynamic instance is created from it later in BeginPlay.
	static ConstructorHelpers::FObjectFinder<UMaterialInterface> TelegraphMatFinder(TEXT("/Game/Materials/M_HazardTelegraph.M_HazardTelegraph"));
	if (TelegraphMatFinder.Succeeded())
	{
		VisualMesh->SetMaterial(0, TelegraphMatFinder.Object);
	}
}

void ALightningStrike::BeginPlay()
{
	Super::BeginPlay();

	if (VisualMesh && VisualMesh->GetMaterial(0))
	{
		VisualMID = UMaterialInstanceDynamic::Create(VisualMesh->GetMaterial(0), this);
		VisualMesh->SetMaterial(0, VisualMID);
	}

	EnterState(EStrikeState::Cooldown);
}

void ALightningStrike::EnterState(const EStrikeState NewState)
{
	State = NewState;
	StateTimer = 0.0f;
	bIsTelegraphing = (NewState == EStrikeState::Telegraphing);
	UpdateVisualTint();
}

void ALightningStrike::UpdateVisualTint()
{
	// The strike flash (set directly in Tick, not through this function)
	// takes priority while active - EnterState() calls this unconditionally,
	// which would otherwise stomp the flash the instant Cooldown is entered.
	if (!VisualMID || FlashTimer > 0.0f)
	{
		return;
	}

	// Dormant dark purple at rest, bright orange-red through the telegraph
	// (the actual warning window), a brief white flash at the strike
	// instant handled separately in Tick via FlashTimer.
	const FLinearColor Tint = (State == EStrikeState::Telegraphing)
		? FLinearColor(1.0f, 0.3f, 0.05f)
		: FLinearColor(0.15f, 0.05f, 0.2f);
	VisualMID->SetVectorParameterValue(TEXT("TintColor"), Tint);
}

void ALightningStrike::Tick(const float DeltaTime)
{
	Super::Tick(DeltaTime);

	StateTimer += DeltaTime;

	if (FlashTimer > 0.0f)
	{
		FlashTimer -= DeltaTime;
		if (FlashTimer <= 0.0f && VisualMID)
		{
			UpdateVisualTint();
		}
	}

	switch (State)
	{
	case EStrikeState::Cooldown:
		if (StateTimer >= CooldownDuration)
		{
			EnterState(EStrikeState::Telegraphing);
		}
		break;

	case EStrikeState::Telegraphing:
		if (StateTimer >= TelegraphDuration)
		{
			ExecuteStrike();
			if (VisualMID)
			{
				VisualMID->SetVectorParameterValue(TEXT("TintColor"), FLinearColor(3.0f, 3.0f, 3.5f));
				FlashTimer = 0.15f;
			}
			EnterState(EStrikeState::Cooldown);
		}
		break;
	}
}

void ALightningStrike::ExecuteStrike()
{
	TArray<AActor*> Overlapping;
	StrikeVolume->GetOverlappingActors(Overlapping, AKyokaiCharacter::StaticClass());
	for (AActor* Actor : Overlapping)
	{
		if (AKyokaiCharacter* Character = Cast<AKyokaiCharacter>(Actor))
		{
			// Real integrity system now (see AKyokaiCharacter::ApplyHazardHit)
			// - most strikes cost 1 segment and knock the character back-and-
			// up rather than a full checkpoint reset; ApplyHazardHit() already
			// handles the full reset itself once segments reach 0.
			if (Character->ApplyHazardHit(TEXT("lightning")))
			{
				// Fixed back-and-up direction (not facing-relative - unlike
				// Bakeneko/Onibi, AKyokaiCharacter's own yaw isn't reliably
				// tied to travel direction) - -X matches this level's near-
				// universal forward-progress direction, so it reads as a
				// real setback rather than an arbitrary shove.
				Character->LaunchCharacter(FVector(-450.0f, 0.0f, 450.0f), true, true);
			}
		}
	}
}
