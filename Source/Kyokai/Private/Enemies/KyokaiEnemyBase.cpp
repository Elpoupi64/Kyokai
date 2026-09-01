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
	if (!Character)
	{
		return;
	}

	// Real integrity system now (see AKyokaiCharacter::ApplyHazardHit) -
	// most contacts cost 1 segment and knock the character away rather
	// than a full checkpoint reset; ApplyHazardHit() already handles the
	// full reset itself once segments reach 0, so this only needs to add
	// the knockback for the "still has integrity left" case. Pushes away
	// from this hazard's own position - generic so it works for both
	// Onibi and Bakeneko without per-type tuning, and gives the player
	// physical feedback for the hit since there's no HUD yet to show the
	// lost segment otherwise.
	if (Character->ApplyHazardHit(Cause))
	{
		FVector PushDir = (Character->GetActorLocation() - GetActorLocation()).GetSafeNormal2D();
		if (PushDir.IsNearlyZero())
		{
			PushDir = FVector(1.0f, 0.0f, 0.0f);
		}
		Character->LaunchCharacter(FVector(PushDir.X, PushDir.Y, 0.6f) * 500.0f, true, true);
	}
}
