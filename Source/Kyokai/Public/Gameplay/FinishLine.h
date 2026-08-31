// Copyright Epic Games, Inc. All Rights Reserved.

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Actor.h"
#include "FinishLine.generated.h"

class UBoxComponent;

/**
 * Marks the level's actual finish, for the build-order step 7 playtest
 * instrumentation - Platform_Finish_L02 existed as a plain platform with
 * no way to know a player actually reached it; this is the trigger that
 * makes "level completed" a real, detectable gameplay event
 * (AKyokaiGameMode::NotifyLevelCompleted()) rather than something only
 * the timing bot could infer from an X threshold.
 */
UCLASS()
class KYOKAI_API AFinishLine : public AActor
{
	GENERATED_BODY()

public:
	AFinishLine();

protected:
	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category = "Kyokai|Finish")
	TObjectPtr<UBoxComponent> FinishTrigger;

private:
	UFUNCTION()
	void OnFinishOverlap(UPrimitiveComponent* OverlappedComponent, AActor* OtherActor,
		UPrimitiveComponent* OtherComp, int32 OtherBodyIndex, bool bFromSweep, const FHitResult& SweepResult);
};
