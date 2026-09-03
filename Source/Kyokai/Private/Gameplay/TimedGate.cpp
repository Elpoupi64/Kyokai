// Copyright Epic Games, Inc. All Rights Reserved.

#include "Gameplay/TimedGate.h"

#include "Components/BoxComponent.h"
#include "Components/StaticMeshComponent.h"
#include "Engine/StaticMesh.h"
#include "Materials/MaterialInstanceDynamic.h"
#include "UObject/ConstructorHelpers.h"

ATimedGate::ATimedGate()
{
	PrimaryActorTick.bCanEverTick = true;

	GateVolume = CreateDefaultSubobject<UBoxComponent>(TEXT("GateVolume"));
	RootComponent = GateVolume;
	GateVolume->SetBoxExtent(FVector(30.0f, 200.0f, 250.0f));
	GateVolume->SetCollisionEnabled(ECollisionEnabled::NoCollision);

	GateMesh = CreateDefaultSubobject<UStaticMeshComponent>(TEXT("GateMesh"));
	GateMesh->SetupAttachment(GateVolume);
	GateMesh->SetMobility(EComponentMobility::Movable);
	GateMesh->SetRelativeScale3D(FVector(0.6f, 4.0f, 5.0f));
	// Blocks the pawn while closed; the Tick state machine flips this to
	// Ignore while Open. Everything else stays blocked always (a gate you
	// could still glide through with any other channel wouldn't be a real
	// wait), only the pawn response toggles.
	GateMesh->SetCollisionEnabled(ECollisionEnabled::QueryAndPhysics);
	GateMesh->SetCollisionResponseToAllChannels(ECR_Block);

	static ConstructorHelpers::FObjectFinder<UStaticMesh> CubeFinder(TEXT("/Engine/BasicShapes/Cube.Cube"));
	if (CubeFinder.Succeeded())
	{
		GateMesh->SetStaticMesh(CubeFinder.Object);
	}

	static ConstructorHelpers::FObjectFinder<UMaterialInterface> TelegraphMatFinder(TEXT("/Game/Materials/M_HazardTelegraph.M_HazardTelegraph"));
	if (TelegraphMatFinder.Succeeded())
	{
		GateMesh->SetMaterial(0, TelegraphMatFinder.Object);
	}
}

void ATimedGate::BeginPlay()
{
	Super::BeginPlay();

	if (GateMesh && GateMesh->GetMaterial(0))
	{
		VisualMID = UMaterialInstanceDynamic::Create(GateMesh->GetMaterial(0), this);
		GateMesh->SetMaterial(0, VisualMID);
	}

	EnterState(EGateState::Closed);
}

void ATimedGate::EnterState(const EGateState NewState)
{
	State = NewState;
	StateTimer = 0.0f;
	bIsOpeningTelegraph = (NewState == EGateState::OpeningTelegraph);
	bIsOpen = (NewState == EGateState::Open);

	// Pawn collision response is the one thing that actually gates passage;
	// every other actor's response stays Block from the constructor.
	if (GateMesh)
	{
		GateMesh->SetCollisionResponseToChannel(ECC_Pawn, bIsOpen ? ECR_Ignore : ECR_Block);
	}

	UpdateVisualTint();
}

void ATimedGate::UpdateVisualTint()
{
	if (!VisualMID)
	{
		return;
	}

	// Deep red while solid/closed, bright amber through the opening
	// telegraph, and a translucent-reading pale green while actually open -
	// deliberately a different palette from WindGust/LightningStrike's
	// blue/yellow/cyan so "you can't pass yet" reads as its own category at
	// a glance, not just another instance of an existing hazard tone.
	FLinearColor Tint = FLinearColor(0.6f, 0.05f, 0.05f);
	if (State == EGateState::OpeningTelegraph)
	{
		Tint = FLinearColor(1.0f, 0.6f, 0.05f);
	}
	else if (State == EGateState::Open)
	{
		Tint = FLinearColor(0.15f, 0.9f, 0.35f);
	}
	VisualMID->SetVectorParameterValue(TEXT("TintColor"), Tint);
}

void ATimedGate::Tick(const float DeltaTime)
{
	Super::Tick(DeltaTime);

	StateTimer += DeltaTime;

	switch (State)
	{
	case EGateState::Closed:
		if (StateTimer >= FMath::Max(ClosedDuration - OpeningTelegraphDuration, 0.0f))
		{
			EnterState(EGateState::OpeningTelegraph);
		}
		break;

	case EGateState::OpeningTelegraph:
		if (StateTimer >= OpeningTelegraphDuration)
		{
			EnterState(EGateState::Open);
		}
		break;

	case EGateState::Open:
		if (StateTimer >= OpenDuration)
		{
			EnterState(EGateState::Closed);
		}
		break;
	}
}
