// Copyright Epic Games, Inc. All Rights Reserved.

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/CharacterMovementComponent.h"
#include "KyokaiMovementComponent.generated.h"

/**
 * Movement defaults for the controller prototype.
 *
 * The component deliberately keeps the first milestone small: responsive
 * ground/air movement, a strict 2.5D plane and a slide speed override. More
 * specialised traversal modes can be added after the controller gym has been
 * play-tested.
 */
UCLASS(ClassGroup = Movement, meta = (BlueprintSpawnableComponent))
class KYOKAI_API UKyokaiMovementComponent : public UCharacterMovementComponent
{
	GENERATED_BODY()

public:
	UKyokaiMovementComponent();

	virtual float GetMaxSpeed() const override;

	UFUNCTION(BlueprintCallable, Category = "Kyokai|Movement")
	void SetSliding(bool bEnabled);

	UFUNCTION(BlueprintPure, Category = "Kyokai|Movement")
	bool IsSliding() const { return bSliding; }

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Kyokai|Movement|Slide", meta = (ClampMin = "0.0"))
	float SlideMaxSpeed = 1400.0f;

	// Sloped-tile grip (Level 02 "tuiles inclinées"): the engine doesn't read
	// a walkable floor's PhysicalMaterial for friction on its own (PhysWalking
	// reads the raw GroundFriction/BrakingDecelerationWalking members
	// directly), so this component rescales both every tick from the current
	// floor's PhysicalMaterial::Friction relative to ReferenceSurfaceFriction
	// (UE's own PhysicalMaterial default, 0.7 - confirmed via a fresh default
	// instance rather than assumed). A tile using the default/no material
	// behaves identically to before this system existed.
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Kyokai|Movement|Tiles", meta = (ClampMin = "0.01"))
	float ReferenceSurfaceFriction = 0.7f;

	virtual void TickComponent(float DeltaTime, ELevelTick TickType, FActorComponentTickFunction* ThisTickFunction) override;

private:
	bool bSliding = false;
	float BaseGroundFriction = 0.0f;
	float BaseBrakingDecelerationWalking = 0.0f;
};
