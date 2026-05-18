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

        ProgressState.Reset();
        for (int i = 0; i < 5; i++) ProgressState.MarkStageCompleted(i);

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
        SetPrivateField(battleUI, "playerShieldText", CreateText("Player Shield Text"));
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
        SetPrivateField(battleUI, "iceSkillButton", CreateButton("Ice Lance Button"));
        SetPrivateField(battleUI, "lightningSkillButton", CreateButton("Lightning Strike Button"));
        SetPrivateField(battleUI, "earthSkillButton", CreateButton("Earth Wall Button"));
        SetPrivateField(battleUI, "guardButton", CreateButton("Guard Button"));
        SetPrivateField(battleUI, "endTurnButton", CreateButton("End Turn Button"));
        SetPrivateField(battleUI, "retryButton", CreateButton("Retry Button"));
        SetPrivateField(battleUI, "continueButton", CreateButton("Continue Button"));
        SetPrivateField(battleUI, "stageSelectButton", CreateButton("Stage Select Button"));
        SetPrivateField(battleUI, "speedToggleButton", CreateButton("Speed Toggle Button"));
        SetPrivateField(battleUI, "autoBattleButton", CreateButton("Auto Battle Button"));
        SetPrivateField(battleUI, "itemButton", CreateButton("Item Button"));
        SetPrivateField(battleUI, "pauseButton", CreateButton("Pause Button"));

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
        AppendCheck(ref passed, ref report, "Enemy intent starts with normal attack preview", battleManager.DebugEnemyIntentText == "Next Enemy: [Fire] Normal Attack (15)");
        AppendCheck(ref passed, ref report, "Skill help explains all skills, Guard, and readable enemy pattern values", battleManager.DebugSkillHelpText.Contains("Slash: 20 power, 0 AP") && battleManager.DebugSkillHelpText.Contains("Fire Bolt: 30 power, 2 AP") && battleManager.DebugSkillHelpText.Contains("Ice Lance: 25 power, 1 AP") && battleManager.DebugSkillHelpText.Contains("Lightning Strike: 40 power, 3 AP") && battleManager.DebugSkillHelpText.Contains("Guard: reduce next enemy attack") && battleManager.DebugSkillHelpText.Contains("Normal attack: 15 damage") && battleManager.DebugSkillHelpText.Contains("Heavy Slam: 30 damage every 3rd enemy turn"));
        AppendCheck(ref passed, ref report, "Battle log has a readable Recent Actions heading", battleManager.DebugBattleLogText.StartsWith("Recent Actions"));
        AppendCheck(ref passed, ref report, "Stage 1 starts with Tutorial Field modifier message", battleManager.DebugBattleLogText.Contains("Tutorial Field"));
        AppendCheck(ref passed, ref report, "Battle log records the latest player turn prompt", battleManager.DebugBattleLogText.Contains("Player Turn: recovered 1 AP. Choose an action."));
        AppendCheck(ref passed, ref report, "Impact text starts with ready feedback", battleManager.DebugImpactText == "Impact: Ready");
        AppendCheck(ref passed, ref report, "Enemy Break starts full", battleManager.DebugEnemyBreakText == "Break: 2/2");

        battleManager.OnClickFireSkillButton();
        AppendCheck(ref passed, ref report, "Player AP bar spends 2 AP after Fire Skill", battleManager.DebugPlayerApBarValue == 1f && battleManager.DebugPlayerApBarMaxValue == 3f);
        AppendCheck(ref passed, ref report, "Player AP text shows 33% after Fire Skill", battleManager.DebugPlayerApText == "AP: 1/3 (33%)");
        AppendCheck(ref passed, ref report, "Enemy HP text shows 44% after Fire Skill weakness damage", battleManager.DebugEnemyHpText == "Slime HP: 35/80 (44%)");
        AppendCheck(ref passed, ref report, "Enemy status shows Burn after Fire Skill", battleManager.DebugEnemyStatusText == "Status: Burn (2 turns)");
        AppendCheck(ref passed, ref report, "Impact text summarizes Fire Skill weakness and Burn", battleManager.DebugImpactText == "Impact: Fire Bolt dealt 45 damage | Weakness x1.5 | Break 1/2 | Burn applied");
        AppendCheck(ref passed, ref report, "Weakness hit reduces Break gauge", battleManager.DebugEnemyBreakText == "Break: 1/2");
        AppendCheck(ref passed, ref report, "Damage dealt tracks Fire Skill weakness damage", battleManager.DebugTotalDamageDealt == 45);
        AppendCheck(ref passed, ref report, "Skills used counter tracks Fire Skill", battleManager.DebugSkillsUsedCount == 1);

        battleManager.DebugStartBattleForTest();
        AppendCheck(ref passed, ref report, "Restart resets Player AP bar to full", battleManager.DebugPlayerApBarValue == 3f && battleManager.DebugPlayerApBarMaxValue == 3f);
        AppendCheck(ref passed, ref report, "Restart resets resource percentage labels", battleManager.DebugPlayerHpText == "Hero HP: 100/100 (100%)" && battleManager.DebugPlayerApText == "AP: 3/3 (100%)" && battleManager.DebugEnemyHpText == "Slime HP: 80/80 (100%)");
        AppendCheck(ref passed, ref report, "Restart resets damage, guard, and skill counters", battleManager.DebugTotalDamageDealt == 0 && battleManager.DebugTotalDamageTaken == 0 && battleManager.DebugGuardUseCount == 0 && battleManager.DebugSkillsUsedCount == 0);
        AppendCheck(ref passed, ref report, "Restart keeps the readable battle log heading", battleManager.DebugBattleLogText.StartsWith("Recent Actions") && battleManager.DebugBattleLogText.Contains("Player Turn"));

        // Ice Lance: costs 1 AP, Ice element, applies Stun
        battleManager.OnClickIceSkillButton();
        AppendCheck(ref passed, ref report, "Player AP spends 1 AP after Ice Lance", battleManager.DebugPlayerApBarValue == 2f && battleManager.DebugPlayerApBarMaxValue == 3f);
        AppendCheck(ref passed, ref report, "Player AP text shows 67% after Ice Lance", battleManager.DebugPlayerApText == "AP: 2/3 (67%)");
        AppendCheck(ref passed, ref report, "Enemy HP shows Ice Lance neutral damage", battleManager.DebugEnemyHpText == "Slime HP: 55/80 (69%)");
        AppendCheck(ref passed, ref report, "Enemy status shows Stun after Ice Lance", battleManager.DebugEnemyStatusText == "Status: Stun (1 turns)");
        AppendCheck(ref passed, ref report, "Impact text summarizes Ice Lance Stun", battleManager.DebugImpactText.Contains("Impact: Ice Lance dealt 25 damage") && battleManager.DebugImpactText.Contains("Stun applied"));
        AppendCheck(ref passed, ref report, "Damage dealt tracks Ice Lance damage", battleManager.DebugTotalDamageDealt == 25);
        AppendCheck(ref passed, ref report, "Skills used counter tracks Ice Lance", battleManager.DebugSkillsUsedCount == 1);

        // Lightning Strike: costs 3 AP, Lightning element, high damage, no status
        battleManager.DebugStartBattleForTest();
        battleManager.OnClickLightningSkillButton();
        AppendCheck(ref passed, ref report, "Player AP spends 3 AP after Lightning Strike", battleManager.DebugPlayerApBarValue == 0f && battleManager.DebugPlayerApBarMaxValue == 3f);
        AppendCheck(ref passed, ref report, "Player AP text shows 0% after Lightning Strike", battleManager.DebugPlayerApText == "AP: 0/3 (0%)");
        AppendCheck(ref passed, ref report, "Enemy HP shows Lightning Strike neutral damage", battleManager.DebugEnemyHpText == "Slime HP: 40/80 (50%)");
        AppendCheck(ref passed, ref report, "Damage dealt tracks Lightning Strike damage", battleManager.DebugTotalDamageDealt == 40);
        AppendCheck(ref passed, ref report, "Skills used counter tracks Lightning Strike", battleManager.DebugSkillsUsedCount == 1);

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
        AppendCheck(ref passed, ref report, "Battle log keeps recent actions in order", battleManager.DebugBattleLogText.Contains("4. Hero guards. Next enemy attack damage is reduced.") && battleManager.DebugBattleLogText.Contains("5. Slime attacks! Hero guards and takes 7 damage."));

        battleManager.DebugStartBattleForTest();
        battleManager.DebugResolveEnemyAttackForTest();
        battleManager.DebugResolveEnemyAttackForTest();
        AppendCheck(ref passed, ref report, "Enemy intent previews strong attack before every 3rd enemy turn", battleManager.DebugEnemyIntentText == "Next Enemy: [Fire] Heavy Slam (30)");
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
        AppendCheck(ref passed, ref report, "Retry resets player/enemy status, intent, and combat report counters", battleManager.DebugPlayerStatusText == "Status: Ready" && battleManager.DebugEnemyStatusText == "Status: None" && battleManager.DebugEnemyIntentText == "Next Enemy: [Fire] Normal Attack (15)" && battleManager.DebugGuardUseCount == 0 && battleManager.DebugSkillsUsedCount == 0);
        AppendCheck(ref passed, ref report, "Retry button hides again after restart", !battleManager.DebugRetryButtonVisible && !battleManager.DebugRetryButtonInteractable);
        AppendCheck(ref passed, ref report, "Stage Select button hides after retry restart", !battleManager.DebugStageSelectButtonVisible && !battleManager.DebugStageSelectButtonInteractable);

        battleManager.DebugEndBattleForTest(BattleState.Victory);
        AppendCheck(ref passed, ref report, "Victory summary appears after victory with carried total gold", battleManager.DebugResultSummaryText.Contains("Result: Victory | Turns: 0") && battleManager.DebugResultSummaryText.Contains("Choices: Guard 0, Skills 0") && battleManager.DebugResultSummaryText.Contains("Pace: Fast | Survival: 100%") && battleManager.DebugResultSummaryText.Contains("Rank: S | Reward: 150G | Total Gold: 240G") && battleManager.DebugResultSummaryText.Contains("Tip: Perfect clear!") && battleManager.DebugTotalGoldEarned == 240);
        AppendCheck(ref passed, ref report, "Continue button is shown after a non-final victory", battleManager.DebugContinueButtonVisible && battleManager.DebugContinueButtonInteractable);
        AppendCheck(ref passed, ref report, "Stage Select button is shown after victory", battleManager.DebugStageSelectButtonVisible && battleManager.DebugStageSelectButtonInteractable);
        AppendCheck(ref passed, ref report, "Stage objective names the next encounter before Continue", battleManager.DebugStageObjectiveText.Contains("Objective Complete: Stage 1-1: Slime Scout") && battleManager.DebugStageObjectiveText.Contains("Continue to Stage 1-2: Slime King"));
        AppendCheck(ref passed, ref report, "Victory message names the cleared and next encounters", battleManager.DebugMessageText.Contains("Stage 1-1: Slime Scout cleared") && battleManager.DebugMessageText.Contains("Press Continue to enter Stage 1-2: Slime King"));
        AppendCheck(ref passed, ref report, "Run status guides the player to continue after encounter clear", battleManager.DebugRunStatusText == "Run Status: Encounter Clear - Continue to Next");
        AppendCheck(ref passed, ref report, "Stage progress marks first encounter clear before Continue", battleManager.DebugStageProgressText == "Progress: Encounter 1/2 | Encounter Clear");
        battleManager.OnClickContinueButton();
        AppendCheck(ref passed, ref report, "Continue advances to the Stage 1 boss encounter", battleManager.DebugStageText == "Stage 1-2: Slime King" && battleManager.DebugStageObjectiveText == "Objective: Defeat Slime King" && battleManager.DebugStageProgressText == "Progress: Encounter 2/2 | Active" && battleManager.DebugEnemyHpText == "Slime King HP: 140/140 (100%)" && battleManager.DebugEnemyIntentText == "Next Enemy: [Fire] Normal Attack (18)");
        AppendCheck(ref passed, ref report, "Continue hides again during the next active battle", !battleManager.DebugContinueButtonVisible && !battleManager.DebugContinueButtonInteractable);
        battleManager.DebugEndBattleForTest(BattleState.Victory);
        AppendCheck(ref passed, ref report, "Final boss victory hides Continue and explains final clear follow-up", !battleManager.DebugContinueButtonVisible && !battleManager.DebugContinueButtonInteractable && battleManager.DebugMessageText.Contains("Final Clear") && battleManager.DebugMessageText.Contains("Review Total Gold"));
        AppendCheck(ref passed, ref report, "Stage Select button stays shown after final boss victory", battleManager.DebugStageSelectButtonVisible && battleManager.DebugStageSelectButtonInteractable);
        AppendCheck(ref passed, ref report, "Run status marks final Stage 1 clear after final clear", battleManager.DebugRunStatusText == "Run Status: Final Clear - Stage 1 Complete");
        AppendCheck(ref passed, ref report, "Final boss victory carries total gold across encounters", battleManager.DebugResultSummaryText.Contains("Rank: S | Reward: 150G | Total Gold: 480G") && battleManager.DebugTotalGoldEarned == 480);
        AppendCheck(ref passed, ref report, "Stage objective marks Stage 1 clear after final boss victory", battleManager.DebugStageObjectiveText == "Objective Complete: Stage 1 cleared | Final Clear");
        AppendCheck(ref passed, ref report, "Stage progress marks final stage clear", battleManager.DebugStageProgressText == "Progress: Encounter 2/2 | Stage Clear");
        AppendCheck(ref passed, ref report, "Retry after final clear keeps the current boss encounter", battleManager.DebugRetryButtonVisible && battleManager.DebugRetryButtonInteractable);
        battleManager.OnClickRetryButton();
        AppendCheck(ref passed, ref report, "Retry restarts the current boss encounter instead of resetting the stage", battleManager.DebugStageText == "Stage 1-2: Slime King" && battleManager.DebugEnemyHpText == "Slime King HP: 140/140 (100%)");
        battleManager.DebugEndBattleForTest(BattleState.Victory);
        AppendCheck(ref passed, ref report, "Retrying an already rewarded encounter does not duplicate Total Gold", battleManager.DebugResultSummaryText.Contains("Total Gold: 480G") && battleManager.DebugTotalGoldEarned == 480);

        // --- End-to-end: BattleManager + ProgressState integration ---
        // Verify that final Victory calls ProgressState.MarkStageCompleted
        // when StageSelectController.SelectedStageIndex is set.
        // This simulates the real game flow: StageSelect → Battle → Victory → unlock.
        ProgressState.Reset();
        var selectedStageField = typeof(StageSelectController).GetField(
            "<SelectedStageIndex>k__BackingField",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        if (selectedStageField != null)
        {
            // Set SelectedStageIndex = 0 (Stage 1) via reflection
            selectedStageField.SetValue(null, 0);
            // Run Stage 1 boss-only clear (single encounter)
            var e2eRoot = new GameObject("E2ETestRoot");
            var e2eManager = e2eRoot.AddComponent<BattleManager>();
            var e2eUI = e2eRoot.AddComponent<BattleUI>();
            SetPrivateField(e2eUI, "playerHpText", CreateText("Player HP Text"));
            SetPrivateField(e2eUI, "playerHpSlider", CreateSlider("Player HP Slider"));
            SetPrivateField(e2eUI, "playerApText", CreateText("Player AP Text"));
            SetPrivateField(e2eUI, "playerApSlider", CreateSlider("Player AP Slider"));
            SetPrivateField(e2eUI, "enemyHpText", CreateText("Enemy HP Text"));
            SetPrivateField(e2eUI, "enemyHpSlider", CreateSlider("Enemy HP Slider"));
            SetPrivateField(e2eUI, "playerStatusText", CreateText("Player Status Text"));
            SetPrivateField(e2eUI, "enemyStatusText", CreateText("Enemy Status Text"));
            SetPrivateField(e2eUI, "enemyIntentText", CreateText("Enemy Intent Text"));
            SetPrivateField(e2eUI, "enemyBreakText", CreateText("Enemy Break Text"));
            SetPrivateField(e2eUI, "enemyBreakSlider", CreateSlider("Enemy Break Slider"));
            SetPrivateField(e2eUI, "runStatusText", CreateText("Run Status Text"));
            SetPrivateField(e2eUI, "stageText", CreateText("Stage Text"));
            SetPrivateField(e2eUI, "stageObjectiveText", CreateText("Stage Objective Text"));
            SetPrivateField(e2eUI, "stageProgressText", CreateText("Stage Progress Text"));
            SetPrivateField(e2eUI, "messageText", CreateText("Message Text"));
            SetPrivateField(e2eUI, "impactText", CreateText("Impact Text"));
            SetPrivateField(e2eUI, "skillHelpText", CreateText("Skill Help Text"));
            SetPrivateField(e2eUI, "battleLogText", CreateText("Battle Log Text"));
            SetPrivateField(e2eUI, "resultSummaryText", CreateText("Result Summary Text"));
            SetPrivateField(e2eUI, "resultSummaryPanel", CreatePanel("Result Summary Panel"));
            SetPrivateField(e2eUI, "attackButton", CreateButton("Attack Button"));
            SetPrivateField(e2eUI, "fireSkillButton", CreateButton("Fire Skill Button"));
            SetPrivateField(e2eUI, "iceSkillButton", CreateButton("Ice Lance Button"));
        SetPrivateField(e2eUI, "lightningSkillButton", CreateButton("Lightning Strike Button"));
            SetPrivateField(e2eUI, "guardButton", CreateButton("Guard Button"));
            SetPrivateField(e2eUI, "endTurnButton", CreateButton("End Turn Button"));
            SetPrivateField(e2eUI, "retryButton", CreateButton("Retry Button"));
            SetPrivateField(e2eUI, "continueButton", CreateButton("Continue Button"));
            SetPrivateField(e2eUI, "stageSelectButton", CreateButton("Stage Select Button"));
            SetPrivateField(e2eUI, "speedToggleButton", CreateButton("Speed Toggle Button"));
            SetPrivateField(e2eUI, "autoBattleButton", CreateButton("Auto Battle Button"));
            SetPrivateField(e2eManager, "battleUI", e2eUI);
            e2eManager.DebugStartBattleForTest();
            AppendCheck(ref passed, ref report, "E2E: Selected Stage 1 loads Stage 1-1", e2eManager.DebugStageText == "Stage 1-1: Slime Scout");
            // Clear all encounters (Normal + Boss = 2 encounters, but DebugEndBattleForTest skips HasNextStage check)
            e2eManager.DebugEndBattleForTest(BattleState.Victory);
            e2eManager.OnClickContinueButton();
            e2eManager.DebugEndBattleForTest(BattleState.Victory);
            AppendCheck(ref passed, ref report, "E2E: Stage 1 final clear marks progress unlocked", ProgressState.IsStageUnlocked(1));
            AppendCheck(ref passed, ref report, "E2E: Stage 1 marked as completed", ProgressState.DebugIsStage0Completed);
            Object.DestroyImmediate(e2eRoot);
        }
        else
        {
            AppendCheck(ref passed, ref report, "E2E: StageSelectController.SelectedStageIndex field accessible", false);
        }

        AppendCheck(ref passed, ref report, "EnemyData and StageData presets can describe multiple encounters", StageData.CreateStage1Boss().enemy.enemyName == "Slime King" && StageData.CreateStage1Normal().enemy.maxHp == 80);
        AppendCheck(ref passed, ref report, "Stage 2 Normal metadata has PackPressure modifier", StageData.CreateStage2Normal().stageModifier == StageModifierType.PackPressure);
        AppendCheck(ref passed, ref report, "Stage 2 Boss metadata has PackPressure modifier", StageData.CreateStage2Boss().stageModifier == StageModifierType.PackPressure);
        AppendCheck(ref passed, ref report, "Stage 3 Normal metadata has Stoneguard modifier", StageData.CreateStage3Normal().stageModifier == StageModifierType.Stoneguard);
        AppendCheck(ref passed, ref report, "Stage 3 Boss metadata has Stoneguard modifier", StageData.CreateStage3Boss().stageModifier == StageModifierType.Stoneguard);
        AppendCheck(ref passed, ref report, "Stage 4 Normal metadata has StormSurge modifier", StageData.CreateStage4Normal().stageModifier == StageModifierType.StormSurge);
        AppendCheck(ref passed, ref report, "Stage 4 Boss metadata has StormSurge modifier", StageData.CreateStage4Boss().stageModifier == StageModifierType.StormSurge);
        AppendCheck(ref passed, ref report, "Stage 5 Normal metadata has VoidDrain modifier", StageData.CreateStage5Normal().stageModifier == StageModifierType.VoidDrain);
        AppendCheck(ref passed, ref report, "Stage 5 Boss metadata has VoidDrain modifier", StageData.CreateStage5Boss().stageModifier == StageModifierType.VoidDrain);
        AppendCheck(ref passed, ref report, "Stage 6 Normal metadata has RadiantTrial modifier", StageData.CreateStage6Normal().stageModifier == StageModifierType.RadiantTrial);
        AppendCheck(ref passed, ref report, "Stage 6 Boss metadata has RadiantTrial modifier", StageData.CreateStage6Boss().stageModifier == StageModifierType.RadiantTrial);
        AppendCheck(ref passed, ref report, "StageData centralizes readable modifier names", StageData.GetModifierDisplayName(StageModifierType.StormSurge) == "Storm Surge" && StageData.GetModifierDisplayName(StageModifierType.VoidDrain) == "Void Drain");
        AppendCheck(ref passed, ref report, "StageData builds modifier summary text from source data", StageData.CreateStage5Normal().BuildModifierSummaryText().Contains("Modifier: Void Drain") && StageData.CreateStage5Normal().BuildModifierSummaryText().Contains("Effect: Shadow energy drains AP over time."));
        AppendCheck(ref passed, ref report, "Stage 3 Normal creates Golem Sentry", StageData.CreateStage3Normal().enemy.enemyName == "Golem Sentry" && StageData.CreateStage3Normal().enemy.maxHp == 120);
        AppendCheck(ref passed, ref report, "Stage 3 Boss creates Ancient Golem", StageData.CreateStage3Boss().enemy.enemyName == "Ancient Golem" && StageData.CreateStage3Boss().enemy.maxHp == 220);
        AppendCheck(ref passed, ref report, "Stage 3 encounters loaded from GetEncountersForStage", StageData.GetEncountersForStage(2).Count == 2 && StageData.GetEncountersForStage(2)[0].encounterName == "Golem Sentry");
        AppendCheck(ref passed, ref report, "Stage 4 Normal creates Storm Hawk", StageData.CreateStage4Normal().enemy.enemyName == "Storm Hawk" && StageData.CreateStage4Normal().enemy.maxHp == 140);
        AppendCheck(ref passed, ref report, "Stage 4 Boss creates Thunder Phoenix", StageData.CreateStage4Boss().enemy.enemyName == "Thunder Phoenix" && StageData.CreateStage4Boss().enemy.maxHp == 250);
        AppendCheck(ref passed, ref report, "Stage 4 encounters loaded from GetEncountersForStage", StageData.GetEncountersForStage(3).Count == 2 && StageData.GetEncountersForStage(3)[0].encounterName == "Storm Hawk");
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

        // --- Stage Modifier Tests ---
        // Stage 2 PackPressure: verify modifier message in battle log
        battleManager.DebugLoadEncountersForStage(1);
        battleManager.DebugStartBattleForTest();
        AppendCheck(ref passed, ref report, "Stage 2 battle log shows Pack Pressure modifier",
            battleManager.DebugBattleLogText.Contains("Pack Pressure"));
        AppendCheck(ref passed, ref report, "Stage 2 PackPressure reduces strong attack period to 2 turns",
            battleManager.DebugSkillHelpText.Contains("every 2"));
        AppendCheck(ref passed, ref report, "Stage 2 skill help shows Pack Pressure modifier guidance",
            battleManager.DebugSkillHelpText.Contains("Stage Modifier: Pack Pressure") && battleManager.DebugSkillHelpText.Contains("Enemy strong attacks come more frequently"));
        // Stage 3 Stoneguard: verify modifier message and break gauge increase
        battleManager.DebugLoadEncountersForStage(2);
        battleManager.DebugStartBattleForTest();
        AppendCheck(ref passed, ref report, "Stage 3 battle log shows Stoneguard modifier",
            battleManager.DebugBattleLogText.Contains("Stoneguard"));
        AppendCheck(ref passed, ref report, "Stage 3 Stoneguard increases break gauge to 3",
            battleManager.DebugEnemyBreakText == "Break: 3/3");

        // Stage 4 StormSurge: periodic hazard damage every 3 enemy turns
        battleManager.DebugLoadEncountersForStage(3);
        battleManager.DebugStartBattleForTest();
        AppendCheck(ref passed, ref report, "Stage 4 battle log shows Storm Surge modifier",
            battleManager.DebugBattleLogText.Contains("Storm Surge"));
        // First two attacks — no surge yet (enemyTurnCount 1 & 2)
        battleManager.DebugResolveEnemyAttackForTest();
        battleManager.DebugResolveEnemyAttackForTest();
        int damageBeforeSurge = battleManager.DebugTotalDamageTaken;
        // Third attack — Storm Surge fires on turn 3
        battleManager.DebugResolveEnemyAttackForTest();
        AppendCheck(ref passed, ref report, "Stage 4 Storm Surge message appears on 3rd enemy turn",
            battleManager.DebugMessageText.Contains("Storm Surge") || battleManager.DebugBattleLogText.Contains("Storm Surge"));
        AppendCheck(ref passed, ref report, "Stage 4 Storm Surge impact shows hazard damage",
            battleManager.DebugImpactText.Contains("hazard") || battleManager.DebugImpactText.Contains("Storm Surge"));
        AppendCheck(ref passed, ref report, "Stage 4 Storm Surge increases total damage taken by at least 8",
            battleManager.DebugTotalDamageTaken >= damageBeforeSurge + 8);

        // Stage 5 VoidDrain: drain AP every 2 enemy turns
        battleManager.DebugLoadEncountersForStage(4);
        battleManager.DebugStartBattleForTest();
        AppendCheck(ref passed, ref report, "Stage 5 battle log shows Void Drain modifier",
            battleManager.DebugBattleLogText.Contains("Void Drain"));
        // Turn 1 — no drain (enemyTurnCount 1, not divisible by 2)
        battleManager.DebugResolveEnemyAttackForTest();
        AppendCheck(ref passed, ref report, "Stage 5 first attack does not trigger drain message",
            !battleManager.DebugMessageText.Contains("Void Drain"));
        // Turn 2 — Void Drain fires, reduces AP by 1 (from 3 to 2)
        battleManager.DebugResolveEnemyAttackForTest();
        AppendCheck(ref passed, ref report, "Stage 5 Void Drain saps AP on 2nd enemy turn",
            battleManager.DebugMessageText.Contains("Void Drain") || battleManager.DebugBattleLogText.Contains("Void Drain"));
        AppendCheck(ref passed, ref report, "Stage 5 Void Drain impact shows reduced AP",
            battleManager.DebugImpactText.Contains("reduced AP"));
        AppendCheck(ref passed, ref report, "Stage 5 Void Drain reduces player AP after 2 turns",
            battleManager.DebugPlayerApText.Contains("2/3") || battleManager.DebugPlayerApText.Contains("1/3"));
        battleManager.DebugStartBattleForTest();
        battleManager.DebugSetPlayerApForTest(0);
        battleManager.DebugResolveEnemyAttackForTest();
        int damageBeforeVoidDrainHp = battleManager.DebugTotalDamageTaken;
        battleManager.DebugResolveEnemyAttackForTest();
        AppendCheck(ref passed, ref report, "Stage 5 Void Drain lashes HP when AP is empty",
            battleManager.DebugMessageText.Contains("Void Drain lashes out") && battleManager.DebugPlayerApText.Contains("0/3"));
        AppendCheck(ref passed, ref report, "Stage 5 Void Drain AP-empty branch deals 5 hazard damage",
            battleManager.DebugImpactText.Contains("5 hazard damage") && battleManager.DebugTotalDamageTaken >= damageBeforeVoidDrainHp + 5);

        // Stage 6 RadiantTrial: strong attack period reduced + break gauge increased
        battleManager.DebugLoadEncountersForStage(5);
        battleManager.DebugStartBattleForTest();
        AppendCheck(ref passed, ref report, "Stage 6 battle log shows Radiant Trial modifier",
            battleManager.DebugBattleLogText.Contains("Radiant Trial"));
        AppendCheck(ref passed, ref report, "Stage 6 RadiantTrial reduces strong attack period in skill help",
            battleManager.DebugSkillHelpText.Contains("every 2"));
        AppendCheck(ref passed, ref report, "Stage 6 RadiantTrial increases break gauge to 3",
            battleManager.DebugEnemyBreakText == "Break: 3/3");

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
        SetPrivateField(ui2, "iceSkillButton", CreateButton("Ice Lance Button"));
        SetPrivateField(ui2, "lightningSkillButton", CreateButton("Lightning Strike Button"));
        SetPrivateField(ui2, "guardButton", CreateButton("Guard Button"));
        SetPrivateField(ui2, "endTurnButton", CreateButton("End Turn Button"));
        SetPrivateField(ui2, "retryButton", CreateButton("Retry Button"));
        SetPrivateField(ui2, "continueButton", CreateButton("Continue Button"));
        SetPrivateField(ui2, "stageSelectButton", CreateButton("Stage Select Button"));
            SetPrivateField(ui2, "speedToggleButton", CreateButton("Speed Toggle Button"));
            SetPrivateField(ui2, "autoBattleButton", CreateButton("Auto Battle Button"));
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
        SetPrivateField(ui1, "iceSkillButton", CreateButton("Ice Lance Button"));
        SetPrivateField(ui1, "lightningSkillButton", CreateButton("Lightning Strike Button"));
        SetPrivateField(ui1, "guardButton", CreateButton("Guard Button"));
        SetPrivateField(ui1, "endTurnButton", CreateButton("End Turn Button"));
        SetPrivateField(ui1, "retryButton", CreateButton("Retry Button"));
        SetPrivateField(ui1, "continueButton", CreateButton("Continue Button"));
        SetPrivateField(ui1, "stageSelectButton", CreateButton("Stage Select Button"));
            SetPrivateField(ui1, "speedToggleButton", CreateButton("Speed Toggle Button"));
            SetPrivateField(ui1, "autoBattleButton", CreateButton("Auto Battle Button"));
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

        // --- Progress State Tests ---
        // Reset any state from previous test runs
        ProgressState.Reset();
        AppendCheck(ref passed, ref report, "ProgressState starts fresh with stage 0 unlocked", ProgressState.IsStageUnlocked(0) && !ProgressState.IsStageUnlocked(1));
        AppendCheck(ref passed, ref report, "ProgressState reset shows no completed stages", ProgressState.DebugCompletedStageCount == 0);
        AppendCheck(ref passed, ref report, "ProgressState default TotalStages is 6", ProgressState.TotalStages == 6);
        AppendCheck(ref passed, ref report, "Stage 2 is locked initially", !ProgressState.IsStageUnlocked(2));

        // Mark stage 1 (index 0) as completed → stage 2 (index 1) unlocks
        ProgressState.MarkStageCompleted(0);
        AppendCheck(ref passed, ref report, "Marking stage 0 completed unlocks stage 1", ProgressState.IsStageUnlocked(0) && ProgressState.IsStageUnlocked(1));
        AppendCheck(ref passed, ref report, "Completed stage count increments", ProgressState.DebugCompletedStageCount == 1);
        AppendCheck(ref passed, ref report, "Stage 0 is tracked as completed", ProgressState.DebugIsStage0Completed);

        // Reset again
        ProgressState.Reset();
        AppendCheck(ref passed, ref report, "ProgressState.Reset restores locked state", ProgressState.IsStageUnlocked(0) && !ProgressState.IsStageUnlocked(1));

        // Edge cases
        ProgressState.MarkStageCompleted(-1);
        AppendCheck(ref passed, ref report, "MarkStageCompleted with negative index is a no-op", ProgressState.DebugCompletedStageCount == 0);

        // Test that marking stage 1 completed doesn't unlock stage 2 unless stage 0 is also completed
        ProgressState.MarkStageCompleted(1);
        AppendCheck(ref passed, ref report, "Marking stage 1 without stage 0 keeps stage 2 locked", !ProgressState.IsStageUnlocked(1));

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