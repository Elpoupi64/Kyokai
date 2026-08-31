// Copyright Epic Games, Inc. All Rights Reserved.

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Actor.h"
#include "KyokaiEnemyBase.generated.h"

class AKyokaiCharacter;

/**
 * Shared groundwork for Level 02's graybox enemies (Onibi, Bakeneko) - both
 * need "is the player nearby" detection and an on-contact consequence, so
 * that much lives here rather than being duplicated per enemy. Each
 * subclass owns its own state machine and movement; this base does not
 * tick or move anything on its own.
 *
 * CONSEQUENCE: contact respawns the character at the last checkpoint
 * (AKyokaiCharacter::RespawnAtCheckpoint()) - a real cost now that
 * checkpoints exist (Level 02 build-order step 6), same as ALightningStrike.
 * This replaced an earlier knockback-only stopgap (LaunchCharacter, no real
 * cost) from when enemies were built (step 5) before checkpoints existed.
 * The level brief's own acceptance criteria talk about hazard "death" and
 * failures directly, which this now actually matches - a graze isn't
 * survivable, it costs the run back to the checkpoint, same genre
 * convention as Celeste/Super Meat Boy rather than a soft push.
 */
UCLASS(Abstract)
class KYOKAI_API AKyokaiEnemyBase : public AActor
{
	GENERATED_BODY()

public:
	AKyokaiEnemyBase();

protected:
	/** Nearest AKyokaiCharacter in the world, or nullptr if none exists - there's only ever one player, so this is a simple actor-iterator, not a spatial query. */
	AKyokaiCharacter* FindPlayerCharacter() const;

	/** Respawns Character at the last checkpoint - see the class comment above. Cause is a short identifier ("onibi", "bakeneko", ...) for the playtest death-cause tally, see AKyokaiCharacter::RespawnAtCheckpoint(). */
	void ApplyContactConsequence(AKyokaiCharacter* Character, const FString& Cause) const;
};
