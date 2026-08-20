using UnityEngine;

public class FoundryWorldReactiveLight : MonoBehaviour
{
    [SerializeField] private Light targetLight;
    [SerializeField] private KikaiWorldManager worldManager;

    [SerializeField] private Color normalColor =
        new Color(0.45f, 0.25f, 0.12f);
    [SerializeField] private float normalIntensity = 0.15f;

    [SerializeField] private Color etherealColor =
        new Color(0.10f, 0.95f, 1.00f);
    [SerializeField] private float etherealIntensity = 4.0f;

    private void Awake()
    {
        if (targetLight == null)
            targetLight = GetComponent<Light>();
    }

    private void OnEnable()
    {
        if (worldManager == null)
            worldManager = KikaiWorldManager.Instance;

        if (worldManager == null)
            worldManager = FindAnyObjectByType<KikaiWorldManager>();

        if (worldManager != null)
        {
            worldManager.ModeChanged -= Apply;
            worldManager.ModeChanged += Apply;
            Apply(worldManager.CurrentMode);
        }
    }

    private void OnDisable()
    {
        if (worldManager != null)
            worldManager.ModeChanged -= Apply;
    }

    private void Apply(KikaiWorldMode mode)
    {
        if (targetLight == null)
            return;

        bool ethereal =
            mode == KikaiWorldMode.Ethereal;

        targetLight.color =
            ethereal ? etherealColor : normalColor;

        targetLight.intensity =
            ethereal ? etherealIntensity : normalIntensity;
    }
}
