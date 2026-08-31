// Copyright Epic Games, Inc. All Rights Reserved.

#include "Gameplay/Checkpoint.h"

#include "Characters/KyokaiCharacter.h"
#include "Components/BoxComponent.h"
#include "Components/StaticMeshComponent.h"
#include "Engine/StaticMesh.h"
#include "Game/KyokaiGameMode.h"
#include "UObject/ConstructorHelpers.h"

ACheckpoint::ACheckpoint()
{
	PrimaryActorTick.bCanEverTick = false;

	Marker = CreateDefaultSubobject<UStaticMeshComponent>(TEXT("Marker"));
	RootComponent = Marker;
	Marker->SetCollisionEnabled(ECollisionEnabled::NoCollision);
	Marker->SetMobility(EComponentMobility::Static);
	Marker->SetWorldScale3D(FVector(0.3f, 2.0f, 4.0f));

	static ConstructorHelpers::FObjectFinder<UStaticMesh> CubeFinder(TEXT("/Engine/BasicShapes/Cube.Cube"));
	if (CubeFinder.Succeeded())
	{
		Marker->SetStaticMesh(CubeFinder.Object);
	}

	// Tall and wide enough to catch the player crossing at running speed
	// without needing to jump/duck for it specifically.
	ActivationTrigger = CreateDefaultSubobject<UBoxComponent>(TEXT("ActivationTrigger"));
	ActivationTrigger->SetupAttachment(Marker);
	ActivationTrigger->SetBoxExtent(FVector(60.0f, 200.0f, 250.0f));
	ActivationTrigger->SetCollisionEnabled(ECollisionEnabled::QueryOnly);
	ActivationTrigger->SetCollisionResponseToAllChannels(ECR_Ignore);
	ActivationTrigger->SetCollisionResponseToChannel(ECC_Pawn, ECR_Overlap);
	ActivationTrigger->SetGenerateOverlapEvents(true);
	ActivationTrigger->OnComponentBeginOverlap.AddDynamic(this, &ACheckpoint::OnActivationOverlap);
}

void ACheckpoint::OnActivationOverlap(UPrimitiveComponent* OverlappedComponent, AActor* OtherActor,
	UPrimitiveComponent* OtherComp, int32 OtherBodyIndex, bool bFromSweep, const FHitResult& SweepResult)
{
	if (bIsActivated || !Cast<AKyokaiCharacter>(OtherActor))
	{
		return;
	}

	bIsActivated = true;

	if (AKyokaiGameMode* GameMode = GetWorld()->GetAuthGameMode<AKyokaiGameMode>())
	{
		GameMode->NotifyCheckpointActivated(GetActorLocation());
	}
}
