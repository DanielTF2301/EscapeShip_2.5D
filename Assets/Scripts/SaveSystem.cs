using UnityEngine;

public static class SaveSystem
{
    const string MaxScoreKey = "MaxScore";
    const string MaxLevelKey = "MaxLevel";
    const string HasCheckpointKey = "HasCheckpoint";
    const string CheckpointSceneKey = "CheckpointScene";
    const string CheckpointXKey = "CheckpointX";
    const string CheckpointYKey = "CheckpointY";
    const string CheckpointZKey = "CheckpointZ";

    // ---------- Best score ----------

    public static int GetMaxScore()
    {
        return PlayerPrefs.GetInt(MaxScoreKey, 0);
    }

    /// <summary>
    /// Saves the score only if it beats the stored one. Returns true when there is a new record.
    /// </summary>
    public static bool SaveMaxScore(int score)
    {
        if (score <= GetMaxScore()) return false;

        PlayerPrefs.SetInt(MaxScoreKey, score);
        PlayerPrefs.Save();
        return true;
    }

    // ---------- Highest level reached (1-based) ----------

    public static int GetMaxLevel()
    {
        return PlayerPrefs.GetInt(MaxLevelKey, 1);
    }

    public static void SaveMaxLevel(int level)
    {
        if (level <= GetMaxLevel()) return;

        PlayerPrefs.SetInt(MaxLevelKey, level);
        PlayerPrefs.Save();
    }

    // ---------- Checkpoint (one, tied to the scene where it was reached) ----------

    public static void SaveCheckpoint(string scenePath, Vector3 position)
    {
        PlayerPrefs.SetInt(HasCheckpointKey, 1);
        PlayerPrefs.SetString(CheckpointSceneKey, scenePath);
        PlayerPrefs.SetFloat(CheckpointXKey, position.x);
        PlayerPrefs.SetFloat(CheckpointYKey, position.y);
        PlayerPrefs.SetFloat(CheckpointZKey, position.z);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Gives the saved checkpoint only if it belongs to the given scene.
    /// </summary>
    public static bool TryGetCheckpoint(string scenePath, out Vector3 position)
    {
        position = Vector3.zero;

        if (PlayerPrefs.GetInt(HasCheckpointKey, 0) == 0) return false;
        if (PlayerPrefs.GetString(CheckpointSceneKey, "") != scenePath) return false;

        position = new Vector3(
            PlayerPrefs.GetFloat(CheckpointXKey, 0f),
            PlayerPrefs.GetFloat(CheckpointYKey, 0f),
            PlayerPrefs.GetFloat(CheckpointZKey, 0f));
        return true;
    }

    public static void ClearCheckpoint()
    {
        PlayerPrefs.DeleteKey(HasCheckpointKey);
        PlayerPrefs.DeleteKey(CheckpointSceneKey);
        PlayerPrefs.DeleteKey(CheckpointXKey);
        PlayerPrefs.DeleteKey(CheckpointYKey);
        PlayerPrefs.DeleteKey(CheckpointZKey);
        PlayerPrefs.Save();
    }

    // ---------- Debug ----------

    /// <summary>
    /// Wipes score, level and checkpoint. Other keys (like volume settings) are left alone.
    /// </summary>
    public static void ClearAll()
    {
        PlayerPrefs.DeleteKey(MaxScoreKey);
        PlayerPrefs.DeleteKey(MaxLevelKey);
        ClearCheckpoint();
    }

    public static bool HasCheckpoint()
    {
        return PlayerPrefs.GetInt(HasCheckpointKey, 0) == 1;
    }
}