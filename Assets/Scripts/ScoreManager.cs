using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [SerializeField] private int levelCompleteBonus = 100;

    bool completed;

    /// <summary>
    /// Points earned in the current attempt of this level.
    /// </summary>
    public int LevelScore { get; private set; }

    /// <summary>
    /// Points banked from previous levels plus the current attempt.
    /// </summary>
    public int TotalScore => GameSession.TotalScore + LevelScore;

    void Awake()
    {
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    void Start()
    {
        // Remember the highest level reached.
        int level = LevelManager.CurrentLevelNumber();
        if (level > 0)
        {
            SaveSystem.SaveMaxLevel(level);
        }
    }

    public void AddScore(int points)
    {
        if (completed) return;
        LevelScore += points;
    }

    /// <summary>
    /// Call when the level is finished: adds the bonus, banks the points and saves the best score.
    /// </summary>
    public void CompleteLevel()
    {
        if (completed) return;
        completed = true;

        LevelScore += levelCompleteBonus;
        GameSession.Bank(LevelScore);
        LevelScore = 0;

        SaveSystem.SaveMaxScore(GameSession.TotalScore);
    }

    /// <summary>
    /// Call when the ship dies: the best score is saved with what was earned so far.
    /// </summary>
    public void RegisterDeath()
    {
        SaveSystem.SaveMaxScore(TotalScore);
    }

    void OnGUI()
    {
        GUI.Label(new Rect(16f, 40f, 260f, 32f), "Score: " + TotalScore);
        GUI.Label(new Rect(16f, 64f, 260f, 32f), "Best: " + Mathf.Max(SaveSystem.GetMaxScore(), TotalScore));
    }

    // Right-click the component title > Reset Save Data (handy while testing).
    [ContextMenu("Reset Save Data")]
    void ResetSaveData()
    {
        SaveSystem.ClearAll();
        GameSession.ResetRun();
        Debug.Log("Save data reset");
    }
}