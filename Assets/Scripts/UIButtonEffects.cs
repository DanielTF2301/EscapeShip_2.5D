using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButtonEffects : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private float hoverScale = 1.06f;
    [SerializeField] private float pressedScale = 0.96f;
    [SerializeField] private float speed = 14f;

    Selectable selectable;
    Vector3 baseScale = Vector3.one;
    float current = 1f;
    float target = 1f;
    bool hovering;

    void Awake()
    {
        selectable = GetComponent<Selectable>();
        baseScale = transform.localScale;
    }

    void OnEnable()
    {
        hovering = false;
        current = 1f;
        target = 1f;
        transform.localScale = baseScale;
    }

    void Update()
    {
        current = Mathf.Lerp(current, target, 1f - Mathf.Exp(-speed * Time.unscaledDeltaTime));
        transform.localScale = baseScale * current;
    }

    bool CanReact()
    {
        return selectable == null || selectable.IsInteractable();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hovering = true;
        if (CanReact()) target = hoverScale;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovering = false;
        target = 1f;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (CanReact()) target = pressedScale;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        target = hovering && CanReact() ? hoverScale : 1f;
    }
}