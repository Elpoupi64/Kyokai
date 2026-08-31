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

void AKyokaiEnemyBase::ApplyContactConsequence(AKyokaiCharacter* Character, const FString& Cause) const
{
	if (Character)
	{
		Character->RespawnAtCheckpoint(Cause);
	}
}
