// Copyright Epic Games, Inc. All Rights Reserved.

#include "Gameplay/WindGust.h"

#include "Characters/KyokaiCharacter.h"
#include "Characters/KyokaiMovementComponent.h"
#include "Components/BoxComponent.h"
#include "Components/StaticMeshComponent.h"
#include "Engine/StaticMesh.h"
#include "Materials/MaterialInstanceDynamic.h"
#include "UObject/ConstructorHelpers.h"

AWindGust::AWindGust()
{
	PrimaryActorTick.bCanEverTick = true;

	GustVolume = CreateDefaultSubobject<UBoxComponent>(TEXT("GustVolume"));
	RootComponent = GustVolume;
	GustVolume->SetBoxExtent(FVector(150.0f, 200.0f, 150.0f));
	GustVolume->SetCollisionEnabled(ECollisionEnabled::QueryOnly);
	GustVolume->SetCollisionResponseToAllChannels(ECR_Ignore);
	GustVolume->SetCollisionResponseToChannel(ECC_Pawn, ECR_Overlap);
	GustVolume->SetGenerateOverlapEvents(true);

	VisualMesh = CreateDefaultSubobject<UStaticMeshComponent>(TEXT("VisualMesh"));
	VisualMesh->SetupAttachment(GustVolume);
	VisualMesh->SetCollisionEnabled(ECollisionEnabled::NoCollision);
	VisualMesh->SetMobility(EComponentMobility::Movable);
	// Thin vertical slab spanning the volume's Y/Z, a fraction of its X
	// depth - a "curtain" marker, not a wall filling the whole volume.
	VisualMesh->SetRelativeScale3D(FVector(0.2f, 4.0f, 3.0f));

	static ConstructorHelpers::FObjectFinder<UStaticMesh> CubeFinder(TEXT("/Engine/BasicShapes/Cube.Cube"));
	if (CubeFinder.Succeeded())
	{
		VisualMesh->SetStaticMesh(CubeFinder.Object);
	}

	// ConstructorHelpers::FObjectFinder is only valid during CDO
	// construction, so the telegraph material is found and assigned here,
	// not in BeginPlay - BeginPlay only creates the dynamic instance from
	// whatever static material ended up on the mesh.
	static ConstructorHelpers::FObjectFinder<UMaterialInterface> TelegraphMatFinder(TEXT("/Game/Materials/M_HazardTelegraph.M_HazardTelegraph"));
	if (TelegraphMatFinder.Succeeded())
	{
		VisualMesh->SetMaterial(0, TelegraphMatFinder.Object);
	}
}

void AWindGust::BeginPlay()
{
	Super::BeginPlay();

	if (VisualMesh && VisualMesh->GetMaterial(0))
	{
		VisualMID = UMaterialInstanceDynamic::Create(VisualMesh->GetMaterial(0), this);
		VisualMesh->SetMaterial(0, VisualMID);
	}

	EnterState(EGustState::Cooldown);
}

void AWindGust::EnterState(const EGustState NewState)
{
	State = NewState;
	StateTimer = 0.0f;
	bIsTelegraphing = (NewState == EGustState::Telegraphing);
	bIsGusting = (NewState == EGustState::Gusting);
	UpdateVisualTint();
}

void AWindGust::UpdateVisualTint()
{
	if (!VisualMID)
	{
		return;
	}

	// Dim slate blue at rest, bright yellow warning through the telegraph
	// window, pale cyan-white while actually pushing.
	FLinearColor Tint = FLinearColor(0.2f, 0.3f, 0.4f);
	if (State == EGustState::Telegraphing)
	{
		Tint = FLinearColor(1.0f, 0.85f, 0.1f);
	}
	else if (State == EGustState::Gusting)
	{
		Tint = FLinearColor(0.7f, 0.9f, 1.0f);
	}
	VisualMID->SetVectorParameterValue(TEXT("TintColor"), Tint);
}

void AWindGust::Tick(const float DeltaTime)
{
	Super::Tick(DeltaTime);

	StateTimer += DeltaTime;

	switch (State)
	{
	case EGustState::Cooldown:
		if (StateTimer >= CooldownDuration)
		{
			EnterState(EGustState::Telegraphing);
		}
		break;

	case EGustState::Telegraphing:
		if (StateTimer >= TelegraphDuration)
		{
			EnterState(EGustState::Gusting);
		}
		break;

	case EGustState::Gusting:
		{
			TArray<AActor*> Overlapping;
			GustVolume->GetOverlappingActors(Overlapping, AKyokaiCharacter::StaticClass());
			for (AActor* Actor : Overlapping)
			{
				AKyokaiCharacter* Character = Cast<AKyokaiCharacter>(Actor);
				UKyokaiMovementComponent* Movement = Character ? Character->GetKyokaiMovement() : nullptr;
				if (Movement)
				{
					Movement->AddInputVector(FVector(GustInputStrengthX, 0.0f, 0.0f));
				}
			}

			if (StateTimer >= GustDuration)
			{
				EnterState(EGustState::Cooldown);
			}
		}
		break;
	}
}
