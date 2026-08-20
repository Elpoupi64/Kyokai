#if UNITY_EDITOR

using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class KatsuhiroV17ExternalPlaytestBuildMenu
{
    [MenuItem("Tools/Katsuhiro/Prepare v17 External Playtest")]
    public static void Prepare()
    {
        FoundryPrototypeSceneBuilder
            .CreateOrRebuild();

        KatsuhiroDemoTitleSceneBuilder
            .CreateOrRebuildTitleScene();

        KatsuhiroDemoTitleSceneBuilder
            .ConfigureBuildSettings();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        KatsuhiroV17PlaytestQAValidator.Result qa =
            KatsuhiroV17PlaytestQAValidator
                .Run(true);

        if (!qa.CanBuild)
        {
            Debug.LogError(
                "Katsuhiro v17 : préparation playtest bloquée. " +
                qa.Blocker +
                " blocker(s). Voir v17_PLAYTEST_QA_REPORT.txt."
            );

            return;
        }

        Debug.Log(
            "Katsuhiro v17 : External Playtest prêt structurellement."
        );
    }

    [MenuItem("Tools/Katsuhiro/Build v17 External Playtest - Current Platform")]
    public static void Build()
    {
        Prepare();

        KatsuhiroV17PlaytestQAValidator.Result qa =
            KatsuhiroV17PlaytestQAValidator
                .Run(false);

        if (!qa.CanBuild)
        {
            Debug.LogError(
                "Build v17 annulée : QA playtest avec blocker."
            );

            return;
        }

        BuildTarget target =
            EditorUserBuildSettings
                .activeBuildTarget;

        if (target == BuildTarget.NoTarget)
        {
            Debug.LogError(
                "Build v17 annulée : aucune plateforme active."
            );

            return;
        }

        string buildRoot =
            Path.GetFullPath(
                "Builds/Katsuhiro_v17_ExternalPlaytest"
            );

        Directory.CreateDirectory(
            buildRoot
        );

        string output =
            GetOutputPath(
                buildRoot,
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

        if (report.summary.result !=
            BuildResult.Succeeded)
        {
            Debug.LogError(
                "Katsuhiro v17 : build externe non aboutie (" +
                report.summary.result +
                ")."
            );

            return;
        }

        CopyTesterDocument(
            "Assets/_Game/Playtest/PLAYTEST_GUIDE_v17.txt",
            buildRoot
        );

        CopyTesterDocument(
            "Assets/_Game/Playtest/EXTERNAL_PLAYTEST_SURVEY_v17.txt",
            buildRoot
        );

        CopyTesterDocument(
            "Assets/_Game/QA/v17_PLAYTEST_QA_REPORT.txt",
            buildRoot
        );

        Debug.Log(
            "Katsuhiro v17 : build External Playtest réussie : " +
            output
        );
    }

    private static void CopyTesterDocument(
        string source,
        string destinationFolder
    )
    {
        if (!File.Exists(source))
            return;

        string target =
            Path.Combine(
                destinationFolder,
                Path.GetFileName(source)
            );

        File.Copy(
            source,
            target,
            true
        );
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
                    "Katsuhiro_v17_Playtest.exe"
                );

            case BuildTarget.StandaloneOSX:
                return Path.Combine(
                    root,
                    "Katsuhiro_v17_Playtest.app"
                );

            case BuildTarget.StandaloneLinux64:
                return Path.Combine(
                    root,
                    "Katsuhiro_v17_Playtest.x86_64"
                );

            default:
                return Path.Combine(
                    root,
                    "Katsuhiro_v17_Playtest"
                );
        }
    }
}

#endif
