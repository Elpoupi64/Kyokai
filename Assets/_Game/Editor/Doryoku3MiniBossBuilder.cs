#if UNITY_EDITOR

using System.IO;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class Doryoku3MiniBossBuilder
{
    private const string ScenePath =
        "Assets/_Game/Scenes/Prototype/Foundry_Prototype.unity";

    private const string MaterialFolder =
        "Assets/_Game/Art/Materials/Prototype";

    static Doryoku3MiniBossBuilder()
    {
        EditorApplication.delayCall +=
            AutoUpgradeSceneIfNeeded;
    }

    private static void
        AutoUpgradeSceneIfNeeded()
    {
        if (EditorApplication
            .isPlayingOrWillChangePlaymode)
        {
            return;
        }

        if (!File.Exists(ScenePath))
            return;

        string sceneText =
            File.ReadAllText(ScenePath);

        if (!sceneText.Contains(
            "Doryoku3_MiniBoss_Unit07"
        ))
        {
            FoundryPrototypeSceneBuilder
                .CreateOrRebuild();
        }
    }

    public static void AddMiniBossToScene(
        Transform levelRoot,
        GameObject doryokuPrefab,
        Transform player,
        int enemyLayer,
        int groundLayer
    )
    {
        if (doryokuPrefab == null ||
            levelRoot == null ||
            player == null)
        {
            return;
        }

        EnsureFolder(
            "Assets/_Game/Art"
        );

        EnsureFolder(
            "Assets/_Game/Art/Materials"
        );

        EnsureFolder(
            MaterialFolder
        );

        Material normalShockwave =
            GetOrCreateMaterial(
                "Doryoku3_Boss_Shockwave",
                new Color(
                    0.42f,
                    0.12f,
                    0.02f
                ),
                new Color(
                    1.00f,
                    0.25f,
                    0.02f
                ) * 3.0f
            );

        Material enragedShockwave =
            GetOrCreateMaterial(
                "Doryoku3_Boss_Shockwave_Enraged",
                new Color(
                    0.30f,
                    0.04f,
                    0.38f
                ),
                new Color(
                    0.85f,
                    0.08f,
                    1.00f
                ) * 3.5f
            );

        Material spectralMaterial =
            AssetDatabase.LoadAssetAtPath<
                Material
            >(
                MaterialFolder +
                "/Doryoku3_Ethereal_Particles.mat"
            );

        if (spectralMaterial == null)
        {
            spectralMaterial =
                enragedShockwave;
        }

        GameObject arena =
            new GameObject(
                "MINI_BOSS_ARENA"
            );

        arena.transform.SetParent(
            levelRoot
        );

        GameObject bossObject =
            PrefabUtility.InstantiatePrefab(
                doryokuPrefab,
                arena.transform
            ) as GameObject;

        bossObject.name =
            "Doryoku3_MiniBoss_Unit07";

        bossObject.transform.position =
            new Vector3(
                11.2f,
                -0.48f,
                0f
            );

        bossObject.transform.rotation =
            Quaternion.identity;

        bossObject.transform.localScale =
            Vector3.one * 1.16f;

        bossObject.layer =
            enemyLayer;

        Doryoku3Enemy regularEnemy =
            bossObject.GetComponent<
                Doryoku3Enemy
            >();

        if (regularEnemy != null)
        {
            Object.DestroyImmediate(
                regularEnemy
            );
        }

        Transform oldHealthBar =
            bossObject.transform.Find(
                "HealthBar"
            );

        if (oldHealthBar != null)
            oldHealthBar.gameObject
                .SetActive(false);

        Doryoku3VisualController visuals =
            bossObject.GetComponent<
                Doryoku3VisualController
            >();

        Doryoku3FXController fx =
            bossObject.GetComponent<
                Doryoku3FXController
            >();

        Transform specialOrigin =
            bossObject.transform.Find(
                "SpecialAttackOrigin"
            );

        Doryoku3MiniBoss boss =
            bossObject.AddComponent<
                Doryoku3MiniBoss
            >();

        SerializedObject bossSO =
            new SerializedObject(boss);

        bossSO.FindProperty(
            "visuals"
        ).objectReferenceValue =
            visuals;

        bossSO.FindProperty(
            "fx"
        ).objectReferenceValue =
            fx;

        bossSO.FindProperty(
            "specialAttackOrigin"
        ).objectReferenceValue =
            specialOrigin;

        bossSO.FindProperty(
            "specialProjectileMaterial"
        ).objectReferenceValue =
            spectralMaterial;

        bossSO.FindProperty(
            "groundWaveMaterial"
        ).objectReferenceValue =
            normalShockwave;

        bossSO.FindProperty(
            "enragedGroundWaveMaterial"
        ).objectReferenceValue =
            enragedShockwave;

        CameraFollow25D camera =
            Camera.main != null
                ? Camera.main.GetComponent<
                    CameraFollow25D
                  >()
                : Object.FindAnyObjectByType<
                    CameraFollow25D
                  >();

        bossSO.FindProperty(
            "bossCamera"
        ).objectReferenceValue =
            camera;

        bossSO.ApplyModifiedPropertiesWithoutUndo();

        // Cinematic HUD.
        GameObject hudObject =
            new GameObject(
                "Doryoku3_BossHUD"
            );

        hudObject.transform.SetParent(
            arena.transform
        );

        Doryoku3BossHUD hud =
            hudObject.AddComponent<
                Doryoku3BossHUD
            >();

        // Invisible arena gates become active only
        // once the intro begins.
        GameObject leftGate =
            CreateArenaGate(
                "BossGate_Left",
                arena.transform,
                new Vector3(
                    1.1f,
                    2.2f,
                    0f
                ),
                groundLayer
            );

        GameObject rightGate =
            CreateArenaGate(
                "BossGate_Right",
                arena.transform,
                new Vector3(
                    16.6f,
                    2.2f,
                    0f
                ),
                groundLayer
            );

        leftGate.SetActive(false);
        rightGate.SetActive(false);

        Doryoku3BossEncounter encounter =
            arena.AddComponent<
                Doryoku3BossEncounter
            >();

        SerializedObject encounterSO =
            new SerializedObject(
                encounter
            );

        encounterSO.FindProperty(
            "boss"
        ).objectReferenceValue =
            boss;

        encounterSO.FindProperty(
            "player"
        ).objectReferenceValue =
            player;

        encounterSO.FindProperty(
            "playerMotor"
        ).objectReferenceValue =
            player.GetComponent<
                PlayerMotor25D
            >();

        encounterSO.FindProperty(
            "playerCombat"
        ).objectReferenceValue =
            player.GetComponent<
                KenjiroCombatController
            >();

        encounterSO.FindProperty(
            "playerAttack"
        ).objectReferenceValue =
            player.GetComponent<
                PlayerAttackPrototype
            >();

        encounterSO.FindProperty(
            "playerHealth"
        ).objectReferenceValue =
            player.GetComponent<
                PlayerHealth
            >();

        encounterSO.FindProperty(
            "bossCamera"
        ).objectReferenceValue =
            camera;

        encounterSO.FindProperty(
            "bossHUD"
        ).objectReferenceValue =
            hud;

        encounterSO.FindProperty(
            "leftGate"
        ).objectReferenceValue =
            leftGate;

        encounterSO.FindProperty(
            "rightGate"
        ).objectReferenceValue =
            rightGate;

        encounterSO.ApplyModifiedPropertiesWithoutUndo();
    }

    private static GameObject
        CreateArenaGate(
            string name,
            Transform parent,
            Vector3 position,
            int groundLayer
        )
    {
        GameObject gate =
            GameObject.CreatePrimitive(
                PrimitiveType.Cube
            );

        gate.name = name;
        gate.transform.SetParent(parent);
        gate.transform.position = position;

        gate.transform.localScale =
            new Vector3(
                0.45f,
                6.5f,
                4.0f
            );

        gate.layer = groundLayer;

        Renderer renderer =
            gate.GetComponent<Renderer>();

        if (renderer != null)
            renderer.enabled = false;

        return gate;
    }

    private static Material
        GetOrCreateMaterial(
            string materialName,
            Color baseColor,
            Color emissionColor
        )
    {
        string path =
            MaterialFolder +
            "/" +
            materialName +
            ".mat";

        Material existing =
            AssetDatabase.LoadAssetAtPath<
                Material
            >(path);

        if (existing != null)
            return existing;

        Shader shader =
            Shader.Find(
                "Universal Render Pipeline/Lit"
            );

        if (shader == null)
            shader =
                Shader.Find("Standard");

        Material material =
            new Material(shader);

        material.name =
            materialName;

        if (material.HasProperty(
            "_BaseColor"
        ))
        {
            material.SetColor(
                "_BaseColor",
                baseColor
            );
        }

        if (material.HasProperty(
            "_Color"
        ))
        {
            material.SetColor(
                "_Color",
                baseColor
            );
        }

        if (material.HasProperty(
            "_EmissionColor"
        ))
        {
            material.EnableKeyword(
                "_EMISSION"
            );

            material.SetColor(
                "_EmissionColor",
                emissionColor
            );
        }

        AssetDatabase.CreateAsset(
            material,
            path
        );

        return material;
    }

    private static void EnsureFolder(
        string path
    )
    {
        if (AssetDatabase.IsValidFolder(
            path
        ))
        {
            return;
        }

        string parent =
            Path.GetDirectoryName(path)
                ?.Replace("\\", "/");

        string folder =
            Path.GetFileName(path);

        if (!string.IsNullOrEmpty(parent) &&
            !AssetDatabase.IsValidFolder(
                parent
            ))
        {
            EnsureFolder(parent);
        }

        if (!string.IsNullOrEmpty(parent))
            AssetDatabase.CreateFolder(
                parent,
                folder
            );
    }
}

#endif
