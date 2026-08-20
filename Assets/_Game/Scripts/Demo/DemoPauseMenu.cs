using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class DemoPauseMenu : MonoBehaviour
{
    [SerializeField] private string titleScene =
        "TitleScreen";

    private bool paused;

    private GUIStyle titleStyle;
    private GUIStyle buttonStyle;

    private void Update()
    {
        bool keyboardPause =
            Keyboard.current != null &&
            Keyboard.current.escapeKey.wasPressedThisFrame;

        bool gamepadPause =
            Gamepad.current != null &&
            Gamepad.current.startButton.wasPressedThisFrame;

        if (keyboardPause || gamepadPause)
            SetPaused(!paused);
    }

    private void OnDisable()
    {
        if (paused)
            Time.timeScale = 1f;
    }

    private void OnGUI()
    {
        if (!paused)
            return;

        EnsureStyles();

        GUI.color =
            new Color(
                0f,
                0f,
                0f,
                0.58f
            );

        GUI.DrawTexture(
            new Rect(
                0f,
                0f,
                Screen.width,
                Screen.height
            ),
            Texture2D.whiteTexture
        );

        GUI.color = Color.white;

        float width = 420f;
        float height = 410f;

        Rect panel =
            new Rect(
                Screen.width * 0.5f - width * 0.5f,
                Screen.height * 0.5f - height * 0.5f,
                width,
                height
            );

        GUI.Box(panel, "");

        GUI.Label(
            new Rect(
                panel.x + 24f,
                panel.y + 22f,
                panel.width - 48f,
                52f
            ),
            "PAUSE",
            titleStyle
        );

        float x = panel.x + 44f;
        float y = panel.y + 92f;
        float buttonWidth = panel.width - 88f;

        if (GUI.Button(
            new Rect(
                x,
                y,
                buttonWidth,
                44f
            ),
            "Reprendre",
            buttonStyle
        ))
        {
            SetPaused(false);
        }

        y += 54f;

        if (GUI.Button(
            new Rect(
                x,
                y,
                buttonWidth,
                44f
            ),
            "Recommencer au checkpoint",
            buttonStyle
        ))
        {
            Time.timeScale = 1f;
            paused = false;

            DemoSceneLoader.Load(
                SceneManager
                    .GetActiveScene()
                    .name
            );
        }

        y += 54f;

        if (GUI.Button(
            new Rect(
                x,
                y,
                buttonWidth,
                44f
            ),
            "Qualité : " +
            DemoQualityManager.CurrentPresetLabel,
            buttonStyle
        ))
        {
            DemoQualityManager
                .EnsureInstance()
                .CyclePreset();
        }

        y += 54f;

        if (GUI.Button(
            new Rect(
                x,
                y,
                buttonWidth,
                44f
            ),
            "Retour au titre",
            buttonStyle
        ))
        {
            Time.timeScale = 1f;
            paused = false;

            DemoSceneLoader.Load(
                titleScene
            );
        }

        y += 54f;

        if (GUI.Button(
            new Rect(
                x,
                y,
                buttonWidth,
                44f
            ),
            "Quitter",
            buttonStyle
        ))
        {
            Application.Quit();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }

    private void EnsureStyles()
    {
        if (titleStyle != null)
            return;

        titleStyle =
            new GUIStyle(GUI.skin.label)
            {
                fontSize = 32,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };

        buttonStyle =
            new GUIStyle(GUI.skin.button)
            {
                fontSize = 16
            };
    }

    private void SetPaused(
        bool value
    )
    {
        paused = value;
        Time.timeScale =
            paused ? 0f : 1f;
    }
}
