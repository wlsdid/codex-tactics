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
    public static int TotalStages { get; set; } = 6;

    /// <summary>Current difficulty mode: 0 = Normal, 1 = Hard.</summary>
    public static int DifficultyMode { get; set; } = 0;
    public static string DifficultyLabel => DifficultyMode == 0 ? "Normal" : "Hard";
    public static float DifficultyHpMultiplier => DifficultyMode == 0 ? 1.0f : 1.5f;
    public static float DifficultyDamageMultiplier => DifficultyMode == 0 ? 1.0f : 1.3f;
    public static int DifficultyBreakGaugeMultiplier => DifficultyMode == 0 ? 1 : 2;

    /// <summary>Persistent run-wide player progression.</summary>
    public static int PlayerLevel { get; set; } = 1;
    public static int PlayerXp { get; set; } = 0;
    public static int TotalGold { get; set; } = 0;

    /// <summary>Skill unlock thresholds: stage index required to unlock each skill.</summary>
    public static bool IsSkillUnlocked(string skillName)
    {
        int completed = 0;
        for (int i = 0; i < TotalStages; i++) if (IsStageCompleted(i)) completed++;
        return skillName switch
        {
            "Slash" => true,
            "Fire Bolt" => completed >= 1,
            "Ice Lance" => completed >= 2,
            "Earth Wall" => completed >= 3,
            "Lightning Strike" => completed >= 4,
            _ => true
        };
    }

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

    public static List<int> GetCompletedStages()
    {
        return new List<int>(completedStages);
    }

    /// <summary>
    /// Reset all progress. Useful for testing or new game.
    /// </summary>
    public static void Reset()
    {
        completedStages.Clear();
        TotalStages = 6;
        PlayerLevel = 1;
        PlayerXp = 0;
        TotalGold = 0;
        EquipmentManager.Reset();
    }

    /// <summary>Ensure starter gear exists. Call once after Reset or when equipment is empty.</summary>
    public static void EnsureStarterEquipment()
    {
        if (EquipmentManager.Inventory.Count > 0) return;
        var weapon = EquipmentManager.GetEquipped(EquipmentSlot.Weapon);
        if (weapon != null && weapon.itemName.StartsWith("Training")) return;
        EquipmentManager.GiveStarterGear();
    }

    // --- Debug accessors (for auto-test) ---

    public static int DebugCompletedStageCount => completedStages.Count;
    public static bool DebugIsStage0Completed => completedStages.Contains(0);
    public static bool DebugIsStage1Completed => completedStages.Contains(1);
}
