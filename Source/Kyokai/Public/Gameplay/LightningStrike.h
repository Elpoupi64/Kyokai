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
 * STOPGAP CONSEQUENCE, read before reusing this pattern elsewhere: there is
 * no health or checkpoint system in this project yet (checkpoints are Level
 * 02 build-order step 6, still ahead of this one, step 4). A real "hazard"
 * needs a real cost or the telegraph teaches nothing, so a strike knocks the
 * character hard backward-and-up (LaunchCharacter, same mechanism as
 * ABouncePad) rather than doing nothing or silently killing/respawning with
 * no checkpoint to return to. When checkpoints exist, reconsider this - a
 * knockback that just costs a few seconds of re-crossing may or may not
 * still be the right consequence once there's a real fail state to fall
 * back to.
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

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Kyokai|Lightning")
	float KnockbackHorizontal = 700.0f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Kyokai|Lightning")
	float KnockbackVertical = 650.0f;

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
