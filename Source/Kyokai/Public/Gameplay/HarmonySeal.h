// Copyright Epic Games, Inc. All Rights Reserved.

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Actor.h"
#include "HarmonySeal.generated.h"

class USphereComponent;
class UStaticMeshComponent;
class UMaterialInstanceDynamic;

/**
 * "Sceaux d'harmonie" (Level 02 brief, secrets section, règle 8.2): three
 * secrets, one per archetype - Reading (spot a nook, no execution
 * required), Mastery (a precise movement-chain test), Risk (a hazard
 * grazed for a resource cost). This class only covers the Reading kind so
 * far - Mastery and Risk need mechanics that don't exist yet (a precise
 * dash-bounce-dash chain; an "intégrité" resource) and are explicitly
 * scoped out of this first pass (see kyokai-level02-toits-pluie memory).
 *
 * Deliberately a separate class from AMemoryFragment, not a reskin of it -
 * the brief counts seals and the memory as distinct content ("3 sceaux, 1
 * mémoire" per the acceptance table), and Mastery/Risk will likely need
 * fields (a required-input-chain check; an integrity cost) that have no
 * business being on a narrative-only pickup.
 */
UENUM(BlueprintType)
enum class ESealKind : uint8
{
	Reading,
	Mastery,
	Risk
};

UCLASS()
class KYOKAI_API AHarmonySeal : public AActor
{
	GENERATED_BODY()

public:
	AHarmonySeal();

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Kyokai|Seal")
	ESealKind SealKind = ESealKind::Reading;

	/** Short stable identifier for playtest logging. */
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Kyokai|Seal")
	FString SealId = TEXT("harmony_seal");

protected:
	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category = "Kyokai|Seal")
	TObjectPtr<USphereComponent> CollectTrigger;

	/** Distinct from AMemoryFragment's warm amber - a cool violet, reading as "secret" rather than "story". */
	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category = "Kyokai|Seal")
	TObjectPtr<UStaticMeshComponent> VisualMesh;

	virtual void BeginPlay() override;

private:
	UFUNCTION()
	void OnCollectOverlap(UPrimitiveComponent* OverlappedComponent, AActor* OtherActor,
		UPrimitiveComponent* OtherComp, int32 OtherBodyIndex, bool bFromSweep, const FHitResult& SweepResult);

	bool bCollected = false;
	TObjectPtr<UMaterialInstanceDynamic> VisualMID;
};
