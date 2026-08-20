#if UNITY_EDITOR

using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class KatsuhiroV171PacingBuildMenu
{
    [MenuItem("Tools/Katsuhiro/Prepare v17.1 Pacing 8-10 Minutes")]
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

        KatsuhiroV171PacingQAValidator.Result qa =
            KatsuhiroV171PacingQAValidator
                .Run(true);

        if (!qa.CanBuild)
        {
            Debug.LogError(
                "Katsuhiro v17.1 : préparation bloquée — " +
                qa.Blocker +
                " blocker(s). Voir v17_1_PACING_QA_REPORT.txt."
            );

            return;
        }

        Debug.Log(
            "Katsuhiro v17.1 : pacing 8–10 minutes prêt pour playtest."
        );
    }

    [MenuItem("Tools/Katsuhiro/Build v17.1 Pacing Playtest - Current Platform")]
    public static void Build()
    {
        Prepare();

        KatsuhiroV171PacingQAValidator.Result qa =
            KatsuhiroV171PacingQAValidator
                .Run(false);

        if (!qa.CanBuild)
        {
            Debug.LogError(
                "Build v17.1 annulée : QA pacing avec blocker."
            );

            return;
        }

        BuildTarget target =
            EditorUserBuildSettings.activeBuildTarget;

        if (target == BuildTarget.NoTarget)
        {
            Debug.LogError(
                "Build v17.1 annulée : aucune plateforme active."
            );

            return;
        }

        string buildRoot =
            Path.GetFullPath(
                "Builds/Katsuhiro_v17_1_Pacing_Playtest"
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
                "Katsuhiro v17.1 : build non aboutie (" +
                report.summary.result +
                ")."
            );

            return;
        }

        CopyDocument(
            "Assets/_Game/Playtest/PACING_TEST_GUIDE_v17_1.txt",
            buildRoot
        );

        CopyDocument(
            "Assets/_Game/Playtest/EXTERNAL_PLAYTEST_SURVEY_v17.txt",
            buildRoot
        );

        CopyDocument(
            "Assets/_Game/QA/v17_1_PACING_QA_REPORT.txt",
            buildRoot
        );

        Debug.Log(
            "Katsuhiro v17.1 : build pacing créée : " +
            output
        );
    }

    private static void CopyDocument(
        string source,
        string destinationFolder
    )
    {
        if (!File.Exists(source))
            return;

        File.Copy(
            source,
            Path.Combine(
                destinationFolder,
                Path.GetFileName(source)
            ),
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
                    "Katsuhiro_v17_1_Pacing.exe"
                );

            case BuildTarget.StandaloneOSX:
                return Path.Combine(
                    root,
                    "Katsuhiro_v17_1_Pacing.app"
                );

            case BuildTarget.StandaloneLinux64:
                return Path.Combine(
                    root,
                    "Katsuhiro_v17_1_Pacing.x86_64"
                );

            default:
                return Path.Combine(
                    root,
                    "Katsuhiro_v17_1_Pacing"
                );
        }
    }
}

#endif
