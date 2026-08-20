using UnityEngine;
using UnityEngine.SceneManagement;

public static class DemoCheckpointPersistence
{
    private const string Prefix = "Katsuhiro.Demo.Checkpoint.";

    private const string HasKey = Prefix + "Has";
    private const string SceneKey = Prefix + "Scene";
    private const string XKey = Prefix + "X";
    private const string YKey = Prefix + "Y";
    private const string ZKey = Prefix + "Z";

    public static bool HasCheckpoint =>
        PlayerPrefs.GetInt(HasKey, 0) == 1;

    public static string SavedScene =>
        PlayerPrefs.GetString(SceneKey, string.Empty);

    public static void SaveCheckpoint(
        Vector3 worldPosition
    )
    {
        string sceneName =
            SceneManager.GetActiveScene().name;

        PlayerPrefs.SetInt(HasKey, 1);
        PlayerPrefs.SetString(SceneKey, sceneName);
        PlayerPrefs.SetFloat(XKey, worldPosition.x);
        PlayerPrefs.SetFloat(YKey, worldPosition.y);
        PlayerPrefs.SetFloat(ZKey, worldPosition.z);
        PlayerPrefs.Save();
    }

    public static bool TryGetForCurrentScene(
        out Vector3 position
    )
    {
        position = Vector3.zero;

        if (!HasCheckpoint)
            return false;

        string currentScene =
            SceneManager.GetActiveScene().name;

        if (string.IsNullOrEmpty(currentScene) ||
            SavedScene != currentScene)
        {
            return false;
        }

        position =
            new Vector3(
                PlayerPrefs.GetFloat(XKey, 0f),
                PlayerPrefs.GetFloat(YKey, 0f),
                PlayerPrefs.GetFloat(ZKey, 0f)
            );

        return true;
    }

    public static void ClearCheckpoint()
    {
        PlayerPrefs.DeleteKey(HasKey);
        PlayerPrefs.DeleteKey(SceneKey);
        PlayerPrefs.DeleteKey(XKey);
        PlayerPrefs.DeleteKey(YKey);
        PlayerPrefs.DeleteKey(ZKey);
        PlayerPrefs.Save();
    }
}
