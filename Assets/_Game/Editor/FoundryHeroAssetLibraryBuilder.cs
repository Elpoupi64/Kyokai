#if UNITY_EDITOR

using System.IO;
using UnityEditor;
using UnityEngine;

public static class FoundryHeroAssetLibraryBuilder
{
    private const string RootFolder =
        "Assets/_Game/Prefabs/Environment/HeroAssets";

    private const string MaterialFolder =
        "Assets/_Game/Art/Materials/FoundryHeroPass";

    public struct HeroLibrary
    {
        public GameObject MeijiGate;
        public GameObject TitanBoiler;
        public GameObject KikaiShrine;
        public GameObject CraneAssembly;
        public GameObject PipeSpine;
        public GameObject PaintedPanelWarm;
        public GameObject PaintedPanelCyan;
        public GameObject RooflineCluster;
        public GameObject ClothStrip;
    }

    public static HeroLibrary BuildOrLoad()
    {
        EnsureFolder("Assets/_Game");
        EnsureFolder("Assets/_Game/Prefabs");
        EnsureFolder("Assets/_Game/Prefabs/Environment");
        EnsureFolder(RootFolder);

        HeroLibrary library = new HeroLibrary();

        library.MeijiGate = BuildMeijiGate("Hero_MeijiGate");
        library.TitanBoiler = BuildTitanBoiler("Hero_TitanBoiler");
        library.KikaiShrine = BuildKikaiShrine("Hero_KikaiShrine");
        library.CraneAssembly = BuildCraneAssembly("Hero_CraneAssembly");
        library.PipeSpine = BuildPipeSpine("Hero_PipeSpine");
        library.PaintedPanelWarm = BuildPaintedPanel("Hero_PaintedPanelWarm", new Color(0.63f, 0.22f, 0.13f));
        library.PaintedPanelCyan = BuildPaintedPanel("Hero_PaintedPanelCyan", new Color(0.10f, 0.37f, 0.43f));
        library.RooflineCluster = BuildRooflineCluster("Hero_RooflineCluster");
        library.ClothStrip = BuildClothStrip("Hero_ClothStrip");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        return library;
    }

    private static GameObject BuildMeijiGate(string prefabName)
    {
        string path = RootFolder + "/" + prefabName + ".prefab";
        GameObject existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (existing != null) return existing;

        Material body = GetOrCreateMaterial("Hero_BodyIron", new Color(0.18f, 0.14f, 0.11f), false, Color.black);
        Material accent = GetOrCreateMaterial("Hero_PaintedCrimson", new Color(0.48f, 0.08f, 0.10f), false, Color.black);
        Material brass = GetOrCreateMaterial("Hero_Brass", new Color(0.59f, 0.43f, 0.13f), false, Color.black);

        GameObject root = new GameObject(prefabName);

        CreatePrimitive(PrimitiveType.Cube, "LeftPillar", root.transform,
            new Vector3(-2.2f, 2.7f, 0f), new Vector3(0.7f, 5.4f, 1.0f), body, false);

        CreatePrimitive(PrimitiveType.Cube, "RightPillar", root.transform,
            new Vector3(2.2f, 2.7f, 0f), new Vector3(0.7f, 5.4f, 1.0f), body, false);

        CreatePrimitive(PrimitiveType.Cube, "TopBeam", root.transform,
            new Vector3(0f, 5.4f, 0f), new Vector3(5.5f, 0.55f, 1.1f), accent, false);

        CreatePrimitive(PrimitiveType.Cube, "RoofCap", root.transform,
            new Vector3(0f, 5.9f, 0f), new Vector3(6.2f, 0.28f, 1.2f), accent, false);

        GameObject emblem = CreatePrimitive(PrimitiveType.Cylinder, "Emblem", root.transform,
            new Vector3(0f, 4.15f, -0.25f), new Vector3(0.42f, 0.08f, 0.42f), brass, false);
        emblem.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

        return SavePrefab(root, path);
    }

