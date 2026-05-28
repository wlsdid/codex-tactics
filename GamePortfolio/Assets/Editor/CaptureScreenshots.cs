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

        // Set build settings: include the portfolio flow scenes and start from Title.
        string titleScenePath = "Assets/Scenes/TitleScene.unity";
        string stageSelectScenePath = "Assets/Scenes/StageSelectScene.unity";
        string battleScenePath = "Assets/Scenes/BattleScene.unity";
        EditorSceneManager.OpenScene(titleScenePath);
        Debug.Log($"[Capture] Scene loaded: {titleScenePath}");

        // Build standalone player
        string buildPath = Path.Combine(projectPath, BuildDir, "CaptureRunner.exe");
        Directory.CreateDirectory(Path.GetDirectoryName(buildPath));

        var buildOptions = new BuildPlayerOptions
        {
            scenes = new[] { titleScenePath, stageSelectScenePath, battleScenePath },
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

        // Write helper files so external scripts can run the player with the correct output folder.
        string pathFile = Path.Combine(projectPath, "Builds", "_build_path.txt");
        File.WriteAllText(pathFile, buildPath);
        Debug.Log($"[Capture] Build path written to {pathFile}");

        string argsFile = Path.Combine(projectPath, "Builds", "_capture_args.txt");
        File.WriteAllText(argsFile,
            $"-screen-width 1920 -screen-height 1080 -capture -captureOutputDir \"{capturesPath}\"");
        Debug.Log($"[Capture] Capture args written to {argsFile}");

        // Save the build info
        EditorApplication.Exit(0);
    }
}
