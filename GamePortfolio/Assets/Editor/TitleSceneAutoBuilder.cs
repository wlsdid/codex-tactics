using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class TitleSceneAutoBuilder
{
    private const string TitleScenePath = "Assets/Scenes/TitleScene.unity";
    private const string BattleScenePath = "Assets/Scenes/BattleScene.unity";

    [MenuItem("Tools/Codex Tactics/Create Title Scene")]
    public static void CreateTitleScene()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        scene.name = "TitleScene";

        Camera camera = CreateMainCamera();
        GameObject titleManager = new GameObject("TitleManager");
        titleManager.AddComponent<TitleManager>();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, TitleScenePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // Add both scenes to Build Settings if not present
        EnsureSceneInBuildSettings(TitleScenePath, 0);
        EnsureSceneInBuildSettings(BattleScenePath, 1);

        EditorUtility.DisplayDialog(
            "TitleScene Created",
            "Assets/Scenes/TitleScene.unity created and added to Build Settings (index 0).\n\n" +
            "Press Play to test the title screen, or set it as the startup scene.",
            "OK"
        );
    }

    private static Camera CreateMainCamera()
    {
        GameObject camObj = new GameObject("Main Camera");
        Camera camera = camObj.AddComponent<Camera>();
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0.05f, 0.06f, 0.09f);
        camera.orthographic = true;
        camera.orthographicSize = 5f;
        camObj.tag = "MainCamera";
        return camera;
    }

    private static void EnsureSceneInBuildSettings(string scenePath, int desiredIndex)
    {
        var scenes = EditorBuildSettings.scenes;
        bool found = false;
        foreach (var s in scenes)
        {
            if (s.path == scenePath)
            {
                found = true;
                break;
            }
        }

        if (!found)
        {
            var newScenes = new EditorBuildSettingsScene[scenes.Length + 1];
            for (int i = 0; i < scenes.Length; i++)
                newScenes[i] = scenes[i];
            newScenes[newScenes.Length - 1] = new EditorBuildSettingsScene(scenePath, true);

            // Sort so TitleScene is at index 0
            System.Array.Sort(newScenes, (a, b) =>
            {
                if (a.path == TitleScenePath) return -1;
                if (b.path == TitleScenePath) return 1;
                return 0;
            });

            EditorBuildSettings.scenes = newScenes;
        }
    }
}
