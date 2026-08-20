using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class DemoPerformanceHUD : MonoBehaviour
{
    private const int SampleCount = 300;

    private bool visible;

    private readonly float[] frameMs =
        new float[SampleCount];

    private int frameIndex;
    private int validSamples;

    private float refreshTimer;
    private int particleCount;

    private float averageFps;
    private float onePercentLowFps;
    private float averageMs;
    private float memoryMb;

    private GUIStyle style;

    private void Update()
    {
        if (Keyboard.current != null &&
            Keyboard.current.f3Key.wasPressedThisFrame)
        {
            visible = !visible;
        }

        float ms =
            Mathf.Clamp(
                Time.unscaledDeltaTime * 1000f,
                0.01f,
                1000f
            );

        frameMs[frameIndex] = ms;

        frameIndex =
            (frameIndex + 1) %
            SampleCount;

        validSamples =
            Mathf.Min(
                validSamples + 1,
                SampleCount
            );

        refreshTimer -=
            Time.unscaledDeltaTime;

        if (refreshTimer <= 0f)
        {
            refreshTimer = 0.75f;
            RefreshMetrics();
        }
    }

    private void RefreshMetrics()
    {
        if (validSamples <= 0)
            return;

        float[] copy =
            new float[validSamples];

        float sum = 0f;

        for (int i = 0; i < validSamples; i++)
        {
            copy[i] = frameMs[i];
            sum += copy[i];
        }

        Array.Sort(copy);

        averageMs =
            sum / validSamples;

        averageFps =
            averageMs > 0.001f
                ? 1000f / averageMs
                : 0f;

        int percentileIndex =
            Mathf.Clamp(
                Mathf.CeilToInt(
                    validSamples * 0.99f
                ) - 1,
                0,
                validSamples - 1
            );

        float percentileMs =
            copy[percentileIndex];

        onePercentLowFps =
            percentileMs > 0.001f
                ? 1000f / percentileMs
                : 0f;

        particleCount =
            CountActiveParticles();

        memoryMb =
            (float)(
                GC.GetTotalMemory(false) /
                (1024.0 * 1024.0)
            );
    }

    private void OnGUI()
    {
        if (!visible)
            return;

        if (style == null)
        {
            style =
                new GUIStyle(GUI.skin.label)
                {
                    fontSize = 13,
                    normal =
                    {
                        textColor =
                            Color.white
                    }
                };
        }

        GUI.Box(
            new Rect(
                16f,
                16f,
                295f,
                145f
            ),
            ""
        );

        GUI.Label(
            new Rect(
                28f,
                26f,
                270f,
                122f
            ),
            "FPS moyen : " +
            averageFps.ToString("0.0") +
            "\n1% low approx. : " +
            onePercentLowFps.ToString("0.0") +
            "\nFrame : " +
            averageMs.ToString("0.00") +
            " ms" +
            "\nQualité : " +
            DemoQualityManager.CurrentPresetLabel +
            "\nParticules : " +
            particleCount +
            "\nMémoire GC : " +
            memoryMb.ToString("0.0") +
            " MB",
            style
        );
    }

    private int CountActiveParticles()
    {
        int total = 0;

        ParticleSystem[] systems =
            FindObjectsByType<ParticleSystem>(
                FindObjectsInactive.Exclude
            );

        for (int i = 0; i < systems.Length; i++)
        {
            if (systems[i] != null)
                total +=
                    systems[i].particleCount;
        }

        return total;
    }
}
