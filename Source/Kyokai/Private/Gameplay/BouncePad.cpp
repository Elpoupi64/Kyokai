// Copyright Epic Games, Inc. All Rights Reserved.

#include "Gameplay/BouncePad.h"

#include "Characters/KyokaiCharacter.h"
#include "Components/BoxComponent.h"
#include "Components/StaticMeshComponent.h"
#include "Engine/StaticMesh.h"
#include "UObject/ConstructorHelpers.h"

ABouncePad::ABouncePad()
{
	PrimaryActorTick.bCanEverTick = false;

	PadMesh = CreateDefaultSubobject<UStaticMeshComponent>(TEXT("PadMesh"));
	RootComponent = PadMesh;
	PadMesh->SetCollisionProfileName(TEXT("BlockAll"));
	PadMesh->SetMobility(EComponentMobility::Static);

	static ConstructorHelpers::FObjectFinder<UStaticMesh> CylinderFinder(TEXT("/Engine/BasicShapes/Cylinder.Cylinder"));
	if (CylinderFinder.Succeeded())
	{
		PadMesh->SetStaticMesh(CylinderFinder.Object);
	}

	// The engine cylinder is a 100x100x100 bounding box centered on its
	// pivot (50cm radius, 100cm tall) - the trigger sits right at its top
	// surface (local Z=50), inset a little from the 50cm radius so it
	// reliably catches a landing without poking out past the pad's edges.
	BounceTrigger = CreateDefaultSubobject<UBoxComponent>(TEXT("BounceTrigger"));
	BounceTrigger->SetupAttachment(PadMesh);
	BounceTrigger->SetRelativeLocation(FVector(0.0f, 0.0f, 50.0f));
	BounceTrigger->SetBoxExtent(FVector(45.0f, 45.0f, 15.0f));
	BounceTrigger->SetCollisionEnabled(ECollisionEnabled::QueryOnly);
	BounceTrigger->SetCollisionResponseToAllChannels(ECR_Ignore);
	BounceTrigger->SetCollisionResponseToChannel(ECC_Pawn, ECR_Overlap);
	BounceTrigger->SetGenerateOverlapEvents(true);
	BounceTrigger->OnComponentBeginOverlap.AddDynamic(this, &ABouncePad::OnBounceTriggerOverlap);
}

void ABouncePad::OnBounceTriggerOverlap(UPrimitiveComponent* OverlappedComponent, AActor* OtherActor,
	UPrimitiveComponent* OtherComp, int32 OtherBodyIndex, bool bFromSweep, const FHitResult& SweepResult)
{
	AKyokaiCharacter* Character = Cast<AKyokaiCharacter>(OtherActor);
	if (!Character)
	{
		return;
	}

	const float Now = GetWorld()->GetTimeSeconds();
	if (Now - LastBounceTime < BounceCooldown)
	{
		return;
	}
	LastBounceTime = Now;

	// Pression: "bounces" is one of the GDD's own listed fill sources -
	// this is what makes a dash->bounce->dash chain (the mastery seal's
	// own design) affordable: the bounce here refunds enough to cover
	// the second dash.
	Character->AddPression(Character->PressionBounceBonus);

	// bXYOverride=false keeps whatever horizontal velocity the character
	// already had (so a running jump into the pad keeps its direction);
	// bZOverride=true replaces vertical velocity outright with the bounce.
	Character->LaunchCharacter(FVector(0.0f, 0.0f, BounceVelocityZ), false, true);
}
