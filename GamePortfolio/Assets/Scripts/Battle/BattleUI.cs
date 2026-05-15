using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles all UI rendering for the battle system.
/// Extracted from BattleManager to separate presentation from game logic.
/// </summary>
public class BattleUI : MonoBehaviour
{
    [Header("Player UI")]
    [SerializeField] private TMP_Text playerHpText;
    [SerializeField] private Slider playerHpSlider;
    [SerializeField] private TMP_Text playerApText;
    [SerializeField] private Slider playerApSlider;
    [SerializeField] private TMP_Text playerStatusText;
    [SerializeField] private Image playerSpriteImage;

    [Header("Enemy UI")]
    [SerializeField] private TMP_Text enemyHpText;
    [SerializeField] private Slider enemyHpSlider;
    [SerializeField] private TMP_Text enemyStatusText;
    [SerializeField] private TMP_Text enemyIntentText;
    [SerializeField] private TMP_Text enemyBreakText;
    [SerializeField] private Slider enemyBreakSlider;
    [SerializeField] private Image enemySpriteImage;

    [Header("Stage UI")]
    [SerializeField] private TMP_Text runStatusText;
    [SerializeField] private TMP_Text stageText;
    [SerializeField] private TMP_Text stageObjectiveText;
    [SerializeField] private TMP_Text stageProgressText;

    [Header("Message & Help")]
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private TMP_Text impactText;
    [SerializeField] private TMP_Text skillHelpText;

    [Header("Battle Log")]
    [SerializeField] private TMP_Text battleLogText;

    [Header("Result")]
    [SerializeField] private TMP_Text resultSummaryText;
    [SerializeField] private GameObject resultSummaryPanel;

    [Header("Buttons")]
    [SerializeField] private Button attackButton;
    [SerializeField] private Button fireSkillButton;
    [SerializeField] private Button endTurnButton;
    [SerializeField] private Button guardButton;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button continueButton;

    private readonly List<string> battleLogEntries = new List<string>();
    private int battleLogSequence;

    public string DebugPlayerHpText => playerHpText != null ? playerHpText.text : "";
    public string DebugPlayerApText => playerApText != null ? playerApText.text : "";
    public string DebugEnemyHpText => enemyHpText != null ? enemyHpText.text : "";
    public float DebugPlayerHpBarValue => playerHpSlider != null ? playerHpSlider.value : -1f;
    public float DebugPlayerHpBarMaxValue => playerHpSlider != null ? playerHpSlider.maxValue : -1f;
    public float DebugPlayerApBarValue => playerApSlider != null ? playerApSlider.value : -1f;
    public float DebugPlayerApBarMaxValue => playerApSlider != null ? playerApSlider.maxValue : -1f;
    public float DebugEnemyHpBarValue => enemyHpSlider != null ? enemyHpSlider.value : -1f;
    public float DebugEnemyHpBarMaxValue => enemyHpSlider != null ? enemyHpSlider.maxValue : -1f;
    public string DebugMessageText => messageText != null ? messageText.text : "";
    public string DebugSkillHelpText => skillHelpText != null ? skillHelpText.text : "";
    public string DebugBattleLogText => battleLogText != null ? battleLogText.text : "";
    public string DebugResultSummaryText => resultSummaryText != null ? resultSummaryText.text : "";
    public string DebugPlayerStatusText => playerStatusText != null ? playerStatusText.text : "";
    public string DebugEnemyStatusText => enemyStatusText != null ? enemyStatusText.text : "";
    public string DebugEnemyIntentText => enemyIntentText != null ? enemyIntentText.text : "";
    public string DebugEnemyBreakText => enemyBreakText != null ? enemyBreakText.text : "";
    public float DebugEnemyBreakBarValue => enemyBreakSlider != null ? enemyBreakSlider.value : -1f;
    public float DebugEnemyBreakBarMaxValue => enemyBreakSlider != null ? enemyBreakSlider.maxValue : -1f;
    public string DebugImpactText => impactText != null ? impactText.text : "";
    public string DebugRunStatusText => runStatusText != null ? runStatusText.text : "";
    public string DebugStageText => stageText != null ? stageText.text : "";
    public string DebugStageObjectiveText => stageObjectiveText != null ? stageObjectiveText.text : "";
    public string DebugStageProgressText => stageProgressText != null ? stageProgressText.text : "";
    public bool DebugRetryButtonVisible => retryButton != null && retryButton.gameObject.activeSelf;
    public bool DebugRetryButtonInteractable => retryButton != null && retryButton.interactable;
    public bool DebugContinueButtonVisible => continueButton != null && continueButton.gameObject.activeSelf;
    public bool DebugContinueButtonInteractable => continueButton != null && continueButton.interactable;
    public bool DebugResultSummaryPanelVisible => resultSummaryPanel != null && resultSummaryPanel.activeSelf;

