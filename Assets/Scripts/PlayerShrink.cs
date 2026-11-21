using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShrink : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputAction shrinkAction =
        new InputAction("Shrink", InputActionType.Button, "<Keyboard>/leftShift");

    [Header("Tamaño")]
    [SerializeField] private float shrunkScaleFactor = 0.5f;
    [SerializeField] private float scaleSpeed = 8f;

    [Header("Efectos mientras está encogido")]
    [SerializeField] private float damageTakenMultiplier = 2f;
    [SerializeField] private float shootRangeMultiplier = 0.5f;

    private Vector3 normalScale;
    private Vector3 shrunkScale;

    public bool IsShrunk { get; private set; }
    public float DamageTakenMultiplier => IsShrunk ? damageTakenMultiplier : 1f;
    public float ShootRangeMultiplier => IsShrunk ? shootRangeMultiplier : 1f;

    void Awake()
    {
        normalScale = transform.localScale;
        shrunkScale = normalScale * shrunkScaleFactor;
    }

    void OnEnable() => shrinkAction.Enable();
    void OnDisable() => shrinkAction.Disable();

    void Update()
    {
        IsShrunk = shrinkAction.IsPressed();

        Vector3 target = IsShrunk ? shrunkScale : normalScale;
        transform.localScale = Vector3.Lerp(transform.localScale, target, scaleSpeed * Time.deltaTime);
    }
}