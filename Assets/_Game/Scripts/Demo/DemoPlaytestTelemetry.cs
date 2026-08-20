using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DemoPlaytestTelemetry : MonoBehaviour
{
    [Serializable]
    public class PlaytestEvent
    {
        public string type;
        public float sessionSeconds;
        public string scene;
        public string detail;
    }

    [Serializable]
    public class PacingStamp
    {
        public string milestone;
        public float runSeconds;
    }

    [Serializable]
    public class SessionData
    {
        public string version;
        public string sessionId;
        public string startedUtc;
        public string endedUtc;
        public string runType;
        public string qualityPreset;

        public float sessionSeconds;
        public float runSeconds;

        public int sceneLoads;
        public int deaths;
        public int respawns;
        public int checkpoints;
        public int bossAttempts;
        public int bossDefeats;
        public int demoCompletions;
        public int kikaiToggles;

        public List<PlaytestEvent> events =
            new List<PlaytestEvent>();

        public List<PacingStamp> pacing =
            new List<PacingStamp>();
    }

    private static DemoPlaytestTelemetry instance;

    private SessionData data;
    private float sessionStartRealtime;
    private float runStartRealtime = -1f;

    private string latestJsonPath = string.Empty;
    private string latestSummaryPath = string.Empty;

    private KikaiWorldManager boundWorld;

    public static DemoPlaytestTelemetry Instance =>
        instance;

    public static string LatestReportPath =>
        instance != null
            ? instance.latestJsonPath
            : string.Empty;

    public static string LatestSummaryPath =>
        instance != null
            ? instance.latestSummaryPath
            : string.Empty;

    public static SessionData CurrentData =>
        instance != null
            ? instance.data
            : null;

    public static DemoPlaytestTelemetry EnsureInstance()
    {
        if (instance != null)
            return instance;

        DemoPlaytestTelemetry existing =
            FindAnyObjectByType<DemoPlaytestTelemetry>();

        if (existing != null)
            return existing;

        GameObject root =
            new GameObject("DemoPlaytestTelemetry_v17_1");

        return root.AddComponent<DemoPlaytestTelemetry>();
    }

    private void Awake()
    {
        if (instance != null &&
            instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        BeginNewSession();

        SceneManager.sceneLoaded +=
            OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (instance != this)
            return;

        SceneManager.sceneLoaded -=
            OnSceneLoaded;

        UnbindSceneEvents();

        SaveReports("OnDestroy");

        instance = null;
    }

    private void OnApplicationQuit()
    {
        SaveReports("ApplicationQuit");
    }

    private void BeginNewSession()
    {
        sessionStartRealtime =
            Time.realtimeSinceStartup;

        data =
            new SessionData
            {
                version =
                    DemoBuildInfo.FullLabel,
                sessionId =
                    DateTime.UtcNow
                        .ToString("yyyyMMdd_HHmmss"),
                startedUtc =
                    DateTime.UtcNow
                        .ToString("o"),
                runType =
                    "NotStarted",
                qualityPreset =
                    DemoQualityManager
                        .CurrentPresetLabel
            };

        AddEvent(
            "SessionStarted",
            data.version
        );

        SaveReports("SessionStarted");
    }

    public static void BeginRun(
        string runType
    )
    {
        DemoPlaytestTelemetry telemetry =
            EnsureInstance();

        telemetry.runStartRealtime =
            Time.realtimeSinceStartup;

        telemetry.data.runType =
            string.IsNullOrWhiteSpace(runType)
                ? "Unknown"
                : runType;

        telemetry.data.pacing.Clear();

        telemetry.AddEvent(
            "RunStarted",
            telemetry.data.runType
        );

        telemetry.SaveReports(
            "RunStarted"
        );
    }

    public static void RecordPacingMilestone(
        string milestone
    )
    {
        if (string.IsNullOrWhiteSpace(milestone))
            return;

        DemoPlaytestTelemetry telemetry =
            EnsureInstance();

        if (telemetry.HasPacingStamp(milestone))
            return;

        float seconds =
            telemetry.GetRunSeconds();

        telemetry.data.pacing.Add(
            new PacingStamp
            {
                milestone = milestone,
                runSeconds = seconds
            }
        );

        telemetry.AddEvent(
            "PacingMilestone",
            milestone
        );

        telemetry.SaveReports(
            "Pacing_" + milestone
        );
    }

    public static void RecordSceneLoadRequested(
        string sceneName
    )
    {
        DemoPlaytestTelemetry telemetry =
            EnsureInstance();

        telemetry.AddEvent(
            "SceneLoadRequested",
            sceneName
        );
    }

    public static void RecordCheckpoint(
        Vector3 position
    )
    {
        DemoPlaytestTelemetry telemetry =
            EnsureInstance();

        telemetry.data.checkpoints++;

        telemetry.AddEvent(
            "Checkpoint",
            position.ToString("F2")
        );

        telemetry.SaveReports(
            "Checkpoint"
        );
    }

    public static void RecordDeath()
    {
        DemoPlaytestTelemetry telemetry =
            EnsureInstance();

        telemetry.data.deaths++;

        telemetry.AddEvent(
            "PlayerDeath",
            string.Empty
        );

        telemetry.SaveReports(
            "PlayerDeath"
        );
    }

    public static void RecordRespawn()
    {
        DemoPlaytestTelemetry telemetry =
            EnsureInstance();

        telemetry.data.respawns++;

        telemetry.AddEvent(
            "PlayerRespawn",
            string.Empty
        );
    }

    public static void RecordBossAttempt()
    {
        DemoPlaytestTelemetry telemetry =
            EnsureInstance();

        telemetry.data.bossAttempts++;

        telemetry.AddEvent(
            "BossAttempt",
            "Doryoku-3 Unit 07"
        );

        telemetry.SaveReports(
            "BossAttempt"
        );
    }

    public static void RecordBossDefeated()
    {
        DemoPlaytestTelemetry telemetry =
            EnsureInstance();

        telemetry.data.bossDefeats++;

        telemetry.AddEvent(
            "BossDefeated",
            "Doryoku-3 Unit 07"
        );

        telemetry.SaveReports(
            "BossDefeated"
        );
    }

    public static void RecordDemoCompleted()
    {
        DemoPlaytestTelemetry telemetry =
            EnsureInstance();

        telemetry.data.demoCompletions++;

        telemetry.AddEvent(
            "DemoCompleted",
            "Chapitre 1 — Les Murmures de l'Acier"
        );

        telemetry.SaveReports(
            "DemoCompleted"
        );
    }

    public static void RecordQualityPreset(
        string preset
    )
    {
        DemoPlaytestTelemetry telemetry =
            EnsureInstance();

        telemetry.data.qualityPreset =
            preset;

        telemetry.AddEvent(
            "QualityPreset",
            preset
        );
    }

    private void OnSceneLoaded(
        Scene scene,
        LoadSceneMode mode
    )
    {
        data.sceneLoads++;

        AddEvent(
            "SceneLoaded",
            scene.name
        );

        BindSceneEvents();

        SaveReports(
            "SceneLoaded"
        );
    }

    private void BindSceneEvents()
    {
        UnbindSceneEvents();

        boundWorld =
            FindAnyObjectByType<KikaiWorldManager>();

        if (boundWorld != null)
        {
            boundWorld.ModeChanged +=
                OnWorldModeChanged;
        }
    }

    private void UnbindSceneEvents()
    {
        if (boundWorld != null)
        {
            boundWorld.ModeChanged -=
                OnWorldModeChanged;
        }

        boundWorld = null;
    }

    private void OnWorldModeChanged(
        KikaiWorldMode mode
    )
    {
        data.kikaiToggles++;

        AddEvent(
            "KikaiWorldMode",
            mode.ToString()
        );
    }

    private void AddEvent(
        string type,
        string detail
    )
    {
        if (data == null)
            return;

        PlaytestEvent entry =
            new PlaytestEvent
            {
                type = type,
                sessionSeconds =
                    Mathf.Max(
                        0f,
                        Time.realtimeSinceStartup -
                        sessionStartRealtime
                    ),
                scene =
                    SceneManager
                        .GetActiveScene()
                        .name,
                detail =
                    detail ?? string.Empty
            };

        data.events.Add(entry);
    }

    private bool HasPacingStamp(
        string milestone
    )
    {
        if (data == null ||
            data.pacing == null)
        {
            return false;
        }

        for (
            int i = 0;
            i < data.pacing.Count;
            i++
        )
        {
            if (data.pacing[i] != null &&
                data.pacing[i].milestone ==
                milestone)
            {
                return true;
            }
        }

        return false;
    }

    private float GetRunSeconds()
    {
        if (runStartRealtime < 0f)
            return 0f;

        return Mathf.Max(
            0f,
            Time.realtimeSinceStartup -
            runStartRealtime
        );
    }

    private bool TryGetPacingTime(
        string milestone,
        out float seconds
    )
    {
        seconds = 0f;

        if (data == null ||
            data.pacing == null)
        {
            return false;
        }

        for (
            int i = 0;
            i < data.pacing.Count;
            i++
        )
        {
            PacingStamp stamp =
                data.pacing[i];

            if (stamp != null &&
                stamp.milestone ==
                milestone)
            {
                seconds =
                    stamp.runSeconds;

                return true;
            }
        }

        return false;
    }

    public static string GetCompactSummary()
    {
        if (instance == null ||
            instance.data == null)
        {
            return "Playtest : aucune session.";
        }

        SessionData d =
            instance.data;

        return
            "Temps : " +
            FormatDuration(
                instance.GetRunSeconds()
            ) +
            "\nMorts : " +
            d.deaths +
            "   Respawns : " +
            d.respawns +
            "\nCheckpoints : " +
            d.checkpoints +
            "   Boss : " +
            d.bossAttempts +
            "/" +
            d.bossDefeats +
            "\nKikai : " +
            d.kikaiToggles +
            "   Fin : " +
            d.demoCompletions;
    }

    public static string GetPacingCompactSummary()
    {
        if (instance == null ||
            instance.data == null)
        {
            return "Pacing : aucune session.";
        }

        string current =
            instance.GetLatestMilestone();

        return
            "Cible niveau : 08:00–10:00" +
            "\nTemps actuel : " +
            FormatDuration(
                instance.GetRunSeconds()
            ) +
            "\nDernier jalon : " +
            (
                string.IsNullOrEmpty(current)
                    ? "—"
                    : current
            );
    }

    private string GetLatestMilestone()
    {
        if (data == null ||
            data.pacing == null ||
            data.pacing.Count == 0)
        {
            return string.Empty;
        }

        PacingStamp stamp =
            data.pacing[
                data.pacing.Count - 1
            ];

        return stamp != null
            ? stamp.milestone
            : string.Empty;
    }

    private static string FormatDuration(
        float seconds
    )
    {
        int total =
            Mathf.Max(
                0,
                Mathf.RoundToInt(seconds)
            );

        int minutes =
            total / 60;

        int remaining =
            total % 60;

        return
            minutes.ToString("00") +
            ":" +
            remaining.ToString("00");
    }

    private void SaveReports(
        string reason
    )
    {
        if (data == null)
            return;

        try
        {
            data.sessionSeconds =
                Mathf.Max(
                    0f,
                    Time.realtimeSinceStartup -
                    sessionStartRealtime
                );

            data.runSeconds =
                GetRunSeconds();

            data.endedUtc =
                DateTime.UtcNow
                    .ToString("o");

            data.qualityPreset =
                DemoQualityManager
                    .CurrentPresetLabel;

            string folder =
                Path.Combine(
                    Application.persistentDataPath,
                    "Katsuhiro_Playtest"
                );

            Directory.CreateDirectory(
                folder
            );

            latestJsonPath =
                Path.Combine(
                    folder,
                    "v17_1_pacing_" +
                    data.sessionId +
                    ".json"
                );

            latestSummaryPath =
                Path.Combine(
                    folder,
                    "v17_1_pacing_" +
                    data.sessionId +
                    "_summary.txt"
                );

            File.WriteAllText(
                latestJsonPath,
                JsonUtility.ToJson(
                    data,
                    true
                )
            );

            File.WriteAllText(
                latestSummaryPath,
                BuildSummary(reason)
            );
        }
        catch (Exception exception)
        {
            Debug.LogWarning(
                "Playtest telemetry : impossible d'écrire le rapport. " +
                exception.Message
            );
        }
    }

    private string BuildSummary(
        string reason
    )
    {
        StringBuilder builder =
            new StringBuilder();

        builder.AppendLine(
            "KATSUHIRO v17.1 — LEVEL PACING 8–10 MINUTES"
        );

        builder.AppendLine(
            "============================================"
        );

        builder.AppendLine(
            "Version : " +
            data.version
        );

        builder.AppendLine(
            "Session : " +
            data.sessionId
        );

        builder.AppendLine(
            "Run : " +
            data.runType
        );

        builder.AppendLine(
            "Qualité : " +
            data.qualityPreset
        );

        builder.AppendLine(
            "Durée run : " +
            FormatDuration(
                GetRunSeconds()
            )
        );

        builder.AppendLine(
            "Objectif : 08:00–10:00"
        );

        builder.AppendLine();

        builder.AppendLine(
            "PACING PAR SECTION"
        );

        builder.AppendLine(
            "------------------"
        );

        AppendSection(
            builder,
            "Tutoriel",
            "LEVEL_START",
            "KIKAI_INTRO",
            35f,
            55f
        );

        AppendSection(
            builder,
            "Première anomalie",
            "KIKAI_INTRO",
            "KIKAI_ACTIVATED",
            45f,
            75f
        );

        AppendSection(
            builder,
            "Pont spectral",
            "KIKAI_ACTIVATED",
            "BRIDGE_COMPLETE",
            45f,
            75f
        );

        AppendSection(
            builder,
            "Premier combat",
            "FIRST_COMBAT_START",
            "FIRST_COMBAT_COMPLETE",
            55f,
            90f
        );

        AppendSection(
            builder,
            "Chaîne 4 possédée",
            "MACHINE_ROOM_START",
            "MACHINE_ROOM_COMPLETE",
            60f,
            90f
        );

        AppendSection(
            builder,
            "Checkpoint / transition",
            "MACHINE_ROOM_COMPLETE",
            "CHASE_START",
            15f,
            35f
        );

        AppendSection(
            builder,
            "Reprise depuis checkpoint",
            "CHECKPOINT",
            "CHASE_START",
            5f,
            20f
        );

        AppendSection(
            builder,
            "Poursuite",
            "CHASE_START",
            "CHASE_COMPLETE",
            35f,
            70f
        );

        AppendSection(
            builder,
            "Approche arène",
            "CHASE_COMPLETE",
            "BOSS_START",
            15f,
            35f
        );

        AppendSection(
            builder,
            "Unité 07",
            "BOSS_START",
            "BOSS_DEFEATED",
            105f,
            135f
        );

        AppendSection(
            builder,
            "Épilogue",
            "BOSS_DEFEATED",
            "DEMO_COMPLETE",
            25f,
            50f
        );

        builder.AppendLine();

        float levelStart;
        float demoComplete;

        if (TryGetPacingTime(
                "LEVEL_START",
                out levelStart
            ) &&
            TryGetPacingTime(
                "DEMO_COMPLETE",
                out demoComplete
            ))
        {
            float total =
                Mathf.Max(
                    0f,
                    demoComplete -
                    levelStart
                );

            builder.AppendLine(
                "TOTAL : " +
                FormatDuration(total) +
                "  " +
                GetTargetStatus(
                    total,
                    480f,
                    600f
                )
            );
        }
        else
        {
            builder.AppendLine(
                "TOTAL : en cours"
            );
        }

        builder.AppendLine();
        builder.AppendLine(
            "Morts : " +
            data.deaths
        );

        builder.AppendLine(
            "Respawns : " +
            data.respawns
        );

        builder.AppendLine(
            "Checkpoints : " +
            data.checkpoints
        );

        builder.AppendLine(
            "Boss attempts : " +
            data.bossAttempts
        );

        builder.AppendLine(
            "Boss defeats : " +
            data.bossDefeats
        );

        builder.AppendLine(
            "Kikai toggles : " +
            data.kikaiToggles
        );

        builder.AppendLine(
            "Demo completions : " +
            data.demoCompletions
        );

        builder.AppendLine(
            "Dernière sauvegarde : " +
            reason
        );

        builder.AppendLine();
        builder.AppendLine(
            "Aucune donnée n'est envoyée sur Internet."
        );

        builder.AppendLine(
            "Les fichiers restent dans Application.persistentDataPath/Katsuhiro_Playtest/."
        );

        return builder.ToString();
    }

    private void AppendSection(
        StringBuilder builder,
        string label,
        string startMilestone,
        string endMilestone,
        float targetMin,
        float targetMax
    )
    {
        float start;
        float end;

        if (TryGetPacingTime(
                startMilestone,
                out start
            ) &&
            TryGetPacingTime(
                endMilestone,
                out end
            ))
        {
            float duration =
                Mathf.Max(
                    0f,
                    end - start
                );

            builder.AppendLine(
                label.PadRight(25) +
                FormatDuration(duration) +
                "  " +
                GetTargetStatus(
                    duration,
                    targetMin,
                    targetMax
                )
            );
        }
        else
        {
            builder.AppendLine(
                label.PadRight(25) +
                "--:--"
            );
        }
    }

    private static string GetTargetStatus(
        float duration,
        float targetMin,
        float targetMax
    )
    {
        if (duration < targetMin)
            return "[RAPIDE]";

        if (duration > targetMax)
            return "[LONG]";

        return "[CIBLE]";
    }
}
