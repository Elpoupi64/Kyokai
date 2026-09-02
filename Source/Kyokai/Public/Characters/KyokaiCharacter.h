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

	/**
	 * Pression (GDD "flow resource" - [[kyokai-project-overview]]): 2
	 * charges, fills via continuous movement/bounces/in-line collection,
	 * decays after 2s idle. Dash costs 1 charge. "Main path is always
	 * clearable without maintaining a full gauge; expert routes spend it
	 * for shortcuts/secrets" - a single dash (the finale's dash-drop)
	 * only ever needs 1 of the 2 charges, and continuous running refills
	 * fast enough that reaching the finale with less than a full charge
	 * shouldn't happen in practice; the expert route's own dash-chains
	 * (e.g. the mastery seal's ruée->rebond->ruée) are exactly the case
	 * this is meant to gate - the mid-chain bounce's Pression bonus is
	 * what makes affording the second dash possible.
	 *
	 * Not yet wired to the "ligne de rivets de cuivre" the level brief
	 * describes powering the expert route (no distinct copper-rivet pickup
	 * exists) - bounces and pickups already on the expert route serve
	 * that role for now; a dedicated rivet actor is future work if the
	 * generic fill sources prove insufficient.
	 */
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Kyokai|Pression", meta = (ClampMin = "0.1"))
	float MaxPressionCharges = 2.0f;

	UPROPERTY(BlueprintReadOnly, Category = "Kyokai|Pression")
	float CurrentPression = 2.0f;

	/** Continuous-movement fill rate (charges/sec) - 0.5 fills empty-to-full in 4s. */
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Kyokai|Pression", meta = (ClampMin = "0.0"))
	float PressionMoveFillRate = 0.5f;

	/** Flat bonus on a successful bounce ("bounces" in the GDD's fill-source list). */
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Kyokai|Pression", meta = (ClampMin = "0.0"))
	float PressionBounceBonus = 0.5f;

	/** Flat bonus on collecting a pickup ("in-line collection"). */
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Kyokai|Pression", meta = (ClampMin = "0.0"))
	float PressionPickupBonus = 0.3f;

	/** How long the character must be idle (not moving horizontally) before Pression starts decaying. */
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Kyokai|Pression", meta = (ClampMin = "0.0"))
	float PressionIdleDecayDelay = 2.0f;

	/** Decay rate once idle (charges/sec) - drains a full gauge in 2s once decay starts. */
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Kyokai|Pression", meta = (ClampMin = "0.0"))
	float PressionIdleDecayRate = 1.0f;

	/** Charges a single dash consumes. */
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Kyokai|Pression", meta = (ClampMin = "0.0"))
	float PressionDashCost = 1.0f;

	/** Adds Pression (clamped to MaxPressionCharges) - called by bounces and pickups. */
	UFUNCTION(BlueprintCallable, Category = "Kyokai|Pression")
	void AddPression(float Amount);

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

	/** Base vertical framing offset while grounded - shifts the camera's focus point above the character for run-time headroom (spotting an upcoming jump). */
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Kyokai|Camera", meta = (ClampMin = "0.0"))
	float CameraGroundedVerticalOffset = 140.0f;

	/**
	 * How much lower the camera's vertical focus drops while airborne, on
	 * top of CameraGroundedVerticalOffset - the same fixed +140 offset
	 * used while grounded means every jump pushes platforms below even
	 * further out of frame, confirmed as a real playability problem (not
	 * just polish): the GDD's own acceptance criteria include "no death
	 * caused by a hazard that was off-camera", and this level uses jump
	 * arcs constantly. Net vertical offset while airborne is
	 * CameraGroundedVerticalOffset - CameraAirborneVerticalDrop.
	 */
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Kyokai|Camera", meta = (ClampMin = "0.0"))
	float CameraAirborneVerticalDrop = 260.0f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Kyokai|Camera", meta = (ClampMin = "0.0"))
	float CameraVerticalInterpSpeed = 4.0f;

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

	/** Seconds since horizontal velocity last exceeded the idle deadzone - drives Pression's idle decay. */
	float PressionIdleTimer = 0.0f;

	FTimerHandle DashTimerHandle;
};
