using System.Collections;
using UnityEngine;

public class VerticalSliceDirector : MonoBehaviour
{
    public enum SliceStep
    {
        Movement,
        KikaiIntroduction,
        EtherealBridge,
        FirstCombat,
        MachineRoom,
        Checkpoint,
        Chase,
        Boss,
        Epilogue,
        Complete
    }

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private PlayerMotor25D playerMotor;
    [SerializeField] private KenjiroCombatController playerCombat;
    [SerializeField] private KikaiWorldManager worldManager;
    [SerializeField] private GameObject firstEnemy;
    [SerializeField] private VerticalSliceMachineRoom machineRoom;
    [SerializeField] private VerticalSliceChaseSequence chaseSequence;
    [SerializeField] private Doryoku3MiniBoss boss;

    [Header("Progression X")]
    [SerializeField] private float movementDoneX = -63f;
    [SerializeField] private float bridgeCrossedX = -45f;

    private SliceStep step = SliceStep.Movement;

    private float bannerTimer = 2.8f;
    private string bannerText = "FONDERIE KATSUHIRO — QUARTIER INDUSTRIEL DE TOKYO";

    private bool checkpointActivated;
    private bool chaseStarted;
    private bool chaseCompleted;
    private bool epilogueStarted;
    private bool blockPlayerControl;

    private int narrativeIndex = -1;
    private float narrativeTimer;

    private GUIStyle objectiveStyle;
    private GUIStyle hintStyle;
    private GUIStyle bannerStyle;
    private GUIStyle narrativeStyle;
    private GUIStyle endStyle;

    public SliceStep CurrentStep => step;
    public bool BlockPlayerControl => blockPlayerControl;

    private readonly string[] narrativeLines =
    {
        "Le Doryoku-3 s'effondre. La vapeur retombe lentement dans la fonderie.",
        "KIKAI-YŪREI : la signature éthérique correspond à celle des Forges Impériales.",
        "KENJIRO : « Ce n'était pas un accident... Quelqu'un ouvre de nouveau la porte. »"
    };

    private void Start()
    {
        if (player == null)
        {
            GameObject playerObject =
                GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
                player = playerObject.transform;
        }

        if (player != null)
        {
            if (playerMotor == null)
                playerMotor = player.GetComponent<PlayerMotor25D>();

            if (playerCombat == null)
                playerCombat = player.GetComponent<KenjiroCombatController>();
        }

        if (worldManager == null)
        {
            worldManager = KikaiWorldManager.Instance;

            if (worldManager == null)
                worldManager = FindAnyObjectByType<KikaiWorldManager>();
        }

        BindBoss();

        Vector3 restoredCheckpoint;

        if (DemoCheckpointPersistence.TryGetForCurrentScene(
            out restoredCheckpoint
        ))
        {
            checkpointActivated = true;
            step = SliceStep.Chase;
            bannerText =
                "POINT DE SYNCHRONISATION RESTAURÉ";
            bannerTimer = 2.2f;

            DemoPlaytestTelemetry
                .RecordPacingMilestone(
                    "CHECKPOINT"
                );
        }
        else
        {
            DemoPlaytestTelemetry
                .RecordPacingMilestone(
                    "LEVEL_START"
                );
        }
    }

    private void OnDestroy()
    {
        UnbindBoss();
    }

    public void BindBoss(Doryoku3MiniBoss targetBoss)
    {
        UnbindBoss();

        boss = targetBoss;

        if (boss != null)
            boss.BossDefeated += OnBossDefeated;
    }

    private void BindBoss()
    {
        if (boss == null)
            boss = FindAnyObjectByType<Doryoku3MiniBoss>();

        if (boss != null)
        {
            boss.BossDefeated -= OnBossDefeated;
            boss.BossDefeated += OnBossDefeated;
        }
    }

    private void UnbindBoss()
    {
        if (boss != null)
            boss.BossDefeated -= OnBossDefeated;
    }

