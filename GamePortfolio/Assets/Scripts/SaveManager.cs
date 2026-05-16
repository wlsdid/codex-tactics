using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Manages persistent save/load of player progress.
/// Saves completed stages, player level, XP, and total gold to a JSON file.
/// </summary>
public static class SaveManager
{
    private static string SavePath => Path.Combine(Application.persistentDataPath, "save.json");

    [System.Serializable]
    private struct SaveData
    {
        public List<int> completedStages;
        public int playerLevel;
        public int playerXp;
        public int totalGold;
    }

    public static void Save()
    {
        var data = new SaveData
        {
            completedStages = ProgressState.GetCompletedStages(),
            playerLevel = ProgressState.PlayerLevel,
            playerXp = ProgressState.PlayerXp,
            totalGold = ProgressState.TotalGold
        };

        string json = JsonUtility.ToJson(data, prettyPrint: true);
        File.WriteAllText(SavePath, json);
        Debug.Log($"Save saved to {SavePath}");
    }

    public static void Load()
    {
        if (!File.Exists(SavePath))
        {
            Debug.Log("No save file found, starting fresh.");
            return;
        }

        string json = File.ReadAllText(SavePath);
        var data = JsonUtility.FromJson<SaveData>(json);

        // Import to ProgressState
        ProgressState.Reset();
        if (data.completedStages != null)
        {
            foreach (int stageIdx in data.completedStages)
            {
                if (stageIdx >= 0 && stageIdx < ProgressState.TotalStages)
                    ProgressState.MarkStageCompleted(stageIdx);
            }
        }

        ProgressState.PlayerLevel = data.playerLevel <= 0 ? 1 : data.playerLevel;
        ProgressState.PlayerXp = data.playerXp < 0 ? 0 : data.playerXp;
        ProgressState.TotalGold = data.totalGold < 0 ? 0 : data.totalGold;

        int completedCount = data.completedStages != null ? data.completedStages.Count : 0;
        Debug.Log($"Save loaded from {SavePath}: {completedCount} stages completed, Lv.{ProgressState.PlayerLevel}, {ProgressState.TotalGold}G.");
    }

    public static void ResetSave()
    {
        if (File.Exists(SavePath))
            File.Delete(SavePath);
        ProgressState.Reset();
        Debug.Log("Save reset to default.");
    }

    public static bool HasSaveFile()
    {
        return File.Exists(SavePath);
    }
}
