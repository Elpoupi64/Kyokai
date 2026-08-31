// Copyright Epic Games, Inc. All Rights Reserved.

#include "Enemies/KyokaiEnemyBase.h"

#include "Characters/KyokaiCharacter.h"
#include "Kismet/GameplayStatics.h"

AKyokaiEnemyBase::AKyokaiEnemyBase()
{
	PrimaryActorTick.bCanEverTick = true;
}

AKyokaiCharacter* AKyokaiEnemyBase::FindPlayerCharacter() const
{
	APawn* PlayerPawn = UGameplayStatics::GetPlayerPawn(this, 0);
	return Cast<AKyokaiCharacter>(PlayerPawn);
}

void AKyokaiEnemyBase::ApplyContactKnockback(AKyokaiCharacter* Character) const
{
	if (!Character)
	{
		return;
	}

	const float AwaySign = (Character->GetActorLocation().X < GetActorLocation().X) ? -1.0f : 1.0f;
	const FVector Knockback(AwaySign * KnockbackHorizontal, 0.0f, KnockbackVertical);
	Character->LaunchCharacter(Knockback, true, true);
}