    private void Update()
    {
        if (bannerTimer > 0f)
            bannerTimer -= Time.unscaledDeltaTime;

        if (player == null ||
            step == SliceStep.Epilogue ||
            step == SliceStep.Complete)
        {
            return;
        }

        float x = player.position.x;

        switch (step)
        {
            case SliceStep.Movement:
                if (x >= movementDoneX)
                    SetStep(SliceStep.KikaiIntroduction);
                break;

            case SliceStep.KikaiIntroduction:
                if (worldManager != null &&
                    worldManager.IsEthereal)
                {
                    SetStep(SliceStep.EtherealBridge);
                }
                break;

            case SliceStep.EtherealBridge:
                if (x >= bridgeCrossedX)
                    SetStep(SliceStep.FirstCombat);
                break;

            case SliceStep.FirstCombat:
                if (firstEnemy == null)
                {
                    SetStep(SliceStep.MachineRoom);
                }
                break;

            case SliceStep.MachineRoom:
                if (machineRoom != null &&
                    machineRoom.Completed)
                {
                    SetStep(SliceStep.Checkpoint);
                }
                break;

            case SliceStep.Checkpoint:
                if (checkpointActivated)
                    SetStep(SliceStep.Chase);
                break;

            case SliceStep.Chase:
                if (chaseCompleted ||
                    (chaseSequence != null &&
                     chaseSequence.Completed))
                {
                    SetStep(SliceStep.Boss);
                }
                break;

            case SliceStep.Boss:
                // Boss encounter is handled by Doryoku3BossEncounter.
                break;
        }
    }

    private void SetStep(SliceStep next)
    {
        step = next;
        bannerTimer = 1.6f;

        switch (step)
        {
            case SliceStep.KikaiIntroduction:
                bannerText = "ANOMALIE ÉTHÉRIQUE DÉTECTÉE";

                DemoPlaytestTelemetry
                    .RecordPacingMilestone(
                        "KIKAI_INTRO"
                    );
                break;

            case SliceStep.EtherealBridge:
                bannerText = "LE KIKAI-YŪREI RÉVÈLE CE QUE L'ŒIL NE PEUT VOIR";

                DemoPlaytestTelemetry
                    .RecordPacingMilestone(
                        "KIKAI_ACTIVATED"
                    );
                break;

            case SliceStep.FirstCombat:
                bannerText = "DORYOKU-3 — SIGNATURE HOSTILE";

                DemoPlaytestTelemetry
                    .RecordPacingMilestone(
                        "BRIDGE_COMPLETE"
                    );

                DemoPlaytestTelemetry
                    .RecordPacingMilestone(
                        "FIRST_COMBAT_START"
                    );
                break;

            case SliceStep.MachineRoom:
                bannerText = "CHAÎNE 4 — CORRUPTION MULTIPLE";

                DemoPlaytestTelemetry
                    .RecordPacingMilestone(
                        "FIRST_COMBAT_COMPLETE"
                    );

                DemoPlaytestTelemetry
                    .RecordPacingMilestone(
                        "MACHINE_ROOM_START"
                    );
                break;

            case SliceStep.Checkpoint:
                bannerText = "STABILISEZ VOTRE POSITION";
                break;
            case SliceStep.Chase:
                bannerText = "ALERTE — ACTIVITÉ MÉCANIQUE MASSIVE";
                break;
            case SliceStep.Boss:
                bannerText = "SOURCE DE LA CORRUPTION À PROXIMITÉ";
                break;
        }
    }

    public void NotifyMachineRoomCompleted()
    {
        DemoPlaytestTelemetry
            .RecordPacingMilestone(
                "MACHINE_ROOM_COMPLETE"
            );

        bannerText =
            "CHAÎNE 4 STABILISÉE — BALISE À PROXIMITÉ";

        bannerTimer =
            2.0f;

        if (step == SliceStep.MachineRoom)
            SetStep(SliceStep.Checkpoint);
    }

