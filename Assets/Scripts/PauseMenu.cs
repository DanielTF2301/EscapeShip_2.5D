using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

// Put it on an always-active object of the level (for example the pause Canvas), not on the panels,
// because inactive objects do not run Update. Esc, P or the gamepad Start button toggle the pause.
public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;     // Resume / Options / Main menu buttons
    [SerializeField] private GameObject optionsPanel;   // volume sliders + back button

    bool isPaused;

    void Start()
    {
        SetPaused(false);
    }

    void Update()
    {
        bool keyboard = Keyboard.current != null
            && (Keyboard.current.escapeKey.wasPressedThisFrame || Keyboard.current.pKey.wasPressedThisFrame);
        bool gamepad = Gamepad.current != null && Gamepad.current.startButton.wasPressedThisFrame;

        if (keyboard || gamepad)
        {
            SetPaused(!isPaused);
        }
    }

    public void SetPaused(bool paused)
    {
        isPaused = paused;
        Time.timeScale = paused ? 0f : 1f;
        AudioListener.pause = paused;

        pausePanel.SetActive(paused);
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(false);
        }
    }

    public void Resume()
    {
        SetPaused(false);
        LevelManager.ReloadLevel();
    }

    public void ShowOptions()
    {
        pausePanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    public void HideOptions()
    {
        optionsPanel.SetActive(false);
        pausePanel.SetActive(true);
    }

    public void ToMainMenu()
    {
        SetPaused(false);
        SceneManager.LoadScene(LevelManager.MenuScene);
    }

    // Never leave the game frozen if the scene changes while paused.
    void OnDestroy()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
    }

    public void Restart()
    {
        SetPaused(false);
        LevelManager.ReloadLevel();
    }
}