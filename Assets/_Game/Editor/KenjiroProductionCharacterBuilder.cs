#if UNITY_EDITOR

using System.IO;
using UnityEditor;
using UnityEngine;

public static class KenjiroProductionCharacterBuilder
{
    private const string MaterialFolder =
        "Assets/_Game/Art/Materials/Characters/Kenjiro_v16";

    public static GameObject UpgradePrefab(
        GameObject prefabAsset
    )
    {
        if (prefabAsset == null)
            return null;

        string path =
            AssetDatabase.GetAssetPath(
                prefabAsset
            );

        if (string.IsNullOrEmpty(path))
            return prefabAsset;

        EnsureFolder("Assets/_Game/Art");
        EnsureFolder("Assets/_Game/Art/Materials");
        EnsureFolder("Assets/_Game/Art/Materials/Characters");
        EnsureFolder(MaterialFolder);

        GameObject root =
            PrefabUtility.LoadPrefabContents(
                path
            );

        try
        {
            Transform modelRoot =
                root.transform.Find("ModelRoot");

            if (modelRoot == null)
                return prefabAsset;

            Transform oldMarker =
                modelRoot.Find("KenjiroCharacter_v16");

            if (oldMarker != null)
                Object.DestroyImmediate(
                    oldMarker.gameObject
                );

            Material coat =
                GetOrCreateMaterial(
                    "Kenjiro_v16_Coat",
                    new Color(0.055f, 0.060f, 0.070f),
                    false,
                    Color.black
                );

            Material coatEdge =
                GetOrCreateMaterial(
                    "Kenjiro_v16_CoatEdge",
                    new Color(0.12f, 0.11f, 0.11f),
                    false,
                    Color.black
                );

            Material leather =
                GetOrCreateMaterial(
                    "Kenjiro_v16_Leather",
                    new Color(0.24f, 0.105f, 0.045f),
                    false,
                    Color.black
                );

            Material leatherDark =
                GetOrCreateMaterial(
                    "Kenjiro_v16_LeatherDark",
                    new Color(0.105f, 0.055f, 0.030f),
                    false,
                    Color.black
                );

            Material brass =
                GetOrCreateMaterial(
                    "Kenjiro_v16_Brass",
                    new Color(0.52f, 0.34f, 0.09f),
                    false,
                    Color.black
                );

            Material skin =
                GetOrCreateMaterial(
                    "Kenjiro_v16_Skin",
                    new Color(0.74f, 0.54f, 0.40f),
                    false,
                    Color.black
                );

            Material hair =
                GetOrCreateMaterial(
                    "Kenjiro_v16_Hair",
                    new Color(0.025f, 0.028f, 0.035f),
                    false,
                    Color.black
                );

            Material white =
                GetOrCreateMaterial(
                    "Kenjiro_v16_Shirt",
                    new Color(0.78f, 0.75f, 0.66f),
                    false,
                    Color.black
                );

            Material ether =
                GetOrCreateMaterial(
                    "Kenjiro_v16_Ether",
                    new Color(0.035f, 0.36f, 0.48f),
                    true,
                    new Color(0.10f, 0.95f, 1.00f) * 3.2f
                );

            GameObject marker =
                new GameObject("KenjiroCharacter_v16");

            marker.transform.SetParent(
                modelRoot
            );

            marker.transform.localPosition =
                Vector3.zero;

            // Long coat silhouette.
            Transform torso =
                modelRoot.Find("Coat_Torso");

            if (torso != null)
            {
                CreatePart(
                    PrimitiveType.Cube,
                    "CoatTail_Back",
                    torso,
                    new Vector3(-0.19f, -0.72f, 0.12f),
                    new Vector3(0.34f, 0.78f, 0.08f),
                    coat
                ).transform.localRotation =
                    Quaternion.Euler(0f, 0f, 8f);

                CreatePart(
                    PrimitiveType.Cube,
                    "CoatTail_Front",
                    torso,
                    new Vector3(0.21f, -0.73f, -0.12f),
                    new Vector3(0.36f, 0.82f, 0.08f),
                    coat
                ).transform.localRotation =
                    Quaternion.Euler(0f, 0f, -8f);

                CreatePart(
                    PrimitiveType.Cube,
                    "Lapel_Left",
                    torso,
                    new Vector3(-0.16f, 0.10f, -0.23f),
                    new Vector3(0.16f, 0.62f, 0.035f),
                    coatEdge
                ).transform.localRotation =
                    Quaternion.Euler(0f, 0f, -18f);

                CreatePart(
                    PrimitiveType.Cube,
                    "Lapel_Right",
                    torso,
                    new Vector3(0.16f, 0.10f, -0.23f),
                    new Vector3(0.16f, 0.62f, 0.035f),
                    coatEdge
                ).transform.localRotation =
                    Quaternion.Euler(0f, 0f, 18f);

                CreatePart(
                    PrimitiveType.Cube,
                    "Collar",
                    torso,
                    new Vector3(0f, 0.45f, -0.15f),
                    new Vector3(0.58f, 0.18f, 0.10f),
                    coatEdge
                );

                CreatePart(
                    PrimitiveType.Cube,
                    "Vest",
                    torso,
                    new Vector3(0f, 0.03f, -0.25f),
                    new Vector3(0.38f, 0.52f, 0.045f),
                    coatEdge
                );

                for (int i = 0; i < 3; i++)
                {
                    CreatePart(
                        PrimitiveType.Sphere,
                        "VestButton_" + i,
                        torso,
                        new Vector3(
                            0.04f,
                            0.18f - i * 0.17f,
                            -0.29f
                        ),
                        new Vector3(
                            0.055f,
                            0.055f,
                            0.035f
                        ),
                        brass
                    );
                }
            }

            // Hands to improve action silhouette.
            Transform frontArm =
                modelRoot.Find("Arm_Front");

            Transform backArm =
                modelRoot.Find("Arm_Back");

            if (frontArm != null)
            {
                CreatePart(
                    PrimitiveType.Sphere,
                    "Hand_Front",
                    frontArm,
                    new Vector3(0f, -0.52f, 0f),
                    new Vector3(0.22f, 0.25f, 0.20f),
                    skin
                );
            }

            if (backArm != null)
            {
                CreatePart(
                    PrimitiveType.Sphere,
                    "Hand_Back",
                    backArm,
                    new Vector3(0f, -0.52f, 0f),
                    new Vector3(0.22f, 0.25f, 0.20f),
                    skin
                );
            }

            // Hair spikes / stronger silhouette.
            Transform hairRoot =
                modelRoot.Find("Hair");

            if (hairRoot != null)
            {
                for (int i = 0; i < 5; i++)
                {
                    GameObject spike =
                        CreatePart(
                            PrimitiveType.Cube,
                            "HairSpike_" + i,
                            hairRoot,
                            new Vector3(
                                -0.22f + i * 0.11f,
                                0.28f + (i % 2) * 0.04f,
                                -0.02f
                            ),
                            new Vector3(
                                0.12f,
                                0.34f + (i % 3) * 0.06f,
                                0.12f
                            ),
                            hair
                        );

                    spike.transform.localRotation =
                        Quaternion.Euler(
                            0f,
                            0f,
                            -28f + i * 14f
                        );
                }
            }

            // Satchel strap + flap + buckles.
            Transform satchel =
                modelRoot.Find("Leather_Satchel");

            if (satchel != null)
            {
                CreatePart(
                    PrimitiveType.Cube,
                    "Satchel_Flap",
                    satchel,
                    new Vector3(0f, 0.19f, -0.13f),
                    new Vector3(0.90f, 0.28f, 0.05f),
                    leatherDark
                );

                for (int i = -1; i <= 1; i += 2)
                {
                    CreatePart(
                        PrimitiveType.Cube,
                        "Satchel_Buckle_" + i,
                        satchel,
                        new Vector3(
                            i * 0.23f,
                            0.05f,
                            -0.16f
                        ),
                        new Vector3(
                            0.09f,
                            0.16f,
                            0.05f
                        ),
                        brass
                    );
                }
            }

            GameObject strap =
                CreatePart(
                    PrimitiveType.Cube,
                    "Satchel_Strap",
                    marker.transform,
                    new Vector3(-0.03f, 0.28f, 0.23f),
                    new Vector3(0.12f, 1.55f, 0.08f),
                    leather
                );

            strap.transform.localRotation =
                Quaternion.Euler(
                    0f,
                    0f,
                    -28f
                );

            // Pocket watch / ether compass.
            GameObject watch =
                CreatePart(
                    PrimitiveType.Cylinder,
                    "PocketWatch",
                    marker.transform,
                    new Vector3(-0.13f, -0.17f, -0.28f),
                    new Vector3(0.14f, 0.045f, 0.14f),
                    brass
                );

            watch.transform.localRotation =
                Quaternion.Euler(90f, 0f, 0f);

            GameObject compass =
                CreatePart(
                    PrimitiveType.Cylinder,
                    "EtherCompass",
                    marker.transform,
                    new Vector3(0.16f, -0.17f, -0.29f),
                    new Vector3(0.13f, 0.045f, 0.13f),
                    brass
                );

            compass.transform.localRotation =
                Quaternion.Euler(90f, 0f, 0f);

            CreatePart(
                PrimitiveType.Sphere,
                "EtherCompass_Core",
                compass.transform,
                new Vector3(0f, 0.02f, 0f),
                new Vector3(0.52f, 0.20f, 0.52f),
                ether
            );

            // Boots: soles and brass buckles.
            DecorateBoot(
                modelRoot.Find("Shoe_Front"),
                leatherDark,
                brass
            );

            DecorateBoot(
                modelRoot.Find("Shoe_Back"),
                leatherDark,
                brass
            );

            // Kikai-Yūrei: preserve the existing gameplay object and add
            // recognizable copper tubes, side coils and a stronger lens.
            Transform device =
                modelRoot.Find("KikaiYurei_Device");

            if (device != null)
            {
                CreatePart(
                    PrimitiveType.Cylinder,
                    "LensRing",
                    device,
                    new Vector3(0f, 0f, -0.19f),
                    new Vector3(0.31f, 0.07f, 0.31f),
                    brass
                ).transform.localRotation =
                    Quaternion.Euler(90f, 0f, 0f);

                CreatePart(
                    PrimitiveType.Sphere,
                    "LensGlass",
                    device,
                    new Vector3(0f, 0f, -0.235f),
                    new Vector3(0.21f, 0.21f, 0.08f),
                    ether
                );

                for (int i = -1; i <= 1; i += 2)
                {
                    GameObject tube =
                        CreatePart(
                            PrimitiveType.Cylinder,
                            "CopperTube_" + i,
                            device,
                            new Vector3(
                                i * 0.23f,
                                0.08f,
                                0f
                            ),
                            new Vector3(
                                0.055f,
                                0.30f,
                                0.055f
                            ),
                            brass
                        );

                    tube.transform.localRotation =
                        Quaternion.Euler(
                            0f,
                            0f,
                            i * 14f
                        );
                }

                CreatePart(
                    PrimitiveType.Cylinder,
                    "LowerCoil",
                    device,
                    new Vector3(0f, -0.27f, 0f),
                    new Vector3(0.15f, 0.22f, 0.15f),
                    brass
                );
            }

            // Small production marker only.
            marker.hideFlags =
                HideFlags.None;

            PrefabUtility.SaveAsPrefabAsset(
                root,
                path
            );
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(
                root
            );
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        return AssetDatabase.LoadAssetAtPath<GameObject>(
            path
        );
    }

    private static void DecorateBoot(
        Transform boot,
        Material sole,
        Material brass
    )
    {
        if (boot == null)
            return;

        CreatePart(
            PrimitiveType.Cube,
            "Sole",
            boot,
            new Vector3(0.08f, -0.46f, 0f),
            new Vector3(1.10f, 0.18f, 1.10f),
            sole
        );

        CreatePart(
            PrimitiveType.Cube,
            "Buckle",
            boot,
            new Vector3(0.20f, 0.12f, -0.53f),
            new Vector3(0.18f, 0.20f, 0.06f),
            brass
        );
    }

    private static GameObject CreatePart(
        PrimitiveType type,
        string name,
        Transform parent,
        Vector3 localPosition,
        Vector3 localScale,
        Material material
    )
    {
        GameObject go =
            GameObject.CreatePrimitive(type);

        go.name = name;
        go.transform.SetParent(parent);
        go.transform.localPosition = localPosition;
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = localScale;

        Renderer renderer =
            go.GetComponent<Renderer>();

        if (renderer != null)
        {
            renderer.sharedMaterial = material;
            renderer.shadowCastingMode =
                UnityEngine.Rendering.ShadowCastingMode.On;

            renderer.receiveShadows = true;
        }

        Collider collider =
            go.GetComponent<Collider>();

        if (collider != null)
            Object.DestroyImmediate(collider);

        return go;
    }

    private static Material GetOrCreateMaterial(
        string name,
        Color baseColor,
        bool emission,
        Color emissionColor
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
            shader = Shader.Find("Standard");

        Material material =
            new Material(shader);

        material.name = name;

        if (material.HasProperty("_BaseColor"))
            material.SetColor(
                "_BaseColor",
                baseColor
            );

        if (material.HasProperty("_Color"))
            material.SetColor(
                "_Color",
                baseColor
            );

        if (emission &&
            material.HasProperty("_EmissionColor"))
        {
            material.EnableKeyword("_EMISSION");
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
