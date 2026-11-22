using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIStyler : MonoBehaviour
{
    [Header("Colors")]
    [SerializeField] private Color panelColor = new Color32(11, 18, 32, 245);
    [SerializeField] private Color accentColor = new Color32(45, 226, 230, 255);
    [SerializeField] private Color highlightColor = new Color32(255, 138, 61, 255);
    [SerializeField] private Color dimColor = new Color(0f, 0.01f, 0.03f, 0.7f);

    [Header("Windows")]
    [SerializeField] private Vector2 windowSize = new Vector2(820f, 900f);

    [Header("Text")]
    [Tooltip("Optional. A TextMeshPro font asset (for example Orbitron).")]
    [SerializeField] private TMP_FontAsset font;
    [Tooltip("Texts with at least this font size are treated as titles.")]
    [SerializeField] private float titleMinSize = 60f;

    [ContextMenu("Apply Style Now")]
    public void ApplyStyle()
    {
        // 1. Panels become windows.
        List<RectTransform> panels = new List<RectTransform>();
        foreach (Transform child in GetComponentsInChildren<Transform>(true))
        {
            if (child != transform && child.name.EndsWith("Panel") && child is RectTransform rect)
            {
                panels.Add(rect);
            }
        }

        foreach (RectTransform panel in panels)
        {
            StylePanel(panel);
        }

        // 2. Buttons, sliders and texts.
        foreach (Button button in GetComponentsInChildren<Button>(true))
        {
            StyleButton(button);
        }

        foreach (Slider slider in GetComponentsInChildren<Slider>(true))
        {
            StyleSlider(slider);
        }

        foreach (TMP_Text text in GetComponentsInChildren<TMP_Text>(true))
        {
            StyleText(text);
        }

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            UnityEditor.EditorUtility.SetDirty(gameObject);
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
        }
#endif
        Debug.Log("UI style applied");
    }

    // ---------- Panels ----------

    void StylePanel(RectTransform panel)
    {
        // The panel covers the whole screen; the visible window is the "Frame" child.
        panel.anchorMin = Vector2.zero;
        panel.anchorMax = Vector2.one;
        panel.offsetMin = Vector2.zero;
        panel.offsetMax = Vector2.zero;

        Image rootImage = panel.GetComponent<Image>();
        Sprite cardSprite = null;
        if (rootImage != null)
        {
            cardSprite = rootImage.sprite;
            rootImage.color = Color.clear;
        }

        // Dim: darkens the game behind the window.
        RectTransform dim = GetOrCreateChild(panel, "Dim");
        dim.SetSiblingIndex(0);
        dim.anchorMin = Vector2.zero;
        dim.anchorMax = Vector2.one;
        dim.offsetMin = new Vector2(-4000f, -4000f);
        dim.offsetMax = new Vector2(4000f, 4000f);
        GetOrAdd<Image>(dim.gameObject).color = dimColor;

        // Frame: the card with a glowing border.
        RectTransform frame = GetOrCreateChild(panel, "Frame");
        frame.SetSiblingIndex(1);
        frame.anchorMin = new Vector2(0.5f, 0.5f);
        frame.anchorMax = new Vector2(0.5f, 0.5f);
        frame.pivot = new Vector2(0.5f, 0.5f);
        frame.sizeDelta = windowSize;
        frame.anchoredPosition = Vector2.zero;

        Image frameImage = GetOrAdd<Image>(frame.gameObject);
        frameImage.color = panelColor;
        if (cardSprite != null)
        {
            frameImage.sprite = cardSprite;
            frameImage.type = Image.Type.Sliced;
        }

        Outline outline = GetOrAdd<Outline>(frame.gameObject);
        outline.effectColor = new Color(accentColor.r, accentColor.g, accentColor.b, 0.9f);
        outline.effectDistance = new Vector2(3f, -3f);

        // A thin orange line under the top edge.
        RectTransform bar = GetOrCreateChild(frame, "AccentBar");
        bar.anchorMin = new Vector2(0f, 1f);
        bar.anchorMax = new Vector2(1f, 1f);
        bar.pivot = new Vector2(0.5f, 1f);
        bar.sizeDelta = new Vector2(-80f, 6f);
        bar.anchoredPosition = new Vector2(0f, -28f);
        GetOrAdd<Image>(bar.gameObject).color = highlightColor;

        // Pop-in animation when the panel opens.
        GetOrAdd<UIPanelAnimator>(panel.gameObject);
    }

    // ---------- Buttons ----------

    void StyleButton(Button button)
    {
        Image image = button.targetGraphic as Image;
        if (image == null)
        {
            image = button.GetComponent<Image>();
        }

        button.transition = Selectable.Transition.ColorTint;
        ColorBlock colors = button.colors;
        colors.normalColor = new Color32(16, 34, 50, 255);
        colors.highlightedColor = new Color32(25, 181, 201, 255);
        colors.pressedColor = highlightColor;
        colors.selectedColor = new Color32(16, 34, 50, 255);
        colors.disabledColor = new Color32(60, 70, 80, 160);
        colors.colorMultiplier = 1f;
        colors.fadeDuration = 0.08f;
        button.colors = colors;

        if (image != null)
        {
            image.color = Color.white;

            Outline outline = GetOrAdd<Outline>(image.gameObject);
            outline.effectColor = new Color(accentColor.r, accentColor.g, accentColor.b, 0.85f);
            outline.effectDistance = new Vector2(2f, -2f);
        }

        TMP_Text label = button.GetComponentInChildren<TMP_Text>(true);
        if (label != null)
        {
            label.color = Color.white;
            label.fontStyle = FontStyles.Bold;
            label.alignment = TextAlignmentOptions.Center;
            label.characterSpacing = 6f;
        }

        GetOrAdd<UIButtonEffects>(button.gameObject);
    }

    // ---------- Sliders ----------

    void StyleSlider(Slider slider)
    {
        Transform background = slider.transform.Find("Background");
        if (background != null)
        {
            Image backgroundImage = background.GetComponent<Image>();
            if (backgroundImage != null)
            {
                backgroundImage.color = new Color32(6, 12, 20, 255);

                Outline outline = GetOrAdd<Outline>(backgroundImage.gameObject);
                outline.effectColor = new Color(accentColor.r, accentColor.g, accentColor.b, 0.5f);
                outline.effectDistance = new Vector2(1.5f, -1.5f);
            }
        }

        if (slider.fillRect != null)
        {
            Image fillImage = slider.fillRect.GetComponent<Image>();
            if (fillImage != null)
            {
                fillImage.color = accentColor;
            }
        }

        if (slider.handleRect != null)
        {
            Image handleImage = slider.handleRect.GetComponent<Image>();
            if (handleImage != null)
            {
                handleImage.color = Color.white;
            }

            slider.handleRect.sizeDelta = new Vector2(36f, slider.handleRect.sizeDelta.y);
        }

        // The handle (target graphic) is orange, white when hovered and cyan while dragging.
        slider.transition = Selectable.Transition.ColorTint;
        ColorBlock colors = slider.colors;
        colors.normalColor = highlightColor;
        colors.highlightedColor = Color.white;
        colors.pressedColor = accentColor;
        colors.selectedColor = highlightColor;
        colors.fadeDuration = 0.08f;
        slider.colors = colors;
    }

    // ---------- Texts ----------

    void StyleText(TMP_Text text)
    {
        text.raycastTarget = false;

        if (font != null)
        {
            text.font = font;
        }

        // Button labels are styled in StyleButton.
        if (text.GetComponentInParent<Button>() != null)
        {
            return;
        }

        bool isTitle = text.fontSize >= titleMinSize;
        text.color = isTitle ? accentColor : Color.white;
        text.fontStyle = isTitle ? FontStyles.Bold : FontStyles.Normal;
        text.characterSpacing = isTitle ? 12f : 4f;
    }

    // ---------- Helpers ----------

    static RectTransform GetOrCreateChild(RectTransform parent, string childName)
    {
        Transform existing = parent.Find(childName);
        if (existing != null)
        {
            return (RectTransform)existing;
        }

        GameObject go = new GameObject(childName, typeof(RectTransform));
        go.layer = parent.gameObject.layer;
        go.transform.SetParent(parent, false);
        return (RectTransform)go.transform;
    }

    static T GetOrAdd<T>(GameObject go) where T : Component
    {
        T component = go.GetComponent<T>();
        return component != null ? component : go.AddComponent<T>();
    }
}