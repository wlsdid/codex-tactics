using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.IO;

public static class CaptureScreenshots
{
    private const string BuildDir = "Builds/CaptureBuild";

    public static void Run()
    {
        string projectPath = Application.dataPath.Replace("/Assets", "");
        Debug.Log($"[Capture] Project path: {projectPath}");

        // Ensure captures directory exists
        string capturesPath = Path.Combine(projectPath, "Docs/Captures");
        Directory.CreateDirectory(capturesPath);

        // Set build settings: include the battle scene
        string scenePath = "Assets/Scenes/BattleScene.unity";
        EditorSceneManager.OpenScene(scenePath);
        Debug.Log($"[Capture] Scene loaded: {scenePath}");

        // Build standalone player
        string buildPath = Path.Combine(projectPath, BuildDir, "CaptureRunner.exe");
        Directory.CreateDirectory(Path.GetDirectoryName(buildPath));

        var buildOptions = new BuildPlayerOptions
        {
            scenes = new[] { scenePath },
            locationPathName = buildPath,
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None
        };

        Debug.Log("[Capture] Building standalone player...");
        BuildReport report = BuildPipeline.BuildPlayer(buildOptions);

        if (report.summary.result != BuildResult.Succeeded)
        {
            Debug.LogError($"[Capture] Build failed: {report.summary.result}");
            EditorApplication.Exit(1);
            return;
        }

        Debug.Log($"[Capture] Build succeeded: {buildPath}");

        // Build succeeded — the build will be run externally (not from batchmode)
        // Write the path so external script can find it
        string pathFile = Path.Combine(projectPath, "Builds", "_build_path.txt");
        File.WriteAllText(pathFile, buildPath);
        Debug.Log($"[Capture] Build path written to {pathFile}");

        // Save the build info
        EditorApplication.Exit(0);
    }
}
