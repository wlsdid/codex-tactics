using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class BattleAutoTestRunner
{
    [MenuItem("Tools/Codex Tactics/Run Battle Logic Auto Test")]
    public static void RunBattleLogicAutoTest()
    {
        bool passed = true;
        string report = "Battle Logic Auto Test\n\n";

        GameObject root = new GameObject("BattleManager Auto Test Root");
        BattleManager battleManager = root.AddComponent<BattleManager>();
        BattleUI battleUI = root.AddComponent<BattleUI>();

        // Assign UI references to BattleUI (private fields via reflection)
        SetPrivateField(battleUI, "playerHpText", CreateText("Player HP Text"));
        SetPrivateField(battleUI, "playerHpSlider", CreateSlider("Player HP Slider"));
        SetPrivateField(battleUI, "playerApText", CreateText("Player AP Text"));
        SetPrivateField(battleUI, "playerApSlider", CreateSlider("Player AP Slider"));
        SetPrivateField(battleUI, "enemyHpText", CreateText("Enemy HP Text"));
        SetPrivateField(battleUI, "enemyHpSlider", CreateSlider("Enemy HP Slider"));
        SetPrivateField(battleUI, "playerStatusText", CreateText("Player Status Text"));
        SetPrivateField(battleUI, "enemyStatusText", CreateText("Enemy Status Text"));
        SetPrivateField(battleUI, "enemyIntentText", CreateText("Enemy Intent Text"));
        SetPrivateField(battleUI, "enemyBreakText", CreateText("Enemy Break Text"));
        SetPrivateField(battleUI, "enemyBreakSlider", CreateSlider("Enemy Break Slider"));
        SetPrivateField(battleUI, "runStatusText", CreateText("Run Status Text"));
        SetPrivateField(battleUI, "stageText", CreateText("Stage Text"));
        SetPrivateField(battleUI, "stageObjectiveText", CreateText("Stage Objective Text"));
        SetPrivateField(battleUI, "stageProgressText", CreateText("Stage Progress Text"));
        SetPrivateField(battleUI, "messageText", CreateText("Message Text"));
        SetPrivateField(battleUI, "impactText", CreateText("Impact Text"));
        SetPrivateField(battleUI, "skillHelpText", CreateText("Skill Help Text"));
        SetPrivateField(battleUI, "battleLogText", CreateText("Battle Log Text"));
        SetPrivateField(battleUI, "resultSummaryText", CreateText("Result Summary Text"));
        SetPrivateField(battleUI, "resultSummaryPanel", CreatePanel("Result Summary Panel"));
        SetPrivateField(battleUI, "attackButton", CreateButton("Attack Button"));
        SetPrivateField(battleUI, "fireSkillButton", CreateButton("Fire Skill Button"));
        SetPrivateField(battleUI, "guardButton", CreateButton("Guard Button"));
        SetPrivateField(battleUI, "endTurnButton", CreateButton("End Turn Button"));
        SetPrivateField(battleUI, "retryButton", CreateButton("Retry Button"));
        SetPrivateField(battleUI, "continueButton", CreateButton("Continue Button"));
        SetPrivateField(battleUI, "stageSelectButton", CreateButton("Stage Select Button"));

        // Link BattleUI to BattleManager
        SetPrivateField(battleManager, "battleUI", battleUI);

        battleManager.DebugStartBattleForTest();
        AppendCheck(ref passed, ref report, "Battle starts at the first slime encounter", battleManager.DebugStageText == "Stage 1-1: Slime Scout");
        AppendCheck(ref passed, ref report, "Run status starts as Stage 1 in progress", battleManager.DebugRunStatusText == "Run Status: Stage 1 In Progress");
        AppendCheck(ref passed, ref report, "Battle starts with a clear first encounter objective", battleManager.DebugStageObjectiveText == "Objective: Defeat Slime Scout");
        AppendCheck(ref passed, ref report, "Battle starts with encounter progress", battleManager.DebugStageProgressText == "Progress: Encounter 1/2 | Active");
        AppendCheck(ref passed, ref report, "Continue button is hidden during active battle", !battleManager.DebugContinueButtonVisible && !battleManager.DebugContinueButtonInteractable);
        AppendCheck(ref passed, ref report, "Stage Select button is hidden during active battle", !battleManager.DebugStageSelectButtonVisible && !battleManager.DebugStageSelectButtonInteractable);
        AppendCheck(ref passed, ref report, "Battle starts with full player HP percentage", battleManager.DebugPlayerHpText == "Hero HP: 100/100 (100%)");
        AppendCheck(ref passed, ref report, "Battle starts with full AP percentage", battleManager.DebugPlayerApText == "AP: 3/3 (100%)");
        AppendCheck(ref passed, ref report, "Battle starts with full enemy HP percentage", battleManager.DebugEnemyHpText == "Slime HP: 80/80 (100%)");
        AppendCheck(ref passed, ref report, "Player HP bar starts full", battleManager.DebugPlayerHpBarValue == 100f && battleManager.DebugPlayerHpBarMaxValue == 100f);
        AppendCheck(ref passed, ref report, "Player AP bar starts full", battleManager.DebugPlayerApBarValue == 3f && battleManager.DebugPlayerApBarMaxValue == 3f);
        AppendCheck(ref passed, ref report, "Enemy HP bar starts full", battleManager.DebugEnemyHpBarValue == 80f && battleManager.DebugEnemyHpBarMaxValue == 80f);
        AppendCheck(ref passed, ref report, "Result summary is cleared during active battle", string.IsNullOrEmpty(battleManager.DebugResultSummaryText));
        AppendCheck(ref passed, ref report, "Result summary panel is hidden during active battle", !battleManager.DebugResultSummaryPanelVisible);
        AppendCheck(ref passed, ref report, "Retry button is hidden during active battle", !battleManager.DebugRetryButtonVisible && !battleManager.DebugRetryButtonInteractable);
        AppendCheck(ref passed, ref report, "Player status starts as Ready", battleManager.DebugPlayerStatusText == "Status: Ready");
        AppendCheck(ref passed, ref report, "Enemy status starts as None", battleManager.DebugEnemyStatusText == "Status: None");
        AppendCheck(ref passed, ref report, "Enemy intent starts with normal attack preview", battleManager.DebugEnemyIntentText == "Next Enemy: Normal Attack (15)");
        AppendCheck(ref passed, ref report, "Skill help explains Attack, Fire Skill, Guard, and readable enemy pattern values", battleManager.DebugSkillHelpText.Contains("Slash: 20 power, 0 AP") && battleManager.DebugSkillHelpText.Contains("Fire Bolt: 30 power, 2 AP") && battleManager.DebugSkillHelpText.Contains("Guard: reduce next enemy attack") && battleManager.DebugSkillHelpText.Contains("Normal attack: 15 damage") && battleManager.DebugSkillHelpText.Contains("Heavy Slam: 30 damage every 3rd enemy turn"));
        AppendCheck(ref passed, ref report, "Battle log has a readable Recent Actions heading", battleManager.DebugBattleLogText.StartsWith("Recent Actions"));
        AppendCheck(ref passed, ref report, "Battle log records the latest player turn prompt", battleManager.DebugBattleLogText.Contains("1. Player Turn: recovered 1 AP. Choose an action."));
        AppendCheck(ref passed, ref report, "Impact text starts with ready feedback", battleManager.DebugImpactText == "Impact: Ready");
        AppendCheck(ref passed, ref report, "Enemy Break starts full", battleManager.DebugEnemyBreakText == "Break: 2/2");

        battleManager.OnClickFireSkillButton();
        AppendCheck(ref passed, ref report, "Player AP bar spends 2 AP after Fire Skill", battleManager.DebugPlayerApBarValue == 1f && battleManager.DebugPlayerApBarMaxValue == 3f);
        AppendCheck(ref passed, ref report, "Player AP text shows 33% after Fire Skill", battleManager.DebugPlayerApText == "AP: 1/3 (33%)");
        AppendCheck(ref passed, ref report, "Enemy HP text shows 50% after Fire Skill weakness damage", battleManager.DebugEnemyHpText == "Slime HP: 40/80 (50%)");
        AppendCheck(ref passed, ref report, "Enemy status shows Burn after Fire Skill", battleManager.DebugEnemyStatusText == "Status: Burn (2 turns)");
        AppendCheck(ref passed, ref report, "Impact text summarizes Fire Skill weakness and Burn", battleManager.DebugImpactText == "Impact: Fire Bolt dealt 40 damage | Weakness hit | Break 1/2 | Burn applied");
        AppendCheck(ref passed, ref report, "Weakness hit reduces Break gauge", battleManager.DebugEnemyBreakText == "Break: 1/2");
        AppendCheck(ref passed, ref report, "Damage dealt tracks Fire Skill weakness damage", battleManager.DebugTotalDamageDealt == 40);
        AppendCheck(ref passed, ref report, "Skills used counter tracks Fire Skill", battleManager.DebugSkillsUsedCount == 1);

        battleManager.DebugStartBattleForTest();
        AppendCheck(ref passed, ref report, "Restart resets Player AP bar to full", battleManager.DebugPlayerApBarValue == 3f && battleManager.DebugPlayerApBarMaxValue == 3f);
        AppendCheck(ref passed, ref report, "Restart resets resource percentage labels", battleManager.DebugPlayerHpText == "Hero HP: 100/100 (100%)" && battleManager.DebugPlayerApText == "AP: 3/3 (100%)" && battleManager.DebugEnemyHpText == "Slime HP: 80/80 (100%)");
        AppendCheck(ref passed, ref report, "Restart resets damage, guard, and skill counters", battleManager.DebugTotalDamageDealt == 0 && battleManager.DebugTotalDamageTaken == 0 && battleManager.DebugGuardUseCount == 0 && battleManager.DebugSkillsUsedCount == 0);
        AppendCheck(ref passed, ref report, "Restart keeps the readable battle log heading", battleManager.DebugBattleLogText.StartsWith("Recent Actions") && battleManager.DebugBattleLogText.Contains("1. Player Turn"));

        battleManager.OnClickGuardButton();
        AppendCheck(ref passed, ref report, "Player status shows Guarding before enemy attack", battleManager.DebugPlayerStatusText == "Status: Guarding");
        battleManager.DebugResolveEnemyAttackForTest();
        AppendCheck(ref passed, ref report, "Player status returns to Ready after guard is consumed", battleManager.DebugPlayerStatusText == "Status: Ready");
        AppendCheck(ref passed, ref report, "Guard reduces 15 enemy damage to 7", battleManager.DebugPlayerHpText == "Hero HP: 93/100 (93%)");
        AppendCheck(ref passed, ref report, "Damage taken tracks guarded hit", battleManager.DebugTotalDamageTaken == 7);
        AppendCheck(ref passed, ref report, "Guard use counter tracks chosen guard action", battleManager.DebugGuardUseCount == 1);
        AppendCheck(ref passed, ref report, "Player HP bar follows guard damage", battleManager.DebugPlayerHpBarValue == 93f && battleManager.DebugPlayerHpBarMaxValue == 100f);
        AppendCheck(ref passed, ref report, "Guard message is shown", battleManager.DebugMessageText == "Slime attacks! Hero guards and takes 7 damage.");
        AppendCheck(ref passed, ref report, "Impact text summarizes guarded enemy damage", battleManager.DebugImpactText == "Impact: Guard reduced incoming damage to 7");
        AppendCheck(ref passed, ref report, "Battle log keeps recent actions in order", battleManager.DebugBattleLogText.Contains("2. Hero guards. Next enemy attack damage is reduced.") && battleManager.DebugBattleLogText.Contains("3. Slime attacks! Hero guards and takes 7 damage."));

        battleManager.DebugStartBattleForTest();
        battleManager.DebugResolveEnemyAttackForTest();
        battleManager.DebugResolveEnemyAttackForTest();
        AppendCheck(ref passed, ref report, "Enemy intent previews strong attack before every 3rd enemy turn", battleManager.DebugEnemyIntentText == "Next Enemy: Heavy Slam (30)");
        battleManager.DebugResolveEnemyAttackForTest();
        AppendCheck(ref passed, ref report, "Slime uses a strong attack on every 3rd enemy turn", battleManager.DebugPlayerHpText == "Hero HP: 40/100 (40%)");
        AppendCheck(ref passed, ref report, "Player HP bar follows repeated enemy damage", battleManager.DebugPlayerHpBarValue == 40f);
        AppendCheck(ref passed, ref report, "Damage taken tracks normal and strong enemy hits", battleManager.DebugTotalDamageTaken == 60);
        AppendCheck(ref passed, ref report, "Strong attack message is shown", battleManager.DebugMessageText == "Slime uses Heavy Slam on turn 3! Hero takes 30 damage.");

        battleManager.DebugEndBattleForTest(BattleState.Defeat);
        AppendCheck(ref passed, ref report, "Retry button is shown after defeat", battleManager.DebugRetryButtonVisible && battleManager.DebugRetryButtonInteractable);
        AppendCheck(ref passed, ref report, "Stage Select button is shown after defeat", battleManager.DebugStageSelectButtonVisible && battleManager.DebugStageSelectButtonInteractable);
        AppendCheck(ref passed, ref report, "Defeat summary shows compact result and remaining resources", battleManager.DebugResultSummaryText.Contains("Result: Defeat | Turns: 3") && battleManager.DebugResultSummaryText.Contains("Hero: HP 40/100") && battleManager.DebugResultSummaryText.Contains("Slime: HP 80/80") && battleManager.DebugResultSummaryText.Contains("Damage: dealt 0, taken 60") && battleManager.DebugResultSummaryText.Contains("Choices: Guard 0, Skills 0") && battleManager.DebugResultSummaryText.Contains("Pace: Defeated | Survival: 40%") && battleManager.DebugResultSummaryText.Contains("Rank: C | Reward: 0G | Total Gold: 0G") && battleManager.DebugResultSummaryText.Contains("Tip: Guard before Heavy Slam.") && battleManager.DebugResultSummaryText.Contains("Last enemy pattern: Heavy Slam"));
        AppendCheck(ref passed, ref report, "Run status marks defeat as retry current encounter", battleManager.DebugRunStatusText == "Run Status: Retry Current Encounter");
        AppendCheck(ref passed, ref report, "Stage progress marks defeat as retry needed", battleManager.DebugStageProgressText == "Progress: Encounter 1/2 | Retry Needed");
        AppendCheck(ref passed, ref report, "Result summary panel is shown after defeat", battleManager.DebugResultSummaryPanelVisible);
        battleManager.OnClickRetryButton();
        AppendCheck(ref passed, ref report, "Retry restarts battle with full HP", battleManager.DebugPlayerHpText == "Hero HP: 100/100 (100%)" && battleManager.DebugMessageText.Contains("Player Turn"));
        AppendCheck(ref passed, ref report, "Retry clears result summary", string.IsNullOrEmpty(battleManager.DebugResultSummaryText));
        AppendCheck(ref passed, ref report, "Retry hides result summary panel", !battleManager.DebugResultSummaryPanelVisible);
        AppendCheck(ref passed, ref report, "Retry resets player/enemy status, intent, and combat report counters", battleManager.DebugPlayerStatusText == "Status: Ready" && battleManager.DebugEnemyStatusText == "Status: None" && battleManager.DebugEnemyIntentText == "Next Enemy: Normal Attack (15)" && battleManager.DebugGuardUseCount == 0 && battleManager.DebugSkillsUsedCount == 0);
        AppendCheck(ref passed, ref report, "Retry button hides again after restart", !battleManager.DebugRetryButtonVisible && !battleManager.DebugRetryButtonInteractable);
        AppendCheck(ref passed, ref report, "Stage Select button hides after retry restart", !battleManager.DebugStageSelectButtonVisible && !battleManager.DebugStageSelectButtonInteractable);

        battleManager.DebugEndBattleForTest(BattleState.Victory);
        AppendCheck(ref passed, ref report, "Victory summary appears after victory with carried total gold", battleManager.DebugResultSummaryText.Contains("Result: Victory | Turns: 0") && battleManager.DebugResultSummaryText.Contains("Choices: Guard 0, Skills 0") && battleManager.DebugResultSummaryText.Contains("Pace: Fast | Survival: 100%") && battleManager.DebugResultSummaryText.Contains("Rank: S | Reward: 150G | Total Gold: 150G") && battleManager.DebugResultSummaryText.Contains("Tip: Perfect clear!") && battleManager.DebugTotalGoldEarned == 150);
        AppendCheck(ref passed, ref report, "Continue button is shown after a non-final victory", battleManager.DebugContinueButtonVisible && battleManager.DebugContinueButtonInteractable);
        AppendCheck(ref passed, ref report, "Stage Select button is shown after victory", battleManager.DebugStageSelectButtonVisible && battleManager.DebugStageSelectButtonInteractable);
        AppendCheck(ref passed, ref report, "Stage objective names the next encounter before Continue", battleManager.DebugStageObjectiveText.Contains("Objective Complete: Stage 1-1: Slime Scout") && battleManager.DebugStageObjectiveText.Contains("Continue to Stage 1-2: Slime King"));
        AppendCheck(ref passed, ref report, "Victory message names the cleared and next encounters", battleManager.DebugMessageText.Contains("Stage 1-1: Slime Scout cleared") && battleManager.DebugMessageText.Contains("Press Continue to enter Stage 1-2: Slime King"));
        AppendCheck(ref passed, ref report, "Run status guides the player to continue after encounter clear", battleManager.DebugRunStatusText == "Run Status: Encounter Clear - Continue to Next");
        AppendCheck(ref passed, ref report, "Stage progress marks first encounter clear before Continue", battleManager.DebugStageProgressText == "Progress: Encounter 1/2 | Encounter Clear");
        battleManager.OnClickContinueButton();
        AppendCheck(ref passed, ref report, "Continue advances to the Stage 1 boss encounter", battleManager.DebugStageText == "Stage 1-2: Slime King" && battleManager.DebugStageObjectiveText == "Objective: Defeat Slime King" && battleManager.DebugStageProgressText == "Progress: Encounter 2/2 | Active" && battleManager.DebugEnemyHpText == "Slime King HP: 140/140 (100%)" && battleManager.DebugEnemyIntentText == "Next Enemy: Normal Attack (18)");
        AppendCheck(ref passed, ref report, "Continue hides again during the next active battle", !battleManager.DebugContinueButtonVisible && !battleManager.DebugContinueButtonInteractable);
        battleManager.DebugEndBattleForTest(BattleState.Victory);
        AppendCheck(ref passed, ref report, "Final boss victory hides Continue and explains final clear follow-up", !battleManager.DebugContinueButtonVisible && !battleManager.DebugContinueButtonInteractable && battleManager.DebugMessageText.Contains("Final Clear") && battleManager.DebugMessageText.Contains("Review Total Gold"));
        AppendCheck(ref passed, ref report, "Stage Select button stays shown after final boss victory", battleManager.DebugStageSelectButtonVisible && battleManager.DebugStageSelectButtonInteractable);
        AppendCheck(ref passed, ref report, "Run status marks final Stage 1 clear after final clear", battleManager.DebugRunStatusText == "Run Status: Final Clear - Stage 1 Complete");
        AppendCheck(ref passed, ref report, "Final boss victory carries total gold across encounters", battleManager.DebugResultSummaryText.Contains("Rank: S | Reward: 150G | Total Gold: 300G") && battleManager.DebugTotalGoldEarned == 300);
        AppendCheck(ref passed, ref report, "Stage objective marks Stage 1 clear after final boss victory", battleManager.DebugStageObjectiveText == "Objective Complete: Stage 1 cleared | Final Clear");
        AppendCheck(ref passed, ref report, "Stage progress marks final stage clear", battleManager.DebugStageProgressText == "Progress: Encounter 2/2 | Stage Clear");
        AppendCheck(ref passed, ref report, "Retry after final clear keeps the current boss encounter", battleManager.DebugRetryButtonVisible && battleManager.DebugRetryButtonInteractable);
        battleManager.OnClickRetryButton();
        AppendCheck(ref passed, ref report, "Retry restarts the current boss encounter instead of resetting the stage", battleManager.DebugStageText == "Stage 1-2: Slime King" && battleManager.DebugEnemyHpText == "Slime King HP: 140/140 (100%)");
        battleManager.DebugEndBattleForTest(BattleState.Victory);
        AppendCheck(ref passed, ref report, "Retrying an already rewarded encounter does not duplicate Total Gold", battleManager.DebugResultSummaryText.Contains("Total Gold: 300G") && battleManager.DebugTotalGoldEarned == 300);
        AppendCheck(ref passed, ref report, "EnemyData and StageData presets can describe multiple encounters", StageData.CreateStage1Boss().enemy.enemyName == "Slime King" && StageData.CreateStage1Normal().enemy.maxHp == 80);
        AppendCheck(ref passed, ref report, "BattleResultEvaluator builds rank, pace, survival, reward, tip, and last pattern", BattleResultEvaluator.BuildRank(BattleState.Victory, 2, 20) == "A" && BattleResultEvaluator.BuildPaceLabel(BattleState.Victory, 2) == "Steady" && BattleResultEvaluator.BuildSurvivalLabel(70, 100) == "70%" && BattleResultEvaluator.BuildRewardGold("A", 150, 120, 100, 0) == 120 && BattleResultEvaluator.BuildResultTip("A", "Normal Attack", "Heavy Slam") == "Take less damage for a higher rank." && BattleResultEvaluator.BuildLastEnemyPatternLabel(3, new EnemyPatternData()) == "Heavy Slam");

        BattleResultData presenterTestData = new BattleResultData
        {
            resultLabel = "Victory",
            enemyTurns = 2,
            playerName = "Hero",
            playerCurrentHp = 70,
            playerMaxHp = 100,
            playerCurrentAp = 1,
            playerMaxAp = 3,
            enemyName = "Slime",
            enemyCurrentHp = 0,
            enemyMaxHp = 80,
            damageDealt = 80,
            damageTaken = 30,
            guardUses = 1,
            skillsUsed = 3,
            paceLabel = "Steady",
            survivalLabel = "70%",
            rank = "A",
            rewardGold = 120,
            totalGold = 270,
            resultTip = "Take less damage for a higher rank.",
            lastEnemyPattern = "Normal Attack"
        };
        string presenterSummary = BattleResultPresenter.BuildSummaryText(presenterTestData);
        AppendCheck(ref passed, ref report, "BattleResultPresenter formats compact result summaries from data", presenterSummary.Contains("Result: Victory | Turns: 2") && presenterSummary.Contains("Hero: HP 70/100, AP 1/3") && presenterSummary.Contains("Choices: Guard 1, Skills 3") && presenterSummary.Contains("Pace: Steady | Survival: 70%") && presenterSummary.Contains("Rank: A | Reward: 120G | Total Gold: 270G") && presenterSummary.Contains("Last enemy pattern: Normal Attack"));

        // --- Break / Weakness Tactical Depth Tests ---

        battleManager.DebugStartBattleForTest();
        AppendCheck(ref passed, ref report, "Restart resets enemy Break gauge to full", battleManager.DebugEnemyBreakText == "Break: 2/2");
        battleManager.DebugForceEnemyBrokenForTest();
        AppendCheck(ref passed, ref report, "Forced BROKEN shows Break: BROKEN", battleManager.DebugEnemyBreakText == "Break: BROKEN");
        battleManager.OnClickAttackButton();
        AppendCheck(ref passed, ref report, "Break bonus increases Slash damage", battleManager.DebugImpactText == "Impact: Slash dealt 30 damage | Break bonus consumed");
        AppendCheck(ref passed, ref report, "Break resets after bonus attack", battleManager.DebugEnemyBreakText == "Break: 2/2");

        // --- Stage Selection Tests ---
        Object.DestroyImmediate(root);

        // Test: Stage 1 encounters selected
        GameObject stage2Root = new GameObject("Stage2TestRoot");
        BattleManager manager2 = stage2Root.AddComponent<BattleManager>();
        BattleUI ui2 = stage2Root.AddComponent<BattleUI>();
        SetPrivateField(ui2, "playerHpText", CreateText("Player HP Text"));
        SetPrivateField(ui2, "playerHpSlider", CreateSlider("Player HP Slider"));
        SetPrivateField(ui2, "playerApText", CreateText("Player AP Text"));
        SetPrivateField(ui2, "playerApSlider", CreateSlider("Player AP Slider"));
        SetPrivateField(ui2, "enemyHpText", CreateText("Enemy HP Text"));
        SetPrivateField(ui2, "enemyHpSlider", CreateSlider("Enemy HP Slider"));
        SetPrivateField(ui2, "playerStatusText", CreateText("Player Status Text"));
        SetPrivateField(ui2, "enemyStatusText", CreateText("Enemy Status Text"));
        SetPrivateField(ui2, "enemyIntentText", CreateText("Enemy Intent Text"));
        SetPrivateField(ui2, "enemyBreakText", CreateText("Enemy Break Text"));
        SetPrivateField(ui2, "enemyBreakSlider", CreateSlider("Enemy Break Slider"));
        SetPrivateField(ui2, "runStatusText", CreateText("Run Status Text"));
        SetPrivateField(ui2, "stageText", CreateText("Stage Text"));
        SetPrivateField(ui2, "stageObjectiveText", CreateText("Stage Objective Text"));
        SetPrivateField(ui2, "stageProgressText", CreateText("Stage Progress Text"));
        SetPrivateField(ui2, "messageText", CreateText("Message Text"));
        SetPrivateField(ui2, "impactText", CreateText("Impact Text"));
        SetPrivateField(ui2, "skillHelpText", CreateText("Skill Help Text"));
        SetPrivateField(ui2, "battleLogText", CreateText("Battle Log Text"));
        SetPrivateField(ui2, "resultSummaryText", CreateText("Result Summary Text"));
        SetPrivateField(ui2, "resultSummaryPanel", CreatePanel("Result Summary Panel"));
        SetPrivateField(ui2, "attackButton", CreateButton("Attack Button"));
        SetPrivateField(ui2, "fireSkillButton", CreateButton("Fire Skill Button"));
        SetPrivateField(ui2, "guardButton", CreateButton("Guard Button"));
        SetPrivateField(ui2, "endTurnButton", CreateButton("End Turn Button"));
        SetPrivateField(ui2, "retryButton", CreateButton("Retry Button"));
        SetPrivateField(ui2, "continueButton", CreateButton("Continue Button"));
        SetPrivateField(ui2, "stageSelectButton", CreateButton("Stage Select Button"));
        SetPrivateField(manager2, "battleUI", ui2);

        // Test: default (no selection) → Stage 1
        manager2.DebugStartBattleForTest();
        AppendCheck(ref passed, ref report, "Default (no selection) shows Stage 1 encounter", manager2.DebugStageText == "Stage 1-1: Slime Scout");

        // Test: Stage 1 selected explicitly
        Object.DestroyImmediate(stage2Root);
        GameObject stage1Root = new GameObject("Stage1TestRoot");
        BattleManager manager1 = stage1Root.AddComponent<BattleManager>();
        BattleUI ui1 = stage1Root.AddComponent<BattleUI>();
        SetPrivateField(ui1, "playerHpText", CreateText("Player HP Text"));
        SetPrivateField(ui1, "playerHpSlider", CreateSlider("Player HP Slider"));
        SetPrivateField(ui1, "playerApText", CreateText("Player AP Text"));
        SetPrivateField(ui1, "playerApSlider", CreateSlider("Player AP Slider"));
        SetPrivateField(ui1, "enemyHpText", CreateText("Enemy HP Text"));
        SetPrivateField(ui1, "enemyHpSlider", CreateSlider("Enemy HP Slider"));
        SetPrivateField(ui1, "playerStatusText", CreateText("Player Status Text"));
        SetPrivateField(ui1, "enemyStatusText", CreateText("Enemy Status Text"));
        SetPrivateField(ui1, "enemyIntentText", CreateText("Enemy Intent Text"));
        SetPrivateField(ui1, "enemyBreakText", CreateText("Enemy Break Text"));
        SetPrivateField(ui1, "enemyBreakSlider", CreateSlider("Enemy Break Slider"));
        SetPrivateField(ui1, "runStatusText", CreateText("Run Status Text"));
        SetPrivateField(ui1, "stageText", CreateText("Stage Text"));
        SetPrivateField(ui1, "stageObjectiveText", CreateText("Stage Objective Text"));
        SetPrivateField(ui1, "stageProgressText", CreateText("Stage Progress Text"));
        SetPrivateField(ui1, "messageText", CreateText("Message Text"));
        SetPrivateField(ui1, "impactText", CreateText("Impact Text"));
        SetPrivateField(ui1, "skillHelpText", CreateText("Skill Help Text"));
        SetPrivateField(ui1, "battleLogText", CreateText("Battle Log Text"));
        SetPrivateField(ui1, "resultSummaryText", CreateText("Result Summary Text"));
        SetPrivateField(ui1, "resultSummaryPanel", CreatePanel("Result Summary Panel"));
        SetPrivateField(ui1, "attackButton", CreateButton("Attack Button"));
        SetPrivateField(ui1, "fireSkillButton", CreateButton("Fire Skill Button"));
        SetPrivateField(ui1, "guardButton", CreateButton("Guard Button"));
        SetPrivateField(ui1, "endTurnButton", CreateButton("End Turn Button"));
        SetPrivateField(ui1, "retryButton", CreateButton("Retry Button"));
        SetPrivateField(ui1, "continueButton", CreateButton("Continue Button"));
        SetPrivateField(ui1, "stageSelectButton", CreateButton("Stage Select Button"));
        SetPrivateField(manager1, "battleUI", ui1);
        manager1.DebugLoadEncountersForStage(0);
        manager1.DebugStartBattleForTest();
        AppendCheck(ref passed, ref report, "Stage 1 selected loads Slime Scout", manager1.DebugStageText == "Stage 1-1: Slime Scout");
        manager1.DebugEndBattleForTest(BattleState.Victory);
        manager1.OnClickContinueButton();
        AppendCheck(ref passed, ref report, "Stage 1 selected advances to Slime King", manager1.DebugStageText == "Stage 1-2: Slime King");

        // Test: invalid index fallback to Stage 1
        manager1.DebugLoadEncountersForStage(-1);
        manager1.DebugStartBattleForTest();
        AppendCheck(ref passed, ref report, "Invalid stage index falls back to Stage 1", manager1.DebugStageText == "Stage 1-1: Slime Scout");

        Object.DestroyImmediate(stage1Root);

        report += passed ? "\n\nRESULT: PASS" : "\n\nRESULT: FAIL";
        Debug.Log(report);
        EditorUtility.DisplayDialog(passed ? "Battle Logic Test Passed" : "Battle Logic Test Failed", report, "OK");
    }

    private static TMP_Text CreateText(string name)
    {
        GameObject textObject = new GameObject(name);
        return textObject.AddComponent<TextMeshProUGUI>();
    }

    private static Button CreateButton(string name)
    {
        GameObject buttonObject = new GameObject(name);
        buttonObject.AddComponent<Image>();
        return buttonObject.AddComponent<Button>();
    }

    private static GameObject CreatePanel(string name)
    {
        GameObject panelObject = new GameObject(name);
        panelObject.AddComponent<Image>();
        panelObject.SetActive(false);
        return panelObject;
    }

    private static Slider CreateSlider(string name)
    {
        GameObject sliderObject = new GameObject(name);
        return sliderObject.AddComponent<Slider>();
    }

    private static void SetPrivateField(object target, string fieldName, object value)
    {
        System.Reflection.FieldInfo field = target.GetType().GetField(fieldName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        if (field != null)
        {
            field.SetValue(target, value);
        }
    }

    private static void AppendCheck(ref bool passed, ref string report, string label, bool condition)
    {
        report += condition ? "[OK] " : "[FAIL] ";
        report += label + "\n";
        if (!condition) passed = false;
    }
}