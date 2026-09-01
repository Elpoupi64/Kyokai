// Copyright Epic Games, Inc. All Rights Reserved.

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Actor.h"
#include "MemoryFragment.generated.h"

class USphereComponent;
class UStaticMeshComponent;
class UMaterialInstanceDynamic;

/**
 * "Mémoire gravée" (Level 02 brief, secrets section, règle 8.2): a short
 * narrative pickup placed somewhere easy during the Accroche/Enseignement
 * segments - explicitly never gated behind a difficult execution, unlike
 * the three "sceaux d'harmonie" secrets (a separate, not-yet-built system).
 *
 * Purely a system + trigger for now: FragmentText is the narrative content
 * a real UI would present, but no journal/dialogue widget exists anywhere
 * in this project yet, so collecting one just logs the text (UE_LOG plus
 * the existing playtest JSONL stream) as a stand-in - a real presentation
 * pass is future work, not built here, same honesty as the "programmer
 * art" note on the earlier décor pass.
 */
UCLASS()
class KYOKAI_API AMemoryFragment : public AActor
{
	GENERATED_BODY()

public:
	AMemoryFragment();

	/** Short narrative snippet this fragment reveals - a stand-in for what a real journal/dialogue UI would show. */
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Kyokai|Memory")
	FString FragmentText;

	/** Short stable identifier for playtest logging, distinct from the display text (which may change/be localized later). */
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Kyokai|Memory")
	FString FragmentId = TEXT("memory_fragment");

protected:
	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category = "Kyokai|Memory")
	TObjectPtr<USphereComponent> CollectTrigger;

	/** Small drifting-orb marker - "objet éveillé bénin" made visible, distinct in color from every hazard telegraph in this level (warm amber, not a warning tone). */
	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category = "Kyokai|Memory")
	TObjectPtr<UStaticMeshComponent> VisualMesh;

	virtual void BeginPlay() override;

private:
	UFUNCTION()
	void OnCollectOverlap(UPrimitiveComponent* OverlappedComponent, AActor* OtherActor,
		UPrimitiveComponent* OtherComp, int32 OtherBodyIndex, bool bFromSweep, const FHitResult& SweepResult);

	bool bCollected = false;
	TObjectPtr<UMaterialInstanceDynamic> VisualMID;
};
