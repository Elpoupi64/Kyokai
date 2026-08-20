#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public static class FoundryHeroAssetsPassBuilder
{
    public static void Apply(Transform levelRoot)
    {
        if (levelRoot == null)
            return;

        FoundryHeroAssetLibraryBuilder.HeroLibrary library =
            FoundryHeroAssetLibraryBuilder.BuildOrLoad();

        Transform old = levelRoot.Find("FOUNDRY_ART_V13");
        if (old != null)
            Object.DestroyImmediate(old.gameObject);

        SetupHeroPassAtmosphere();

        GameObject root = new GameObject("FOUNDRY_ART_V13");
        root.transform.SetParent(levelRoot);

        BuildPaintedBackgrounds(root.transform, library);
        BuildEntryZone(root.transform, library);
        BuildEtherZone(root.transform, library);
        BuildProductionZone(root.transform, library);
        BuildBossApproachZone(root.transform, library);
        BuildBossArena(levelRoot, library);

        GameObject marker = new GameObject("FoundryHeroAssets_v13");
        marker.transform.SetParent(root.transform);
    }

    private static void SetupHeroPassAtmosphere()
    {
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.Linear;
        RenderSettings.fogColor = new Color(0.08f, 0.06f, 0.08f);
        RenderSettings.fogStartDistance = 8f;
        RenderSettings.fogEndDistance = 65f;

        RenderSettings.ambientMode = AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.25f, 0.19f, 0.17f);
    }

    private static void BuildPaintedBackgrounds(Transform parent, FoundryHeroAssetLibraryBuilder.HeroLibrary library)
    {
        Camera camera = Camera.main != null ? Camera.main : Object.FindAnyObjectByType<Camera>();

        GameObject far = CreateParallaxLayer(parent, "HeroBackdrop_Far", camera, 0.05f, 0.01f);
        GameObject mid = CreateParallaxLayer(parent, "HeroBackdrop_Mid", camera, 0.14f, 0.03f);
        GameObject near = CreateParallaxLayer(parent, "HeroBackdrop_Near", camera, 0.24f, 0.05f);

        float[] warmX = { -74f, -54f, -34f, -14f, 6f, 26f };
        for (int i = 0; i < warmX.Length; i++)
        {
            GameObject panel = InstantiateModule(library.PaintedPanelWarm, far.transform,
                new Vector3(warmX[i], 12.0f + (i % 2), 30f), new Vector3(1.5f, 1.5f, 1f));
            if (panel != null)
                AddBackdropDrift(panel, new Vector3(0.3f, 0.14f, 0f), 0.45f, 0.25f);
        }

        float[] cyanX = { -66f, -44f, -20f, 2f, 22f };
        for (int i = 0; i < cyanX.Length; i++)
        {
            GameObject panel = InstantiateModule(library.PaintedPanelCyan, mid.transform,
                new Vector3(cyanX[i], 10.5f + (i % 3) * 0.4f, 24f), new Vector3(1.3f, 1.25f, 1f));
            if (panel != null)
                AddBackdropDrift(panel, new Vector3(0.22f, 0.12f, 0f), 0.38f, 0.34f);
        }

        float[] roofX = { -72f, -58f, -44f, -30f, -16f, -2f, 14f };
        for (int i = 0; i < roofX.Length; i++)
        {
            InstantiateModule(library.RooflineCluster, near.transform,
                new Vector3(roofX[i], 1.2f, 17f), new Vector3(1.2f + (i % 2) * 0.2f, 1.1f, 1f));
        }
    }

    private static void BuildEntryZone(Transform parent, FoundryHeroAssetLibraryBuilder.HeroLibrary library)
    {
        GameObject root = new GameObject("EntryZone_HeroAssets");
        root.transform.SetParent(parent);

        InstantiateModule(library.MeijiGate, root.transform, new Vector3(-71.5f, 0f, 1.6f), new Vector3(1.25f, 1.10f, 1f));
        InstantiateModule(library.TitanBoiler, root.transform, new Vector3(-61.0f, 0f, 1.6f), new Vector3(1.15f, 1.05f, 1f));
        InstantiateModule(library.ClothStrip, root.transform, new Vector3(-68.6f, 6.8f, -0.8f), Vector3.one);
        InstantiateModule(library.ClothStrip, root.transform, new Vector3(-65.9f, 6.1f, -0.8f), new Vector3(0.9f, 1.2f, 1f));
    }

    private static void BuildEtherZone(Transform parent, FoundryHeroAssetLibraryBuilder.HeroLibrary library)
    {
        GameObject root = new GameObject("EtherZone_HeroAssets");
        root.transform.SetParent(parent);

        InstantiateModule(library.KikaiShrine, root.transform, new Vector3(-55.2f, 0f, 1.5f), new Vector3(0.95f, 0.95f, 1f));
        InstantiateModule(library.PipeSpine, root.transform, new Vector3(-48.8f, 4.9f, 1.4f), new Vector3(1.2f, 1.05f, 1f));
        InstantiateModule(library.ClothStrip, root.transform, new Vector3(-50.4f, 6.4f, -0.7f), new Vector3(0.8f, 1.1f, 1f));
    }

    private static void BuildProductionZone(Transform parent, FoundryHeroAssetLibraryBuilder.HeroLibrary library)
    {
        GameObject root = new GameObject("ProductionZone_HeroAssets");
        root.transform.SetParent(parent);

        InstantiateModule(library.CraneAssembly, root.transform, new Vector3(-21.0f, 0f, 1.4f), new Vector3(0.95f, 0.95f, 1f));
        InstantiateModule(library.TitanBoiler, root.transform, new Vector3(-30.8f, 0f, 1.6f), new Vector3(0.85f, 0.85f, 1f));
        InstantiateModule(library.PipeSpine, root.transform, new Vector3(-8.5f, 5.3f, 1.4f), new Vector3(1.4f, 1f, 1f));
        InstantiateModule(library.MeijiGate, root.transform, new Vector3(2.8f, 0f, 1.4f), new Vector3(0.95f, 0.90f, 1f));
        InstantiateModule(library.ClothStrip, root.transform, new Vector3(-10.8f, 6.3f, -0.8f), Vector3.one);
    }

    private static void BuildBossApproachZone(Transform parent, FoundryHeroAssetLibraryBuilder.HeroLibrary library)
    {
        GameObject root = new GameObject("BossApproach_HeroAssets");
        root.transform.SetParent(parent);

        InstantiateModule(library.KikaiShrine, root.transform, new Vector3(8.4f, 0f, 1.5f), new Vector3(0.85f, 0.85f, 1f));
        InstantiateModule(library.CraneAssembly, root.transform, new Vector3(12.8f, 0f, 1.2f), new Vector3(0.72f, 0.72f, 1f));
    }

    private static void BuildBossArena(Transform levelRoot, FoundryHeroAssetLibraryBuilder.HeroLibrary library)
    {
        Transform arena = levelRoot.Find("MINI_BOSS_ARENA");
        if (arena == null)
            return;

        Transform old = arena.Find("BossArena_HeroAssets_v13");
        if (old != null)
            Object.DestroyImmediate(old.gameObject);

        GameObject root = new GameObject("BossArena_HeroAssets_v13");
        root.transform.SetParent(arena);

        InstantiateModule(library.MeijiGate, root.transform, new Vector3(0f, 0f, 1.5f), new Vector3(1.9f, 1.15f, 1f));
        InstantiateModule(library.KikaiShrine, root.transform, new Vector3(0f, 0f, 2.8f), new Vector3(0.92f, 0.92f, 1f));
        InstantiateModule(library.PipeSpine, root.transform, new Vector3(-9.4f, 4.9f, 1.5f), new Vector3(1.1f, 1.0f, 1f));
        InstantiateModule(library.PipeSpine, root.transform, new Vector3(9.4f, 4.9f, 1.5f), new Vector3(1.1f, 1.0f, 1f));
        InstantiateModule(library.ClothStrip, root.transform, new Vector3(-6.0f, 6.6f, -0.9f), new Vector3(1.2f, 1.3f, 1f));
        InstantiateModule(library.ClothStrip, root.transform, new Vector3(6.0f, 6.6f, -0.9f), new Vector3(1.2f, 1.3f, 1f));
    }

    private static GameObject CreateParallaxLayer(Transform parent, string name, Camera camera, float xFactor, float yFactor)
    {
        GameObject layer = new GameObject(name);
        layer.transform.SetParent(parent);

        ParallaxLayer parallax = layer.AddComponent<ParallaxLayer>();
        SerializedObject so = new SerializedObject(parallax);

        if (camera != null)
            so.FindProperty("targetCamera").objectReferenceValue = camera.transform;

        so.FindProperty("xFactor").floatValue = xFactor;
        so.FindProperty("yFactor").floatValue = yFactor;
        so.FindProperty("affectY").boolValue = true;
        so.ApplyModifiedPropertiesWithoutUndo();

        return layer;
    }

    private static GameObject InstantiateModule(GameObject prefab, Transform parent, Vector3 position, Vector3 scale)
    {
        if (prefab == null)
            return null;

        GameObject instance = PrefabUtility.InstantiatePrefab(prefab, parent) as GameObject;
        instance.transform.position = position;
        instance.transform.localScale = scale;
        return instance;
    }

    private static void AddBackdropDrift(GameObject target, Vector3 movement, float amplitude, float speed)
    {
        if (target == null)
            return;

        FoundryBackdropDrift drift = target.AddComponent<FoundryBackdropDrift>();
        SerializedObject so = new SerializedObject(drift);
        so.FindProperty("movement").vector3Value = movement;
        so.FindProperty("amplitude").floatValue = amplitude;
        so.FindProperty("speed").floatValue = speed;
        so.FindProperty("localSpace").boolValue = true;
        so.ApplyModifiedPropertiesWithoutUndo();
    }
}

#endif
