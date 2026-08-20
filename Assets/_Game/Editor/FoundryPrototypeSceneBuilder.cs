#if UNITY_EDITOR

using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class FoundryPrototypeSceneBuilder
{
    private const string SceneFolder = "Assets/_Game/Scenes/Prototype";
    private const string ScenePath = SceneFolder + "/Foundry_Prototype.unity";

    private const string InputFolder = "Assets/_Game/Input";
    private const string InputAssetPath = InputFolder + "/PlayerControls.asset";

    private const string PrefabFolder = "Assets/_Game/Prefabs/Characters/Kenjiro";
    private const string KenjiroPrefabPath = PrefabFolder + "/Kenjiro_Prototype.prefab";

    private const string MaterialFolder = "Assets/_Game/Art/Materials/Prototype";

    static FoundryPrototypeSceneBuilder()
    {
        EditorApplication.delayCall += AutoUpgradeIfNeeded;
    }

    [MenuItem("Tools/Katsuhiro/Create or Rebuild Foundry Prototype")]
    public static void CreateOrRebuild()
    {
        EnsureFolder("Assets/_Game");
        EnsureFolder("Assets/_Game/Scenes");
        EnsureFolder(SceneFolder);
        EnsureFolder(InputFolder);
        EnsureFolder("Assets/_Game/Prefabs");
        EnsureFolder("Assets/_Game/Prefabs/Characters");
        EnsureFolder(PrefabFolder);
        EnsureFolder("Assets/_Game/Art");
        EnsureFolder("Assets/_Game/Art/Materials");
        EnsureFolder(MaterialFolder);

        int groundLayer = EnsureLayer("Ground");
        int enemyLayer = EnsureLayer("Enemy");

        InputActionAsset inputAsset = GetOrCreateInputActions();
        GameObject kenjiroPrefab = GetOrCreateKenjiroPrefab();

        kenjiroPrefab =
            KenjiroProductionCharacterBuilder.UpgradePrefab(
                kenjiroPrefab
            );

        GameObject doryokuPrefab =
            Doryoku3PrototypeBuilder.GetOrCreateDoryoku3Prefab(enemyLayer);

        Scene scene = EditorSceneManager.NewScene(
            NewSceneSetup.EmptyScene,
            NewSceneMode.Single
        );

        GameObject gameplayRoot = new GameObject("GAMEPLAY");
        GameObject levelRoot = new GameObject("LEVEL");
        GameObject cameraRoot = new GameObject("CAMERA");
        GameObject lightingRoot = new GameObject("LIGHTING");

        GameObject hitStopObject =
            new GameObject("HitStopManager");

        hitStopObject.transform.SetParent(
            gameplayRoot.transform
        );

        hitStopObject.AddComponent<HitStopManager>();

        GameObject worldObject = new GameObject("KikaiWorld");
        worldObject.transform.SetParent(gameplayRoot.transform);
        KikaiWorldManager worldManager = worldObject.AddComponent<KikaiWorldManager>();

        GameObject hudObject = new GameObject("PrototypeHUD");
        hudObject.transform.SetParent(gameplayRoot.transform);
        PrototypeWorldModeHUD hud = hudObject.AddComponent<PrototypeWorldModeHUD>();
        SerializedObject hudSO = new SerializedObject(hud);
        hudSO.FindProperty("worldManager").objectReferenceValue = worldManager;
        hudSO.ApplyModifiedPropertiesWithoutUndo();

        GameObject player = CreatePlayer(
            gameplayRoot.transform,
            inputAsset,
            kenjiroPrefab,
            worldManager,
            groundLayer,
            enemyLayer
        );

        CreateCamera(cameraRoot.transform, player.transform);

        VerticalSliceDirector sliceDirector =
            FoundryVerticalSliceBuilder.BuildSlice(
                levelRoot.transform,
                gameplayRoot.transform,
                player.transform,
                doryokuPrefab,
                worldManager,
                groundLayer,
                enemyLayer
            );

        Doryoku3MiniBossBuilder.AddMiniBossToScene(
            levelRoot.transform,
            doryokuPrefab,
            player.transform,
            enemyLayer,
            groundLayer
        );

        FoundryVerticalSliceBuilder.FinalizeSlice(
            sliceDirector
        );

        KatsuhiroV16GameplayPolishBuilder.Apply(
            player
        );

        KatsuhiroV17FinalPolishBuilder.Apply(
            player
        );

        KatsuhiroV171LevelPacingBuilder.Apply(
            levelRoot.transform,
            gameplayRoot.transform,
            player.transform,
            doryokuPrefab,
            worldManager,
            groundLayer,
            enemyLayer,
            sliceDirector
        );

        CreateLighting(lightingRoot.transform);

        FoundrySteampunkArtBuilder.Decorate(
            levelRoot.transform
        );

        FoundryModularArtPassBuilder.Apply(
            levelRoot.transform
        );

        FoundryFinalArtPassBuilder.Apply(
            levelRoot.transform
        );

        FoundryHeroAssetsPassBuilder.Apply(
            levelRoot.transform
        );

        FoundryTruePaintedAtmosphereBuilder.Apply(
            levelRoot.transform
        );

        KatsuhiroProductionOptimizationBuilder.Apply(
            levelRoot.transform,
            gameplayRoot.transform,
            player.transform
        );

        KatsuhiroV16ReleaseCandidateBuilder.Apply(
            gameplayRoot.transform
        );

        KatsuhiroV17ExternalPlaytestBuilder.Apply(
            gameplayRoot.transform
        );

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, ScenePath);
        AddSceneToBuildSettings(ScenePath);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Selection.activeGameObject = player;
        EditorGUIUtility.PingObject(
            AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath)
        );

        Debug.Log(
            "Katsuhiro v17.1 : Level Pacing 8–10 Minutes, " +
            "avec chaîne 4 possédée, poursuite enrichie et métriques de pacing."
        );
    }

    [MenuItem("Tools/Katsuhiro/Rebuild Kenjiro Prototype Prefab")]
    public static void RebuildKenjiroPrefab()
    {
        EnsureFolder("Assets/_Game/Prefabs");
        EnsureFolder("Assets/_Game/Prefabs/Characters");
        EnsureFolder(PrefabFolder);
        EnsureFolder("Assets/_Game/Art");
        EnsureFolder("Assets/_Game/Art/Materials");
        EnsureFolder(MaterialFolder);

        AssetDatabase.DeleteAsset(KenjiroPrefabPath);
        GetOrCreateKenjiroPrefab();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorGUIUtility.PingObject(
            AssetDatabase.LoadAssetAtPath<GameObject>(KenjiroPrefabPath)
        );
    }

    private static void AutoUpgradeIfNeeded()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
            return;

        GameObject prefab =
            AssetDatabase.LoadAssetAtPath<GameObject>(KenjiroPrefabPath);

        if (prefab == null)
            CreateOrRebuild();
    }

    private static GameObject CreatePlayer(
        Transform parent,
        InputActionAsset inputAsset,
        GameObject kenjiroPrefab,
        KikaiWorldManager worldManager,
        int groundLayer,
        int enemyLayer
    )
    {
        GameObject player = new GameObject("Player_Kenjiro");
        player.transform.SetParent(parent);
        player.transform.position = new Vector3(0f, 0.5f, 0f);
        player.tag = "Player";

        Rigidbody rb = player.AddComponent<Rigidbody>();
        rb.mass = 1f;
        rb.useGravity = true;
        rb.isKinematic = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.constraints =
            RigidbodyConstraints.FreezePositionZ |
            RigidbodyConstraints.FreezeRotation;

        CapsuleCollider physicsCollider = player.AddComponent<CapsuleCollider>();
        physicsCollider.height = 2f;
        physicsCollider.radius = 0.45f;
        physicsCollider.center = Vector3.zero;

        if (inputAsset == null)
        {
            // The reference captured earlier in CreateOrRebuild() can go
            // stale (Unity's "fake null") if anything reimported the asset
            // in between — e.g. prefab builders running after it was
            // fetched. Reload fresh from disk rather than assign a dead
            // reference.
            inputAsset =
                AssetDatabase.LoadAssetAtPath<InputActionAsset>(
                    InputAssetPath
                );
        }

        PlayerInput playerInput = player.AddComponent<PlayerInput>();
        playerInput.actions = inputAsset;
        playerInput.defaultActionMap = "Player";
        playerInput.neverAutoSwitchControlSchemes = false;

        player.AddComponent<GameplayPlane25D>();
        player.AddComponent<PlayerHealth>();

        GameObject visual =
            PrefabUtility.InstantiatePrefab(
                kenjiroPrefab,
                player.transform
            ) as GameObject;

        visual.name = "Visual_Kenjiro";
        visual.transform.localPosition = Vector3.zero;
        visual.transform.localRotation = Quaternion.identity;
        visual.transform.localScale = Vector3.one;

        Animator visualAnimator =
            visual.GetComponent<Animator>();

        if (visualAnimator == null)
            visualAnimator =
                visual.AddComponent<Animator>();

        visualAnimator.runtimeAnimatorController =
            KenjiroAnimatorBuilder.GetOrCreateController();

        visualAnimator.applyRootMotion = false;

        KenjiroAnimatorDriver animatorDriver =
            player.AddComponent<KenjiroAnimatorDriver>();

        SerializedObject animatorDriverSO =
            new SerializedObject(animatorDriver);

        animatorDriverSO.FindProperty("animator").objectReferenceValue =
            visualAnimator;

        animatorDriverSO.ApplyModifiedPropertiesWithoutUndo();

        GameObject groundCheck = new GameObject("GroundCheck");
        groundCheck.transform.SetParent(player.transform);
        groundCheck.transform.localPosition = new Vector3(0f, -1.05f, 0f);

        GameObject attackPoint = new GameObject("AttackPoint");
        attackPoint.transform.SetParent(player.transform);
        attackPoint.transform.localPosition = new Vector3(1.0f, 0.0f, 0f);

        GameObject abilityOrigin = new GameObject("AbilityOrigin");
        abilityOrigin.transform.SetParent(player.transform);
        abilityOrigin.transform.localPosition = new Vector3(0.55f, 0.30f, 0f);

        PlayerMotor25D motor = player.AddComponent<PlayerMotor25D>();
        SerializedObject motorSO = new SerializedObject(motor);
        motorSO.FindProperty("groundCheck").objectReferenceValue = groundCheck.transform;
        motorSO.FindProperty("groundLayer").intValue = 1 << groundLayer;
        motorSO.FindProperty("visualRoot").objectReferenceValue = visual.transform;
        motorSO.FindProperty("facingRightAngle").floatValue = 0f;
        motorSO.FindProperty("facingLeftAngle").floatValue = 180f;
        motorSO.ApplyModifiedPropertiesWithoutUndo();

        Transform modelRoot =
            visual.transform.Find("ModelRoot");

        Transform torso =
            visual.transform.Find("ModelRoot/Coat_Torso");

        Transform frontArm =
            visual.transform.Find("ModelRoot/Arm_Front");

        Transform backArm =
            visual.transform.Find("ModelRoot/Arm_Back");

        Transform deviceForCombat =
            visual.transform.Find("ModelRoot/KikaiYurei_Device");

        Transform deviceLightForCombat =
            visual.transform.Find("ModelRoot/KikaiYurei_Device/KikaiLight");

        Material combatSparkMaterial =
            GetOrCreateMaterial(
                "Kenjiro_CombatSparks",
                new Color(0.58f, 0.20f, 0.04f),
                true,
                new Color(1.00f, 0.38f, 0.06f) * 2.8f
            );

        Material combatEtherMaterial =
            GetOrCreateMaterial(
                "Kenjiro_CombatEther",
                new Color(0.04f, 0.42f, 0.52f),
                true,
                new Color(0.08f, 1.00f, 1.00f) * 3.0f
            );

        Material dustMaterial =
            GetOrCreateMaterial(
                "Kenjiro_Dust",
                new Color(0.34f, 0.28f, 0.22f),
                false,
                Color.black
            );

        GameObject trailObject =
            new GameObject("AttackTrail");

        trailObject.transform.SetParent(
            frontArm != null
                ? frontArm
                : visual.transform
        );

        trailObject.transform.localPosition =
            new Vector3(0f, -0.45f, -0.08f);

        TrailRenderer trailRenderer =
            trailObject.AddComponent<TrailRenderer>();

        trailRenderer.time = 0.16f;
        trailRenderer.startWidth = 0.12f;
        trailRenderer.endWidth = 0.01f;
        trailRenderer.minVertexDistance = 0.025f;
        trailRenderer.sharedMaterial = combatEtherMaterial;
        trailRenderer.emitting = false;

        AttackTrail attackTrail =
            player.AddComponent<AttackTrail>();

        SerializedObject attackTrailSO =
            new SerializedObject(attackTrail);

        attackTrailSO.FindProperty("trail").objectReferenceValue =
            trailRenderer;

        attackTrailSO.ApplyModifiedPropertiesWithoutUndo();

        DodgeFX dodgeFX =
            player.AddComponent<DodgeFX>();

        SerializedObject dodgeFXSO =
            new SerializedObject(dodgeFX);

        dodgeFXSO.FindProperty("material").objectReferenceValue =
            dustMaterial;

        dodgeFXSO.FindProperty("origin").objectReferenceValue =
            groundCheck.transform;

        dodgeFXSO.ApplyModifiedPropertiesWithoutUndo();

        LandingFX landingFX =
            player.AddComponent<LandingFX>();

        SerializedObject landingFXSO =
            new SerializedObject(landingFX);

        landingFXSO.FindProperty("motor").objectReferenceValue =
            motor;

        landingFXSO.FindProperty("origin").objectReferenceValue =
            groundCheck.transform;

        landingFXSO.FindProperty("dustMaterial").objectReferenceValue =
            dustMaterial;

        landingFXSO.ApplyModifiedPropertiesWithoutUndo();

        KikaiSpecialFX specialFX =
            player.AddComponent<KikaiSpecialFX>();

        SerializedObject specialFXSO =
            new SerializedObject(specialFX);

        specialFXSO.FindProperty("origin").objectReferenceValue =
            abilityOrigin.transform;

        specialFXSO.FindProperty("material").objectReferenceValue =
            combatEtherMaterial;

        if (deviceLightForCombat != null)
        {
            specialFXSO.FindProperty("deviceLight").objectReferenceValue =
                deviceLightForCombat.GetComponent<Light>();
        }

        specialFXSO.ApplyModifiedPropertiesWithoutUndo();

        CombatImpactFX impactFX =
            player.AddComponent<CombatImpactFX>();

        SerializedObject impactFXSO =
            new SerializedObject(impactFX);

        impactFXSO.FindProperty("mechanicalMaterial").objectReferenceValue =
            combatSparkMaterial;

        impactFXSO.FindProperty("etherealMaterial").objectReferenceValue =
            combatEtherMaterial;

        impactFXSO.ApplyModifiedPropertiesWithoutUndo();

        KenjiroCombatVisuals combatVisuals =
            player.AddComponent<KenjiroCombatVisuals>();

        SerializedObject combatVisualSO =
            new SerializedObject(combatVisuals);

        combatVisualSO.FindProperty("animatorDriver").objectReferenceValue =
            animatorDriver;

        combatVisualSO.FindProperty("attackTrail").objectReferenceValue =
            attackTrail;

        combatVisualSO.FindProperty("dodgeFX").objectReferenceValue =
            dodgeFX;

        combatVisualSO.FindProperty("kikaiSpecialFX").objectReferenceValue =
            specialFX;

        combatVisualSO.ApplyModifiedPropertiesWithoutUndo();

        Material kenjiroSpecialMaterial =
            GetOrCreateMaterial(
                "Kenjiro_KikaiBurst",
                new Color(0.05f, 0.48f, 0.60f),
                true,
                new Color(0.10f, 1.00f, 1.00f) * 3.2f
            );

        KenjiroCombatController combat =
            player.AddComponent<KenjiroCombatController>();

        SerializedObject combatSO =
            new SerializedObject(combat);

        combatSO.FindProperty("attackPoint").objectReferenceValue =
            attackPoint.transform;

        combatSO.FindProperty("specialOrigin").objectReferenceValue =
            abilityOrigin.transform;

        combatSO.FindProperty("enemyLayer").intValue =
            1 << enemyLayer;

        combatSO.FindProperty("motor").objectReferenceValue =
            motor;

        combatSO.FindProperty("health").objectReferenceValue =
            player.GetComponent<PlayerHealth>();

        combatSO.FindProperty("visuals").objectReferenceValue =
            combatVisuals;

        combatSO.FindProperty("impactFX").objectReferenceValue =
            impactFX;

        combatSO.FindProperty("worldManager").objectReferenceValue =
            worldManager;

        combatSO.FindProperty("specialProjectileMaterial").objectReferenceValue =
            kenjiroSpecialMaterial;

        combatSO.ApplyModifiedPropertiesWithoutUndo();

        GameObject combatMarker =
            new GameObject("CombatSystem_v7");

        combatMarker.transform.SetParent(player.transform);

        KenjiroCombatHUD combatHUD =
            combatMarker.AddComponent<KenjiroCombatHUD>();

        SerializedObject hudSO =
            new SerializedObject(combatHUD);

        hudSO.FindProperty("combat").objectReferenceValue =
            combat;

        hudSO.FindProperty("health").objectReferenceValue =
            player.GetComponent<PlayerHealth>();

        hudSO.FindProperty("worldManager").objectReferenceValue =
            worldManager;

        hudSO.ApplyModifiedPropertiesWithoutUndo();

        KikaiYureiController kikai = player.AddComponent<KikaiYureiController>();
        SerializedObject kikaiSO = new SerializedObject(kikai);

        Transform device =
            visual.transform.Find("ModelRoot/KikaiYurei_Device");

        Transform etherCore =
            visual.transform.Find("ModelRoot/KikaiYurei_Device/EtherCore");

        Transform kikaiLight =
            visual.transform.Find("ModelRoot/KikaiYurei_Device/KikaiLight");

        kikaiSO.FindProperty("worldManager").objectReferenceValue = worldManager;
        kikaiSO.FindProperty("deviceTransform").objectReferenceValue = device;

        if (etherCore != null)
            kikaiSO.FindProperty("etherCoreRenderer").objectReferenceValue =
                etherCore.GetComponent<Renderer>();

        if (kikaiLight != null)
            kikaiSO.FindProperty("deviceLight").objectReferenceValue =
                kikaiLight.GetComponent<Light>();

        kikaiSO.ApplyModifiedPropertiesWithoutUndo();

        KenjiroDamageReaction damageReaction =
            player.AddComponent<KenjiroDamageReaction>();

        SerializedObject reactionSO =
            new SerializedObject(damageReaction);

        reactionSO.FindProperty("health").objectReferenceValue =
            player.GetComponent<PlayerHealth>();

        reactionSO.FindProperty("animatorDriver").objectReferenceValue =
            animatorDriver;

        reactionSO.FindProperty("visualRoot").objectReferenceValue =
            visual.transform;

        reactionSO.ApplyModifiedPropertiesWithoutUndo();

        GameObject polishMarker =
            new GameObject("CombatPolish_v8");

        polishMarker.transform.SetParent(
            player.transform
        );

        return player;
    }

    private static void CreateLevel(Transform parent, int groundLayer)
    {
        Material groundMaterial = GetOrCreateMaterial(
            "Foundry_Iron",
            new Color(0.24f, 0.20f, 0.17f),
            false,
            Color.black
        );

        Material normalMachineMaterial = GetOrCreateMaterial(
            "Foundry_Machine",
            new Color(0.18f, 0.16f, 0.15f),
            false,
            Color.black
        );

        Material etherMaterial = GetOrCreateMaterial(
            "Ethereal_Cyan",
            new Color(0.06f, 0.35f, 0.42f),
            true,
            new Color(0.10f, 1.00f, 1.00f) * 2.2f
        );

        Material etherVioletMaterial = GetOrCreateMaterial(
            "Ethereal_Violet",
            new Color(0.25f, 0.08f, 0.34f),
            true,
            new Color(0.70f, 0.15f, 1.00f) * 1.8f
        );

        GameObject persistent = new GameObject("FoundryPersistent");
        persistent.transform.SetParent(parent);

        GameObject groundRoot = new GameObject("Ground");
        groundRoot.transform.SetParent(persistent.transform);

        CreateBlock(
            "Ground_Main",
            groundRoot.transform,
            new Vector3(0f, -1f, 0f),
            new Vector3(34f, 1f, 4f),
            groundLayer,
            groundMaterial,
            true
        );

        GameObject platforms = new GameObject("Platforms");
        platforms.transform.SetParent(persistent.transform);

        CreateBlock(
            "Platform_01",
            platforms.transform,
            new Vector3(-8f, 1.0f, 0f),
            new Vector3(4f, 0.5f, 4f),
            groundLayer,
            groundMaterial,
            true
        );

        CreateBlock(
            "Platform_02",
            platforms.transform,
            new Vector3(-2f, 2.5f, 0f),
            new Vector3(4f, 0.5f, 4f),
            groundLayer,
            groundMaterial,
            true
        );

        CreateBlock(
            "Platform_03",
            platforms.transform,
            new Vector3(6f, 1.25f, 0f),
            new Vector3(5f, 0.5f, 4f),
            groundLayer,
            groundMaterial,
            true
        );

        CreateBlock(
            "Platform_04",
            platforms.transform,
            new Vector3(13f, 3.0f, 0f),
            new Vector3(4f, 0.5f, 4f),
            groundLayer,
            groundMaterial,
            true
        );

        // Normal-only foundry machinery in the background.
        GameObject normalLayer = new GameObject("NormalLayer_Machinery");
        normalLayer.transform.SetParent(parent);

        CreateBlock(
            "Boiler_Normal",
            normalLayer.transform,
            new Vector3(2f, 2.2f, 5f),
            new Vector3(3f, 4.5f, 2.5f),
            0,
            normalMachineMaterial,
            false
        );

        CreateBlock(
            "FactoryTower_Normal",
            normalLayer.transform,
            new Vector3(10f, 4f, 8f),
            new Vector3(3.5f, 8f, 3f),
            0,
            normalMachineMaterial,
            false
        );

        KikaiWorldVisibility normalVisibility =
            normalLayer.AddComponent<KikaiWorldVisibility>();

        SerializedObject normalSO = new SerializedObject(normalVisibility);
        normalSO.FindProperty("visibilityMode").enumValueIndex =
            (int)KikaiVisibilityMode.NormalOnly;
        normalSO.FindProperty("affectColliders").boolValue = false;
        normalSO.ApplyModifiedPropertiesWithoutUndo();

        // Ethereal-only layer: real platforms + spectral forms.
        GameObject etherealLayer = new GameObject("EtherealLayer");
        etherealLayer.transform.SetParent(parent);

        CreateBlock(
            "SpiritPlatform_01",
            etherealLayer.transform,
            new Vector3(-5f, 4.1f, 0f),
            new Vector3(3.5f, 0.45f, 4f),
            groundLayer,
            etherMaterial,
            true
        );

        CreateBlock(
            "SpiritPlatform_02",
            etherealLayer.transform,
            new Vector3(2.5f, 4.6f, 0f),
            new Vector3(3.8f, 0.45f, 4f),
            groundLayer,
            etherMaterial,
            true
        );

        CreateBlock(
            "SpiritPlatform_03",
            etherealLayer.transform,
            new Vector3(8.5f, 5.2f, 0f),
            new Vector3(3.8f, 0.45f, 4f),
            groundLayer,
            etherMaterial,
            true
        );

        GameObject spiritA = CreateVisualPrimitive(
            PrimitiveType.Sphere,
            "YokaiEcho_01",
            etherealLayer.transform,
            new Vector3(0f, 2.4f, 4f),
            new Vector3(1.5f, 2.4f, 1.1f),
            etherVioletMaterial
        );

        GameObject spiritB = CreateVisualPrimitive(
            PrimitiveType.Sphere,
            "YokaiEcho_02",
            etherealLayer.transform,
            new Vector3(11f, 3.0f, 5.5f),
            new Vector3(1.2f, 1.8f, 1.0f),
            etherMaterial
        );

        CreateVisualPrimitive(
            PrimitiveType.Cylinder,
            "EtherRift_Column",
            etherealLayer.transform,
            new Vector3(15f, 3.5f, 6f),
            new Vector3(0.7f, 3.5f, 0.7f),
            etherVioletMaterial
        );

        KikaiWorldVisibility etherealVisibility =
            etherealLayer.AddComponent<KikaiWorldVisibility>();

        SerializedObject etherealSO = new SerializedObject(etherealVisibility);
        etherealSO.FindProperty("visibilityMode").enumValueIndex =
            (int)KikaiVisibilityMode.EtherealOnly;
        etherealSO.FindProperty("affectColliders").boolValue = true;
        etherealSO.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void CreateCamera(Transform parent, Transform target)
    {
        GameObject cameraObject = new GameObject("Main Camera");
        cameraObject.transform.SetParent(parent);
        cameraObject.tag = "MainCamera";
        cameraObject.transform.position = new Vector3(0f, 2f, -10f);
        cameraObject.transform.rotation = Quaternion.identity;

        Camera camera = cameraObject.AddComponent<Camera>();
        camera.fieldOfView = 50f;
        camera.nearClipPlane = 0.1f;
        camera.farClipPlane = 250f;
        camera.backgroundColor = new Color(0.09f, 0.08f, 0.07f);

        cameraObject.AddComponent<AudioListener>();

        CameraFollow25D follow = cameraObject.AddComponent<CameraFollow25D>();
        SerializedObject followSO = new SerializedObject(follow);
        followSO.FindProperty("target").objectReferenceValue = target;
        followSO.FindProperty("offset").vector3Value = new Vector3(0f, 2f, -10f);
        followSO.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void CreateLighting(Transform parent)
    {
        GameObject lightObject = new GameObject("Directional Light");
        lightObject.transform.SetParent(parent);
        lightObject.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

        Light light = lightObject.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 1.15f;
        light.shadows = LightShadows.Soft;
        light.color = new Color(1.0f, 0.78f, 0.58f);
    }

    private static GameObject GetOrCreateKenjiroPrefab()
    {
        GameObject existing =
            AssetDatabase.LoadAssetAtPath<GameObject>(KenjiroPrefabPath);

        if (existing != null)
            return existing;

        Material suit = GetOrCreateMaterial(
            "Kenjiro_Suit",
            new Color(0.07f, 0.09f, 0.12f),
            false,
            Color.black
        );

        Material shirt = GetOrCreateMaterial(
            "Kenjiro_Shirt",
            new Color(0.78f, 0.76f, 0.68f),
            false,
            Color.black
        );

        Material skin = GetOrCreateMaterial(
            "Kenjiro_Skin",
            new Color(0.72f, 0.52f, 0.39f),
            false,
            Color.black
        );

        Material hair = GetOrCreateMaterial(
            "Kenjiro_Hair",
            new Color(0.03f, 0.035f, 0.045f),
            false,
            Color.black
        );

        Material leather = GetOrCreateMaterial(
            "Kenjiro_Leather",
            new Color(0.22f, 0.10f, 0.045f),
            false,
            Color.black
        );

        Material brass = GetOrCreateMaterial(
            "Kenjiro_Brass",
            new Color(0.45f, 0.27f, 0.08f),
            false,
            Color.black
        );

        Material ether = GetOrCreateMaterial(
            "Kenjiro_EtherCore",
            new Color(0.06f, 0.45f, 0.60f),
            true,
            new Color(0.10f, 0.90f, 1.00f) * 2.5f
        );

        GameObject root = new GameObject("Kenjiro_Prototype");
        root.AddComponent<Animator>();

        GameObject modelRoot = new GameObject("ModelRoot");
        modelRoot.transform.SetParent(root.transform);
        modelRoot.transform.localPosition = Vector3.zero;

        // Torso / coat.
        CreateVisualPrimitive(
            PrimitiveType.Cube,
            "Coat_Torso",
            modelRoot.transform,
            new Vector3(0f, 0.15f, 0f),
            new Vector3(0.62f, 0.88f, 0.38f),
            suit
        );

        CreateVisualPrimitive(
            PrimitiveType.Cube,
            "Shirt_Front",
            modelRoot.transform,
            new Vector3(0.03f, 0.28f, -0.205f),
            new Vector3(0.28f, 0.55f, 0.035f),
            shirt
        );

        // Head and hair.
        CreateVisualPrimitive(
            PrimitiveType.Sphere,
            "Head",
            modelRoot.transform,
            new Vector3(0.02f, 0.91f, 0f),
            new Vector3(0.50f, 0.56f, 0.50f),
            skin
        );

        CreateVisualPrimitive(
            PrimitiveType.Sphere,
            "Hair",
            modelRoot.transform,
            new Vector3(-0.04f, 1.12f, 0.02f),
            new Vector3(0.58f, 0.34f, 0.54f),
            hair
        );

        // Legs.
        GameObject leftLeg = CreateVisualPrimitive(
            PrimitiveType.Capsule,
            "Leg_Back",
            modelRoot.transform,
            new Vector3(-0.17f, -0.62f, 0.08f),
            new Vector3(0.24f, 0.53f, 0.24f),
            suit
        );

        GameObject rightLeg = CreateVisualPrimitive(
            PrimitiveType.Capsule,
            "Leg_Front",
            modelRoot.transform,
            new Vector3(0.18f, -0.62f, -0.08f),
            new Vector3(0.24f, 0.53f, 0.24f),
            suit
        );

        CreateVisualPrimitive(
            PrimitiveType.Cube,
            "Shoe_Back",
            modelRoot.transform,
            new Vector3(-0.11f, -1.02f, 0.08f),
            new Vector3(0.40f, 0.18f, 0.30f),
            hair
        );

        CreateVisualPrimitive(
            PrimitiveType.Cube,
            "Shoe_Front",
            modelRoot.transform,
            new Vector3(0.25f, -1.02f, -0.08f),
            new Vector3(0.40f, 0.18f, 0.30f),
            hair
        );

        // Arms.
        GameObject backArm = CreateVisualPrimitive(
            PrimitiveType.Capsule,
            "Arm_Back",
            modelRoot.transform,
            new Vector3(-0.34f, 0.16f, 0.12f),
            new Vector3(0.18f, 0.48f, 0.18f),
            suit
        );
        backArm.transform.localRotation = Quaternion.Euler(0f, 0f, -10f);

        GameObject frontArm = CreateVisualPrimitive(
            PrimitiveType.Capsule,
            "Arm_Front",
            modelRoot.transform,
            new Vector3(0.38f, 0.14f, -0.13f),
            new Vector3(0.18f, 0.48f, 0.18f),
            suit
        );
        frontArm.transform.localRotation = Quaternion.Euler(0f, 0f, 18f);

        // Satchel: silhouette immediately associated with Kenjiro.
        CreateVisualPrimitive(
            PrimitiveType.Cube,
            "Leather_Satchel",
            modelRoot.transform,
            new Vector3(-0.38f, 0.05f, 0.28f),
            new Vector3(0.48f, 0.62f, 0.20f),
            leather
        );

        // Kikai-Yurei prototype device.
        GameObject device = new GameObject("KikaiYurei_Device");
        device.transform.SetParent(modelRoot.transform);
        device.transform.localPosition = new Vector3(0.48f, 0.28f, -0.32f);

        GameObject housing = CreateVisualPrimitive(
            PrimitiveType.Cylinder,
            "Housing",
            device.transform,
            Vector3.zero,
            new Vector3(0.24f, 0.16f, 0.24f),
            brass
        );
        housing.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);

        CreateVisualPrimitive(
            PrimitiveType.Sphere,
            "EtherCore",
            device.transform,
            new Vector3(0f, 0f, -0.10f),
            new Vector3(0.20f, 0.20f, 0.20f),
            ether
        );

        GameObject lightObject = new GameObject("KikaiLight");
        lightObject.transform.SetParent(device.transform);
        lightObject.transform.localPosition = new Vector3(0f, 0f, -0.18f);

        Light kikaiLight = lightObject.AddComponent<Light>();
        kikaiLight.type = LightType.Point;
        kikaiLight.range = 4.0f;
        kikaiLight.intensity = 0.5f;
        kikaiLight.color = new Color(0.10f, 0.75f, 1.00f);
        kikaiLight.shadows = LightShadows.None;

        GameObject prefab =
            PrefabUtility.SaveAsPrefabAsset(root, KenjiroPrefabPath);

        Object.DestroyImmediate(root);

        return prefab;
    }

    private static Material GetOrCreateMaterial(
        string materialName,
        Color baseColor,
        bool emission,
        Color emissionColor
    )
    {
        string path = MaterialFolder + "/" + materialName + ".mat";

        Material existing = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (existing != null)
            return existing;

        Shader shader = Shader.Find("Universal Render Pipeline/Lit");

        if (shader == null)
            shader = Shader.Find("Standard");

        Material material = new Material(shader);
        material.name = materialName;

        if (material.HasProperty("_BaseColor"))
            material.SetColor("_BaseColor", baseColor);

        if (material.HasProperty("_Color"))
            material.SetColor("_Color", baseColor);

        if (emission && material.HasProperty("_EmissionColor"))
        {
            material.EnableKeyword("_EMISSION");
            material.SetColor("_EmissionColor", emissionColor);
        }

        AssetDatabase.CreateAsset(material, path);
        return material;
    }

    private static GameObject CreateVisualPrimitive(
        PrimitiveType primitiveType,
        string name,
        Transform parent,
        Vector3 localPosition,
        Vector3 localScale,
        Material material
    )
    {
        GameObject obj = GameObject.CreatePrimitive(primitiveType);
        obj.name = name;
        obj.transform.SetParent(parent);
        obj.transform.localPosition = localPosition;
        obj.transform.localRotation = Quaternion.identity;
        obj.transform.localScale = localScale;

        Collider collider = obj.GetComponent<Collider>();
        if (collider != null)
            Object.DestroyImmediate(collider);

        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null && material != null)
            renderer.sharedMaterial = material;

        return obj;
    }

    private static GameObject CreateBlock(
        string name,
        Transform parent,
        Vector3 position,
        Vector3 scale,
        int layer,
        Material material,
        bool keepCollider
    )
    {
        GameObject block = GameObject.CreatePrimitive(PrimitiveType.Cube);
        block.name = name;
        block.transform.SetParent(parent);
        block.transform.position = position;
        block.transform.localScale = scale;
        block.layer = layer;

        Renderer renderer = block.GetComponent<Renderer>();
        if (renderer != null && material != null)
            renderer.sharedMaterial = material;

        if (!keepCollider)
        {
            Collider collider = block.GetComponent<Collider>();
            if (collider != null)
                Object.DestroyImmediate(collider);
        }

        return block;
    }

    private static InputActionAsset GetOrCreateInputActions()
    {
        InputActionAsset asset =
            AssetDatabase.LoadAssetAtPath<InputActionAsset>(InputAssetPath);

        if (asset == null)
        {
            asset = ScriptableObject.CreateInstance<InputActionAsset>();
            asset.name = "PlayerControls";
            AssetDatabase.CreateAsset(asset, InputAssetPath);
        }

        InputActionMap map = asset.FindActionMap("Player", false);

        if (map == null)
            map = asset.AddActionMap("Player");

        InputAction move = map.FindAction("Move", false);
        if (move == null)
        {
            move = map.AddAction(
                "Move",
                InputActionType.Value,
                expectedControlLayout: "Vector2"
            );

            move.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/w")
                .With("Down", "<Keyboard>/s")
                .With("Left", "<Keyboard>/a")
                .With("Right", "<Keyboard>/d");

            move.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/upArrow")
                .With("Down", "<Keyboard>/downArrow")
                .With("Left", "<Keyboard>/leftArrow")
                .With("Right", "<Keyboard>/rightArrow");

            move.AddBinding("<Gamepad>/leftStick");
        }

        InputAction jump = map.FindAction("Jump", false);
        if (jump == null)
        {
            jump = map.AddAction("Jump", InputActionType.Button);
            jump.AddBinding("<Keyboard>/space");
            jump.AddBinding("<Gamepad>/buttonSouth");
        }

        InputAction attack = map.FindAction("Attack", false);
        if (attack == null)
        {
            attack = map.AddAction("Attack", InputActionType.Button);
            attack.AddBinding("<Keyboard>/j");
            attack.AddBinding("<Gamepad>/buttonWest");
        }

        InputAction heavyAttack =
            map.FindAction("HeavyAttack", false);

        if (heavyAttack == null)
        {
            heavyAttack =
                map.AddAction(
                    "HeavyAttack",
                    InputActionType.Button
                );

            heavyAttack.AddBinding("<Keyboard>/i");
            heavyAttack.AddBinding("<Gamepad>/rightShoulder");
        }

        InputAction dodge =
            map.FindAction("Dodge", false);

        if (dodge == null)
        {
            dodge =
                map.AddAction(
                    "Dodge",
                    InputActionType.Button
                );

            dodge.AddBinding("<Keyboard>/leftShift");
            dodge.AddBinding("<Gamepad>/buttonEast");
        }

        InputAction specialAttack =
            map.FindAction("SpecialAttack", false);

        if (specialAttack == null)
        {
            specialAttack =
                map.AddAction(
                    "SpecialAttack",
                    InputActionType.Button
                );

            specialAttack.AddBinding("<Keyboard>/l");
            specialAttack.AddBinding("<Gamepad>/rightTrigger");
        }

        InputAction ability = map.FindAction("Ability", false);
        if (ability == null)
        {
            ability = map.AddAction("Ability", InputActionType.Button);
            ability.AddBinding("<Keyboard>/k");
            ability.AddBinding("<Gamepad>/buttonNorth");
        }
        else
        {
            bool hasKeyboardK = false;
            bool hasGamepadNorth = false;

            foreach (InputBinding binding in ability.bindings)
            {
                if (binding.path == "<Keyboard>/k")
                    hasKeyboardK = true;

                if (binding.path == "<Gamepad>/buttonNorth")
                    hasGamepadNorth = true;
            }

            if (!hasKeyboardK)
                ability.AddBinding("<Keyboard>/k");

            if (!hasGamepadNorth)
                ability.AddBinding("<Gamepad>/buttonNorth");
        }

        EditorUtility.SetDirty(asset);
        AssetDatabase.SaveAssets();

        return asset;
    }

    private static int EnsureLayer(string layerName)
    {
        int existingLayer = LayerMask.NameToLayer(layerName);
        if (existingLayer >= 0)
            return existingLayer;

        Object tagManagerAsset =
            AssetDatabase.LoadAllAssetsAtPath(
                "ProjectSettings/TagManager.asset"
            )[0];

        SerializedObject tagManager =
            new SerializedObject(tagManagerAsset);

        SerializedProperty layers =
            tagManager.FindProperty("layers");

        for (int i = 8; i < 32; i++)
        {
            SerializedProperty layer =
                layers.GetArrayElementAtIndex(i);

            if (string.IsNullOrEmpty(layer.stringValue))
            {
                layer.stringValue = layerName;
                tagManager.ApplyModifiedProperties();
                return i;
            }
        }

        Debug.LogError(
            "Aucun emplacement de Layer libre pour : " + layerName
        );

        return 0;
    }

    private static void AddSceneToBuildSettings(string scenePath)
    {
        EditorBuildSettingsScene[] current =
            EditorBuildSettings.scenes;

        foreach (EditorBuildSettingsScene scene in current)
        {
            if (scene.path == scenePath)
                return;
        }

        EditorBuildSettingsScene[] updated =
            new EditorBuildSettingsScene[current.Length + 1];

        for (int i = 0; i < current.Length; i++)
            updated[i] = current[i];

        updated[current.Length] =
            new EditorBuildSettingsScene(scenePath, true);

        EditorBuildSettings.scenes = updated;
    }

    private static void EnsureFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path))
            return;

        string parent =
            Path.GetDirectoryName(path)?.Replace("\\", "/");

        string folder = Path.GetFileName(path);

        if (!string.IsNullOrEmpty(parent) &&
            !AssetDatabase.IsValidFolder(parent))
        {
            EnsureFolder(parent);
        }

        if (!string.IsNullOrEmpty(parent))
            AssetDatabase.CreateFolder(parent, folder);
    }
}

#endif
