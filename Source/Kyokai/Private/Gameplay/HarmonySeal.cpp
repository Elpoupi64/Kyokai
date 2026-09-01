// Copyright Epic Games, Inc. All Rights Reserved.

#include "Gameplay/HarmonySeal.h"

#include "Characters/KyokaiCharacter.h"
#include "Components/SphereComponent.h"
#include "Components/StaticMeshComponent.h"
#include "Engine/StaticMesh.h"
#include "Game/KyokaiGameMode.h"
#include "Materials/MaterialInstanceDynamic.h"
#include "UObject/ConstructorHelpers.h"

namespace
{
	FString SealKindToString(const ESealKind Kind)
	{
		switch (Kind)
		{
		case ESealKind::Reading: return TEXT("reading");
		case ESealKind::Mastery: return TEXT("mastery");
		case ESealKind::Risk: return TEXT("risk");
		default: return TEXT("unknown");
		}
	}
}

AHarmonySeal::AHarmonySeal()
{
	PrimaryActorTick.bCanEverTick = false;

	VisualMesh = CreateDefaultSubobject<UStaticMeshComponent>(TEXT("VisualMesh"));
	RootComponent = VisualMesh;
	VisualMesh->SetCollisionEnabled(ECollisionEnabled::NoCollision);
	VisualMesh->SetMobility(EComponentMobility::Movable);
	VisualMesh->SetRelativeScale3D(FVector(0.35f, 0.35f, 0.5f));

	static ConstructorHelpers::FObjectFinder<UStaticMesh> ConeFinder(TEXT("/Engine/BasicShapes/Cone.Cone"));
	if (ConeFinder.Succeeded())
	{
		VisualMesh->SetStaticMesh(ConeFinder.Object);
	}

	// Same reused parametrized material as every other tinted actor this
	// level (TintColor vector param) - see AMemoryFragment for the same
	// pattern and the ConstructorHelpers-in-constructor-only reasoning.
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
	CollectTrigger->OnComponentBeginOverlap.AddDynamic(this, &AHarmonySeal::OnCollectOverlap);
}

void AHarmonySeal::BeginPlay()
{
	Super::BeginPlay();

	if (VisualMesh && VisualMesh->GetMaterial(0))
	{
		VisualMID = UMaterialInstanceDynamic::Create(VisualMesh->GetMaterial(0), this);
		VisualMesh->SetMaterial(0, VisualMID);
		// Cool violet - distinct from AMemoryFragment's warm amber, so a
		// player can tell "secret" from "story fragment" at a glance.
		VisualMID->SetVectorParameterValue(TEXT("TintColor"), FLinearColor(0.55f, 0.35f, 0.95f));
	}
}

void AHarmonySeal::OnCollectOverlap(UPrimitiveComponent* OverlappedComponent, AActor* OtherActor,
	UPrimitiveComponent* OtherComp, int32 OtherBodyIndex, bool bFromSweep, const FHitResult& SweepResult)
{
	if (bCollected || !Cast<AKyokaiCharacter>(OtherActor))
	{
		return;
	}
	bCollected = true;

	VisualMesh->SetVisibility(false);
	CollectTrigger->SetCollisionEnabled(ECollisionEnabled::NoCollision);

	if (AKyokaiGameMode* GameMode = GetWorld()->GetAuthGameMode<AKyokaiGameMode>())
	{
		GameMode->NotifySealCollected(SealId, SealKindToString(SealKind), GetActorLocation());
	}
}
