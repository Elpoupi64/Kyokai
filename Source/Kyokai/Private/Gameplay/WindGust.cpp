// Copyright Epic Games, Inc. All Rights Reserved.

#include "Gameplay/WindGust.h"

#include "Characters/KyokaiCharacter.h"
#include "Characters/KyokaiMovementComponent.h"
#include "Components/BoxComponent.h"

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
}

void AWindGust::BeginPlay()
{
	Super::BeginPlay();
	EnterState(EGustState::Cooldown);
}

void AWindGust::EnterState(const EGustState NewState)
{
	State = NewState;
	StateTimer = 0.0f;
	bIsTelegraphing = (NewState == EGustState::Telegraphing);
	bIsGusting = (NewState == EGustState::Gusting);
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
