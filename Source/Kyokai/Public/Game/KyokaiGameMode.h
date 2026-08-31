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
};
