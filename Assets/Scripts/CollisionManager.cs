using System.Collections;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{

    [Header("Audio Settings")]
    [SerializeField] private AudioClip explosionAudioClip;
    [SerializeField] private AudioClip landingAudioClip;

    [Header("Particle Systems")]
    [SerializeField] private ParticleSystem explosionParticleSystem;
    [SerializeField] private ParticleSystem landingParticleSystem;

    [Header("Restart Settings")]
    [Tooltip("Minimum seconds before the next level loads. It always waits for the whole landing clip.")]
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
        endSound.outputAudioMixerGroup = VolumeManager.SfxGroup;
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
                CompleteLevel();
                break;
            case "Explosion":
                Explode();
                break;
            default:                
                break;
        }
    }

    /// <summary>
    /// Level completed: landing music, then the next level.
    /// </summary>
    private void CompleteLevel()
    {
        if (!isControllable) return;

        // The checkpoint of this level is no longer needed.
        CheckpointManager.Clear();

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.CompleteLevel();
        }

        ManageCollision(landingAudioClip, landingParticleSystem, landingReloadDelay, true);
    }

    /// <summary>
    /// Blows up the ship: explosion sound and particles, then the level restarts.
    /// Public so other scripts (for example PlayerHealth at zero lives) can trigger it.
    /// </summary>
    public void Explode()
    {
        if (!isControllable) return;

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.RegisterDeath();
        }

        ManageCollision(explosionAudioClip, explosionParticleSystem, explosionReloadDelay, false);
    }

    /// <summary>
    /// Disables player control, stops the engine sound, plays the clip on its own AudioSource,
    /// plays the particles and, once the whole clip has played, goes to the next level or restarts this one.
    /// </summary>
    private void ManageCollision(AudioClip audioClip, ParticleSystem particleSystem, float minDelay, bool goToNextLevel)
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

        Debug.Log($"Changing scene in {delay:0.0}s");
        StartCoroutine(ChangeSceneRoutine(delay, goToNextLevel));
    }

    /// <summary>
    /// Waits so the sound can be heard, then loads the next level or reloads the current one.
    /// </summary>
    private IEnumerator ChangeSceneRoutine(float delay, bool goToNextLevel)
    {
        yield return new WaitForSeconds(delay);

        if (goToNextLevel)
        {
            LevelManager.LoadNextLevel();
        }
        else
        {
            LevelManager.ReloadLevel();
        }
    }

}