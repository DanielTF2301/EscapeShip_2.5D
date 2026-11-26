using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

// Put it on an empty object in the MainMenu scene and hook the buttons to its public methods.
public class MainMenu : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject optionsPanel;

    [Header("Info")]
    [SerializeField] private Button continueButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private TMP_Text bestScoreText;
    [SerializeField] private TMP_Text maxLevelText;

    void Start()
    {
        // Coming from the pause menu the game can be frozen.
        Time.timeScale = 1f;
        AudioListener.pause = false;

        ShowMain();

        bool canContinue = SaveSystem.GetMaxLevel() > 1 || SaveSystem.HasCheckpoint();
        if (continueButton != null)
        {
            continueButton.interactable = canContinue;
        }

        if (bestScoreText != null)
        {
            bestScoreText.text = "Best highscore: " + SaveSystem.GetMaxScore();
        }

        if (maxLevelText != null)
        {
            maxLevelText.text = "Last level: " + SaveSystem.GetMaxLevel();
        }

#if UNITY_WEBGL
        // Application.Quit does nothing in a browser.
        if (quitButton != null)
        {
            quitButton.gameObject.SetActive(false);
        }
#endif
    }

    public void NewGame()
    {
        SaveSystem.ClearCheckpoint();
        GameSession.ResetRun();
        LevelManager.LoadLevel(1);
    }

    public void Continue()
    {
        GameSession.ResetRun();
        LevelManager.LoadLevel(SaveSystem.GetMaxLevel());
    }

    public void ShowOptions()
    {
        mainPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    public void ShowMain()
    {
        optionsPanel.SetActive(false);
        mainPanel.SetActive(true);
    }

    public void OpenCredits()
    {
        SceneManager.LoadScene(LevelManager.CreditsScene);
    }

    public void Quit()
    {
        Application.Quit();
    }
}