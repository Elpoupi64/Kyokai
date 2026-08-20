#if UNITY_EDITOR

using System.IO;
using UnityEditor;
using UnityEngine;

public static class FoundryArtModuleLibraryBuilder
{
    private const string RootFolder =
        "Assets/_Game/Prefabs/Environment/FoundryModules";

    private const string MaterialFolder =
        "Assets/_Game/Art/Materials/FoundrySteampunk";

    public struct ModuleLibrary
    {
        public GameObject WallFacadeA;
        public GameObject WallFacadeB;
        public GameObject ArchGate;
        public GameObject BoilerSmall;
        public GameObject BoilerLarge;
        public GameObject FurnaceMachine;
        public GameObject Conveyor;
        public GameObject PipeRack;
        public GameObject GearAssemblySmall;
        public GameObject GearAssemblyLarge;
        public GameObject GasLamp;
        public GameObject SteamVent;
        public GameObject EtherNode;
        public GameObject RailSection;
        public GameObject ChainHang;
        public GameObject BackgroundFactoryA;
        public GameObject BackgroundFactoryB;
    }

    public static ModuleLibrary BuildOrLoad()
    {
        EnsureFolder("Assets/_Game");
        EnsureFolder("Assets/_Game/Prefabs");
        EnsureFolder("Assets/_Game/Prefabs/Environment");
        EnsureFolder(RootFolder);

        ModuleLibrary library = new ModuleLibrary();

        library.WallFacadeA = BuildWallFacade("WallFacade_A", 8f, 5.3f, true);
        library.WallFacadeB = BuildWallFacade("WallFacade_B", 11f, 6.4f, false);
        library.ArchGate = BuildArchGate("ArchGate_A", 4.6f, 5.2f);
        library.BoilerSmall = BuildBoiler("Boiler_Small", 1.0f);
        library.BoilerLarge = BuildBoiler("Boiler_Large", 1.4f);
        library.FurnaceMachine = BuildFurnaceMachine("Furnace_Machine");
        library.Conveyor = BuildConveyor("Conveyor_Line", 6f);
        library.PipeRack = BuildPipeRack("PipeRack_Modular");
        library.GearAssemblySmall = BuildGearAssembly("GearAssembly_Small", 0.8f, 75f);
        library.GearAssemblyLarge = BuildGearAssembly("GearAssembly_Large", 1.2f, -95f);
        library.GasLamp = BuildGasLamp("GasLamp_Modular");
        library.SteamVent = BuildSteamVent("SteamVent_Modular");
        library.EtherNode = BuildEtherNode("EtherNode_Modular");
        library.RailSection = BuildRailSection("RailSection_Modular", 4f);
        library.ChainHang = BuildChainHang("ChainHang_Modular", 6);
        library.BackgroundFactoryA = BuildBackgroundFactory("BackgroundFactory_A", 10f);
        library.BackgroundFactoryB = BuildBackgroundFactory("BackgroundFactory_B", 14f);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        return library;
    }

    private static GameObject BuildWallFacade(string prefabName, float width, float height, bool addWindowRow)
    {
        string path = RootFolder + "/" + prefabName + ".prefab";
        GameObject existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (existing != null)
            return existing;

        Material wall = LoadMat("WeatheredSteel");
        Material trim = LoadMat("FrontTrim");
        Material brass = LoadMat("Brass");

        GameObject root = new GameObject(prefabName);

        CreatePrimitive(PrimitiveType.Cube, "WallBody", root.transform,
            new Vector3(0f, height * 0.5f - 0.4f, 0f),
            new Vector3(width, height, 0.5f), wall, false);

        CreatePrimitive(PrimitiveType.Cube, "BaseTrim", root.transform,
            new Vector3(0f, 0.15f, -0.3f),
            new Vector3(width, 0.3f, 0.8f), trim, false);

        for (int i = -2; i <= 2; i++)
        {
            CreatePrimitive(PrimitiveType.Cube, "Rib_" + i, root.transform,
                new Vector3(i * (width / 5f), height * 0.55f, -0.1f),
                new Vector3(0.22f, height * 0.85f, 0.65f), trim, false);
        }

        if (addWindowRow)
        {
            for (int i = -2; i <= 2; i++)
            {
                CreatePrimitive(PrimitiveType.Cube, "Window_" + i, root.transform,
                    new Vector3(i * 1.4f, 2.2f, -0.35f),
                    new Vector3(0.72f, 0.55f, 0.05f), brass, false);
            }
        }

        return SavePrefab(root, path);
    }

