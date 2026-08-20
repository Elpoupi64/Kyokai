#if UNITY_EDITOR

using System.IO;
using UnityEditor;

[InitializeOnLoad]
public static class KatsuhiroV171UpgradeBuilder
{
    private const string ScenePath =
        "Assets/_Game/Scenes/Prototype/Foundry_Prototype.unity";

    static KatsuhiroV171UpgradeBuilder()
    {
        EditorApplication.delayCall +=
            AutoUpgradeIfNeeded;
    }

    private static void AutoUpgradeIfNeeded()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
            return;

        if (!File.Exists(ScenePath))
            return;

        string sceneText =
            File.ReadAllText(
                ScenePath
            );

        if (!sceneText.Contains(
            "LevelPacing_v17_1"
        ))
        {
            FoundryPrototypeSceneBuilder
                .CreateOrRebuild();
        }
    }
}

#endif
