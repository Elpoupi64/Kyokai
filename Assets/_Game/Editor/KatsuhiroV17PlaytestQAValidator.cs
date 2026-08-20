#if UNITY_EDITOR

using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public static class KatsuhiroV17PlaytestQAValidator
{
    private const string ReportFolder =
        "Assets/_Game/QA";

    private const string ReportPath =
        ReportFolder +
        "/v17_PLAYTEST_QA_REPORT.txt";

    public struct Result
    {
        public int Pass;
        public int Warning;
        public int Blocker;

        public bool CanBuild =>
            Blocker == 0;
    }

    [MenuItem("Tools/Katsuhiro/Run v17 External Playtest QA")]
    public static void RunMenu()
    {
        Run(true);
    }

    public static Result Run(
        bool selectReport
    )
    {
        KatsuhiroV16QAValidator.ValidationResult baseResult =
            KatsuhiroV16QAValidator
                .RunPreflight(false);

        List<string> pass =
            new List<string>();

        List<string> warning =
            new List<string>();

        List<string> blocker =
            new List<string>();

        if (baseResult.BlockerCount > 0)
        {
            blocker.Add(
                "v16.1 preflight contient " +
                baseResult.BlockerCount +
                " blocker(s)."
            );
        }
        else
        {
            pass.Add(
                "v16.1 preflight sans blocker."
            );
        }

        CheckAsset(
            "Playtest telemetry",
            "Assets/_Game/Scripts/Demo/DemoPlaytestTelemetry.cs",
            true,
            pass,
            warning,
            blocker
        );

        CheckAsset(
            "Playtest HUD",
            "Assets/_Game/Scripts/Demo/DemoPlaytestHUD.cs",
            true,
            pass,
            warning,
            blocker
        );

        CheckAsset(
            "Final polish builder",
            "Assets/_Game/Editor/KatsuhiroV17FinalPolishBuilder.cs",
            true,
            pass,
            warning,
            blocker
        );

        CheckAsset(
            "Playtest guide",
            "Assets/_Game/Playtest/PLAYTEST_GUIDE_v17.txt",
            true,
            pass,
            warning,
            blocker
        );

        CheckAsset(
            "Playtest survey",
            "Assets/_Game/Playtest/EXTERNAL_PLAYTEST_SURVEY_v17.txt",
            true,
            pass,
            warning,
            blocker
        );

        string gameplayScene =
            "Assets/_Game/Scenes/Prototype/Foundry_Prototype.unity";

        if (File.Exists(gameplayScene))
        {
            string text =
                File.ReadAllText(
                    gameplayScene
                );

            if (text.Contains(
                "ExternalPlaytest_v17"
            ))
            {
                pass.Add(
                    "Marker ExternalPlaytest_v17 présent."
                );
            }
            else
            {
                blocker.Add(
                    "Marker ExternalPlaytest_v17 absent : reconstruire la scène."
                );
            }
        }
        else
        {
            blocker.Add(
                "Foundry_Prototype.unity absent."
            );
        }

        string buildInfo =
            "Assets/_Game/Scripts/Demo/DemoBuildInfo.cs";

        if (File.Exists(buildInfo) &&
            File.ReadAllText(buildInfo)
                .Contains("Version = \"v17"))
        {
            pass.Add(
                "Version v17.x active."
            );
        }
        else
        {
            blocker.Add(
                "DemoBuildInfo n'indique pas une version v17.x."
            );
        }

        WriteReport(
            pass,
            warning,
            blocker,
            baseResult
        );

        AssetDatabase.Refresh();

        if (selectReport)
        {
            TextAsset asset =
                AssetDatabase
                    .LoadAssetAtPath<TextAsset>(
                        ReportPath
                    );

            if (asset != null)
            {
                Selection.activeObject =
                    asset;

                EditorGUIUtility
                    .PingObject(asset);
            }
        }

        return
            new Result
            {
                Pass =
                    pass.Count +
                    baseResult.PassCount,
                Warning =
                    warning.Count +
                    baseResult.WarningCount,
                Blocker =
                    blocker.Count
            };
    }

    private static void CheckAsset(
        string label,
        string path,
        bool critical,
        List<string> pass,
        List<string> warning,
        List<string> blocker
    )
    {
        Object asset =
            AssetDatabase
                .LoadMainAssetAtPath(
                    path
                );

        if (asset != null)
        {
            pass.Add(
                label + " présent."
            );
        }
        else if (critical)
        {
            blocker.Add(
                label +
                " manquant : " +
                path
            );
        }
        else
        {
            warning.Add(
                label +
                " manquant : " +
                path
            );
        }
    }

    private static void WriteReport(
        List<string> pass,
        List<string> warning,
        List<string> blocker,
        KatsuhiroV16QAValidator.ValidationResult baseResult
    )
    {
        if (!AssetDatabase.IsValidFolder(
            ReportFolder
        ))
        {
            AssetDatabase.CreateFolder(
                "Assets/_Game",
                "QA"
            );
        }

        StringBuilder builder =
            new StringBuilder();

        builder.AppendLine(
            "KATSUHIRO v17 — EXTERNAL PLAYTEST QA"
        );

        builder.AppendLine(
            "===================================="
        );

        builder.AppendLine(
            "Base v16.1 : PASS=" +
            baseResult.PassCount +
            " WARNING=" +
            baseResult.WarningCount +
            " BLOCKER=" +
            baseResult.BlockerCount
        );

        builder.AppendLine();
        builder.AppendLine("PASS");

        foreach (string item in pass)
            builder.AppendLine(
                "[PASS] " + item
            );

        builder.AppendLine();
        builder.AppendLine("WARNING");

        if (warning.Count == 0)
            builder.AppendLine(
                "[WARNING] Aucun."
            );
        else
        {
            foreach (string item in warning)
                builder.AppendLine(
                    "[WARNING] " + item
                );
        }

        builder.AppendLine();
        builder.AppendLine("BLOCKER");

        if (blocker.Count == 0)
            builder.AppendLine(
                "[BLOCKER] Aucun."
            );
        else
        {
            foreach (string item in blocker)
                builder.AppendLine(
                    "[BLOCKER] " + item
                );
        }

        File.WriteAllText(
            ReportPath,
            builder.ToString()
        );
    }
}

#endif
