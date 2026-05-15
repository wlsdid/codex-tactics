using System.Collections;
using System.IO;
using System.Reflection;
using UnityEngine;

public class ScreenshotCaptureJob : MonoBehaviour
{
    private static string _capturesDir;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Initialize()
    {
        // Run in both Editor batchmode AND standalone player with -capture flag
        bool shouldCapture = Application.isBatchMode;
        if (!shouldCapture)
        {
            // Check command-line args for standalone player
            var args = System.Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-capture" || args[i] == "--capture")
                {
                    shouldCapture = true;
                    break;
                }
            }
        }

        if (!shouldCapture)
            return;

        Debug.Log("[Capture] Initializing screenshot capture job...");

        _capturesDir = Path.Combine(Application.dataPath.Replace("/Assets", ""), "Docs/Captures");
        Directory.CreateDirectory(_capturesDir);

        var go = new GameObject("ScreenshotCaptureJob");
        DontDestroyOnLoad(go);
        go.AddComponent<ScreenshotCaptureJob>();
    }

    private void Start()
    {
        StartCoroutine(CaptureSequence());
    }

    private void Capture(string filename)
    {
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
        yield return new WaitForSeconds(2.0f);

        var manager = FindFirstObjectByType<BattleManager>();
        if (manager == null)
        {
            Debug.LogError("[Capture] BattleManager not found!");
            yield break;
        }

        Debug.Log("[Capture] BattleManager found. Starting capture.");

        Capture("01_battle_start.png");
        yield return new WaitForSeconds(1.0f);

        manager.OnClickFireSkillButton();
        yield return new WaitForSeconds(4.0f);
        Capture("02_fire_skill_burn.png");
        yield return new WaitForSeconds(0.5f);

        manager.OnClickGuardButton();
        yield return new WaitForSeconds(3.0f);
        Capture("03_guard_status.png");
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
        Capture("04_result_summary_rank.png");
        yield return new WaitForSeconds(0.5f);

        if (manager != null)
        {
            manager.OnClickRetryButton();
            yield return new WaitForSeconds(2.0f);
            Capture("05_retry_reset.png");
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