    // --- Lifecycle ---

    public void SetupButtonListeners(
        UnityEngine.Events.UnityAction onAttack,
        UnityEngine.Events.UnityAction onFireSkill,
        UnityEngine.Events.UnityAction onEndTurn,
        UnityEngine.Events.UnityAction onGuard,
        UnityEngine.Events.UnityAction onRetry,
        UnityEngine.Events.UnityAction onContinue)
    {
        WireButton(attackButton, onAttack);
        WireButton(fireSkillButton, onFireSkill);
        WireButton(endTurnButton, onEndTurn);
        WireButton(guardButton, onGuard);
        WireButton(retryButton, onRetry);
        WireButton(continueButton, onContinue);
    }

    private static void WireButton(Button btn, UnityEngine.Events.UnityAction action)
    {
        if (btn == null) return;
        btn.onClick.RemoveListener(action);
        btn.onClick.AddListener(action);
    }

    public void StartNewBattle()
    {
        battleLogEntries.Clear();
        battleLogSequence = 0;
        RefreshBattleLogText();
        SetRetryButtonVisible(false);
        SetContinueButtonVisible(false);
        SetResultSummaryVisible(false, "");
        SetupPlaceholderSprites();
    }

    public void SetupPlaceholderSprites(bool isBoss = false)
    {
        if (playerSpriteImage != null && playerSpriteImage.sprite == null)
            playerSpriteImage.sprite = PlaceholderSpriteGenerator.CreateHeroSprite();
        if (enemySpriteImage != null)
            enemySpriteImage.sprite = PlaceholderSpriteGenerator.CreateEnemySprite(isBoss);
    }

    // --- Main Update ---

    public void UpdateAllUI(
        BattleState currentState,
        CharacterData player,
        CharacterData enemy,
        EnemyPatternData enemyPattern,
        int enemyTurnCount,
        int currentStageIndex,
        List<StageData> stageEncounters,
        string playerName,
        string enemyName,
        int totalGoldEarned,
        int guardReductionPercent,
        int burnTurnDuration,
        bool playerIsGuarding,
        string message,
        SkillData basicSkill,
        SkillData fireSkill,
        int maxBattleLogEntries)
    {
        SetPlayerHp(player.currentHp, player.maxHp, playerName);
        SetPlayerAp(player.currentAp, player.maxAp);
        SetPlayerStatusText(currentState, playerIsGuarding);
        SetEnemyHp(enemy.currentHp, enemy.maxHp, enemyName);
        SetEnemyStatusText(enemy);
        SetEnemyBreakText(enemy);
        SetEnemyIntentText(currentState, enemyPattern, enemyTurnCount);
        SetRunStatusText(currentState, currentStageIndex, stageEncounters);
        SetStageText(currentStageIndex, stageEncounters);
        SetStageObjectiveText(currentState, currentStageIndex, stageEncounters);
        SetStageProgressText(currentState, currentStageIndex, stageEncounters);
        SetMessageText(message);
        UpdateSkillHelpText(basicSkill, fireSkill, guardReductionPercent, enemyPattern);
        AddBattleLogEntry(message, maxBattleLogEntries);
    }

    public void SetActionButtonsInteractable(bool isInteractable)
    {
        SetButtonInteractable(attackButton, isInteractable);
        SetButtonInteractable(fireSkillButton, isInteractable);
        SetButtonInteractable(endTurnButton, isInteractable);
        SetButtonInteractable(guardButton, isInteractable);
    }

    public void UpdateActionButtons(CharacterData player, SkillData basicSkill, SkillData fireSkill, BattleState currentState)
    {
        SetButtonInteractable(attackButton, player.HasEnoughAp(basicSkill.apCost));
        SetButtonInteractable(fireSkillButton, player.HasEnoughAp(fireSkill.apCost));
        SetButtonInteractable(endTurnButton, currentState == BattleState.PlayerTurn);
        SetButtonInteractable(guardButton, currentState == BattleState.PlayerTurn);
    }

    // --- Result ---

    public void SetResultSummaryVisible(bool isVisible, string summary)
    {
        if (resultSummaryText != null)
        {
            resultSummaryText.text = summary;
            resultSummaryText.gameObject.SetActive(isVisible);
        }
        if (resultSummaryPanel != null)
            resultSummaryPanel.SetActive(isVisible);
    }

