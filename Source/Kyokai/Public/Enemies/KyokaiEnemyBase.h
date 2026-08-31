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
 * STOPGAP CONSEQUENCE, same situation as ALightningStrike: no health or
 * checkpoint system exists yet in this project (checkpoints are Level 02
 * build-order step 6, still ahead of enemies at step 5), so contact knocks
 * the character back-and-up via LaunchCharacter rather than doing nothing
 * or killing with nowhere to respawn to. Reconsider once checkpoints exist.
 */
UCLASS(Abstract)
class KYOKAI_API AKyokaiEnemyBase : public AActor
{
	GENERATED_BODY()

public:
	AKyokaiEnemyBase();

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Kyokai|Enemy")
	float KnockbackHorizontal = 700.0f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Kyokai|Enemy")
	float KnockbackVertical = 600.0f;

protected:
	/** Nearest AKyokaiCharacter in the world, or nullptr if none exists - there's only ever one player, so this is a simple actor-iterator, not a spatial query. */
	AKyokaiCharacter* FindPlayerCharacter() const;

	/** Knocks Character away from this actor's X position - see the stopgap note above before reusing/extending this. */
	void ApplyContactKnockback(AKyokaiCharacter* Character) const;
};
