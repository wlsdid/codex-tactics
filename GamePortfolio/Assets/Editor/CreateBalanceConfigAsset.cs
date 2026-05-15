using UnityEngine;
using UnityEditor;

public static class CreateBalanceConfigAsset
{
    [MenuItem("Tools/Codex Tactics/Create Battle Balance Config Asset")]
    public static void CreateAsset()
    {
        var config = ScriptableObject.CreateInstance<BattleBalanceConfig>();
        string path = "Assets/Settings/BattleBalanceConfig.asset";
        AssetDatabase.CreateAsset(config, path);
        AssetDatabase.SaveAssets();
        Debug.Log($"Created BattleBalanceConfig at {path}");
    }
}