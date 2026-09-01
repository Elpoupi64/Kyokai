// Copyright Epic Games, Inc. All Rights Reserved.

#include "Game/KyokaiGameMode.h"

#include "Characters/KyokaiCharacter.h"
#include "Characters/KyokaiMovementComponent.h"
#include "Enemies/Bakeneko.h"
#include "Enemies/Onibi.h"
#include "GameFramework/PlayerController.h"
#include "GameFramework/PlayerStart.h"
#include "HAL/PlatformMisc.h"
#include "Kismet/GameplayStatics.h"
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

	if (const AActor* Start = UGameplayStatics::GetActorOfClass(this, APlayerStart::StaticClass()))
	{
		RespawnLocation = Start->GetActorLocation();
	}

	TryStartInputSmokeTest();
	TryStartWallJumpTest();
	TryStartBounceTest();
	TryStartDropTest();
	TryStartLevel02Timing();
	TryStartExpertRouteTest();
	TryStartMasterySealTest();
	StartPlaytestLogging();
}

void AKyokaiGameMode::EndPlay(const EEndPlayReason::Type EndPlayReason)
{
	if (bPlaytestActive && !bPlaytestLevelCompleted)
	{
		const float Elapsed = GetWorld() ? GetWorld()->GetTimeSeconds() - PlaytestStartTime : 0.0f;
		LogPlaytestEvent(FString::Printf(
			TEXT("{\"event\": \"session_end_incomplete\", \"elapsed_s\": %.2f}"), Elapsed));
	}

	Super::EndPlay(EndPlayReason);
}

void AKyokaiGameMode::NotifyCheckpointActivated(const FVector& Location)
{
	RespawnLocation = Location;

	if (bPlaytestActive)
	{
		const float Elapsed = GetWorld()->GetTimeSeconds() - PlaytestStartTime;
		LogPlaytestEvent(FString::Printf(
			TEXT("{\"event\": \"checkpoint_activated\", \"elapsed_s\": %.2f, \"location_x\": %.2f}"),
			Elapsed, Location.X));
	}
}

void AKyokaiGameMode::NotifyPlayerDeath(const FString& Cause, const FVector& Location)
{
	if (bPlaytestActive)
	{
		const float Elapsed = GetWorld()->GetTimeSeconds() - PlaytestStartTime;
		LogPlaytestEvent(FString::Printf(
			TEXT("{\"event\": \"death\", \"cause\": \"%s\", \"elapsed_s\": %.2f, \"location_x\": %.2f, \"location_z\": %.2f}"),
			*Cause, Elapsed, Location.X, Location.Z));
	}
}

void AKyokaiGameMode::NotifyLevelCompleted()
{
	if (bPlaytestActive && !bPlaytestLevelCompleted)
	{
		bPlaytestLevelCompleted = true;
		const float Elapsed = GetWorld()->GetTimeSeconds() - PlaytestStartTime;
		LogPlaytestEvent(FString::Printf(
			TEXT("{\"event\": \"level_completed\", \"total_time_s\": %.2f}"), Elapsed));
		GetWorldTimerManager().ClearTimer(PlaytestSampleHandle);
	}
}

void AKyokaiGameMode::NotifyMemoryCollected(const FString& FragmentId, const FString& FragmentText, const FVector& Location)
{
	// No journal/dialogue UI exists anywhere in this project yet (see
	// AMemoryFragment's header) - this print is the stand-in presentation
	// until one does.
	UE_LOG(LogTemp, Display, TEXT("Memory fragment collected [%s]: %s"), *FragmentId, *FragmentText);

	if (bPlaytestActive)
	{
		const float Elapsed = GetWorld()->GetTimeSeconds() - PlaytestStartTime;
		LogPlaytestEvent(FString::Printf(
			TEXT("{\"event\": \"memory_collected\", \"fragment_id\": \"%s\", \"elapsed_s\": %.2f, \"location_x\": %.2f}"),
			*FragmentId, Elapsed, Location.X));
	}
}

void AKyokaiGameMode::NotifySealCollected(const FString& SealId, const FString& SealKind, const FVector& Location)
{
	// Same "no UI yet, log it" stopgap as NotifyMemoryCollected - see that
	// method's comment and AHarmonySeal's header.
	UE_LOG(LogTemp, Display, TEXT("Harmony seal collected [%s] (%s)"), *SealId, *SealKind);

	if (bPlaytestActive)
	{
		const float Elapsed = GetWorld()->GetTimeSeconds() - PlaytestStartTime;
		LogPlaytestEvent(FString::Printf(
			TEXT("{\"event\": \"seal_collected\", \"seal_id\": \"%s\", \"seal_kind\": \"%s\", \"elapsed_s\": %.2f, \"location_x\": %.2f}"),
			*SealId, *SealKind, Elapsed, Location.X));
	}
}

