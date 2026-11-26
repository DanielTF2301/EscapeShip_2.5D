using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Credits scene. The text scrolls up by itself and, when it is gone, goes back to the main menu.
// Esc or the Back button (hooked to BackToMenu) skip it.
// The credits text RectTransform: anchored at the bottom center, pivot (0.5, 1), with a
// Content Size Fitter (Horizontal: Unconstrained, Vertical: Preferred Size) so its height follows the text.
public class CreditsScreen : MonoBehaviour
{
    [SerializeField] private RectTransform creditsText;
    [SerializeField] private float scrollSpeed = 110f;

    RectTransform parentRect;

    void Start()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;

        parentRect = (RectTransform)creditsText.parent;

        // Make sure the text height is calculated before anything is measured.
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(creditsText);

        // The text starts just below the screen.
        creditsText.anchoredPosition = Vector2.zero;
    }

    void Update()
    {
        creditsText.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;

        // Measured every frame: the rect height can still be the old saved one (50) right after
        // the scene loads, so the real preferred height of the text is used as well.
        float textHeight = Mathf.Max(creditsText.rect.height, LayoutUtility.GetPreferredHeight(creditsText));
        float endY = parentRect.rect.height + textHeight;

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