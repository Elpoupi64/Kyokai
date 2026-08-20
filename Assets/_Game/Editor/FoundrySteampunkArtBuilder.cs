#if UNITY_EDITOR

using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public static class FoundrySteampunkArtBuilder
{
    private const string MaterialFolder =
        "Assets/_Game/Art/Materials/FoundrySteampunk";

    public static void Decorate(Transform levelRoot)
    {
        if (levelRoot == null)
            return;

        Transform existing =
            levelRoot.Find("FOUNDRY_ART_V10");

        if (existing != null)
            Object.DestroyImmediate(existing.gameObject);

        DisableGreyboxBackground(levelRoot);
        SetupAtmosphere();
        Camera camera = ConfigureCamera();

        Material castIron =
            GetOrCreateMaterial(
                "CastIron",
                new Color(0.12f, 0.11f, 0.10f),
                false,
                Color.black
            );

        Material weatheredSteel =
            GetOrCreateMaterial(
                "WeatheredSteel",
                new Color(0.22f, 0.20f, 0.18f),
                false,
                Color.black
            );

        Material brass =
            GetOrCreateMaterial(
                "Brass",
                new Color(0.50f, 0.34f, 0.10f),
                false,
                Color.black
            );

        Material copper =
            GetOrCreateMaterial(
                "Copper",
                new Color(0.45f, 0.21f, 0.07f),
                false,
                Color.black
            );

        Material furnaceGlow =
            GetOrCreateMaterial(
                "FurnaceGlow",
                new Color(0.22f, 0.08f, 0.03f),
                true,
                new Color(1.00f, 0.34f, 0.08f) * 3.8f
            );

        Material etherGlow =
            GetOrCreateMaterial(
                "EtherGlow",
                new Color(0.06f, 0.35f, 0.42f),
                true,
                new Color(0.10f, 1.00f, 1.00f) * 3.6f
            );

        Material steamMaterial =
            GetOrCreateMaterial(
                "Steam",
                new Color(0.78f, 0.80f, 0.82f),
                false,
                Color.black
            );

        Material silhouette =
            GetOrCreateMaterial(
                "Silhouette",
                new Color(0.07f, 0.08f, 0.10f),
                false,
                Color.black
            );

        Material midSilhouette =
            GetOrCreateMaterial(
                "MidSilhouette",
                new Color(0.12f, 0.13f, 0.14f),
                false,
                Color.black
            );

        Material frontTrim =
            GetOrCreateMaterial(
                "FrontTrim",
                new Color(0.16f, 0.13f, 0.11f),
                false,
                Color.black
            );

        GameObject artRoot =
            new GameObject("FOUNDRY_ART_V10");

        artRoot.transform.SetParent(levelRoot);

        RetintGameplaySurfaces(
            levelRoot,
            weatheredSteel,
            brass,
            etherGlow
        );

        BuildParallaxBackground(
            artRoot.transform,
            camera,
            silhouette,
            midSilhouette,
            castIron,
            copper,
            brass,
            steamMaterial
        );

        BuildArchitecture(
            artRoot.transform,
            castIron,
            weatheredSteel,
            brass,
            copper,
            furnaceGlow,
            etherGlow,
            frontTrim,
            steamMaterial
        );

        BuildForegroundDress(
            artRoot.transform,
            weatheredSteel,
            brass,
            frontTrim
        );

        GameObject marker =
            new GameObject("FoundryArt_v10");

        marker.transform.SetParent(artRoot.transform);
    }

    private static void DisableGreyboxBackground(
        Transform levelRoot
    )
    {
        Transform sliceRoot =
            levelRoot.Find("VERTICAL_SLICE_V9");

        if (sliceRoot == null)
            return;

        Transform grey =
            sliceRoot.Find("Industrial_Background_Greybox");

        if (grey != null)
            grey.gameObject.SetActive(false);
    }

    private static void SetupAtmosphere()
    {
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.Linear;
        RenderSettings.fogColor =
            new Color(0.08f, 0.08f, 0.10f);
        RenderSettings.fogStartDistance = 16f;
        RenderSettings.fogEndDistance = 85f;

        RenderSettings.ambientMode =
            AmbientMode.Flat;

        RenderSettings.ambientLight =
            new Color(0.19f, 0.17f, 0.16f);
    }

    private static Camera ConfigureCamera()
    {
        Camera camera = Camera.main;

        if (camera == null)
            camera = Object.FindAnyObjectByType<Camera>();

        if (camera != null)
        {
            camera.backgroundColor =
                new Color(0.05f, 0.05f, 0.07f);
            camera.clearFlags =
                CameraClearFlags.SolidColor;
        }

        return camera;
    }

    private static void RetintGameplaySurfaces(
        Transform levelRoot,
        Material groundMaterial,
        Material platformMaterial,
        Material etherealMaterial
    )
    {
        Transform sliceRoot =
            levelRoot.Find("VERTICAL_SLICE_V9");

        if (sliceRoot == null)
            return;

        Renderer[] renderers =
            sliceRoot.GetComponentsInChildren<Renderer>(true);

        foreach (Renderer renderer in renderers)
        {
            if (renderer == null)
                continue;

            string lower =
                renderer.gameObject.name.ToLowerInvariant();

            if (lower.Contains("spiritbridge") ||
                lower.Contains("ethereal"))
            {
                renderer.sharedMaterial = etherealMaterial;
            }
            else if (lower.Contains("platform"))
            {
                renderer.sharedMaterial = platformMaterial;
            }
            else if (lower.Contains("ground") ||
                     lower.Contains("block") ||
                     lower.Contains("checkpoint"))
            {
                renderer.sharedMaterial = groundMaterial;
            }
        }
    }

    private static void BuildParallaxBackground(
        Transform root,
        Camera camera,
        Material farMat,
        Material midMat,
        Material nearMat,
        Material copper,
        Material brass,
        Material steamMaterial
    )
    {
        GameObject farLayer =
            CreateParallaxLayer(
                "Parallax_Far",
                root,
                camera,
                0.10f,
                0.02f
            );

        GameObject midLayer =
            CreateParallaxLayer(
                "Parallax_Mid",
                root,
                camera,
                0.24f,
                0.04f
            );

        GameObject nearLayer =
            CreateParallaxLayer(
                "Parallax_Near",
                root,
                camera,
                0.40f,
                0.06f
            );

        float[] farX = { -74f, -52f, -28f, -6f, 18f };
        float[] farH = { 12f, 16f, 14f, 18f, 16f };

        for (int i = 0; i < farX.Length; i++)
        {
            CreateFactorySilhouette(
                farLayer.transform,
                farX[i],
                8f,
                farH[i],
                30f,
                farMat,
                copper,
                false
            );
        }

        float[] midX = { -70f, -58f, -44f, -30f, -16f, -2f, 12f };
        float[] midH = { 10f, 9f, 11f, 13f, 9f, 12f, 11f };

        for (int i = 0; i < midX.Length; i++)
        {
            CreateFactorySilhouette(
                midLayer.transform,
                midX[i],
                6f,
                midH[i],
                22f,
                midMat,
                brass,
                true
            );
        }

        CreatePipeBand(
            nearLayer.transform,
            -62f,
            5.4f,
            30f,
            nearMat,
            brass,
            18f
        );

        CreatePipeBand(
            nearLayer.transform,
            -18f,
            6.3f,
            38f,
            nearMat,
            copper,
            18f
        );

        CreateSteamVent(
            nearLayer.transform,
            new Vector3(-52f, 4.0f, 15f),
            steamMaterial,
            12f,
            2.2f
        );

        CreateSteamVent(
            nearLayer.transform,
            new Vector3(-11f, 4.8f, 15f),
            steamMaterial,
            14f,
            2.8f
        );
    }

    private static void BuildArchitecture(
        Transform root,
        Material castIron,
        Material steel,
        Material brass,
        Material copper,
        Material furnaceGlow,
        Material etherGlow,
        Material frontTrim,
        Material steamMaterial
    )
    {
        GameObject architecture =
            new GameObject("Architecture_And_Machines");

        architecture.transform.SetParent(root);

        CreateSectionFacade(
            architecture.transform,
            "EntryFacade",
            -66f,
            20f,
            5.5f,
            castIron,
            brass,
            true
        );

        CreateSectionFacade(
            architecture.transform,
            "BridgeFacade",
            -50f,
            16f,
            6.2f,
            steel,
            copper,
            false
        );

        CreateSectionFacade(
            architecture.transform,
            "CombatFacade",
            -38f,
            16f,
            5.8f,
            castIron,
            brass,
            false
        );

        CreateSectionFacade(
            architecture.transform,
            "CorridorFacade",
            -12f,
            36f,
            6.4f,
            steel,
            copper,
            false
        );

        CreateSectionFacade(
            architecture.transform,
            "BossApproachFacade",
            8f,
            22f,
            7.0f,
            castIron,
            brass,
            true
        );

        // Boilers and furnaces
        CreateBoilerCluster(
            architecture.transform,
            new Vector3(-61f, 0.2f, 2.2f),
            castIron,
            copper,
            furnaceGlow
        );

        CreateBoilerCluster(
            architecture.transform,
            new Vector3(-31f, 0.1f, 2.2f),
            steel,
            brass,
            furnaceGlow
        );

        CreateConveyorLine(
            architecture.transform,
            new Vector3(-8f, -0.05f, 2.1f),
            12f,
            steel,
            brass
        );

        CreateMachineRig(
            architecture.transform,
            new Vector3(4.5f, 0.1f, 2.2f),
            castIron,
            brass,
            furnaceGlow
        );

        // Ethereal shrine / reader near bridge.
        CreateEtherNode(
            architecture.transform,
            new Vector3(-55.6f, 0.85f, -0.8f),
            etherGlow,
            brass
        );

        // Steam vents across sections
        CreateSteamVent(
            architecture.transform,
            new Vector3(-64f, 0.5f, 2.2f),
            steamMaterial,
            18f,
            2.5f
        );

        CreateSteamVent(
            architecture.transform,
            new Vector3(-34f, 0.5f, 2.2f),
            steamMaterial,
            16f,
            2.3f
        );

        CreateSteamVent(
            architecture.transform,
            new Vector3(0f, 0.4f, 2.2f),
            steamMaterial,
            15f,
            2.4f
        );

        // Lamps
        float[] lampXs =
        {
            -71f, -63f, -55f, -41f, -25f, -17f, -8f, 0f, 9f
        };

        foreach (float x in lampXs)
        {
            CreateGasLamp(
                architecture.transform,
                new Vector3(x, 0f, -1.8f),
                brass,
                new Color(1.00f, 0.63f, 0.22f),
                x > -58f && x < -46f
                    ? 0.0f
                    : 4.0f
            );
        }

        // Boss arena dress if present.
        Transform bossArena =
            root.parent.Find("MINI_BOSS_ARENA");

        if (bossArena != null)
        {
            CreateBossArenaDress(
                bossArena,
                castIron,
                brass,
                furnaceGlow,
                etherGlow,
                steamMaterial,
                frontTrim
            );
        }
    }

    private static void BuildForegroundDress(
        Transform root,
        Material metal,
        Material brass,
        Material trim
    )
    {
        GameObject fg =
            new GameObject("Foreground_Decor");

        fg.transform.SetParent(root);

        float[] chainXs =
        {
            -69f, -64f, -58f, -49f, -43f,
            -35f, -21f, -10f, -2f, 8f
        };

        for (int i = 0; i < chainXs.Length; i++)
        {
            CreateHangingChain(
                fg.transform,
                new Vector3(chainXs[i], 7.2f, -3.2f),
                5 + (i % 3),
                metal
            );
        }

        CreateRailBand(
            fg.transform,
            -66f,
            18f,
            -2.6f,
            trim
        );

        CreateRailBand(
            fg.transform,
            -10f,
            30f,
            -2.6f,
            trim
        );

        CreateRailBand(
            fg.transform,
            7f,
            16f,
            -2.6f,
            trim
        );
    }

    private static GameObject CreateParallaxLayer(
        string name,
        Transform parent,
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

    private static void CreateFactorySilhouette(
        Transform parent,
        float x,
        float y,
        float height,
        float z,
        Material buildingMaterial,
        Material pipeMaterial,
        bool addLightWindows
    )
    {
        GameObject body =
            CreatePrimitive(
                PrimitiveType.Cube,
                "FactoryBody",
                parent,
                new Vector3(x, y, z),
                new Vector3(8f, height, 3f),
                buildingMaterial,
                false
            );

        CreatePrimitive(
            PrimitiveType.Cylinder,
            "Smokestack_A",
            parent,
            new Vector3(x - 2.0f, y + height * 0.50f + 2.5f, z + 1f),
            new Vector3(0.55f, height * 0.28f, 0.55f),
            buildingMaterial,
            false
        );

        CreatePrimitive(
            PrimitiveType.Cylinder,
            "Smokestack_B",
            parent,
            new Vector3(x + 2.0f, y + height * 0.44f + 1.6f, z - 0.5f),
            new Vector3(0.40f, height * 0.20f, 0.40f),
            pipeMaterial,
            false
        );

        if (addLightWindows)
        {
            for (int i = 0; i < 4; i++)
            {
                CreatePrimitive(
                    PrimitiveType.Cube,
                    "WindowGlow_" + i,
                    parent,
                    new Vector3(x - 2.2f + (i * 1.4f), y - 1.2f, z - 1.6f),
                    new Vector3(0.6f, 0.5f, 0.08f),
                    pipeMaterial,
                    false
                );
            }
        }
    }

    private static void CreatePipeBand(
        Transform parent,
        float centerX,
        float y,
        float width,
        Material pipeMaterial,
        Material jointMaterial,
        float z
    )
    {
        GameObject pipe =
            CreatePrimitive(
                PrimitiveType.Cylinder,
                "PipeBand_Main",
                parent,
                new Vector3(centerX, y, z),
                new Vector3(0.18f, width * 0.5f, 0.18f),
                pipeMaterial,
                false
            );

        pipe.transform.rotation =
            Quaternion.Euler(0f, 0f, 90f);

        for (int i = -2; i <= 2; i++)
        {
            CreatePrimitive(
                PrimitiveType.Cylinder,
                "PipeBand_Joint_" + i,
                parent,
                new Vector3(centerX + i * (width / 5f), y, z),
                new Vector3(0.28f, 0.12f, 0.28f),
                jointMaterial,
                false
            );
        }
    }

    private static void CreateSectionFacade(
        Transform parent,
        string name,
        float centerX,
        float width,
        float height,
        Material wallMaterial,
        Material trimMaterial,
        bool bigArch
    )
    {
        GameObject root =
            new GameObject(name);

        root.transform.SetParent(parent);

        CreatePrimitive(
            PrimitiveType.Cube,
            "BackWall",
            root.transform,
            new Vector3(centerX, height * 0.45f - 1.2f, 3.2f),
            new Vector3(width, height, 0.6f),
            wallMaterial,
            false
        );

        CreatePrimitive(
            PrimitiveType.Cube,
            "BaseTrim",
            root.transform,
            new Vector3(centerX, -0.48f, 2.6f),
            new Vector3(width, 0.45f, 0.6f),
            trimMaterial,
            false
        );

        float archWidth = bigArch ? 4.4f : 3.0f;

        CreatePrimitive(
            PrimitiveType.Cube,
            "Arch_Left",
            root.transform,
            new Vector3(centerX - archWidth * 0.5f, 1.8f, 2.5f),
            new Vector3(0.45f, 4.8f, 0.8f),
            trimMaterial,
            false
        );

        CreatePrimitive(
            PrimitiveType.Cube,
            "Arch_Right",
            root.transform,
            new Vector3(centerX + archWidth * 0.5f, 1.8f, 2.5f),
            new Vector3(0.45f, 4.8f, 0.8f),
            trimMaterial,
            false
        );

        CreatePrimitive(
            PrimitiveType.Cube,
            "Arch_Top",
            root.transform,
            new Vector3(centerX, 4.0f, 2.5f),
            new Vector3(archWidth + 0.6f, 0.45f, 0.8f),
            trimMaterial,
            false
        );
    }

    private static void CreateBoilerCluster(
        Transform parent,
        Vector3 center,
        Material bodyMaterial,
        Material pipeMaterial,
        Material glowMaterial
    )
    {
        GameObject root =
            new GameObject("BoilerCluster");

        root.transform.SetParent(parent);

        CreatePrimitive(
            PrimitiveType.Cylinder,
            "BoilerBody",
            root.transform,
            center + new Vector3(0f, 1.2f, 0f),
            new Vector3(1.5f, 1.9f, 1.5f),
            bodyMaterial,
            false
        );

        CreatePrimitive(
            PrimitiveType.Cube,
            "BoilerSupport",
            root.transform,
            center + new Vector3(0f, 0.2f, 0f),
            new Vector3(2.2f, 0.35f, 1.6f),
            bodyMaterial,
            false
        );

        CreatePrimitive(
            PrimitiveType.Cylinder,
            "FurnaceDoor",
            root.transform,
            center + new Vector3(0f, 1.2f, -1.45f),
            new Vector3(0.55f, 0.12f, 0.55f),
            glowMaterial,
            false
        );

        CreatePrimitive(
            PrimitiveType.Cylinder,
            "BoilerPipe",
            root.transform,
            center + new Vector3(1.4f, 2.5f, 0f),
            new Vector3(0.18f, 1.6f, 0.18f),
            pipeMaterial,
            false
        );

        CreatePrimitive(
            PrimitiveType.Cylinder,
            "BoilerPipeHorizontal",
            root.transform,
            center + new Vector3(2.4f, 3.4f, 0f),
            new Vector3(0.20f, 1.1f, 0.20f),
            pipeMaterial,
            false
        ).transform.rotation =
            Quaternion.Euler(0f, 0f, 90f);
    }

    private static void CreateConveyorLine(
        Transform parent,
        Vector3 start,
        float length,
        Material metal,
        Material trim
    )
    {
        GameObject root =
            new GameObject("ConveyorLine");

        root.transform.SetParent(parent);

        CreatePrimitive(
            PrimitiveType.Cube,
            "Belt",
            root.transform,
            start + new Vector3(length * 0.5f, 0.45f, 0f),
            new Vector3(length, 0.20f, 1.8f),
            metal,
            false
        );

        for (int i = 0; i <= 3; i++)
        {
            float x =
                start.x + i * (length / 3f);

            CreatePrimitive(
                PrimitiveType.Cylinder,
                "Roller_" + i,
                root.transform,
                new Vector3(x, 0.45f, 0f),
                new Vector3(0.18f, 0.95f, 0.18f),
                trim,
                false
            ).transform.rotation =
                Quaternion.Euler(0f, 0f, 90f);
        }

        GameObject gear =
            CreateGearAssembly(
                root.transform,
                start + new Vector3(length + 1.5f, 1.2f, 0f),
                0.95f,
                trim,
                70f
            );

        GameObject gear2 =
            CreateGearAssembly(
                root.transform,
                start + new Vector3(length + 2.85f, 2.1f, 0.1f),
                0.62f,
                metal,
                -115f
            );
    }

    private static void CreateMachineRig(
        Transform parent,
        Vector3 center,
        Material frame,
        Material trim,
        Material glow
    )
    {
        GameObject root =
            new GameObject("MachineRig");

        root.transform.SetParent(parent);

        CreatePrimitive(
            PrimitiveType.Cube,
            "MachineBase",
            root.transform,
            center + new Vector3(0f, 0.45f, 0f),
            new Vector3(4.8f, 0.9f, 2.2f),
            frame,
            false
        );

        CreatePrimitive(
            PrimitiveType.Cube,
            "MachineHead",
            root.transform,
            center + new Vector3(0f, 2.2f, 0f),
            new Vector3(3.5f, 1.3f, 2.0f),
            frame,
            false
        );

        CreatePrimitive(
            PrimitiveType.Cube,
            "FurnaceWindow",
            root.transform,
            center + new Vector3(0.65f, 2.15f, -1.05f),
            new Vector3(1.15f, 0.55f, 0.10f),
            glow,
            false
        );

        CreateGearAssembly(
            root.transform,
            center + new Vector3(-2.6f, 1.4f, 0.1f),
            1.10f,
            trim,
            78f
        );

        CreateGearAssembly(
            root.transform,
            center + new Vector3(2.6f, 1.1f, -0.2f),
            0.72f,
            trim,
            -122f
        );
    }

    private static void CreateEtherNode(
        Transform parent,
        Vector3 position,
        Material ether,
        Material brass
    )
    {
        GameObject root =
            new GameObject("KikaiNode");

        root.transform.SetParent(parent);

        CreatePrimitive(
            PrimitiveType.Cylinder,
            "Base",
            root.transform,
            position + new Vector3(0f, -0.25f, 0f),
            new Vector3(0.45f, 0.35f, 0.45f),
            brass,
            false
        );

        CreatePrimitive(
            PrimitiveType.Sphere,
            "Core",
            root.transform,
            position + new Vector3(0f, 0.55f, 0f),
            new Vector3(0.55f, 0.55f, 0.55f),
            ether,
            false
        );

        GameObject lightObject =
            new GameObject("EtherLight");

        lightObject.transform.SetParent(root.transform);
        lightObject.transform.position =
            position + new Vector3(0f, 0.55f, 0.25f);

        Light light =
            lightObject.AddComponent<Light>();

        light.type = LightType.Point;
        light.range = 5f;
        light.intensity = 3.4f;
        light.color =
            new Color(0.10f, 1.00f, 1.00f);

        lightObject.AddComponent<FoundryLightFlicker>();
    }

    private static void CreateBossArenaDress(
        Transform parent,
        Material castIron,
        Material brass,
        Material glow,
        Material ether,
        Material steamMaterial,
        Material frontTrim
    )
    {
        GameObject root =
            new GameObject("BossArena_FoundryDress");

        root.transform.SetParent(parent);

        CreatePrimitive(
            PrimitiveType.Cube,
            "ArenaBackWall",
            root.transform,
            new Vector3(0f, 3.0f, 4.2f),
            new Vector3(24f, 8.5f, 0.8f),
            castIron,
            false
        );

        CreatePrimitive(
            PrimitiveType.Cube,
            "ArenaTopWalkway",
            root.transform,
            new Vector3(0f, 6.2f, 2.2f),
            new Vector3(15f, 0.35f, 1.4f),
            frontTrim,
            false
        );

        for (int i = -3; i <= 3; i++)
        {
            float x = i * 3.1f;

            CreateGasLamp(
                root.transform,
                new Vector3(x, 0f, -1.6f),
                brass,
                new Color(1.00f, 0.55f, 0.20f),
                4.5f
            );

            CreateSteamVent(
                root.transform,
                new Vector3(x, 0.5f, 2.4f),
                steamMaterial,
                12f,
                1.8f
            );
        }

        CreateGearAssembly(
            root.transform,
            new Vector3(-8f, 2.0f, 2.0f),
            1.25f,
            brass,
            85f
        );

        CreateGearAssembly(
            root.transform,
            new Vector3(8f, 2.0f, 2.0f),
            1.25f,
            brass,
            -85f
        );

        CreatePrimitive(
            PrimitiveType.Cube,
            "EtherealRiftFrame",
            root.transform,
            new Vector3(0f, 4.2f, 1.8f),
            new Vector3(2.6f, 3.8f, 0.18f),
            ether,
            false
        );

        GameObject riftLight =
            new GameObject("RiftLight");

        riftLight.transform.SetParent(root.transform);
        riftLight.transform.position =
            new Vector3(0f, 4.2f, 1.2f);

        Light light =
            riftLight.AddComponent<Light>();

        light.type = LightType.Point;
        light.range = 7f;
        light.intensity = 3.4f;
        light.color = new Color(0.10f, 1.00f, 1.00f);
    }

    private static void CreateGasLamp(
        Transform parent,
        Vector3 basePosition,
        Material poleMaterial,
        Color lightColor,
        float intensity
    )
    {
        GameObject root =
            new GameObject("GasLamp");

        root.transform.SetParent(parent);

        CreatePrimitive(
            PrimitiveType.Cylinder,
            "Pole",
            root.transform,
            basePosition + new Vector3(0f, 1.55f, 0f),
            new Vector3(0.08f, 1.55f, 0.08f),
            poleMaterial,
            false
        );

        CreatePrimitive(
            PrimitiveType.Sphere,
            "Glass",
            root.transform,
            basePosition + new Vector3(0f, 3.2f, 0f),
            new Vector3(0.28f, 0.28f, 0.28f),
            poleMaterial,
            false
        );

        GameObject lightObject =
            new GameObject("LampLight");

        lightObject.transform.SetParent(root.transform);
        lightObject.transform.position =
            basePosition + new Vector3(0f, 3.2f, 0.1f);

        Light light =
            lightObject.AddComponent<Light>();

        light.type = LightType.Point;
        light.range = 7f;
        light.intensity = intensity;
        light.color = lightColor;
        light.shadows = LightShadows.None;

        if (intensity > 0.01f)
        {
            FoundryLightFlicker flicker =
                lightObject.AddComponent<FoundryLightFlicker>();

            SerializedObject flickerSO =
                new SerializedObject(flicker);

            flickerSO.FindProperty("targetLight").objectReferenceValue = light;
            flickerSO.FindProperty("minIntensity").floatValue = intensity * 0.72f;
            flickerSO.FindProperty("maxIntensity").floatValue = intensity * 1.12f;
            flickerSO.FindProperty("speed").floatValue = 8.5f;
            flickerSO.ApplyModifiedPropertiesWithoutUndo();
        }
    }

    private static void CreateHangingChain(
        Transform parent,
        Vector3 topPosition,
        int links,
        Material material
    )
    {
        GameObject root =
            new GameObject("HangingChain");

        root.transform.SetParent(parent);

        for (int i = 0; i < links; i++)
        {
            CreatePrimitive(
                PrimitiveType.Cylinder,
                "Link_" + i,
                root.transform,
                topPosition + new Vector3(0f, -i * 0.55f, 0f),
                new Vector3(0.10f, 0.25f, 0.10f),
                material,
                false
            );
        }

        CreatePrimitive(
            PrimitiveType.Cube,
            "Hook",
            root.transform,
            topPosition + new Vector3(0f, -(links * 0.55f) - 0.25f, 0f),
            new Vector3(0.25f, 0.42f, 0.10f),
            material,
            false
        );
    }

    private static void CreateRailBand(
        Transform parent,
        float centerX,
        float width,
        float z,
        Material material
    )
    {
        GameObject root =
            new GameObject("RailBand");

        root.transform.SetParent(parent);

        CreatePrimitive(
            PrimitiveType.Cube,
            "RailTop",
            root.transform,
            new Vector3(centerX, 1.0f, z),
            new Vector3(width, 0.10f, 0.10f),
            material,
            false
        );

        for (int i = -4; i <= 4; i++)
        {
            CreatePrimitive(
                PrimitiveType.Cube,
                "Post_" + i,
                root.transform,
                new Vector3(centerX + i * (width / 8f), 0.45f, z),
                new Vector3(0.08f, 0.90f, 0.08f),
                material,
                false
            );
        }
    }

    private static GameObject CreateGearAssembly(
        Transform parent,
        Vector3 position,
        float scale,
        Material material,
        float speed
    )
    {
        GameObject root =
            new GameObject("GearAssembly");

        root.transform.SetParent(parent);
        root.transform.position = position;

        GameObject hub =
            CreatePrimitive(
                PrimitiveType.Cylinder,
                "Hub",
                root.transform,
                position,
                new Vector3(scale, 0.12f, scale),
                material,
                false
            );

        for (int i = 0; i < 8; i++)
        {
            float angle = i * 45f;
            Vector3 offset =
                Quaternion.Euler(0f, 0f, angle) *
                Vector3.right *
                (scale * 0.72f);

            GameObject spoke =
                CreatePrimitive(
                    PrimitiveType.Cube,
                    "Tooth_" + i,
                    root.transform,
                    position + offset,
                    new Vector3(0.24f * scale, 0.48f * scale, 0.12f),
                    material,
                    false
                );

            spoke.transform.rotation =
                Quaternion.Euler(0f, 0f, angle);
        }

        RotatingGear rotator =
            root.AddComponent<RotatingGear>();

        SerializedObject so =
            new SerializedObject(rotator);

        so.FindProperty("localAxis").vector3Value =
            Vector3.forward;

        so.FindProperty("speed").floatValue =
            speed;

        so.ApplyModifiedPropertiesWithoutUndo();

        return root;
    }

    private static void CreateSteamVent(
        Transform parent,
        Vector3 position,
        Material material,
        float rate,
        float height
    )
    {
        GameObject root =
            new GameObject("SteamVent");

        root.transform.SetParent(parent);
        root.transform.position = position;

        CreatePrimitive(
            PrimitiveType.Cylinder,
            "VentPipe",
            root.transform,
            position + new Vector3(0f, -0.18f, 0f),
            new Vector3(0.10f, 0.35f, 0.10f),
            material,
            false
        );

        ParticleSystem ps =
            root.AddComponent<ParticleSystem>();

        var main = ps.main;
        main.loop = true;
        main.duration = 1.6f;
        main.startLifetime = 2.2f;
        main.startSpeed = height;
        main.startSize = 0.60f;
        main.startColor =
            new Color(0.82f, 0.84f, 0.86f, 0.35f);
        main.maxParticles = 200;
        main.simulationSpace =
            ParticleSystemSimulationSpace.World;
        main.gravityModifier = -0.05f;

        var emission = ps.emission;
        emission.rateOverTime = rate;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 12f;
        shape.radius = 0.08f;

        var sizeOverLifetime = ps.sizeOverLifetime;
        sizeOverLifetime.enabled = true;

        AnimationCurve sizeCurve =
            new AnimationCurve(
                new Keyframe(0f, 0.2f),
                new Keyframe(0.4f, 0.65f),
                new Keyframe(1f, 1.45f)
            );

        sizeOverLifetime.size =
            new ParticleSystem.MinMaxCurve(1f, sizeCurve);

        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;

        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new[]
            {
                new GradientColorKey(
                    new Color(0.80f, 0.83f, 0.86f),
                    0f
                ),
                new GradientColorKey(
                    new Color(0.58f, 0.62f, 0.67f),
                    1f
                )
            },
            new[]
            {
                new GradientAlphaKey(0.0f, 0f),
                new GradientAlphaKey(0.28f, 0.12f),
                new GradientAlphaKey(0.18f, 0.55f),
                new GradientAlphaKey(0.0f, 1f)
            }
        );

        colorOverLifetime.color =
            new ParticleSystem.MinMaxGradient(gradient);

        var velocity = ps.velocityOverLifetime;
        velocity.enabled = true;
        velocity.x =
            new ParticleSystem.MinMaxCurve(-0.18f, 0.18f);
        velocity.y =
            new ParticleSystem.MinMaxCurve(0.3f, 1.2f);
        velocity.z =
            new ParticleSystem.MinMaxCurve(0f, 0f);

        var noise = ps.noise;
        noise.enabled = true;
        noise.strength = 0.35f;
        noise.frequency = 0.4f;
        noise.scrollSpeed = 0.25f;

        ParticleSystemRenderer renderer =
            ps.GetComponent<ParticleSystemRenderer>();

        if (material != null)
            renderer.sharedMaterial = material;

        renderer.renderMode =
            ParticleSystemRenderMode.Billboard;
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
