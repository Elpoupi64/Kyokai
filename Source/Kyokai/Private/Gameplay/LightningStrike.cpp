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
		AKyokaiCharacter* Character = Cast<AKyokaiCharacter>(Actor);
		if (!Character)
		{
			continue;
		}

		// Knock the character back the way it came, not forward - being hit
		// costs distance, it doesn't help skip ahead. See the stopgap note
		// in the header before reusing/extending this consequence.
		// FacingDirection itself is private to AKyokaiCharacter; its own
		// yaw (0 facing +X, 180 facing -X, set alongside FacingDirection in
		// PerformWallJump/movement) is the public equivalent to read here.
		const float FacingSign = FMath::IsNearlyZero(FMath::UnwindDegrees(Character->GetActorRotation().Yaw)) ? 1.0f : -1.0f;
		const FVector Knockback(-FacingSign * KnockbackHorizontal, 0.0f, KnockbackVertical);
		Character->LaunchCharacter(Knockback, true, true);
	}
}
