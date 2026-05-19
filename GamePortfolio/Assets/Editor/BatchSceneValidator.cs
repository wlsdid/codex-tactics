using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Validates that all scenes have their required components.
/// </summary>
public static class BatchSceneValidator
{
    public static void ValidateScenes()
    {
        Debug.Log("=== Scene Validation ===");
        string[] scenes = { "Assets/Scenes/TitleScene.unity", "Assets/Scenes/StageSelectScene.unity", "Assets/Scenes/BattleScene.unity", "Assets/Scenes/SettingsScene.unity" };

        foreach (string path in scenes)
        {
            var scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
            SceneManager.SetActiveScene(scene);

            bool ok = true;

            int canvasCount = 0;
            int cameras = 0;
            GameObject[] roots = scene.GetRootGameObjects();
            foreach (var root in roots)
            {
                if (root.GetComponent<Canvas>() != null) canvasCount++;
                if (root.GetComponent<Camera>() != null) cameras++;
            }

            string checks = $"Canvas={canvasCount}, Cameras={cameras}";

            if (canvasCount == 0) { ok = false; checks += " [MISSING CANVAS]"; }

            Debug.Log($"Scene: {path} — {checks} — {(ok ? "OK" : "FAIL")}");
        }

        Debug.Log("=== Validation Complete ===");
    }
}
