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
            completedStages = new List<int>(),
            playerLevel = 1,
            playerXp = 0,
            totalGold = 0
        };

        // Export from ProgressState
        for (int i = 0; i < ProgressState.TotalStages; i++)
        {
            if (ProgressState.IsStageCompleted(i))
                data.completedStages.Add(i);
        }

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
        foreach (int stageIdx in data.completedStages)
        {
            if (stageIdx >= 0 && stageIdx < ProgressState.TotalStages)
                ProgressState.MarkStageCompleted(stageIdx);
        }

        Debug.Log($"Save loaded from {SavePath}: {data.completedStages.Count} stages completed.");
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