void AKyokaiGameMode::NotifyIntegrityLost(const FString& Cause, const int32 RemainingSegments, const FVector& Location)
{
	if (bPlaytestActive)
	{
		const float Elapsed = GetWorld()->GetTimeSeconds() - PlaytestStartTime;
		LogPlaytestEvent(FString::Printf(
			TEXT("{\"event\": \"integrity_lost\", \"cause\": \"%s\", \"remaining_segments\": %d, \"elapsed_s\": %.2f, \"location_x\": %.2f}"),
			*Cause, RemainingSegments, Elapsed, Location.X));
	}
}

bool AKyokaiGameMode::IsAutomatedTestRun() const
{
	return FParse::Param(FCommandLine::Get(), TEXT("KyokaiInputSmokeTest"))
		|| FParse::Param(FCommandLine::Get(), TEXT("KyokaiWallJumpTest"))
		|| FParse::Param(FCommandLine::Get(), TEXT("KyokaiBounceTest"))
		|| FParse::Param(FCommandLine::Get(), TEXT("KyokaiDropTest"))
		|| FParse::Param(FCommandLine::Get(), TEXT("KyokaiLevel02Timing"))
		|| FParse::Param(FCommandLine::Get(), TEXT("KyokaiExpertRouteTest"))
		|| FParse::Param(FCommandLine::Get(), TEXT("KyokaiMasterySealTest"));
}

void AKyokaiGameMode::StartPlaytestLogging()
{
	if (IsAutomatedTestRun())
	{
		return;
	}

	bPlaytestActive = true;
	bPlaytestLevelCompleted = false;
	bPlaytestExpertRouteUsed = false;
	PlaytestStartTime = GetWorld()->GetTimeSeconds();
	PlaytestMinFps = 0.0f;

	const FString SessionId = FDateTime::Now().ToString(TEXT("%Y%m%d_%H%M%S"));
	PlaytestLogPath = FPaths::ProjectSavedDir() / TEXT("Playtests") / FString::Printf(TEXT("Playtest_%s.jsonl"), *SessionId);

	LogPlaytestEvent(FString::Printf(TEXT("{\"event\": \"session_start\", \"session_id\": \"%s\"}"), *SessionId));

	GetWorldTimerManager().SetTimer(PlaytestSampleHandle, this, &AKyokaiGameMode::SamplePlaytestFpsAndExpertRoute, 2.0f, true);
}

void AKyokaiGameMode::LogPlaytestEvent(const FString& EventJson)
{
	if (PlaytestLogPath.IsEmpty())
	{
		return;
	}
	FFileHelper::SaveStringToFile(EventJson + LINE_TERMINATOR, *PlaytestLogPath,
		FFileHelper::EEncodingOptions::AutoDetect, &IFileManager::Get(), FILEWRITE_Append);
}

