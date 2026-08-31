// Copyright Epic Games, Inc. All Rights Reserved.

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Actor.h"
#include "WindGust.generated.h"

class UBoxComponent;

/**
 * "Rafales de vent annoncées visuellement" (Level 02 obstacle brief). Cycles
 * through Cooldown -> Telegraphing -> Gusting on a timer: Telegraphing is a
 * clear warning window before the push lands (bIsTelegraphing/bIsGusting
 * are BlueprintReadOnly so an art pass can drive a visual cue off them
 * without touching this logic), and Gusting applies a steady lateral
 * acceleration - not an instant shove - to any AKyokaiCharacter standing in
 * the volume, so the player can lean into or fight the wind rather than
 * being flicked by it. Deliberately additive (Velocity += Accel * DeltaTime)
 * rather than BouncePad's LaunchCharacter-style outright replacement: a
 * gust should feel like a force to push against, not a single displacement.
 */
UCLASS()
class KYOKAI_API AWindGust : public AActor
{
	GENERATED_BODY()

public:
	AWindGust();

	/** How long the warning phase lasts before the gust actually pushes - the player's reaction window. */
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Kyokai|Wind", meta = (ClampMin = "0.0"))
	float TelegraphDuration = 1.0f;

	/** How long the push itself lasts once it starts. */
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Kyokai|Wind", meta = (ClampMin = "0.0"))
	float GustDuration = 0.6f;

	/** How long the volume stays quiet between one gust ending and the next telegraph starting. */
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Kyokai|Wind", meta = (ClampMin = "0.0"))
	float CooldownDuration = 2.0f;

	/**
	 * Movement input strength the gust feeds in every tick while active, in
	 * the same units as the player's own held-forward input (1.0 = full).
	 * -1.0 exactly cancels a held-forward run (net input 0, character
	 * decelerates via friction/braking) - this is deliberately an input,
	 * not a raw velocity/acceleration delta: CharacterMovementComponent
	 * recomputes Acceleration from the combined input vector every tick, so
	 * a velocity nudged some other way just gets corrected right back by
	 * the player's own MaxAcceleration on the very next tick. Only a dash
	 * (which sets Velocity directly, bypassing input entirely) reliably
	 * punches through a full-strength gust - intentional, ties into this
	 * zone's "rappel de la ruée" test per the level brief.
	 */
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Kyokai|Wind", meta = (ClampMin = "-1.0", ClampMax = "1.0"))
	float GustInputStrengthX = -1.0f;

	UPROPERTY(BlueprintReadOnly, Category = "Kyokai|Wind")
	bool bIsTelegraphing = false;

	UPROPERTY(BlueprintReadOnly, Category = "Kyokai|Wind")
	bool bIsGusting = false;

protected:
	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category = "Kyokai|Wind")
	TObjectPtr<UBoxComponent> GustVolume;

	virtual void BeginPlay() override;
	virtual void Tick(float DeltaTime) override;

private:
	enum class EGustState : uint8
	{
		Cooldown,
		Telegraphing,
		Gusting
	};

	EGustState State = EGustState::Cooldown;
	float StateTimer = 0.0f;

	void EnterState(EGustState NewState);
};
