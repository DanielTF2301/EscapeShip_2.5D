using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Checkpoint : MonoBehaviour
{
    [Tooltip("Optional. Where the ship reappears. If empty, this object's position plus Respawn Offset is used.")]
    [SerializeField] private Transform respawnPoint;

    [Tooltip("Only used when Respawn Point is empty. Keep it above a solid checkpoint so the ship does not reappear inside it.")]
    [SerializeField] private Vector3 respawnOffset = new Vector3(0f, 1f, 0f);

    [Header("Feedback (optional)")]
    [SerializeField] private AudioClip activateClip;
    [SerializeField] private ParticleSystem activateParticles;

    bool activated;

    void OnTriggerEnter(Collider other)
    {
        Activate(other);
    }

    void OnCollisionEnter(Collision collision)
    {
        Activate(collision.collider);
    }

    void Activate(Collider other)
    {
        if (activated) return;

        // Only the ship counts (bullets and enemies are ignored).
        if (other.GetComponentInParent<PlayerHealth>() == null) return;

        activated = true;
        Vector3 position = respawnPoint != null ? respawnPoint.position : transform.position + respawnOffset;

        // After a respawn the ship starts touching this checkpoint: don't repeat the effects.
        if (CheckpointManager.IsCurrent(position)) return;

        CheckpointManager.SetCheckpoint(position);

        if (activateParticles != null)
        {
            activateParticles.Play();
        }

        if (activateClip != null)
        {
            AudioSource.PlayClipAtPoint(activateClip, transform.position);
        }

        Debug.Log("Checkpoint reached");
    }
}