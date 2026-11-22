using UnityEngine;

public static class GameSession
{
    /// <summary>
    /// Score banked from the levels already completed in this run.
    /// </summary>
    public static int TotalScore { get; private set; }

    // Keeps the data clean between Play sessions even if domain reload is disabled.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetState()
    {
        TotalScore = 0;
    }

    public static void Bank(int levelScore)
    {
        TotalScore += levelScore;
    }

    public static void ResetRun()
    {
        TotalScore = 0;
    }
}