    private static GameObject BuildTitanBoiler(string prefabName)
    {
        string path = RootFolder + "/" + prefabName + ".prefab";
        GameObject existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (existing != null) return existing;

        Material iron = GetOrCreateMaterial("Hero_DarkIron", new Color(0.15f, 0.12f, 0.10f), false, Color.black);
        Material copper = GetOrCreateMaterial("Hero_Copper", new Color(0.46f, 0.22f, 0.09f), false, Color.black);
        Material glow = GetOrCreateMaterial("Hero_FurnaceGlow", new Color(0.30f, 0.11f, 0.04f), true, new Color(1.0f, 0.38f, 0.11f) * 4.0f);

        GameObject root = new GameObject(prefabName);

        CreatePrimitive(PrimitiveType.Cylinder, "MainTank", root.transform,
            new Vector3(0f, 2.3f, 0f), new Vector3(2.0f, 3.0f, 2.0f), iron, false);

        CreatePrimitive(PrimitiveType.Cube, "Base", root.transform,
            new Vector3(0f, 0.45f, 0f), new Vector3(3.2f, 0.9f, 2.6f), iron, false);

        CreatePrimitive(PrimitiveType.Cube, "FurnaceDoor", root.transform,
            new Vector3(0f, 2.0f, -1.75f), new Vector3(1.15f, 1.25f, 0.10f), glow, false);

        CreatePrimitive(PrimitiveType.Cylinder, "PipeStackLeft", root.transform,
            new Vector3(-1.6f, 5.3f, 0.1f), new Vector3(0.22f, 1.6f, 0.22f), copper, false);

        CreatePrimitive(PrimitiveType.Cylinder, "PipeStackRight", root.transform,
            new Vector3(1.6f, 5.0f, -0.1f), new Vector3(0.26f, 1.8f, 0.26f), copper, false);

        GameObject gauge = CreatePrimitive(PrimitiveType.Cylinder, "Gauge", root.transform,
            new Vector3(1.3f, 2.8f, -1.7f), new Vector3(0.28f, 0.08f, 0.28f), copper, false);
        gauge.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

        AddPulse(root, new Color(1.0f, 0.42f, 0.12f));
        return SavePrefab(root, path);
    }

    private static GameObject BuildKikaiShrine(string prefabName)
    {
        string path = RootFolder + "/" + prefabName + ".prefab";
        GameObject existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (existing != null) return existing;

        Material stone = GetOrCreateMaterial("Hero_ShrineStone", new Color(0.18f, 0.19f, 0.20f), false, Color.black);
        Material brass = GetOrCreateMaterial("Hero_ShrineBrass", new Color(0.55f, 0.42f, 0.14f), false, Color.black);
        Material cyan = GetOrCreateMaterial("Hero_EtherCore", new Color(0.08f, 0.32f, 0.38f), true, new Color(0.12f, 1.00f, 1.00f) * 4.3f);

        GameObject root = new GameObject(prefabName);

        CreatePrimitive(PrimitiveType.Cube, "Pedestal", root.transform,
            new Vector3(0f, 0.55f, 0f), new Vector3(2.2f, 1.1f, 2.2f), stone, false);

        CreatePrimitive(PrimitiveType.Cube, "Column", root.transform,
            new Vector3(0f, 2.3f, 0f), new Vector3(0.9f, 2.2f, 0.9f), brass, false);

        CreatePrimitive(PrimitiveType.Sphere, "EtherCore", root.transform,
            new Vector3(0f, 3.9f, 0f), new Vector3(1.1f, 1.1f, 1.1f), cyan, false);

        CreatePrimitive(PrimitiveType.Cube, "CrossArmX", root.transform,
            new Vector3(0f, 3.9f, 0f), new Vector3(2.2f, 0.15f, 0.15f), brass, false);

        CreatePrimitive(PrimitiveType.Cube, "CrossArmY", root.transform,
            new Vector3(0f, 3.9f, 0f), new Vector3(0.15f, 2.2f, 0.15f), brass, false);

        GameObject lightObject = new GameObject("EtherLight");
        lightObject.transform.SetParent(root.transform);
        lightObject.transform.localPosition = new Vector3(0f, 3.9f, 0.18f);

        Light light = lightObject.AddComponent<Light>();
        light.type = LightType.Point;
        light.range = 7f;
        light.intensity = 3.8f;
        light.color = new Color(0.12f, 1f, 1f);

        FoundryLightFlicker flicker = lightObject.AddComponent<FoundryLightFlicker>();
        SerializedObject lightSO = new SerializedObject(flicker);
        lightSO.FindProperty("targetLight").objectReferenceValue = light;
        lightSO.FindProperty("minIntensity").floatValue = 2.8f;
        lightSO.FindProperty("maxIntensity").floatValue = 4.4f;
        lightSO.FindProperty("speed").floatValue = 4.4f;
        lightSO.ApplyModifiedPropertiesWithoutUndo();

        AddPulse(root, new Color(0.12f, 1f, 1f));
        return SavePrefab(root, path);
    }