    private static GameObject BuildArchGate(string prefabName, float width, float height)
    {
        string path = RootFolder + "/" + prefabName + ".prefab";
        GameObject existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (existing != null)
            return existing;

        Material trim = LoadMat("FrontTrim");
        Material metal = LoadMat("CastIron");

        GameObject root = new GameObject(prefabName);

        CreatePrimitive(PrimitiveType.Cube, "LeftPillar", root.transform,
            new Vector3(-width * 0.5f, height * 0.5f, 0f),
            new Vector3(0.42f, height, 0.8f), trim, false);

        CreatePrimitive(PrimitiveType.Cube, "RightPillar", root.transform,
            new Vector3(width * 0.5f, height * 0.5f, 0f),
            new Vector3(0.42f, height, 0.8f), trim, false);

        CreatePrimitive(PrimitiveType.Cube, "TopBeam", root.transform,
            new Vector3(0f, height, 0f),
            new Vector3(width + 0.7f, 0.42f, 0.8f), trim, false);

        CreatePrimitive(PrimitiveType.Cube, "InnerLintel", root.transform,
            new Vector3(0f, height * 0.76f, 0.24f),
            new Vector3(width - 0.3f, 0.16f, 0.4f), metal, false);

        return SavePrefab(root, path);
    }

    private static GameObject BuildBoiler(string prefabName, float scale)
    {
        string path = RootFolder + "/" + prefabName + ".prefab";
        GameObject existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (existing != null)
            return existing;

        Material body = LoadMat("CastIron");
        Material copper = LoadMat("Copper");
        Material glow = LoadMat("FurnaceGlow");

        GameObject root = new GameObject(prefabName);

        CreatePrimitive(PrimitiveType.Cylinder, "Body", root.transform,
            new Vector3(0f, 1.3f * scale, 0f),
            new Vector3(1.4f * scale, 1.9f * scale, 1.4f * scale), body, false);

        CreatePrimitive(PrimitiveType.Cube, "Base", root.transform,
            new Vector3(0f, 0.18f * scale, 0f),
            new Vector3(2.1f * scale, 0.35f * scale, 1.8f * scale), body, false);

        CreatePrimitive(PrimitiveType.Cylinder, "Door", root.transform,
            new Vector3(0f, 1.2f * scale, -1.35f * scale),
            new Vector3(0.55f * scale, 0.12f * scale, 0.55f * scale), glow, false);

        CreatePrimitive(PrimitiveType.Cylinder, "Pipe_Vertical", root.transform,
            new Vector3(1.22f * scale, 2.6f * scale, 0f),
            new Vector3(0.18f * scale, 1.5f * scale, 0.18f * scale), copper, false);

        GameObject pipeH = CreatePrimitive(PrimitiveType.Cylinder, "Pipe_Horizontal", root.transform,
            new Vector3(2.35f * scale, 3.45f * scale, 0f),
            new Vector3(0.18f * scale, 1.1f * scale, 0.18f * scale), copper, false);
        pipeH.transform.rotation = Quaternion.Euler(0f, 0f, 90f);

        AddPulse(root, new Color(1.0f, 0.35f, 0.08f));
        return SavePrefab(root, path);
    }

