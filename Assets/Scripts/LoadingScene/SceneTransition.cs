using UnityEngine.SceneManagement;

public static class SceneTransition
{
    public static string TargetScence { get; private set; }

    public static void LoadScene(string sceneName)
    {
        TargetScence = sceneName;
        SceneManager.LoadScene("LoadingScene");
    }
}
