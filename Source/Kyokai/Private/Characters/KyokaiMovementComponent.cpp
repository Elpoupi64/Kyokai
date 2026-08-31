// Copyright Epic Games, Inc. All Rights Reserved.

#include "Characters/KyokaiMovementComponent.h"
#include "PhysicalMaterials/PhysicalMaterial.h"
#include "Components/CapsuleComponent.h"
#include "GameFramework/Character.h"

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

	BaseGroundFriction = GroundFriction;
	BaseBrakingDecelerationWalking = BrakingDecelerationWalking;

	NavAgentProps.bCanCrouch = true;
	SetPlaneConstraintEnabled(true);
	SetPlaneConstraintNormal(FVector::YAxisVector);
	bSnapToPlaneAtStart = true;
}

void UKyokaiMovementComponent::TickComponent(const float DeltaTime, const ELevelTick TickType, FActorComponentTickFunction* ThisTickFunction)
{
	float SurfaceFrictionScale = 1.0f;
	// CurrentFloor.HitResult.PhysMaterial is never populated - the engine's
	// own floor sweep (ComputeFloorDist's downward sweep, ultimately fed by
	// MovementComponent's internal MoveAlongFloor calls) explicitly sets
	// bReturnPhysicalMaterial = false for performance. A dedicated trace is
	// the only reliable way to read the current floor's PhysicalMaterial.
	if (IsMovingOnGround() && CharacterOwner)
	{
		const UCapsuleComponent* Capsule = CharacterOwner->GetCapsuleComponent();
		const float HalfHeight = Capsule ? Capsule->GetScaledCapsuleHalfHeight() : 0.0f;
		const FVector Start = CharacterOwner->GetActorLocation();
		const FVector End = Start - FVector(0.0f, 0.0f, HalfHeight + 20.0f);

		FCollisionQueryParams QueryParams(SCENE_QUERY_STAT(KyokaiSurfaceFrictionTrace), false, CharacterOwner);
		QueryParams.bReturnPhysicalMaterial = true;

		FHitResult Hit;
		if (GetWorld()->LineTraceSingleByChannel(Hit, Start, End, ECC_Visibility, QueryParams))
		{
			if (const UPhysicalMaterial* PhysMat = Hit.PhysMaterial.Get())
			{
				SurfaceFrictionScale = FMath::Max(0.05f, PhysMat->Friction / ReferenceSurfaceFriction);
			}
		}
	}
	GroundFriction = BaseGroundFriction * SurfaceFrictionScale;
	BrakingDecelerationWalking = BaseBrakingDecelerationWalking * SurfaceFrictionScale;

	Super::TickComponent(DeltaTime, TickType, ThisTickFunction);
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
