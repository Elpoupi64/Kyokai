#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

public static class KatsuhiroV17ExternalPlaytestBuilder
{
    public static void Apply(
        Transform gameplayRoot
    )
    {
        if (gameplayRoot == null)
            return;

        Transform old =
            gameplayRoot.Find(
                "ExternalPlaytest_v17"
            );

        if (old != null)
            Object.DestroyImmediate(
                old.gameObject
            );

        GameObject root =
            new GameObject(
                "ExternalPlaytest_v17"
            );

        root.transform.SetParent(
            gameplayRoot
        );

        GameObject telemetry =
            new GameObject(
                "DemoPlaytestTelemetry"
            );

        telemetry.transform.SetParent(
            root.transform
        );

        telemetry.AddComponent<
            DemoPlaytestTelemetry
        >();

        GameObject hud =
            new GameObject(
                "DemoPlaytestHUD"
            );

        hud.transform.SetParent(
            root.transform
        );

        hud.AddComponent<
            DemoPlaytestHUD
        >();
    }
}

#endif
