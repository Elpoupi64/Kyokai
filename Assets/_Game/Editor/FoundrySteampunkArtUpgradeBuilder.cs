#if UNITY_EDITOR

using System.IO;
using UnityEditor;

[InitializeOnLoad]
public static class FoundrySteampunkArtUpgradeBuilder
{
    private const string ScenePath =
        "Assets/_Game/Scenes/Prototype/Foundry_Prototype.unity";

    static FoundrySteampunkArtUpgradeBuilder()
    {
        EditorApplication.delayCall += AutoUpgradeIfNeeded;
    }

    private static void AutoUpgradeIfNeeded()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
            return;

        if (!File.Exists(ScenePath))
            return;

        string sceneText =
            File.ReadAllText(ScenePath);

        if (!sceneText.Contains("FoundryArt_v10"))
            FoundryPrototypeSceneBuilder.CreateOrRebuild();
    }
}

#endif
