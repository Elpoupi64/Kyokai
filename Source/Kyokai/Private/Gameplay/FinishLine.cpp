// Copyright Epic Games, Inc. All Rights Reserved.

#include "Gameplay/FinishLine.h"

#include "Characters/KyokaiCharacter.h"
#include "Components/BoxComponent.h"
#include "Game/KyokaiGameMode.h"

AFinishLine::AFinishLine()
{
	PrimaryActorTick.bCanEverTick = false;

	FinishTrigger = CreateDefaultSubobject<UBoxComponent>(TEXT("FinishTrigger"));
	RootComponent = FinishTrigger;
	FinishTrigger->SetBoxExtent(FVector(100.0f, 200.0f, 250.0f));
	FinishTrigger->SetCollisionEnabled(ECollisionEnabled::QueryOnly);
	FinishTrigger->SetCollisionResponseToAllChannels(ECR_Ignore);
	FinishTrigger->SetCollisionResponseToChannel(ECC_Pawn, ECR_Overlap);
	FinishTrigger->SetGenerateOverlapEvents(true);
	FinishTrigger->OnComponentBeginOverlap.AddDynamic(this, &AFinishLine::OnFinishOverlap);
}

void AFinishLine::OnFinishOverlap(UPrimitiveComponent* OverlappedComponent, AActor* OtherActor,
	UPrimitiveComponent* OtherComp, int32 OtherBodyIndex, bool bFromSweep, const FHitResult& SweepResult)
{
	if (!Cast<AKyokaiCharacter>(OtherActor))
	{
		return;
	}

	if (AKyokaiGameMode* GameMode = GetWorld()->GetAuthGameMode<AKyokaiGameMode>())
	{
		GameMode->NotifyLevelCompleted();
	}
}
