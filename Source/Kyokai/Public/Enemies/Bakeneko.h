// Copyright Epic Games, Inc. All Rights Reserved.

#pragma once

#include "CoreMinimal.h"
#include "Enemies/KyokaiEnemyBase.h"
#include "Bakeneko.generated.h"

class UBoxComponent;
class UMaterialInstanceDynamic;
class UStaticMeshComponent;

/**
 * "Ennemi Bakeneko de gouttière : poursuite et bond" (Level 02 obstacle
 * brief, Segment 5 "Gouttières et Bakeneko" - "saut mural et poursuite").
 * Ground-based (fixed to its home Z - the Segment 5 chase stretch is flat,
 * so no floor-tracing needed for graybox), waits until the player is
 * within range, then chases along X at ChaseSpeed. Closing to pounce range
 * telegraphs, then lunges (a fast horizontal+vertical burst, not unlike a
 * jump) - contact during the lunge knocks the player back (see the stopgap
 * note on AKyokaiEnemyBase). Loses interest and returns home if the player
 * gets far enough away.
 */
UCLASS()
class KYOKAI_API ABakeneko : public AKyokaiEnemyBase
{
	GENERATED_BODY()

public:
	ABakeneko();

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Kyokai|Bakeneko")
	float DetectionRangeX = 1200.0f;

	/** Beyond this X distance from home, give up the chase and return - keeps it from following the player past its own territory. */
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Kyokai|Bakeneko")
	float LeashRangeX = 1600.0f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Kyokai|Bakeneko")
	float PounceRangeX = 350.0f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Kyokai|Bakeneko")
	float ChaseSpeed = 900.0f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Kyokai|Bakeneko")
	float TelegraphDuration = 0.5f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Kyokai|Bakeneko")
	float PounceSpeed = 1800.0f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Kyokai|Bakeneko")
	float PounceDuration = 0.4f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Kyokai|Bakeneko")
	float PounceCooldown = 1.0f;

	UPROPERTY(BlueprintReadOnly, Category = "Kyokai|Bakeneko")
	bool bIsChasing = false;

	UPROPERTY(BlueprintReadOnly, Category = "Kyokai|Bakeneko")
	bool bIsTelegraphingPounce = false;

	UPROPERTY(BlueprintReadOnly, Category = "Kyokai|Bakeneko")
	bool bIsPouncing = false;

protected:
	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category = "Kyokai|Bakeneko")
	TObjectPtr<UStaticMeshComponent> BodyMesh;

	/** A second, smaller box offset toward the front for a head silhouette - the flat squashed-cube body alone reads as a crate, not a creature. */
	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category = "Kyokai|Bakeneko")
	TObjectPtr<UStaticMeshComponent> HeadMesh;

	/**
	 * "Décor artistique" pass, enemy visual polish (2026-08-31): glowing
	 * eyes are the classic bakeneko/cat-yokai cue (a cat's eyes catching
	 * light in the dark) - two small emissive spheres on the head, tinted
	 * via the same dynamic-material-instance pattern as everywhere else in
	 * this pass, brightening through telegraph/pounce.
	 */
	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category = "Kyokai|Bakeneko")
	TObjectPtr<UStaticMeshComponent> EyeLeft;

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category = "Kyokai|Bakeneko")
	TObjectPtr<UStaticMeshComponent> EyeRight;

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category = "Kyokai|Bakeneko")
	TObjectPtr<UBoxComponent> HitBox;

	virtual void BeginPlay() override;
	virtual void Tick(float DeltaTime) override;

private:
	enum class EBakenekoState : uint8
	{
		Idle,
		Chasing,
		Telegraphing,
		Pouncing,
		Cooldown
	};

	EBakenekoState State = EBakenekoState::Idle;
	float StateTimer = 0.0f;
	FVector HomeLocation = FVector::ZeroVector;
	float PounceDirectionSign = 1.0f;
	TObjectPtr<UMaterialInstanceDynamic> EyeMID;

	void EnterState(EBakenekoState NewState);
	void UpdateEyeGlow();
	void UpdateFacing(float MoveSignX);

	UFUNCTION()
	void OnHitBoxOverlap(UPrimitiveComponent* OverlappedComponent, AActor* OtherActor,
		UPrimitiveComponent* OtherComp, int32 OtherBodyIndex, bool bFromSweep, const FHitResult& SweepResult);
};
