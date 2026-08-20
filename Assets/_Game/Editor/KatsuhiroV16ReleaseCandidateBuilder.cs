#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

public static class KatsuhiroV16ReleaseCandidateBuilder
{
    public static void Apply(
        Transform gameplayRoot
    )
    {
        if (gameplayRoot == null)
            return;

        Transform oldBase =
            gameplayRoot.Find(
                "DemoReleaseCandidate_v16"
            );

        if (oldBase != null)
            Object.DestroyImmediate(
                oldBase.gameObject
            );

        Transform oldHotfix =
            gameplayRoot.Find(
                "DemoReleaseCandidate_v16_1"
            );

        if (oldHotfix != null)
            Object.DestroyImmediate(
                oldHotfix.gameObject
            );

        GameObject baseMarker =
            new GameObject(
                "DemoReleaseCandidate_v16"
            );

        baseMarker.transform.SetParent(
            gameplayRoot
        );

        GameObject hotfixMarker =
            new GameObject(
                "DemoReleaseCandidate_v16_1"
            );

        hotfixMarker.transform.SetParent(
            baseMarker.transform
        );
    }
}

#endif
