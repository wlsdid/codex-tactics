using System.Collections;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenshotCaptureJob : MonoBehaviour
{
    private static string _capturesDir;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Initialize()
    {
        // Run in Editor batchmode OR in a standalone player only when the capture flag is present.
        // Standalone captures should write back to the project Docs/Captures folder, not the
        // built player's *_Data folder, so automated README images never point at stale/hidden files.
        bool hasCaptureFlag = HasArg("-capture") || HasArg("--capture");
        bool shouldCapture = Application.isBatchMode || hasCaptureFlag;

        if (!shouldCapture)
            return;

        Debug.Log("[Capture] Initializing screenshot capture job...");

        _capturesDir = ResolveCapturesDirectory();
        Directory.CreateDirectory(_capturesDir);
        Debug.Log($"[Capture] Output directory: {_capturesDir}");

        var go = new GameObject("ScreenshotCaptureJob");
        DontDestroyOnLoad(go);
        go.AddComponent<ScreenshotCaptureJob>();
    }

    private void Start()
    {
        StartCoroutine(CaptureSequence());
    }

    private static bool HasArg(string name)
    {
        var args = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == name)
                return true;
        }

        return false;
    }

    private static string GetArgValue(string name)
    {
        var args = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length - 1; i++)
        {
            if (args[i] == name)
                return args[i + 1];
        }

        return string.Empty;
    }

    private static string ResolveCapturesDirectory()
    {
        string explicitDir = GetArgValue("-captureOutputDir");
        if (!string.IsNullOrWhiteSpace(explicitDir))
            return explicitDir;

        explicitDir = System.Environment.GetEnvironmentVariable("CODEX_TACTICS_CAPTURE_DIR");
        if (!string.IsNullOrWhiteSpace(explicitDir))
            return explicitDir;

        string projectDir = Application.dataPath.Replace("/Assets", "");
        if (!projectDir.EndsWith("_Data"))
            return Path.Combine(projectDir, "Docs/Captures");

        // Fallback for manually launched standalone builds without -captureOutputDir:
        // .../GamePortfolio/Builds/CaptureBuild/CaptureRunner_Data -> .../GamePortfolio/Docs/Captures
        DirectoryInfo dir = Directory.GetParent(projectDir);
        for (int i = 0; i < 2 && dir != null; i++)
            dir = dir.Parent;

        if (dir != null)
            return Path.Combine(dir.FullName, "Docs/Captures");

        return Path.Combine(projectDir, "Docs/Captures");
    }

    private IEnumerator Capture(string filename)
    {
        // ReadPixels must happen after rendering for the frame has completed. Capturing from a
        // normal coroutine tick can produce black frames or "not inside drawing frame" warnings.
        yield return new WaitForEndOfFrame();

        string path = Path.Combine(_capturesDir, filename);
        int w = Screen.width > 0 ? Screen.width : 1920;
        int h = Screen.height > 0 ? Screen.height : 1080;

        var tex = new Texture2D(w, h, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, w, h), 0, 0);
        tex.Apply();
        byte[] bytes = tex.EncodeToPNG();
        Destroy(tex);
        File.WriteAllBytes(path, bytes);
        Debug.Log($"[Capture] Saved ({bytes.Length} bytes): {path}");
    }

    private IEnumerator CaptureSequence()
    {
        yield return new WaitForSeconds(1.5f);

        yield return Capture("00_title_scene.png");
        yield return new WaitForSeconds(0.5f);

        SceneManager.LoadScene(GameSceneFlow.StageSelectSceneName);
        yield return new WaitForSeconds(1.5f);
        yield return Capture("00_stage_select_scene.png");
        yield return new WaitForSeconds(0.5f);

        SceneManager.LoadScene(GameSceneFlow.BattleSceneName);
        yield return new WaitForSeconds(2.0f);

        var manager = FindAnyObjectByType<BattleManager>();
        if (manager == null)
        {
            Debug.LogError("[Capture] BattleManager not found!");
            yield break;
        }

        Debug.Log("[Capture] BattleManager found. Starting capture.");

        yield return Capture("01_battle_start.png");
        yield return new WaitForSeconds(1.0f);

        manager.OnClickFireSkillButton();
        yield return new WaitForSeconds(4.0f);
        yield return Capture("02_fire_skill_burn.png");
        yield return new WaitForSeconds(0.5f);

        manager.OnClickGuardButton();
        yield return new WaitForSeconds(3.0f);
        yield return Capture("03_guard_status.png");
        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < 6; i++)
        {
            if (manager == null) break;

            var stateField = typeof(BattleManager).GetField("currentState",
                BindingFlags.NonPublic | BindingFlags.Instance);
            if (stateField != null)
            {
                var state = (BattleState)stateField.GetValue(manager);
                if (state == BattleState.Victory || state == BattleState.Defeat)
                    break;
            }

            manager.OnClickAttackButton();
            yield return new WaitForSeconds(3.0f);
        }

        yield return new WaitForSeconds(2.0f);
        yield return Capture("04_result_summary_rank.png");
        yield return new WaitForSeconds(0.5f);

        if (manager != null)
        {
            manager.OnClickRetryButton();
            yield return new WaitForSeconds(2.0f);
            yield return Capture("05_retry_reset.png");
        }

        Debug.Log("[Capture] All screenshots captured! Exiting.");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        UnityEditor.EditorApplication.Exit(0);
#else
        Application.Quit();
#endif
    }
}
