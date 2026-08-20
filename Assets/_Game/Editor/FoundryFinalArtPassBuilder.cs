#if UNITY_EDITOR

using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public static class FoundryFinalArtPassBuilder
{
    private const string MaterialFolder =
        "Assets/_Game/Art/Materials/FoundryFinalPass";

    public static void Apply(Transform levelRoot)
    {
        if (levelRoot == null)
            return;

        FoundryArtModuleLibraryBuilder.ModuleLibrary library =
            FoundryArtModuleLibraryBuilder.BuildOrLoad();

        Transform oldRoot =
            levelRoot.Find("FOUNDRY_ART_V12");

        if (oldRoot != null)
            Object.DestroyImmediate(oldRoot.gameObject);

        SetupFinalAtmosphere();

        Material bannerRed =
            GetOrCreateMaterial(
                "Final_BannerRed",
                new Color(0.52f, 0.08f, 0.09f),
                false,
                Color.black
            );

        Material parchment =
            GetOrCreateMaterial(
                "Final_Parchment",
                new Color(0.78f, 0.70f, 0.52f),
                false,
                Color.black
            );

        Material jade =
            GetOrCreateMaterial(
                "Final_Jade",
                new Color(0.12f, 0.42f, 0.38f),
                false,
                Color.black
            );

        Color inkDark =
            new Color(0.07f, 0.07f, 0.08f);

        Material warmGlow =
            GetOrCreateMaterial(
                "Final_WarmGlow",
                new Color(0.28f, 0.09f, 0.03f),
                true,
                new Color(1.0f, 0.45f, 0.16f) * 4.2f
            );

        Material cyanGlow =
            GetOrCreateMaterial(
                "Final_CyanGlow",
                new Color(0.08f, 0.34f, 0.40f),
                true,
                new Color(0.12f, 0.95f, 1.00f) * 4.3f
            );

        Material sunDisc =
            GetOrCreateMaterial(
                "Final_SunDisc",
                new Color(0.62f, 0.23f, 0.14f),
                true,
                new Color(0.72f, 0.20f, 0.10f) * 1.6f
            );

        Material smokeCard =
            GetOrCreateMaterial(
                "Final_SmokeCard",
                new Color(0.21f, 0.22f, 0.25f),
                false,
                Color.black
            );

        Material silhouette =
            GetOrCreateMaterial(
                "Final_SilhouetteDeep",
                new Color(0.04f, 0.05f, 0.06f),
                false,
                Color.black
            );

        Material foreground =
            GetOrCreateMaterial(
                "Final_ForegroundFrame",
                new Color(0.08f, 0.06f, 0.05f),
                false,
                Color.black
            );

        GameObject artRoot =
            new GameObject("FOUNDRY_ART_V12");

        artRoot.transform.SetParent(levelRoot);

        BuildPainterlyBackdrop(
            artRoot.transform,
            library,
            sunDisc,
            smokeCard,
            silhouette
        );

        BuildNarrativeSetDressing(
            artRoot.transform,
            library,
            bannerRed,
            parchment,
            jade,
            inkDark,
            warmGlow,
            cyanGlow
        );

        BuildForegroundFrame(
            artRoot.transform,
            foreground,
            bannerRed
        );

        BuildBossFinalDress(
            levelRoot,
            library,
            bannerRed,
            parchment,
            jade,
            cyanGlow,
            warmGlow,
            foreground
        );

        GameObject marker =
            new GameObject("FoundryFinalArt_v12");

        marker.transform.SetParent(artRoot.transform);
    }

    private static void SetupFinalAtmosphere()
    {
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.Linear;
        RenderSettings.fogColor =
            new Color(0.07f, 0.06f, 0.08f);
        RenderSettings.fogStartDistance = 10f;
        RenderSettings.fogEndDistance = 70f;

        RenderSettings.ambientMode =
            AmbientMode.Flat;
        RenderSettings.ambientLight =
            new Color(0.23f, 0.18f, 0.16f);

        Camera camera =
            Camera.main != null
                ? Camera.main
                : Object.FindAnyObjectByType<Camera>();

        if (camera != null)
        {
            camera.backgroundColor =
                new Color(0.06f, 0.05f, 0.07f);
            camera.clearFlags =
                CameraClearFlags.SolidColor;
            camera.nearClipPlane = 0.1f;
            camera.farClipPlane = 200f;
        }
    }

    private static void BuildPainterlyBackdrop(
        Transform root,
        FoundryArtModuleLibraryBuilder.ModuleLibrary library,
        Material sunDisc,
        Material smokeCard,
        Material silhouette
    )
    {
        Camera camera =
            Camera.main != null
                ? Camera.main
                : Object.FindAnyObjectByType<Camera>();

        GameObject far =
            CreateParallaxLayer(root, "Painterly_Backdrop_Far", camera, 0.06f, 0.01f);

        GameObject mid =
            CreateParallaxLayer(root, "Painterly_Backdrop_Mid", camera, 0.16f, 0.03f);

        GameObject near =
            CreateParallaxLayer(root, "Painterly_Backdrop_Near", camera, 0.28f, 0.05f);

        GameObject sun =
            CreatePrimitive(
                PrimitiveType.Cylinder,
                "SunDisc",
                far.transform,
                new Vector3(-8f, 13.0f, 33f),
                new Vector3(5.8f, 0.08f, 5.8f),
                sunDisc,
                false
            );

        sun.transform.rotation =
            Quaternion.Euler(90f, 0f, 0f);

        float[] farX = { -78f, -58f, -38f, -16f, 5f, 24f };

        for (int i = 0; i < farX.Length; i++)
        {
            InstantiateModule(
                i % 2 == 0
                    ? library.BackgroundFactoryB
                    : library.BackgroundFactoryA,
                far.transform,
                new Vector3(farX[i], 0f, 28f),
                new Vector3(1.2f, 1.1f, 1f)
            );
        }

        float[] smokeX = { -66f, -48f, -32f, -12f, 7f };

        for (int i = 0; i < smokeX.Length; i++)
        {
            CreatePrimitive(
                PrimitiveType.Cube,
                "SmokeCard_" + i,
                mid.transform,
                new Vector3(smokeX[i], 12.5f + (i % 2), 24f),
                new Vector3(8f, 2.5f, 0.1f),
                smokeCard,
                false
            );
        }

        float[] nearX = { -70f, -56f, -43f, -30f, -17f, -4f, 10f };

        for (int i = 0; i < nearX.Length; i++)
        {
            CreatePrimitive(
                PrimitiveType.Cube,
                "SilhouetteColumn_" + i,
                near.transform,
                new Vector3(nearX[i], 5.0f + (i % 3), 18f),
                new Vector3(4.0f, 10f + (i % 3) * 2.0f, 1.2f),
                silhouette,
                false
            );
        }
    }

    private static void BuildNarrativeSetDressing(
        Transform root,
        FoundryArtModuleLibraryBuilder.ModuleLibrary library,
        Material bannerRed,
        Material parchment,
        Material jade,
        Color inkDark,
        Material warmGlow,
        Material cyanGlow
    )
    {
        GameObject dress =
            new GameObject("Narrative_SetDressing");

        dress.transform.SetParent(root);

        // Entrance identity.
        CreateHangingBanner(
            dress.transform,
            new Vector3(-72.5f, 6.6f, -1.2f),
            bannerRed,
            "KATSUHIRO",
            Color.white,
            2.1f,
            4.4f
        );

        CreateHangingBanner(
            dress.transform,
            new Vector3(-68.2f, 6.2f, -1.2f),
            bannerRed,
            "FOUNDRY",
            Color.white,
            1.9f,
            4.0f
        );

        CreateSignboard(
            dress.transform,
            new Vector3(-64.0f, 4.6f, -0.9f),
            parchment,
            inkDark,
            "TOKYO 1889"
        );

        // Kikai bridge motifs.
        CreateSignboard(
            dress.transform,
            new Vector3(-54.7f, 3.7f, -0.9f),
            parchment,
            inkDark,
            "ETHER LOCK"
        );

        CreateLanternString(
            dress.transform,
            new Vector3(-55f, 5.9f, -1.0f),
            4,
            warmGlow
        );

        CreateHangingBanner(
            dress.transform,
            new Vector3(-50.3f, 5.8f, -1.1f),
            jade,
            "KIKAI",
            Color.white,
            1.5f,
            3.2f
        );

        // First combat warning.
        CreateSignboard(
            dress.transform,
            new Vector3(-40f, 4.3f, -0.9f),
            bannerRed,
            Color.white,
            "DANGER"
        );

        InstantiateModule(
            library.GearAssemblySmall,
            dress.transform,
            new Vector3(-35.8f, 3.2f, 1.2f),
            new Vector3(0.85f, 0.85f, 0.85f)
        );

        // Corridor / production lane.
        CreateLanternString(
            dress.transform,
            new Vector3(-18f, 6.1f, -1.0f),
            6,
            warmGlow
        );

        CreateSignboard(
            dress.transform,
            new Vector3(-21f, 4.7f, -0.9f),
            parchment,
            inkDark,
            "SYNCHRONIZE"
        );

        CreateSignboard(
            dress.transform,
            new Vector3(-8f, 4.2f, -0.9f),
            bannerRed,
            Color.white,
            "KEEP MOVING"
        );

        InstantiateModule(
            library.PipeRack,
            dress.transform,
            new Vector3(-2f, 5.2f, 1.1f),
            new Vector3(2.3f, 1f, 1f)
        );

        // Boss approach / cyan shrine.
        CreateHangingBanner(
            dress.transform,
            new Vector3(7.2f, 6.1f, -1.2f),
            bannerRed,
            "UNIT 07",
            Color.white,
            2.0f,
            4.2f
        );

        CreateSignboard(
            dress.transform,
            new Vector3(10.5f, 4.6f, -0.9f),
            cyanGlow,
            Color.white,
            "BREACH"
        );
    }

    private static void BuildForegroundFrame(
        Transform root,
        Material foreground,
        Material bannerRed
    )
    {
        GameObject fg =
            new GameObject("Foreground_Frame");

        fg.transform.SetParent(root);

        float[] leftPipeX = { -74f, -58f, -40f, -21f, -2f, 12f };

        for (int i = 0; i < leftPipeX.Length; i++)
        {
            GameObject pipe =
                CreatePrimitive(
                    PrimitiveType.Cylinder,
                    "ForegroundPipe_" + i,
                    fg.transform,
                    new Vector3(leftPipeX[i], 4.8f, -5.2f),
                    new Vector3(0.34f, 6.5f, 0.34f),
                    foreground,
                    false
                );

            pipe.transform.rotation =
                Quaternion.Euler(0f, 0f, 90f);
        }

        float[] framePillarX = { -76f, -60f, -44f, -28f, -12f, 4f, 18f };

        for (int i = 0; i < framePillarX.Length; i++)
        {
            CreatePrimitive(
                PrimitiveType.Cube,
                "FramePillar_" + i,
                fg.transform,
                new Vector3(framePillarX[i], 4.8f, -4.9f),
                new Vector3(0.55f, 10.0f, 0.8f),
                foreground,
                false
            );
        }

        CreateHangingBanner(
            fg.transform,
            new Vector3(-27f, 7.0f, -4.4f),
            bannerRed,
            "",
            Color.white,
            1.4f,
            3.6f
        );

        CreateHangingBanner(
            fg.transform,
            new Vector3(2.0f, 6.8f, -4.4f),
            bannerRed,
            "",
            Color.white,
            1.3f,
            3.4f
        );
    }

    private static void BuildBossFinalDress(
        Transform levelRoot,
        FoundryArtModuleLibraryBuilder.ModuleLibrary library,
        Material bannerRed,
        Material parchment,
        Material jade,
        Material cyanGlow,
        Material warmGlow,
        Material foreground
    )
    {
        Transform arena =
            levelRoot.Find("MINI_BOSS_ARENA");

        if (arena == null)
            return;

        Transform old =
            arena.Find("BossArena_FinalDress");

        if (old != null)
            Object.DestroyImmediate(old.gameObject);

        GameObject root =
            new GameObject("BossArena_FinalDress");

        root.transform.SetParent(arena);

        CreateHangingBanner(
            root.transform,
            new Vector3(-6f, 6.4f, -1.3f),
            bannerRed,
            "UNIT",
            Color.white,
            1.9f,
            4.3f
        );

        CreateHangingBanner(
            root.transform,
            new Vector3(6f, 6.4f, -1.3f),
            bannerRed,
            "07",
            Color.white,
            1.9f,
            4.3f
        );

        CreateSignboard(
            root.transform,
            new Vector3(0f, 6.2f, -0.9f),
            parchment,
            Color.black,
            "KIKAI-YUREI"
        );

        InstantiateModule(
            library.GearAssemblyLarge,
            root.transform,
            new Vector3(-10.2f, 2.3f, 1.6f),
            new Vector3(1.2f, 1.2f, 1f)
        );

        InstantiateModule(
            library.GearAssemblyLarge,
            root.transform,
            new Vector3(10.2f, 2.3f, 1.6f),
            new Vector3(1.2f, 1.2f, 1f)
        );

        GameObject rift =
            CreatePrimitive(
                PrimitiveType.Cylinder,
                "BossRiftHalo",
                root.transform,
                new Vector3(0f, 4.1f, 1.45f),
                new Vector3(2.2f, 0.1f, 2.2f),
                cyanGlow,
                false
            );

        rift.transform.rotation =
            Quaternion.Euler(90f, 0f, 0f);

        AddPulse(
            rift,
            new Color(0.12f, 1.00f, 1.00f)
        );

        CreateLanternString(
            root.transform,
            new Vector3(0f, 5.6f, -1.2f),
            5,
            warmGlow
        );

        // Slight frame around arena.
        CreatePrimitive(
            PrimitiveType.Cube,
            "ArenaFrameTop",
            root.transform,
            new Vector3(0f, 7.1f, -4.4f),
            new Vector3(24f, 0.5f, 0.8f),
            foreground,
            false
        );
    }

    private static GameObject CreateParallaxLayer(
        Transform parent,
        string name,
        Camera camera,
        float xFactor,
        float yFactor
    )
    {
        GameObject layer =
            new GameObject(name);

        layer.transform.SetParent(parent);

        ParallaxLayer parallax =
            layer.AddComponent<ParallaxLayer>();

        SerializedObject so =
            new SerializedObject(parallax);

        if (camera != null)
            so.FindProperty("targetCamera").objectReferenceValue =
                camera.transform;

        so.FindProperty("xFactor").floatValue = xFactor;
        so.FindProperty("yFactor").floatValue = yFactor;
        so.FindProperty("affectY").boolValue = true;
        so.ApplyModifiedPropertiesWithoutUndo();

        return layer;
    }

    private static GameObject CreatePrimitive(
        PrimitiveType type,
        string name,
        Transform parent,
        Vector3 position,
        Vector3 scale,
        Material material,
        bool keepCollider
    )
    {
        GameObject go =
            GameObject.CreatePrimitive(type);

        go.name = name;
        go.transform.SetParent(parent);
        go.transform.position = position;
        go.transform.localScale = scale;

        Renderer renderer =
            go.GetComponent<Renderer>();

        if (renderer != null && material != null)
            renderer.sharedMaterial = material;

        if (!keepCollider)
        {
            Collider collider =
                go.GetComponent<Collider>();

            if (collider != null)
                Object.DestroyImmediate(collider);
        }

        return go;
    }

    private static GameObject InstantiateModule(
        GameObject prefab,
        Transform parent,
        Vector3 position,
        Vector3 scale
    )
    {
        if (prefab == null)
            return null;

        GameObject instance =
            PrefabUtility.InstantiatePrefab(
                prefab,
                parent
            ) as GameObject;

        instance.transform.position = position;
        instance.transform.localScale = scale;
        return instance;
    }

    private static void CreateHangingBanner(
        Transform parent,
        Vector3 position,
        Material clothMaterial,
        string text,
        Color textColor,
        float width,
        float height
    )
    {
        GameObject root =
            new GameObject("Banner");

        root.transform.SetParent(parent);
        root.transform.position = position;

        CreatePrimitive(
            PrimitiveType.Cube,
            "CrossBar",
            root.transform,
            position + new Vector3(0f, 0f, 0f),
            new Vector3(width + 0.4f, 0.08f, 0.08f),
            clothMaterial,
            false
        );

        GameObject cloth =
            CreatePrimitive(
                PrimitiveType.Cube,
                "Cloth",
                root.transform,
                position + new Vector3(0f, -height * 0.5f, 0.02f),
                new Vector3(width, height, 0.06f),
                clothMaterial,
                false
            );

        FoundryAutoSway sway =
            cloth.AddComponent<FoundryAutoSway>();

        SerializedObject swaySO =
            new SerializedObject(sway);

        swaySO.FindProperty("axis").vector3Value =
            Vector3.forward;

        swaySO.FindProperty("amplitude").floatValue =
            2.5f;

        swaySO.FindProperty("speed").floatValue =
            1.15f;

        swaySO.ApplyModifiedPropertiesWithoutUndo();

        if (!string.IsNullOrEmpty(text))
        {
            GameObject textObject =
                new GameObject("Text");

            textObject.transform.SetParent(cloth.transform);
            textObject.transform.localPosition =
                new Vector3(0f, 0f, -0.04f);

            TextMesh mesh =
                textObject.AddComponent<TextMesh>();

            mesh.text = text;
            mesh.fontSize = 64;
            mesh.characterSize = 0.08f;
            mesh.anchor = TextAnchor.MiddleCenter;
            mesh.alignment = TextAlignment.Center;
            mesh.color = textColor;
        }
    }

    private static void CreateSignboard(
        Transform parent,
        Vector3 position,
        Material boardMaterial,
        Color textColor,
        string text
    )
    {
        GameObject root =
            new GameObject("Signboard");

        root.transform.SetParent(parent);
        root.transform.position = position;

        CreatePrimitive(
            PrimitiveType.Cube,
            "Board",
            root.transform,
            position,
            new Vector3(2.4f, 0.9f, 0.12f),
            boardMaterial,
            false
        );

        CreatePrimitive(
            PrimitiveType.Cube,
            "Post_Left",
            root.transform,
            position + new Vector3(-0.9f, -1.1f, 0f),
            new Vector3(0.08f, 1.2f, 0.08f),
            boardMaterial,
            false
        );

        CreatePrimitive(
            PrimitiveType.Cube,
            "Post_Right",
            root.transform,
            position + new Vector3(0.9f, -1.1f, 0f),
            new Vector3(0.08f, 1.2f, 0.08f),
            boardMaterial,
            false
        );

        GameObject textObject =
            new GameObject("Text");

        textObject.transform.SetParent(root.transform);
        textObject.transform.position =
            position + new Vector3(0f, 0f, -0.08f);

        TextMesh mesh =
            textObject.AddComponent<TextMesh>();

        mesh.text = text;
        mesh.fontSize = 56;
        mesh.characterSize = 0.07f;
        mesh.anchor = TextAnchor.MiddleCenter;
        mesh.alignment = TextAlignment.Center;
        mesh.color = textColor;
    }

    private static void CreateLanternString(
        Transform parent,
        Vector3 center,
        int count,
        Material glowMaterial
    )
    {
        GameObject root =
            new GameObject("LanternString");

        root.transform.SetParent(parent);

        float spacing = 1.1f;
        float startX = -((count - 1) * spacing) * 0.5f;

        CreatePrimitive(
            PrimitiveType.Cube,
            "Wire",
            root.transform,
            center,
            new Vector3(count * spacing, 0.05f, 0.05f),
            glowMaterial,
            false
        );

        for (int i = 0; i < count; i++)
        {
            float x = center.x + startX + i * spacing;

            GameObject lantern =
                CreatePrimitive(
                    PrimitiveType.Sphere,
                    "Lantern_" + i,
                    root.transform,
                    new Vector3(x, center.y - 0.35f - Mathf.Abs(i - (count * 0.5f)) * 0.04f, center.z),
                    new Vector3(0.22f, 0.22f, 0.22f),
                    glowMaterial,
                    false
                );

            AddPulse(
                lantern,
                new Color(1.0f, 0.48f, 0.18f)
            );
        }
    }

    private static void AddPulse(
        GameObject target,
        Color emission
    )
    {
        FoundryModulePulse pulse =
            target.AddComponent<FoundryModulePulse>();

        SerializedObject so =
            new SerializedObject(pulse);

        so.FindProperty("emissionColor").colorValue =
            emission;

        so.FindProperty("minIntensity").floatValue =
            0.8f;

        so.FindProperty("maxIntensity").floatValue =
            1.4f;

        so.FindProperty("speed").floatValue =
            2.0f;

        so.ApplyModifiedPropertiesWithoutUndo();
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

        if (emission && material.HasProperty("_EmissionColor"))
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
