#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

public static class KatsuhiroV171LevelPacingBuilder
{
    public static void Apply(
        Transform levelRoot,
        Transform gameplayRoot,
        Transform player,
        GameObject doryokuPrefab,
        KikaiWorldManager worldManager,
        int groundLayer,
        int enemyLayer,
        VerticalSliceDirector director
    )
    {
        if (levelRoot == null ||
            gameplayRoot == null ||
            player == null ||
            director == null)
        {
            return;
        }

        Transform sliceRoot =
            FindRecursive(
                levelRoot,
                "VERTICAL_SLICE_V9"
            );

        if (sliceRoot == null)
            return;

        Transform oldMarker =
            gameplayRoot.Find(
                "LevelPacing_v17_1"
            );

        if (oldMarker != null)
        {
            Object.DestroyImmediate(
                oldMarker.gameObject
            );
        }

        GameObject marker =
            new GameObject(
                "LevelPacing_v17_1"
            );

        marker.transform.SetParent(
            gameplayRoot
        );

        Material iron =
            GetMaterial(
                "Slice_Foundry_Iron"
            );

        Material platform =
            GetMaterial(
                "Slice_Platform"
            );

        Material machine =
            GetMaterial(
                "Slice_Machine"
            );

        Material copper =
            GetMaterial(
                "Slice_Copper"
            );

        Material ether =
            GetMaterial(
                "Slice_Ethereal"
            );

        Material violet =
            GetMaterial(
                "Ethereal_Violet"
            );

        if (violet == null)
            violet = ether;

        BuildMachineRoom(
            sliceRoot,
            director,
            doryokuPrefab,
            worldManager,
            groundLayer,
            enemyLayer,
            iron,
            platform,
            machine,
            copper,
            ether,
            violet
        );

        RebuildChaseCourse(
            levelRoot,
            sliceRoot,
            groundLayer,
            iron,
            platform,
            machine,
            copper,
            ether
        );

        ConfigureDirector(
            director
        );

        ConfigureChase(
            sliceRoot
        );
    }

