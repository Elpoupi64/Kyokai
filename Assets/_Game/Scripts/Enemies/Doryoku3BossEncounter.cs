using System.Collections;
using UnityEngine;

public class Doryoku3BossEncounter : MonoBehaviour
{
    [Header("Encounter")]
    [SerializeField] private Doryoku3MiniBoss boss;
    [SerializeField] private Transform player;
    [SerializeField] private float activationDistance = 8.5f;
    [SerializeField] private float introDuration = 2.0f;

    [Header("Player")]
    [SerializeField] private PlayerMotor25D playerMotor;
    [SerializeField] private KenjiroCombatController playerCombat;
    [SerializeField] private PlayerAttackPrototype playerAttack;
    [SerializeField] private PlayerHealth playerHealth;

    [Header("Presentation")]
    [SerializeField] private CameraFollow25D bossCamera;
    [SerializeField] private Doryoku3BossHUD bossHUD;

    [Header("Arena Gates")]
    [SerializeField] private GameObject leftGate;
    [SerializeField] private GameObject rightGate;

    private bool started;
    private bool completed;

    private void Awake()
    {
        SetGates(false);

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

            if (playerAttack == null)
                playerAttack = player.GetComponent<PlayerAttackPrototype>();

            if (playerHealth == null)
                playerHealth = player.GetComponent<PlayerHealth>();
        }

        if (bossCamera == null &&
            Camera.main != null)
        {
            bossCamera =
                Camera.main.GetComponent<CameraFollow25D>();
        }
    }

    private void OnEnable()
    {
        if (boss != null)
            boss.BossDefeated += OnBossDefeated;
    }

    private void OnDisable()
    {
        if (boss != null)
            boss.BossDefeated -= OnBossDefeated;
    }

    private void Update()
    {
        if (started ||
            completed ||
            boss == null ||
            player == null)
        {
            return;
        }

        float distance =
            Mathf.Abs(
                player.position.x -
                boss.transform.position.x
            );

        if (distance <= activationDistance)
        {
            StartCoroutine(
                BeginEncounterRoutine()
            );
        }
    }

    private IEnumerator BeginEncounterRoutine()
    {
        started = true;

        DemoPlaytestTelemetry.RecordBossAttempt();

        DemoPlaytestTelemetry
            .RecordPacingMilestone(
                "BOSS_START"
            );

        SetGates(true);

        if (bossCamera != null)
            bossCamera.EnterBossMode(boss.transform);

        if (bossHUD != null)
            bossHUD.ShowBoss(boss);

        SetPlayerControl(false);

        boss.BeginEncounterIntro();

        if (bossCamera != null)
            bossCamera.Shake(0.55f, 0.16f);

        yield return new WaitForSeconds(introDuration);

        if (boss == null ||
            boss.IsDead)
        {
            SetPlayerControl(true);
            yield break;
        }

        boss.BeginCombat();
        SetPlayerControl(true);
    }

    private void OnBossDefeated()
    {
        if (completed)
            return;

        completed = true;

        DemoPlaytestTelemetry.RecordBossDefeated();

        StartCoroutine(
            FinishEncounterRoutine()
        );
    }

    private IEnumerator FinishEncounterRoutine()
    {
        SetPlayerControl(false);

        yield return new WaitForSeconds(0.75f);

        SetGates(false);

        if (bossCamera != null)
            bossCamera.ExitBossMode();

        if (bossHUD != null)
            bossHUD.HideBoss();

        yield return new WaitForSeconds(0.25f);

        SetPlayerControl(true);
    }

    private void SetPlayerControl(bool enabled)
    {
        if (enabled)
        {
            VerticalSliceDirector sliceDirector =
                FindAnyObjectByType<VerticalSliceDirector>();

            if (sliceDirector != null &&
                sliceDirector.BlockPlayerControl)
            {
                return;
            }
        }

        if (playerMotor != null)
            playerMotor.enabled = enabled;

        if (playerCombat != null)
            playerCombat.enabled = enabled;

        if (playerAttack != null)
            playerAttack.enabled = enabled;
    }

    private void SetGates(bool active)
    {
        if (leftGate != null)
            leftGate.SetActive(active);

        if (rightGate != null)
            rightGate.SetActive(active);
    }
}
