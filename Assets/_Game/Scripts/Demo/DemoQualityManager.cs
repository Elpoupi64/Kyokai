using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DemoQualityManager : MonoBehaviour
{
    public enum DemoPreset
    {
        Performance = 0,
        Balanced = 1,
        Cinematic = 2
    }

    private const string PresetKey =
        "Katsuhiro.Demo.QualityPreset";

    private static DemoQualityManager instance;

    [SerializeField]
    private DemoPreset currentPreset =
        DemoPreset.Balanced;

    public static DemoQualityManager Instance =>
        instance;

    public DemoPreset CurrentPreset =>
        currentPreset;

    public static string CurrentPresetLabel
    {
        get
        {
            if (instance == null)
                return "Balanced";

            return instance.currentPreset.ToString();
        }
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

        currentPreset =
            (DemoPreset)Mathf.Clamp(
                PlayerPrefs.GetInt(
                    PresetKey,
                    (int)DemoPreset.Balanced
                ),
                0,
                2
            );

        SceneManager.sceneLoaded += OnSceneLoaded;

        ApplyPreset(currentPreset, false);
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            instance = null;
        }
    }

    private void OnSceneLoaded(
        Scene scene,
        LoadSceneMode mode
    )
    {
        ApplySceneBudget();
    }

    public static DemoQualityManager
        EnsureInstance()
    {
        if (instance != null)
            return instance;

        DemoQualityManager existing =
            FindAnyObjectByType<DemoQualityManager>();

        if (existing != null)
            return existing;

        GameObject root =
            new GameObject("DemoQualityManager_v15");

        return root.AddComponent<DemoQualityManager>();
    }

    public void CyclePreset()
    {
        DemoPreset next =
            (DemoPreset)(
                ((int)currentPreset + 1) % 3
            );

        ApplyPreset(next, true);
    }

    public void ApplyPreset(
        DemoPreset preset,
        bool save
    )
    {
        currentPreset = preset;

        switch (preset)
        {
            case DemoPreset.Performance:
                Application.targetFrameRate = 60;
                QualitySettings.vSyncCount = 0;
                QualitySettings.shadowDistance = 18f;
                QualitySettings.lodBias = 0.85f;
                QualitySettings.antiAliasing = 0;
                break;

            case DemoPreset.Cinematic:
                Application.targetFrameRate = 60;
                QualitySettings.vSyncCount = 1;
                QualitySettings.shadowDistance = 45f;
                QualitySettings.lodBias = 1.55f;
                QualitySettings.antiAliasing = 4;
                break;

            default:
                Application.targetFrameRate = 60;
                QualitySettings.vSyncCount = 1;
                QualitySettings.shadowDistance = 30f;
                QualitySettings.lodBias = 1.10f;
                QualitySettings.antiAliasing = 2;
                break;
        }

        if (save)
        {
            PlayerPrefs.SetInt(
                PresetKey,
                (int)currentPreset
            );

            PlayerPrefs.Save();
        }

        ApplySceneBudget();

        DemoPlaytestTelemetry.RecordQualityPreset(
            currentPreset.ToString()
        );
    }

    public void ApplySceneBudget()
    {
        float particleScale =
            GetParticleScale();

        DemoParticleBudgetTag[] tags =
            FindObjectsByType<DemoParticleBudgetTag>(FindObjectsInactive.Include);

        for (int i = 0; i < tags.Length; i++)
        {
            if (tags[i] != null)
                tags[i].ApplyBudget(particleScale);
        }
    }

    public float GetParticleScale()
    {
        switch (currentPreset)
        {
            case DemoPreset.Performance:
                return 0.40f;

            case DemoPreset.Cinematic:
                return 0.95f;

            default:
                return 0.64f;
        }
    }
}
