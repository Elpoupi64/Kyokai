using UnityEngine;

public class Doryoku3BossHUD :
    MonoBehaviour
{
    [Header("Labels")]
    [SerializeField]
    private string bossName =
        "DORYOKU-3 // UNITÉ 07";

    [SerializeField]
    private string bossSubtitle =
        "AUTOMATE POSSÉDÉ";

    [Header("Animation")]
    [SerializeField]
    private float fadeSpeed = 3.5f;

    private Doryoku3MiniBoss boss;

    private float displayedHealth = 1f;
    private float targetHealth = 1f;

    private float alpha;
    private float targetAlpha;

    private float introTitleTimer;
    private float phaseAnnouncementTimer;

    private int phase = 1;

    private GUIStyle titleStyle;
    private GUIStyle subtitleStyle;
    private GUIStyle phaseStyle;
    private GUIStyle centerTitleStyle;

    public void ShowBoss(
        Doryoku3MiniBoss targetBoss
    )
    {
        Unbind();

        boss = targetBoss;

        if (boss != null)
        {
            targetHealth =
                boss.MaxHealth > 0
                    ? (float)boss.CurrentHealth /
                      boss.MaxHealth
                    : 1f;

            displayedHealth =
                targetHealth;

            phase =
                (int)boss.CurrentPhase;

            boss.HealthChanged +=
                OnHealthChanged;

            boss.PhaseChanged +=
                OnPhaseChanged;

            boss.BossDefeated +=
                OnBossDefeated;
        }

        introTitleTimer = 2.2f;
        targetAlpha = 1f;
    }

    public void HideBoss()
    {
        targetAlpha = 0f;
    }

    private void OnDestroy()
    {
        Unbind();
    }

    private void Update()
    {
        alpha =
            Mathf.MoveTowards(
                alpha,
                targetAlpha,
                fadeSpeed *
                Time.deltaTime
            );

        displayedHealth =
            Mathf.Lerp(
                displayedHealth,
                targetHealth,
                1f -
                Mathf.Exp(
                    -8f * Time.deltaTime
                )
            );

        if (introTitleTimer > 0f)
            introTitleTimer -= Time.deltaTime;

        if (phaseAnnouncementTimer > 0f)
            phaseAnnouncementTimer -=
                Time.deltaTime;
    }

    private void OnHealthChanged(
        int current,
        int maximum
    )
    {
        targetHealth =
            maximum > 0
                ? Mathf.Clamp01(
                    (float)current /
                    maximum
                )
                : 0f;
    }

    private void OnPhaseChanged(
        Doryoku3MiniBoss.BossPhase bossPhase
    )
    {
        phase = (int)bossPhase;

        if (phase >= 2)
            phaseAnnouncementTimer = 2.5f;
    }

    private void OnBossDefeated()
    {
        targetHealth = 0f;
    }

    private void Unbind()
    {
        if (boss == null)
            return;

        boss.HealthChanged -=
            OnHealthChanged;

        boss.PhaseChanged -=
            OnPhaseChanged;

        boss.BossDefeated -=
            OnBossDefeated;

        boss = null;
    }

    private void EnsureStyles()
    {
        if (titleStyle != null)
            return;

        titleStyle =
            new GUIStyle(GUI.skin.label);

        titleStyle.fontSize = 18;
        titleStyle.fontStyle =
            FontStyle.Bold;

        titleStyle.alignment =
            TextAnchor.MiddleCenter;

        subtitleStyle =
            new GUIStyle(GUI.skin.label);

        subtitleStyle.fontSize = 11;
        subtitleStyle.alignment =
            TextAnchor.MiddleCenter;

        phaseStyle =
            new GUIStyle(GUI.skin.label);

        phaseStyle.fontSize = 12;
        phaseStyle.fontStyle =
            FontStyle.Bold;

        phaseStyle.alignment =
            TextAnchor.MiddleCenter;

        centerTitleStyle =
            new GUIStyle(GUI.skin.label);

        centerTitleStyle.fontSize = 28;
        centerTitleStyle.fontStyle =
            FontStyle.Bold;

        centerTitleStyle.alignment =
            TextAnchor.MiddleCenter;
    }

    private void OnGUI()
    {
        if (alpha <= 0.001f)
            return;

        EnsureStyles();

        Color previousColor = GUI.color;

        float width =
            Mathf.Min(
                Screen.width * 0.72f,
                850f
            );

        float x =
            (Screen.width - width) * 0.5f;

        float y = 28f;

        GUI.color =
            new Color(
                1f,
                1f,
                1f,
                alpha
            );

        GUI.Box(
            new Rect(
                x - 18f,
                y - 12f,
                width + 36f,
                96f
            ),
            ""
        );

        GUI.Label(
            new Rect(
                x,
                y - 4f,
                width,
                26f
            ),
            bossName,
            titleStyle
        );

        GUI.Label(
            new Rect(
                x,
                y + 20f,
                width,
                18f
            ),
            bossSubtitle,
            subtitleStyle
        );

        Rect bar =
            new Rect(
                x,
                y + 44f,
                width,
                18f
            );

        GUI.color =
            new Color(
                0.05f,
                0.04f,
                0.04f,
                0.92f * alpha
            );

        GUI.DrawTexture(
            bar,
            Texture2D.whiteTexture
        );

        Rect fill = bar;
        fill.width *=
            Mathf.Clamp01(
                displayedHealth
            );

        GUI.color =
            phase >= 2
                ? new Color(
                    0.65f,
                    0.08f,
                    0.82f,
                    alpha
                )
                : new Color(
                    0.72f,
                    0.08f,
                    0.04f,
                    alpha
                );

        GUI.DrawTexture(
            fill,
            Texture2D.whiteTexture
        );

        GUI.color =
            new Color(
                1f,
                1f,
                1f,
                alpha
            );

        string phaseLabel =
            phase >= 2
                ? "PHASE II — FUREUR ÉTHÉRIQUE"
                : "PHASE I — PROTOCOLE D'EXÉCUTION";

        GUI.Label(
            new Rect(
                x,
                y + 63f,
                width,
                20f
            ),
            phaseLabel,
            phaseStyle
        );

        if (introTitleTimer > 0f)
        {
            float introAlpha =
                Mathf.Clamp01(
                    introTitleTimer
                );

            GUI.color =
                new Color(
                    1f,
                    1f,
                    1f,
                    introAlpha
                );

            GUI.Label(
                new Rect(
                    0f,
                    Screen.height * 0.33f,
                    Screen.width,
                    60f
                ),
                "UNITÉ 07 — DORYOKU-3 POSSÉDÉ",
                centerTitleStyle
            );
        }

        if (phaseAnnouncementTimer > 0f)
        {
            float announceAlpha =
                Mathf.Clamp01(
                    phaseAnnouncementTimer
                );

            GUI.color =
                new Color(
                    0.90f,
                    0.45f,
                    1.0f,
                    announceAlpha
                );

            GUI.Label(
                new Rect(
                    0f,
                    Screen.height * 0.40f,
                    Screen.width,
                    60f
                ),
                "SURCHARGE — PHASE II",
                centerTitleStyle
            );
        }

        GUI.color = previousColor;
    }
}
