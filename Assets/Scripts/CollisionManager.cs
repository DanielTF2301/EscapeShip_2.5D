using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CollisionManager : MonoBehaviour
{

    [Header("Audio Settings")]
    [SerializeField] private AudioClip explosionAudioClip;
    [SerializeField] private AudioClip landingAudioClip;

    [Header("Particle Systems")]
    [SerializeField] private ParticleSystem explosionParticleSystem;
    [SerializeField] private ParticleSystem landingParticleSystem;

    [Header("Restart Settings")]
    [Tooltip("Minimum seconds before the level restarts. It always waits for the whole landing clip.")]
    [SerializeField] private float landingReloadDelay = 1f;
    [Tooltip("Minimum seconds before the level restarts. It always waits for the whole explosion clip.")]
    [SerializeField] private float explosionReloadDelay = 1f;

    AudioSource audioSource;   // engine sound (other scripts call Stop() on it)
    AudioSource endSound;      // dedicated source for the landing and explosion sounds

    bool isControllable = true;


    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // Separate source so nobody can cut the landing/explosion sound with Stop().
        endSound = gameObject.AddComponent<AudioSource>();
        endSound.playOnAwake = false;
        endSound.loop = false;
        endSound.spatialBlend = 0f;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!isControllable) return;

        switch (collision.gameObject.tag)
        {
            case "LaunchPad":
                Debug.Log("LaunchPad Collision Detected");
                break;
            case "LandingPad":
                // Level completed: the next start has to be from the beginning.
                CheckpointManager.Clear();
                ManageCollision(landingAudioClip, landingParticleSystem, landingReloadDelay);
                break;
            case "Explosion":
                Explode();
                break;
            default:                
                break;
        }
    }

    /// <summary>
    /// Blows up the ship: explosion sound and particles, then the level restarts.
    /// Public so other scripts (for example PlayerHealth at zero lives) can trigger it.
    /// </summary>
    public void Explode()
    {
        if (!isControllable) return;
        ManageCollision(explosionAudioClip, explosionParticleSystem, explosionReloadDelay);
    }

    /// <summary>
    /// Disables player control, stops the engine sound, plays the clip on its own AudioSource,
    /// plays the particles and restarts the level once the whole clip has played.
    /// </summary>
    private void ManageCollision(AudioClip audioClip, ParticleSystem particleSystem, float minDelay)
    {
        isControllable = false;
        audioSource.Stop();

        float delay = minDelay;
        if (audioClip != null)
        {
            endSound.clip = audioClip;
            endSound.Play();
            delay = Mathf.Max(minDelay, audioClip.length);
        }

        if (particleSystem != null)
        {
            particleSystem.Play();
        }

        GetComponent<PlayerMovement>().enabled = false;

        Debug.Log($"Restarting level in {delay:0.0}s");
        StartCoroutine(ReloadLevel(delay));
    }

    /// <summary>
    /// Waits so the sound can be heard, then reloads the current scene (back to the start of the level).
    /// </summary>
    private IEnumerator ReloadLevel(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}