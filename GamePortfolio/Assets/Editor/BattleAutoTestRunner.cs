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

        SetPrivateField(battleManager, "playerHpText", CreateText("Player HP Text"));
        SetPrivateField(battleManager, "playerHpSlider", CreateSlider("Player HP Slider"));
        SetPrivateField(battleManager, "playerApText", CreateText("Player AP Text"));
        SetPrivateField(battleManager, "playerApSlider", CreateSlider("Player AP Slider"));
        SetPrivateField(battleManager, "enemyHpText", CreateText("Enemy HP Text"));
        SetPrivateField(battleManager, "enemyHpSlider", CreateSlider("Enemy HP Slider"));
        SetPrivateField(battleManager, "playerStatusText", CreateText("Player Status Text"));
        SetPrivateField(battleManager, "enemyStatusText", CreateText("Enemy Status Text"));
        SetPrivateField(battleManager, "enemyIntentText", CreateText("Enemy Intent Text"));
        SetPrivateField(battleManager, "messageText", CreateText("Message Text"));
        SetPrivateField(battleManager, "skillHelpText", CreateText("Skill Help Text"));
        SetPrivateField(battleManager, "battleLogText", CreateText("Battle Log Text"));
        SetPrivateField(battleManager, "resultSummaryText", CreateText("Result Summary Text"));
        SetPrivateField(battleManager, "resultSummaryPanel", CreatePanel("Result Summary Panel"));
        SetPrivateField(battleManager, "attackButton", CreateButton("Attack Button"));
        SetPrivateField(battleManager, "fireSkillButton", CreateButton("Fire Skill Button"));
        SetPrivateField(battleManager, "guardButton", CreateButton("Guard Button"));
        SetPrivateField(battleManager, "endTurnButton", CreateButton("End Turn Button"));
        SetPrivateField(battleManager, "retryButton", CreateButton("Retry Button"));

        battleManager.DebugStartBattleForTest();
        AppendCheck(ref passed, ref report, "Battle starts with full player HP", battleManager.DebugPlayerHpText == "Hero HP: 100/100");
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
        AppendCheck(ref passed, ref report, "Battle log records the latest player turn prompt", battleManager.DebugBattleLogText.Contains("1. Player Turn: recovered 1 AP. Choose an action."));

        battleManager.OnClickFireSkillButton();
        AppendCheck(ref passed, ref report, "Player AP bar spends 2 AP after Fire Skill", battleManager.DebugPlayerApBarValue == 1f && battleManager.DebugPlayerApBarMaxValue == 3f);
        AppendCheck(ref passed, ref report, "Enemy status shows Burn after Fire Skill", battleManager.DebugEnemyStatusText == "Status: Burn (2 turns)");
        AppendCheck(ref passed, ref report, "Damage dealt tracks Fire Skill weakness damage", battleManager.DebugTotalDamageDealt == 40);
        AppendCheck(ref passed, ref report, "Skills used counter tracks Fire Skill", battleManager.DebugSkillsUsedCount == 1);

        battleManager.DebugStartBattleForTest();
        AppendCheck(ref passed, ref report, "Restart resets Player AP bar to full", battleManager.DebugPlayerApBarValue == 3f && battleManager.DebugPlayerApBarMaxValue == 3f);
        AppendCheck(ref passed, ref report, "Restart resets damage, guard, and skill counters", battleManager.DebugTotalDamageDealt == 0 && battleManager.DebugTotalDamageTaken == 0 && battleManager.DebugGuardUseCount == 0 && battleManager.DebugSkillsUsedCount == 0);

        battleManager.OnClickGuardButton();
        AppendCheck(ref passed, ref report, "Player status shows Guarding before enemy attack", battleManager.DebugPlayerStatusText == "Status: Guarding");
        battleManager.DebugResolveEnemyAttackForTest();
        AppendCheck(ref passed, ref report, "Player status returns to Ready after guard is consumed", battleManager.DebugPlayerStatusText == "Status: Ready");
        AppendCheck(ref passed, ref report, "Guard reduces 15 enemy damage to 7", battleManager.DebugPlayerHpText == "Hero HP: 93/100");
        AppendCheck(ref passed, ref report, "Damage taken tracks guarded hit", battleManager.DebugTotalDamageTaken == 7);
        AppendCheck(ref passed, ref report, "Guard use counter tracks chosen guard action", battleManager.DebugGuardUseCount == 1);
        AppendCheck(ref passed, ref report, "Player HP bar follows guard damage", battleManager.DebugPlayerHpBarValue == 93f && battleManager.DebugPlayerHpBarMaxValue == 100f);
        AppendCheck(ref passed, ref report, "Guard message is shown", battleManager.DebugMessageText == "Slime attacks! Hero guards and takes 7 damage.");
        AppendCheck(ref passed, ref report, "Battle log keeps recent actions in order", battleManager.DebugBattleLogText.Contains("2. Hero guards. Next enemy attack damage is reduced.") && battleManager.DebugBattleLogText.Contains("3. Slime attacks! Hero guards and takes 7 damage."));

        battleManager.DebugStartBattleForTest();
        battleManager.DebugResolveEnemyAttackForTest();
        battleManager.DebugResolveEnemyAttackForTest();
        AppendCheck(ref passed, ref report, "Enemy intent previews strong attack before every 3rd enemy turn", battleManager.DebugEnemyIntentText == "Next Enemy: Heavy Slam (30)");
        battleManager.DebugResolveEnemyAttackForTest();
        AppendCheck(ref passed, ref report, "Slime uses a strong attack on every 3rd enemy turn", battleManager.DebugPlayerHpText == "Hero HP: 40/100");
        AppendCheck(ref passed, ref report, "Player HP bar follows repeated enemy damage", battleManager.DebugPlayerHpBarValue == 40f);
        AppendCheck(ref passed, ref report, "Damage taken tracks normal and strong enemy hits", battleManager.DebugTotalDamageTaken == 60);
        AppendCheck(ref passed, ref report, "Strong attack message is shown", battleManager.DebugMessageText == "Slime uses Heavy Slam on turn 3! Hero takes 30 damage.");

        battleManager.DebugEndBattleForTest(BattleState.Defeat);
        AppendCheck(ref passed, ref report, "Retry button is shown after defeat", battleManager.DebugRetryButtonVisible && battleManager.DebugRetryButtonInteractable);
        AppendCheck(ref passed, ref report, "Defeat summary shows result and remaining resources", battleManager.DebugResultSummaryText.Contains("Result: Defeat") && battleManager.DebugResultSummaryText.Contains("Enemy turns: 3") && battleManager.DebugResultSummaryText.Contains("Hero HP: 40/100") && battleManager.DebugResultSummaryText.Contains("Slime HP: 80/80") && battleManager.DebugResultSummaryText.Contains("Damage dealt: 0") && battleManager.DebugResultSummaryText.Contains("Damage taken: 60") && battleManager.DebugResultSummaryText.Contains("Guard uses: 0") && battleManager.DebugResultSummaryText.Contains("Skills used: 0") && battleManager.DebugResultSummaryText.Contains("Rank: C") && battleManager.DebugResultSummaryText.Contains("Reward: 0G") && battleManager.DebugResultSummaryText.Contains("Tip: Guard before Heavy Slam.") && battleManager.DebugResultSummaryText.Contains("Last enemy pattern: Heavy Slam"));
        AppendCheck(ref passed, ref report, "Result summary panel is shown after defeat", battleManager.DebugResultSummaryPanelVisible);
        battleManager.OnClickRetryButton();
        AppendCheck(ref passed, ref report, "Retry restarts battle with full HP", battleManager.DebugPlayerHpText == "Hero HP: 100/100" && battleManager.DebugMessageText.Contains("Player Turn"));
        AppendCheck(ref passed, ref report, "Retry clears result summary", string.IsNullOrEmpty(battleManager.DebugResultSummaryText));
        AppendCheck(ref passed, ref report, "Retry hides result summary panel", !battleManager.DebugResultSummaryPanelVisible);
        AppendCheck(ref passed, ref report, "Retry resets player/enemy status, intent, and combat report counters", battleManager.DebugPlayerStatusText == "Status: Ready" && battleManager.DebugEnemyStatusText == "Status: None" && battleManager.DebugEnemyIntentText == "Next Enemy: Normal Attack (15)" && battleManager.DebugGuardUseCount == 0 && battleManager.DebugSkillsUsedCount == 0);
        AppendCheck(ref passed, ref report, "Retry button hides again after restart", !battleManager.DebugRetryButtonVisible && !battleManager.DebugRetryButtonInteractable);

        battleManager.DebugEndBattleForTest(BattleState.Victory);
        AppendCheck(ref passed, ref report, "Victory summary appears after victory", battleManager.DebugResultSummaryText.Contains("Result: Victory") && battleManager.DebugResultSummaryText.Contains("Enemy turns: 0") && battleManager.DebugResultSummaryText.Contains("Skills used: 0") && battleManager.DebugResultSummaryText.Contains("Rank: S") && battleManager.DebugResultSummaryText.Contains("Reward: 150G") && battleManager.DebugResultSummaryText.Contains("Tip: Perfect clear!"));

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
            rank = "A",
            rewardGold = 120,
            resultTip = "Take less damage for a higher rank.",
            lastEnemyPattern = "Normal Attack"
        };
        string presenterSummary = BattleResultPresenter.BuildSummaryText(presenterTestData);
        AppendCheck(ref passed, ref report, "BattleResultPresenter formats result summaries from data", presenterSummary.Contains("Result: Victory") && presenterSummary.Contains("Hero HP: 70/100 | AP: 1/3") && presenterSummary.Contains("Guard uses: 1 | Skills used: 3") && presenterSummary.Contains("Reward: 120G") && presenterSummary.Contains("Last enemy pattern: Normal Attack"));

        Object.DestroyImmediate(root);

        report += passed ? "\nRESULT: PASS" : "\nRESULT: FAIL";
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

        if (!condition)
        {
            passed = false;
        }
    }
}
