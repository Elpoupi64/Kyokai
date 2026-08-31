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
#include "UObject/ConstructorHelpers.h"

AKyokaiGameMode::AKyokaiGameMode()
{
	// Prefer BP_AikoPrototype (child of AKyokaiCharacter) so movement/jump/
	// slide/dash values are tunable from the Blueprint's Class Defaults
	// without recompiling - falls back to the raw C++ class if the
	// Blueprint hasn't been created yet (e.g. a fresh checkout).
	static ConstructorHelpers::FClassFinder<AKyokaiCharacter> AikoPrototypeFinder(
		TEXT("/Game/Blueprints/Characters/BP_AikoPrototype"));
	if (AikoPrototypeFinder.Succeeded())
	{
		DefaultPawnClass = AikoPrototypeFinder.Class;
	}
	else
	{
		DefaultPawnClass = AKyokaiCharacter::StaticClass();
	}
}

void AKyokaiGameMode::BeginPlay()
{
	Super::BeginPlay();
	TryStartInputSmokeTest();
	TryStartWallJumpTest();
	TryStartBounceTest();
	TryStartDropTest();
	TryStartLevel02Timing();
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

void AKyokaiGameMode::TryStartWallJumpTest()
{
	if (!FParse::Param(FCommandLine::Get(), TEXT("KyokaiWallJumpTest")))
	{
		return;
	}

	WallJumpTestEntries.Reset();
	WallJumpTestPollAttempts = 0;
	GetWorldTimerManager().SetTimer(
		WallJumpTestPollHandle, this, &AKyokaiGameMode::PollForPawnThenRunWallJumpTest, 0.2f, true);
}

void AKyokaiGameMode::PollForPawnThenRunWallJumpTest()
{
	++WallJumpTestPollAttempts;

	APlayerController* PC = GetWorld() ? GetWorld()->GetFirstPlayerController() : nullptr;
	AKyokaiCharacter* Character = PC ? Cast<AKyokaiCharacter>(PC->GetPawn()) : nullptr;

	if (!Character && WallJumpTestPollAttempts < 25) // ~5s at 0.2s intervals
	{
		return;
	}

	GetWorldTimerManager().ClearTimer(WallJumpTestPollHandle);

	if (!Character)
	{
		FinishWallJumpTest(TEXT("no pawn possessed within timeout"));
		return;
	}

	WallJumpTestController = PC;
	WallJumpTestCharacter = Character;
	RunWallJumpTestStep(0);
}

void AKyokaiGameMode::RunWallJumpTestStep(const int32 StepIndex)
{
	APlayerController* PC = WallJumpTestController.Get();
	AKyokaiCharacter* Character = WallJumpTestCharacter.Get();
	if (!PC || !Character)
	{
		FinishWallJumpTest(TEXT("pawn or controller became invalid mid-test"));
		return;
	}

	UKyokaiMovementComponent* Movement = Character->GetKyokaiMovement();
	float NextDelay = 0.1f;

	// L_ControllerGym Zone 6a: Wall_Zone6_Left spans X=9150-9250, interior
	// gap up to X=9410 (Wall_Zone6_Right). Teleporting next to the left wall
	// instead of walking a scripted character in from the course avoids the
	// generic InputSmokeTest's problem here: the shaft interior has no
	// floor, so a character that starts falling from anywhere else reaches
	// the bottom (800cm down) well before a fixed step sequence gets to
	// pressing jump.
	// Wall_Zone6_Left's interior face is at X=9250; capsule radius is 42cm,
	// so the capsule must start at X>=9292 to not spawn embedded in the
	// wall (the first version of this test started at 9270 - inside the
	// wall by 22cm - and the resulting depenetration push, not a real wall
	// jump, was what got measured for the first jump).
	constexpr float StartX = 9300.0f;
	constexpr float StartZ = 200.0f;

	switch (StepIndex)
	{
	case 0:
		Character->SetActorLocation(FVector(StartX, 0.0f, StartZ), false, nullptr, ETeleportType::TeleportPhysics);
		if (Movement)
		{
			Movement->Velocity = FVector::ZeroVector;
			Movement->SetMovementMode(MOVE_Falling);
		}
		WallJumpTestEntries.Add(FString::Printf(
			TEXT("{\"step\": \"teleported\", \"location_x\": %.2f, \"location_z\": %.2f}"), StartX, StartZ));
		// Long enough for UpdateWallDetection to register the wall AND for
		// coyote time to expire from whatever ground contact happened right
		// before this teleport (e.g. the moment the pawn was first
		// possessed at its real PlayerStart) - otherwise the first jump
		// legitimately (and correctly) takes the coyote-time path instead
		// of the wall-jump path this test wants to exercise.
		NextDelay = 0.3f;
		break;

	case 1:
		WallJumpTestEntries.Add(FString::Printf(
			TEXT("{\"step\": \"before_1st_jump\", \"is_touching_wall\": %s, \"location_x\": %.2f}"),
			Character->IsTouchingWall() ? TEXT("true") : TEXT("false"), Character->GetActorLocation().X));
		PC->InputKey(FInputKeyEventArgs::CreateSimulated(EKeys::SpaceBar, IE_Pressed, 1.0f));
		NextDelay = 0.05f;
		break;

	case 2:
		WallJumpTestEntries.Add(FString::Printf(
			TEXT("{\"step\": \"after_1st_jump\", \"velocity_x\": %.2f, \"velocity_z\": %.2f}"),
			Character->GetVelocity().X, Character->GetVelocity().Z));
		PC->InputKey(FInputKeyEventArgs::CreateSimulated(EKeys::SpaceBar, IE_Released, 0.0f));
		NextDelay = 0.15f; // 160cm gap at ~800cm/s horizontal - enough time to reach the right wall
		break;

	case 3:
		WallJumpTestEntries.Add(FString::Printf(
			TEXT("{\"step\": \"before_2nd_jump\", \"is_touching_wall\": %s, \"location_x\": %.2f, \"location_z\": %.2f}"),
			Character->IsTouchingWall() ? TEXT("true") : TEXT("false"),
			Character->GetActorLocation().X, Character->GetActorLocation().Z));
		PC->InputKey(FInputKeyEventArgs::CreateSimulated(EKeys::SpaceBar, IE_Pressed, 1.0f));
		NextDelay = 0.05f;
		break;

	case 4:
		WallJumpTestEntries.Add(FString::Printf(
			TEXT("{\"step\": \"after_2nd_jump\", \"velocity_x\": %.2f, \"velocity_z\": %.2f}"),
			Character->GetVelocity().X, Character->GetVelocity().Z));
		PC->InputKey(FInputKeyEventArgs::CreateSimulated(EKeys::SpaceBar, IE_Released, 0.0f));
		NextDelay = 0.15f;
		break;

	case 5:
		WallJumpTestEntries.Add(FString::Printf(
			TEXT("{\"step\": \"final\", \"location_x\": %.2f, \"location_z\": %.2f, \"net_climb_z\": %.2f}"),
			Character->GetActorLocation().X, Character->GetActorLocation().Z,
			Character->GetActorLocation().Z - StartZ));
		FinishWallJumpTest(TEXT("completed"));
		return;

	default:
		FinishWallJumpTest(TEXT("completed"));
		return;
	}

	GetWorldTimerManager().SetTimer(
		WallJumpTestStepHandle,
		FTimerDelegate::CreateUObject(this, &AKyokaiGameMode::RunWallJumpTestStep, StepIndex + 1),
		NextDelay,
		false);
}

void AKyokaiGameMode::FinishWallJumpTest(const FString& Outcome)
{
	FString Json = TEXT("{\n  \"outcome\": \"");
	Json += Outcome;
	Json += TEXT("\",\n  \"steps\": [\n");
	for (int32 Index = 0; Index < WallJumpTestEntries.Num(); ++Index)
	{
		Json += TEXT("    ");
		Json += WallJumpTestEntries[Index];
		Json += (Index + 1 < WallJumpTestEntries.Num()) ? TEXT(",\n") : TEXT("\n");
	}
	Json += TEXT("  ]\n}\n");

	const FString OutPath = FPaths::ProjectSavedDir() / TEXT("WallJumpSmokeTest.json");
	FFileHelper::SaveStringToFile(Json, *OutPath);

	FGenericPlatformMisc::RequestExit(false);
}

void AKyokaiGameMode::TryStartBounceTest()
{
	if (!FParse::Param(FCommandLine::Get(), TEXT("KyokaiBounceTest")))
	{
		return;
	}

	BounceTestEntries.Reset();
	BounceTestPollAttempts = 0;
	GetWorldTimerManager().SetTimer(
		BounceTestPollHandle, this, &AKyokaiGameMode::PollForPawnThenRunBounceTest, 0.2f, true);
}

void AKyokaiGameMode::PollForPawnThenRunBounceTest()
{
	++BounceTestPollAttempts;

	APlayerController* PC = GetWorld() ? GetWorld()->GetFirstPlayerController() : nullptr;
	AKyokaiCharacter* Character = PC ? Cast<AKyokaiCharacter>(PC->GetPawn()) : nullptr;

	if (!Character && BounceTestPollAttempts < 25) // ~5s at 0.2s intervals
	{
		return;
	}

	GetWorldTimerManager().ClearTimer(BounceTestPollHandle);

	if (!Character)
	{
		FinishBounceTest(TEXT("no pawn possessed within timeout"));
		return;
	}

	BounceTestCharacter = Character;
	RunBounceTestStep(0);
}

void AKyokaiGameMode::RunBounceTestStep(const int32 StepIndex)
{
	AKyokaiCharacter* Character = BounceTestCharacter.Get();
	if (!Character)
	{
		FinishBounceTest(TEXT("pawn became invalid mid-test"));
		return;
	}

	float NextDelay = 0.2f;

	// BouncePad_Zone6 is centered at X=10210 (200cm diameter after 2x
	// scale); its overlap trigger sits at world Z=600 (pad base Z=500,
	// +50cm unscaled half-height to the top, +50cm more to the trigger's
	// own local offset). Dropping from Z=900 gives ~300cm of real fall
	// (~0.5s at this project's 2352cm/s^2 effective gravity) before it
	// should land on the trigger.
	constexpr float PadX = 10210.0f;
	constexpr float DropStartZ = 900.0f;

	switch (StepIndex)
	{
	case 0:
		Character->SetActorLocation(FVector(PadX, 0.0f, DropStartZ), false, nullptr, ETeleportType::TeleportPhysics);
		if (UKyokaiMovementComponent* Movement = Character->GetKyokaiMovement())
		{
			Movement->Velocity = FVector::ZeroVector;
			Movement->SetMovementMode(MOVE_Falling);
		}
		BounceTestEntries.Add(FString::Printf(
			TEXT("{\"step\": \"teleported\", \"location_x\": %.2f, \"location_z\": %.2f}"), PadX, DropStartZ));
		NextDelay = 0.6f; // past the expected ~0.5s fall to the pad
		break;

	case 1:
		BounceTestEntries.Add(FString::Printf(
			TEXT("{\"step\": \"just_after_bounce\", \"velocity_z\": %.2f, \"location_z\": %.2f}"),
			Character->GetVelocity().Z, Character->GetActorLocation().Z));
		NextDelay = 0.2f;
		break;

	case 2:
		// A second, later reading: still clearly ascending (not just a
		// one-frame velocity blip that gravity already erased) confirms
		// this was a real launch, not a fluke.
		BounceTestEntries.Add(FString::Printf(
			TEXT("{\"step\": \"still_ascending\", \"velocity_z\": %.2f, \"location_z\": %.2f}"),
			Character->GetVelocity().Z, Character->GetActorLocation().Z));
		FinishBounceTest(TEXT("completed"));
		return;

	default:
		FinishBounceTest(TEXT("completed"));
		return;
	}

	GetWorldTimerManager().SetTimer(
		BounceTestStepHandle,
		FTimerDelegate::CreateUObject(this, &AKyokaiGameMode::RunBounceTestStep, StepIndex + 1),
		NextDelay,
		false);
}

void AKyokaiGameMode::FinishBounceTest(const FString& Outcome)
{
	FString Json = TEXT("{\n  \"outcome\": \"");
	Json += Outcome;
	Json += TEXT("\",\n  \"steps\": [\n");
	for (int32 Index = 0; Index < BounceTestEntries.Num(); ++Index)
	{
		Json += TEXT("    ");
		Json += BounceTestEntries[Index];
		Json += (Index + 1 < BounceTestEntries.Num()) ? TEXT(",\n") : TEXT("\n");
	}
	Json += TEXT("  ]\n}\n");

	const FString OutPath = FPaths::ProjectSavedDir() / TEXT("BounceSmokeTest.json");
	FFileHelper::SaveStringToFile(Json, *OutPath);

	FGenericPlatformMisc::RequestExit(false);
}

void AKyokaiGameMode::TryStartDropTest()
{
	if (!FParse::Param(FCommandLine::Get(), TEXT("KyokaiDropTest")))
	{
		return;
	}

	DropTestEntries.Reset();
	DropTestPollAttempts = 0;
	GetWorldTimerManager().SetTimer(DropTestPollHandle, this, &AKyokaiGameMode::PollForPawnThenRunDropTest, 0.2f, true);
}

void AKyokaiGameMode::PollForPawnThenRunDropTest()
{
	++DropTestPollAttempts;

	APlayerController* PC = GetWorld() ? GetWorld()->GetFirstPlayerController() : nullptr;
	AKyokaiCharacter* Character = PC ? Cast<AKyokaiCharacter>(PC->GetPawn()) : nullptr;

	if (!Character && DropTestPollAttempts < 25) // ~5s at 0.2s intervals
	{
		return;
	}

	GetWorldTimerManager().ClearTimer(DropTestPollHandle);

	if (!Character)
	{
		FinishDropTest(TEXT("no pawn possessed within timeout"));
		return;
	}

	DropTestController = PC;
	DropTestCharacter = Character;
	RunDropTestStep(0);
}

void AKyokaiGameMode::RunDropTestStep(const int32 StepIndex)
{
	APlayerController* PC = DropTestController.Get();
	AKyokaiCharacter* Character = DropTestCharacter.Get();
	if (!PC || !Character)
	{
		FinishDropTest(TEXT("pawn or controller became invalid mid-test"));
		return;
	}

	UKyokaiMovementComponent* Movement = Character->GetKyokaiMovement();
	float NextDelay = 0.1f;

	// Drop C (the hardest of the three): Platform_Zone7_LedgeC ends at
	// X=14110, top Z=-250. Teleporting 10cm past the edge, already
	// falling, isolates "does the drop itself require a dash" from
	// "did the character actually run off the edge cleanly", which isn't
	// what this test is checking. Landing C starts at X=15010, top Z=-1050
	// (standing height -954).
	constexpr float StartX = 14120.0f;
	constexpr float StartZ = -154.0f; // ledge top (-250) + capsule half-height (96)
	constexpr float LandingX = 15010.0f;
	constexpr float LandingStandZ = -954.0f; // landing top (-1050) + capsule half-height (96)

	switch (StepIndex)
	{
	case 0:
		Character->SetActorLocation(FVector(StartX, 0.0f, StartZ), false, nullptr, ETeleportType::TeleportPhysics);
		if (Movement)
		{
			Movement->Velocity = FVector(850.0f, 0.0f, 0.0f);
			Movement->SetMovementMode(MOVE_Falling);
		}
		DropTestEntries.Add(TEXT("{\"step\": \"no_dash_attempt_started\"}"));
		NextDelay = 1.0f; // past the ~0.82s expected fall time for an 800cm drop
		break;

	case 1:
	{
		const FVector NoDashResult = Character->GetActorLocation();
		const bool bNoDashReachedLanding = NoDashResult.X >= LandingX && NoDashResult.Z >= LandingStandZ - 50.0f;
		DropTestEntries.Add(FString::Printf(
			TEXT("{\"step\": \"no_dash_result\", \"location_x\": %.2f, \"location_z\": %.2f, \"reached_landing\": %s}"),
			NoDashResult.X, NoDashResult.Z, bNoDashReachedLanding ? TEXT("true") : TEXT("false")));

		Character->SetActorLocation(FVector(StartX, 0.0f, StartZ), false, nullptr, ETeleportType::TeleportPhysics);
		if (Movement)
		{
			Movement->Velocity = FVector(850.0f, 0.0f, 0.0f);
			Movement->SetMovementMode(MOVE_Falling);
		}
		PC->InputKey(FInputKeyEventArgs::CreateSimulated(EKeys::D, IE_Pressed, 1.0f));
		PC->InputKey(FInputKeyEventArgs::CreateSimulated(EKeys::LeftShift, IE_Pressed, 1.0f));
		DropTestEntries.Add(FString::Printf(
			TEXT("{\"step\": \"dash_pressed\", \"location_z\": %.2f, \"velocity_z\": %.2f}"),
			Character->GetActorLocation().Z, Character->GetVelocity().Z));
		NextDelay = 0.1f;
		break;
	}

	// Sample every 0.1s (same reasoning as the earlier gap test): a single
	// fixed-time snapshot conflates "traveled far enough in X" with
	// "actually at landing height when it got there".
	case 2: case 3: case 4: case 5: case 6: case 7: case 8: case 9: case 10: case 11: case 12:
		DropTestEntries.Add(FString::Printf(
			TEXT("{\"step\": \"dash_trajectory\", \"t\": %d, \"location_x\": %.2f, \"location_z\": %.2f}"),
			StepIndex - 1, Character->GetActorLocation().X, Character->GetActorLocation().Z));
		NextDelay = 0.1f;
		break;

	case 13:
		PC->InputKey(FInputKeyEventArgs::CreateSimulated(EKeys::D, IE_Released, 0.0f));
		PC->InputKey(FInputKeyEventArgs::CreateSimulated(EKeys::LeftShift, IE_Released, 0.0f));
		FinishDropTest(TEXT("completed"));
		return;

	default:
		FinishDropTest(TEXT("completed"));
		return;
	}

	GetWorldTimerManager().SetTimer(
		DropTestStepHandle,
		FTimerDelegate::CreateUObject(this, &AKyokaiGameMode::RunDropTestStep, StepIndex + 1),
		NextDelay,
		false);
}

void AKyokaiGameMode::FinishDropTest(const FString& Outcome)
{
	FString Json = TEXT("{\n  \"outcome\": \"");
	Json += Outcome;
	Json += TEXT("\",\n  \"steps\": [\n");
	for (int32 Index = 0; Index < DropTestEntries.Num(); ++Index)
	{
		Json += TEXT("    ");
		Json += DropTestEntries[Index];
		Json += (Index + 1 < DropTestEntries.Num()) ? TEXT(",\n") : TEXT("\n");
	}
	Json += TEXT("  ]\n}\n");

	const FString OutPath = FPaths::ProjectSavedDir() / TEXT("DropSmokeTest.json");
	FFileHelper::SaveStringToFile(Json, *OutPath);

	FGenericPlatformMisc::RequestExit(false);
}

void AKyokaiGameMode::TryStartLevel02Timing()
{
	if (!FParse::Param(FCommandLine::Get(), TEXT("KyokaiLevel02Timing")))
	{
		return;
	}

	Level02TimingEntries.Reset();
	Level02TimingPollAttempts = 0;
	GetWorldTimerManager().SetTimer(
		Level02TimingPollHandle, this, &AKyokaiGameMode::PollForPawnThenRunLevel02Timing, 0.2f, true);
}

void AKyokaiGameMode::PollForPawnThenRunLevel02Timing()
{
	++Level02TimingPollAttempts;

	APlayerController* PC = GetWorld() ? GetWorld()->GetFirstPlayerController() : nullptr;
	AKyokaiCharacter* Character = PC ? Cast<AKyokaiCharacter>(PC->GetPawn()) : nullptr;

	if (!Character && Level02TimingPollAttempts < 25) // ~5s at 0.2s intervals
	{
		return;
	}

	GetWorldTimerManager().ClearTimer(Level02TimingPollHandle);

	if (!Character)
	{
		FinishLevel02Timing(TEXT("no pawn possessed within timeout"));
		return;
	}

	Level02TimingController = PC;
	Level02TimingCharacter = Character;
	Level02TimingStartTime = GetWorld()->GetTimeSeconds();
	Level02TimingNextTrigger = 0;
	Level02TimingNextSegment = 0;
	Level02TimingLastShaftPress = -1000.0f;
	bLevel02TimingSliding = false;

	PC->InputKey(FInputKeyEventArgs::CreateSimulated(EKeys::D, IE_Pressed, 1.0f));
	Level02TimingEntries.Add(TEXT("{\"step\": \"run_started\"}"));

	GetWorldTimerManager().SetTimer(Level02TimingTickHandle, this, &AKyokaiGameMode::TickLevel02Timing, 0.05f, true);
}

void AKyokaiGameMode::TickLevel02Timing()
{
	APlayerController* PC = Level02TimingController.Get();
	AKyokaiCharacter* Character = Level02TimingCharacter.Get();
	if (!PC || !Character)
	{
		GetWorldTimerManager().ClearTimer(Level02TimingTickHandle);
		FinishLevel02Timing(TEXT("pawn or controller became invalid mid-run"));
		return;
	}

	const float Elapsed = GetWorld()->GetTimeSeconds() - Level02TimingStartTime;
	const FVector Loc = Character->GetActorLocation();

	// One-shot reactive triggers, in ascending X order - computed from this
	// level's own build coordinates (see kyokai-level02-toits-pluie memory).
	// 0=JumpTap 1=SlideStart 2=SlideEnd 3=DashTap
	// Slide start/end triggers sit much further from the ceiling than the
	// nominal clearance boundary would suggest - a 50cm buffer looked fine
	// on paper but wasn't: the capsule's own 42cm radius plus a tick or two
	// of input-dispatch latency (InputKey() doesn't process synchronously)
	// eats almost all of a tight margin, and the character hit the ceiling
	// face-on while still standing before the crouch could apply. Same
	// logic in reverse for release: uncrouching needs the capsule's trailing
	// edge clear of the ceiling too, not just its center.
	// Dash trigger (16610) fires just AFTER the ledge edge (16600), not
	// before: pressing dash while still grounded flies a flat trajectory
	// that zeroes vertical velocity and covers less ground than falling
	// first - already learned the hard way on the controller gym's Zone 7
	// (see kyokai-prototype-state memory) - only dashing once already
	// falling gets the height needed to clear the drop.
	// The Segment 2 bounce pad (BouncePad_Seg2, X=6350, radius 65cm after
	// scale) is a solid mesh, not a pass-through trigger volume - a jump
	// timed too close to its near face (6285) just runs into its side wall
	// and stops dead, same as hitting any other wall. Jumping well before
	// it (6150, 135cm of margin) arcs onto/over it instead.
	static const float TriggerX[] = {
		1970.f, 2570.f, 3370.f, 5200.f, 6100.f, 6200.f, 6820.f, 7320.f, 7820.f,
		9270.f, 11670.f, 12470.f, 15470.f, 15800.f, 16610.f, 16700.f, 17570.f
	};
	static const int32 TriggerType[] = {
		0, 0, 0, 1, 2, 0, 0, 0, 0,
		0, 0, 0, 0, 1, 3, 2, 0
	};
	static const int32 NumTriggers = UE_ARRAY_COUNT(TriggerX);

	while (Level02TimingNextTrigger < NumTriggers && Loc.X >= TriggerX[Level02TimingNextTrigger])
	{
		switch (TriggerType[Level02TimingNextTrigger])
		{
		case 0: // jump tap
			PC->InputKey(FInputKeyEventArgs::CreateSimulated(EKeys::SpaceBar, IE_Pressed, 1.0f));
			PC->InputKey(FInputKeyEventArgs::CreateSimulated(EKeys::SpaceBar, IE_Released, 0.0f));
			break;
		case 1: // slide start
			PC->InputKey(FInputKeyEventArgs::CreateSimulated(EKeys::LeftControl, IE_Pressed, 1.0f));
			bLevel02TimingSliding = true;
			break;
		case 2: // slide end
			PC->InputKey(FInputKeyEventArgs::CreateSimulated(EKeys::LeftControl, IE_Released, 0.0f));
			bLevel02TimingSliding = false;
			break;
		case 3: // dash tap
			PC->InputKey(FInputKeyEventArgs::CreateSimulated(EKeys::LeftShift, IE_Pressed, 1.0f));
			PC->InputKey(FInputKeyEventArgs::CreateSimulated(EKeys::LeftShift, IE_Released, 0.0f));
			break;
		default:
			break;
		}
		++Level02TimingNextTrigger;
	}

	// Wall-jump shaft (Segment 5, X=12600-12860): periodic jump taps while
	// below the exit height - PerformWallJump() only fires as a fallback
	// when CanJump() is false and a wall is touched, so spamming this is
	// safe outside the shaft too, but it's scoped here anyway for clarity.
	if (Loc.X >= 12600.f && Loc.X <= 12860.f && Loc.Z < 1150.f && Elapsed - Level02TimingLastShaftPress >= 0.25f)
	{
		PC->InputKey(FInputKeyEventArgs::CreateSimulated(EKeys::SpaceBar, IE_Pressed, 1.0f));
		PC->InputKey(FInputKeyEventArgs::CreateSimulated(EKeys::SpaceBar, IE_Released, 0.0f));
		Level02TimingLastShaftPress = Elapsed;
	}

	// Segment splits - X thresholds match each SegmentMarker_N_End platform.
	static const float SegmentEndX[] = { 4250.f, 6850.f, 10450.f, 12200.f, 14060.f, 15500.f, 18000.f };
	static const TCHAR* SegmentNames[] = {
		TEXT("Seg1_Accroche"), TEXT("Seg2_Enseignement"), TEXT("Seg3_Enseignes"),
		TEXT("Seg4_Onibi"), TEXT("Seg5_Gouttieres"), TEXT("Seg6_Paratonnerres"), TEXT("Seg7_Finish")
	};
	static const int32 NumSegments = UE_ARRAY_COUNT(SegmentEndX);

	while (Level02TimingNextSegment < NumSegments && Loc.X >= SegmentEndX[Level02TimingNextSegment])
	{
		Level02TimingEntries.Add(FString::Printf(
			TEXT("{\"segment\": \"%s\", \"elapsed_s\": %.2f, \"location_x\": %.2f, \"location_z\": %.2f}"),
			SegmentNames[Level02TimingNextSegment], Elapsed, Loc.X, Loc.Z));
		++Level02TimingNextSegment;

		if (Level02TimingNextSegment >= NumSegments)
		{
			GetWorldTimerManager().ClearTimer(Level02TimingTickHandle);
			PC->InputKey(FInputKeyEventArgs::CreateSimulated(EKeys::D, IE_Released, 0.0f));
			FinishLevel02Timing(TEXT("completed"));
			return;
		}
	}

	static float LastDebugLogTime = -1000.0f;
	if (Elapsed - LastDebugLogTime >= 1.0f)
	{
		LastDebugLogTime = Elapsed;
		Level02TimingEntries.Add(FString::Printf(
			TEXT("{\"step\": \"debug_trace\", \"elapsed_s\": %.2f, \"location_x\": %.2f, \"location_z\": %.2f, \"velocity_x\": %.2f, \"is_grounded\": %s}"),
			Elapsed, Loc.X, Loc.Z, Character->GetVelocity().X,
			Character->GetKyokaiMovement() && Character->GetKyokaiMovement()->IsMovingOnGround() ? TEXT("true") : TEXT("false")));
	}

	// Safety timeout - a stuck or fallen-through run shouldn't hang forever.
	if (Elapsed > 90.0f)
	{
		GetWorldTimerManager().ClearTimer(Level02TimingTickHandle);
		Level02TimingEntries.Add(FString::Printf(
			TEXT("{\"step\": \"timeout\", \"elapsed_s\": %.2f, \"location_x\": %.2f, \"location_z\": %.2f, \"next_trigger_index\": %d, \"next_segment_index\": %d}"),
			Elapsed, Loc.X, Loc.Z, Level02TimingNextTrigger, Level02TimingNextSegment));
		FinishLevel02Timing(TEXT("timeout - likely stuck or fell through"));
		return;
	}
}

void AKyokaiGameMode::FinishLevel02Timing(const FString& Outcome)
{
	FString Json = TEXT("{\n  \"outcome\": \"");
	Json += Outcome;
	Json += TEXT("\",\n  \"steps\": [\n");
	for (int32 Index = 0; Index < Level02TimingEntries.Num(); ++Index)
	{
		Json += TEXT("    ");
		Json += Level02TimingEntries[Index];
		Json += (Index + 1 < Level02TimingEntries.Num()) ? TEXT(",\n") : TEXT("\n");
	}
	Json += TEXT("  ]\n}\n");

	const FString OutPath = FPaths::ProjectSavedDir() / TEXT("Level02TimingReport.json");
	FFileHelper::SaveStringToFile(Json, *OutPath);

	FGenericPlatformMisc::RequestExit(false);
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
