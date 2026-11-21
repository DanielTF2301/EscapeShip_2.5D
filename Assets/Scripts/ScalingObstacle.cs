
using UnityEngine;

public class ScalingObstacle : MonoBehaviour
{
    public enum ScaleAxis
    {
        X,
        Y,
        Z,
        All
    }

    [Header("Scaling Settings")]
    [SerializeField] private float scaleSpeed = 2f;
    [SerializeField] private float minScale = 0.5f;
    [SerializeField] private float maxScale = 1.5f;
    [SerializeField] private ScaleAxis scaleAxis = ScaleAxis.All;

    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;
    }

    void Update()
    {
        float scaleFactor = Mathf.Lerp(minScale, maxScale, Mathf.PingPong(Time.time * scaleSpeed, 1f));

        Vector3 newScale = originalScale;

        switch (scaleAxis)
        {
            case ScaleAxis.X:
                newScale.x = originalScale.x * scaleFactor;
                break;

            case ScaleAxis.Y:
                newScale.y = originalScale.y * scaleFactor;
                break;

            case ScaleAxis.Z:
                newScale.z = originalScale.z * scaleFactor;
                break;

            case ScaleAxis.All:
                newScale = originalScale * scaleFactor;
                break;
        }

        transform.localScale = newScale;
    }
}
