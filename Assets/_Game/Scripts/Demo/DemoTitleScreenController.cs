using UnityEngine;
using UnityEngine.SceneManagement;

public class DemoTitleScreenController : MonoBehaviour
{
    [SerializeField] private Texture2D kenjiroTexture;
    [SerializeField] private Texture2D yukiTexture;
    [SerializeField] private Texture2D takedaTexture;

    [SerializeField] private string gameplayScene =
        "Foundry_Prototype";

    private bool galleryOpen;
    private int galleryIndex;

    private GUIStyle titleStyle;
    private GUIStyle subtitleStyle;
    private GUIStyle buttonStyle;
    private GUIStyle smallStyle;
    private GUIStyle galleryTitleStyle;

    private readonly string[] galleryNames =
    {
        "KENJIRO TANAKA — Ingénieur-détective",
        "YUKI ISHIKAWA — Prêtresse miko",
        "TAKEDA — Rōnin"
    };

    private void Awake()
    {
        Time.timeScale = 1f;
        DemoQualityManager.EnsureInstance();
        DemoPlaytestTelemetry.EnsureInstance();
    }

    private void EnsureStyles()
    {
        if (titleStyle != null)
            return;

        titleStyle =
            new GUIStyle(GUI.skin.label)
            {
                fontSize = 36,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft,
                wordWrap = true,
                normal =
                {
                    textColor = Color.white
                }
            };

        subtitleStyle =
            new GUIStyle(GUI.skin.label)
            {
                fontSize = 17,
                alignment = TextAnchor.MiddleLeft,
                wordWrap = true,
                normal =
                {
                    textColor =
                        new Color(
                            0.90f,
                            0.82f,
                            0.70f
                        )
                }
            };

        buttonStyle =
            new GUIStyle(GUI.skin.button)
            {
                fontSize = 17,
                alignment = TextAnchor.MiddleLeft,
                padding =
                    new RectOffset(
                        18,
                        12,
                        8,
                        8
                    )
            };

        smallStyle =
            new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                wordWrap = true,
                normal =
                {
                    textColor =
                        new Color(
                            0.78f,
                            0.78f,
                            0.78f
                        )
                }
            };