    public void SetRetryButtonVisible(bool isVisible)
    {
        if (retryButton == null) return;
        retryButton.interactable = isVisible;
        retryButton.gameObject.SetActive(isVisible);
    }

    public void SetContinueButtonVisible(bool isVisible)
    {
        if (continueButton == null) return;
        continueButton.interactable = isVisible;
        continueButton.gameObject.SetActive(isVisible);
    }

    // --- Private helpers ---

    private void SetPlayerHp(int current, int max, string name)
    {
        if (playerHpText != null)
            playerHpText.text = BuildResourceText($"{name} HP", current, max);
        UpdateResourceSlider(playerHpSlider, current, max);
    }

    private void SetPlayerAp(int current, int max)
    {
        if (playerApText != null)
            playerApText.text = BuildResourceText("AP", current, max);
        UpdateResourceSlider(playerApSlider, current, max);
    }

    private void SetEnemyHp(int current, int max, string name)
    {
        if (enemyHpText != null)
            enemyHpText.text = BuildResourceText($"{name} HP", current, max);
        UpdateResourceSlider(enemyHpSlider, current, max);
    }

    private void SetPlayerStatusText(BattleState state, bool isGuarding)
    {
        if (playerStatusText == null) return;
        if (state == BattleState.Victory || state == BattleState.Defeat)
            playerStatusText.text = "Status: Battle ended";
        else if (isGuarding)
            playerStatusText.text = "Status: Guarding";
        else
            playerStatusText.text = "Status: Ready";
    }

    private void SetEnemyStatusText(CharacterData enemy)
    {
        if (enemyStatusText == null) return;
        enemyStatusText.text = enemy.currentStatusEffect == StatusEffectType.None
            ? "Status: None"
            : $"Status: {enemy.currentStatusEffect} ({enemy.statusTurnsRemaining} turns)";
    }

    private void SetEnemyBreakText(CharacterData enemy)
    {
        if (enemyBreakText != null)
            enemyBreakText.text = enemy.DebugBuildBreakText();
        if (enemyBreakSlider != null)
        {
            enemyBreakSlider.minValue = 0f;
            enemyBreakSlider.maxValue = enemy.maxBreakGauge;
            enemyBreakSlider.value = enemy.isBroken ? 0f : enemy.currentBreakGauge;
        }
    }

    private void SetEnemyIntentText(BattleState state, EnemyPatternData pattern, int turnCount)
    {
        if (enemyIntentText == null) return;
        if (state == BattleState.Victory || state == BattleState.Defeat)
        {
            enemyIntentText.text = "Next Enemy: Battle ended";
            return;
        }
        int nextTurn = turnCount + 1;
        enemyIntentText.text = pattern.IsStrongAttackTurn(nextTurn)
            ? $"Next Enemy: {pattern.strongAttackName} ({pattern.strongAttackDamage})"
            : $"Next Enemy: Normal Attack ({pattern.normalAttackDamage})";
    }

    private void SetRunStatusText(BattleState state, int stageIndex, List<StageData> encounters)
    {
        if (runStatusText == null) return;
        if (state == BattleState.Victory)
            runStatusText.text = HasNextStage(stageIndex, encounters)
                ? "Run Status: Encounter Clear - Continue to Next"
                : "Run Status: Final Clear - Stage 1 Complete";
        else if (state == BattleState.Defeat)
            runStatusText.text = "Run Status: Retry Current Encounter";
        else
            runStatusText.text = "Run Status: Stage 1 In Progress";
    }

    private void SetStageText(int stageIndex, List<StageData> encounters)
    {
        if (stageText == null) return;
        var stage = GetStageData(stageIndex, encounters);
        stageText.text = stage != null ? stage.BuildDisplayName() : "Stage: Unknown";
    }

    private void SetStageObjectiveText(BattleState state, int stageIndex, List<StageData> encounters)
    {
        if (stageObjectiveText == null) return;
        var current = GetStageData(stageIndex, encounters);
        if (current == null) { stageObjectiveText.text = "Objective: Unknown"; return; }

        if (state == BattleState.Victory)
        {
            if (HasNextStage(stageIndex, encounters))
            {
                var next = GetStageData(stageIndex + 1, encounters);
                string nextName = next != null ? next.BuildDisplayName() : "next encounter";
                stageObjectiveText.text = $"Objective Complete: {current.BuildDisplayName()} | Continue to {nextName}";
            }
            else
                stageObjectiveText.text = "Objective Complete: Stage 1 cleared | Final Clear";
        }
        else if (state == BattleState.Defeat)
            stageObjectiveText.text = $"Objective Failed: Retry {current.BuildDisplayName()}";
        else
            stageObjectiveText.text = current.BuildObjectiveText();
    }

