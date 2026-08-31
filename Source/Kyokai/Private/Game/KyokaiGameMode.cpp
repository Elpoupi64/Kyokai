// Copyright Epic Games, Inc. All Rights Reserved.

#include "Game/KyokaiGameMode.h"

#include "Characters/KyokaiCharacter.h"
#include "Characters/KyokaiMovementComponent.h"
#include "GameFramework/PlayerController.h"
#include "HAL/PlatformMisc.h"
#include "InputCoreTypes.h"
#include "InputKeyEventArgs.h"
#include "Misc/CommandLine.h"
#include "Misc/FileHelper.h"
#include "Misc/Paths.h"
#include "TimerManager.h"

AKyokaiGameMode::AKyokaiGameMode()
{
	DefaultPawnClass = AKyokaiCharacter::StaticClass();
}

void AKyokaiGameMode::BeginPlay()
{
	Super::BeginPlay();
	TryStartInputSmokeTest();
}

void AKyokaiGameMode::TryStartInputSmokeTest()
{
	if (!FParse::Param(FCommandLine::Get(), TEXT("KyokaiInputSmokeTest")))
	{
		return;
	}

	SmokeTestEntries.Reset();
	SmokeTestPollAttempts = 0;
	GetWorldTimerManager().SetTimer(SmokeTestPollHandle, this, &AKyokaiGameMode::PollForPawnThenRunSmokeTest, 0.2f, true);
}

void AKyokaiGameMode::PollForPawnThenRunSmokeTest()
{
	++SmokeTestPollAttempts;

	APlayerController* PC = GetWorld() ? GetWorld()->GetFirstPlayerController() : nullptr;
	AKyokaiCharacter* Character = PC ? Cast<AKyokaiCharacter>(PC->GetPawn()) : nullptr;

	if (!Character && SmokeTestPollAttempts < 25) // ~5s at 0.2s intervals
	{
		return;
	}

	GetWorldTimerManager().ClearTimer(SmokeTestPollHandle);

	if (!Character)
	{
		FinishInputSmokeTest(TEXT("no pawn possessed within timeout"));
		return;
	}

	SmokeTestController = PC;
	SmokeTestCharacter = Character;
	RunSmokeTestStep(0);
}

