// Copyright Epic Games, Inc. All Rights Reserved.

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Actor.h"
#include "LightningStrike.generated.h"

class UBoxComponent;

/**
 * "Éclairs avec télégraphie claire avant impact" (Level 02 obstacle brief,
 * Segment 6 "Paratonnerres et éclairs rythmés" - "lecture des dangers").
 * Cycles Cooldown -> Telegraphing -> strike (instantaneous) -> Cooldown.
 * bIsTelegraphing is BlueprintReadOnly for an art pass to drive a warning
 * cue off later; the read-the-danger test is the telegraph window itself,
 * not this class's job to visualize yet.
 *
 * CONSEQUENCE: a strike respawns the character at the last checkpoint
 * (AKyokaiCharacter::RespawnAtCheckpoint()) - a real cost, matching the
 * level brief's own acceptance criteria talking about hazard "death" and
 * failures directly. This replaced an earlier knockback-only stopgap
 * (LaunchCharacter, no real cost) from when this was built (step 4) before
 * checkpoints existed (step 6).
 */
UCLASS()
class KYOKAI_API ALightningStrike : public AActor
{
	GENERATED_BODY()

public:
	ALightningStrike();

	/** Warning window before the strike - the player's reaction/dodge time. */
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Kyokai|Lightning", meta = (ClampMin = "0.0"))
	float TelegraphDuration = 1.2f;

	/** Time between one strike and the next telegraph starting. */
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Kyokai|Lightning", meta = (ClampMin = "0.0"))
	float CooldownDuration = 3.0f;

	UPROPERTY(BlueprintReadOnly, Category = "Kyokai|Lightning")
	bool bIsTelegraphing = false;

protected:
	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category = "Kyokai|Lightning")
	TObjectPtr<UBoxComponent> StrikeVolume;

	virtual void Tick(float DeltaTime) override;

private:
	enum class EStrikeState : uint8
	{
		Cooldown,
		Telegraphing
	};

	EStrikeState State = EStrikeState::Cooldown;
	float StateTimer = 0.0f;

	void EnterState(EStrikeState NewState);
	void ExecuteStrike();
};
