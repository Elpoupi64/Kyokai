#if UNITY_EDITOR

using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public static class KatsuhiroV171PacingQAValidator
{
    private const string ReportFolder =
        "Assets/_Game/QA";

    private const string ReportPath =
        ReportFolder +
        "/v17_1_PACING_QA_REPORT.txt";

    public struct Result
    {
        public int Pass;
        public int Warning;
        public int Blocker;

        public bool CanBuild =>
            Blocker == 0;
    }

    [MenuItem("Tools/Katsuhiro/Run v17.1 Pacing QA")]
    public static void RunMenu()
    {
        Run(true);
    }

    public static Result Run(
        bool selectReport
    )
    {
        KatsuhiroV17PlaytestQAValidator.Result baseQA =
            KatsuhiroV17PlaytestQAValidator
                .Run(false);

        List<string> pass =
            new List<string>();

        List<string> warning =
            new List<string>();

        List<string> blocker =
            new List<string>();

        if (!baseQA.CanBuild)
        {
            blocker.Add(
                "QA v17 contient " +
                baseQA.Blocker +
                " blocker(s)."
            );
        }
        else
        {
            pass.Add(
                "QA v17 sans blocker."
            );
        }

        CheckFile(
            "Machine room controller",
            "Assets/_Game/Scripts/VerticalSlice/VerticalSliceMachineRoom.cs",
            true,
            pass,
            warning,
            blocker
        );

        CheckFile(
            "Kikai relay node",
            "Assets/_Game/Scripts/VerticalSlice/VerticalSliceKikaiRelayNode.cs",
            true,
            pass,
            warning,
            blocker
        );

        CheckFile(
            "Steam vent hazard",
            "Assets/_Game/Scripts/VerticalSlice/VerticalSliceSteamVent.cs",
            true,
            pass,
            warning,
            blocker
        );

        CheckFile(
            "Level pacing builder",
            "Assets/_Game/Editor/KatsuhiroV171LevelPacingBuilder.cs",
            true,
            pass,
            warning,
            blocker
        );

        string scene =
            "Assets/_Game/Scenes/Prototype/Foundry_Prototype.unity";

        if (File.Exists(scene))
        {
            string text =
                File.ReadAllText(scene);

            string[] markers =
            {
                "LevelPacing_v17_1",
                "04_Chain4_Possessed_v17_1",
                "ChaseCourse_v17_1"
            };

            for (int i = 0; i < markers.Length; i++)
            {
                if (text.Contains(markers[i]))
                    pass.Add("Marker : " + markers[i]);
                else
                    blocker.Add(
                        "Marker absent : " +
                        markers[i] +
                        " — reconstruire la scène."
                    );
            }
        }
        else
        {
            blocker.Add(
                "Foundry_Prototype.unity absent."
            );
        }

        string director =
            "Assets/_Game/Scripts/VerticalSlice/VerticalSliceDirector.cs";

        CheckContract(
            director,
            "SliceStep.MachineRoom",
            "Director : étape MachineRoom",
            pass,
            blocker
        );

        CheckContract(
            director,
            "MACHINE_ROOM_COMPLETE",
            "Director : milestone MachineRoom",
            pass,
            blocker
        );

        string telemetry =
            "Assets/_Game/Scripts/Demo/DemoPlaytestTelemetry.cs";

        string[] pacingTokens =
        {
            "PACING PAR SECTION",
            "LEVEL_START",
            "KIKAI_INTRO",
            "KIKAI_ACTIVATED",
            "BRIDGE_COMPLETE",
            "FIRST_COMBAT_START",
            "FIRST_COMBAT_COMPLETE",
            "MACHINE_ROOM_START",
            "MACHINE_ROOM_COMPLETE",
            "CHECKPOINT",
            "CHASE_START",
            "CHASE_COMPLETE",
            "BOSS_START",
            "BOSS_DEFEATED",
            "DEMO_COMPLETE",
            "480f",
            "600f"
        };

        for (int i = 0; i < pacingTokens.Length; i++)
        {
            CheckContract(
                telemetry,
                pacingTokens[i],
                "Telemetry : " +
                pacingTokens[i],
                pass,
                blocker
            );
        }

        WriteReport(
            pass,
            warning,
            blocker,
            baseQA
        );

        AssetDatabase.Refresh();

        if (selectReport)
        {
            TextAsset asset =
                AssetDatabase.LoadAssetAtPath<TextAsset>(
                    ReportPath
                );

            if (asset != null)
            {
                Selection.activeObject =
                    asset;

                EditorGUIUtility.PingObject(
                    asset
                );
            }
        }

        return new Result
        {
            Pass =
                pass.Count +
                baseQA.Pass,
            Warning =
                warning.Count +
                baseQA.Warning,
            Blocker =
                blocker.Count
        };
    }

    private static void CheckFile(
        string label,
        string path,
        bool critical,
        List<string> pass,
        List<string> warning,
        List<string> blocker
    )
    {
        Object asset =
            AssetDatabase.LoadMainAssetAtPath(
                path
            );

        if (asset != null)
            pass.Add(label + " présent.");
        else if (critical)
            blocker.Add(label + " absent : " + path);
        else
            warning.Add(label + " absent : " + path);
    }

    private static void CheckContract(
        string path,
        string token,
        string label,
        List<string> pass,
        List<string> blocker
    )
    {
        if (!File.Exists(path))
        {
            blocker.Add(
                label +
                " — fichier absent."
            );

            return;
        }

        string text =
            File.ReadAllText(path);

        if (text.Contains(token))
            pass.Add(label);
        else
            blocker.Add(
                label +
                " — contrat absent."
            );
    }

    private static void WriteReport(
        List<string> pass,
        List<string> warning,
        List<string> blocker,
        KatsuhiroV17PlaytestQAValidator.Result baseQA
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

        StringBuilder report =
            new StringBuilder();

        report.AppendLine(
            "KATSUHIRO v17.1 — LEVEL PACING QA"
        );

        report.AppendLine(
            "================================="
        );

        report.AppendLine(
            "Objectif durée : 08:00–10:00"
        );

        report.AppendLine(
            "Base v17 : PASS=" +
            baseQA.Pass +
            " WARNING=" +
            baseQA.Warning +
            " BLOCKER=" +
            baseQA.Blocker
        );

        report.AppendLine();

        report.AppendLine("PASS");
        foreach (string item in pass)
            report.AppendLine("[PASS] " + item);

        report.AppendLine();
        report.AppendLine("WARNING");

        if (warning.Count == 0)
            report.AppendLine("[WARNING] Aucun.");
        else
        {
            foreach (string item in warning)
                report.AppendLine("[WARNING] " + item);
        }

        report.AppendLine();
        report.AppendLine("BLOCKER");

        if (blocker.Count == 0)
            report.AppendLine("[BLOCKER] Aucun.");
        else
        {
            foreach (string item in blocker)
                report.AppendLine("[BLOCKER] " + item);
        }

        report.AppendLine();
        report.AppendLine(
            "La durée réelle ne peut être validée qu'en Play Mode / build externe."
        );

        File.WriteAllText(
            ReportPath,
            report.ToString()
        );
    }
}

#endif