    public void NotifyCheckpointActivated()
    {
        checkpointActivated = true;

        DemoPlaytestTelemetry
            .RecordPacingMilestone(
                "CHECKPOINT"
            );

        bannerText = "POINT DE SYNCHRONISATION ÉTABLI";
        bannerTimer = 2.0f;

        if (step == SliceStep.Checkpoint)
            SetStep(SliceStep.Chase);
    }

    public void NotifyChaseStarted()
    {
        chaseStarted = true;

        DemoPlaytestTelemetry
            .RecordPacingMilestone(
                "CHASE_START"
            );

        bannerText = "COUREZ !";
        bannerTimer = 1.8f;
    }

    public void NotifyChaseCompleted()
    {
        chaseCompleted = true;

        DemoPlaytestTelemetry
            .RecordPacingMilestone(
                "CHASE_COMPLETE"
            );

        DemoPlaytestTelemetry
            .RecordPacingMilestone(
                "BOSS_APPROACH"
            );

        bannerText = "VOIE DÉGAGÉE — CONTINUEZ";
        bannerTimer = 1.8f;

        if (step == SliceStep.Chase)
            SetStep(SliceStep.Boss);
    }

    private void OnBossDefeated()
    {
        if (epilogueStarted)
            return;

        epilogueStarted = true;

        DemoPlaytestTelemetry
            .RecordPacingMilestone(
                "BOSS_DEFEATED"
            );

        step = SliceStep.Epilogue;
        StartCoroutine(EpilogueRoutine());
    }

    private IEnumerator EpilogueRoutine()
    {
        blockPlayerControl = true;

        if (playerMotor != null)
            playerMotor.enabled = false;

        if (playerCombat != null)
            playerCombat.enabled = false;

        yield return new WaitForSecondsRealtime(0.85f);

        for (int i = 0; i < narrativeLines.Length; i++)
        {
            narrativeIndex = i;
            narrativeTimer = 2.65f;

            while (narrativeTimer > 0f)
            {
                narrativeTimer -= Time.unscaledDeltaTime;
                yield return null;
            }
        }

        narrativeIndex = -1;
        step = SliceStep.Complete;
        bannerText = "FIN DE LA VERTICAL SLICE";
        bannerTimer = 999f;

        DemoPlaytestTelemetry
            .RecordPacingMilestone(
                "DEMO_COMPLETE"
            );

        DemoPlaytestTelemetry.RecordDemoCompleted();
    }

    private string GetObjective()
    {
        switch (step)
        {
            case SliceStep.Movement:
                return "OBJECTIF : avancez dans la fonderie.";
            case SliceStep.KikaiIntroduction:
                return "OBJECTIF : activez le Kikai-Yūrei avec K.";
            case SliceStep.EtherealBridge:
                return "OBJECTIF : traversez le passage révélé dans le monde éthérique.";
            case SliceStep.FirstCombat:
                return "OBJECTIF : neutralisez le Doryoku-3 possédé.";
            case SliceStep.MachineRoom:
                return "OBJECTIF : synchronisez les trois relais de la chaîne 4 et neutralisez l'automate réveillé.";
            case SliceStep.Checkpoint:
                return "OBJECTIF : atteignez la balise de synchronisation.";
            case SliceStep.Chase:
                return chaseStarted
                    ? "OBJECTIF : échappez à l'automate — ne vous arrêtez pas."
                    : "OBJECTIF : avancez dans le corridor de production.";
            case SliceStep.Boss:
                return "OBJECTIF : trouvez et détruisez la source de la corruption.";
            case SliceStep.Epilogue:
                return "";
            case SliceStep.Complete:
                return "Chapitre 1 — Les Murmures de l'Acier";
        }

        return "";
    }

