#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

public static class FoundryModularArtPassBuilder
{
    public static void Apply(Transform levelRoot)
    {
        if (levelRoot == null)
            return;

        FoundryArtModuleLibraryBuilder.ModuleLibrary library =
            FoundryArtModuleLibraryBuilder.BuildOrLoad();

        Transform oldRoot = levelRoot.Find("FOUNDRY_ART_V10");
        if (oldRoot != null)
            Object.DestroyImmediate(oldRoot.gameObject);

        Transform oldV11 = levelRoot.Find("FOUNDRY_ART_V11");
        if (oldV11 != null)
            Object.DestroyImmediate(oldV11.gameObject);

        GameObject artRoot = new GameObject("FOUNDRY_ART_V11");
        artRoot.transform.SetParent(levelRoot);

        Transform slice = levelRoot.Find("VERTICAL_SLICE_V9");
        if (slice != null)
        {
            Transform grey = slice.Find("Industrial_Background_Greybox");
            if (grey != null)
                grey.gameObject.SetActive(false);
        }

        BuildParallaxFactories(artRoot.transform, library);
        BuildEnvironmentModules(artRoot.transform, library);
        BuildBossArenaModules(levelRoot, library);

        GameObject marker = new GameObject("FoundryModularArt_v11");
        marker.transform.SetParent(artRoot.transform);
    }

    private static void BuildParallaxFactories(Transform root, FoundryArtModuleLibraryBuilder.ModuleLibrary library)
    {
        Camera camera = Camera.main != null ? Camera.main : Object.FindAnyObjectByType<Camera>();

        GameObject far = CreateLayer(root, "Parallax_Far_Modular", camera, 0.10f, 0.02f);
        GameObject mid = CreateLayer(root, "Parallax_Mid_Modular", camera, 0.24f, 0.04f);
        GameObject near = CreateLayer(root, "Parallax_Near_Modular", camera, 0.40f, 0.06f);

        float[] farX = { -74f, -52f, -28f, -6f, 18f };
        for (int i = 0; i < farX.Length; i++)
        {
            InstantiateModule(i % 2 == 0 ? library.BackgroundFactoryA : library.BackgroundFactoryB,
                far.transform, new Vector3(farX[i], 0f, 26f), Vector3.one);
        }

        float[] midX = { -70f, -58f, -44f, -30f, -16f, -2f, 12f };
        for (int i = 0; i < midX.Length; i++)
        {
            InstantiateModule(i % 2 == 0 ? library.BackgroundFactoryB : library.BackgroundFactoryA,
                mid.transform, new Vector3(midX[i], 0f, 20f), Vector3.one * 0.9f);
        }

        float[] nearPipeX = { -62f, -18f };
        foreach (float x in nearPipeX)
        {
            InstantiateModule(library.PipeRack, near.transform, new Vector3(x, 4.4f, 15f), new Vector3(2.8f, 1.1f, 1f));
            InstantiateModule(library.SteamVent, near.transform, new Vector3(x - 2.6f, 4.0f, 15.2f), Vector3.one);
        }
    }