    private static GameObject BuildFurnaceMachine(string prefabName)
    {
        string path = RootFolder + "/" + prefabName + ".prefab";
        GameObject existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (existing != null)
            return existing;

        Material frame = LoadMat("WeatheredSteel");
        Material brass = LoadMat("Brass");
        Material glow = LoadMat("FurnaceGlow");

        GameObject root = new GameObject(prefabName);

        CreatePrimitive(PrimitiveType.Cube, "Base", root.transform,
            new Vector3(0f, 0.5f, 0f),
            new Vector3(4.8f, 1f, 2.2f), frame, false);

        CreatePrimitive(PrimitiveType.Cube, "Head", root.transform,
            new Vector3(0f, 2.1f, 0f),
            new Vector3(3.4f, 1.25f, 2f), frame, false);

        CreatePrimitive(PrimitiveType.Cube, "Window", root.transform,
            new Vector3(0.65f, 2.1f, -1.06f),
            new Vector3(1.2f, 0.6f, 0.06f), glow, false);

        GameObject gearA = CreateGearChild(root.transform, new Vector3(-2.4f, 1.35f, 0f), 1.05f, brass);
        GameObject gearB = CreateGearChild(root.transform, new Vector3(2.5f, 1.0f, 0.05f), 0.72f, brass);

        AddRotatingGear(gearA, 82f);
        AddRotatingGear(gearB, -124f);
        AddPulse(root, new Color(1.0f, 0.35f, 0.08f));

        return SavePrefab(root, path);
    }

    private static GameObject BuildConveyor(string prefabName, float length)
    {
        string path = RootFolder + "/" + prefabName + ".prefab";
        GameObject existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (existing != null)
            return existing;

        Material body = LoadMat("WeatheredSteel");
        Material trim = LoadMat("Brass");
        GameObject root = new GameObject(prefabName);

        CreatePrimitive(PrimitiveType.Cube, "Belt", root.transform,
            new Vector3(length * 0.5f, 0.45f, 0f),
            new Vector3(length, 0.18f, 1.8f), body, false);

        for (int i = 0; i <= 3; i++)
        {
            GameObject roll = CreatePrimitive(PrimitiveType.Cylinder, "Roller_" + i, root.transform,
                new Vector3(i * (length / 3f), 0.45f, 0f),
                new Vector3(0.18f, 0.95f, 0.18f), trim, false);
            roll.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        }

        return SavePrefab(root, path);
    }

    private static GameObject BuildPipeRack(string prefabName)
    {
        string path = RootFolder + "/" + prefabName + ".prefab";
        GameObject existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (existing != null)
            return existing;

        Material pipe = LoadMat("Copper");
        Material trim = LoadMat("Brass");
        GameObject root = new GameObject(prefabName);

        for (int row = 0; row < 3; row++)
        {
            GameObject cyl = CreatePrimitive(PrimitiveType.Cylinder, "PipeRow_" + row, root.transform,
                new Vector3(0f, 0.4f + row * 0.55f, 0f),
                new Vector3(0.14f, 3f, 0.14f), pipe, false);
            cyl.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        }

        for (int post = -1; post <= 1; post++)
        {
            CreatePrimitive(PrimitiveType.Cube, "Post_" + post, root.transform,
                new Vector3(post * 2.4f, 0.78f, 0f),
                new Vector3(0.16f, 1.7f, 0.16f), trim, false);
        }

        return SavePrefab(root, path);
    }

    private static GameObject BuildGearAssembly(string prefabName, float scale, float speed)
    {
        string path = RootFolder + "/" + prefabName + ".prefab";
        GameObject existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (existing != null)
            return existing;

        Material brass = LoadMat("Brass");
        GameObject root = new GameObject(prefabName);
        CreateGearChild(root.transform, Vector3.zero, scale, brass);
        AddRotatingGear(root, speed);
        return SavePrefab(root, path);
    }

