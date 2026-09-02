// Copyright Epic Games, Inc. All Rights Reserved.

#include "Gameplay/MemoryFragment.h"

#include "Characters/KyokaiCharacter.h"
#include "Components/SphereComponent.h"
#include "Components/StaticMeshComponent.h"
#include "Engine/StaticMesh.h"
#include "Game/KyokaiGameMode.h"
#include "Materials/MaterialInstanceDynamic.h"
#include "UObject/ConstructorHelpers.h"

AMemoryFragment::AMemoryFragment()
{
	PrimaryActorTick.bCanEverTick = false;

	VisualMesh = CreateDefaultSubobject<UStaticMeshComponent>(TEXT("VisualMesh"));
	RootComponent = VisualMesh;
	VisualMesh->SetCollisionEnabled(ECollisionEnabled::NoCollision);
	VisualMesh->SetMobility(EComponentMobility::Movable);
	VisualMesh->SetRelativeScale3D(FVector(0.4f, 0.4f, 0.4f));

	static ConstructorHelpers::FObjectFinder<UStaticMesh> SphereFinder(TEXT("/Engine/BasicShapes/Sphere.Sphere"));
	if (SphereFinder.Succeeded())
	{
		VisualMesh->SetStaticMesh(SphereFinder.Object);
	}

	// Reuses the same parametrized material as every hazard telegraph
	// (TintColor vector param) rather than a bespoke asset - this pickup
	// never changes state, so it's just set once in BeginPlay to a warm
	// amber, deliberately distinct from every hazard tone in this level
	// (slate blue/yellow/cyan for wind, purple/orange/white for lightning,
	// blue/yellow/orange for Onibi) so it reads as "friendly, not a
	// warning" at a glance. ConstructorHelpers::FObjectFinder is only
	// valid during CDO construction, so the material is found and
	// assigned here - BeginPlay only creates the dynamic instance from it.
	static ConstructorHelpers::FObjectFinder<UMaterialInterface> TintMatFinder(TEXT("/Game/Materials/M_HazardTelegraph.M_HazardTelegraph"));
	if (TintMatFinder.Succeeded())
	{
		VisualMesh->SetMaterial(0, TintMatFinder.Object);
	}

	CollectTrigger = CreateDefaultSubobject<USphereComponent>(TEXT("CollectTrigger"));
	CollectTrigger->SetupAttachment(VisualMesh);
	CollectTrigger->SetSphereRadius(120.0f);
	CollectTrigger->SetCollisionEnabled(ECollisionEnabled::QueryOnly);
	CollectTrigger->SetCollisionResponseToAllChannels(ECR_Ignore);
	CollectTrigger->SetCollisionResponseToChannel(ECC_Pawn, ECR_Overlap);
	CollectTrigger->SetGenerateOverlapEvents(true);
	CollectTrigger->OnComponentBeginOverlap.AddDynamic(this, &AMemoryFragment::OnCollectOverlap);
}

void AMemoryFragment::BeginPlay()
{
	Super::BeginPlay();

	if (VisualMesh && VisualMesh->GetMaterial(0))
	{
		VisualMID = UMaterialInstanceDynamic::Create(VisualMesh->GetMaterial(0), this);
		VisualMesh->SetMaterial(0, VisualMID);
		VisualMID->SetVectorParameterValue(TEXT("TintColor"), FLinearColor(1.0f, 0.75f, 0.3f));
	}
}

void AMemoryFragment::OnCollectOverlap(UPrimitiveComponent* OverlappedComponent, AActor* OtherActor,
	UPrimitiveComponent* OtherComp, int32 OtherBodyIndex, bool bFromSweep, const FHitResult& SweepResult)
{
	AKyokaiCharacter* Character = Cast<AKyokaiCharacter>(OtherActor);
	if (bCollected || !Character)
	{
		return;
	}
	bCollected = true;

	VisualMesh->SetVisibility(false);
	CollectTrigger->SetCollisionEnabled(ECollisionEnabled::NoCollision);

	// Pression: "in-line collection" is one of the GDD's own listed fill
	// sources - collecting without stopping keeps the flow going.
	Character->AddPression(Character->PressionPickupBonus);

	if (AKyokaiGameMode* GameMode = GetWorld()->GetAuthGameMode<AKyokaiGameMode>())
	{
		GameMode->NotifyMemoryCollected(FragmentId, FragmentText, GetActorLocation());
	}
}
