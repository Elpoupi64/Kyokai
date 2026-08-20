#if UNITY_EDITOR

using System.IO;
using UnityEditor;
using UnityEngine;

public static class FoundryVerticalSliceBuilder
{
    private const string MaterialFolder =
        "Assets/_Game/Art/Materials/Prototype";

    public static VerticalSliceDirector BuildSlice(
        Transform levelRoot,
        Transform gameplayRoot,
        Transform player,
        GameObject doryokuPrefab,
        KikaiWorldManager worldManager,
        int groundLayer,
        int enemyLayer
    )
    {
        if (levelRoot == null ||
            gameplayRoot == null ||
            player == null)
        {
            return null;
        }

        player.position =
            new Vector3(-72f, 0.55f, 0f);

        Material iron =
            GetOrCreateMaterial(
                "Slice_Foundry_Iron",
                new Color(0.22f, 0.18f, 0.15f),
                false,
                Color.black
            );

        Material platform =
            GetOrCreateMaterial(
                "Slice_Platform",
                new Color(0.32f, 0.24f, 0.17f),
                false,
                Color.black
            );

        Material machine =
            GetOrCreateMaterial(
                "Slice_Machine",
                new Color(0.12f, 0.14f, 0.15f),
                false,
                Color.black
            );

        Material copper =
            GetOrCreateMaterial(
                "Slice_Copper",
                new Color(0.44f, 0.18f, 0.06f),
                false,
                Color.black
            );

        Material ether =
            GetOrCreateMaterial(
                "Slice_Ethereal",
                new Color(0.04f, 0.40f, 0.48f),
                true,
                new Color(0.08f, 1.0f, 1.0f) * 3.0f
            );

        Material checkpointMaterial =
            GetOrCreateMaterial(
                "Slice_Checkpoint",
                new Color(0.30f, 0.18f, 0.07f),
                true,
                new Color(0.18f, 0.09f, 0.02f)
            );

        GameObject sliceRoot =
            new GameObject("VERTICAL_SLICE_V9");

        sliceRoot.transform.SetParent(levelRoot);

        // -------------------------------------------------------
        // 1. Tutorial de mouvement
        // -------------------------------------------------------
        GameObject movement =
            new GameObject("01_Tutorial_Movement");

        movement.transform.SetParent(sliceRoot.transform);

        CreateBlock(
            "Ground_Start",
            movement.transform,
            new Vector3(-66f, -1f, 0f),
            new Vector3(20f, 1f, 4f),
            groundLayer,
            iron,
            true
        );

        CreateBlock(
            "Jump_Platform_01",
            movement.transform,
            new Vector3(-68.5f, 1.05f, 0f),
            new Vector3(3.2f, 0.45f, 4f),
            groundLayer,
            platform,
            true
        );

        CreateBlock(
            "Jump_Platform_02",
            movement.transform,
            new Vector3(-63.5f, 2.15f, 0f),
            new Vector3(3.0f, 0.45f, 4f),
            groundLayer,
            platform,
            true
        );

        // -------------------------------------------------------
        // 2. Kikai-Yurei / pont spectral
        // -------------------------------------------------------
        GameObject kikaiZone =
            new GameObject("02_Kikai_Yurei_Bridge");

        kikaiZone.transform.SetParent(sliceRoot.transform);

        GameObject etherealBridge =
            new GameObject("EtherealBridge");

        etherealBridge.transform.SetParent(kikaiZone.transform);

        float[] bridgeX =
        {
            -54.6f,
            -51.8f,
            -49.0f,
            -46.4f
        };

        for (int i = 0; i < bridgeX.Length; i++)
        {
            CreateBlock(
                "SpiritBridge_" + (i + 1),
                etherealBridge.transform,
                new Vector3(
                    bridgeX[i],
                    -0.20f + Mathf.Sin(i * 0.8f) * 0.30f,
                    0f
                ),
                new Vector3(2.3f, 0.38f, 4f),
                groundLayer,
                ether,
                true
            );
        }

        KikaiWorldVisibility bridgeVisibility =
            etherealBridge.AddComponent<KikaiWorldVisibility>();

        SerializedObject bridgeSO =
            new SerializedObject(bridgeVisibility);

        bridgeSO.FindProperty("visibilityMode").enumValueIndex =
            (int)KikaiVisibilityMode.EtherealOnly;

        bridgeSO.FindProperty("affectColliders").boolValue =
            true;

        bridgeSO.ApplyModifiedPropertiesWithoutUndo();

        // -------------------------------------------------------
        // 3. Premier combat
        // -------------------------------------------------------
        GameObject combatZone =
            new GameObject("03_First_Doryoku3_Combat");

        combatZone.transform.SetParent(sliceRoot.transform);

        CreateBlock(
            "Ground_Combat",
            combatZone.transform,
            new Vector3(-39f, -1f, 0f),
            new Vector3(14f, 1f, 4f),
            groundLayer,
            iron,
            true
        );

        GameObject firstEnemy = null;

        if (doryokuPrefab != null)
        {
            firstEnemy =
                PrefabUtility.InstantiatePrefab(
                    doryokuPrefab,
                    combatZone.transform
                ) as GameObject;

            firstEnemy.name =
                "Doryoku3_FirstEncounter";

            firstEnemy.transform.position =
                new Vector3(-38.0f, -0.48f, 0f);

            firstEnemy.transform.localScale =
                Vector3.one * 0.92f;

            Doryoku3Enemy enemy =
                firstEnemy.GetComponent<Doryoku3Enemy>();

            if (enemy != null)
            {
                SerializedObject enemySO =
                    new SerializedObject(enemy);

                enemySO.FindProperty("patrolDistance").floatValue =
                    2.2f;

                enemySO.FindProperty("detectionRange").floatValue =
                    7.5f;

                enemySO.FindProperty("chaseSpeed").floatValue =
                    2.8f;

                enemySO.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        // -------------------------------------------------------
        // 4. Checkpoint + corridor
        // -------------------------------------------------------
        GameObject corridor =
            new GameObject("04_Checkpoint_And_Chase_Corridor");

        corridor.transform.SetParent(sliceRoot.transform);

        CreateBlock(
            "Ground_Corridor_And_BossApproach",
            corridor.transform,
            new Vector3(-7f, -1f, 0f),
            new Vector3(50f, 1f, 4f),
            groundLayer,
            iron,
            true
        );

        CreateBlock(
            "Corridor_Platform_01",
            corridor.transform,
            new Vector3(-12f, 1.25f, 0f),
            new Vector3(3.8f, 0.45f, 4f),
            groundLayer,
            platform,
            true
        );

        CreateBlock(
            "Corridor_Platform_02",
            corridor.transform,
            new Vector3(-6f, 2.15f, 0f),
            new Vector3(3.3f, 0.45f, 4f),
            groundLayer,
            platform,
            true
        );

        GameObject checkpointRoot =
            new GameObject("Checkpoint_Katsuhiro");

        checkpointRoot.transform.SetParent(corridor.transform);
        checkpointRoot.transform.position =
            new Vector3(-21.5f, -0.25f, 0f);

        GameObject checkpointVisual =
            GameObject.CreatePrimitive(PrimitiveType.Cylinder);

        checkpointVisual.name = "CheckpointBeacon";
        checkpointVisual.transform.SetParent(checkpointRoot.transform);
        checkpointVisual.transform.localPosition =
            new Vector3(0f, 0.75f, 0f);

        checkpointVisual.transform.localScale =
            new Vector3(0.38f, 1.1f, 0.38f);

        Renderer checkpointRenderer =
            checkpointVisual.GetComponent<Renderer>();

        checkpointRenderer.sharedMaterial =
            checkpointMaterial;

        Collider visualCollider =
            checkpointVisual.GetComponent<Collider>();

        if (visualCollider != null)
            Object.DestroyImmediate(visualCollider);

        BoxCollider checkpointTrigger =
            checkpointRoot.AddComponent<BoxCollider>();

        checkpointTrigger.isTrigger = true;
        checkpointTrigger.size =
            new Vector3(2.0f, 3.2f, 4f);

        GameObject respawn =
            new GameObject("RespawnPoint");

        respawn.transform.SetParent(checkpointRoot.transform);
        respawn.transform.localPosition =
            new Vector3(1.4f, 0.80f, 0f);

        GameObject checkpointLightObject =
            new GameObject("CheckpointLight");

        checkpointLightObject.transform.SetParent(checkpointRoot.transform);
        checkpointLightObject.transform.localPosition =
            new Vector3(0f, 1.4f, -0.7f);

        Light checkpointLight =
            checkpointLightObject.AddComponent<Light>();

        checkpointLight.type = LightType.Point;
        checkpointLight.range = 4.5f;
        checkpointLight.intensity = 0.6f;
        checkpointLight.color =
            new Color(0.30f, 0.18f, 0.07f);

        VerticalSliceCheckpoint checkpoint =
            checkpointRoot.AddComponent<VerticalSliceCheckpoint>();

        SerializedObject checkpointSO =
            new SerializedObject(checkpoint);

        checkpointSO.FindProperty("respawnPoint").objectReferenceValue =
            respawn.transform;

        checkpointSO.FindProperty("checkpointRenderer").objectReferenceValue =
            checkpointRenderer;

        checkpointSO.FindProperty("checkpointLight").objectReferenceValue =
            checkpointLight;

        checkpointSO.ApplyModifiedPropertiesWithoutUndo();

        // -------------------------------------------------------
        // 5. Chase Doryoku
        // -------------------------------------------------------
        GameObject chasePursuer = null;

        if (doryokuPrefab != null)
        {
            chasePursuer =
                PrefabUtility.InstantiatePrefab(
                    doryokuPrefab,
                    corridor.transform
                ) as GameObject;

            chasePursuer.name =
                "Doryoku3_ChaseUnit";

            chasePursuer.transform.position =
                new Vector3(-15.0f, -0.48f, 0f);

            chasePursuer.transform.localScale =
                Vector3.one * 1.06f;

            Doryoku3Enemy chaseEnemy =
                chasePursuer.GetComponent<Doryoku3Enemy>();

            if (chaseEnemy != null)
            {
                SerializedObject chaseEnemySO =
                    new SerializedObject(chaseEnemy);

                chaseEnemySO.FindProperty("patrolDistance").floatValue =
                    0.5f;

                chaseEnemySO.FindProperty("detectionRange").floatValue =
                    60f;

                chaseEnemySO.FindProperty("verticalDetectionRange").floatValue =
                    7f;

                chaseEnemySO.FindProperty("chaseSpeed").floatValue =
                    5.6f;

                chaseEnemySO.FindProperty("lostTargetMultiplier").floatValue =
                    4f;

                chaseEnemySO.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        CameraFollow25D camera =
            Camera.main != null
                ? Camera.main.GetComponent<CameraFollow25D>()
                : Object.FindAnyObjectByType<CameraFollow25D>();

        VerticalSliceChaseSequence chase =
            corridor.AddComponent<VerticalSliceChaseSequence>();

        SerializedObject chaseSO =
            new SerializedObject(chase);

        chaseSO.FindProperty("player").objectReferenceValue =
            player;

        chaseSO.FindProperty("pursuer").objectReferenceValue =
            chasePursuer;

        chaseSO.FindProperty("cameraFollow").objectReferenceValue =
            camera;

        chaseSO.ApplyModifiedPropertiesWithoutUndo();

        // -------------------------------------------------------
        // Background industrial greybox for visual depth.
        // -------------------------------------------------------
        GameObject background =
            new GameObject("Industrial_Background_Greybox");

        background.transform.SetParent(sliceRoot.transform);

        CreateBackgroundTower(
            background.transform,
            -68f,
            6f,
            machine,
            copper
        );

        CreateBackgroundTower(
            background.transform,
            -43f,
            8f,
            machine,
            copper
        );

        CreateBackgroundTower(
            background.transform,
            -18f,
            7f,
            machine,
            copper
        );

        CreateBackgroundTower(
            background.transform,
            7f,
            10f,
            machine,
            copper
        );

        // -------------------------------------------------------
        // Director.
        // -------------------------------------------------------
        GameObject directorObject =
            new GameObject("VerticalSliceDirector");

        directorObject.transform.SetParent(gameplayRoot);

        VerticalSliceDirector director =
            directorObject.AddComponent<VerticalSliceDirector>();

        SerializedObject directorSO =
            new SerializedObject(director);

        directorSO.FindProperty("player").objectReferenceValue =
            player;

        directorSO.FindProperty("playerMotor").objectReferenceValue =
            player.GetComponent<PlayerMotor25D>();

        directorSO.FindProperty("playerCombat").objectReferenceValue =
            player.GetComponent<KenjiroCombatController>();

        directorSO.FindProperty("worldManager").objectReferenceValue =
            worldManager;

        directorSO.FindProperty("firstEnemy").objectReferenceValue =
            firstEnemy;

        directorSO.FindProperty("chaseSequence").objectReferenceValue =
            chase;

        directorSO.ApplyModifiedPropertiesWithoutUndo();

        SerializedObject checkpointLinkSO =
            new SerializedObject(checkpoint);

        checkpointLinkSO.FindProperty("director").objectReferenceValue =
            director;

        checkpointLinkSO.ApplyModifiedPropertiesWithoutUndo();

        SerializedObject chaseLinkSO =
            new SerializedObject(chase);

        chaseLinkSO.FindProperty("director").objectReferenceValue =
            director;

        chaseLinkSO.ApplyModifiedPropertiesWithoutUndo();

        GameObject marker =
            new GameObject("VerticalSlice_v9");

        marker.transform.SetParent(gameplayRoot);

        return director;
    }

    public static void FinalizeSlice(
        VerticalSliceDirector director
    )
    {
        if (director == null)
            return;

        Doryoku3MiniBoss boss =
            Object.FindAnyObjectByType<Doryoku3MiniBoss>();

        SerializedObject directorSO =
            new SerializedObject(director);

        directorSO.FindProperty("boss").objectReferenceValue =
            boss;

        directorSO.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void CreateBackgroundTower(
        Transform parent,
        float x,
        float height,
        Material machine,
        Material copper
    )
    {
        CreateBlock(
            "FactoryBlock_" + x,
            parent,
            new Vector3(x, height * 0.42f, 7.5f),
            new Vector3(5f, height, 3f),
            0,
            machine,
            false
        );

        GameObject chimney =
            GameObject.CreatePrimitive(PrimitiveType.Cylinder);

        chimney.name = "Chimney_" + x;
        chimney.transform.SetParent(parent);
        chimney.transform.position =
            new Vector3(x + 1.4f, height + 1.5f, 8.5f);

        chimney.transform.localScale =
            new Vector3(0.55f, height * 0.42f, 0.55f);

        Renderer renderer =
            chimney.GetComponent<Renderer>();

        renderer.sharedMaterial = machine;

        Collider collider =
            chimney.GetComponent<Collider>();

        if (collider != null)
            Object.DestroyImmediate(collider);

        GameObject pipe =
            GameObject.CreatePrimitive(PrimitiveType.Cylinder);

        pipe.name = "CopperPipe_" + x;
        pipe.transform.SetParent(parent);
        pipe.transform.position =
            new Vector3(x - 1.55f, 2.4f, 5.8f);

        pipe.transform.localScale =
            new Vector3(0.16f, 2.2f, 0.16f);

        Renderer pipeRenderer =
            pipe.GetComponent<Renderer>();

        pipeRenderer.sharedMaterial = copper;

        Collider pipeCollider =
            pipe.GetComponent<Collider>();

        if (pipeCollider != null)
            Object.DestroyImmediate(pipeCollider);
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
        GameObject block =
            GameObject.CreatePrimitive(PrimitiveType.Cube);

        block.name = name;
        block.transform.SetParent(parent);
        block.transform.position = position;
        block.transform.localScale = scale;
        block.layer = layer;

        Renderer renderer =
            block.GetComponent<Renderer>();

        if (renderer != null)
            renderer.sharedMaterial = material;

        if (!keepCollider)
        {
            Collider collider =
                block.GetComponent<Collider>();

            if (collider != null)
                Object.DestroyImmediate(collider);
        }

        return block;
    }

    private static Material GetOrCreateMaterial(
        string materialName,
        Color baseColor,
        bool emission,
        Color emissionColor
    )
    {
        EnsureFolder("Assets/_Game/Art");
        EnsureFolder("Assets/_Game/Art/Materials");
        EnsureFolder(MaterialFolder);

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

        Material material =
            new Material(shader);

        material.name = materialName;

        if (material.HasProperty("_BaseColor"))
            material.SetColor("_BaseColor", baseColor);

        if (material.HasProperty("_Color"))
            material.SetColor("_Color", baseColor);

        if (emission &&
            material.HasProperty("_EmissionColor"))
        {
            material.EnableKeyword("_EMISSION");
            material.SetColor(
                "_EmissionColor",
                emissionColor
            );
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
