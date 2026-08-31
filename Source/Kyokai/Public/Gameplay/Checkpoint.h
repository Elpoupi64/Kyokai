// Copyright Epic Games, Inc. All Rights Reserved.

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Actor.h"
#include "Checkpoint.generated.h"

class UBoxComponent;
class UStaticMeshComponent;

/**
 * Level 02 build-order step 6: "Placer les trois checkpoints". First real
 * use case for a checkpoint in this project - on overlap, tells
 * AKyokaiGameMode where to send the player back to (AKyokaiCharacter's
 * fall-catch in Tick() reads that back). bIsActivated is BlueprintReadOnly
 * so an art pass can drive a visual state change off it later.
 */
UCLASS()
class KYOKAI_API ACheckpoint : public AActor
{
	GENERATED_BODY()

public:
	ACheckpoint();

	UPROPERTY(BlueprintReadOnly, Category = "Kyokai|Checkpoint")
	bool bIsActivated = false;

protected:
	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category = "Kyokai|Checkpoint")
	TObjectPtr<UStaticMeshComponent> Marker;

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category = "Kyokai|Checkpoint")
	TObjectPtr<UBoxComponent> ActivationTrigger;

private:
	UFUNCTION()
	void OnActivationOverlap(UPrimitiveComponent* OverlappedComponent, AActor* OtherActor,
		UPrimitiveComponent* OtherComp, int32 OtherBodyIndex, bool bFromSweep, const FHitResult& SweepResult);
};
