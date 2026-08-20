using UnityEngine;

public class FoundryGameplayFocusLight : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Light focusLight;
    [SerializeField] private KikaiWorldManager worldManager;
    [SerializeField] private Vector3 offset =
        new Vector3(0f, 1.4f, -1.2f);

    [Header("Normal")]
    [SerializeField] private Color normalColor =
        new Color(1.0f, 0.60f, 0.30f);
    [SerializeField] private float normalIntensity = 1.35f;

    [Header("Ethereal")]
    [SerializeField] private Color etherealColor =
        new Color(0.22f, 0.90f, 1.0f);
    [SerializeField] private float etherealIntensity = 1.75f;

    private void Awake()
    {
        if (focusLight == null)
            focusLight = GetComponent<Light>();

        if (worldManager == null)
            worldManager = KikaiWorldManager.Instance;
    }

    private void OnEnable()
    {
        if (worldManager == null)
            worldManager = FindAnyObjectByType<KikaiWorldManager>();

        if (worldManager != null)
        {
            worldManager.ModeChanged -= ApplyMode;
            worldManager.ModeChanged += ApplyMode;
            ApplyMode(worldManager.CurrentMode);
        }
    }

    private void OnDisable()
    {
        if (worldManager != null)
            worldManager.ModeChanged -= ApplyMode;
    }

    private void LateUpdate()
    {
        if (target != null)
            transform.position = target.position + offset;
    }

    private void ApplyMode(KikaiWorldMode mode)
    {
        if (focusLight == null)
            return;

        bool ethereal =
            mode == KikaiWorldMode.Ethereal;

        focusLight.color =
            ethereal ? etherealColor : normalColor;

        focusLight.intensity =
            ethereal ? etherealIntensity : normalIntensity;
    }
}