    private static void BuildEnvironmentModules(Transform root, FoundryArtModuleLibraryBuilder.ModuleLibrary library)
    {
        GameObject env = new GameObject("Environment_Modules");
        env.transform.SetParent(root);

        InstantiateModule(library.WallFacadeB, env.transform, new Vector3(-66f, 0f, 3.2f), Vector3.one);
        InstantiateModule(library.ArchGate, env.transform, new Vector3(-72f, 0f, 2.2f), Vector3.one);
        InstantiateModule(library.BoilerLarge, env.transform, new Vector3(-61f, 0f, 2.3f), Vector3.one);
        InstantiateModule(library.GasLamp, env.transform, new Vector3(-71f, 0f, -1.8f), Vector3.one);
        InstantiateModule(library.GasLamp, env.transform, new Vector3(-63f, 0f, -1.8f), Vector3.one);
        InstantiateModule(library.RailSection, env.transform, new Vector3(-66f, 0f, -2.6f), new Vector3(4.5f, 1f, 1f));
        InstantiateModule(library.ChainHang, env.transform, new Vector3(-69f, 7.1f, -3.1f), Vector3.one);
        InstantiateModule(library.ChainHang, env.transform, new Vector3(-64f, 7.4f, -3.1f), Vector3.one);

        InstantiateModule(library.WallFacadeA, env.transform, new Vector3(-50f, 0f, 3.2f), Vector3.one);
        InstantiateModule(library.PipeRack, env.transform, new Vector3(-48.5f, 4.8f, 2.8f), new Vector3(2.2f, 1f, 1f));
        InstantiateModule(library.EtherNode, env.transform, new Vector3(-55.6f, 0f, -0.9f), Vector3.one);
        InstantiateModule(library.GasLamp, env.transform, new Vector3(-55f, 0f, -1.8f), Vector3.one);
        InstantiateModule(library.SteamVent, env.transform, new Vector3(-53.8f, 0f, 2.3f), Vector3.one);

        InstantiateModule(library.WallFacadeA, env.transform, new Vector3(-38f, 0f, 3.2f), new Vector3(1.3f, 1f, 1f));
        InstantiateModule(library.BoilerSmall, env.transform, new Vector3(-31.2f, 0f, 2.2f), Vector3.one);
        InstantiateModule(library.GearAssemblyLarge, env.transform, new Vector3(-42.4f, 1.7f, 2.0f), Vector3.one);
        InstantiateModule(library.GasLamp, env.transform, new Vector3(-41f, 0f, -1.8f), Vector3.one);
        InstantiateModule(library.SteamVent, env.transform, new Vector3(-34f, 0f, 2.2f), Vector3.one);

        InstantiateModule(library.WallFacadeB, env.transform, new Vector3(-15f, 0f, 3.2f), new Vector3(1.8f, 1f, 1f));
        InstantiateModule(library.FurnaceMachine, env.transform, new Vector3(4.5f, 0f, 2.2f), Vector3.one);
        InstantiateModule(library.Conveyor, env.transform, new Vector3(-8f, 0f, 2.15f), new Vector3(2f, 1f, 1f));
        InstantiateModule(library.PipeRack, env.transform, new Vector3(-12f, 5.3f, 2.6f), new Vector3(3.2f, 1f, 1f));
        InstantiateModule(library.GasLamp, env.transform, new Vector3(-25f, 0f, -1.8f), Vector3.one);
        InstantiateModule(library.GasLamp, env.transform, new Vector3(-17f, 0f, -1.8f), Vector3.one);
        InstantiateModule(library.GasLamp, env.transform, new Vector3(-8f, 0f, -1.8f), Vector3.one);
        InstantiateModule(library.GasLamp, env.transform, new Vector3(0f, 0f, -1.8f), Vector3.one);
        InstantiateModule(library.RailSection, env.transform, new Vector3(-10f, 0f, -2.6f), new Vector3(7.5f, 1f, 1f));
        InstantiateModule(library.ChainHang, env.transform, new Vector3(-21f, 7.2f, -3.1f), Vector3.one);
        InstantiateModule(library.ChainHang, env.transform, new Vector3(-10f, 7.1f, -3.1f), Vector3.one);
        InstantiateModule(library.SteamVent, env.transform, new Vector3(0f, 0f, 2.2f), Vector3.one);

        InstantiateModule(library.WallFacadeA, env.transform, new Vector3(8f, 0f, 3.2f), new Vector3(1.5f, 1f, 1f));
        InstantiateModule(library.GasLamp, env.transform, new Vector3(9f, 0f, -1.8f), Vector3.one);
        InstantiateModule(library.GearAssemblySmall, env.transform, new Vector3(12f, 1.6f, 2.0f), Vector3.one);
    }

    private static void BuildBossArenaModules(Transform levelRoot, FoundryArtModuleLibraryBuilder.ModuleLibrary library)
    {
        Transform arena = levelRoot.Find("MINI_BOSS_ARENA");
        if (arena == null)
            return;

        Transform old = arena.Find("BossArena_FoundryDress");
        if (old != null)
            Object.DestroyImmediate(old.gameObject);

        Transform oldV11 = arena.Find("BossArena_ModularDress");
        if (oldV11 != null)
            Object.DestroyImmediate(oldV11.gameObject);

        GameObject root = new GameObject("BossArena_ModularDress");
        root.transform.SetParent(arena);

        InstantiateModule(library.WallFacadeB, root.transform, new Vector3(0f, 0f, 4.0f), new Vector3(2.1f, 1.35f, 1f));
        InstantiateModule(library.ArchGate, root.transform, new Vector3(0f, 1.1f, 2.3f), new Vector3(1.2f, 1.1f, 1f));
        InstantiateModule(library.GearAssemblyLarge, root.transform, new Vector3(-8f, 2.0f, 2.0f), Vector3.one);
        InstantiateModule(library.GearAssemblyLarge, root.transform, new Vector3(8f, 2.0f, 2.0f), Vector3.one);
        InstantiateModule(library.EtherNode, root.transform, new Vector3(0f, 3.35f, 1.75f), new Vector3(1.3f, 1.3f, 1.3f));

        for (int i = -3; i <= 3; i++)
        {
            float x = i * 3.1f;
            InstantiateModule(library.GasLamp, root.transform, new Vector3(x, 0f, -1.6f), Vector3.one);
            InstantiateModule(library.SteamVent, root.transform, new Vector3(x, 0f, 2.35f), Vector3.one);
        }
    }

    private static GameObject CreateLayer(Transform parent, string name, Camera camera, float xFactor, float yFactor)
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
}

#endif
