using System.Collections;
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
    [SerializeField] private TMP_Text playerShieldText;
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
    [SerializeField] private Button iceSkillButton;
    [SerializeField] private Button lightningSkillButton;
    [SerializeField] private Button earthSkillButton;
    [SerializeField] private Button endTurnButton;
    [SerializeField] private Button guardButton;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button continueButton;
    /// <summary>Child Text component of continueButton, for dynamic label.</summary>
    private TMP_Text continueButtonText;
    [SerializeField] private Button stageSelectButton;
    [SerializeField] private Button speedToggleButton;
    [SerializeField] private Button autoBattleButton;
    [SerializeField] private TMP_Text autoBattleIndicatorText;

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
    public bool DebugStageSelectButtonVisible => stageSelectButton != null && stageSelectButton.gameObject.activeSelf;
    public bool DebugStageSelectButtonInteractable => stageSelectButton != null && stageSelectButton.interactable;
    public bool DebugResultSummaryPanelVisible => resultSummaryPanel != null && resultSummaryPanel.activeSelf;

    // --- Lifecycle ---

    public void SetupButtonListeners(
        UnityEngine.Events.UnityAction onAttack,
        UnityEngine.Events.UnityAction onFireSkill,
        UnityEngine.Events.UnityAction onIceSkill,
        UnityEngine.Events.UnityAction onLightningSkill,
        UnityEngine.Events.UnityAction onEarthSkill,
        UnityEngine.Events.UnityAction onEndTurn,
        UnityEngine.Events.UnityAction onGuard,
        UnityEngine.Events.UnityAction onRetry,
        UnityEngine.Events.UnityAction onContinue,
        UnityEngine.Events.UnityAction onStageSelect = null,
        UnityEngine.Events.UnityAction onSpeedToggle = null,
        UnityEngine.Events.UnityAction onAutoBattleToggle = null)
    {
        WireButton(attackButton, onAttack);
        WireButton(fireSkillButton, onFireSkill);
        WireButton(iceSkillButton, onIceSkill);
        WireButton(lightningSkillButton, onLightningSkill);
        WireButton(earthSkillButton, onEarthSkill);
        WireButton(endTurnButton, onEndTurn);
        WireButton(guardButton, onGuard);
        WireButton(retryButton, onRetry);
        WireButton(continueButton, onContinue);
        if (onStageSelect != null)
            WireButton(stageSelectButton, onStageSelect);
        if (onSpeedToggle != null)
            WireButton(speedToggleButton, onSpeedToggle);
        if (onAutoBattleToggle != null)
            WireButton(autoBattleButton, onAutoBattleToggle);
    }

    private static void WireButton(Button btn, UnityEngine.Events.UnityAction action)
    {
        if (btn == null) return;
        btn.onClick.RemoveListener(action);
        btn.onClick.AddListener(action);
    }

    public void SetAutoBattleIndicator(bool enabled)
    {
        if (autoBattleIndicatorText == null && autoBattleButton != null)
            autoBattleIndicatorText = autoBattleButton.GetComponentInChildren<TMP_Text>();
        if (autoBattleIndicatorText != null)
            autoBattleIndicatorText.text = enabled ? "Auto: ON" : "Auto: OFF";
    }

    public void SetSpeedToggleButton(int speedState, Button speedButton)
    {
        if (speedButton == null) return;
        if (speedToggleButton == null) speedToggleButton = speedButton;
        UpdateSpeedLabel(speedState);
    }

    public void UpdateSpeedLabel(int speedState)
    {
        if (speedToggleButton == null) return;
        TMP_Text label = speedToggleButton.GetComponentInChildren<TMP_Text>();
        if (label != null) label.text = speedState >= 2 ? "2x" : "1x";
    }

    public void StartNewBattle()
    {
        battleLogEntries.Clear();
        battleLogSequence = 0;
        RefreshBattleLogText();
        SetRetryButtonVisible(false);
        SetContinueButtonVisible(false);
        SetStageSelectButtonVisible(false);
        SetResultSummaryVisible(false, "");
        SetupPlaceholderSprites();
        // Cache continue button's child text component if not yet set
        if (continueButtonText == null && continueButton != null)
            continueButtonText = continueButton.GetComponentInChildren<TMP_Text>();
    }

    public void SetContinueButtonLabel(string label)
    {
        if (continueButtonText != null) continueButtonText.text = label;
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
        SkillData iceSkill,
        SkillData lightningSkill,
        SkillData earthSkill,
        int maxBattleLogEntries)
    {
        SetPlayerHp(player.currentHp, player.maxHp, playerName);
        SetPlayerAp(player.currentAp, player.maxAp);
        SetPlayerStatusText(currentState, playerIsGuarding);
        SetEnemyHp(enemy.currentHp, enemy.maxHp, enemyName);
        SetEnemyStatusText(enemy);
        SetEnemyBreakText(enemy);
        SetEnemyElementLabel(enemy.weaknessElement);
        SetEnemyIntentText(currentState, enemyPattern, enemyTurnCount);
        SetRunStatusText(currentState, currentStageIndex, stageEncounters);
        SetStageText(currentStageIndex, stageEncounters);
        SetStageObjectiveText(currentState, currentStageIndex, stageEncounters);
        SetStageProgressText(currentState, currentStageIndex, stageEncounters);
        SetMessageText(message);
        UpdateSkillHelpText(basicSkill, fireSkill, iceSkill, lightningSkill, earthSkill, guardReductionPercent, enemyPattern);
        AddBattleLogEntry(message, maxBattleLogEntries);
    }

    public void SetActionButtonsInteractable(bool isInteractable)
    {
        SetButtonInteractable(attackButton, isInteractable);
        SetButtonInteractable(fireSkillButton, isInteractable);
        SetButtonInteractable(iceSkillButton, isInteractable);
        SetButtonInteractable(lightningSkillButton, isInteractable);
        SetButtonInteractable(earthSkillButton, isInteractable);
        SetButtonInteractable(endTurnButton, isInteractable);
        SetButtonInteractable(guardButton, isInteractable);
    }

    public void UpdateActionButtons(CharacterData player, SkillData basicSkill, SkillData fireSkill, SkillData iceSkill, SkillData lightningSkill, SkillData earthSkill, BattleState currentState)
    {
        SetButtonInteractable(attackButton, player.HasEnoughAp(basicSkill.apCost));
        SetButtonInteractable(fireSkillButton, player.HasEnoughAp(fireSkill.apCost));
        SetButtonInteractable(iceSkillButton, player.HasEnoughAp(iceSkill.apCost));
        SetButtonInteractable(lightningSkillButton, player.HasEnoughAp(lightningSkill.apCost));
        SetButtonInteractable(earthSkillButton, player.HasEnoughAp(earthSkill.apCost));
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

    public void SetStageSelectButtonVisible(bool isVisible)
    {
        if (stageSelectButton == null) return;
        stageSelectButton.interactable = isVisible;
        stageSelectButton.gameObject.SetActive(isVisible);
    }

    // --- Private helpers ---

    private void SetPlayerHp(int current, int max, string name)
    {
        if (playerHpText != null)
            playerHpText.text = BuildResourceText($"{name} HP", current, max);
        UpdateResourceSlider(playerHpSlider, current, max);
        SetSliderColorByRatio(playerHpSlider, current, max, new Color(0.22f, 0.72f, 0.38f), new Color(0.85f, 0.72f, 0.18f), new Color(0.82f, 0.22f, 0.24f));
    }

    private void SetPlayerAp(int current, int max)
    {
        if (playerApText != null)
            playerApText.text = BuildResourceText("AP", current, max);
        UpdateResourceSlider(playerApSlider, current, max);
        SetSliderColorByRatio(playerApSlider, current, max, new Color(0.26f, 0.56f, 1.0f), new Color(0.26f, 0.86f, 0.76f), new Color(0.92f, 0.56f, 0.18f));
    }

    private void SetEnemyHp(int current, int max, string name)
    {
        if (enemyHpText != null)
            enemyHpText.text = BuildResourceText($"{name} HP", current, max);
        UpdateResourceSlider(enemyHpSlider, current, max);
        SetSliderColorByRatio(enemyHpSlider, current, max, new Color(0.22f, 0.72f, 0.38f), new Color(0.85f, 0.72f, 0.18f), new Color(0.82f, 0.22f, 0.24f));
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

    private string enemyElementLabel = "";
    public void SetEnemyElementLabel(ElementType element)
    {
        if (element == ElementType.None || element == ElementType.Physical)
            enemyElementLabel = "";
        else
            enemyElementLabel = $"[{element}] ";
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
            ? $"Next Enemy: {enemyElementLabel}{pattern.strongAttackName} ({pattern.strongAttackDamage})"
            : $"Next Enemy: {enemyElementLabel}Normal Attack ({pattern.normalAttackDamage})";
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

    public void SetPlayerShieldText(int shieldAmount)
    {
        if (playerShieldText != null)
            playerShieldText.text = shieldAmount > 0 ? $"Shield: {shieldAmount}" : "";
    }

    /// <summary>Brief flash effect on enemy sprite when damage is dealt.</summary>
    public void FlashEnemyDamage()
    {
        if (enemySpriteImage != null)
            StartCoroutine(FlashRoutine(enemySpriteImage, Color.white, 0.1f));
    }

    /// <summary>Brief flash effect on player sprite when damaged.</summary>
    public void FlashPlayerDamage()
    {
        if (playerSpriteImage != null)
            StartCoroutine(FlashRoutine(playerSpriteImage, Color.red, 0.15f));
    }

    private IEnumerator FlashRoutine(Image target, Color flashColor, float duration)
    {
        if (target == null) yield break;
        Color original = target.color;
        target.color = flashColor;
        yield return new WaitForSeconds(duration);
        target.color = original;
    }

    private void UpdateSkillHelpText(SkillData basicSkill, SkillData fireSkill, SkillData iceSkill, SkillData lightningSkill, SkillData earthSkill, int guardReduction, EnemyPatternData pattern)
    {
        if (skillHelpText == null || basicSkill == null || fireSkill == null || iceSkill == null || lightningSkill == null || earthSkill == null) return;
        string attackHelp = BuildSkillHelpLine(basicSkill);
        string fireHelp = BuildSkillHelpLine(fireSkill);
        string iceHelp = BuildSkillHelpLine(iceSkill);
        string lightningHelp = BuildSkillHelpLine(lightningSkill);
        string earthHelp = BuildSkillHelpLine(earthSkill);
        string guardHelp = $"Guard: reduce next enemy attack by {guardReduction}%.";
        string turnHint = pattern?.BuildPatternHelpText() ?? "";
        skillHelpText.text = $"{attackHelp}\n{fireHelp}\n{iceHelp}\n{lightningHelp}\n{earthHelp}\n{guardHelp}\n{turnHint}";
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

    public void SetLevelText(string playerName, int playerMaxHp, int level)
    {
        if (runStatusText != null)
        {
            string baseText = runStatusText.text;
            // Prepend level info if not already there
            if (!baseText.Contains("Lv."))
                runStatusText.text = $"Lv.{level} | {baseText}";
        }
    }

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

    private static void SetSliderColorByRatio(Slider slider, int current, int max, Color highColor, Color midColor, Color lowColor)
    {
        if (slider == null) return;
        Image fill = slider.fillRect?.GetComponent<Image>();
        if (fill == null) return;
        float ratio = max > 0 ? (float)current / max : 0f;
        if (ratio > 0.60f)
            fill.color = highColor;
        else if (ratio > 0.30f)
            fill.color = midColor;
        else
            fill.color = lowColor;
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
