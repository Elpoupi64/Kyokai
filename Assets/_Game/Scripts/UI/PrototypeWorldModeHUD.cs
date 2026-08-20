using UnityEngine;

public class PrototypeWorldModeHUD : MonoBehaviour
{
    [SerializeField] private KikaiWorldManager worldManager;

    private GUIStyle titleStyle;
    private GUIStyle bodyStyle;

    private void Awake()
    {
        if (worldManager == null)
            worldManager = FindAnyObjectByType<KikaiWorldManager>();
    }

    private void OnGUI()
    {
        if (worldManager == null)
            return;

        if (titleStyle == null)
        {
            titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 18,
                fontStyle = FontStyle.Bold
            };

            bodyStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 14
            };
        }

        GUI.Box(new Rect(18f, 18f, 310f, 88f), "");

        string mode = worldManager.IsEthereal
            ? "MONDE ÉTHÉRIQUE"
            : "MONDE NORMAL";

        GUI.Label(new Rect(32f, 29f, 280f, 28f), "KIKAI-YŪREI : " + mode, titleStyle);
        GUI.Label(new Rect(32f, 61f, 270f, 22f), "K : basculer entre les deux mondes", bodyStyle);
        GUI.Label(new Rect(32f, 82f, 270f, 20f), "Les plateformes spectrales ont une collision réelle.", bodyStyle);
    }
}
