using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

// Credits scene. The text scrolls up by itself and, when it is gone, goes back to the main menu.
// Esc or the Back button (hooked to BackToMenu) skip it.
// The credits text RectTransform: anchored at the bottom center, pivot (0.5, 1), with a
// Content Size Fitter (Vertical Fit: Preferred Size) so its height follows the text.
public class CreditsScreen : MonoBehaviour
{
    [SerializeField] private RectTransform creditsText;
    [SerializeField] private float scrollSpeed = 60f;

    float endY;

    void Start()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;

        // The text starts just below the screen and ends once it has fully left through the top.
        RectTransform parent = (RectTransform)creditsText.parent;
        creditsText.anchoredPosition = Vector2.zero;
        endY = parent.rect.height + creditsText.rect.height;
    }

    void Update()
    {
        creditsText.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;

        bool skip = Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame;

        if (skip || creditsText.anchoredPosition.y >= endY)
        {
            BackToMenu();
        }
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene(LevelManager.MenuScene);
    }
}