    private static GameObject BuildGasLamp(string prefabName)
    {
        string path = RootFolder + "/" + prefabName + ".prefab";
        GameObject existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (existing != null)
            return existing;

        Material brass = LoadMat("Brass");
        GameObject root = new GameObject(prefabName);

        CreatePrimitive(PrimitiveType.Cylinder, "Pole", root.transform,
            new Vector3(0f, 1.55f, 0f),
            new Vector3(0.08f, 1.55f, 0.08f), brass, false);

        CreatePrimitive(PrimitiveType.Sphere, "Lantern", root.transform,
            new Vector3(0f, 3.1f, 0f),
            new Vector3(0.28f, 0.28f, 0.28f), brass, false);

        GameObject lightObject = new GameObject("LampLight");
        lightObject.transform.SetParent(root.transform);
        lightObject.transform.localPosition = new Vector3(0f, 3.1f, 0.08f);

        Light light = lightObject.AddComponent<Light>();
        light.type = LightType.Point;
        light.range = 7f;
        light.intensity = 4f;
        light.color = new Color(1f, 0.62f, 0.22f);

        FoundryLightFlicker flicker = lightObject.AddComponent<FoundryLightFlicker>();
        SerializedObject so = new SerializedObject(flicker);
        so.FindProperty("targetLight").objectReferenceValue = light;
        so.FindProperty("minIntensity").floatValue = 2.8f;
        so.FindProperty("maxIntensity").floatValue = 4.4f;
        so.FindProperty("speed").floatValue = 8.2f;
        so.ApplyModifiedPropertiesWithoutUndo();

        return SavePrefab(root, path);
    }

    private static GameObject BuildSteamVent(string prefabName)
    {
        string path = RootFolder + "/" + prefabName + ".prefab";
        GameObject existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (existing != null)
            return existing;

        Material steam = LoadMat("Steam");
        GameObject root = new GameObject(prefabName);

        CreatePrimitive(PrimitiveType.Cylinder, "VentPipe", root.transform,
            new Vector3(0f, 0.2f, 0f),
            new Vector3(0.10f, 0.3f, 0.10f), steam, false);

        ParticleSystem ps = root.AddComponent<ParticleSystem>();
        ConfigureSteamParticles(ps, steam);

        return SavePrefab(root, path);
    }

    private static GameObject BuildEtherNode(string prefabName)
    {
        string path = RootFolder + "/" + prefabName + ".prefab";
        GameObject existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (existing != null)
            return existing;

        Material ether = LoadMat("EtherGlow");
        Material brass = LoadMat("Brass");
        GameObject root = new GameObject(prefabName);

        CreatePrimitive(PrimitiveType.Cylinder, "Base", root.transform,
            new Vector3(0f, 0.2f, 0f),
            new Vector3(0.42f, 0.3f, 0.42f), brass, false);

        CreatePrimitive(PrimitiveType.Sphere, "Core", root.transform,
            new Vector3(0f, 0.85f, 0f),
            new Vector3(0.52f, 0.52f, 0.52f), ether, false);

        GameObject lightObject = new GameObject("EtherLight");
        lightObject.transform.SetParent(root.transform);
        lightObject.transform.localPosition = new Vector3(0f, 0.8f, 0.2f);

        Light light = lightObject.AddComponent<Light>();
        light.type = LightType.Point;
        light.range = 5f;
        light.intensity = 3.2f;
        light.color = new Color(0.10f, 1f, 1f);

        FoundryLightFlicker flicker = lightObject.AddComponent<FoundryLightFlicker>();
        SerializedObject so = new SerializedObject(flicker);
        so.FindProperty("targetLight").objectReferenceValue = light;
        so.FindProperty("minIntensity").floatValue = 2.2f;
        so.FindProperty("maxIntensity").floatValue = 3.8f;
        so.FindProperty("speed").floatValue = 5.2f;
        so.ApplyModifiedPropertiesWithoutUndo();

        AddPulse(root, new Color(0.10f, 1f, 1f));
        return SavePrefab(root, path);
    }

