// Copyright Epic Games, Inc. All Rights Reserved.

#include "Gameplay/CopperRivet.h"

#include "Characters/KyokaiCharacter.h"
#include "Components/SphereComponent.h"
#include "Components/StaticMeshComponent.h"
#include "Engine/StaticMesh.h"
#include "Materials/MaterialInstanceDynamic.h"
#include "UObject/ConstructorHelpers.h"

ACopperRivet::ACopperRivet()
{
	PrimaryActorTick.bCanEverTick = false;

	VisualMesh = CreateDefaultSubobject<UStaticMeshComponent>(TEXT("VisualMesh"));
	RootComponent = VisualMesh;
	VisualMesh->SetCollisionEnabled(ECollisionEnabled::NoCollision);
	VisualMesh->SetMobility(EComponentMobility::Movable);
	// Small stud - a fraction the size of the other pickups (memory,
	// seals), since these are meant to read as a repeating line, not
	// individually notable landmarks.
	VisualMesh->SetRelativeScale3D(FVector(0.18f, 0.18f, 0.18f));

	static ConstructorHelpers::FObjectFinder<UStaticMesh> SphereFinder(TEXT("/Engine/BasicShapes/Sphere.Sphere"));
	if (SphereFinder.Succeeded())
	{
		VisualMesh->SetStaticMesh(SphereFinder.Object);
	}

	// Same reused parametrized material as every other tinted actor this
	// level - see AMemoryFragment/AHarmonySeal for the same pattern.
	static ConstructorHelpers::FObjectFinder<UMaterialInterface> TintMatFinder(TEXT("/Game/Materials/M_HazardTelegraph.M_HazardTelegraph"));
	if (TintMatFinder.Succeeded())
	{
		VisualMesh->SetMaterial(0, TintMatFinder.Object);
	}

	TriggerVolume = CreateDefaultSubobject<USphereComponent>(TEXT("TriggerVolume"));
	TriggerVolume->SetupAttachment(VisualMesh);
	// Without this, TriggerVolume inherits VisualMesh's own 0.18 relative
	// scale as a child component, silently shrinking the "90cm" radius
	// below to ~16cm in world space - confirmed via a temporary probe
	// that zero overlaps ever fired at the original size.
	TriggerVolume->SetUsingAbsoluteScale(true);
	TriggerVolume->SetSphereRadius(90.0f);
	TriggerVolume->SetCollisionEnabled(ECollisionEnabled::QueryOnly);
	TriggerVolume->SetCollisionResponseToAllChannels(ECR_Ignore);
	TriggerVolume->SetCollisionResponseToChannel(ECC_Pawn, ECR_Overlap);
	TriggerVolume->SetGenerateOverlapEvents(true);
	TriggerVolume->OnComponentBeginOverlap.AddDynamic(this, &ACopperRivet::OnTriggerOverlap);
}

void ACopperRivet::BeginPlay()
{
	Super::BeginPlay();

	if (VisualMesh && VisualMesh->GetMaterial(0))
	{
		VisualMID = UMaterialInstanceDynamic::Create(VisualMesh->GetMaterial(0), this);
		VisualMesh->SetMaterial(0, VisualMID);
		// Copper/bronze - distinct from every other pickup's color
		// (memory's warm amber, seals' cool violet).
		VisualMID->SetVectorParameterValue(TEXT("TintColor"), FLinearColor(0.72f, 0.45f, 0.2f));
	}
}

void ACopperRivet::OnTriggerOverlap(UPrimitiveComponent* OverlappedComponent, AActor* OtherActor,
	UPrimitiveComponent* OtherComp, int32 OtherBodyIndex, bool bFromSweep, const FHitResult& SweepResult)
{
	AKyokaiCharacter* Character = Cast<AKyokaiCharacter>(OtherActor);
	if (!Character)
	{
		return;
	}

	const float Now = GetWorld()->GetTimeSeconds();
	if (Now - LastTriggerTime < TriggerCooldown)
	{
		return;
	}
	LastTriggerTime = Now;

	Character->AddPression(PressionBonus);
}
