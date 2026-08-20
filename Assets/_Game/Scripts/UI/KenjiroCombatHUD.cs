using UnityEngine;

public class KenjiroCombatHUD : MonoBehaviour
{
    [SerializeField] private KenjiroCombatController combat;
    [SerializeField] private PlayerHealth health;
    [SerializeField] private KikaiWorldManager worldManager;

    private GUIStyle titleStyle;
    private GUIStyle textStyle;
    private GUIStyle smallStyle;

    private float currentHealth01 = 1f;

    private void Awake()
    {
        if (combat == null)
            combat = FindAnyObjectByType<KenjiroCombatController>();

        if (health == null)
            health = FindAnyObjectByType<PlayerHealth>();

        if (worldManager == null)
            worldManager = KikaiWorldManager.Instance;

        if (worldManager == null)
            worldManager = FindAnyObjectByType<KikaiWorldManager>();
    }

    private void OnEnable()
    {
        if (health != null)
            health.HealthChanged += OnHealthChanged;
    }

    private void OnDisable()
    {
        if (health != null)
            health.HealthChanged -= OnHealthChanged;
    }

    private void Start()
    {
        if (health != null && health.MaxHealth > 0)
        {
            currentHealth01 =
                (float)health.CurrentHealth /
                health.MaxHealth;
        }
    }

    private void OnHealthChanged(int current, int maximum)
    {
        currentHealth01 =
            maximum > 0
                ? Mathf.Clamp01((float)current / maximum)
                : 0f;
    }

    private void EnsureStyles()
    {
        if (titleStyle != null)
            return;

        titleStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 14,
            fontStyle = FontStyle.Bold
        };

        textStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 12
        };

        smallStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 10
        };
    }

    private void OnGUI()
    {
        if (combat == null)
            return;

        EnsureStyles();

        float boxWidth = 350f;
        float boxHeight = 112f;
        float x = 18f;
        float y = Screen.height - boxHeight - 18f;

        GUI.Box(
            new Rect(x, y, boxWidth, boxHeight),
            ""
        );

        GUI.Label(
            new Rect(x + 14f, y + 8f, 180f, 22f),
            "KENJIRO — COMBAT",
            titleStyle
        );

        GUI.Label(
            new Rect(x + 14f, y + 29f, 50f, 18f),
            "PV",
            textStyle
        );

        DrawBar(
            new Rect(x + 54f, y + 31f, 150f, 12f),
            currentHealth01,
            new Color(0.75f, 0.12f, 0.08f)
        );

        GUI.Label(
            new Rect(x + 14f, y + 50f, 50f, 18f),
            "ÉTHER",
            textStyle
        );

        DrawBar(
            new Rect(x + 54f, y + 52f, 150f, 12f),
            combat.EtherNormalized,
            new Color(0.08f, 0.78f, 0.92f)
        );

        bool ethereal =
            worldManager != null &&
            worldManager.IsEthereal;

        string specialStatus =
            combat.CanUseSpecial
                ? "L : SPÉCIALE PRÊTE"
                : ethereal
                    ? "L : éther insuffisant"
                    : "K puis L : spéciale Kikai-Yūrei";

        GUI.Label(
            new Rect(x + 214f, y + 47f, 126f, 36f),
            specialStatus,
            smallStyle
        );

        string combo =
            combat.ComboStep > 0
                ? $"COMBO x{combat.ComboStep}"
                : "";

        GUI.Label(
            new Rect(x + 214f, y + 26f, 120f, 20f),
            combo,
            titleStyle
        );

        GUI.Label(
            new Rect(x + 14f, y + 76f, 330f, 28f),
            "J léger/air • I lourd • Shift esquive • L spéciale • K Kikai-Yūrei",
            smallStyle
        );
    }

    private void DrawBar(
        Rect rect,
        float normalized,
        Color fillColor
    )
    {
        Color old = GUI.color;

        GUI.color = new Color(0.05f, 0.05f, 0.05f, 0.95f);
        GUI.DrawTexture(rect, Texture2D.whiteTexture);

        Rect fill = rect;
        fill.width *= Mathf.Clamp01(normalized);

        GUI.color = fillColor;
        GUI.DrawTexture(fill, Texture2D.whiteTexture);

        GUI.color = old;
    }
}