    private void SetStageProgressText(BattleState state, int stageIndex, List<StageData> encounters)
    {
        if (stageProgressText == null) return;
        int count = encounters?.Count ?? 0;
        int currentNum = Mathf.Clamp(stageIndex + 1, 1, Mathf.Max(1, count));
        string statusLabel = "Active";
        if (state == BattleState.Victory)
            statusLabel = HasNextStage(stageIndex, encounters) ? "Encounter Clear" : "Stage Clear";
        else if (state == BattleState.Defeat)
            statusLabel = "Retry Needed";
        stageProgressText.text = $"Progress: Encounter {currentNum}/{count} | {statusLabel}";
    }

    private void SetMessageText(string message)
    {
        if (messageText != null)
            messageText.text = message;
    }

    public void SetImpactText(string text)
    {
        if (impactText != null)
            impactText.text = text;
    }

    private void UpdateSkillHelpText(SkillData basicSkill, SkillData fireSkill, int guardReduction, EnemyPatternData pattern)
    {
        if (skillHelpText == null || basicSkill == null || fireSkill == null) return;
        string attackHelp = BuildSkillHelpLine(basicSkill);
        string fireHelp = BuildSkillHelpLine(fireSkill);
        string guardHelp = $"Guard: reduce next enemy attack by {guardReduction}%.";
        string turnHint = pattern?.BuildPatternHelpText() ?? "";
        skillHelpText.text = $"{attackHelp}\n{fireHelp}\n{guardHelp}\n{turnHint}";
    }

    private string BuildSkillHelpLine(SkillData skill)
    {
        string line = $"{skill.skillName}: {skill.power} power, {skill.apCost} AP, {skill.elementType}.";
        if (skill.HasStatusEffect()) line += $" Applies {skill.statusEffectType}.";
        if (!string.IsNullOrWhiteSpace(skill.description)) line += $" {skill.description}";
        return line;
    }

    private void AddBattleLogEntry(string message, int maxEntries)
    {
        if (string.IsNullOrWhiteSpace(message) || message == "Battle Start!") return;
        battleLogSequence++;
        battleLogEntries.Add($"{battleLogSequence}. {message}");
        while (battleLogEntries.Count > maxEntries)
            battleLogEntries.RemoveAt(0);
        RefreshBattleLogText();
    }

    private void RefreshBattleLogText()
    {
        if (battleLogText == null) return;
        battleLogText.text = battleLogEntries.Count == 0
            ? "Recent Actions\nNo actions yet."
            : "Recent Actions\n" + string.Join("\n", battleLogEntries);
    }

    // --- Utility ---

    public string BuildVictoryGuideMessage(int stageIndex, List<StageData> encounters)
    {
        var current = GetStageData(stageIndex, encounters);
        string currentName = current != null ? current.BuildDisplayName() : "Encounter";
        if (HasNextStage(stageIndex, encounters))
        {
            var next = GetStageData(stageIndex + 1, encounters);
            string nextName = next != null ? next.BuildDisplayName() : "the next encounter";
            return $"Victory! {currentName} cleared. Press Continue to enter {nextName}.";
        }
        return "Final Clear! Stage 1 completed. Review Total Gold, then Retry the boss if you want to practice.";
    }

    public void ClearBattleLog()
    {
        battleLogEntries.Clear();
        battleLogSequence = 0;
        RefreshBattleLogText();
    }

    public void SetResultText(string text)
    {
        if (resultSummaryText != null) resultSummaryText.text = text;
    }

    // --- Static helpers ---

    private static string BuildResourceText(string label, int currentValue, int maxValue)
    {
        int pct = maxValue > 0 ? Mathf.RoundToInt((float)currentValue / maxValue * 100f) : 0;
        return $"{label}: {currentValue}/{maxValue} ({pct}%)";
    }

    private static void UpdateResourceSlider(Slider slider, int currentValue, int maxValue)
    {
        if (slider == null) return;
        slider.minValue = 0f;
        slider.maxValue = maxValue;
        slider.value = Mathf.Clamp(currentValue, 0, maxValue);
    }

    private static void SetButtonInteractable(Button btn, bool interactable)
    {
        if (btn != null) btn.interactable = interactable;
    }

    private static StageData GetStageData(int index, List<StageData> encounters)
    {
        if (encounters == null || index < 0 || index >= encounters.Count) return null;
        return encounters[index];
    }

    private static bool HasNextStage(int stageIndex, List<StageData> encounters)
    {
        return encounters != null && stageIndex < encounters.Count - 1;
    }
}
