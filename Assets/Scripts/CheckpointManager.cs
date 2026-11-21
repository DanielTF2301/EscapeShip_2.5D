using UnityEngine;
using UnityEngine.SceneManagement;

public static class CheckpointManager
{
    static bool hasCheckpoint;
    static string checkpointScenePath;
    static Vector3 checkpointPosition;

    // Keeps the data clean between Play sessions even if domain reload is disabled.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetState()
    {
        hasCheckpoint = false;
    }

    public static void SetCheckpoint(Vector3 position)
    {
        hasCheckpoint = true;
        checkpointScenePath = SceneManager.GetActiveScene().path;
        checkpointPosition = position;
    }

    /// <summary>
    /// True when this exact checkpoint is already the active one in the current level.
    /// </summary>
    public static bool IsCurrent(Vector3 position)
    {
        return hasCheckpoint
            && checkpointScenePath == SceneManager.GetActiveScene().path
            && (checkpointPosition - position).sqrMagnitude < 0.0001f;
    }

    /// <summary>
    /// Gives the checkpoint of the current level, if there is one.
    /// A checkpoint from another level is discarded.
    /// </summary>
    public static bool TryGetCheckpoint(out Vector3 position)
    {
        if (hasCheckpoint && checkpointScenePath == SceneManager.GetActiveScene().path)
        {
            position = checkpointPosition;
            return true;
        }

        hasCheckpoint = false;
        position = Vector3.zero;
        return false;
    }

    public static void Clear()
    {
        hasCheckpoint = false;
    }
}