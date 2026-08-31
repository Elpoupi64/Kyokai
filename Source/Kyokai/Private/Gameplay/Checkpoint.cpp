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
	AKyokaiCharacter* Character = Cast<AKyokaiCharacter>(OtherActor);
	if (bIsActivated || !Character)
	{
		return;
	}

	bIsActivated = true;

	if (AKyokaiGameMode* GameMode = GetWorld()->GetAuthGameMode<AKyokaiGameMode>())
	{
		// The PLAYER's own location, not GetActorLocation() (this actor's
		// own transform) - Marker's pivot sits at its visual center, ~200cm
		// above the walkway (matches its half-height, see the constructor),
		// not the ~98cm a standing character's origin actually sits at. A
		// checkpoint respawn using this actor's own transform drops the
		// character ~100cm above the floor every time - harmless on its
		// own (falls, lands, continues), but real bug found once enough
		// lightning-triggered respawns hit Checkpoint_3 specifically to
		// expose it: the extra fall time let the character cross the
		// Segment 7 slide-start check (x=15800) still airborne, missing
		// the slide and ramming Ceiling_Seg7_Tunnel's face standing tall -
		// same failure shape as the wall-jump-shaft-variance case fixed
		// earlier, different root cause. The player is already standing
		// correctly on the ground the moment they touch the checkpoint, so
		// their own location is the correct one to remember.
		GameMode->NotifyCheckpointActivated(Character->GetActorLocation());
	}
}
