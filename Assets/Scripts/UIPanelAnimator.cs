using System.Collections;
using UnityEngine;

public class UIPanelAnimator : MonoBehaviour
{
    [SerializeField] private float duration = 0.2f;
    [SerializeField] private float startScale = 0.92f;

    CanvasGroup group;

    void Awake()
    {
        group = GetComponent<CanvasGroup>();
        if (group == null)
        {
            group = gameObject.AddComponent<CanvasGroup>();
        }
    }

    void OnEnable()
    {
        StopAllCoroutines();
        StartCoroutine(Play());
    }

    IEnumerator Play()
    {
        float time = 0f;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            float k = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(time / duration));

            group.alpha = k;
            transform.localScale = Vector3.one * Mathf.Lerp(startScale, 1f, k);
            yield return null;
        }

        group.alpha = 1f;
        transform.localScale = Vector3.one;
    }
}