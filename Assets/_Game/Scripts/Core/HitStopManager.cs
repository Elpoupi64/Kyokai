using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(-500)]
public class HitStopManager : MonoBehaviour
{
    public static HitStopManager Instance { get; private set; }

    [SerializeField] private float defaultTimeScale = 0.04f;

    private Coroutine activeRoutine;
    private float previousTimeScale = 1f;
    private float previousFixedDeltaTime = 0.02f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnDisable()
    {
        RestoreTime();
    }

    public void Request(float realSeconds)
    {
        Request(realSeconds, defaultTimeScale);
    }

    public void Request(float realSeconds, float slowedTimeScale)
    {
        realSeconds = Mathf.Max(0f, realSeconds);
        slowedTimeScale = Mathf.Clamp(slowedTimeScale, 0f, 1f);

        if (realSeconds <= 0f)
            return;

        if (activeRoutine != null)
            StopCoroutine(activeRoutine);

        RestoreTime();
        activeRoutine = StartCoroutine(HitStopRoutine(realSeconds, slowedTimeScale));
    }

    private IEnumerator HitStopRoutine(float realSeconds, float slowedTimeScale)
    {
        previousTimeScale = Time.timeScale;
        previousFixedDeltaTime = Time.fixedDeltaTime;

        Time.timeScale = slowedTimeScale;

        // Keep physics step proportional while the world is slowed.
        Time.fixedDeltaTime =
            previousFixedDeltaTime *
            Mathf.Max(0.01f, slowedTimeScale);

        yield return new WaitForSecondsRealtime(realSeconds);

        RestoreTime();
        activeRoutine = null;
    }

    private void RestoreTime()
    {
        if (!Mathf.Approximately(Time.timeScale, previousTimeScale))
            Time.timeScale = previousTimeScale;

        if (!Mathf.Approximately(Time.fixedDeltaTime, previousFixedDeltaTime))
            Time.fixedDeltaTime = previousFixedDeltaTime;
    }
}
