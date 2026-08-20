#if UNITY_EDITOR

using System.IO;
using UnityEditor;

[InitializeOnLoad]
public static class KenjiroCombatUpgradeBuilder
{
    private const string ScenePath =
        "Assets/_Game/Scenes/Prototype/Foundry_Prototype.unity";

    static KenjiroCombatUpgradeBuilder()
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

        if (!sceneText.Contains("CombatSystem_v7"))
        {
            FoundryPrototypeSceneBuilder.CreateOrRebuild();
        }
    }
}

#endif