void AKyokaiGameMode::RunSmokeTestStep(const int32 StepIndex)
{
	APlayerController* PC = SmokeTestController.Get();
	AKyokaiCharacter* Character = SmokeTestCharacter.Get();
	if (!PC || !Character)
	{
		FinishInputSmokeTest(TEXT("pawn or controller became invalid mid-test"));
		return;
	}

	UKyokaiMovementComponent* Movement = Character->GetKyokaiMovement();
	float NextDelay = 0.3f;

	switch (StepIndex)
	{
	case 0:
		SmokeTestRefLocation = Character->GetActorLocation();
		SmokeTestEntries.Add(FString::Printf(
			TEXT("{\"step\": \"start\", \"location_x\": %.2f}"), SmokeTestRefLocation.X));
		PC->InputKey(FInputKeyEventArgs::CreateSimulated(EKeys::D, IE_Pressed, 1.0f));
		NextDelay = 0.2f;
		break;

	case 1:
		// Sample velocity while D is still held, before releasing - a
		// direct cause/effect reading that isn't diluted by whatever
		// happens over a longer accumulate-then-read window.
		SmokeTestEntries.Add(FString::Printf(
			TEXT("{\"step\": \"holding_D\", \"location_x\": %.2f, \"delta_x\": %.2f, \"velocity_x\": %.2f}"),
			Character->GetActorLocation().X, Character->GetActorLocation().X - SmokeTestRefLocation.X,
			Character->GetVelocity().X));
		PC->InputKey(FInputKeyEventArgs::CreateSimulated(EKeys::D, IE_Released, 0.0f));
		SmokeTestRefLocation = Character->GetActorLocation();
		PC->InputKey(FInputKeyEventArgs::CreateSimulated(EKeys::A, IE_Pressed, 1.0f));
		NextDelay = 0.2f;
		break;

	case 2:
		SmokeTestEntries.Add(FString::Printf(
			TEXT("{\"step\": \"holding_A\", \"location_x\": %.2f, \"delta_x\": %.2f, \"velocity_x\": %.2f}"),
			Character->GetActorLocation().X, Character->GetActorLocation().X - SmokeTestRefLocation.X,
			Character->GetVelocity().X));
		PC->InputKey(FInputKeyEventArgs::CreateSimulated(EKeys::A, IE_Released, 0.0f));
		NextDelay = 0.3f;
		break;

	case 3:
		SmokeTestEntries.Add(FString::Printf(
			TEXT("{\"step\": \"before_jump\", \"is_grounded\": %s}"),
			Movement && Movement->IsMovingOnGround() ? TEXT("true") : TEXT("false")));
		PC->InputKey(FInputKeyEventArgs::CreateSimulated(EKeys::SpaceBar, IE_Pressed, 1.0f));
		NextDelay = 0.05f;
		break;

	case 4:
		SmokeTestEntries.Add(FString::Printf(
			TEXT("{\"step\": \"jump_ascending\", \"velocity_z\": %.2f}"), Character->GetVelocity().Z));
		PC->InputKey(FInputKeyEventArgs::CreateSimulated(EKeys::SpaceBar, IE_Released, 0.0f));
		NextDelay = 0.05f;
		break;

	case 5:
		SmokeTestEntries.Add(FString::Printf(
			TEXT("{\"step\": \"jump_after_release\", \"velocity_z\": %.2f}"), Character->GetVelocity().Z));
		NextDelay = 1.0f; // generous margin so the character is back on the ground by the next step
		break;

	case 6:
		SmokeTestEntries.Add(FString::Printf(
			TEXT("{\"step\": \"landed\", \"is_grounded\": %s}"),
			Movement && Movement->IsMovingOnGround() ? TEXT("true") : TEXT("false")));
		PC->InputKey(FInputKeyEventArgs::CreateSimulated(EKeys::D, IE_Pressed, 1.0f));
		NextDelay = 0.8f;
		break;

	case 7:
		SmokeTestEntries.Add(FString::Printf(
			TEXT("{\"step\": \"before_slide\", \"speed\": %.2f, \"is_grounded\": %s}"),
			Character->GetVelocity().Size2D(), Movement && Movement->IsMovingOnGround() ? TEXT("true") : TEXT("false")));
		PC->InputKey(FInputKeyEventArgs::CreateSimulated(EKeys::LeftControl, IE_Pressed, 1.0f));
		NextDelay = 0.05f;
		break;

	case 8:
		SmokeTestEntries.Add(FString::Printf(
			TEXT("{\"step\": \"slide_pressed\", \"is_sliding\": %s}"),
			Character->IsSliding() ? TEXT("true") : TEXT("false")));
		NextDelay = 0.3f;
		break;

	case 9:
		PC->InputKey(FInputKeyEventArgs::CreateSimulated(EKeys::LeftControl, IE_Released, 0.0f));
		PC->InputKey(FInputKeyEventArgs::CreateSimulated(EKeys::D, IE_Released, 0.0f));
		NextDelay = 0.05f;
		break;

	case 10:
		// A separate step from the release above: IsSliding() reflects
		// OnSlideReleased(), which only runs once the input system's next
		// tick dispatches the release event, not the instant InputKey() is
		// called - reading it in the same step as the release would race
		// the dispatch and read stale (still-sliding) state.
		SmokeTestEntries.Add(FString::Printf(
			TEXT("{\"step\": \"slide_released\", \"is_sliding\": %s}"),
			Character->IsSliding() ? TEXT("true") : TEXT("false")));
		NextDelay = 0.35f;
		break;

	case 11:
		PC->InputKey(FInputKeyEventArgs::CreateSimulated(EKeys::D, IE_Pressed, 1.0f));
		NextDelay = 0.3f;
		break;

	case 12:
		SmokeTestEntries.Add(FString::Printf(
			TEXT("{\"step\": \"before_dash\", \"speed\": %.2f}"), Character->GetVelocity().Size2D()));
		PC->InputKey(FInputKeyEventArgs::CreateSimulated(EKeys::LeftShift, IE_Pressed, 1.0f));
		NextDelay = 0.05f;
		break;

	case 13:
		SmokeTestEntries.Add(FString::Printf(
			TEXT("{\"step\": \"dash_pressed\", \"is_dashing\": %s, \"velocity_x\": %.2f}"),
			Character->IsDashing() ? TEXT("true") : TEXT("false"), Character->GetVelocity().X));
		NextDelay = 0.3f;
		break;

	case 14:
		PC->InputKey(FInputKeyEventArgs::CreateSimulated(EKeys::LeftShift, IE_Released, 0.0f));
		PC->InputKey(FInputKeyEventArgs::CreateSimulated(EKeys::D, IE_Released, 0.0f));
		SmokeTestEntries.Add(FString::Printf(
			TEXT("{\"step\": \"dash_ended\", \"is_dashing\": %s}"),
			Character->IsDashing() ? TEXT("true") : TEXT("false")));
		NextDelay = 0.2f;
		break;

	case 15:
		SmokeTestEntries.Add(FString::Printf(
			TEXT("{\"step\": \"before_f1\", \"debug_visible\": %s}"),
			Character->IsPrototypeDebugVisible() ? TEXT("true") : TEXT("false")));
		PC->InputKey(FInputKeyEventArgs::CreateSimulated(EKeys::F1, IE_Pressed, 1.0f));
		NextDelay = 0.05f;
		break;

	case 16:
		PC->InputKey(FInputKeyEventArgs::CreateSimulated(EKeys::F1, IE_Released, 0.0f));
		SmokeTestEntries.Add(FString::Printf(
			TEXT("{\"step\": \"after_first_f1\", \"debug_visible\": %s}"),
			Character->IsPrototypeDebugVisible() ? TEXT("true") : TEXT("false")));
		PC->InputKey(FInputKeyEventArgs::CreateSimulated(EKeys::F1, IE_Pressed, 1.0f));
		NextDelay = 0.05f;
		break;

	case 17:
		PC->InputKey(FInputKeyEventArgs::CreateSimulated(EKeys::F1, IE_Released, 0.0f));
		SmokeTestEntries.Add(FString::Printf(
			TEXT("{\"step\": \"after_second_f1\", \"debug_visible\": %s}"),
			Character->IsPrototypeDebugVisible() ? TEXT("true") : TEXT("false")));
		FinishInputSmokeTest(TEXT("completed"));
		return;

	default:
		FinishInputSmokeTest(TEXT("completed"));
		return;
	}

	GetWorldTimerManager().SetTimer(
		SmokeTestStepHandle,
		FTimerDelegate::CreateUObject(this, &AKyokaiGameMode::RunSmokeTestStep, StepIndex + 1),
		NextDelay,
		false);
}

void AKyokaiGameMode::FinishInputSmokeTest(const FString& Outcome)
{
	FString Json = TEXT("{\n  \"outcome\": \"");
	Json += Outcome;
	Json += TEXT("\",\n  \"steps\": [\n");
	for (int32 Index = 0; Index < SmokeTestEntries.Num(); ++Index)
	{
		Json += TEXT("    ");
		Json += SmokeTestEntries[Index];
		Json += (Index + 1 < SmokeTestEntries.Num()) ? TEXT(",\n") : TEXT("\n");
	}
	Json += TEXT("  ]\n}\n");

	const FString OutPath = FPaths::ProjectSavedDir() / TEXT("InputSmokeTest.json");
	FFileHelper::SaveStringToFile(Json, *OutPath);

	FGenericPlatformMisc::RequestExit(false);
}
