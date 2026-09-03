// Copyright Epic Games, Inc. All Rights Reserved.

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Actor.h"
#include "TimedGate.generated.h"

class UBoxComponent;
class UMaterialInstanceDynamic;
class UStaticMeshComponent;

/**
 * A solid barrier that cycles Closed -> Opening (telegraph) -> Open -> back
 * to Closed on a timer. Unlike AWindGust/ALightningStrike/AOnibi/ABakeneko
 * (all "react once, instantly" hazards - a single dodge or jump resolves
 * them), this actor's whole point is to force a real wait: a player who
 * arrives while Closed must stand and watch the telegraph before they can
 * pass, up to ClosedDuration seconds. Kept under the level brief's own
 * pacing rule (8.5: no forced wait > 5s) by construction - see the
 * per-instance duration comment below.
 *
 * Added 2026-09-03 as part of the Niveau 02 pacing redesign (see
 * Docs/Analyse_Rythme_Niveau02_v0.1.md): distance-only extension and
 * more copies of the existing "react once" hazards were both already
 * tried and shown (via real playtest data) to barely change real
 * completion time. A genuine forced wait is a lever neither of those
 * two prior passes used.
 */
UCLASS()
class KYOKAI_API ATimedGate : public AActor
{
	GENERATED_BODY()

public:
	ATimedGate();

	/** How long the gate stays fully closed (including the opening telegraph window below) before it opens. */
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Kyokai|Gate", meta = (ClampMin = "0.0"))
	float ClosedDuration = 3.5f;

	/** Trailing slice of ClosedDuration where the gate is still solid but visibly warns it's about to open. */
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Kyokai|Gate", meta = (ClampMin = "0.0"))
	float OpeningTelegraphDuration = 1.0f;

	/** How long the gate stays open (passable) before closing again. */
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Kyokai|Gate", meta = (ClampMin = "0.0"))
	float OpenDuration = 2.5f;

	UPROPERTY(BlueprintReadOnly, Category = "Kyokai|Gate")
	bool bIsOpeningTelegraph = false;

	UPROPERTY(BlueprintReadOnly, Category = "Kyokai|Gate")
	bool bIsOpen = false;

protected:
	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category = "Kyokai|Gate")
	TObjectPtr<UBoxComponent> GateVolume;

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category = "Kyokai|Gate")
	TObjectPtr<UStaticMeshComponent> GateMesh;

	virtual void BeginPlay() override;
	virtual void Tick(float DeltaTime) override;

private:
	enum class EGateState : uint8
	{
		Closed,
		OpeningTelegraph,
		Open
	};

	EGateState State = EGateState::Closed;
	float StateTimer = 0.0f;
	TObjectPtr<UMaterialInstanceDynamic> VisualMID;

	void EnterState(EGateState NewState);
	void UpdateVisualTint();
};
