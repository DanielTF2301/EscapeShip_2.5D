using UnityEngine;

// Fuel of the ship. PlayerMovement spends it while thrusting. Fuel pickups (PowerUp) refill it,
// and so does resting on the LaunchPad (so the player can never get stuck without fuel).
public class PlayerFuel : MonoBehaviour
{
    [SerializeField] private float maxFuel = 100f;
    [Tooltip("Fuel spent per second of thrust. 6 = about 16 seconds with a full tank.")]
    [SerializeField] private float consumptionPerSecond = 6f;
    [Tooltip("Fuel per second recovered while the ship rests on the LaunchPad. 0 = no refuel there.")]
    [SerializeField] private float launchPadRefuelPerSecond = 25f;

    float currentFuel;

    public bool HasFuel => currentFuel > 0f;
    public float Fraction => maxFuel > 0f ? currentFuel / maxFuel : 0f;

    void Awake()
    {
        currentFuel = maxFuel;
    }

    /// <summary>
    /// Spends fuel for the given amount of seconds of thrust.
    /// </summary>
    public void Consume(float seconds)
    {
        currentFuel = Mathf.Max(0f, currentFuel - consumptionPerSecond * seconds);
    }

    /// <summary>
    /// Adds fuel. Returns false if the tank was already full.
    /// </summary>
    public bool Refuel(float amount)
    {
        if (currentFuel >= maxFuel) return false;

        currentFuel = Mathf.Min(maxFuel, currentFuel + amount);
        return true;
    }

    void OnCollisionStay(Collision collision)
    {
        if (launchPadRefuelPerSecond > 0f && collision.gameObject.CompareTag("LaunchPad"))
        {
            Refuel(launchPadRefuelPerSecond * Time.fixedDeltaTime);
        }
    }

    void OnGUI()
    {
        Rect back = new Rect(16f, 134f, 220f, 18f);

        GUI.color = new Color(0f, 0f, 0f, 0.6f);
        GUI.DrawTexture(back, Texture2D.whiteTexture);

        GUI.color = Fraction > 0.25f ? new Color(0.18f, 0.89f, 0.9f) : new Color(1f, 0.35f, 0.2f);
        GUI.DrawTexture(new Rect(back.x + 2f, back.y + 2f, (back.width - 4f) * Fraction, back.height - 4f), Texture2D.whiteTexture);

        GUI.color = Color.white;
        GUI.Label(new Rect(back.x, back.y - 20f, 220f, 20f), HasFuel ? "Fuel" : "NO FUEL");
    }
}