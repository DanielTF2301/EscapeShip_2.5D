using UnityEngine;
using UnityEngine.SceneManagement;

// Remembers the last checkpoint reached. It is saved with SaveSystem (PlayerPrefs), so it survives
// level reloads and also closing the game.
// It only counts for the scene where it was reached: in any other level it is ignored.
public static class CheckpointManager
{
    public static void SetCheckpoint(Vector3 position)
    {
        SaveSystem.SaveCheckpoint(SceneManager.GetActiveScene().path, position);
    }

    /// <summary>
    /// True when this exact checkpoint is already the active one in the current level.
    /// </summary>
    public static bool IsCurrent(Vector3 position)
    {
        return TryGetCheckpoint(out Vector3 saved)
            && (saved - position).sqrMagnitude < 0.0001f;
    }

    /// <summary>
    /// Gives the checkpoint of the current level, if there is one.
    /// A checkpoint saved in another level is ignored.
    /// </summary>
    public static bool TryGetCheckpoint(out Vector3 position)
    {
        return SaveSystem.TryGetCheckpoint(SceneManager.GetActiveScene().path, out position);
    }

    public static void Clear()
    {
        SaveSystem.ClearCheckpoint();
    }
}