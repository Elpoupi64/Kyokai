#if UNITY_EDITOR

using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class KatsuhiroDemoBuildMenu
{
    [MenuItem("Tools/Katsuhiro/Prepare v16.1 Demo RC2")]
    public static void PrepareV161ReleaseCandidate()
    {
        PrepareDemoBuild();
    }

    [MenuItem("Tools/Katsuhiro/Prepare v16 Demo Release Candidate")]
    public static void PrepareV16ReleaseCandidate()
    {
        PrepareDemoBuild();
    }

    [MenuItem("Tools/Katsuhiro/Prepare v15 Demo Build")]
    public static void PrepareDemoBuild()
    {
        FoundryPrototypeSceneBuilder
            .CreateOrRebuild();

        KatsuhiroDemoTitleSceneBuilder
            .CreateOrRebuildTitleScene();

        KatsuhiroDemoTitleSceneBuilder
            .ConfigureBuildSettings();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        KatsuhiroV16QAValidator.ValidationResult result =
            KatsuhiroV16QAValidator
                .RunPreflight(true);

        if (!result.CanBuild)
        {
            Debug.LogError(
                "Katsuhiro v16.1 RC2 : préparation terminée, " +
                "mais le QA contient " +
                result.BlockerCount +
                " blocker(s). Consultez Assets/_Game/QA/v16_1_QA_REPORT.txt."
            );

            return;
        }

        Debug.Log(
            "Katsuhiro v16.1 RC2 : démo candidate prête. " +
            "TitleScreen est la scène 0, Foundry_Prototype la scène 1. " +
            "QA structurel sans blocker."
        );
    }

    [MenuItem("Tools/Katsuhiro/Build v16.1 RC2 - Current Platform")]
    public static void BuildV161CurrentPlatform()
    {
        BuildCurrentPlatform();
    }

    [MenuItem("Tools/Katsuhiro/Build Demo - Current Platform")]
    public static void BuildCurrentPlatform()
    {
        PrepareDemoBuild();

        KatsuhiroV16QAValidator.ValidationResult validation =
            KatsuhiroV16QAValidator
                .RunPreflight(false);

        if (!validation.CanBuild)
        {
            Debug.LogError(
                "Build annulée : " +
                validation.BlockerCount +
                " blocker(s) QA. " +
                "Corrigez le rapport v16_1_QA_REPORT.txt avant de construire."
            );

            return;
        }

        string root =
            Path.GetFullPath("Builds");

        Directory.CreateDirectory(root);

        BuildTarget target =
            EditorUserBuildSettings.activeBuildTarget;

        if (target == BuildTarget.NoTarget)
        {
            Debug.LogError(
                "Build annulée : aucune plateforme active."
            );

            return;
        }

        string output =
            GetOutputPath(
                root,
                target
            );

        BuildPlayerOptions options =
            new BuildPlayerOptions
            {
                scenes = new[]
                {
                    KatsuhiroDemoTitleSceneBuilder.TitleScenePath,
                    KatsuhiroDemoTitleSceneBuilder.GameplayScenePath
                },
                locationPathName = output,
                target = target,
                options = BuildOptions.None
            };

        BuildReport report =
            BuildPipeline.BuildPlayer(
                options
            );

        if (report.summary.result ==
            BuildResult.Succeeded)
        {
            Debug.Log(
                "Katsuhiro v16.1 RC2 : build réussie : " +
                output +
                " | Taille : " +
                report.summary.totalSize +
                " octets."
            );
        }
        else
        {
            Debug.LogError(
                "Katsuhiro v16.1 RC2 : build non aboutie (" +
                report.summary.result +
                "). Vérifiez les erreurs Console et le module de plateforme Unity."
            );
        }
    }

    private static string GetOutputPath(
        string root,
        BuildTarget target
    )
    {
        switch (target)
        {
            case BuildTarget.StandaloneWindows:
            case BuildTarget.StandaloneWindows64:
                return Path.Combine(
                    root,
                    "Katsuhiro_Demo_v16_1_RC2.exe"
                );

            case BuildTarget.StandaloneOSX:
                return Path.Combine(
                    root,
                    "Katsuhiro_Demo_v16_1_RC2.app"
                );

            case BuildTarget.StandaloneLinux64:
                return Path.Combine(
                    root,
                    "Katsuhiro_Demo_v16_1_RC2.x86_64"
                );

            default:
                return Path.Combine(
                    root,
                    "Katsuhiro_Demo_v16_1_RC2"
                );
        }
    }
}

#endif
