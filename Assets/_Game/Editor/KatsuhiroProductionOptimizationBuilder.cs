#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public static class KatsuhiroProductionOptimizationBuilder
{
    public static void Apply(
        Transform levelRoot,
        Transform gameplayRoot,
        Transform player
    )
    {
        if (levelRoot == null ||
            gameplayRoot == null)
        {
            return;
        }

        Transform old =
            gameplayRoot.Find("DemoProduction_v15");

        if (old != null)
            UnityEngine.Object.DestroyImmediate(
                old.gameObject
            );

        GameObject marker =
            new GameObject("DemoProduction_v15");

        marker.transform.SetParent(
            gameplayRoot
        );

        GameObject pause =
            new GameObject("DemoPauseMenu");

        pause.transform.SetParent(
            gameplayRoot
        );

        pause.AddComponent<DemoPauseMenu>();

        GameObject perf =
            new GameObject("DemoPerformanceHUD");

        perf.transform.SetParent(
            gameplayRoot
        );

        perf.AddComponent<DemoPerformanceHUD>();

        EnsureQualityManager();
        OptimizeCamera();
        OptimizeLights(levelRoot);
        OptimizeRenderers(levelRoot);
        TagParticles();

        DemoQualityManager manager =
            UnityEngine.Object.FindAnyObjectByType<DemoQualityManager>();

        if (manager != null)
            manager.ApplySceneBudget();
    }

    private static void EnsureQualityManager()
    {
        DemoQualityManager existing =
            UnityEngine.Object.FindAnyObjectByType<DemoQualityManager>();

        if (existing != null)
            return;

        GameObject root =
            new GameObject("DemoQualityManager_v15");

        root.AddComponent<DemoQualityManager>();
    }

    private static void OptimizeCamera()
    {
        Camera camera =
            Camera.main != null
                ? Camera.main
                : UnityEngine.Object.FindAnyObjectByType<Camera>();

        if (camera == null)
            return;

        camera.allowHDR = true;
        camera.allowDynamicResolution = true;
        camera.farClipPlane =
            Mathf.Min(
                camera.farClipPlane,
                160f
            );
    }

    private static void OptimizeLights(
        Transform levelRoot
    )
    {
        Light[] lights =
            UnityEngine.Object.FindObjectsByType<Light>(FindObjectsInactive.Include);

        for (int i = 0; i < lights.Length; i++)
        {
            Light light = lights[i];

            if (light == null)
                continue;

            if (light.type == LightType.Point ||
                light.type == LightType.Spot)
            {
                light.shadows =
                    LightShadows.None;
            }
        }
    }

    private static void OptimizeRenderers(
        Transform levelRoot
    )
    {
        Renderer[] renderers =
            levelRoot.GetComponentsInChildren<Renderer>(
                true
            );

        for (int i = 0; i < renderers.Length; i++)
        {
            Renderer renderer =
                renderers[i];

            if (renderer == null)
                continue;

            if (IsBackground(renderer.transform))
            {
                renderer.shadowCastingMode =
                    ShadowCastingMode.Off;

                renderer.receiveShadows =
                    false;

                continue;
            }

            if (CanBeBatchingStatic(
                renderer.transform
            ))
            {
                StaticEditorFlags current =
                    GameObjectUtility
                        .GetStaticEditorFlags(
                            renderer.gameObject
                        );

                current |=
                    StaticEditorFlags.BatchingStatic |
                    StaticEditorFlags.OccludeeStatic;

                GameObjectUtility
                    .SetStaticEditorFlags(
                        renderer.gameObject,
                        current
                    );
            }
        }
    }

    private static bool IsBackground(
        Transform transform
    )
    {
        Transform current =
            transform;

        while (current != null)
        {
            string name =
                current.name.ToLowerInvariant();

            if (name.Contains("parallax") ||
                name.Contains("backdrop") ||
                name.Contains("skyline") ||
                name.Contains("silhouette") ||
                name.Contains("paintedsky") ||
                name.Contains("smokecard") ||
                name.Contains("ink"))
            {
                return true;
            }

            current = current.parent;
        }

        return transform.position.z > 7f;
    }

    private static bool CanBeBatchingStatic(
        Transform transform
    )
    {
        Transform current =
            transform;

        while (current != null)
        {
            string name =
                current.name.ToLowerInvariant();

            if (name.Contains("parallax") ||
                name.Contains("backdrop") ||
                name.Contains("gear") ||
                name.Contains("steam") ||
                name.Contains("particle") ||
                name.Contains("banner") ||
                name.Contains("cloth") ||
                name.Contains("rift") ||
                name.Contains("sway") ||
                name.Contains("focuslight"))
            {
                return false;
            }

            if (current.GetComponent<ParallaxLayer>() != null ||
                current.GetComponent<RotatingGear>() != null ||
                current.GetComponent<FoundryAutoSway>() != null ||
                current.GetComponent<FoundryBackdropDrift>() != null ||
                current.GetComponent<ParticleSystem>() != null)
            {
                return false;
            }

            current = current.parent;
        }

        return true;
    }

    private static void TagParticles()
    {
        ParticleSystem[] systems =
            UnityEngine.Object.FindObjectsByType<ParticleSystem>(FindObjectsInactive.Include);

        for (int i = 0; i < systems.Length; i++)
        {
            if (systems[i] == null)
                continue;

            if (systems[i].GetComponent<DemoParticleBudgetTag>() == null)
                systems[i].gameObject.AddComponent<DemoParticleBudgetTag>();
        }
    }
}

#endif