    private static GameObject BuildCraneAssembly(string prefabName)
    {
        string path = RootFolder + "/" + prefabName + ".prefab";
        GameObject existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (existing != null) return existing;

        Material iron = GetOrCreateMaterial("Hero_CraneIron", new Color(0.14f, 0.12f, 0.11f), false, Color.black);
        Material brass = GetOrCreateMaterial("Hero_CraneBrass", new Color(0.54f, 0.40f, 0.12f), false, Color.black);

        GameObject root = new GameObject(prefabName);

        CreatePrimitive(PrimitiveType.Cube, "Tower", root.transform,
            new Vector3(0f, 4.0f, 0f), new Vector3(0.9f, 8.0f, 0.9f), iron, false);

        CreatePrimitive(PrimitiveType.Cube, "Arm", root.transform,
            new Vector3(3.5f, 7.2f, 0f), new Vector3(7.2f, 0.5f, 0.5f), iron, false);

        GameObject pulley = CreatePrimitive(PrimitiveType.Cylinder, "Pulley", root.transform,
            new Vector3(6.6f, 7.0f, 0f), new Vector3(0.45f, 0.1f, 0.45f), brass, false);
        pulley.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

        CreatePrimitive(PrimitiveType.Cube, "Cable", root.transform,
            new Vector3(6.6f, 4.0f, 0f), new Vector3(0.08f, 6.0f, 0.08f), brass, false);

        CreatePrimitive(PrimitiveType.Cube, "Hook", root.transform,
            new Vector3(6.6f, 0.8f, 0f), new Vector3(0.35f, 0.7f, 0.12f), brass, false);

        GameObject gear = CreateGearChild(root.transform, new Vector3(-0.7f, 6.3f, 0.1f), 1.1f, brass);
        AddRotatingGear(gear, 58f);
        return SavePrefab(root, path);
    }

    private static GameObject BuildPipeSpine(string prefabName)
    {
        string path = RootFolder + "/" + prefabName + ".prefab";
        GameObject existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (existing != null) return existing;

        Material pipe = GetOrCreateMaterial("Hero_PipeCopper", new Color(0.45f, 0.21f, 0.09f), false, Color.black);
        Material brace = GetOrCreateMaterial("Hero_PipeBrace", new Color(0.17f, 0.14f, 0.12f), false, Color.black);
        GameObject root = new GameObject(prefabName);

        for (int row = 0; row < 4; row++)
        {
            GameObject cyl = CreatePrimitive(PrimitiveType.Cylinder, "Pipe_" + row, root.transform,
                new Vector3(0f, 0.4f + row * 0.55f, 0f), new Vector3(0.14f, 4.0f, 0.14f), pipe, false);
            cyl.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        }

        for (int i = -1; i <= 1; i++)
        {
            CreatePrimitive(PrimitiveType.Cube, "Brace_" + i, root.transform,
                new Vector3(i * 3.2f, 1.1f, 0f), new Vector3(0.20f, 2.3f, 0.20f), brace, false);
        }

        return SavePrefab(root, path);
    }

    private static GameObject BuildPaintedPanel(string prefabName, Color tint)
    {
        string path = RootFolder + "/" + prefabName + ".prefab";
        GameObject existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (existing != null) return existing;

        Material panel = GetOrCreateMaterial(prefabName + "_Mat", tint, false, Color.black);
        Material ink = GetOrCreateMaterial("Hero_BackdropInk", new Color(0.05f, 0.05f, 0.06f), false, Color.black);
        GameObject root = new GameObject(prefabName);

        CreatePrimitive(PrimitiveType.Quad, "Panel", root.transform,
            new Vector3(0f, 0f, 0f), new Vector3(12f, 6.5f, 1f), panel, false);

        CreatePrimitive(PrimitiveType.Cube, "InkShapeA", root.transform,
            new Vector3(-3.2f, -0.6f, -0.05f), new Vector3(4.2f, 1.6f, 0.02f), ink, false);

        CreatePrimitive(PrimitiveType.Cube, "InkShapeB", root.transform,
            new Vector3(2.8f, 1.1f, -0.05f), new Vector3(3.6f, 1.2f, 0.02f), ink, false);

        return SavePrefab(root, path);
    }

