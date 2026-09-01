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

	/**
	 * Teleports back to AKyokaiGameMode::GetRespawnLocation() (the last
	 * checkpoint reached, or the level's PlayerStart if none has been
	 * activated yet) and stops all momentum. Used by the fall-catch in
	 * Tick() and by hazard contact (lightning, enemies) - see
	 * AKyokaiEnemyBase's header for why hazards use this instead of a
	 * knockback now that a real checkpoint system exists.
	 *
	 * Cause is reported to AKyokaiGameMode::NotifyPlayerDeath() before
	 * teleporting, for the build-order step 7 playtest instrumentation
	 * (the brief's own acceptance criterion "no single obstacle causes
	 * more than 20% of failures" needs a per-cause tally) - pass a short
	 * identifier ("fall", "lightning", "onibi", "bakeneko", ...).
	 */
	UFUNCTION(BlueprintCallable, Category = "Kyokai|Checkpoint")
	void RespawnAtCheckpoint(const FString& Cause = TEXT("unknown"));

	/**
	 * Real integrity system per the GDD (rule 8.2 / [[kyokai-project-overview]]):
	 * MaxIntegritySegments segments, IFrameDuration of invulnerability per
	 * hit, most hits cost 1 segment and knock back rather than a full
	 * reset - only reaching 0 triggers RespawnAtCheckpoint() (which also
	 * refills segments back to max, matching "instant respawn at 0 HP...
	 * no lives system").
	 *
	 * Replaces the earlier stopgap where every hazard (Onibi, Bakeneko,
	 * lightning, falling off the level) called RespawnAtCheckpoint()
	 * directly on any contact - that was flagged from the start as
	 * temporary pending a real checkpoint/health system, both of which
	 * now exist.
	 *
	 * Returns true if the hit actually applied (not absorbed by i-frames,
	 * and integrity was still above 0 going in) - callers should follow up
	 * with their own knockback/consequence in that case. Returns false if
	 * it was absorbed by i-frames, or if this hit brought integrity to 0
	 * (RespawnAtCheckpoint() already ran internally) - callers should do
	 * nothing further either way.
	 */
	UFUNCTION(BlueprintCallable, Category = "Kyokai|Integrity")
	bool ApplyHazardHit(const FString& Cause);

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Kyokai|Integrity", meta = (ClampMin = "1"))
	int32 MaxIntegritySegments = 3;

	UPROPERTY(BlueprintReadOnly, Category = "Kyokai|Integrity")
	int32 CurrentIntegritySegments = 3;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Kyokai|Integrity", meta = (ClampMin = "0.0"))
	float IFrameDuration = 1.0f;

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

	/** Counts down from IFrameDuration after a hit; ApplyHazardHit() no-ops while > 0. */
	float IFrameTimer = 0.0f;

	/**
	 * Continuously updated while grounded - where a "fall" hit (Tick()'s
	 * Z<-600 catch) recovers to instead of a full checkpoint teleport, per
	 * the GDD's "falls send you back to the last safe ground (cost 1
	 * segment, not a full restart)". Starts at the spawn location so an
	 * implausible same-tick fall has somewhere sane to land.
	 */
	FVector LastSafeGroundLocation = FVector::ZeroVector;

	FTimerHandle DashTimerHandle;
};