    private static void BuildMachineRoom(
        Transform sliceRoot,
        VerticalSliceDirector director,
        GameObject doryokuPrefab,
        KikaiWorldManager worldManager,
        int groundLayer,
        int enemyLayer,
        Material iron,
        Material platform,
        Material machine,
        Material copper,
        Material ether,
        Material violet
    )
    {
        Transform old =
            sliceRoot.Find(
                "04_Chain4_Possessed_v17_1"
            );

        if (old != null)
            Object.DestroyImmediate(
                old.gameObject
            );

        GameObject roomRoot =
            new GameObject(
                "04_Chain4_Possessed_v17_1"
            );

        roomRoot.transform.SetParent(
            sliceRoot
        );

        CreateBlock(
            "MachineRoom_Ground",
            roomRoot.transform,
            new Vector3(
                -27.2f,
                -1f,
                0f
            ),
            new Vector3(
                10.4f,
                1f,
                4f
            ),
            groundLayer,
            iron,
            true
        );

        CreateBlock(
            "InspectionDeck",
            roomRoot.transform,
            new Vector3(
                -27.8f,
                1.55f,
                0f
            ),
            new Vector3(
                3.1f,
                0.40f,
                4f
            ),
            groundLayer,
            platform,
            true
        );

        CreateBlock(
            "MaintenanceDeck",
            roomRoot.transform,
            new Vector3(
                -24.5f,
                2.70f,
                0f
            ),
            new Vector3(
                2.5f,
                0.38f,
                4f
            ),
            groundLayer,
            platform,
            true
        );

        // Industrial silhouettes / inactive line 4 automata.
        GameObject dormantA =
            CreateDormantDoryoku(
                doryokuPrefab,
                roomRoot.transform,
                "Doryoku3_Dormant_04",
                new Vector3(
                    -30.4f,
                    -0.48f,
                    1.25f
                ),
                enemyLayer
            );

        GameObject dormantB =
            CreateDormantDoryoku(
                doryokuPrefab,
                roomRoot.transform,
                "Doryoku3_Dormant_11",
                new Vector3(
                    -24.2f,
                    -0.48f,
                    1.25f
                ),
                enemyLayer
            );

        CreateCorruptionAura(
            dormantA,
            violet
        );

        CreateCorruptionAura(
            dormantB,
            violet
        );

        GameObject awakenedEnemy = null;

        if (doryokuPrefab != null)
        {
            awakenedEnemy =
                PrefabUtility.InstantiatePrefab(
                    doryokuPrefab,
                    roomRoot.transform
                ) as GameObject;

            if (awakenedEnemy != null)
            {
                awakenedEnemy.name =
                    "Doryoku3_Chain4_Awakened";

                awakenedEnemy.transform.position =
                    new Vector3(
                        -27.0f,
                        -0.48f,
                        0f
                    );

                awakenedEnemy.transform.localScale =
                    Vector3.one * 0.88f;

                Doryoku3Enemy enemy =
                    awakenedEnemy
                        .GetComponent<Doryoku3Enemy>();

                if (enemy != null)
                {
                    SerializedObject so =
                        new SerializedObject(
                            enemy
                        );

                    SetFloat(
                        so,
                        "patrolDistance",
                        1.5f
                    );

                    SetFloat(
                        so,
                        "detectionRange",
                        8.5f
                    );

                    SetFloat(
                        so,
                        "chaseSpeed",
                        2.65f
                    );

                    SetFloat(
                        so,
                        "attackWindup",
                        0.62f
                    );

                    so.ApplyModifiedPropertiesWithoutUndo();
                }
            }
        }

        GameObject entryGate =
            CreateGate(
                "MachineRoom_EntryGate",
                roomRoot.transform,
                new Vector3(
                    -32.05f,
                    1.0f,
                    0f
                ),
                machine,
                groundLayer
            );

        GameObject exitGate =
            CreateGate(
                "MachineRoom_ExitGate",
                roomRoot.transform,
                new Vector3(
                    -22.15f,
                    1.0f,
                    0f
                ),
                violet,
                groundLayer
            );

        Renderer exitRenderer =
            exitGate != null
                ? exitGate.GetComponent<Renderer>()
                : null;

        GameObject lightObject =
            new GameObject(
                "Chain4_CorruptionLight"
            );

        lightObject.transform.SetParent(
            roomRoot.transform
        );

        lightObject.transform.position =
            new Vector3(
                -27f,
                3.5f,
                -1.4f
            );

        Light roomLight =
            lightObject.AddComponent<Light>();

        roomLight.type =
            LightType.Point;

        roomLight.range =
            10f;

        roomLight.intensity =
            4.2f;

        roomLight.color =
            new Color(
                0.72f,
                0.15f,
                0.92f
            );

        VerticalSliceMachineRoom room =
            roomRoot.AddComponent<
                VerticalSliceMachineRoom
            >();

        VerticalSliceKikaiRelayNode[] relays =
            new VerticalSliceKikaiRelayNode[3];

        relays[0] =
            CreateRelay(
                roomRoot.transform,
                "Relay_A_Ethereal",
                0,
                new Vector3(
                    -30.6f,
                    0.3f,
                    0f
                ),
                KikaiWorldMode.Ethereal,
                room,
                ether
            );

        relays[1] =
            CreateRelay(
                roomRoot.transform,
                "Relay_B_Normal",
                1,
                new Vector3(
                    -27.7f,
                    2.15f,
                    0f
                ),
                KikaiWorldMode.Normal,
                room,
                copper
            );

        relays[2] =
            CreateRelay(
                roomRoot.transform,
                "Relay_C_Ethereal",
                2,
                new Vector3(
                    -24.5f,
                    3.35f,
                    0f
                ),
                KikaiWorldMode.Ethereal,
                room,
                ether
            );

        // Ethereal access step makes the last relay a small Kikai puzzle.
        GameObject spiritStep =
            CreateBlock(
                "Chain4_SpiritStep",
                roomRoot.transform,
                new Vector3(
                    -25.7f,
                    1.25f,
                    0f
                ),
                new Vector3(
                    1.6f,
                    0.32f,
                    4f
                ),
                groundLayer,
                ether,
                true
            );

        if (spiritStep != null)
        {
            KikaiWorldVisibility visibility =
                spiritStep.AddComponent<
                    KikaiWorldVisibility
                >();

            SerializedObject visSO =
                new SerializedObject(
                    visibility
                );

            visSO.FindProperty(
                "visibilityMode"
            ).enumValueIndex =
                (int)
                KikaiVisibilityMode
                    .EtherealOnly;

            visSO.FindProperty(
                "affectColliders"
            ).boolValue =
                true;

            visSO.ApplyModifiedPropertiesWithoutUndo();
        }

        SerializedObject roomSO =
            new SerializedObject(
                room
            );

        roomSO.FindProperty(
            "director"
        ).objectReferenceValue =
            director;

        roomSO.FindProperty(
            "firstCombatEnemy"
        ).objectReferenceValue =
            GetFirstEnemy(
                sliceRoot
            );

        roomSO.FindProperty(
            "awakenedEnemy"
        ).objectReferenceValue =
            awakenedEnemy;

        SerializedProperty relaysProperty =
            roomSO.FindProperty(
                "relays"
            );

        relaysProperty.arraySize =
            relays.Length;

        for (int i = 0; i < relays.Length; i++)
        {
            relaysProperty
                .GetArrayElementAtIndex(i)
                .objectReferenceValue =
                relays[i];
        }

        roomSO.FindProperty(
            "entryGate"
        ).objectReferenceValue =
            entryGate;

        roomSO.FindProperty(
            "exitGate"
        ).objectReferenceValue =
            exitGate;

        roomSO.FindProperty(
            "exitGateRenderer"
        ).objectReferenceValue =
            exitRenderer;

        roomSO.FindProperty(
            "roomLight"
        ).objectReferenceValue =
            roomLight;

        roomSO.ApplyModifiedPropertiesWithoutUndo();

        SerializedObject directorSO =
            new SerializedObject(
                director
            );

        SerializedProperty roomProperty =
            directorSO.FindProperty(
                "machineRoom"
            );

        if (roomProperty != null)
        {
            roomProperty.objectReferenceValue =
                room;
        }

        directorSO.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void RebuildChaseCourse(
        Transform levelRoot,
        Transform sliceRoot,
        int groundLayer,
        Material iron,
        Material platform,
        Material machine,
        Material copper,
        Material ether
    )
    {
        Transform corridor =
            sliceRoot.Find(
                "04_Checkpoint_And_Chase_Corridor"
            );

        if (corridor == null)
            return;

        Transform oldCourse =
            corridor.Find(
                "ChaseCourse_v17_1"
            );

        if (oldCourse != null)
            Object.DestroyImmediate(
                oldCourse.gameObject
            );

        // Remove the v9 continuous floor; v17.1 replaces it with
        // gameplay-readable segments.
        Transform oldGround =
            corridor.Find(
                "Ground_Corridor_And_BossApproach"
            );

        if (oldGround != null)
            Object.DestroyImmediate(
                oldGround.gameObject
            );

        Transform oldPlatform1 =
            corridor.Find(
                "Corridor_Platform_01"
            );

        if (oldPlatform1 != null)
            Object.DestroyImmediate(
                oldPlatform1.gameObject
            );

        Transform oldPlatform2 =
            corridor.Find(
                "Corridor_Platform_02"
            );

        if (oldPlatform2 != null)
            Object.DestroyImmediate(
                oldPlatform2.gameObject
            );

        // The old generic Ground_Main underneath the corridor would fill
        // the chase gaps, so restrict it to the boss arena.
        Transform persistentGround =
            FindRecursive(
                levelRoot,
                "Ground_Main"
            );

        if (persistentGround != null)
        {
            persistentGround.position =
                new Vector3(
                    9.0f,
                    -1f,
                    0f
                );

            persistentGround.localScale =
                new Vector3(
                    18f,
                    1f,
                    4f
                );
        }

        GameObject course =
            new GameObject(
                "ChaseCourse_v17_1"
            );

        course.transform.SetParent(
            corridor
        );

        CreateBlock(
            "CheckpointFloor",
            course.transform,
            new Vector3(
                -20.2f,
                -1f,
                0f
            ),
            new Vector3(
                4.4f,
                1f,
                4f
            ),
            groundLayer,
            iron,
            true
        );

        CreateBlock(
            "ChaseFloor_A",
            course.transform,
            new Vector3(
                -15.4f,
                -1f,
                0f
            ),
            new Vector3(
                5.2f,
                1f,
                4f
            ),
            groundLayer,
            iron,
            true
        );

        CreateBlock(
            "ChaseFloor_B",
            course.transform,
            new Vector3(
                -8.3f,
                -1f,
                0f
            ),
            new Vector3(
                5.2f,
                1f,
                4f
            ),
            groundLayer,
            iron,
            true
        );

        CreateBlock(
            "ChaseFloor_C",
            course.transform,
            new Vector3(
                -1.7f,
                -1f,
                0f
            ),
            new Vector3(
                4.6f,
                1f,
                4f
            ),
            groundLayer,
            iron,
            true
        );

        // Micro-event 1: broken pipe / normal jump.
        CreateBlock(
            "BrokenPipe_Lip_Left",
            course.transform,
            new Vector3(
                -12.55f,
                0.12f,
                0f
            ),
            new Vector3(
                0.55f,
                1.2f,
                4f
            ),
            groundLayer,
            copper,
            true
        );

        CreateBlock(
            "BrokenPipe_Lip_Right",
            course.transform,
            new Vector3(
                -11.25f,
                0.12f,
                0f
            ),
            new Vector3(
                0.55f,
                1.2f,
                4f
            ),
            groundLayer,
            copper,
            true
        );

        // Micro-event 2: ethereal-only bridge.
        GameObject spiritBridge =
            new GameObject(
                "Chase_SpectralBridge"
            );

        spiritBridge.transform.SetParent(
            course.transform
        );

        float[] spectralX =
        {
            -5.45f,
            -4.30f,
            -3.20f
        };

        for (int i = 0; i < spectralX.Length; i++)
        {
            CreateBlock(
                "ChaseSpirit_" + i,
                spiritBridge.transform,
                new Vector3(
                    spectralX[i],
                    -0.05f +
                    i * 0.12f,
                    0f
                ),
                new Vector3(
                    1.0f,
                    0.30f,
                    4f
                ),
                groundLayer,
                ether,
                true
            );
        }

        KikaiWorldVisibility bridgeVisibility =
            spiritBridge.AddComponent<
                KikaiWorldVisibility
            >();

        SerializedObject bridgeSO =
            new SerializedObject(
                bridgeVisibility
            );

        bridgeSO.FindProperty(
            "visibilityMode"
        ).enumValueIndex =
            (int)
            KikaiVisibilityMode
                .EtherealOnly;

        bridgeSO.FindProperty(
            "affectColliders"
        ).boolValue =
            true;

        bridgeSO.ApplyModifiedPropertiesWithoutUndo();

        // Micro-event 3: timed steam vents.
        CreateSteamVent(
            course.transform,
            "SteamVent_A",
            new Vector3(
                -9.2f,
                0.15f,
                0f
            ),
            0.0f,
            machine,
            copper
        );

        CreateSteamVent(
            course.transform,
            "SteamVent_B",
            new Vector3(
                -1.6f,
                0.15f,
                0f
            ),
            1.1f,
            machine,
            copper
        );

        // Vertical rhythm to prevent a pure sprint.
        CreateBlock(
            "ChasePlatform_Upper_A",
            course.transform,
            new Vector3(
                -8.4f,
                1.65f,
                0f
            ),
            new Vector3(
                2.6f,
                0.36f,
                4f
            ),
            groundLayer,
            platform,
            true
        );

        CreateBlock(
            "ChasePlatform_Upper_B",
            course.transform,
            new Vector3(
                -1.0f,
                2.05f,
                0f
            ),
            new Vector3(
                2.4f,
                0.36f,
                4f
            ),
            groundLayer,
            platform,
            true
        );

        // Approach floor links the chase to the existing boss arena floor.
        CreateBlock(
            "BossApproachFloor",
            course.transform,
            new Vector3(
                0.2f,
                -1f,
                0f
            ),
            new Vector3(
                2.5f,
                1f,
                4f
            ),
            groundLayer,
            iron,
            true
        );
    }

    private static void ConfigureDirector(
        VerticalSliceDirector director
    )
    {
        SerializedObject so =
            new SerializedObject(
                director
            );

        SetFloat(
            so,
            "movementDoneX",
            -63f
        );

        SetFloat(
            so,
            "bridgeCrossedX",
            -45f
        );

        so.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void ConfigureChase(
        Transform sliceRoot
    )
    {
        VerticalSliceChaseSequence chase =
            sliceRoot
                .GetComponentInChildren<
                    VerticalSliceChaseSequence
                >(
                    true
                );

        if (chase == null)
            return;

        SerializedObject so =
            new SerializedObject(
                chase
            );

        SetFloat(
            so,
            "startX",
            -18.6f
        );

        SetFloat(
            so,
            "endX",
            0.15f
        );

        so.ApplyModifiedPropertiesWithoutUndo();

        SerializedProperty pursuerProperty =
            so.FindProperty(
                "pursuer"
            );

        GameObject pursuer =
            pursuerProperty != null
                ? pursuerProperty
                    .objectReferenceValue
                    as GameObject
                : null;

        if (pursuer != null)
        {
            pursuer.transform.position =
                new Vector3(
                    -20.6f,
                    -0.48f,
                    0f
                );

            Doryoku3Enemy enemy =
                pursuer
                    .GetComponent<Doryoku3Enemy>();

            if (enemy != null)
            {
                SerializedObject enemySO =
                    new SerializedObject(
                        enemy
                    );

                SetFloat(
                    enemySO,
                    "chaseSpeed",
                    5.2f
                );

                SetFloat(
                    enemySO,
                    "attackWindup",
                    0.72f
                );

                enemySO.ApplyModifiedPropertiesWithoutUndo();
            }
        }
    }

    private static GameObject GetFirstEnemy(
        Transform sliceRoot
    )
    {
        Transform combat =
            sliceRoot.Find(
                "03_First_Doryoku3_Combat"
            );

        if (combat == null)
            return null;

        Transform enemy =
            combat.Find(
                "Doryoku3_FirstEncounter"
            );

        return enemy != null
            ? enemy.gameObject
            : null;
    }

    private static VerticalSliceKikaiRelayNode CreateRelay(
        Transform parent,
        string name,
        int index,
        Vector3 position,
        KikaiWorldMode mode,
        VerticalSliceMachineRoom room,
        Material material
    )
    {
        GameObject root =
            new GameObject(name);

        root.transform.SetParent(parent);
        root.transform.position =
            position;

        BoxCollider trigger =
            root.AddComponent<BoxCollider>();

        trigger.isTrigger = true;
        trigger.size =
            new Vector3(
                1.45f,
                3.0f,
                4f
            );

        GameObject visual =
            GameObject.CreatePrimitive(
                PrimitiveType.Cylinder
            );

        visual.name =
            "RelayVisual";

        visual.transform.SetParent(
            root.transform
        );

        visual.transform.localPosition =
            Vector3.zero;

        visual.transform.localScale =
            new Vector3(
                0.30f,
                0.72f,
                0.30f
            );

        Renderer renderer =
            visual.GetComponent<Renderer>();

        renderer.sharedMaterial =
            material;

        Collider visualCollider =
            visual.GetComponent<Collider>();

        if (visualCollider != null)
            Object.DestroyImmediate(
                visualCollider
            );

        GameObject lightObject =
            new GameObject(
                "RelayLight"
            );

        lightObject.transform.SetParent(
            root.transform
        );

        lightObject.transform.localPosition =
            new Vector3(
                0f,
                0.65f,
                -0.45f
            );

        Light relayLight =
            lightObject.AddComponent<Light>();

        relayLight.type =
            LightType.Point;

        relayLight.range =
            3.0f;

        VerticalSliceKikaiRelayNode relay =
            root.AddComponent<
                VerticalSliceKikaiRelayNode
            >();

        SerializedObject so =
            new SerializedObject(
                relay
            );

        so.FindProperty(
            "relayIndex"
        ).intValue =
            index;

        so.FindProperty(
            "requiredMode"
        ).enumValueIndex =
            (int)mode;

        so.FindProperty(
            "room"
        ).objectReferenceValue =
            room;

        so.FindProperty(
            "relayRenderer"
        ).objectReferenceValue =
            renderer;

        so.FindProperty(
            "relayLight"
        ).objectReferenceValue =
            relayLight;

        so.ApplyModifiedPropertiesWithoutUndo();

        return relay;
    }

    private static GameObject CreateDormantDoryoku(
        GameObject prefab,
        Transform parent,
        string name,
        Vector3 position,
        int enemyLayer
    )
    {
        if (prefab == null)
            return null;

        GameObject go =
            PrefabUtility.InstantiatePrefab(
                prefab,
                parent
            ) as GameObject;

        if (go == null)
            return null;

        go.name = name;
        go.transform.position =
            position;

        go.transform.localScale =
            Vector3.one * 0.82f;

        go.layer =
            enemyLayer;

        Doryoku3Enemy enemy =
            go.GetComponent<Doryoku3Enemy>();

        if (enemy != null)
            enemy.enabled = false;

        Doryoku3FXController fx =
            go.GetComponent<Doryoku3FXController>();

        if (fx != null)
            fx.enabled = false;

        Rigidbody rb =
            go.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity =
                Vector3.zero;
            rb.isKinematic = true;
        }

        Collider[] colliders =
            go.GetComponentsInChildren<Collider>(
                true
            );

        for (int i = 0; i < colliders.Length; i++)
            colliders[i].enabled = false;

        return go;
    }

    private static void CreateCorruptionAura(
        GameObject target,
        Material violet
    )
    {
        if (target == null ||
            violet == null)
        {
            return;
        }

        GameObject aura =
            GameObject.CreatePrimitive(
                PrimitiveType.Sphere
            );

        aura.name =
            "EtherealCorruptionAura";

        aura.transform.SetParent(
            target.transform
        );

        aura.transform.localPosition =
            new Vector3(
                0f,
                1.65f,
                0.15f
            );

        aura.transform.localScale =
            new Vector3(
                1.2f,
                1.8f,
                0.55f
            );

        Renderer renderer =
            aura.GetComponent<Renderer>();

        renderer.sharedMaterial =
            violet;

        Collider collider =
            aura.GetComponent<Collider>();

        if (collider != null)
            Object.DestroyImmediate(
                collider
            );

        KikaiWorldVisibility visibility =
            aura.AddComponent<
                KikaiWorldVisibility
            >();

        SerializedObject so =
            new SerializedObject(
                visibility
            );

        so.FindProperty(
            "visibilityMode"
        ).enumValueIndex =
            (int)
            KikaiVisibilityMode
                .EtherealOnly;

        so.FindProperty(
            "affectColliders"
        ).boolValue =
            false;

        so.ApplyModifiedPropertiesWithoutUndo();
    }

    private static GameObject CreateGate(
        string name,
        Transform parent,
        Vector3 position,
        Material material,
        int groundLayer
    )
    {
        return CreateBlock(
            name,
            parent,
            position,
            new Vector3(
                0.42f,
                4.2f,
                4f
            ),
            groundLayer,
            material,
            true
        );
    }

    private static void CreateSteamVent(
        Transform parent,
        string name,
        Vector3 position,
        float offset,
        Material machine,
        Material copper
    )
    {
        GameObject root =
            new GameObject(name);

        root.transform.SetParent(parent);
        root.transform.position =
            position;

        BoxCollider trigger =
            root.AddComponent<BoxCollider>();

        trigger.isTrigger = true;
        trigger.size =
            new Vector3(
                1.25f,
                3.3f,
                4f
            );

        GameObject vent =
            GameObject.CreatePrimitive(
                PrimitiveType.Cylinder
            );

        vent.name =
            "VentPipe";

        vent.transform.SetParent(
            root.transform
        );

        vent.transform.localPosition =
            new Vector3(
                0f,
                -0.35f,
                0f
            );

        vent.transform.localScale =
            new Vector3(
                0.34f,
                0.55f,
                0.34f
            );

        Renderer renderer =
            vent.GetComponent<Renderer>();

        renderer.sharedMaterial =
            copper != null
                ? copper
                : machine;

        Collider collider =
            vent.GetComponent<Collider>();

        if (collider != null)
            Object.DestroyImmediate(
                collider
            );

        GameObject warning =
            GameObject.CreatePrimitive(
                PrimitiveType.Sphere
            );

        warning.name =
            "SteamWarning";

        warning.transform.SetParent(
            root.transform
        );

        warning.transform.localPosition =
            new Vector3(
                0f,
                1.0f,
                -0.25f
            );

        warning.transform.localScale =
            new Vector3(
                0.28f,
                0.28f,
                0.16f
            );

        Renderer warningRenderer =
            warning.GetComponent<Renderer>();

        warningRenderer.sharedMaterial =
            machine;

        Collider warningCollider =
            warning.GetComponent<Collider>();

        if (warningCollider != null)
            Object.DestroyImmediate(
                warningCollider
            );

        GameObject lightObject =
            new GameObject(
                "SteamWarningLight"
            );

        lightObject.transform.SetParent(
            root.transform
        );

        lightObject.transform.localPosition =
            new Vector3(
                0f,
                1.0f,
                -0.55f
            );

        Light light =
            lightObject.AddComponent<Light>();

        light.type =
            LightType.Point;

        light.range =
            3.5f;

        VerticalSliceSteamVent hazard =
            root.AddComponent<
                VerticalSliceSteamVent
            >();

        SerializedObject so =
            new SerializedObject(
                hazard
            );

        SetFloat(
            so,
            "cycleDuration",
            3.2f
        );

        SetFloat(
            so,
            "activeDuration",
            1.15f
        );

        SetFloat(
            so,
            "cycleOffset",
            offset
        );

        so.FindProperty(
            "warningRenderer"
        ).objectReferenceValue =
            warningRenderer;

        so.FindProperty(
            "warningLight"
        ).objectReferenceValue =
            light;

        so.ApplyModifiedPropertiesWithoutUndo();
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
            GameObject.CreatePrimitive(
                PrimitiveType.Cube
            );

        block.name = name;
        block.transform.SetParent(parent);
        block.transform.position =
            position;

        block.transform.localScale =
            scale;

        block.layer = layer;

        Renderer renderer =
            block.GetComponent<Renderer>();

        if (renderer != null &&
            material != null)
        {
            renderer.sharedMaterial =
                material;
        }

        if (!keepCollider)
        {
            Collider collider =
                block.GetComponent<Collider>();

            if (collider != null)
                Object.DestroyImmediate(
                    collider
                );
        }

        return block;
    }

    private static Material GetMaterial(
        string name
    )
    {
        return AssetDatabase.LoadAssetAtPath<Material>(
            "Assets/_Game/Art/Materials/Prototype/" +
            name +
            ".mat"
        );
    }

    private static void SetFloat(
        SerializedObject so,
        string propertyName,
        float value
    )
    {
        SerializedProperty property =
            so.FindProperty(
                propertyName
            );

        if (property != null)
            property.floatValue =
                value;
    }

    private static Transform FindRecursive(
        Transform root,
        string name
    )
    {
        if (root == null)
            return null;

        if (root.name == name)
            return root;

        for (
            int i = 0;
            i < root.childCount;
            i++
        )
        {
            Transform result =
                FindRecursive(
                    root.GetChild(i),
                    name
                );

            if (result != null)
                return result;
        }

        return null;
    }
}

#endif
