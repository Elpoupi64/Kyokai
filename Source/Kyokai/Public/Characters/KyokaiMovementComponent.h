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

private:
	bool bSliding = false;
};