        galleryTitleStyle =
            new GUIStyle(titleStyle)
            {
                fontSize = 24,
                alignment = TextAnchor.MiddleCenter
            };
    }

    private void OnGUI()
    {
        EnsureStyles();

        DrawBackground();

        if (galleryOpen)
        {
            DrawGallery();
            return;
        }

        float margin =
            Mathf.Max(26f, Screen.width * 0.045f);

        float panelWidth =
            Mathf.Clamp(
                Screen.width * 0.38f,
                430f,
                700f
            );

        Rect panel =
            new Rect(
                margin,
                margin,
                panelWidth,
                Screen.height - margin * 2f
            );

        GUI.Box(panel, "");

        float x =
            panel.x + 30f;

        float y =
            panel.y + 32f;

        float width =
            panel.width - 60f;

        GUI.Label(
            new Rect(
                x,
                y,
                width,
                98f
            ),
            "L'INGÉNIEUR, LA PRÊTRESSE ET LE RONIN",
            titleStyle
        );

        y += 100f;

        GUI.Label(
            new Rect(
                x,
                y,
                width,
                68f
            ),
            "Prototype jouable — Chapitre 1 : Les Murmures de l'Acier\nFonderie Katsuhiro • Tokyo • 1889",
            subtitleStyle
        );

        y += 95f;

        if (DemoCheckpointPersistence.HasCheckpoint)
        {
            if (GUI.Button(
                new Rect(
                    x,
                    y,
                    width,
                    46f
                ),
                "Continuer",
                buttonStyle
            ))
            {
                StartGame(false);
            }

            y += 55f;
        }

        if (GUI.Button(
            new Rect(
                x,
                y,
                width,
                46f
            ),
            "Nouvelle partie",
            buttonStyle
        ))
        {
            StartGame(true);
        }

        y += 55f;

        if (GUI.Button(
            new Rect(
                x,
                y,
                width,
                46f
            ),
            "Qualité : " +
            DemoQualityManager.CurrentPresetLabel,
            buttonStyle
        ))
        {
            DemoQualityManager.EnsureInstance()
                .CyclePreset();
        }

        y += 55f;

        if (GUI.Button(
            new Rect(
                x,
                y,
                width,
                46f
            ),
            "Galerie des personnages",
            buttonStyle
        ))
        {
            galleryOpen = true;
            galleryIndex = 0;
        }

        y += 55f;

        if (GUI.Button(
            new Rect(
                x,
                y,
                width,
                46f
            ),
            "Quitter",
            buttonStyle
        ))
        {
            QuitApplication();
        }

        GUI.Label(
            new Rect(
                x,
                panel.yMax - 90f,
                width,
                62f
            ),
            "A/D : mouvement • Espace : saut • J : attaque • I : lourd • Shift : esquive • K : Kikai-Yūrei • L : spéciale\nÉchap : pause • F3 : performances • F4 : playtest\n" + DemoBuildInfo.FullLabel,
            smallStyle
        );
    }

    private void DrawBackground()
    {
        if (kenjiroTexture != null)
        {
            GUI.DrawTexture(
                new Rect(
                    0f,
                    0f,
                    Screen.width,
                    Screen.height
                ),
                kenjiroTexture,
                ScaleMode.ScaleAndCrop
            );
        }
        else
        {
            GUI.Box(
                new Rect(
                    0f,
                    0f,
                    Screen.width,
                    Screen.height
                ),
                ""
            );
        }

        GUI.color =
            new Color(
                0f,
                0f,
                0f,
                0.38f
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
    }

    private void DrawGallery()
    {
        Texture2D texture =
            GetGalleryTexture();

        float imageWidth =
            Screen.width * 0.64f;

        float imageHeight =
            Screen.height * 0.72f;

        Rect imageRect =
            new Rect(
                (Screen.width - imageWidth) * 0.5f,
                Screen.height * 0.11f,
                imageWidth,
                imageHeight
            );

        GUI.Box(
            new Rect(
                imageRect.x - 18f,
                imageRect.y - 54f,
                imageRect.width + 36f,
                imageRect.height + 118f
            ),
            ""
        );

        GUI.Label(
            new Rect(
                imageRect.x,
                imageRect.y - 44f,
                imageRect.width,
                38f
            ),
            galleryNames[galleryIndex],
            galleryTitleStyle
        );

        if (texture != null)
        {
            GUI.DrawTexture(
                imageRect,
                texture,
                ScaleMode.ScaleToFit
            );
        }

        float buttonY =
            imageRect.yMax + 14f;

        if (GUI.Button(
            new Rect(
                imageRect.x,
                buttonY,
                120f,
                40f
            ),
            "◀ Précédent"
        ))
        {
            galleryIndex =
                (galleryIndex + 2) % 3;
        }

        if (GUI.Button(
            new Rect(
                imageRect.xMax - 120f,
                buttonY,
                120f,
                40f
            ),
            "Suivant ▶"
        ))
        {
            galleryIndex =
                (galleryIndex + 1) % 3;
        }

        if (GUI.Button(
            new Rect(
                Screen.width * 0.5f - 70f,
                buttonY,
                140f,
                40f
            ),
            "Retour"
        ))
        {
            galleryOpen = false;
        }
    }

    private Texture2D GetGalleryTexture()
    {
        switch (galleryIndex)
        {
            case 1:
                return yukiTexture;

            case 2:
                return takedaTexture;

            default:
                return kenjiroTexture;
        }
    }

    private void StartGame(
        bool resetCheckpoint
    )
    {
        if (resetCheckpoint)
            DemoCheckpointPersistence.ClearCheckpoint();

        DemoPlaytestTelemetry.BeginRun(
            resetCheckpoint
                ? "Nouvelle partie"
                : "Continuer"
        );

        Time.timeScale = 1f;

        DemoSceneLoader.Load(
            gameplayScene
        );
    }

    private void QuitApplication()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
