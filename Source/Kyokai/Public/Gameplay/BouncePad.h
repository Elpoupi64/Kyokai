// Copyright Epic Games, Inc. All Rights Reserved.

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Actor.h"
#include "BouncePad.generated.h"

class UBoxComponent;
class UStaticMeshComponent;

/**
 * Generic bounce object for the controller gym's "rebond" zone - a stand-in
 * for "ressort, objet ou futur ennemi" (design doc). Solid collision (so
 * the character actually lands on it, triggering the normal Landed() state
 * resets) plus an on-hit launch: touching it replaces vertical velocity
 * with BounceVelocityZ while leaving horizontal velocity untouched, so a
 * running jump into one keeps its direction and just gets thrown upward.
 */
UCLASS()
class KYOKAI_API ABouncePad : public AActor
{
	GENERATED_BODY()

public:
	ABouncePad();

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Kyokai|Bounce", meta = (ClampMin = "0.0"))
	float BounceVelocityZ = 1800.0f;

	/** Minimum time between two bounces off this pad, so a character resting on it doesn't get launched every tick. */
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Kyokai|Bounce", meta = (ClampMin = "0.0"))
	float BounceCooldown = 0.3f;

protected:
	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category = "Kyokai|Bounce")
	TObjectPtr<UStaticMeshComponent> PadMesh;

	/**
	 * Overlap-only trigger sitting right at the pad's top surface. Detecting
	 * the bounce via overlap (rather than a blocking hit event on PadMesh
	 * itself) is deliberate: a kinematic character sweep landing on static
	 * blocking geometry doesn't reliably fire OnComponentHit without extra
	 * physics-simulation flags that would also make the pad itself movable;
	 * overlap detection has no such dependency and PadMesh can stay a plain
	 * solid platform.
	 */
	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category = "Kyokai|Bounce")
	TObjectPtr<UBoxComponent> BounceTrigger;

private:
	UFUNCTION()
	void OnBounceTriggerOverlap(UPrimitiveComponent* OverlappedComponent, AActor* OtherActor,
		UPrimitiveComponent* OtherComp, int32 OtherBodyIndex, bool bFromSweep, const FHitResult& SweepResult);

	float LastBounceTime = -1000.0f;
};
