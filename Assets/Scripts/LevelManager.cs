using UnityEngine;
using UnityEngine.SceneManagement;

public static class LevelManager
{
    public const string MenuScene = "MainMenu";
    public const string CreditsScene = "Credits";

    static readonly string[] LevelScenes = { "Level1", "Level2", "Level3" };

    public static int LevelCount => LevelScenes.Length;

    /// <summary>
    /// Number (starting at 1) of the active level, or 0 if the active scene is not a level.
    /// </summary>
    public static int CurrentLevelNumber()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        for (int i = 0; i < LevelScenes.Length; i++)
        {
            if (LevelScenes[i] == sceneName)
            {
                return i + 1;
            }
        }

        return 0;
    }

    /// <summary>
    /// Back to the start of the current level.
    /// </summary>
    public static void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public static void LoadLevel(int number)
    {
        number = Mathf.Clamp(number, 1, LevelScenes.Length);
        SceneManager.LoadScene(LevelScenes[number - 1]);
    }

    /// <summary>
    /// Goes to the next level. After the last one it goes to the credits, or to the menu if there are no credits yet.
    /// </summary>
    public static void LoadNextLevel()
    {
        int current = CurrentLevelNumber();

        if (current == 0)
        {
            Debug.LogWarning("The active scene is not Level1, Level2 or Level3. Reloading it instead.");
            ReloadLevel();
            return;
        }

        if (current < LevelScenes.Length)
        {
            LoadLevel(current + 1);
            return;
        }

        if (Application.CanStreamedLevelBeLoaded(CreditsScene))
        {
            SceneManager.LoadScene(CreditsScene);
        }
        else if (Application.CanStreamedLevelBeLoaded(MenuScene))
        {
            SceneManager.LoadScene(MenuScene);
        }
        else
        {
            Debug.LogWarning("Last level finished and there is no Credits or MainMenu scene yet. Going back to level 1.");
            LoadLevel(1);
        }
    }
}