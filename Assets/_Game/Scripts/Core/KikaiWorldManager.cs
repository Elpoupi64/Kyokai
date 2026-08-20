using System;
using UnityEngine;

public enum KikaiWorldMode
{
    Normal,
    Ethereal
}

public class KikaiWorldManager : MonoBehaviour
{
    public static KikaiWorldManager Instance { get; private set; }

    [Header("Starting State")]
    [SerializeField] private KikaiWorldMode startingMode = KikaiWorldMode.Normal;

    [Header("Normal Atmosphere")]
    [SerializeField] private Color normalAmbient = new Color(0.32f, 0.28f, 0.24f);
    [SerializeField] private Color normalFogColor = new Color(0.16f, 0.14f, 0.12f);
    [SerializeField] private float normalFogDensity = 0.012f;

    [Header("Ethereal Atmosphere")]
    [SerializeField] private Color etherealAmbient = new Color(0.12f, 0.28f, 0.32f);
    [SerializeField] private Color etherealFogColor = new Color(0.05f, 0.15f, 0.20f);
    [SerializeField] private float etherealFogDensity = 0.022f;

    public KikaiWorldMode CurrentMode { get; private set; }
    public bool IsEthereal => CurrentMode == KikaiWorldMode.Ethereal;

    public event Action<KikaiWorldMode> ModeChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        CurrentMode = startingMode;
    }

    private void Start()
    {
        ApplyAtmosphere();
        ModeChanged?.Invoke(CurrentMode);
    }

    public void ToggleMode()
    {
        SetMode(IsEthereal ? KikaiWorldMode.Normal : KikaiWorldMode.Ethereal);
    }

    public void SetMode(KikaiWorldMode mode)
    {
        CurrentMode = mode;
        ApplyAtmosphere();
        ModeChanged?.Invoke(CurrentMode);
    }

    private void ApplyAtmosphere()
    {
        bool ethereal = CurrentMode == KikaiWorldMode.Ethereal;

        RenderSettings.ambientLight = ethereal ? etherealAmbient : normalAmbient;
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.ExponentialSquared;
        RenderSettings.fogColor = ethereal ? etherealFogColor : normalFogColor;
        RenderSettings.fogDensity = ethereal ? etherealFogDensity : normalFogDensity;
    }
}