    private static GameObject BuildRooflineCluster(string prefabName)
    {
        string path = RootFolder + "/" + prefabName + ".prefab";
        GameObject existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (existing != null) return existing;

        Material body = GetOrCreateMaterial("Hero_RooflineSilhouette", new Color(0.06f, 0.06f, 0.07f), false, Color.black);
        GameObject root = new GameObject(prefabName);

        CreatePrimitive(PrimitiveType.Cube, "BodyA", root.transform,
            new Vector3(-2.6f, 2.6f, 0f), new Vector3(4.4f, 5.2f, 0.5f), body, false);

        CreatePrimitive(PrimitiveType.Cube, "BodyB", root.transform,
            new Vector3(2.2f, 3.3f, 0f), new Vector3(3.8f, 6.6f, 0.5f), body, false);

        CreatePrimitive(PrimitiveType.Cylinder, "Stack", root.transform,
            new Vector3(4.6f, 6.4f, 0.1f), new Vector3(0.42f, 1.4f, 0.42f), body, false);

        return SavePrefab(root, path);
    }

    private static GameObject BuildClothStrip(string prefabName)
    {
        string path = RootFolder + "/" + prefabName + ".prefab";
        GameObject existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (existing != null) return existing;

        Material cloth = GetOrCreateMaterial("Hero_Cloth", new Color(0.45f, 0.11f, 0.09f), false, Color.black);
        GameObject root = new GameObject(prefabName);

        GameObject strip = CreatePrimitive(PrimitiveType.Cube, "Strip", root.transform,
            Vector3.zero, new Vector3(0.9f, 3.0f, 0.08f), cloth, false);

        FoundryAutoSway sway = strip.AddComponent<FoundryAutoSway>();
        SerializedObject swaySO = new SerializedObject(sway);
        swaySO.FindProperty("axis").vector3Value = Vector3.forward;
        swaySO.FindProperty("amplitude").floatValue = 3.2f;
        swaySO.FindProperty("speed").floatValue = 1.15f;
        swaySO.ApplyModifiedPropertiesWithoutUndo();

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
        if (renderer != null && material != null)
            renderer.sharedMaterial = material;

        if (!keepCollider)
        {
            Collider collider = go.GetComponent<Collider>();
            if (collider != null)
                Object.DestroyImmediate(collider);
        }

        return go;
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
            Vector3 offset = Quaternion.Euler(0f, 0f, angle) * Vector3.right * (scale * 0.72f);

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
        so.FindProperty("minIntensity").floatValue = 0.8f;
        so.FindProperty("maxIntensity").floatValue = 1.45f;
        so.FindProperty("speed").floatValue = 2.2f;
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    private static GameObject SavePrefab(GameObject root, string path)
    {
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        Object.DestroyImmediate(root);
        return prefab;
    }

    private static Material GetOrCreateMaterial(string materialName, Color baseColor, bool emission, Color emissionColor)
    {
        EnsureFolder("Assets/_Game/Art");
        EnsureFolder("Assets/_Game/Art/Materials");
        EnsureFolder(MaterialFolder);

        string path = MaterialFolder + "/" + materialName + ".mat";
        Material existing = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (existing != null) return existing;

        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) shader = Shader.Find("Standard");

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

        string parent = Path.GetDirectoryName(path)?.Replace("\\", "/");
        string folder = Path.GetFileName(path);

        if (!string.IsNullOrEmpty(parent) && !AssetDatabase.IsValidFolder(parent))
            EnsureFolder(parent);

        if (!string.IsNullOrEmpty(parent))
            AssetDatabase.CreateFolder(parent, folder);
    }
}

#endif