    private string GetHint()
    {
        switch (step)
        {
            case SliceStep.Movement:
                return "A / D : déplacement    •    Espace : saut";
            case SliceStep.KikaiIntroduction:
                return "K : monde normal ↔ monde éthérique";
            case SliceStep.EtherealBridge:
                return "Les plateformes cyan n'existent que lorsque le Kikai-Yūrei est actif.";
            case SliceStep.FirstCombat:
                return "J : combo    •    I : lourd    •    Shift : esquive    •    K + L : spéciale";
            case SliceStep.MachineRoom:
                return "Relais cyan : monde éthérique • relais cuivre : monde normal • alternez avec K.";
            case SliceStep.Checkpoint:
                return "La balise cyan devient votre nouveau point de réapparition.";
            case SliceStep.Chase:
                return "Utilisez saut et esquive pour conserver votre vitesse.";
            case SliceStep.Boss:
                return "Observez ses télégraphies. En phase II, l'éther devient plus dangereux.";
        }

        return "";
    }

    private void EnsureStyles()
    {
        if (objectiveStyle != null)
            return;

        objectiveStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 16,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter,
            wordWrap = true
        };

        hintStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 12,
            alignment = TextAnchor.MiddleCenter,
            wordWrap = true
        };

        bannerStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 22,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter
        };

        narrativeStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 19,
            alignment = TextAnchor.MiddleCenter,
            wordWrap = true
        };

        endStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 30,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter
        };
    }

    private void OnGUI()
    {
        EnsureStyles();

        if (bannerTimer > 0f)
        {
            float width =
                Mathf.Min(Screen.width * 0.78f, 900f);

            float x = (Screen.width - width) * 0.5f;

            GUI.Box(
                new Rect(x, 118f, width, 42f),
                ""
            );

            GUI.Label(
                new Rect(x + 10f, 121f, width - 20f, 36f),
                bannerText,
                bannerStyle
            );
        }

        if (step != SliceStep.Epilogue &&
            step != SliceStep.Complete)
        {
            float width =
                Mathf.Min(Screen.width * 0.72f, 760f);

            float x =
                (Screen.width - width) * 0.5f;

            float y =
                Screen.height - 118f;

            GUI.Box(
                new Rect(x, y, width, 88f),
                ""
            );

            GUI.Label(
                new Rect(x + 12f, y + 8f, width - 24f, 30f),
                GetObjective(),
                objectiveStyle
            );

            GUI.Label(
                new Rect(x + 12f, y + 42f, width - 24f, 34f),
                GetHint(),
                hintStyle
            );
        }

        if (step == SliceStep.Epilogue &&
            narrativeIndex >= 0)
        {
            GUI.Box(
                new Rect(
                    0f,
                    Screen.height * 0.66f,
                    Screen.width,
                    Screen.height * 0.34f
                ),
                ""
            );

            GUI.Label(
                new Rect(
                    Screen.width * 0.10f,
                    Screen.height * 0.72f,
                    Screen.width * 0.80f,
                    100f
                ),
                narrativeLines[narrativeIndex],
                narrativeStyle
            );
        }

        if (step == SliceStep.Complete)
        {
            GUI.Box(
                new Rect(0f, 0f, Screen.width, Screen.height),
                ""
            );

            GUI.Label(
                new Rect(
                    0f,
                    Screen.height * 0.36f,
                    Screen.width,
                    60f
                ),
                "FIN DE LA VERTICAL SLICE",
                endStyle
            );

            GUI.Label(
                new Rect(
                    0f,
                    Screen.height * 0.46f,
                    Screen.width,
                    40f
                ),
                "Chapitre 1 — Les Murmures de l'Acier",
                objectiveStyle
            );

            GUI.Label(
                new Rect(
                    0f,
                    Screen.height * 0.54f,
                    Screen.width,
                    34f
                ),
                "Fonderie Katsuhiro • Tokyo • 1889",
                hintStyle
            );

            GUI.Label(
                new Rect(
                    0f,
                    Screen.height * 0.60f,
                    Screen.width,
                    28f
                ),
                DemoBuildInfo.FullLabel,
                hintStyle
            );

            if (GUI.Button(
                new Rect(
                    Screen.width * 0.5f - 120f,
                    Screen.height * 0.68f,
                    240f,
                    46f
                ),
                "Retour au titre"
            ))
            {
                DemoSceneLoader.Load(
                    "TitleScreen"
                );
            }
        }
    }
}
