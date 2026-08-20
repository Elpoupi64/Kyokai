using UnityEngine;
using UnityEngine.InputSystem;

public class DemoPlaytestHUD : MonoBehaviour
{
    private bool visible;
    private GUIStyle style;
    private GUIStyle titleStyle;

    private void Update()
    {
        if (Keyboard.current != null &&
            Keyboard.current.f4Key.wasPressedThisFrame)
        {
            visible = !visible;
        }
    }

    private void OnGUI()
    {
        if (!visible)
            return;

        EnsureStyles();

        float width = 330f;
        float height = 270f;

        Rect panel =
            new Rect(
                Screen.width - width - 18f,
                18f,
                width,
                height
            );

        GUI.Box(panel, "");

        GUI.Label(
            new Rect(
                panel.x + 14f,
                panel.y + 10f,
                panel.width - 28f,
                30f
            ),
            "PLAYTEST v17",
            titleStyle
        );

        GUI.Label(
            new Rect(
                panel.x + 16f,
                panel.y + 43f,
                panel.width - 32f,
                166f
            ),
            DemoPlaytestTelemetry
                .GetCompactSummary() +
            "\n\n" +
            DemoPlaytestTelemetry
                .GetPacingCompactSummary(),
            style
        );

        if (GUI.Button(
            new Rect(
                panel.x + 16f,
                panel.yMax - 43f,
                panel.width - 32f,
                30f
            ),
            "Copier le chemin du rapport"
        ))
        {
            string path =
                DemoPlaytestTelemetry
                    .LatestSummaryPath;

            if (!string.IsNullOrEmpty(path))
                GUIUtility.systemCopyBuffer =
                    path;
        }
    }

    private void EnsureStyles()
    {
        if (style != null)
            return;

        style =
            new GUIStyle(GUI.skin.label)
            {
                fontSize = 13,
                normal =
                {
                    textColor = Color.white
                }
            };

        titleStyle =
            new GUIStyle(style)
            {
                fontSize = 17,
                fontStyle =
                    FontStyle.Bold
            };
    }
}