void AKyokaiGameMode::SamplePlaytestFpsAndExpertRoute()
{
	const UWorld* World = GetWorld();
	if (!World)
	{
		return;
	}

	const float DeltaSeconds = World->GetDeltaSeconds();
	const float Fps = DeltaSeconds > 0.0f ? 1.0f / DeltaSeconds : 0.0f;
	if (PlaytestMinFps <= 0.0f || Fps < PlaytestMinFps)
	{
		PlaytestMinFps = Fps;
	}

	const float Elapsed = World->GetTimeSeconds() - PlaytestStartTime;
	LogPlaytestEvent(FString::Printf(
		TEXT("{\"event\": \"fps_sample\", \"elapsed_s\": %.2f, \"fps\": %.1f, \"min_fps_so_far\": %.1f}"),
		Elapsed, Fps, PlaytestMinFps));

	if (!bPlaytestExpertRouteUsed)
	{
		const APlayerController* PC = World->GetFirstPlayerController();
		const AKyokaiCharacter* Character = PC ? Cast<AKyokaiCharacter>(PC->GetPawn()) : nullptr;
		if (Character)
		{
			const FVector Loc = Character->GetActorLocation();
			// Expert_Seg3_Upper: x=8380-9150, top=430 (origin z=528.15 when
			// standing on it) - well above the main path's ~248 there.
			if (Loc.X >= 8380.f && Loc.X <= 9150.f && Loc.Z > 500.f)
			{
				bPlaytestExpertRouteUsed = true;
				LogPlaytestEvent(FString::Printf(
					TEXT("{\"event\": \"expert_route_used\", \"elapsed_s\": %.2f}"), Elapsed));
			}
		}
	}
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
	Level02TimingLastDodgeJump = -1000.0f;
	Level02TimingLastX = 0.0f;
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

	// Checkpoint/fall-catch respawn detection: X drops sharply (far more
	// than one tick's normal movement, ~42cm at 850cm/s * 0.05s) means the
	// character just got teleported back, not that it walked backward.
	// Level02TimingNextTrigger/NextSegment are monotonic counters that
	// would otherwise never re-fire for anything before the respawn point,
	// stranding the bot - confirmed: without this resync, a hazard/fall
	// respawn left it stuck for the full 90s timeout every time once real
	// checkpoint consequences replaced the old knockback-only stopgap.
	const bool bJustRespawned = (Level02TimingLastX - Loc.X) > 200.f;
	Level02TimingLastX = Loc.X;

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
	// Segments 2-3 used to have two BouncePad-driven elevation gains here;
	// both produced arcs that overshot their landing platforms (a bounce's
	// horizontal carry is much harder to predict than a plain jump's - the
	// character was still ~340 units above the intended 150-unit landing
	// height when passing directly over the platform, per a traced run in
	// Saved/Level02TimingReport.json, and only came back down to that
	// height around x=7036, well past the platform's original 6450-6850
	// span). Replaced both pads with plain jump-tap climbs onto much wider
	// landing platforms (Roof_Seg2_BigLanding, Roof_Seg3_BigLanding) sized
	// from the actual jump arc (v=1250, g=2352 => lands ~x=7036 and ~x=10037
	// respectively, both with 400cm+ margin to either edge). Plain jumps are
	// the proven-reliable mechanism here - Segment 1's own ~300cm elevation
	// gain (x=1970) already matches this same model to within ~4%.
	// 11670 (originally, pre-dating this level's debugging) fired 220cm past
	// Roof_Seg4_Arena's actual edge (11450) - the character was already
	// falling for 220cm before the jump input landed. Moved to 11350
	// (100cm before the edge), matching every other working trigger's margin.
	// 14600 (removed): used to fire for the Roof_Seg6_A -> Roof_Seg6_B gap.
	// That gap is gone now too (Roof_Seg6_A extended to connect flush with
	// Roof_Seg6_B at 15000) - a genuine fall was found in this exact
	// stretch during a later, more extreme wall-jump-shaft-climb-variance
	// run (discovered via the new checkpoint respawn looping here instead
	// of the bot just ending the run, per the same run-to-run variance
	// already documented for the Seg6_B->tunnel gap below). Same fix.
	// 15470 (removed): used to fire for the Roof_Seg6_B -> RunupTunnel gap,
	// but that gap is gone now (Roof_Seg6_B extended to connect flush with
	// the tunnel at 15800) - the character's height on arrival here varies
	// run-to-run (the wall-jump shaft's climb isn't precise), so a jump-
	// precision gap right after it was fragile by construction, not a
	// simple mistimed trigger like the others.
	// 7750 (removed): used for a manual jump across the old plain 350cm
	// Sign_Seg3_2 -> Sign_Seg3_3 gap. That gap is now bridged by
	// BouncePad_Seg3_Sign2 (step 4 "enseignes servant de rebonds") - the
	// character just runs into its overlap trigger and gets launched
	// automatically, no jump input needed. A stray jump right at the pad's
	// position risks clipping its solid mesh sideways (the same "too close
	// to the pad's face" issue documented for BouncePad_Seg2 earlier).
	// 15800 slide-start (removed from this fixed-X array, handled below as
	// a grounded-reactive check instead): OnSlidePressed() silently no-ops
	// if !IsMovingOnGround(), and the wall-jump shaft's climb variance can
	// leave the character still airborne when X crosses 15800 (confirmed:
	// one run was still falling from an overshoot to Z=1487 at this point)
	// - the slide press was discarded, and the character later landed
	// standing tall and rammed Ceiling_Seg7_Tunnel's face at x=16158,
	// stuck for the rest of the run. A third distinct downstream failure
	// mode from the same shaft variance (after the two gap-merges already
	// fixed), this time breaking a precondition rather than just position.
	// Tried replacing 9200 (Roof_Seg3_Landing -> Roof_Seg3_BigLanding manual
	// jump, 150->350 gain, lands ~x=10037) with a BouncePad instead, twice -
	// reverted both times, kept as the plain jump trigger. See
	// kyokai-level02-toits-pluie memory for the full diagnosis: a
	// ground-level flush pad approached by flat running (not landed on from
	// an already-airborne jump arc, unlike BouncePad_Seg3_Sign2) loses all
	// horizontal velocity to the pad's own solid side collision before the
	// bounce trigger can act - confirmed via a temporary UE_LOG probe that
	// the overlap genuinely fires and LaunchCharacter genuinely runs, but
	// with zero horizontal carry left, so it just pops straight up and back
	// down onto the same blocked spot forever. BouncePad is built for being
	// landed on top of mid-jump ("a running jump into one keeps its
	// direction" - see the class's own header comment), not for a flat
	// walk-in.
	static const float TriggerX[] = {
		1970.f, 2570.f, 3370.f, 5200.f, 6100.f, 6150.f, 7350.f,
		9200.f, 11350.f, 12470.f, 16610.f, 16700.f, 17570.f
	};
	static const int32 TriggerType[] = {
		0, 0, 0, 1, 2, 0, 0,
		0, 0, 0, 3, 2, 0
	};
	static const int32 NumTriggers = UE_ARRAY_COUNT(TriggerX);

	if (bJustRespawned)
	{
		Level02TimingNextTrigger = 0;
		while (Level02TimingNextTrigger < NumTriggers && Loc.X >= TriggerX[Level02TimingNextTrigger])
		{
			++Level02TimingNextTrigger;
		}
	}

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

	// Defensive dodge: Onibi/Bakeneko now respawn the character on contact
	// (a real checkpoint cost - see kyokai-level02-toits-pluie memory on
	// reworking the old knockback-only stopgap), so a bot that never reacts
	// gets stuck looping forever at the first one it can't avoid (confirmed:
	// without this, the run just cycled Checkpoint_1 <-> Onibi's charge for
	// the whole 90s timeout).
	// Reacts to the TELEGRAPH, not the attack itself - a first attempt
	// jumping on bIsCharging/bIsPouncing still looped forever. Onibi's
	// charge (1400cm/s) closes on a still-approaching player (850cm/s) at
	// up to 2250cm/s combined, and by the time telegraph ends the gap is
	// already small (well under 400cm here) - under 0.1s to impact, far
	// less than one InputKey() dispatch cycle. Jumping the moment the
	// telegraph *starts* instead uses the full 0.8s/0.5s warning window as
	// intended: still airborne and well above the hitbox height when the
	// attack actually launches (checked: a jump from Onibi's arena height
	// is still ~247cm up at t=0.8s, comfortably above its 420-520 hitbox).
	if (Elapsed - Level02TimingLastDodgeJump >= 0.3f)
	{
		bool bShouldDodge = false;

		// GetActorOfClass() only returns ONE actor - fine for a single
		// Onibi, but the Segment 4 gauntlet (Onibi_Seg4/_B/_C) needs every
		// instance checked, or the bot would only ever react to whichever
		// one the query happens to return and walk straight into the others.
		TArray<AActor*> OnibiActors;
		UGameplayStatics::GetAllActorsOfClass(this, AOnibi::StaticClass(), OnibiActors);
		for (const AActor* Actor : OnibiActors)
		{
			if (const AOnibi* Onibi = Cast<AOnibi>(Actor))
			{
				bShouldDodge |= Onibi->bIsTelegraphingCharge;
			}
		}

		TArray<AActor*> BakenekoActors;
		UGameplayStatics::GetAllActorsOfClass(this, ABakeneko::StaticClass(), BakenekoActors);
		for (const AActor* Actor : BakenekoActors)
		{
			if (const ABakeneko* Bakeneko = Cast<ABakeneko>(Actor))
			{
				bShouldDodge |= Bakeneko->bIsTelegraphingPounce;
			}
		}

		if (bShouldDodge)
		{
			PC->InputKey(FInputKeyEventArgs::CreateSimulated(EKeys::SpaceBar, IE_Pressed, 1.0f));
			PC->InputKey(FInputKeyEventArgs::CreateSimulated(EKeys::SpaceBar, IE_Released, 0.0f));
			Level02TimingLastDodgeJump = Elapsed;
		}
	}

	// Wall-jump shaft (Segment 5, X=12600-12860): periodic jump taps while
	// below the exit height - PerformWallJump() only fires as a fallback
	// when CanJump() is false and a wall is touched, so spamming this is
	// safe outside the shaft too, but it's scoped here anyway for clarity.
	// PerformWallJump() sets Velocity.X outright (WallPushDirection *
	// WallJumpHorizontalVelocity) - holding D the whole time fights that with
	// continuous forward AddMovementInput, dragging the character straight
	// through the 260cm interior gap in ~0.3s instead of letting it bounce
	// wall-to-wall and actually climb. Release D for the climb itself, and
	// resume holding it once past the shaft (or once high enough to have
	// cleared it) so normal running resumes afterward.
	const bool bInShaft = Loc.X >= 12600.f && Loc.X <= 13060.f && Loc.Z < 1150.f;
	if (bInShaft && bLevel02TimingDHeld)
	{
		PC->InputKey(FInputKeyEventArgs::CreateSimulated(EKeys::D, IE_Released, 0.0f));
		bLevel02TimingDHeld = false;
	}
	else if (!bInShaft && !bLevel02TimingDHeld)
	{
		PC->InputKey(FInputKeyEventArgs::CreateSimulated(EKeys::D, IE_Pressed, 1.0f));
		bLevel02TimingDHeld = true;
	}

	// Slide-start for the Segment 7 tunnel (see the removed-15800 comment
	// above) - reacts to grounded state instead of a fixed X, so it still
	// fires correctly even if the character is still airborne (wall-jump
	// shaft overshoot) when it first crosses this X.
	if (!bLevel02TimingSliding && Loc.X >= 15800.f && Loc.X <= 16150.f
		&& Character->GetKyokaiMovement() && Character->GetKyokaiMovement()->IsMovingOnGround())
	{
		PC->InputKey(FInputKeyEventArgs::CreateSimulated(EKeys::LeftControl, IE_Pressed, 1.0f));
		bLevel02TimingSliding = true;
	}

	// Z<1050 (was <1150): a naive bot that keeps tapping until fully clear
	// of the exit height has no judgment about "high enough" the way a
	// real player would - one run overshot to Z=1487 (vs. the ~1150-1600
	// range seen before) and slammed face-on into Ceiling_Seg7_Tunnel's
	// near side while still descending from that height at x=16158, a new
	// failure mode distinct from the two gap-merges already fixed for this
	// shaft's variance. Stopping the spam earlier reduces how many bounces
	// can accumulate within the shaft's brief ~0.3-0.5s crossing window.
	if (Loc.X >= 12600.f && Loc.X <= 12860.f && Loc.Z < 1050.f && Elapsed - Level02TimingLastShaftPress >= 0.25f)
	{
		PC->InputKey(FInputKeyEventArgs::CreateSimulated(EKeys::SpaceBar, IE_Pressed, 1.0f));
		PC->InputKey(FInputKeyEventArgs::CreateSimulated(EKeys::SpaceBar, IE_Released, 0.0f));
		Level02TimingLastShaftPress = Elapsed;
	}

	// Sanity check: freefall past the level's floor means the character
	// missed a landing and is drifting through the void, not progressing -
	// without this, X can still cross every later segment's threshold while
	// falling and produce a false "completed" outcome (happened for real:
	// see kyokai-level02-toits-pluie memory). Threshold is -800, well past
	// AKyokaiCharacter's own real fall-catch/respawn-to-checkpoint at -600
	// (step 6) - anything shallower than that is now the game's own job to
	// recover from, not a bot failure; this only fires if that system
	// somehow didn't catch it either.
	if (Loc.Z < -800.f)
	{
		GetWorldTimerManager().ClearTimer(Level02TimingTickHandle);
		Level02TimingEntries.Add(FString::Printf(
			TEXT("{\"step\": \"fell_through\", \"elapsed_s\": %.2f, \"location_x\": %.2f, \"location_z\": %.2f, \"next_segment_index\": %d}"),
			Elapsed, Loc.X, Loc.Z, Level02TimingNextSegment));
		FinishLevel02Timing(TEXT("fell through the level"));
		return;
	}

	// Segment splits - X thresholds match each SegmentMarker_N_End platform.
	static const float SegmentEndX[] = { 4250.f, 6850.f, 10450.f, 12200.f, 14060.f, 15500.f, 18000.f };
	static const TCHAR* SegmentNames[] = {
		TEXT("Seg1_Accroche"), TEXT("Seg2_Enseignement"), TEXT("Seg3_Enseignes"),
		TEXT("Seg4_Onibi"), TEXT("Seg5_Gouttieres"), TEXT("Seg6_Paratonnerres"), TEXT("Seg7_Finish")
	};
	static const int32 NumSegments = UE_ARRAY_COUNT(SegmentEndX);

	if (bJustRespawned)
	{
		Level02TimingNextSegment = 0;
		while (Level02TimingNextSegment < NumSegments && Loc.X >= SegmentEndX[Level02TimingNextSegment])
		{
			++Level02TimingNextSegment;
		}
	}

	while (Level02TimingNextSegment < NumSegments && Loc.X >= SegmentEndX[Level02TimingNextSegment])
	{
		Level02TimingEntries.Add(FString::Printf(
			TEXT("{\"segment\": \"%s\", \"elapsed_s\": %.2f, \"location_x\": %.2f, \"location_z\": %.2f}"),
			SegmentNames[Level02TimingNextSegment], Elapsed, Loc.X, Loc.Z));
		++Level02TimingNextSegment;
	}

	if (Level02TimingNextSegment >= NumSegments)
	{
		GetWorldTimerManager().ClearTimer(Level02TimingTickHandle);
		PC->InputKey(FInputKeyEventArgs::CreateSimulated(EKeys::D, IE_Released, 0.0f));
		FinishLevel02Timing(TEXT("completed"));
		return;
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

void AKyokaiGameMode::TryStartExpertRouteTest()
{
	if (!FParse::Param(FCommandLine::Get(), TEXT("KyokaiExpertRouteTest")))
	{
		return;
	}

	ExpertRouteTestEntries.Reset();
	ExpertRouteTestPollAttempts = 0;
	GetWorldTimerManager().SetTimer(
		ExpertRouteTestPollHandle, this, &AKyokaiGameMode::PollForPawnThenRunExpertRouteTest, 0.2f, true);
}

void AKyokaiGameMode::PollForPawnThenRunExpertRouteTest()
{
	++ExpertRouteTestPollAttempts;

	APlayerController* PC = GetWorld() ? GetWorld()->GetFirstPlayerController() : nullptr;
	AKyokaiCharacter* Character = PC ? Cast<AKyokaiCharacter>(PC->GetPawn()) : nullptr;

	if (!Character && ExpertRouteTestPollAttempts < 25) // ~5s at 0.2s intervals
	{
		return;
	}

	GetWorldTimerManager().ClearTimer(ExpertRouteTestPollHandle);

	if (!Character)
	{
		FinishExpertRouteTest(TEXT("no pawn possessed within timeout"));
		return;
	}

	ExpertRouteTestController = PC;
	ExpertRouteTestCharacter = Character;

	// Sign_Seg3_3 (7950-8350, top=150) already running toward the jump
	// point - the entry jump itself (see Expert_Seg3_Upper's spawn
	// comment) is timed from x=8050, so start a bit before that.
	Character->SetActorLocation(FVector(7950.0f, 0.0f, 248.15f), false, nullptr, ETeleportType::TeleportPhysics);
	Character->GetKyokaiMovement()->StopMovementImmediately();

	ExpertRouteTestStartTime = GetWorld()->GetTimeSeconds();
	bExpertRouteJumpFired = false;
	bExpertRouteJumpFired2 = false;
	PC->InputKey(FInputKeyEventArgs::CreateSimulated(EKeys::D, IE_Pressed, 1.0f));
	ExpertRouteTestEntries.Add(TEXT("{\"step\": \"run_started\"}"));

	GetWorldTimerManager().SetTimer(ExpertRouteTestTickHandle, this, &AKyokaiGameMode::TickExpertRouteTest, 0.05f, true);
}

void AKyokaiGameMode::TickExpertRouteTest()
{
	APlayerController* PC = ExpertRouteTestController.Get();
	AKyokaiCharacter* Character = ExpertRouteTestCharacter.Get();
	if (!PC || !Character)
	{
		GetWorldTimerManager().ClearTimer(ExpertRouteTestTickHandle);
		FinishExpertRouteTest(TEXT("pawn or controller became invalid mid-test"));
		return;
	}

	const float Elapsed = GetWorld()->GetTimeSeconds() - ExpertRouteTestStartTime;
	const FVector Loc = Character->GetActorLocation();

	if (!bExpertRouteJumpFired && Loc.X >= 8050.f)
	{
		bExpertRouteJumpFired = true;
		PC->InputKey(FInputKeyEventArgs::CreateSimulated(EKeys::SpaceBar, IE_Pressed, 1.0f));
		PC->InputKey(FInputKeyEventArgs::CreateSimulated(EKeys::SpaceBar, IE_Released, 0.0f));
		ExpertRouteTestEntries.Add(TEXT("{\"step\": \"jump_fired\"}"));
	}

	// Second leg (Segment 4 extension): once back on the main path and
	// running through Roof_Seg4_Arena's flat, jump-free stretch
	// (10450-11450 - the same reason this arena was picked as the safe
	// spot for the new platform, see Expert_Seg4_Upper's spawn comment),
	// fire a second jump to reach Expert_Seg4_Upper. 10600 gives 550cm+
	// clearance past the main path's own 9200 jump landing (~10037), so
	// this entry can't interact with that arc at all.
	if (!bExpertRouteJumpFired2 && Loc.X >= 10600.f)
	{
		bExpertRouteJumpFired2 = true;
		PC->InputKey(FInputKeyEventArgs::CreateSimulated(EKeys::SpaceBar, IE_Pressed, 1.0f));
		PC->InputKey(FInputKeyEventArgs::CreateSimulated(EKeys::SpaceBar, IE_Released, 0.0f));
		ExpertRouteTestEntries.Add(TEXT("{\"step\": \"jump_fired_2\"}"));
	}

	static float LastDebugLogTime = -1000.0f;
	if (Elapsed - LastDebugLogTime >= 0.1f)
	{
		LastDebugLogTime = Elapsed;
		ExpertRouteTestEntries.Add(FString::Printf(
			TEXT("{\"step\": \"debug_trace\", \"elapsed_s\": %.2f, \"location_x\": %.2f, \"location_z\": %.2f, \"is_grounded\": %s}"),
			Elapsed, Loc.X, Loc.Z,
			Character->GetKyokaiMovement() && Character->GetKyokaiMovement()->IsMovingOnGround() ? TEXT("true") : TEXT("false")));
	}

	if (Loc.Z < -800.f)
	{
		GetWorldTimerManager().ClearTimer(ExpertRouteTestTickHandle);
		FinishExpertRouteTest(TEXT("fell through the level"));
		return;
	}

	// First-leg checkpoint (not the test's own success condition anymore -
	// logged so a trace still shows the Segment 3 landing happened, since
	// the test now continues past it): back down onto the main path
	// (Roof_Seg3_Landing, 8950-9300, top=150 - directly below
	// Expert_Seg3_Upper's far end at 9150), grounded.
	static bool bLoggedFirstLegLanding = false;
	if (!bLoggedFirstLegLanding && Loc.X >= 9160.f && Character->GetKyokaiMovement() && Character->GetKyokaiMovement()->IsMovingOnGround())
	{
		bLoggedFirstLegLanding = true;
		ExpertRouteTestEntries.Add(TEXT("{\"step\": \"first_leg_landed\"}"));
	}

	// Success: landed on Expert_Seg4_Upper (top=650, so Z>=600 rules out
	// still being on the arena floor below at capsule-center ~448).
	if (Loc.X >= 11000.f && Loc.Z >= 600.f && Character->GetKyokaiMovement() && Character->GetKyokaiMovement()->IsMovingOnGround())
	{
		GetWorldTimerManager().ClearTimer(ExpertRouteTestTickHandle);
		PC->InputKey(FInputKeyEventArgs::CreateSimulated(EKeys::D, IE_Released, 0.0f));
		FinishExpertRouteTest(TEXT("completed"));
		return;
	}

	if (Elapsed > 20.f)
	{
		GetWorldTimerManager().ClearTimer(ExpertRouteTestTickHandle);
		FinishExpertRouteTest(TEXT("timeout - likely stuck or fell through"));
	}
}

void AKyokaiGameMode::FinishExpertRouteTest(const FString& Outcome)
{
	FString Json = TEXT("{\n  \"outcome\": \"");
	Json += Outcome;
	Json += TEXT("\",\n  \"steps\": [\n");
	for (int32 Index = 0; Index < ExpertRouteTestEntries.Num(); ++Index)
	{
		Json += TEXT("    ");
		Json += ExpertRouteTestEntries[Index];
		Json += (Index + 1 < ExpertRouteTestEntries.Num()) ? TEXT(",\n") : TEXT("\n");
	}
	Json += TEXT("  ]\n}\n");

	const FString OutPath = FPaths::ProjectSavedDir() / TEXT("ExpertRouteTest.json");
	FFileHelper::SaveStringToFile(Json, *OutPath);

	FGenericPlatformMisc::RequestExit(false);
}

void AKyokaiGameMode::TryStartMasterySealTest()
{
	if (!FParse::Param(FCommandLine::Get(), TEXT("KyokaiMasterySealTest")))
	{
		return;
	}

	MasterySealTestEntries.Reset();
	MasterySealTestPollAttempts = 0;
	GetWorldTimerManager().SetTimer(
		MasterySealTestPollHandle, this, &AKyokaiGameMode::PollForPawnThenRunMasterySealTest, 0.2f, true);
}

void AKyokaiGameMode::PollForPawnThenRunMasterySealTest()
{
	++MasterySealTestPollAttempts;

	APlayerController* PC = GetWorld() ? GetWorld()->GetFirstPlayerController() : nullptr;
	AKyokaiCharacter* Character = PC ? Cast<AKyokaiCharacter>(PC->GetPawn()) : nullptr;

	if (!Character && MasterySealTestPollAttempts < 25) // ~5s at 0.2s intervals
	{
		return;
	}

	GetWorldTimerManager().ClearTimer(MasterySealTestPollHandle);

	if (!Character)
	{
		FinishMasterySealTest(TEXT("no pawn possessed within timeout"));
		return;
	}

	MasterySealTestController = PC;
	MasterySealTestCharacter = Character;

	// Same starting spot as ExpertRouteTest - Sign_Seg3_3, already running
	// toward Expert_Seg3_Upper's own existing entry jump (fires at x=8050).
	Character->SetActorLocation(FVector(7950.0f, 0.0f, 248.15f), false, nullptr, ETeleportType::TeleportPhysics);
	Character->GetKyokaiMovement()->StopMovementImmediately();

	MasterySealTestStartTime = GetWorld()->GetTimeSeconds();
	bMasteryJumpFired = false;
	bMasteryJumpAFired = false;
	bMasteryJumpBFired = false;
	bMasteryDash1Fired = false;
	bMasteryDash2Fired = false;
	bMasteryWasGroundedNearBounce = false;
	PC->InputKey(FInputKeyEventArgs::CreateSimulated(EKeys::D, IE_Pressed, 1.0f));
	MasterySealTestEntries.Add(TEXT("{\"step\": \"run_started\"}"));

	GetWorldTimerManager().SetTimer(MasterySealTestTickHandle, this, &AKyokaiGameMode::TickMasterySealTest, 0.05f, true);
}

void AKyokaiGameMode::TickMasterySealTest()
{
	APlayerController* PC = MasterySealTestController.Get();
	AKyokaiCharacter* Character = MasterySealTestCharacter.Get();
	if (!PC || !Character)
	{
		GetWorldTimerManager().ClearTimer(MasterySealTestTickHandle);
		FinishMasterySealTest(TEXT("pawn or controller became invalid mid-test"));
		return;
	}

	const float Elapsed = GetWorld()->GetTimeSeconds() - MasterySealTestStartTime;
	const FVector Loc = Character->GetActorLocation();
	const bool bGrounded = Character->GetKyokaiMovement() && Character->GetKyokaiMovement()->IsMovingOnGround();

	if (!bMasteryJumpFired && Loc.X >= 8050.f)
	{
		bMasteryJumpFired = true;
		PC->InputKey(FInputKeyEventArgs::CreateSimulated(EKeys::SpaceBar, IE_Pressed, 1.0f));
		PC->InputKey(FInputKeyEventArgs::CreateSimulated(EKeys::SpaceBar, IE_Released, 0.0f));
		MasterySealTestEntries.Add(TEXT("{\"step\": \"jump_expert_fired\"}"));
	}
	// Grounded-reactive, not just an X threshold: firing while still
	// airborne from the previous jump wastes the input entirely (no
	// double jump in this project) - confirmed happening with a plain X
	// trigger on the first test run, which silently skipped both new
	// jumps and just fell back onto the already-fixed Expert-edge fall.
	if (!bMasteryJumpAFired && Loc.X >= 8600.f && bGrounded)
	{
		bMasteryJumpAFired = true;
		PC->InputKey(FInputKeyEventArgs::CreateSimulated(EKeys::SpaceBar, IE_Pressed, 1.0f));
		PC->InputKey(FInputKeyEventArgs::CreateSimulated(EKeys::SpaceBar, IE_Released, 0.0f));
		MasterySealTestEntries.Add(TEXT("{\"step\": \"jump_a_fired\"}"));
	}
	if (!bMasteryJumpBFired && Loc.X >= 9050.f && bGrounded)
	{
		bMasteryJumpBFired = true;
		PC->InputKey(FInputKeyEventArgs::CreateSimulated(EKeys::SpaceBar, IE_Pressed, 1.0f));
		PC->InputKey(FInputKeyEventArgs::CreateSimulated(EKeys::SpaceBar, IE_Released, 0.0f));
		MasterySealTestEntries.Add(TEXT("{\"step\": \"jump_b_fired\"}"));
	}
	if (!bMasteryDash1Fired && Loc.X >= 9940.f)
	{
		bMasteryDash1Fired = true;
		PC->InputKey(FInputKeyEventArgs::CreateSimulated(EKeys::LeftShift, IE_Pressed, 1.0f));
		PC->InputKey(FInputKeyEventArgs::CreateSimulated(EKeys::LeftShift, IE_Released, 0.0f));
		MasterySealTestEntries.Add(TEXT("{\"step\": \"dash_1_fired\"}"));
	}
	// Second dash: fire the instant the character goes airborne again past
	// x=10900 (Mastery_Bounce sits at x=10960) - catches the bounce's own
	// launch as early as possible, matching the finale's own "fire the
	// dash right as you leave solid ground" timing principle.
	if (bMasteryDash1Fired && !bMasteryDash2Fired && Loc.X >= 10580.f)
	{
		if (bGrounded)
		{
			bMasteryWasGroundedNearBounce = true;
		}
		else if (bMasteryWasGroundedNearBounce)
		{
			bMasteryDash2Fired = true;
			PC->InputKey(FInputKeyEventArgs::CreateSimulated(EKeys::LeftShift, IE_Pressed, 1.0f));
			PC->InputKey(FInputKeyEventArgs::CreateSimulated(EKeys::LeftShift, IE_Released, 0.0f));
			MasterySealTestEntries.Add(TEXT("{\"step\": \"dash_2_fired\"}"));
		}
	}

	static float LastDebugLogTime = -1000.0f;
	if (Elapsed - LastDebugLogTime >= 0.05f)
	{
		LastDebugLogTime = Elapsed;
		MasterySealTestEntries.Add(FString::Printf(
			TEXT("{\"step\": \"debug_trace\", \"elapsed_s\": %.2f, \"location_x\": %.2f, \"location_z\": %.2f, \"is_grounded\": %s}"),
			Elapsed, Loc.X, Loc.Z, bGrounded ? TEXT("true") : TEXT("false")));
	}

	if (Loc.Z < -800.f)
	{
		GetWorldTimerManager().ClearTimer(MasterySealTestTickHandle);
		FinishMasterySealTest(TEXT("fell through the level"));
		return;
	}

	// No seal placed yet at this stage - success just means landing
	// safely somewhere past the bounce, so the exact landing spot can be
	// read from the trace and the seal placed there for a follow-up run.
	if (bMasteryDash2Fired && Loc.X >= 10641.f && bGrounded)
	{
		GetWorldTimerManager().ClearTimer(MasterySealTestTickHandle);
		PC->InputKey(FInputKeyEventArgs::CreateSimulated(EKeys::D, IE_Released, 0.0f));
		FinishMasterySealTest(TEXT("completed"));
		return;
	}

	if (Elapsed > 20.f)
	{
		GetWorldTimerManager().ClearTimer(MasterySealTestTickHandle);
		FinishMasterySealTest(TEXT("timeout - likely stuck or fell through"));
	}
}

void AKyokaiGameMode::FinishMasterySealTest(const FString& Outcome)
{
	FString Json = TEXT("{\n  \"outcome\": \"");
	Json += Outcome;
	Json += TEXT("\",\n  \"steps\": [\n");
	for (int32 Index = 0; Index < MasterySealTestEntries.Num(); ++Index)
	{
		Json += TEXT("    ");
		Json += MasterySealTestEntries[Index];
		Json += (Index + 1 < MasterySealTestEntries.Num()) ? TEXT(",\n") : TEXT("\n");
	}
	Json += TEXT("  ]\n}\n");

	const FString OutPath = FPaths::ProjectSavedDir() / TEXT("MasterySealTest.json");
	FFileHelper::SaveStringToFile(Json, *OutPath);

	FGenericPlatformMisc::RequestExit(false);
}
