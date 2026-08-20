#if UNITY_EDITOR

using System.IO;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class Doryoku3PrototypeBuilder
{
    private const string PrefabFolder =
        "Assets/_Game/Prefabs/Enemies/Doryoku3";

    private const string PrefabPath =
        PrefabFolder + "/Doryoku3_Possessed.prefab";

    private const string MaterialFolder =
        "Assets/_Game/Art/Materials/Prototype";

    static Doryoku3PrototypeBuilder()
    {
        EditorApplication.delayCall += AutoUpgradeIfNeeded;
    }

    private static void AutoUpgradeIfNeeded()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
            return;

        GameObject prefab =
            AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);

        bool needsUpgrade =
            prefab == null ||
            prefab.GetComponent<Doryoku3FXController>() == null;

        if (needsUpgrade)
        {
            AssetDatabase.DeleteAsset(PrefabPath);
            FoundryPrototypeSceneBuilder.CreateOrRebuild();
        }
    }

    [MenuItem("Tools/Katsuhiro/Rebuild Doryoku-3 Possessed Prefab")]
    public static void RebuildDoryoku3Prefab()
    {
        EnsureFolder("Assets/_Game/Prefabs");
        EnsureFolder("Assets/_Game/Prefabs/Enemies");
        EnsureFolder(PrefabFolder);

        AssetDatabase.DeleteAsset(PrefabPath);

        int enemyLayer = LayerMask.NameToLayer("Enemy");

        if (enemyLayer < 0)
        {
            Debug.LogWarning(
                "La Layer Enemy n'existe pas encore. " +
                "Utilisez d'abord Create or Rebuild Foundry Prototype."
            );

            FoundryPrototypeSceneBuilder.CreateOrRebuild();
            return;
        }

        GetOrCreateDoryoku3Prefab(enemyLayer);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorGUIUtility.PingObject(
            AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath)
        );
    }

    public static GameObject GetOrCreateDoryoku3Prefab(int enemyLayer)
    {
        EnsureFolder("Assets/_Game/Prefabs");
        EnsureFolder("Assets/_Game/Prefabs/Enemies");
        EnsureFolder(PrefabFolder);
        EnsureFolder("Assets/_Game/Art");
        EnsureFolder("Assets/_Game/Art/Materials");
        EnsureFolder(MaterialFolder);

        GameObject existing =
            AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);

        if (existing != null)
            return existing;

        Material iron = GetOrCreateMaterial(
            "Doryoku3_Iron",
            new Color(0.12f, 0.14f, 0.15f),
            false,
            Color.black
        );

        Material steel = GetOrCreateMaterial(
            "Doryoku3_Steel",
            new Color(0.25f, 0.28f, 0.28f),
            false,
            Color.black
        );

        Material brass = GetOrCreateMaterial(
            "Doryoku3_Brass",
            new Color(0.42f, 0.27f, 0.08f),
            false,
            Color.black
        );

        Material copper = GetOrCreateMaterial(
            "Doryoku3_Copper",
            new Color(0.48f, 0.18f, 0.07f),
            false,
            Color.black
        );

        Material eye = GetOrCreateMaterial(
            "Doryoku3_Eye",
            new Color(0.75f, 0.03f, 0.02f),
            true,
            new Color(1.0f, 0.03f, 0.01f) * 2.4f
        );

        Material corruption = GetOrCreateMaterial(
            "Doryoku3_Corruption",
            new Color(0.26f, 0.04f, 0.35f),
            true,
            new Color(0.85f, 0.08f, 1.0f) * 2.8f
        );

        Material spirit = GetOrCreateMaterial(
            "Doryoku3_Spirit",
            new Color(0.04f, 0.34f, 0.42f),
            true,
            new Color(0.08f, 1.0f, 1.0f) * 2.5f
        );

        Material healthBackground = GetOrCreateMaterial(
            "Doryoku3_Health_BG",
            new Color(0.04f, 0.04f, 0.04f),
            false,
            Color.black
        );

        Material healthFillMaterial = GetOrCreateMaterial(
            "Doryoku3_Health_Fill",
            new Color(0.65f, 0.05f, 0.04f),
            true,
            new Color(0.80f, 0.03f, 0.02f)
        );

        Material steamParticleMaterial =
            GetOrCreateParticleMaterial(
                "Doryoku3_Steam_Particles",
                new Color(0.78f, 0.82f, 0.86f, 0.52f)
            );

        Material sparkParticleMaterial =
            GetOrCreateParticleMaterial(
                "Doryoku3_Spark_Particles",
                new Color(1.0f, 0.42f, 0.05f, 1.0f)
            );

        Material etherealParticleMaterial =
            GetOrCreateParticleMaterial(
                "Doryoku3_Ethereal_Particles",
                new Color(0.68f, 0.10f, 1.0f, 0.92f)
            );

        GameObject root = new GameObject("Doryoku3_Possessed");
        root.layer = enemyLayer;

        Rigidbody rb = root.AddComponent<Rigidbody>();
        rb.mass = 4.0f;
        rb.useGravity = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.constraints =
            RigidbodyConstraints.FreezePositionZ |
            RigidbodyConstraints.FreezeRotation;

        CapsuleCollider collider = root.AddComponent<CapsuleCollider>();
        collider.height = 3.0f;
        collider.radius = 0.68f;
        collider.center = new Vector3(0f, 1.5f, 0f);

        root.AddComponent<GameplayPlane25D>();

        GameObject modelRoot = new GameObject("ModelRoot");
        modelRoot.transform.SetParent(root.transform);
        modelRoot.transform.localPosition = Vector3.zero;

        // Massive Doryoku-3 torso and internal boiler.
        CreatePrimitive(
            PrimitiveType.Cube,
            "Torso_Armor",
            modelRoot.transform,
            new Vector3(0f, 1.55f, 0f),
            new Vector3(1.25f, 1.30f, 0.80f),
            iron
        );

        GameObject boiler = CreatePrimitive(
            PrimitiveType.Cylinder,
            "Boiler_Core",
            modelRoot.transform,
            new Vector3(0f, 1.45f, 0.16f),
            new Vector3(0.48f, 0.68f, 0.48f),
            steel
        );
        boiler.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);

        // Boiler bands.
        CreatePrimitive(
            PrimitiveType.Cylinder,
            "Boiler_Band_Upper",
            modelRoot.transform,
            new Vector3(0f, 1.72f, 0.17f),
            new Vector3(0.52f, 0.08f, 0.52f),
            brass
        );

        CreatePrimitive(
            PrimitiveType.Cylinder,
            "Boiler_Band_Lower",
            modelRoot.transform,
            new Vector3(0f, 1.18f, 0.17f),
            new Vector3(0.52f, 0.08f, 0.52f),
            brass
        );

        // Spherical rotating head.
        GameObject head = CreatePrimitive(
            PrimitiveType.Sphere,
            "Head",
            modelRoot.transform,
            new Vector3(0f, 2.62f, 0f),
            new Vector3(0.86f, 0.68f, 0.78f),
            iron
        );

        GameObject eyeObject = CreatePrimitive(
            PrimitiveType.Sphere,
            "Optical_Lens",
            modelRoot.transform,
            new Vector3(0.18f, 2.62f, -0.38f),
            new Vector3(0.32f, 0.32f, 0.18f),
            eye
        );

        GameObject eyeLightObject = new GameObject("Optical_Light");
        eyeLightObject.transform.SetParent(modelRoot.transform);
        eyeLightObject.transform.localPosition =
            new Vector3(0.18f, 2.62f, -0.58f);

        Light eyeLight = eyeLightObject.AddComponent<Light>();
        eyeLight.type = LightType.Point;
        eyeLight.range = 4.2f;
        eyeLight.intensity = 2.2f;
        eyeLight.color = new Color(0.75f, 0.03f, 0.02f);
        eyeLight.shadows = LightShadows.None;

        // Four articulated work arms.
        GameObject attackArm = CreatePrimitive(
            PrimitiveType.Capsule,
            "AttackArm_FrontUpper",
            modelRoot.transform,
            new Vector3(0.87f, 1.85f, -0.18f),
            new Vector3(0.24f, 0.62f, 0.24f),
            steel
        );
        attackArm.transform.localRotation = Quaternion.Euler(0f, 0f, -58f);

        GameObject attackPincer = CreatePrimitive(
            PrimitiveType.Sphere,
            "Pincer_FrontUpper",
            modelRoot.transform,
            new Vector3(1.28f, 1.45f, -0.18f),
            new Vector3(0.30f, 0.30f, 0.30f),
            brass
        );

        GameObject arm2 = CreatePrimitive(
            PrimitiveType.Capsule,
            "Arm_FrontLower",
            modelRoot.transform,
            new Vector3(0.80f, 1.05f, -0.12f),
            new Vector3(0.22f, 0.58f, 0.22f),
            steel
        );
        arm2.transform.localRotation = Quaternion.Euler(0f, 0f, -38f);

        CreatePrimitive(
            PrimitiveType.Sphere,
            "Tool_FrontLower",
            modelRoot.transform,
            new Vector3(1.15f, 0.70f, -0.12f),
            new Vector3(0.27f, 0.27f, 0.27f),
            brass
        );

        GameObject arm3 = CreatePrimitive(
            PrimitiveType.Capsule,
            "Arm_BackUpper",
            modelRoot.transform,
            new Vector3(-0.86f, 1.84f, 0.18f),
            new Vector3(0.23f, 0.61f, 0.23f),
            steel
        );
        arm3.transform.localRotation = Quaternion.Euler(0f, 0f, 55f);

        CreatePrimitive(
            PrimitiveType.Sphere,
            "Pincer_BackUpper",
            modelRoot.transform,
            new Vector3(-1.25f, 1.46f, 0.18f),
            new Vector3(0.29f, 0.29f, 0.29f),
            brass
        );

        GameObject arm4 = CreatePrimitive(
            PrimitiveType.Capsule,
            "Arm_BackLower",
            modelRoot.transform,
            new Vector3(-0.76f, 1.05f, 0.12f),
            new Vector3(0.21f, 0.56f, 0.21f),
            steel
        );
        arm4.transform.localRotation = Quaternion.Euler(0f, 0f, 35f);

        // Steam pipes / copper veins.
        GameObject pipeLeft = CreatePrimitive(
            PrimitiveType.Cylinder,
            "CopperPipe_Left",
            modelRoot.transform,
            new Vector3(-0.43f, 1.55f, -0.43f),
            new Vector3(0.07f, 0.63f, 0.07f),
            copper
        );
        pipeLeft.transform.localRotation = Quaternion.Euler(0f, 0f, -8f);

        GameObject pipeRight = CreatePrimitive(
            PrimitiveType.Cylinder,
            "CopperPipe_Right",
            modelRoot.transform,
            new Vector3(0.43f, 1.55f, -0.43f),
            new Vector3(0.07f, 0.63f, 0.07f),
            copper
        );
        pipeRight.transform.localRotation = Quaternion.Euler(0f, 0f, 8f);

        // Heavy legs.
        CreatePrimitive(
            PrimitiveType.Cube,
            "Leg_Left",
            modelRoot.transform,
            new Vector3(-0.38f, 0.55f, 0f),
            new Vector3(0.46f, 0.88f, 0.58f),
            iron
        );

        CreatePrimitive(
            PrimitiveType.Cube,
            "Leg_Right",
            modelRoot.transform,
            new Vector3(0.38f, 0.55f, 0f),
            new Vector3(0.46f, 0.88f, 0.58f),
            iron
        );

        CreatePrimitive(
            PrimitiveType.Cube,
            "Foot_Left",
            modelRoot.transform,
            new Vector3(-0.44f, 0.12f, -0.10f),
            new Vector3(0.62f, 0.24f, 0.86f),
            steel
        );

        CreatePrimitive(
            PrimitiveType.Cube,
            "Foot_Right",
            modelRoot.transform,
            new Vector3(0.44f, 0.12f, -0.10f),
            new Vector3(0.62f, 0.24f, 0.86f),
            steel
        );

        // Ethereal corruption: hidden in the normal world and revealed by Kikai-Yurei.
        GameObject overlay = new GameObject("Ethereal_Corruption");
        overlay.transform.SetParent(modelRoot.transform);
        overlay.transform.localPosition = Vector3.zero;

        CreatePrimitive(
            PrimitiveType.Sphere,
            "Possessing_Yokai_Core",
            overlay.transform,
            new Vector3(0f, 1.60f, -0.52f),
            new Vector3(0.72f, 1.00f, 0.28f),
            corruption
        );

        CreatePrimitive(
            PrimitiveType.Sphere,
            "Spectral_Head",
            overlay.transform,
            new Vector3(0f, 2.85f, -0.24f),
            new Vector3(0.82f, 0.56f, 0.34f),
            spirit
        );

        GameObject hornL = CreatePrimitive(
            PrimitiveType.Capsule,
            "Spectral_Horn_Left",
            overlay.transform,
            new Vector3(-0.42f, 3.16f, -0.18f),
            new Vector3(0.13f, 0.40f, 0.13f),
            corruption
        );
        hornL.transform.localRotation = Quaternion.Euler(0f, 0f, 32f);

        GameObject hornR = CreatePrimitive(
            PrimitiveType.Capsule,
            "Spectral_Horn_Right",
            overlay.transform,
            new Vector3(0.42f, 3.16f, -0.18f),
            new Vector3(0.13f, 0.40f, 0.13f),
            corruption
        );
        hornR.transform.localRotation = Quaternion.Euler(0f, 0f, -32f);

        for (int i = 0; i < 3; i++)
        {
            GameObject tendril = CreatePrimitive(
                PrimitiveType.Capsule,
                "Spectral_Tendril_" + (i + 1),
                overlay.transform,
                new Vector3(-0.48f + i * 0.48f, 0.92f, -0.46f),
                new Vector3(0.10f, 0.72f, 0.10f),
                i == 1 ? spirit : corruption
            );

            tendril.transform.localRotation =
                Quaternion.Euler(0f, 0f, -22f + i * 22f);
        }

        GameObject corruptionLightObject =
            new GameObject("Corruption_Light");

        corruptionLightObject.transform.SetParent(overlay.transform);
        corruptionLightObject.transform.localPosition =
            new Vector3(0f, 1.65f, -0.7f);

        Light corruptionLight =
            corruptionLightObject.AddComponent<Light>();

        corruptionLight.type = LightType.Point;
        corruptionLight.range = 5.0f;
        corruptionLight.intensity = 4.0f;
        corruptionLight.color = new Color(0.75f, 0.08f, 1.0f);
        corruptionLight.shadows = LightShadows.None;

        overlay.SetActive(false);


        // VFX anchors and particle systems.
        GameObject fxRoot = new GameObject("FX");
        fxRoot.transform.SetParent(root.transform);
        fxRoot.transform.localPosition = Vector3.zero;

        ParticleSystem idleSteamLeft = CreateParticleSystem(
            "IdleSteam_Left",
            fxRoot.transform,
            new Vector3(-0.46f, 2.08f, 0.16f),
            steamParticleMaterial,
            true,
            true,
            7.5f,
            0.95f,
            0.75f,
            0.15f,
            new Color(0.82f, 0.86f, 0.90f, 0.44f),
            0.0f,
            false
        );

        ParticleSystem idleSteamRight = CreateParticleSystem(
            "IdleSteam_Right",
            fxRoot.transform,
            new Vector3(0.46f, 2.08f, 0.16f),
            steamParticleMaterial,
            true,
            true,
            7.5f,
            0.95f,
            0.75f,
            0.15f,
            new Color(0.82f, 0.86f, 0.90f, 0.44f),
            0.0f,
            false
        );

        ParticleSystem attackSteam = CreateParticleSystem(
            "AttackSteam_Burst",
            fxRoot.transform,
            new Vector3(0.70f, 1.72f, -0.14f),
            steamParticleMaterial,
            false,
            false,
            0f,
            0.48f,
            2.1f,
            0.13f,
            new Color(0.90f, 0.94f, 0.98f, 0.72f),
            0.0f,
            false
        );

        ParticleSystem hitSparks = CreateParticleSystem(
            "HitSparks",
            fxRoot.transform,
            new Vector3(0f, 1.55f, -0.45f),
            sparkParticleMaterial,
            false,
            false,
            0f,
            0.42f,
            3.8f,
            0.065f,
            new Color(1.0f, 0.38f, 0.04f, 1.0f),
            1.2f,
            true
        );

        ParticleSystem specialCharge = CreateParticleSystem(
            "SpecialCharge",
            fxRoot.transform,
            new Vector3(0f, 1.70f, -0.55f),
            etherealParticleMaterial,
            true,
            false,
            18f,
            0.65f,
            0.45f,
            0.10f,
            new Color(0.66f, 0.12f, 1.0f, 0.92f),
            0.0f,
            true
        );

        ParticleSystem specialRelease = CreateParticleSystem(
            "SpecialRelease",
            fxRoot.transform,
            new Vector3(0f, 1.72f, -0.62f),
            etherealParticleMaterial,
            false,
            false,
            0f,
            0.48f,
            3.4f,
            0.13f,
            new Color(0.72f, 0.14f, 1.0f, 1.0f),
            0.0f,
            true
        );

        GameObject deathFxRoot = new GameObject("DeathExplosionFX");
        deathFxRoot.transform.SetParent(root.transform);
        deathFxRoot.transform.localPosition = new Vector3(0f, 1.45f, 0f);

        ParticleSystem deathSmoke = CreateParticleSystem(
            "DeathSmoke",
            deathFxRoot.transform,
            Vector3.zero,
            steamParticleMaterial,
            false,
            false,
            0f,
            1.8f,
            1.8f,
            0.34f,
            new Color(0.36f, 0.31f, 0.28f, 0.62f),
            -0.10f,
            true
        );

        ParticleSystem deathSparks = CreateParticleSystem(
            "DeathSparks",
            deathFxRoot.transform,
            Vector3.zero,
            sparkParticleMaterial,
            false,
            false,
            0f,
            0.90f,
            5.8f,
            0.075f,
            new Color(1.0f, 0.28f, 0.02f, 1.0f),
            1.5f,
            true
        );

        GameObject deathLightObject = new GameObject("DeathFlash");
        deathLightObject.transform.SetParent(deathFxRoot.transform);
        deathLightObject.transform.localPosition = Vector3.zero;

        Light deathFlash = deathLightObject.AddComponent<Light>();
        deathFlash.type = LightType.Point;
        deathFlash.range = 7.5f;
        deathFlash.intensity = 0f;
        deathFlash.color = new Color(1.0f, 0.34f, 0.08f);
        deathFlash.shadows = LightShadows.None;

        deathFxRoot.SetActive(false);

        GameObject specialOrigin = new GameObject("SpecialAttackOrigin");
        specialOrigin.transform.SetParent(root.transform);
        specialOrigin.transform.localPosition =
            new Vector3(1.55f, 1.65f, -0.20f);

        // World-space health bar.
        GameObject healthBar = new GameObject("HealthBar");
        healthBar.transform.SetParent(root.transform);
        healthBar.transform.localPosition = new Vector3(0f, 3.55f, -0.55f);

        CreatePrimitive(
            PrimitiveType.Cube,
            "Background",
            healthBar.transform,
            Vector3.zero,
            new Vector3(1.65f, 0.15f, 0.08f),
            healthBackground
        );

        GameObject healthFill = CreatePrimitive(
            PrimitiveType.Cube,
            "Fill",
            healthBar.transform,
            new Vector3(0f, 0f, -0.05f),
            new Vector3(1.48f, 0.095f, 0.07f),
            healthFillMaterial
        );

        Doryoku3VisualController visuals =
            root.AddComponent<Doryoku3VisualController>();

        SerializedObject visualSO = new SerializedObject(visuals);
        visualSO.FindProperty("modelRoot").objectReferenceValue = modelRoot.transform;
        visualSO.FindProperty("head").objectReferenceValue = head.transform;
        visualSO.FindProperty("attackArm").objectReferenceValue = attackArm.transform;
        visualSO.FindProperty("attackPincer").objectReferenceValue = attackPincer.transform;
        visualSO.FindProperty("secondaryAttackArm").objectReferenceValue = arm2.transform;
        visualSO.FindProperty("etherealOverlay").objectReferenceValue = overlay.transform;
        visualSO.FindProperty("eyeRenderer").objectReferenceValue = eyeObject.GetComponent<Renderer>();
        visualSO.FindProperty("eyeLight").objectReferenceValue = eyeLight;
        visualSO.FindProperty("healthFill").objectReferenceValue = healthFill.transform;
        visualSO.ApplyModifiedPropertiesWithoutUndo();

        Doryoku3FXController fx =
            root.AddComponent<Doryoku3FXController>();

        SerializedObject fxSO = new SerializedObject(fx);
        fxSO.FindProperty("idleSteamLeft").objectReferenceValue = idleSteamLeft;
        fxSO.FindProperty("idleSteamRight").objectReferenceValue = idleSteamRight;
        fxSO.FindProperty("attackSteam").objectReferenceValue = attackSteam;
        fxSO.FindProperty("hitSparks").objectReferenceValue = hitSparks;
        fxSO.FindProperty("specialCharge").objectReferenceValue = specialCharge;
        fxSO.FindProperty("specialRelease").objectReferenceValue = specialRelease;
        fxSO.FindProperty("deathFxRoot").objectReferenceValue = deathFxRoot.transform;
        fxSO.FindProperty("deathSmoke").objectReferenceValue = deathSmoke;
        fxSO.FindProperty("deathSparks").objectReferenceValue = deathSparks;
        fxSO.FindProperty("deathFlashLight").objectReferenceValue = deathFlash;
        fxSO.FindProperty("debrisMaterial").objectReferenceValue = steel;
        fxSO.ApplyModifiedPropertiesWithoutUndo();

        Doryoku3Enemy enemy =
            root.AddComponent<Doryoku3Enemy>();

        SerializedObject enemySO = new SerializedObject(enemy);
        enemySO.FindProperty("visuals").objectReferenceValue = visuals;
        enemySO.FindProperty("fx").objectReferenceValue = fx;
        enemySO.FindProperty("specialAttackOrigin").objectReferenceValue = specialOrigin.transform;
        enemySO.FindProperty("specialProjectileMaterial").objectReferenceValue = etherealParticleMaterial;
        enemySO.ApplyModifiedPropertiesWithoutUndo();

        GameObject prefab =
            PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);

        Object.DestroyImmediate(root);

        return prefab;
    }

    public static void AddEnemiesToScene(
        Transform levelRoot,
        GameObject doryokuPrefab,
        Transform player,
        int enemyLayer,
        int groundLayer
    )
    {
        if (doryokuPrefab == null)
            return;

        GameObject enemiesRoot = new GameObject("Enemies_Doryoku3");
        enemiesRoot.transform.SetParent(levelRoot);

        CreateEnemyInstance(
            "Doryoku3_Unit07",
            doryokuPrefab,
            enemiesRoot.transform,
            new Vector3(7.7f, -0.48f, 0f),
            3.0f
        );

        CreateEnemyInstance(
            "Doryoku3_Unit11",
            doryokuPrefab,
            enemiesRoot.transform,
            new Vector3(-12.0f, -0.48f, 0f),
            2.6f
        );
    }

    private static void CreateEnemyInstance(
        string name,
        GameObject prefab,
        Transform parent,
        Vector3 position,
        float patrolDistance
    )
    {
        GameObject instance =
            PrefabUtility.InstantiatePrefab(prefab, parent) as GameObject;

        if (instance == null)
            return;

        instance.name = name;
        instance.transform.position = position;
        instance.transform.rotation = Quaternion.identity;

        Doryoku3Enemy enemy =
            instance.GetComponent<Doryoku3Enemy>();

        if (enemy != null)
        {
            SerializedObject enemySO =
                new SerializedObject(enemy);

            enemySO.FindProperty("patrolDistance").floatValue =
                patrolDistance;

            enemySO.ApplyModifiedPropertiesWithoutUndo();
        }
    }

    private static GameObject CreatePrimitive(
        PrimitiveType type,
        string name,
        Transform parent,
        Vector3 localPosition,
        Vector3 localScale,
        Material material
    )
    {
        GameObject obj = GameObject.CreatePrimitive(type);
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


    private static ParticleSystem CreateParticleSystem(
        string name,
        Transform parent,
        Vector3 localPosition,
        Material material,
        bool loop,
        bool playOnAwake,
        float emissionRate,
        float lifetime,
        float speed,
        float size,
        Color color,
        float gravityModifier,
        bool sphericalShape
    )
    {
        GameObject particleObject = new GameObject(name);
        particleObject.transform.SetParent(parent);
        particleObject.transform.localPosition = localPosition;
        particleObject.transform.localRotation = Quaternion.identity;

        ParticleSystem particles =
            particleObject.AddComponent<ParticleSystem>();

        var main = particles.main;
        main.loop = loop;
        main.playOnAwake = playOnAwake;
        main.startLifetime = lifetime;
        main.startSpeed = speed;
        main.startSize = size;
        main.startColor = color;
        main.gravityModifier = gravityModifier;
        main.maxParticles = loop ? 80 : 120;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        var emission = particles.emission;
        emission.enabled = true;
        emission.rateOverTime = emissionRate;

        var shape = particles.shape;
        shape.enabled = true;
        shape.shapeType =
            sphericalShape
                ? ParticleSystemShapeType.Sphere
                : ParticleSystemShapeType.Cone;

        if (sphericalShape)
        {
            shape.radius = 0.22f;
        }
        else
        {
            shape.radius = 0.08f;
            shape.angle = 18f;
        }

        var colorOverLifetime =
            particles.colorOverLifetime;

        colorOverLifetime.enabled = true;

        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new[]
            {
                new GradientColorKey(color, 0f),
                new GradientColorKey(color, 1f)
            },
            new[]
            {
                new GradientAlphaKey(color.a, 0f),
                new GradientAlphaKey(0f, 1f)
            }
        );

        colorOverLifetime.color = gradient;

        ParticleSystemRenderer renderer =
            particleObject.GetComponent<ParticleSystemRenderer>();

        if (renderer != null && material != null)
            renderer.sharedMaterial = material;

        return particles;
    }

    private static Material GetOrCreateParticleMaterial(
        string materialName,
        Color color
    )
    {
        string path =
            MaterialFolder + "/" + materialName + ".mat";

        Material existing =
            AssetDatabase.LoadAssetAtPath<Material>(path);

        if (existing != null)
            return existing;

        Shader shader =
            Shader.Find("Universal Render Pipeline/Particles/Unlit");

        if (shader == null)
            shader = Shader.Find("Particles/Standard Unlit");

        if (shader == null)
            shader = Shader.Find("Sprites/Default");

        Material material = new Material(shader);
        material.name = materialName;
        material.renderQueue = 3000;

        if (material.HasProperty("_BaseColor"))
            material.SetColor("_BaseColor", color);

        if (material.HasProperty("_Color"))
            material.SetColor("_Color", color);

        if (material.HasProperty("_Surface"))
            material.SetFloat("_Surface", 1f);

        material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");

        AssetDatabase.CreateAsset(material, path);
        return material;
    }

    private static Material GetOrCreateMaterial(
        string materialName,
        Color baseColor,
        bool emission,
        Color emissionColor
    )
    {
        string path =
            MaterialFolder + "/" + materialName + ".mat";

        Material existing =
            AssetDatabase.LoadAssetAtPath<Material>(path);

        if (existing != null)
            return existing;

        Shader shader =
            Shader.Find("Universal Render Pipeline/Lit");

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

    private static void EnsureFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path))
            return;

        string parent =
            Path.GetDirectoryName(path)?.Replace("\\", "/");

        string folder =
            Path.GetFileName(path);

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
