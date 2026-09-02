// Copyright Epic Games, Inc. All Rights Reserved.

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Actor.h"
#include "CopperRivet.generated.h"

class USphereComponent;
class UStaticMeshComponent;
class UMaterialInstanceDynamic;

/**
 * "Une ligne de rivets de cuivre qui maintient la jauge de Pression pour
 * tenir le rythme de ruées/rebonds sans jamais retoucher le toit
 * principal" - the level brief's own literal fill mechanism for the
 * expert route (see AKyokaiCharacter's Pression block for the resource
 * itself, built earlier the same day). Placed in a line along every
 * Expert_Seg*_Upper/Bridge platform.
 *
 * Deliberately NOT single-use like AMemoryFragment/AHarmonySeal - a rivet
 * "maintains" the gauge, so it stays triggerable on a cooldown (matching
 * ABouncePad's own repeat-trigger pattern) rather than being spent once.
 * A player just running the route past it once only gets one hit either
 * way; the difference only matters if they linger or double back.
 */
UCLASS()
class KYOKAI_API ACopperRivet : public AActor
{
	GENERATED_BODY()

public:
	ACopperRivet();

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Kyokai|Pression", meta = (ClampMin = "0.0"))
	float PressionBonus = 0.3f;

	/** Minimum time between two triggers, so standing on/near one doesn't refill every tick. */
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Kyokai|Pression", meta = (ClampMin = "0.0"))
	float TriggerCooldown = 1.0f;

protected:
	/** Small stud, tinted copper - distinct from every other pickup's own color (memory's amber, seals' violet). */
	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category = "Kyokai|Pression")
	TObjectPtr<UStaticMeshComponent> VisualMesh;

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category = "Kyokai|Pression")
	TObjectPtr<USphereComponent> TriggerVolume;

	virtual void BeginPlay() override;

private:
	UFUNCTION()
	void OnTriggerOverlap(UPrimitiveComponent* OverlappedComponent, AActor* OtherActor,
		UPrimitiveComponent* OtherComp, int32 OtherBodyIndex, bool bFromSweep, const FHitResult& SweepResult);

	float LastTriggerTime = -1000.0f;
	TObjectPtr<UMaterialInstanceDynamic> VisualMID;
};
