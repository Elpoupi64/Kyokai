// Copyright Epic Games, Inc. All Rights Reserved.

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/GameModeBase.h"
#include "TimerManager.h"
#include "KyokaiGameMode.generated.h"

class AKyokaiCharacter;
class APlayerController;

UCLASS()
class KYOKAI_API AKyokaiGameMode : public AGameModeBase
{
	GENERATED_BODY()

public:
	AKyokaiGameMode();

protected:
	virtual void BeginPlay() override;

private:
	/**
	 * Optional headless functional check for the fallback keyboard bindings
	 * (A/D, Space, LeftControl, LeftShift, F1). Enabled only with
	 * -KyokaiInputSmokeTest on the command line: it simulates real key
	 * events through the PlayerController, which exercises the actual
	 * DefaultInput.ini mappings (not a shortcut call into the character's
	 * handler functions), then writes a JSON report to
	 * Saved/InputSmokeTest.json and quits the process.
	 */
	void TryStartInputSmokeTest();
	void PollForPawnThenRunSmokeTest();
	void RunSmokeTestStep(int32 StepIndex);
	void FinishInputSmokeTest(const FString& Outcome);

	FTimerHandle SmokeTestPollHandle;
	FTimerHandle SmokeTestStepHandle;
	TWeakObjectPtr<AKyokaiCharacter> SmokeTestCharacter;
	TWeakObjectPtr<APlayerController> SmokeTestController;
	TArray<FString> SmokeTestEntries;
	FVector SmokeTestRefLocation = FVector::ZeroVector;
	int32 SmokeTestPollAttempts = 0;

	/**
	 * Dedicated wall-jump check for the L_ControllerGym shaft (Zone 6a).
	 * Enabled only with -KyokaiWallJumpTest: teleports the pawn to a
	 * controlled spot next to Wall_Zone6_Left instead of relying on the
	 * generic smoke test's fixed multi-step choreography to happen to line
	 * up with being near a wall (it doesn't - the shaft has no floor and
	 * free-falls past it well before the generic test presses jump). Writes
	 * Saved/WallJumpSmokeTest.json.
	 */
	void TryStartWallJumpTest();
	void PollForPawnThenRunWallJumpTest();
	void RunWallJumpTestStep(int32 StepIndex);
	void FinishWallJumpTest(const FString& Outcome);

	FTimerHandle WallJumpTestPollHandle;
	FTimerHandle WallJumpTestStepHandle;
	TWeakObjectPtr<AKyokaiCharacter> WallJumpTestCharacter;
	TWeakObjectPtr<APlayerController> WallJumpTestController;
	TArray<FString> WallJumpTestEntries;
	int32 WallJumpTestPollAttempts = 0;

	/**
	 * Dedicated bounce-pad check for BouncePad_Zone6. Enabled only with
	 * -KyokaiBounceTest: teleports the pawn above the pad and lets it fall
	 * onto the overlap trigger, then samples velocity twice afterward to
	 * confirm it's a real sustained upward launch (not, say, a one-frame
	 * blip that gravity immediately erases). Writes Saved/BounceSmokeTest.json.
	 */
	void TryStartBounceTest();
	void PollForPawnThenRunBounceTest();
	void RunBounceTestStep(int32 StepIndex);
	void FinishBounceTest(const FString& Outcome);

	FTimerHandle BounceTestPollHandle;
	FTimerHandle BounceTestStepHandle;
	TWeakObjectPtr<AKyokaiCharacter> BounceTestCharacter;
	TArray<FString> BounceTestEntries;
	int32 BounceTestPollAttempts = 0;

	/**
	 * Dedicated Zone 7 drop-crossing check (Zone 7 was redesigned around
	 * drops rather than flat gaps after -KyokaiGapTest showed a plain held
	 * jump outperforms jump+dash on a flat same-height crossing here - see
	 * kyokai-prototype-state memory for the full story). Enabled only with
	 * -KyokaiDropTest: teleports the pawn just past Ledge C's edge (the
	 * hardest of the three drops) already falling, and tries it two ways -
	 * no dash, then (after resetting) dashing immediately on leaving the
	 * ledge - to confirm the drop is real airtime a plain fall can't cross
	 * but an immediate dash can. Writes Saved/DropSmokeTest.json.
	 */
	void TryStartDropTest();
	void PollForPawnThenRunDropTest();
	void RunDropTestStep(int32 StepIndex);
	void FinishDropTest(const FString& Outcome);

	FTimerHandle DropTestPollHandle;
	FTimerHandle DropTestStepHandle;
	TWeakObjectPtr<AKyokaiCharacter> DropTestCharacter;
	TWeakObjectPtr<APlayerController> DropTestController;
	TArray<FString> DropTestEntries;
	int32 DropTestPollAttempts = 0;

	/**
	 * Level 02 ("Les Toits sous la pluie") main-path timing bot. Enabled
	 * only with -KyokaiLevel02Timing: holds D for the whole run and fires
	 * jump/slide/dash taps reactively as the pawn's X position crosses
	 * known obstacle thresholds (computed from the level's own build
	 * coordinates), plus a periodic jump tap while inside the wall-jump
	 * shaft's X range and below its exit height. This is a mechanical
	 * traversal time, not a human playtest - build-order step 3 ("time the
	 * level without enemies") explicitly wants this as an early pacing
	 * sanity check before step 7's real 5-player test. Writes
	 * Saved/Level02TimingReport.json with the total time and a per-segment
	 * split (segment boundaries are the X thresholds used to build the
	 * level's SegmentMarker_N_End platforms).
	 */
	void TryStartLevel02Timing();
	void PollForPawnThenRunLevel02Timing();
	void TickLevel02Timing();
	void FinishLevel02Timing(const FString& Outcome);

	FTimerHandle Level02TimingPollHandle;
	FTimerHandle Level02TimingTickHandle;
	TWeakObjectPtr<AKyokaiCharacter> Level02TimingCharacter;
	TWeakObjectPtr<APlayerController> Level02TimingController;
	TArray<FString> Level02TimingEntries;
	int32 Level02TimingPollAttempts = 0;
	float Level02TimingStartTime = 0.0f;
	int32 Level02TimingNextTrigger = 0;
	int32 Level02TimingNextSegment = 0;
	float Level02TimingLastShaftPress = -1000.0f;
	bool bLevel02TimingSliding = false;
	bool bLevel02TimingDHeld = true;
};
