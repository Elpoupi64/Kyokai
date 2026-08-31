// Copyright Epic Games, Inc. All Rights Reserved.

#pragma once

#include "CoreMinimal.h"
#include "Enemies/KyokaiEnemyBase.h"
#include "Onibi.generated.h"

class UBoxComponent;
class UMaterialInstanceDynamic;
class UPointLightComponent;
class USphereComponent;
class UStaticMeshComponent;

/**
 * "Ennemi Onibi simple : déplacement flottant et charge" (Level 02 obstacle
 * brief, Segment 4 "Première rencontre avec les Onibi" - "mouvement autour
 * d'un ennemi"). Floats in place (a gentle bob, no patrol path needed for
 * graybox), and when the player is close enough on the same X/Z plane it
 * telegraphs, then charges horizontally at them - the "movement around an
 * enemy" test is jumping the charge's hitbox height, no player-side attack
 * exists (or is needed) in this project.
 */
UCLASS()
class KYOKAI_API AOnibi : public AKyokaiEnemyBase
{
	GENERATED_BODY()

public:
	AOnibi();

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Kyokai|Onibi")
	float DetectionRadiusX = 900.0f;

	/** Player must be within this Z of the Onibi's hover height to trigger a charge - keeps it from reacting to a player on a completely different platform. */
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Kyokai|Onibi")
	float DetectionRangeZ = 300.0f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Kyokai|Onibi")
	float TelegraphDuration = 0.8f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Kyokai|Onibi")
	float ChargeSpeed = 1400.0f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Kyokai|Onibi")
	float ChargeDuration = 0.7f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Kyokai|Onibi")
	float AttackCooldown = 1.5f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Kyokai|Onibi")
	float BobAmplitude = 25.0f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Kyokai|Onibi")
	float BobSpeed = 2.0f;

	UPROPERTY(BlueprintReadOnly, Category = "Kyokai|Onibi")
	bool bIsTelegraphingCharge = false;

	UPROPERTY(BlueprintReadOnly, Category = "Kyokai|Onibi")
	bool bIsCharging = false;

protected:
	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category = "Kyokai|Onibi")
	TObjectPtr<UStaticMeshComponent> BodyMesh;

	/**
	 * "Décor artistique" pass, enemy visual polish (2026-08-31): a will-o-
	 * wisp reads as inert without actually casting light, so this is a real
	 * PointLight, not just an emissive surface - it lights up nearby
	 * geometry, which a flat-shaded sphere alone can't do. Flickers via a
	 * sine+noise blend in Tick() and intensifies through the telegraph/
	 * charge states, same state-driven-tint idea as AWindGust/ALightningStrike.
	 */
	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category = "Kyokai|Onibi")
	TObjectPtr<UPointLightComponent> GlowLight;

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category = "Kyokai|Onibi")
	TObjectPtr<UBoxComponent> HitBox;

	virtual void BeginPlay() override;
	virtual void Tick(float DeltaTime) override;

private:
	enum class EOnibiState : uint8
	{
		Patrol,
		Telegraphing,
		Charging,
		Cooldown
	};

	EOnibiState State = EOnibiState::Patrol;
	float StateTimer = 0.0f;
	float BobTime = 0.0f;
	float FlickerTime = 0.0f;
	FVector HomeLocation = FVector::ZeroVector;
	float ChargeDirectionSign = 1.0f;
	TObjectPtr<UMaterialInstanceDynamic> BodyMID;

	void EnterState(EOnibiState NewState);
	void UpdateGlow(float DeltaTime);

	UFUNCTION()
	void OnHitBoxOverlap(UPrimitiveComponent* OverlappedComponent, AActor* OtherActor,
		UPrimitiveComponent* OtherComp, int32 OtherBodyIndex, bool bFromSweep, const FHitResult& SweepResult);
};
