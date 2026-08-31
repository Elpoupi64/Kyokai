// Copyright Epic Games, Inc. All Rights Reserved.

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Character.h"
#include "InputActionValue.h"
#include "TimerManager.h"
#include "KyokaiCharacter.generated.h"

class UCameraComponent;
class UInputAction;
class UInputMappingContext;
class UKyokaiMovementComponent;
class USpringArmComponent;
class UStaticMeshComponent;

/**
 * First playable controller for Kyokai.
 *
 * It is intentionally art-agnostic and can be used directly as a capsule or
 * subclassed as BP_AikoPrototype. Input Action assets are optional: when they
 * are not assigned, the character uses the classic mappings shipped in
 * DefaultInput.ini so the empty project remains testable.
 */
UCLASS(Blueprintable)
class KYOKAI_API AKyokaiCharacter : public ACharacter
{
	GENERATED_BODY()

public:
	AKyokaiCharacter(const FObjectInitializer& ObjectInitializer);

	virtual void Tick(float DeltaSeconds) override;
	virtual void SetupPlayerInputComponent(UInputComponent* PlayerInputComponent) override;
	virtual void Landed(const FHitResult& Hit) override;

	UFUNCTION(BlueprintPure, Category = "Kyokai|Movement")
	UKyokaiMovementComponent* GetKyokaiMovement() const;

	UFUNCTION(BlueprintPure, Category = "Kyokai|Movement")
	bool IsSliding() const { return bIsSliding; }

	UFUNCTION(BlueprintPure, Category = "Kyokai|Movement")
	bool IsDashing() const { return bIsDashing; }

	UFUNCTION(BlueprintPure, Category = "Kyokai|Debug")
	bool IsPrototypeDebugVisible() const { return bShowPrototypeDebug; }

	UFUNCTION(BlueprintPure, Category = "Kyokai|Movement")
	bool IsTouchingWall() const { return bIsTouchingWall; }

protected:
	virtual void BeginPlay() override;
	virtual bool CanJumpInternal_Implementation() const override;

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category = "Kyokai|Camera")
	TObjectPtr<USpringArmComponent> CameraBoom;

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category = "Kyokai|Camera")
	TObjectPtr<UCameraComponent> SideViewCamera;

	/**
	 * No-art-yet placeholder: a simple cube sized to the capsule so the
	 * character is actually visible in PIE. Matches the project's existing
	 * "placeholder cube" convention until a real mesh/BP_AikoPrototype
	 * exists. Purely visual - NoCollision, the capsule keeps handling
	 * movement collision.
	 */
	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category = "Kyokai|Visual")
	TObjectPtr<UStaticMeshComponent> BodyMesh;

	UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Category = "Kyokai|Input")
	TObjectPtr<UInputMappingContext> GameplayMappingContext;

	UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Category = "Kyokai|Input")
	TObjectPtr<UInputAction> MoveAction;

	UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Category = "Kyokai|Input")
	TObjectPtr<UInputAction> JumpAction;

	UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Category = "Kyokai|Input")
	TObjectPtr<UInputAction> SlideAction;

	UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Category = "Kyokai|Input")
	TObjectPtr<UInputAction> DashAction;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Kyokai|Movement|Jump", meta = (ClampMin = "0.0", ClampMax = "0.5"))
	float CoyoteTime = 0.12f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Kyokai|Movement|Jump", meta = (ClampMin = "0.0", ClampMax = "0.5"))
	float JumpBufferTime = 0.12f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Kyokai|Movement|Slide", meta = (ClampMin = "0.0"))
	float MinimumSlideSpeed = 450.0f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Kyokai|Movement|Slide", meta = (ClampMin = "0.0"))
	float SlideEntrySpeed = 1100.0f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Kyokai|Movement|Dash", meta = (ClampMin = "0.0"))
	float DashSpeed = 1500.0f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Kyokai|Movement|Dash", meta = (ClampMin = "0.01", ClampMax = "1.0"))
	float DashDuration = 0.18f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Kyokai|Movement|Dash", meta = (ClampMin = "0.0"))
	float DashCooldown = 0.35f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Kyokai|Camera", meta = (ClampMin = "0.0"))
	float CameraLookAheadDistance = 220.0f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Kyokai|Camera", meta = (ClampMin = "0.0"))
	float CameraLookAheadSpeed = 7.0f;

	/**
	 * Distance beyond the capsule radius the side wall-check traces reach -
	 * how close a wall has to be, while airborne, to count as "touching" it
	 * for a wall jump.
	 */
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Kyokai|Movement|WallJump", meta = (ClampMin = "0.0"))
	float WallCheckDistance = 60.0f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Kyokai|Movement|WallJump", meta = (ClampMin = "0.0"))
	float WallJumpZVelocity = 1100.0f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Kyokai|Movement|WallJump", meta = (ClampMin = "0.0"))
	float WallJumpHorizontalVelocity = 800.0f;

private:
	void MoveEnhanced(const FInputActionValue& Value);
	void MoveLegacy(float Value);
	void ApplyMoveInput(float Value);
	void OnJumpPressed();
	void OnJumpReleased();
	void OnSlidePressed();
	void OnSlideReleased();
	void OnDashPressed();
	void StopDash();
	void TogglePrototypeDebug();
	void TryConsumeJumpBuffer();
	void UpdateCameraLookAhead(float DeltaSeconds);
	void UpdateWallDetection();
	void PerformWallJump();
	void DrawPrototypeDebug() const;

	float LastGroundedTime = -1000.0f;
	float LastJumpPressedTime = -1000.0f;
	float LastDashTime = -1000.0f;
	float MoveInput = 0.0f;
	float FacingDirection = 1.0f;
	float WallPushDirection = 0.0f;
	bool bJumpBuffered = false;
	bool bIsSliding = false;
	bool bIsDashing = false;
	bool bAirDashAvailable = true;
	bool bShowPrototypeDebug = true;
	bool bIsTouchingWall = false;

	FTimerHandle DashTimerHandle;
};
