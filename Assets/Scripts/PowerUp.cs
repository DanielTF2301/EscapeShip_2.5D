using UnityEngine;

public enum PowerUpType
{
    Life,
    Ammo,
    Fuel
}

// Put it on any object (a sphere, a model...). It becomes a trigger, spins and floats,
// and when the ship touches it, it gives lives, ammo or fuel. If the ship does not need it
// (full lives, full ammo, full tank) the pickup is left where it is.
[RequireComponent(typeof(Collider))]
public class PowerUp : MonoBehaviour
{
    [SerializeField] private PowerUpType type = PowerUpType.Life;
    [Tooltip("Lives, bullets or fuel units, depending on the type.")]
    [SerializeField] private int amount = 1;

    [Header("Feedback")]
    [SerializeField] private AudioClip pickupClip;
    [Tooltip("Optional. A child Particle System with Play On Awake turned off.")]
    [SerializeField] private ParticleSystem pickupParticles;

    [Header("Animation")]
    [SerializeField] private float spinSpeed = 90f;
    [SerializeField] private float bobHeight = 0.25f;
    [SerializeField] private float bobSpeed = 2f;

    Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
        GetComponent<Collider>().isTrigger = true;
    }

    void Update()
    {
        transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime, Space.World);
        transform.position = startPosition + Vector3.up * (Mathf.Sin(Time.time * bobSpeed) * bobHeight);
    }

    void OnTriggerEnter(Collider other)
    {
        // Only the ship can pick it up (bullets and enemies are ignored).
        PlayerHealth health = other.GetComponentInParent<PlayerHealth>();
        if (health == null) return;

        bool used = false;

        switch (type)
        {
            case PowerUpType.Life:
            {
                used = health.AddLife(amount);
                break;
            }
            case PowerUpType.Ammo:
            {
                PlayerShooting shooting = other.GetComponentInParent<PlayerShooting>();
                used = shooting != null && shooting.AddAmmo(amount);
                break;
            }
            case PowerUpType.Fuel:
            {
                PlayerFuel fuel = other.GetComponentInParent<PlayerFuel>();
                used = fuel != null && fuel.Refuel(amount);
                break;
            }
        }

        if (!used) return;

        VolumeManager.PlaySfxAtPoint(pickupClip, transform.position);

        if (pickupParticles != null)
        {
            // Let the particles finish after the pickup disappears.
            pickupParticles.transform.SetParent(null);
            pickupParticles.Play();
            Destroy(pickupParticles.gameObject, 2f);
        }

        gameObject.SetActive(false);
    }
}