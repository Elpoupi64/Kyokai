using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class KikaiYureiController : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private string abilityActionName = "Ability";
    [SerializeField] private float toggleCooldown = 0.15f;

    [Header("World")]
    [SerializeField] private KikaiWorldManager worldManager;

    [Header("Kikai-Yurei Device")]
    [SerializeField] private Transform deviceTransform;
    [SerializeField] private Renderer etherCoreRenderer;
    [SerializeField] private Light deviceLight;

    [Header("Visual Feedback")]
    [SerializeField] private Color normalCoreColor = new Color(0.10f, 0.55f, 0.75f);
    [SerializeField] private Color etherealCoreColor = new Color(0.25f, 1.00f, 1.00f);
    [SerializeField] private float normalLightIntensity = 0.5f;
    [SerializeField] private float etherealLightIntensity = 5.0f;
    [SerializeField] private float normalDeviceScale = 1.0f;
    [SerializeField] private float etherealDeviceScale = 1.12f;

    private PlayerInput playerInput;
    private InputAction abilityAction;
    private Material runtimeCoreMaterial;
    private float nextToggleTime;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        if (playerInput.actions != null)
            abilityAction = playerInput.actions.FindAction(abilityActionName, false);

        if (worldManager == null)
            worldManager = KikaiWorldManager.Instance;

        if (worldManager == null)
            worldManager = FindAnyObjectByType<KikaiWorldManager>();
    }

    private void OnEnable()
    {
        abilityAction?.Enable();

        if (worldManager != null)
        {
            worldManager.ModeChanged -= OnWorldModeChanged;
            worldManager.ModeChanged += OnWorldModeChanged;
        }
    }

    private void Start()
    {
        OnWorldModeChanged(
            worldManager != null
                ? worldManager.CurrentMode
                : KikaiWorldMode.Normal
        );
    }

    private void OnDisable()
    {
        abilityAction?.Disable();

        if (worldManager != null)
            worldManager.ModeChanged -= OnWorldModeChanged;
    }

    private void Update()
    {
        if (abilityAction == null || worldManager == null)
            return;

        if (Time.unscaledTime < nextToggleTime)
            return;

        if (abilityAction.WasPressedThisFrame())
        {
            nextToggleTime = Time.unscaledTime + toggleCooldown;
            worldManager.ToggleMode();
        }
    }

    private void OnWorldModeChanged(KikaiWorldMode mode)
    {
        bool ethereal = mode == KikaiWorldMode.Ethereal;

        if (deviceLight != null)
        {
            deviceLight.enabled = true;
            deviceLight.intensity = ethereal
                ? etherealLightIntensity
                : normalLightIntensity;

            deviceLight.color = ethereal
                ? etherealCoreColor
                : normalCoreColor;
        }

        if (deviceTransform != null)
        {
            float scale = ethereal
                ? etherealDeviceScale
                : normalDeviceScale;

            deviceTransform.localScale = Vector3.one * scale;
        }

        if (etherCoreRenderer != null)
        {
            if (runtimeCoreMaterial == null)
                runtimeCoreMaterial = etherCoreRenderer.material;

            Color color = ethereal
                ? etherealCoreColor
                : normalCoreColor;

            if (runtimeCoreMaterial.HasProperty("_BaseColor"))
                runtimeCoreMaterial.SetColor("_BaseColor", color);

            if (runtimeCoreMaterial.HasProperty("_Color"))
                runtimeCoreMaterial.SetColor("_Color", color);

            if (runtimeCoreMaterial.HasProperty("_EmissionColor"))
            {
                runtimeCoreMaterial.EnableKeyword("_EMISSION");
                runtimeCoreMaterial.SetColor(
                    "_EmissionColor",
                    color * (ethereal ? 4.0f : 1.2f)
                );
            }
        }
    }
}