    private static GameObject BuildRailSection(string prefabName, float width)
    {
        string path = RootFolder + "/" + prefabName + ".prefab";
        GameObject existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (existing != null)
            return existing;

        Material trim = LoadMat("FrontTrim");
        GameObject root = new GameObject(prefabName);

        CreatePrimitive(PrimitiveType.Cube, "TopRail", root.transform,
            new Vector3(0f, 1f, 0f),
            new Vector3(width, 0.10f, 0.10f), trim, false);

        for (int i = -2; i <= 2; i++)
        {
            CreatePrimitive(PrimitiveType.Cube, "Post_" + i, root.transform,
                new Vector3(i * (width / 4f), 0.45f, 0f),
                new Vector3(0.08f, 0.90f, 0.08f), trim, false);
        }

        return SavePrefab(root, path);
    }

    private static GameObject BuildChainHang(string prefabName, int links)
    {
        string path = RootFolder + "/" + prefabName + ".prefab";
        GameObject existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (existing != null)
            return existing;

        Material iron = LoadMat("CastIron");
        GameObject root = new GameObject(prefabName);

        for (int i = 0; i < links; i++)
        {
            CreatePrimitive(PrimitiveType.Cylinder, "Link_" + i, root.transform,
                new Vector3(0f, -i * 0.52f, 0f),
                new Vector3(0.10f, 0.25f, 0.10f), iron, false);
        }

        CreatePrimitive(PrimitiveType.Cube, "Hook", root.transform,
            new Vector3(0f, -(links * 0.52f) - 0.25f, 0f),
            new Vector3(0.22f, 0.42f, 0.10f), iron, false);

        return SavePrefab(root, path);
    }

    private static GameObject BuildBackgroundFactory(string prefabName, float height)
    {
        string path = RootFolder + "/" + prefabName + ".prefab";
        GameObject existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (existing != null)
            return existing;

        Material body = LoadMat(height > 12f ? "MidSilhouette" : "Silhouette");
        Material pipe = LoadMat("Copper");
        GameObject root = new GameObject(prefabName);

        CreatePrimitive(PrimitiveType.Cube, "Body", root.transform,
            new Vector3(0f, height * 0.42f, 0f),
            new Vector3(8f, height, 3f), body, false);

        CreatePrimitive(PrimitiveType.Cylinder, "StackA", root.transform,
            new Vector3(-2f, height * 0.88f, 1f),
            new Vector3(0.55f, height * 0.28f, 0.55f), body, false);

        CreatePrimitive(PrimitiveType.Cylinder, "StackB", root.transform,
            new Vector3(2f, height * 0.75f, -0.4f),
            new Vector3(0.40f, height * 0.20f, 0.40f), pipe, false);

        return SavePrefab(root, path);
    }

    private static GameObject CreatePrimitive(PrimitiveType type, string name, Transform parent, Vector3 localPosition, Vector3 localScale, Material material, bool keepCollider)
    {
        GameObject go = GameObject.CreatePrimitive(type);
        go.name = name;
        go.transform.SetParent(parent);
        go.transform.localPosition = localPosition;
        go.transform.localScale = localScale;

        Renderer renderer = go.GetComponent<Renderer>();
        if (renderer != null)
            renderer.sharedMaterial = material;

        if (!keepCollider)
        {
            Collider collider = go.GetComponent<Collider>();
            if (collider != null)
                Object.DestroyImmediate(collider);
        }

        return go;
    }

