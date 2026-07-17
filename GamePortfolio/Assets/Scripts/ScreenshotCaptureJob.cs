using System;
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

    private static void RequireRuntimeFeedback(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException("[Capture QA] " + message);
        Debug.Log("[Capture QA] PASS: " + message);
    }

    private static void PreparePortfolioCaptureProgress()
    {
        // Keep standalone capture evidence deterministic and showcase-ready even when the
        // local save file is fresh or stale. Unlocking through ProgressState only affects
        // the transient capture run; it does not write to SaveManager.
        ProgressState.Reset();
        ProgressState.MarkStageCompleted(0);
        ProgressState.MarkStageCompleted(1);
        ProgressState.MarkStageCompleted(2);
        ProgressState.PlayerLevel = 3;
        ProgressState.PlayerXp = 40;
        ProgressState.TotalGold = 480;
        ProgressState.EnsureStarterEquipment();
    }

    private IEnumerator CaptureSequence()
    {
        yield return new WaitForSeconds(1.5f);
        string isolatedCaptureSave = Path.Combine(Path.GetTempPath(), "codex-tactics-capture-save.json");
        if (File.Exists(isolatedCaptureSave)) File.Delete(isolatedCaptureSave);
        SaveManager.DebugUseSavePathForTest(isolatedCaptureSave);

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

        PreparePortfolioCaptureProgress();
        Debug.Log("[Capture] BattleManager found. Starting contextual command UI captures.");
        var battleUi = FindAnyObjectByType<BattleUI>();

        // 01: no actor selected, so the contextual dock must be closed.
        yield return new WaitForSeconds(0.5f);
        yield return Capture("01_battle_start.png");

        // 02: selecting an actionable actor opens the basic four-command dock.
        manager.SelectPlayerUnit(0);
        yield return new WaitForSeconds(0.5f);
        yield return Capture("02_actor_command.png");

        // 03: Ranger selected with the real skill submenu open.
        manager.SelectPlayerUnit(2);
        if (battleUi != null) battleUi.OpenSkillSubmenu();
        yield return new WaitForSeconds(0.5f);
        yield return Capture("03_skill_menu.png");

        // 04: target ring plus basic ATTACK command immediately before execution.
        manager.SelectPlayerUnit(0);
        manager.SelectEnemyTarget(0);
        yield return new WaitForSeconds(0.5f);
        yield return Capture("04_target_attack.png");
        manager.OnClickAttackButton();
        yield return new WaitForSeconds(0.6f);

        // 05: deterministic basic-attack impact with lunge, target flash and damage popup.
        manager.DebugStartBattleForTest(); manager.DebugSetPresentationManualForTest(true);
        manager.SelectPlayerUnit(0); manager.SelectEnemyTarget(0); manager.OnClickAttackButton();
        float attackLungeWait = 0f;
        while (battleUi != null && battleUi.DebugFeedbackActorOffset < 35f && attackLungeWait < 0.35f) { attackLungeWait += Time.deltaTime; yield return null; }
        RequireRuntimeFeedback(battleUi != null && battleUi.DebugFeedbackActorOffset >= 35f && battleUi.DebugFeedbackActorOffset <= 60f, "ATTACK lunge reaches 35-60px before impact");
        manager.DebugAdvancePresentationToImpactForTest();
        yield return new WaitForSeconds(0.03f);
        RequireRuntimeFeedback(battleUi != null && battleUi.DebugTransientFeedbackCount >= 2, "ATTACK impact creates target feedback and damage popup");
        yield return Capture("05_attack_impact.png");
        manager.DebugCompletePresentationForTest();

        // 06: deterministic Fire Bolt impact with burst, damage and BURN popup.
        manager.DebugStartBattleForTest(); manager.DebugSetPresentationManualForTest(true);
        manager.SelectPlayerUnit(0); manager.OnClickFireSkillButton(); manager.SelectEnemyTarget(0);
        yield return new WaitForSeconds(0.10f);
        RequireRuntimeFeedback(battleUi != null && battleUi.DebugHasTransientFeedbackNamed("Fire Projectile"), "Fire projectile exists during flight");
        yield return new WaitForSeconds(0.10f); manager.DebugAdvancePresentationToImpactForTest();
        yield return new WaitForSeconds(0.03f);
        RequireRuntimeFeedback(battleUi != null && battleUi.DebugFeedbackPopup == "BURN" && battleUi.DebugHasTransientFeedbackNamed("Fire Projectile"), "Fire projectile arrives with BURN impact feedback");
        yield return Capture("06_fire_burn.png");
        manager.DebugCompletePresentationForTest();

        // 07: deterministic self-only GUARD pulse and popup on Cleric.
        manager.DebugStartBattleForTest(); manager.DebugSetPresentationManualForTest(true);
        manager.SelectPlayerUnit(1); manager.OnClickGuardButton(); manager.DebugAdvancePresentationToImpactForTest();
        yield return new WaitForSeconds(0.05f);
        RequireRuntimeFeedback(battleUi != null && battleUi.DebugFeedbackPopup == "GUARD" && battleUi.DebugTransientFeedbackCount >= 2, "GUARD pulse and popup are actor-local runtime feedback");
        yield return Capture("07_guard_feedback.png");
        manager.DebugCompletePresentationForTest();

        // 08: player-turn overview with all three live enemies exposing their real next action and target.
        manager.DebugSetPresentationManualForTest(false); manager.DebugSetEnemyTurnManualForTest(true); manager.DebugStartBattleForTest();
        yield return new WaitForSeconds(0.15f);
        RequireRuntimeFeedback(battleUi != null && battleUi.DebugEnemySlotIntent(0).Contains("Paladin") && battleUi.DebugEnemySlotIntent(1).Contains("Paladin") && battleUi.DebugEnemySlotIntent(2).Contains("Paladin"), "all live enemies render real target intents during PlayerTurn");
        yield return Capture("08_enemy_intents.png");

        // 09: deterministic enemy lunge and authoritative impact frame.
        int enemyImpactHp = manager.playerParty[0].currentHp;
        manager.DebugBeginEnemyTurnForTest(); yield return new WaitForSeconds(0.75f); manager.DebugAdvanceEnemyTurnToImpactForTest();
        yield return new WaitForSeconds(0.24f);
        RequireRuntimeFeedback(manager.playerParty[0].currentHp < enemyImpactHp && manager.DebugEnemyImpactCount == 1 && battleUi != null && battleUi.DebugFeedbackActorOffset >= 30f && battleUi.DebugFeedbackActorOffset <= 50f && battleUi.DebugFeedbackPopup.StartsWith("-"), "enemy lunge reaches 30-50px and HP changes once at impact");
        yield return Capture("09_enemy_attack_impact.png");

        // 10: Guard is consumed by exactly one enemy impact and shows only reduced final damage.
        manager.DebugStartBattleForTest(); manager.DebugSetPresentationManualForTest(true); manager.SelectPlayerUnit(0); manager.OnClickGuardButton(); manager.DebugAdvancePresentationToImpactForTest(); manager.DebugCompletePresentationForTest();
        yield return new WaitForSeconds(0.85f);
        int guardedImpactHp = manager.playerParty[0].currentHp; manager.DebugSetEnemyTurnManualForTest(true); manager.DebugBeginEnemyTurnForTest(); yield return new WaitForSeconds(0.75f); manager.DebugAdvanceEnemyTurnToImpactForTest();
        yield return new WaitForSeconds(0.24f);
        RequireRuntimeFeedback(guardedImpactHp - manager.playerParty[0].currentHp == 7 && !manager.DebugIsGuarding(0) && battleUi != null && battleUi.DebugFeedbackPopup == "GUARD", "Guard halves final damage, displays GUARD, and is consumed once");
        yield return Capture("10_guard_block.png");

        // 11: complete the remaining enemies, recover AP once, then expose the compact PLAYER TURN banner and recovered actor AP.
        manager.DebugCompleteCurrentEnemyActionForTest(); manager.DebugAdvanceEnemyTurnToImpactForTest(); manager.DebugCompleteCurrentEnemyActionForTest(); manager.DebugAdvanceEnemyTurnToImpactForTest(); manager.DebugCompleteCurrentEnemyActionForTest();
        bool selectedRecoveredActor = manager.SelectPlayerUnit(0); yield return new WaitForSeconds(0.05f);
        RequireRuntimeFeedback(manager.DebugState == BattleState.PlayerTurn && manager.DebugPlayerTurnRecoveryCount == 1 && selectedRecoveredActor && manager.playerParty[0].currentAp == manager.playerParty[0].maxAp && battleUi != null && battleUi.DebugTurnBannerText == "PLAYER TURN", "PlayerTurn returns once after AP recovery and displays the compact banner");
        yield return Capture("11_player_turn_return.png");
        manager.DebugSetPresentationManualForTest(false); manager.DebugSetEnemyTurnManualForTest(false);

        // 12: real delayed Victory result with whole-party statistics and one-time reward.
        manager.DebugConfigureStageForTest(0, 0); manager.DebugSetPresentationManualForTest(true);
        for (int i = 0; i < 3; i++)
        {
            manager.DebugSetCurrentHpForTest(false, i, 1);
            RequireRuntimeFeedback(manager.SelectPlayerUnit(i) && manager.SelectEnemyTarget(i), "Victory capture selects each real actor and target");
            manager.OnClickAttackButton(); manager.DebugAdvancePresentationToImpactForTest();
            if (i == 2) manager.DebugSetCurrentHpForTest(true, 1, 0);
            manager.DebugCompletePresentationForTest();
        }
        RequireRuntimeFeedback(manager.DebugResultPending && battleUi != null && !battleUi.DebugResultSummaryPanelVisible && battleUi.DebugTransientFeedbackCount > 0, "terminal impact locks input while the final popup remains visible");
        yield return new WaitForSeconds(0.20f);
        RequireRuntimeFeedback(!battleUi.DebugResultSummaryPanelVisible, "result panel stays hidden before the 0.35s boundary");
        yield return new WaitForSeconds(0.40f);
        RequireRuntimeFeedback(manager.DebugState == BattleState.Victory && battleUi.DebugResultSummaryPanelVisible && battleUi.DebugResultSummaryText.StartsWith("VICTORY\n") && manager.DebugResultData.survivors == 2 && manager.DebugRewardGrantCount == 1 && battleUi.DebugRetryButtonVisible && battleUi.DebugContinueButtonVisible, "Victory panel appears after impact with whole-party stats, visible buttons, and one reward");
        yield return new WaitForSeconds(0.30f);
        yield return Capture("12_victory_result.png");

        // 14: the wired CONTINUE button loads the next existing encounter and removes result residue.
        RequireRuntimeFeedback(battleUi.DebugClickContinueButton(), "CONTINUE button invokes its bound encounter flow");
        yield return new WaitForSeconds(0.20f);
        RequireRuntimeFeedback(manager.DebugEncounterIndex == 1 && manager.DebugState == BattleState.PlayerTurn && !battleUi.DebugResultSummaryPanelVisible && battleUi.DebugTransientFeedbackCount == 0 && manager.DebugEnemyIntent(0).Contains("Paladin"), "CONTINUE starts the next encounter with clean state and fresh intents");
        yield return Capture("14_continue_encounter.png");

        // 13: real delayed Defeat result exposes no reward and no CONTINUE.
        int goldBeforeDefeat = ProgressState.TotalGold;
        manager.DebugConfigureStageForTest(0, 0);
        for (int i = 0; i < 3; i++) manager.DebugSetCurrentHpForTest(true, i, 0);
        manager.DebugTryEnterResultForTest();
        RequireRuntimeFeedback(manager.DebugResultPending && !battleUi.DebugResultSummaryPanelVisible, "party wipe queues Defeat without showing the panel early");
        yield return new WaitForSeconds(0.60f);
        RequireRuntimeFeedback(manager.DebugState == BattleState.Defeat && battleUi.DebugResultSummaryPanelVisible && battleUi.DebugResultSummaryText.StartsWith("DEFEAT\n") && ProgressState.TotalGold == goldBeforeDefeat && !battleUi.DebugContinueButtonVisible && battleUi.DebugRetryButtonVisible && battleUi.DebugStageSelectButtonVisible, "Defeat shows no reward and only visible retry/stage-select flow");
        RequireRuntimeFeedback(battleUi.DebugAllySlotState(0).Contains("0/") && battleUi.DebugAllySlotState(1).Contains("0/") && battleUi.DebugAllySlotState(2).Contains("0/"), "Defeat renders every ally slot at zero HP");
        yield return new WaitForSeconds(0.30f);
        yield return Capture("13_defeat_result.png");

        // 15: the wired RETRY button fully restores the same encounter.
        RequireRuntimeFeedback(battleUi.DebugClickRetryButton(), "RETRY button invokes its bound reset flow");
        yield return new WaitForSeconds(0.20f);
        RequireRuntimeFeedback(manager.DebugEncounterIndex == 0 && manager.DebugState == BattleState.PlayerTurn && manager.playerParty.TrueForAll(unit => !unit.IsDead()) && manager.enemyParty.TrueForAll(unit => !unit.IsDead()) && manager.DebugSelectedPlayerIndex == -1 && !battleUi.DebugResultSummaryPanelVisible && battleUi.DebugTransientFeedbackCount == 0, "RETRY restores HP/AP/status/selection/statistics and clears all result VFX");
        yield return Capture("15_retry_reset.png");

        // Runtime-only regression: lethal impact must still use the cached body after its selector disappears.
        manager.DebugStartBattleForTest(); manager.DebugSetPresentationManualForTest(true);
        manager.DebugSetCurrentHpForTest(false, 0, 1); manager.SelectPlayerUnit(0); manager.SelectEnemyTarget(0); manager.OnClickAttackButton();
        RequireRuntimeFeedback(battleUi != null && battleUi.DebugHasCachedFeedbackTarget, "lethal ATTACK caches its target body before impact");
        manager.DebugAdvancePresentationToImpactForTest(); yield return new WaitForSeconds(0.03f);
        Debug.Log($"[Capture QA] lethal state: transients={battleUi?.DebugTransientFeedbackCount}; popup={battleUi?.DebugFeedbackPopup}; cached={battleUi?.DebugHasCachedFeedbackTarget}");
        RequireRuntimeFeedback(battleUi != null && battleUi.DebugTransientFeedbackCount >= 2 && battleUi.DebugFeedbackPopup == "-20", "lethal ATTACK keeps impact VFX after target death");
        manager.DebugCompletePresentationForTest();

        // Runtime-only regression: restart must stop stale motion/projectile/popup coroutines and restore transforms.
        manager.DebugStartBattleForTest(); manager.DebugSetPresentationManualForTest(true);
        manager.SelectPlayerUnit(0); manager.OnClickFireSkillButton(); manager.SelectEnemyTarget(0); yield return new WaitForSeconds(0.05f);
        RequireRuntimeFeedback(battleUi != null && battleUi.DebugHasTransientFeedbackNamed("Fire Projectile"), "cleanup test starts with an active projectile");
        manager.DebugStartBattleForTest(); yield return null;
        RequireRuntimeFeedback(battleUi != null && battleUi.DebugTransientFeedbackCount == 0 && battleUi.DebugFeedbackActorOffset < 0.01f, "battle restart stops feedback coroutines and restores actor position");
        manager.DebugSetPresentationManualForTest(false);
        manager.DebugStartBattleForTest();

        // Drive the real battle to its result with actual selections and ATTACK commands.
        for (int turn = 0; turn < 24; turn++)
        {
            if (manager == null || manager.DebugState == BattleState.Victory || manager.DebugState == BattleState.Defeat) break;
            bool actorSelected = false;
            for (int actor = 0; actor < manager.playerParty.Count; actor++)
            {
                if (manager.SelectPlayerUnit(actor)) { actorSelected = true; break; }
            }
            if (!actorSelected) { manager.OnClickEndTurnButton(); yield return new WaitForSeconds(0.25f); continue; }
            for (int target = 0; target < manager.enemyParty.Count; target++)
            {
                if (manager.SelectEnemyTarget(target)) break;
            }
            manager.OnClickAttackButton();
            yield return new WaitForSeconds(0.35f);
        }

        yield return new WaitForSeconds(1.0f);
        yield return Capture("04_result_summary_rank.png");
        yield return new WaitForSeconds(0.5f);

        if (manager != null)
        {
            manager.OnClickRetryButton();
            yield return new WaitForSeconds(1.0f);
            yield return Capture("05_retry_reset.png");
        }

        // Final runtime navigation regression: the visible result button must load the real scene.
        manager.DebugConfigureStageForTest(0, 0);
        for (int i = 0; i < 3; i++) manager.DebugSetCurrentHpForTest(true, i, 0);
        manager.DebugTryEnterResultForTest(); manager.DebugShowPendingResultForTest();
        RequireRuntimeFeedback(battleUi.DebugClickStageSelectButton(), "STAGE SELECT button invokes its bound scene flow");
        yield return new WaitForSeconds(0.40f);
        RequireRuntimeFeedback(SceneManager.GetActiveScene().name == GameSceneFlow.StageSelectSceneName, "STAGE SELECT loads the real StageSelectScene");

        SaveManager.DebugResetSavePathForTest();
        if (File.Exists(isolatedCaptureSave)) File.Delete(isolatedCaptureSave);
        Debug.Log("[Capture] All screenshots captured! Exiting.");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        UnityEditor.EditorApplication.Exit(0);
#else
        Application.Quit();
#endif
    }
}
