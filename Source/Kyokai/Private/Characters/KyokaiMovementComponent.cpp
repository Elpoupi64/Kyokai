// Copyright Epic Games, Inc. All Rights Reserved.

#include "Characters/KyokaiMovementComponent.h"

UKyokaiMovementComponent::UKyokaiMovementComponent()
{
	GravityScale = 2.4f;
	MaxWalkSpeed = 850.0f;
	MaxWalkSpeedCrouched = 1150.0f;
	MaxAcceleration = 6000.0f;
	BrakingDecelerationWalking = 5200.0f;
	GroundFriction = 8.0f;
	AirControl = 0.65f;
	FallingLateralFriction = 0.15f;
	JumpZVelocity = 1250.0f;

	NavAgentProps.bCanCrouch = true;
	SetPlaneConstraintEnabled(true);
	SetPlaneConstraintNormal(FVector::YAxisVector);
	bSnapToPlaneAtStart = true;
}

float UKyokaiMovementComponent::GetMaxSpeed() const
{
	if (bSliding && IsMovingOnGround())
	{
		return SlideMaxSpeed;
	}

	return Super::GetMaxSpeed();
}

void UKyokaiMovementComponent::SetSliding(const bool bEnabled)
{
	bSliding = bEnabled;
}
