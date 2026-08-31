// Copyright Epic Games, Inc. All Rights Reserved.

#include "Gameplay/LightningStrike.h"

#include "Characters/KyokaiCharacter.h"
#include "Components/BoxComponent.h"

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
}

void ALightningStrike::EnterState(const EStrikeState NewState)
{
	State = NewState;
	StateTimer = 0.0f;
	bIsTelegraphing = (NewState == EStrikeState::Telegraphing);
}

void ALightningStrike::Tick(const float DeltaTime)
{
	Super::Tick(DeltaTime);

	StateTimer += DeltaTime;

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
			Character->RespawnAtCheckpoint();
		}
	}
}
