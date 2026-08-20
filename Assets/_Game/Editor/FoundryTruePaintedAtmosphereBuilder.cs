#if UNITY_EDITOR

using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public static class FoundryTruePaintedAtmosphereBuilder
{
    private const string TextureFolder =
        "Assets/_Game/Art/Textures/FoundryPainted";

    private const string MaterialFolder =
        "Assets/_Game/Art/Materials/FoundryAtmosphereV14";

    private const string MeshFolder =
        "Assets/_Game/Art/Meshes/FoundryPainted";

    private const string PostFolder =
        "Assets/_Game/Art/PostProcess";

    public static void Apply(Transform levelRoot)
    {
        if (levelRoot == null)
            return;

        Transform old =
            levelRoot.Find("FOUNDRY_ART_V14");

        if (old != null)
            UnityEngine.Object.DestroyImmediate(old.gameObject);

        DisableEarlierBackgroundLayers(levelRoot);
        EnsureFolders();

        SetupWorldPalette();

        Material warmSky =
            CreateTexturedMaterial(
                "V14_PaintedSkyWarm",
                TextureFolder + "/FoundrySky_WarmPainted.png",
                Color.white,
                false
            );

        Material etherSky =
            CreateTexturedMaterial(
                "V14_PaintedSkyEther",
                TextureFolder + "/FoundrySky_EtherPainted.png",
                Color.white,
                false
            );

        Material smoke =
            CreateTexturedMaterial(
                "V14_PaintedSmoke",
                TextureFolder + "/FoundrySmoke_Painted.png",
                Color.white,
                true
            );

        Material ink =
            CreateTexturedMaterial(
                "V14_PaintedInk",
                TextureFolder + "/FoundryInk_Silhouette.png",
                Color.white,
                true
            );

        Material rift =
            CreateTexturedMaterial(
                "V14_PaintedRift",
                TextureFolder + "/FoundryBoss_RiftPainted.png",
                new Color(0.80f, 1.00f, 1.00f),
                true
            );

        Material organicSilhouette =
            CreateFlatUnlitMaterial(
                "V14_OrganicSilhouette",
                new Color(0.035f, 0.035f, 0.045f, 1f)
            );

        Material gameplayWarm =
            CreateEmissiveMaterial(
                "V14_GameplayWarmEdge",
                new Color(0.44f, 0.18f, 0.06f),
                new Color(0.95f, 0.34f, 0.08f) * 1.15f
            );

        Material gameplayEther =
            CreateEmissiveMaterial(
                "V14_GameplayEtherEdge",
                new Color(0.04f, 0.27f, 0.31f),
                new Color(0.10f, 0.95f, 1.00f) * 1.50f
            );

        GameObject root =
            new GameObject("FOUNDRY_ART_V14");

        root.transform.SetParent(levelRoot);

        BuildPaintedBackdrop(
            root.transform,
            warmSky,
            etherSky,
            smoke,
            ink,
            organicSilhouette
        );

        BuildAmbientVFX(root.transform);
        BuildReactiveLighting(root.transform);
        BuildGameplayReadability(
            root.transform,
            gameplayWarm,
            gameplayEther
        );
        BuildBossRift(levelRoot, rift);
        BuildPostProcessing(root.transform);

        GameObject marker =
            new GameObject("FoundryTruePainted_v14");

        marker.transform.SetParent(root.transform);
    }

    private static void DisableEarlierBackgroundLayers(
        Transform levelRoot
    )
    {
        DisableChildrenStartingWith(
            levelRoot.Find("FOUNDRY_ART_V11"),
            "Parallax_"
        );

        DisableChildrenStartingWith(
            levelRoot.Find("FOUNDRY_ART_V12"),
            "Painterly_Backdrop_"
        );

        DisableChildrenStartingWith(
            levelRoot.Find("FOUNDRY_ART_V13"),
            "HeroBackdrop_"
        );
    }

    private static void DisableChildrenStartingWith(
        Transform root,
        string prefix
    )
    {
        if (root == null)
            return;

        for (int i = 0; i < root.childCount; i++)
        {
            Transform child = root.GetChild(i);

            if (child.name.StartsWith(
                prefix,
                StringComparison.OrdinalIgnoreCase
            ))
            {
                child.gameObject.SetActive(false);
            }
        }
    }

    private static void SetupWorldPalette()
    {
        KikaiWorldManager manager =
            KikaiWorldManager.Instance;

        if (manager == null)
            manager =
                UnityEngine.Object.FindAnyObjectByType<KikaiWorldManager>();

        if (manager != null)
        {
            SerializedObject so =
                new SerializedObject(manager);

            so.FindProperty("normalAmbient").colorValue =
                new Color(0.28f, 0.21f, 0.17f);

            so.FindProperty("normalFogColor").colorValue =
                new Color(0.085f, 0.065f, 0.070f);

            so.FindProperty("normalFogDensity").floatValue =
                0.018f;

            so.FindProperty("etherealAmbient").colorValue =
                new Color(0.10f, 0.23f, 0.28f);

            so.FindProperty("etherealFogColor").colorValue =
                new Color(0.035f, 0.115f, 0.155f);

            so.FindProperty("etherealFogDensity").floatValue =
                0.028f;

            so.ApplyModifiedPropertiesWithoutUndo();
        }

        RenderSettings.ambientMode =
            AmbientMode.Flat;

        RenderSettings.fog = true;
    }

    private static void BuildPaintedBackdrop(
        Transform root,
        Material warmSky,
        Material etherSky,
        Material smoke,
        Material ink,
        Material organicSilhouette
    )
    {
        Camera camera =
            Camera.main != null
                ? Camera.main
                : UnityEngine.Object.FindAnyObjectByType<Camera>();

        GameObject far =
            CreateParallaxLayer(
                root,
                "TruePainted_Far",
                camera,
                0.035f,
                0.008f
            );

        GameObject mid =
            CreateParallaxLayer(
                root,
                "TruePainted_Mid",
                camera,
                0.10f,
                0.018f
            );

        GameObject near =
            CreateParallaxLayer(
                root,
                "TruePainted_Near",
                camera,
                0.21f,
                0.035f
            );

        CreateQuad(
            "WarmPaintedSky",
            far.transform,
            new Vector3(-22f, 10.5f, 39f),
            new Vector3(105f, 45f, 1f),
            warmSky
        );

        GameObject etherealCard =
            CreateQuad(
                "EtherealPaintedSky",
                far.transform,
                new Vector3(-22f, 10.5f, 38.8f),
                new Vector3(105f, 45f, 1f),
                etherSky
            );

        KikaiWorldVisibility etherealVisibility =
            etherealCard.AddComponent<KikaiWorldVisibility>();

        SerializedObject visibilitySO =
            new SerializedObject(etherealVisibility);

        visibilitySO.FindProperty("visibilityMode").enumValueIndex =
            (int)KikaiVisibilityMode.EtherealOnly;

        visibilitySO.FindProperty("affectColliders").boolValue =
            false;

        visibilitySO.ApplyModifiedPropertiesWithoutUndo();

        float[] smokeX =
        {
            -72f, -54f, -36f, -18f, 0f, 18f
        };

        for (int i = 0; i < smokeX.Length; i++)
        {
            GameObject card =
                CreateQuad(
                    "PaintedSmoke_" + i,
                    mid.transform,
                    new Vector3(
                        smokeX[i],
                        10.5f + (i % 2) * 1.5f,
                        28f
                    ),
                    new Vector3(19f, 10f, 1f),
                    smoke
                );

            AddBackdropDrift(
                card,
                new Vector3(0.35f, 0.08f, 0f),
                0.55f + i * 0.03f,
                0.12f + i * 0.015f
            );
        }

        float[] inkX =
        {
            -74f, -48f, -22f, 4f, 30f
        };

        for (int i = 0; i < inkX.Length; i++)
        {
            CreateQuad(
                "InkSilhouette_" + i,
                mid.transform,
                new Vector3(
                    inkX[i],
                    4.6f,
                    23f
                ),
                new Vector3(26f, 13f, 1f),
                ink
            );
        }

        Mesh skylineA =
            GetOrCreateSkylineMesh(
                "OrganicSkyline_A",
                new float[]
                {
                    4.0f, 5.2f, 4.5f, 7.4f, 6.0f, 8.1f,
                    5.4f, 6.8f, 4.7f, 7.0f, 5.1f, 6.2f
                }
            );

        Mesh skylineB =
            GetOrCreateSkylineMesh(
                "OrganicSkyline_B",
                new float[]
                {
                    5.1f, 4.2f, 6.5f, 5.7f, 8.0f, 4.8f,
                    7.1f, 5.0f, 6.4f, 7.7f, 4.5f, 5.9f
                }
            );

        CreateMeshObject(
            "OrganicSkyline_Left",
            near.transform,
            skylineA,
            organicSilhouette,
            new Vector3(-58f, -1f, 18f),
            new Vector3(1.8f, 1.25f, 1f)
        );

        CreateMeshObject(
            "OrganicSkyline_Right",
            near.transform,
            skylineB,
            organicSilhouette,
            new Vector3(-4f, -1f, 18f),
            new Vector3(1.8f, 1.25f, 1f)
        );
    }

    private static void BuildAmbientVFX(
        Transform root
    )
    {
        Texture2D soft =
            AssetDatabase.LoadAssetAtPath<Texture2D>(
                TextureFolder + "/FoundrySoftParticle.png"
            );

        Material dustMat =
            CreateParticleMaterial(
                "V14_DustParticle",
                soft,
                new Color(0.62f, 0.58f, 0.55f, 0.25f)
            );

        Material emberMat =
            CreateParticleMaterial(
                "V14_EmberParticle",
                soft,
                new Color(1.00f, 0.36f, 0.08f, 0.78f)
            );

        Material steamMat =
            CreateParticleMaterial(
                "V14_SteamParticle",
                soft,
                new Color(0.72f, 0.76f, 0.80f, 0.20f)
            );

        Material etherMat =
            CreateParticleMaterial(
                "V14_EtherParticle",
                soft,
                new Color(0.10f, 0.95f, 1.00f, 0.72f)
            );

        GameObject vfxRoot =
            new GameObject("Atmosphere_VFX");

        vfxRoot.transform.SetParent(root);

        CreateParticleField(
            "DustMotes",
            vfxRoot.transform,
            new Vector3(-25f, 5f, -0.2f),
            new Vector3(105f, 16f, 5f),
            28f,
            7.0f,
            0.10f,
            0.15f,
            new Vector3(0.10f, 0.02f, 0f),
            dustMat,
            false
        );

        CreateParticleField(
            "EmberField",
            vfxRoot.transform,
            new Vector3(-12f, 3f, 0f),
            new Vector3(52f, 7f, 5f),
            22f,
            3.2f,
            0.07f,
            0.13f,
            new Vector3(0.05f, 0.70f, 0f),
            emberMat,
            false
        );

        CreateParticleField(
            "LowSteamMist",
            vfxRoot.transform,
            new Vector3(-22f, 0.7f, 0.4f),
            new Vector3(100f, 1.8f, 5f),
            18f,
            4.5f,
            0.28f,
            0.65f,
            new Vector3(0.05f, 0.35f, 0f),
            steamMat,
            false
        );

        GameObject etherField =
            CreateParticleField(
                "EtherMotes",
                vfxRoot.transform,
                new Vector3(-12f, 4f, 0f),
                new Vector3(100f, 12f, 5f),
                36f,
                4.0f,
                0.06f,
                0.12f,
                new Vector3(0.04f, 0.22f, 0f),
                etherMat,
                true
            );

        FoundryWorldReactiveParticles reactive =
            etherField.AddComponent<FoundryWorldReactiveParticles>();

        SerializedObject reactiveSO =
            new SerializedObject(reactive);

        reactiveSO.FindProperty("particles").objectReferenceValue =
            etherField.GetComponent<ParticleSystem>();

        reactiveSO.FindProperty("playInNormal").boolValue =
            false;

        reactiveSO.FindProperty("playInEthereal").boolValue =
            true;

        reactiveSO.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void BuildReactiveLighting(
        Transform root
    )
    {
        GameObject player =
            GameObject.FindGameObjectWithTag("Player");

        if (player == null)
            player =
                GameObject.Find("Player_Kenjiro");

        if (player != null)
        {
            GameObject focus =
                new GameObject("GameplayFocusLight");

            focus.transform.SetParent(root);

            Light light =
                focus.AddComponent<Light>();

            light.type =
                LightType.Point;

            light.range =
                7.0f;

            light.intensity =
                1.35f;

            light.shadows =
                LightShadows.None;

            FoundryGameplayFocusLight follow =
                focus.AddComponent<FoundryGameplayFocusLight>();

            SerializedObject so =
                new SerializedObject(follow);

            so.FindProperty("target").objectReferenceValue =
                player.transform;

            so.FindProperty("focusLight").objectReferenceValue =
                light;

            so.ApplyModifiedPropertiesWithoutUndo();
        }

        CreateReactiveLight(
            root,
            "EtherBridgeReactiveLight",
            new Vector3(-51f, 2.2f, -1.2f),
            7.5f,
            4.3f
        );

        CreateReactiveLight(
            root,
            "BossReactiveLight",
            new Vector3(7.5f, 3.0f, -1.2f),
            9.0f,
            5.2f
        );
    }

    private static void BuildGameplayReadability(
        Transform root,
        Material warm,
        Material ether
    )
    {
        GameObject readability =
            new GameObject("Gameplay_Readability");

        readability.transform.SetParent(root);

        CreateEdgeStrip(
            readability.transform,
            "StartGroundEdge",
            new Vector3(-66f, -0.44f, -2.03f),
            new Vector3(20f, 0.08f, 0.08f),
            warm,
            false
        );

        CreateEdgeStrip(
            readability.transform,
            "CombatGroundEdge",
            new Vector3(-39f, -0.44f, -2.03f),
            new Vector3(14f, 0.08f, 0.08f),
            warm,
            false
        );

        CreateEdgeStrip(
            readability.transform,
            "CorridorGroundEdge",
            new Vector3(-7f, -0.44f, -2.03f),
            new Vector3(50f, 0.08f, 0.08f),
            warm,
            false
        );

        CreateEdgeStrip(
            readability.transform,
            "TutorialPlatform01Edge",
            new Vector3(-68.5f, 1.29f, -2.03f),
            new Vector3(3.2f, 0.08f, 0.08f),
            warm,
            false
        );

        CreateEdgeStrip(
            readability.transform,
            "TutorialPlatform02Edge",
            new Vector3(-63.5f, 2.39f, -2.03f),
            new Vector3(3.0f, 0.08f, 0.08f),
            warm,
            false
        );

        float[] bridgeX =
        {
            -54.6f, -51.8f, -49.0f, -46.4f
        };

        for (int i = 0; i < bridgeX.Length; i++)
        {
            float y =
                -0.20f +
                Mathf.Sin(i * 0.8f) * 0.30f +
                0.23f;

            CreateEdgeStrip(
                readability.transform,
                "EtherBridgeEdge_" + i,
                new Vector3(bridgeX[i], y, -2.03f),
                new Vector3(2.3f, 0.08f, 0.08f),
                ether,
                true
            );
        }
    }

    private static void BuildBossRift(
        Transform levelRoot,
        Material riftMaterial
    )
    {
        Transform arena =
            levelRoot.Find("MINI_BOSS_ARENA");

        if (arena == null)
            return;

        Transform old =
            arena.Find("BossRift_Painted_v14");

        if (old != null)
            UnityEngine.Object.DestroyImmediate(old.gameObject);

        GameObject card =
            CreateQuad(
                "BossRift_Painted_v14",
                arena,
                new Vector3(0f, 4.2f, 1.25f),
                new Vector3(6.2f, 6.2f, 1f),
                riftMaterial
            );

        FoundryBackdropDrift drift =
            card.AddComponent<FoundryBackdropDrift>();

        SerializedObject driftSO =
            new SerializedObject(drift);

        driftSO.FindProperty("movement").vector3Value =
            new Vector3(0.03f, 0.05f, 0f);

        driftSO.FindProperty("amplitude").floatValue =
            0.45f;

        driftSO.FindProperty("speed").floatValue =
            0.75f;

        driftSO.ApplyModifiedPropertiesWithoutUndo();

        KikaiWorldVisibility visibility =
            card.AddComponent<KikaiWorldVisibility>();

        SerializedObject visibilitySO =
            new SerializedObject(visibility);

        visibilitySO.FindProperty("visibilityMode").enumValueIndex =
            (int)KikaiVisibilityMode.EtherealOnly;

        visibilitySO.FindProperty("affectColliders").boolValue =
            false;

        visibilitySO.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void BuildPostProcessing(
        Transform root
    )
    {
        EnsureFolder(PostFolder);

        string profilePath =
            PostFolder + "/Foundry_Atmosphere_v14.asset";

        VolumeProfile profile =
            AssetDatabase.LoadAssetAtPath<VolumeProfile>(
                profilePath
            );

        if (profile == null)
        {
            profile =
                ScriptableObject.CreateInstance<VolumeProfile>();

            AssetDatabase.CreateAsset(
                profile,
                profilePath
            );
        }

        AddOrConfigureVolumeOverride(
            profile,
            "UnityEngine.Rendering.Universal.Bloom",
            new string[]
            {
                "intensity=0.38",
                "threshold=1.05",
                "scatter=0.62"
            }
        );

        AddOrConfigureVolumeOverride(
            profile,
            "UnityEngine.Rendering.Universal.ColorAdjustments",
            new string[]
            {
                "postExposure=0.08",
                "contrast=12",
                "saturation=-6"
            }
        );

        AddOrConfigureVolumeOverride(
            profile,
            "UnityEngine.Rendering.Universal.Vignette",
            new string[]
            {
                "intensity=0.22",
                "smoothness=0.44"
            }
        );

        AddOrConfigureVolumeOverride(
            profile,
            "UnityEngine.Rendering.Universal.Tonemapping",
            new string[]
            {
                "mode=ACES"
            }
        );

        EditorUtility.SetDirty(profile);

        GameObject volumeObject =
            new GameObject("GlobalPostProcess_v14");

        volumeObject.transform.SetParent(root);

        Volume volume =
            volumeObject.AddComponent<Volume>();

        volume.isGlobal = true;
        volume.priority = 50f;
        volume.weight = 1f;
        volume.sharedProfile = profile;

        Camera camera =
            Camera.main != null
                ? Camera.main
                : UnityEngine.Object.FindAnyObjectByType<Camera>();

        if (camera != null)
        {
            camera.allowHDR = true;
            EnableURPPostProcessing(camera);
        }
    }

    private static void EnableURPPostProcessing(
        Camera camera
    )
    {
        Type additionalDataType =
            FindLoadedType(
                "UnityEngine.Rendering.Universal.UniversalAdditionalCameraData"
            );

        if (additionalDataType == null)
            return;

        Component data =
            camera.GetComponent(additionalDataType);

        if (data == null)
            data =
                camera.gameObject.AddComponent(additionalDataType);

        PropertyInfo property =
            additionalDataType.GetProperty(
                "renderPostProcessing",
                BindingFlags.Public |
                BindingFlags.Instance
            );

        if (property != null &&
            property.CanWrite)
        {
            property.SetValue(data, true);
        }
    }

    private static void AddOrConfigureVolumeOverride(
        VolumeProfile profile,
        string typeName,
        string[] settings
    )
    {
        Type componentType =
            FindLoadedType(typeName);

        if (componentType == null)
        {
            Debug.LogWarning(
                "Katsuhiro v14 : post-process URP non trouvé : " +
                typeName
            );
            return;
        }

        VolumeComponent component = null;

        foreach (VolumeComponent item in profile.components)
        {
            if (item != null &&
                item.GetType() == componentType)
            {
                component = item;
                break;
            }
        }

        if (component == null)
        {
            MethodInfo addMethod =
                typeof(VolumeProfile).GetMethod(
                    "Add",
                    new Type[]
                    {
                        typeof(Type),
                        typeof(bool)
                    }
                );

            if (addMethod != null)
            {
                try
                {
                    component =
                        addMethod.Invoke(
                            profile,
                            new object[]
                            {
                                componentType,
                                true
                            }
                        ) as VolumeComponent;
                }
                catch (TargetInvocationException)
                {
                    // The profile already has this override (e.g. the
                    // asset was built by an earlier pass and the
                    // in-memory components list hadn't picked it up
                    // yet). Re-scan instead of crashing.
                    foreach (VolumeComponent item in profile.components)
                    {
                        if (item != null &&
                            item.GetType() == componentType)
                        {
                            component = item;
                            break;
                        }
                    }
                }
            }
        }

        if (component == null)
            return;

        foreach (string setting in settings)
        {
            string[] parts =
                setting.Split('=');

            if (parts.Length != 2)
                continue;

            SetVolumeParameter(
                component,
                parts[0],
                parts[1]
            );
        }

        EditorUtility.SetDirty(component);
    }

    private static void SetVolumeParameter(
        VolumeComponent component,
        string fieldName,
        string rawValue
    )
    {
        FieldInfo field =
            component.GetType().GetField(
                fieldName,
                BindingFlags.Public |
                BindingFlags.Instance
            );

        if (field == null)
            return;

        object parameter =
            field.GetValue(component);

        if (parameter == null)
            return;

        Type parameterType =
            parameter.GetType();

        PropertyInfo overrideProperty =
            parameterType.GetProperty(
                "overrideState",
                BindingFlags.Public |
                BindingFlags.Instance
            );

        if (overrideProperty != null &&
            overrideProperty.CanWrite)
        {
            overrideProperty.SetValue(
                parameter,
                true
            );
        }

        PropertyInfo valueProperty =
            parameterType.GetProperty(
                "value",
                BindingFlags.Public |
                BindingFlags.Instance
            );

        if (valueProperty == null ||
            !valueProperty.CanWrite)
            return;

        Type valueType =
            valueProperty.PropertyType;

        try
        {
            object converted;

            if (valueType == typeof(float))
            {
                converted =
                    float.Parse(
                        rawValue,
                        System.Globalization.CultureInfo.InvariantCulture
                    );
            }
            else if (valueType == typeof(int))
            {
                converted =
                    int.Parse(rawValue);
            }
            else if (valueType == typeof(bool))
            {
                converted =
                    bool.Parse(rawValue);
            }
            else if (valueType.IsEnum)
            {
                converted =
                    Enum.Parse(
                        valueType,
                        rawValue,
                        true
                    );
            }
            else
            {
                return;
            }

            valueProperty.SetValue(
                parameter,
                converted
            );
        }
        catch
        {
            // Keep default if the URP version uses a different parameter type.
        }
    }

    private static Type FindLoadedType(
        string fullName
    )
    {
        foreach (
            System.Reflection.Assembly assembly
            in AppDomain.CurrentDomain.GetAssemblies()
        )
        {
            Type type =
                assembly.GetType(fullName);

            if (type != null)
                return type;
        }

        return null;
    }

    private static GameObject CreateParticleField(
        string name,
        Transform parent,
        Vector3 position,
        Vector3 boxScale,
        float emissionRate,
        float lifetime,
        float minSize,
        float maxSize,
        Vector3 velocity,
        Material material,
        bool startStopped
    )
    {
        GameObject root =
            new GameObject(name);

        root.transform.SetParent(parent);
        root.transform.position = position;

        ParticleSystem ps =
            root.AddComponent<ParticleSystem>();

        var main = ps.main;
        main.loop = true;
        main.duration = 2f;
        main.startLifetime = lifetime;
        main.startSpeed = 0f;
        main.startSize =
            new ParticleSystem.MinMaxCurve(
                minSize,
                maxSize
            );

        main.simulationSpace =
            ParticleSystemSimulationSpace.World;

        main.maxParticles =
            600;

        var emission = ps.emission;
        emission.rateOverTime =
            emissionRate;

        var shape = ps.shape;
        shape.shapeType =
            ParticleSystemShapeType.Box;

        shape.scale =
            boxScale;

        var velocityModule =
            ps.velocityOverLifetime;

        velocityModule.enabled = true;

        velocityModule.x =
            velocity.x;

        velocityModule.y =
            velocity.y;

        velocityModule.z =
            velocity.z;

        var noise = ps.noise;
        noise.enabled = true;
        noise.strength = 0.20f;
        noise.frequency = 0.25f;
        noise.scrollSpeed = 0.20f;

        ParticleSystemRenderer renderer =
            ps.GetComponent<ParticleSystemRenderer>();

        renderer.renderMode =
            ParticleSystemRenderMode.Billboard;

        renderer.sharedMaterial =
            material;

        if (startStopped)
            ps.Stop(
                true,
                ParticleSystemStopBehavior.StopEmittingAndClear
            );
        else
            ps.Play();

        return root;
    }

    private static void CreateReactiveLight(
        Transform parent,
        string name,
        Vector3 position,
        float range,
        float etherealIntensity
    )
    {
        GameObject root =
            new GameObject(name);

        root.transform.SetParent(parent);
        root.transform.position = position;

        Light light =
            root.AddComponent<Light>();

        light.type =
            LightType.Point;

        light.range =
            range;

        light.intensity =
            0.15f;

        light.shadows =
            LightShadows.None;

        FoundryWorldReactiveLight reactive =
            root.AddComponent<FoundryWorldReactiveLight>();

        SerializedObject so =
            new SerializedObject(reactive);

        so.FindProperty("targetLight").objectReferenceValue =
            light;

        so.FindProperty("etherealIntensity").floatValue =
            etherealIntensity;

        so.ApplyModifiedPropertiesWithoutUndo();
    }

    private static GameObject CreateEdgeStrip(
        Transform parent,
        string name,
        Vector3 position,
        Vector3 scale,
        Material material,
        bool etherealOnly
    )
    {
        GameObject strip =
            GameObject.CreatePrimitive(
                PrimitiveType.Cube
            );

        strip.name = name;
        strip.transform.SetParent(parent);
        strip.transform.position = position;
        strip.transform.localScale = scale;

        Renderer renderer =
            strip.GetComponent<Renderer>();

        renderer.sharedMaterial =
            material;

        Collider collider =
            strip.GetComponent<Collider>();

        if (collider != null)
            UnityEngine.Object.DestroyImmediate(
                collider
            );

        if (etherealOnly)
        {
            KikaiWorldVisibility visibility =
                strip.AddComponent<KikaiWorldVisibility>();

            SerializedObject so =
                new SerializedObject(visibility);

            so.FindProperty("visibilityMode").enumValueIndex =
                (int)KikaiVisibilityMode.EtherealOnly;

            so.FindProperty("affectColliders").boolValue =
                false;

            so.ApplyModifiedPropertiesWithoutUndo();
        }

        return strip;
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

        so.FindProperty("xFactor").floatValue =
            xFactor;

        so.FindProperty("yFactor").floatValue =
            yFactor;

        so.FindProperty("affectY").boolValue =
            true;

        so.ApplyModifiedPropertiesWithoutUndo();

        return layer;
    }

    private static GameObject CreateQuad(
        string name,
        Transform parent,
        Vector3 position,
        Vector3 scale,
        Material material
    )
    {
        GameObject card =
            GameObject.CreatePrimitive(
                PrimitiveType.Quad
            );

        card.name = name;
        card.transform.SetParent(parent);
        card.transform.position = position;
        card.transform.localScale = scale;

        Renderer renderer =
            card.GetComponent<Renderer>();

        renderer.sharedMaterial =
            material;

        Collider collider =
            card.GetComponent<Collider>();

        if (collider != null)
            UnityEngine.Object.DestroyImmediate(
                collider
            );

        return card;
    }

    private static void AddBackdropDrift(
        GameObject target,
        Vector3 movement,
        float amplitude,
        float speed
    )
    {
        FoundryBackdropDrift drift =
            target.AddComponent<FoundryBackdropDrift>();

        SerializedObject so =
            new SerializedObject(drift);

        so.FindProperty("movement").vector3Value =
            movement;

        so.FindProperty("amplitude").floatValue =
            amplitude;

        so.FindProperty("speed").floatValue =
            speed;

        so.FindProperty("localSpace").boolValue =
            true;

        so.ApplyModifiedPropertiesWithoutUndo();
    }

    private static Mesh GetOrCreateSkylineMesh(
        string name,
        float[] heights
    )
    {
        string path =
            MeshFolder + "/" + name + ".asset";

        Mesh existing =
            AssetDatabase.LoadAssetAtPath<Mesh>(
                path
            );

        if (existing != null)
            return existing;

        float step = 2.5f;
        float bottom = -2.0f;
        int count = heights.Length;

        Vector3[] vertices =
            new Vector3[count * 2];

        Vector2[] uv =
            new Vector2[count * 2];

        int[] triangles =
            new int[(count - 1) * 6];

        for (int i = 0; i < count; i++)
        {
            float x =
                i * step;

            vertices[i * 2] =
                new Vector3(x, bottom, 0f);

            vertices[i * 2 + 1] =
                new Vector3(
                    x,
                    heights[i],
                    0f
                );

            uv[i * 2] =
                new Vector2(
                    i / (float)(count - 1),
                    0f
                );

            uv[i * 2 + 1] =
                new Vector2(
                    i / (float)(count - 1),
                    1f
                );
        }

        for (int i = 0; i < count - 1; i++)
        {
            int t = i * 6;
            int v = i * 2;

            triangles[t + 0] = v;
            triangles[t + 1] = v + 1;
            triangles[t + 2] = v + 3;

            triangles[t + 3] = v;
            triangles[t + 4] = v + 3;
            triangles[t + 5] = v + 2;
        }

        Mesh mesh =
            new Mesh();

        mesh.name = name;
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        AssetDatabase.CreateAsset(
            mesh,
            path
        );

        return mesh;
    }

    private static GameObject CreateMeshObject(
        string name,
        Transform parent,
        Mesh mesh,
        Material material,
        Vector3 position,
        Vector3 scale
    )
    {
        GameObject root =
            new GameObject(name);

        root.transform.SetParent(parent);
        root.transform.position = position;
        root.transform.localScale = scale;

        MeshFilter filter =
            root.AddComponent<MeshFilter>();

        filter.sharedMesh =
            mesh;

        MeshRenderer renderer =
            root.AddComponent<MeshRenderer>();

        renderer.sharedMaterial =
            material;

        return root;
    }

    private static Material CreateTexturedMaterial(
        string name,
        string texturePath,
        Color tint,
        bool transparent
    )
    {
        EnsureFolder(MaterialFolder);

        string path =
            MaterialFolder + "/" + name + ".mat";

        Material existing =
            AssetDatabase.LoadAssetAtPath<Material>(
                path
            );

        if (existing != null)
            return existing;

        Shader shader =
            Shader.Find(
                "Universal Render Pipeline/Unlit"
            );

        if (shader == null)
            shader =
                Shader.Find("Unlit/Texture");

        if (shader == null)
            shader =
                Shader.Find("Standard");

        Material material =
            new Material(shader);

        material.name = name;

        Texture2D texture =
            AssetDatabase.LoadAssetAtPath<Texture2D>(
                texturePath
            );

        if (material.HasProperty("_BaseMap"))
            material.SetTexture("_BaseMap", texture);

        if (material.HasProperty("_MainTex"))
            material.SetTexture("_MainTex", texture);

        if (material.HasProperty("_BaseColor"))
            material.SetColor("_BaseColor", tint);

        if (material.HasProperty("_Color"))
            material.SetColor("_Color", tint);

        if (transparent)
            ConfigureTransparentMaterial(material);

        AssetDatabase.CreateAsset(
            material,
            path
        );

        return material;
    }

    private static Material CreateFlatUnlitMaterial(
        string name,
        Color color
    )
    {
        EnsureFolder(MaterialFolder);

        string path =
            MaterialFolder + "/" + name + ".mat";

        Material existing =
            AssetDatabase.LoadAssetAtPath<Material>(
                path
            );

        if (existing != null)
            return existing;

        Shader shader =
            Shader.Find(
                "Universal Render Pipeline/Unlit"
            );

        if (shader == null)
            shader =
                Shader.Find("Unlit/Color");

        if (shader == null)
            shader =
                Shader.Find("Standard");

        Material material =
            new Material(shader);

        if (material.HasProperty("_BaseColor"))
            material.SetColor("_BaseColor", color);

        if (material.HasProperty("_Color"))
            material.SetColor("_Color", color);

        AssetDatabase.CreateAsset(
            material,
            path
        );

        return material;
    }

    private static Material CreateEmissiveMaterial(
        string name,
        Color baseColor,
        Color emission
    )
    {
        EnsureFolder(MaterialFolder);

        string path =
            MaterialFolder + "/" + name + ".mat";

        Material existing =
            AssetDatabase.LoadAssetAtPath<Material>(
                path
            );

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

        if (material.HasProperty("_BaseColor"))
            material.SetColor("_BaseColor", baseColor);

        if (material.HasProperty("_Color"))
            material.SetColor("_Color", baseColor);

        if (material.HasProperty("_EmissionColor"))
        {
            material.EnableKeyword("_EMISSION");
            material.SetColor(
                "_EmissionColor",
                emission
            );
        }

        AssetDatabase.CreateAsset(
            material,
            path
        );

        return material;
    }

    private static Material CreateParticleMaterial(
        string name,
        Texture2D texture,
        Color color
    )
    {
        EnsureFolder(MaterialFolder);

        string path =
            MaterialFolder + "/" + name + ".mat";

        Material existing =
            AssetDatabase.LoadAssetAtPath<Material>(
                path
            );

        if (existing != null)
            return existing;

        Shader shader =
            Shader.Find(
                "Universal Render Pipeline/Particles/Unlit"
            );

        if (shader == null)
            shader =
                Shader.Find(
                    "Particles/Standard Unlit"
                );

        if (shader == null)
            shader =
                Shader.Find(
                    "Universal Render Pipeline/Unlit"
                );

        Material material =
            new Material(shader);

        if (material.HasProperty("_BaseMap"))
            material.SetTexture("_BaseMap", texture);

        if (material.HasProperty("_MainTex"))
            material.SetTexture("_MainTex", texture);

        if (material.HasProperty("_BaseColor"))
            material.SetColor("_BaseColor", color);

        if (material.HasProperty("_Color"))
            material.SetColor("_Color", color);

        ConfigureTransparentMaterial(
            material
        );

        AssetDatabase.CreateAsset(
            material,
            path
        );

        return material;
    }

    private static void ConfigureTransparentMaterial(
        Material material
    )
    {
        if (material.HasProperty("_Surface"))
            material.SetFloat("_Surface", 1f);

        if (material.HasProperty("_Blend"))
            material.SetFloat("_Blend", 0f);

        if (material.HasProperty("_ZWrite"))
            material.SetFloat("_ZWrite", 0f);

        if (material.HasProperty("_SrcBlend"))
        {
            material.SetFloat(
                "_SrcBlend",
                (float)BlendMode.SrcAlpha
            );
        }

        if (material.HasProperty("_DstBlend"))
        {
            material.SetFloat(
                "_DstBlend",
                (float)BlendMode.OneMinusSrcAlpha
            );
        }

        material.EnableKeyword(
            "_SURFACE_TYPE_TRANSPARENT"
        );

        material.renderQueue =
            (int)RenderQueue.Transparent;
    }

    private static void EnsureFolders()
    {
        EnsureFolder("Assets/_Game/Art");
        EnsureFolder("Assets/_Game/Art/Materials");
        EnsureFolder(MaterialFolder);
        EnsureFolder("Assets/_Game/Art/Meshes");
        EnsureFolder(MeshFolder);
        EnsureFolder(PostFolder);
    }

    private static void EnsureFolder(
        string path
    )
    {
        if (AssetDatabase.IsValidFolder(path))
            return;

        string parent =
            Path.GetDirectoryName(path)
                ?.Replace("\\", "/");

        string folder =
            Path.GetFileName(path);

        if (!string.IsNullOrEmpty(parent) &&
            !AssetDatabase.IsValidFolder(parent))
        {
            EnsureFolder(parent);
        }

        if (!string.IsNullOrEmpty(parent))
        {
            AssetDatabase.CreateFolder(
                parent,
                folder
            );
        }
    }
}

#endif
