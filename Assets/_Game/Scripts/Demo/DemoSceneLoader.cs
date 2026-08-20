using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DemoSceneLoader : MonoBehaviour
{
    private static DemoSceneLoader instance;

    private bool loading;
    private float progress;
    private string targetScene;

    private GUIStyle titleStyle;
    private GUIStyle smallStyle;

    public static void Load(
        string sceneName
    )
    {
        if (string.IsNullOrWhiteSpace(sceneName))
            return;

        DemoPlaytestTelemetry.RecordSceneLoadRequested(
            sceneName
        );

        EnsureInstance().Begin(sceneName);
    }

    private static DemoSceneLoader EnsureInstance()
    {
        if (instance != null)
            return instance;

        GameObject root =
            new GameObject("DemoSceneLoader_v16");

        instance =
            root.AddComponent<DemoSceneLoader>();

        DontDestroyOnLoad(root);

        return instance;
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
    }

    private void Begin(
        string sceneName
    )
    {
        if (loading)
            return;

        targetScene = sceneName;
        StartCoroutine(LoadRoutine());
    }

    private IEnumerator LoadRoutine()
    {
        loading = true;
        progress = 0f;
        Time.timeScale = 1f;

        AsyncOperation operation =
            SceneManager.LoadSceneAsync(
                targetScene
            );

        if (operation == null)
        {
            loading = false;
            yield break;
        }

        operation.allowSceneActivation = false;

        float minimumDisplay = 0.45f;
        float timer = 0f;

        while (operation.progress < 0.9f ||
               timer < minimumDisplay)
        {
            timer += Time.unscaledDeltaTime;

            progress =
                Mathf.Clamp01(
                    operation.progress / 0.9f
                );

            yield return null;
        }

        progress = 1f;
        yield return new WaitForSecondsRealtime(0.10f);

        operation.allowSceneActivation = true;

        while (!operation.isDone)
            yield return null;

        loading = false;
    }

    private void OnGUI()
    {
        if (!loading)
            return;

        EnsureStyles();

        GUI.color =
            new Color(
                0.025f,
                0.022f,
                0.026f,
                0.98f
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

        GUI.Label(
            new Rect(
                0f,
                Screen.height * 0.40f,
                Screen.width,
                46f
            ),
            "FONDERIE KATSUHIRO",
            titleStyle
        );

        float barWidth =
            Mathf.Min(
                560f,
                Screen.width * 0.60f
            );

        float barX =
            (Screen.width - barWidth) * 0.5f;

        float barY =
            Screen.height * 0.52f;

        GUI.Box(
            new Rect(
                barX,
                barY,
                barWidth,
                20f
            ),
            ""
        );

        GUI.DrawTexture(
            new Rect(
                barX + 3f,
                barY + 3f,
                (barWidth - 6f) * progress,
                14f
            ),
            Texture2D.whiteTexture
        );

        GUI.Label(
            new Rect(
                0f,
                barY + 38f,
                Screen.width,
                30f
            ),
            "Chargement... " +
            Mathf.RoundToInt(progress * 100f) +
            " %",
            smallStyle
        );

        GUI.Label(
            new Rect(
                0f,
                Screen.height - 54f,
                Screen.width,
                26f
            ),
            DemoBuildInfo.FullLabel,
            smallStyle
        );
    }

    private void EnsureStyles()
    {
        if (titleStyle != null)
            return;

        titleStyle =
            new GUIStyle(GUI.skin.label)
            {
                fontSize = 28,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                normal =
                {
                    textColor = Color.white
                }
            };

        smallStyle =
            new GUIStyle(GUI.skin.label)
            {
                fontSize = 13,
                alignment = TextAnchor.MiddleCenter,
                normal =
                {
                    textColor =
                        new Color(
                            0.85f,
                            0.80f,
                            0.72f
                        )
                }
            };
    }
}