    private static void ConfigureSteamParticles(ParticleSystem ps, Material material)
    {
        var main = ps.main;
        main.loop = true;
        main.duration = 1.6f;
        main.startLifetime = 2.2f;
        main.startSpeed = 2.4f;
        main.startSize = 0.6f;
        main.startColor = new Color(0.82f, 0.84f, 0.86f, 0.35f);
        main.maxParticles = 180;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.gravityModifier = -0.05f;

        var emission = ps.emission;
        emission.rateOverTime = 14f;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 12f;
        shape.radius = 0.08f;

        var sizeOverLifetime = ps.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        AnimationCurve sizeCurve = new AnimationCurve(
            new Keyframe(0f, 0.2f),
            new Keyframe(0.4f, 0.65f),
            new Keyframe(1f, 1.45f)
        );
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, sizeCurve);

        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new[]
            {
                new GradientColorKey(new Color(0.80f, 0.83f, 0.86f), 0f),
                new GradientColorKey(new Color(0.58f, 0.62f, 0.67f), 1f)
            },
            new[]
            {
                new GradientAlphaKey(0.0f, 0f),
                new GradientAlphaKey(0.28f, 0.12f),
                new GradientAlphaKey(0.18f, 0.55f),
                new GradientAlphaKey(0.0f, 1f)
            }
        );
        colorOverLifetime.color = new ParticleSystem.MinMaxGradient(gradient);

        var velocity = ps.velocityOverLifetime;
        velocity.enabled = true;
        velocity.x = new ParticleSystem.MinMaxCurve(-0.18f, 0.18f);
        velocity.y = new ParticleSystem.MinMaxCurve(0.3f, 1.2f);
        velocity.z = new ParticleSystem.MinMaxCurve(0f, 0f);

        var noise = ps.noise;
        noise.enabled = true;
        noise.strength = 0.35f;
        noise.frequency = 0.4f;
        noise.scrollSpeed = 0.25f;

        ParticleSystemRenderer renderer = ps.GetComponent<ParticleSystemRenderer>();
        renderer.renderMode = ParticleSystemRenderMode.Billboard;
        renderer.sharedMaterial = material;
    }

    private static GameObject CreateGearChild(Transform parent, Vector3 localPosition, float scale, Material material)
    {
        GameObject gear = new GameObject("Gear");
        gear.transform.SetParent(parent);
        gear.transform.localPosition = localPosition;

        CreatePrimitive(PrimitiveType.Cylinder, "Hub", gear.transform,
            Vector3.zero, new Vector3(scale, 0.12f, scale), material, false);

        for (int i = 0; i < 8; i++)
        {
            float angle = i * 45f;
            Vector3 offset =
                Quaternion.Euler(0f, 0f, angle) *
                Vector3.right * (scale * 0.72f);

            GameObject tooth = CreatePrimitive(PrimitiveType.Cube, "Tooth_" + i, gear.transform,
                offset, new Vector3(0.24f * scale, 0.48f * scale, 0.12f), material, false);
            tooth.transform.localRotation = Quaternion.Euler(0f, 0f, angle);
        }

        return gear;
    }

    private static void AddRotatingGear(GameObject root, float speed)
    {
        RotatingGear rotator = root.AddComponent<RotatingGear>();
        SerializedObject so = new SerializedObject(rotator);
        so.FindProperty("localAxis").vector3Value = Vector3.forward;
        so.FindProperty("speed").floatValue = speed;
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void AddPulse(GameObject root, Color emission)
    {
        FoundryModulePulse pulse = root.AddComponent<FoundryModulePulse>();
        SerializedObject so = new SerializedObject(pulse);
        so.FindProperty("emissionColor").colorValue = emission;
        so.FindProperty("minIntensity").floatValue = 0.7f;
        so.FindProperty("maxIntensity").floatValue = 1.35f;
        so.FindProperty("speed").floatValue = 2.8f;
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    private static GameObject SavePrefab(GameObject root, string path)
    {
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        Object.DestroyImmediate(root);
        return prefab;
    }

    private static Material LoadMat(string name)
    {
        string path = MaterialFolder + "/" + name + ".mat";
        return AssetDatabase.LoadAssetAtPath<Material>(path);
    }

    private static void EnsureFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path))
            return;

        string parent = Path.GetDirectoryName(path)?.Replace("\\", "/");
        string folder = Path.GetFileName(path);

        if (!string.IsNullOrEmpty(parent) && !AssetDatabase.IsValidFolder(parent))
            EnsureFolder(parent);

        if (!string.IsNullOrEmpty(parent))
            AssetDatabase.CreateFolder(parent, folder);
    }
}

#endif
