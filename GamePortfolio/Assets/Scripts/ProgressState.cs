using System.Collections.Generic;

/// <summary>
/// Tracks game progression across scene loads.
/// Static fields persist within a single play session (scene transitions).
/// Reset on entering/exiting Play Mode.
/// Future: replace with serialized save/load for persistent storage.
/// </summary>
public static class ProgressState
{
    /// <summary>
    /// Stage indices that have been fully cleared (all encounters defeated).
    /// Stage 0 is always unlocked by default.
    /// </summary>
    private static readonly HashSet<int> completedStages = new HashSet<int>();

    /// <summary>Total number of stages defined in the game.</summary>
    public static int TotalStages { get; set; } = 3;

    /// <summary>
    /// Check if a stage is unlocked for play.
    /// Stage 0 is always unlocked. Subsequent stages unlock when the previous stage is completed.
    /// </summary>
    public static bool IsStageUnlocked(int stageIndex)
    {
        if (stageIndex == 0) return true;
        return completedStages.Contains(stageIndex - 1);
    }

    /// <summary>
    /// Mark a stage as fully completed, unlocking the next stage.
    /// </summary>
    public static void MarkStageCompleted(int stageIndex)
    {
        if (stageIndex < 0) return;
        completedStages.Add(stageIndex);
    }

    /// <summary>
    /// Check if a stage has been completed before.
    /// </summary>
    public static bool IsStageCompleted(int stageIndex)
    {
        return completedStages.Contains(stageIndex);
    }

    /// <summary>
    /// Reset all progress. Useful for testing or new game.
    /// </summary>
    public static void Reset()
    {
        completedStages.Clear();
        TotalStages = 3;
    }

    // --- Debug accessors (for auto-test) ---

    public static int DebugCompletedStageCount => completedStages.Count;
    public static bool DebugIsStage0Completed => completedStages.Contains(0);
    public static bool DebugIsStage1Completed => completedStages.Contains(1);
}
