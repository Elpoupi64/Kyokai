#if UNITY_EDITOR

using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class KatsuhiroDemoTitleSceneBuilder
{
    public const string DemoSceneFolder =
        "Assets/_Game/Scenes/Demo";

    public const string TitleScenePath =
        DemoSceneFolder + "/TitleScreen.unity";

    public const string GameplayScenePath =
        "Assets/_Game/Scenes/Prototype/Foundry_Prototype.unity";

    private const string ReferenceFolder =
        "Assets/_Game/Art/Characters/ProductionReferences/v16";

    static KatsuhiroDemoTitleSceneBuilder()
    {
        EditorApplication.delayCall +=
            AutoEnsureDemoSetup;
    }

    public static void CreateOrRebuildTitleScene()
    {
        EnsureFolder("Assets/_Game");
        EnsureFolder("Assets/_Game/Scenes");
        EnsureFolder(DemoSceneFolder);

        ConfigureReferenceTextureImporters();

        Scene scene =
            EditorSceneManager.NewScene(
                NewSceneSetup.EmptyScene,
                NewSceneMode.Single
            );

        GameObject cameraObject =
            new GameObject("Main Camera");

        cameraObject.tag = "MainCamera";

        Camera camera =
            cameraObject.AddComponent<Camera>();

        camera.clearFlags =
            CameraClearFlags.SolidColor;

        camera.backgroundColor =
            new Color(
                0.035f,
                0.028f,
                0.030f
            );

        cameraObject.transform.position =
            new Vector3(
                0f,
                0f,
                -10f
            );

        GameObject qualityRoot =
            new GameObject("DemoQualityManager_v15");

        qualityRoot.AddComponent<DemoQualityManager>();

        GameObject controllerObject =
            new GameObject("DemoTitleScreen");

        DemoTitleScreenController controller =
            controllerObject.AddComponent<DemoTitleScreenController>();

        SerializedObject so =
            new SerializedObject(controller);

        so.FindProperty("kenjiroTexture").objectReferenceValue =
            AssetDatabase.LoadAssetAtPath<Texture2D>(
                ReferenceFolder +
                "/Kenjiro_TitleReference.png"
            );

        so.FindProperty("yukiTexture").objectReferenceValue =
            AssetDatabase.LoadAssetAtPath<Texture2D>(
                ReferenceFolder +
                "/Yuki_TitleReference.png"
            );

        so.FindProperty("takedaTexture").objectReferenceValue =
            AssetDatabase.LoadAssetAtPath<Texture2D>(
                ReferenceFolder +
                "/Takeda_TitleReference.png"
            );

        so.FindProperty("gameplayScene").stringValue =
            "Foundry_Prototype";

        so.ApplyModifiedPropertiesWithoutUndo();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(
            scene,
            TitleScenePath
        );

        ConfigureBuildSettings();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log(
            "Katsuhiro v16.1 RC2 : TitleScreen créé et Build Settings configurés."
        );
    }

    public static void ConfigureBuildSettings()
    {
        List<EditorBuildSettingsScene> scenes =
            new List<EditorBuildSettingsScene>();

        if (File.Exists(TitleScenePath))
        {
            scenes.Add(
                new EditorBuildSettingsScene(
                    TitleScenePath,
                    true
                )
            );
        }

        if (File.Exists(GameplayScenePath))
        {
            scenes.Add(
                new EditorBuildSettingsScene(
                    GameplayScenePath,
                    true
                )
            );
        }

        EditorBuildSettings.scenes =
            scenes.ToArray();
    }

    private static void AutoEnsureDemoSetup()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
            return;

        // In batch mode this fires on the same delayCall queue as an
        // -executeMethod build automation (e.g. KatsuhiroV17ExternalPlaytestBuildMenu),
        // which does its own deterministic scene setup via Prepare()/
        // CreateOrRebuildTitleScene(). Racing NewScene/OpenScene calls from
        // both at once corrupts in-flight asset references, so skip the
        // background auto-heal here — there's no interactive user to heal for.
        if (Application.isBatchMode)
            return;

        if (!File.Exists(TitleScenePath))
        {
            CreateOrRebuildTitleScene();
            return;
        }

        ConfigureReferenceTextureImporters();
        ConfigureBuildSettings();
        BackfillMissingTitleScreenReferences();
    }

    private static void BackfillMissingTitleScreenReferences()
    {
        Scene scene =
            EditorSceneManager.GetSceneByPath(
                TitleScenePath
            );

        bool openedTemporarily = false;

        if (!scene.IsValid() || !scene.isLoaded)
        {
            scene =
                EditorSceneManager.OpenScene(
                    TitleScenePath,
                    OpenSceneMode.Additive
                );

            openedTemporarily = true;
        }

        try
        {
            DemoTitleScreenController controller = null;

            foreach (GameObject root in scene.GetRootGameObjects())
            {
                controller =
                    root.GetComponentInChildren<DemoTitleScreenController>(
                        true
                    );

                if (controller != null)
                    break;
            }

            if (controller == null)
                return;

            SerializedObject so =
                new SerializedObject(controller);

            bool changed = false;

            changed |=
                BackfillTexture(
                    so,
                    "kenjiroTexture",
                    "Kenjiro_TitleReference.png"
                );

            changed |=
                BackfillTexture(
                    so,
                    "yukiTexture",
                    "Yuki_TitleReference.png"
                );

            changed |=
                BackfillTexture(
                    so,
                    "takedaTexture",
                    "Takeda_TitleReference.png"
                );

            if (changed)
            {
                so.ApplyModifiedPropertiesWithoutUndo();

                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);

                Debug.Log(
                    "Katsuhiro v16.1 RC2 : TitleScreen — référence(s) " +
                    "texture manquante(s) reliée(s) automatiquement."
                );
            }
        }
        finally
        {
            if (openedTemporarily)
            {
                EditorSceneManager.CloseScene(
                    scene,
                    true
                );
            }
        }
    }

    private static bool BackfillTexture(
        SerializedObject so,
        string propertyName,
        string textureFileName
    )
    {
        SerializedProperty property =
            so.FindProperty(propertyName);

        if (property == null ||
            property.objectReferenceValue != null)
        {
            return false;
        }

        Texture2D texture =
            AssetDatabase.LoadAssetAtPath<Texture2D>(
                ReferenceFolder +
                "/" +
                textureFileName
            );

        if (texture == null)
            return false;

        property.objectReferenceValue = texture;
        return true;
    }

    private static void ConfigureReferenceTextureImporters()
    {
        string[] names =
        {
            "Kenjiro_TitleReference.png",
            "Yuki_TitleReference.png",
            "Takeda_TitleReference.png",
            "Kenjiro_ModelSheet.png",
            "Yuki_ModelSheet.png",
            "Takeda_ModelSheet.png"
        };

        for (int i = 0; i < names.Length; i++)
        {
            string path =
                ReferenceFolder + "/" + names[i];

            if (File.Exists(path) &&
                AssetDatabase.LoadAssetAtPath<Texture2D>(path) == null)
            {
                // AssetDatabase can end up with a stale/desynced entry for
                // a file that exists on disk (e.g. after a .meta was
                // regenerated). Force a reimport so it resolves again.
                AssetDatabase.ImportAsset(
                    path,
                    ImportAssetOptions.ForceUpdate
                );
            }

            TextureImporter importer =
                AssetImporter.GetAtPath(path)
                    as TextureImporter;

            if (importer == null)
                continue;

            bool changed = false;

            if (importer.mipmapEnabled)
            {
                importer.mipmapEnabled = false;
                changed = true;
            }

            if (importer.maxTextureSize != 2048)
            {
                importer.maxTextureSize = 2048;
                changed = true;
            }

            if (importer.textureCompression !=
                TextureImporterCompression.Compressed)
            {
                importer.textureCompression =
                    TextureImporterCompression.Compressed;

                changed = true;
            }

            importer.alphaIsTransparency = true;

            if (changed)
                importer.SaveAndReimport();
        }
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
