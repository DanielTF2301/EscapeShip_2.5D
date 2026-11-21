using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    void Start()
    {
        if (!CheckpointManager.TryGetCheckpoint(out Vector3 checkpoint))
        {
            return;
        }

        // Keep the ship's own depth (Z); only X and Y come from the checkpoint.
        Vector3 target = new Vector3(checkpoint.x, checkpoint.y, transform.position.z);
        transform.position = target;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.position = target;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}