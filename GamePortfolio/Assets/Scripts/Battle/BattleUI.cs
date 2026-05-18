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
    [SerializeField] private Image burnOverlay;
    [SerializeField] private Image stunOverlay;
    [SerializeField] private Image brokenOverlay;

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
    [SerializeField] private Button itemButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private TMP_Text autoBattleIndicatorText;

    private readonly List<string> battleLogEntries = new List<string>();
    private int battleLogSequence;

    [Header("Pause Menu")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button quitButton;

    [Header("VFX")]
    [SerializeField] private Image screenFlashImage;
    private Canvas cachedCanvas;
    private Transform cachedCanvasTransform;

    [Header("Result Panel Styling")]
    [SerializeField] private Image resultPanelBackground;

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
        UnityEngine.Events.UnityAction onAutoBattleToggle = null,
        UnityEngine.Events.UnityAction onItem = null,
        UnityEngine.Events.UnityAction onPause = null)
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
        if (onItem != null)
            WireButton(itemButton, onItem);
        if (pauseButton != null)
        {
            WireButton(pauseButton, onPause);
        }
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
        // Cache continue button's child text component if not yet set
        if (continueButtonText == null && continueButton != null)
            continueButtonText = continueButton.GetComponentInChildren<TMP_Text>();
        // Apply element colors to skill buttons
        StyleSkillButtons();
    }

    /// <summary>Applies element-appropriate colors to skill buttons for visual hierarchy.</summary>
    public void StyleSkillButtons()
    {
        StyleButtonWithElement(attackButton, ElementType.Physical, "⚔");
        StyleButtonWithElement(fireSkillButton, ElementType.Fire, "🔥");
        StyleButtonWithElement(iceSkillButton, ElementType.Ice, "❄");
        StyleButtonWithElement(lightningSkillButton, ElementType.Lightning, "⚡");
        StyleButtonWithElement(earthSkillButton, ElementType.Earth, "\U0001F33F");
        StyleButtonWithTint(guardButton, ElementGuardColor, "🛡");
        StyleButtonWithTint(endTurnButton, ElementEndTurnColor, "⏭");
        if (itemButton != null)
        {
            Image img = itemButton.GetComponent<Image>();
            if (img != null) img.color = new Color(0.15f, 0.30f, 0.22f, 0.90f);
            TMP_Text lbl = itemButton.GetComponentInChildren<TMP_Text>();
            if (lbl != null) lbl.text = "🧪 Items";
        }
    }

    private void StyleButtonWithElement(Button btn, ElementType element, string symbol)
    {
        if (btn == null) return;
        Image img = btn.GetComponent<Image>();
        if (img != null)
        {
            Color baseColor = GetElementButtonColor(element);
            img.color = new Color(baseColor.r * 0.35f, baseColor.g * 0.35f, baseColor.b * 0.35f, 0.85f);
        }
        TMP_Text label = btn.GetComponentInChildren<TMP_Text>();
        if (label != null)
        {
            Color elemColor = GetElementButtonColor(element);
            label.color = new Color(elemColor.r * 0.9f + 0.3f, elemColor.g * 0.9f + 0.3f, elemColor.b * 0.9f + 0.3f);
            label.text = $"{symbol} {label.text.Replace("⚔", "").Replace("🔥", "").Replace("❄", "").Replace("⚡", "").Replace("\U0001F33F", "").Replace("🛡", "").Replace("⏭", "").Replace("🧪", "").Trim()}";
        }
    }

    private void StyleButtonWithTint(Button btn, Color tint, string symbol)
    {
        if (btn == null) return;
        Image img = btn.GetComponent<Image>();
        if (img != null)
            img.color = new Color(tint.r * 0.30f, tint.g * 0.30f, tint.b * 0.30f, 0.85f);
        TMP_Text label = btn.GetComponentInChildren<TMP_Text>();
        if (label != null)
        {
            label.color = new Color(tint.r * 0.9f + 0.3f, tint.g * 0.9f + 0.3f, tint.b * 0.9f + 0.3f);
            label.text = $"{symbol} {label.text.Replace("⚔", "").Replace("🔥", "").Replace("❄", "").Replace("⚡", "").Replace("\U0001F33F", "").Replace("🛡", "").Replace("⏭", "").Replace("🧪", "").Trim()}";
        }
    }

    public void SetContinueButtonLabel(string label)
    {
        if (continueButtonText != null) continueButtonText.text = label;
    }

    public void SetupPlaceholderSprites(ElementType enemyElement = ElementType.Fire, bool isBoss = false)
    {
        if (playerSpriteImage != null && playerSpriteImage.sprite == null)
            playerSpriteImage.sprite = PlaceholderSpriteGenerator.CreateHeroSprite();
        if (enemySpriteImage != null)
            enemySpriteImage.sprite = PlaceholderSpriteGenerator.CreateEnemySprite(enemyElement, isBoss);
    }

    /// <summary>Clears cached sprite references so they can be regenerated on next battle.</summary>
    public void ClearCachedSprites()
    {
        if (playerSpriteImage != null) playerSpriteImage.sprite = null;
        if (enemySpriteImage != null) enemySpriteImage.sprite = null;
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
        SetButtonInteractable(attackButton, player.HasEnoughAp(basicSkill.apCost) && ProgressState.IsSkillUnlocked(basicSkill.skillName));
        SetButtonInteractable(fireSkillButton, player.HasEnoughAp(fireSkill.apCost) && ProgressState.IsSkillUnlocked(fireSkill.skillName));
        SetButtonInteractable(iceSkillButton, player.HasEnoughAp(iceSkill.apCost) && ProgressState.IsSkillUnlocked(iceSkill.skillName));
        SetButtonInteractable(lightningSkillButton, player.HasEnoughAp(lightningSkill.apCost) && ProgressState.IsSkillUnlocked(lightningSkill.skillName));
        SetButtonInteractable(earthSkillButton, player.HasEnoughAp(earthSkill.apCost) && ProgressState.IsSkillUnlocked(earthSkill.skillName));
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
        // Style the result panel background
        if (resultPanelBackground != null)
        {
            resultPanelBackground.gameObject.SetActive(isVisible);
            bool isVictory = summary != null && summary.Contains("VICTORY");
            resultPanelBackground.color = isVictory
                ? new Color(0.05f, 0.08f, 0.15f, 0.92f)  // dark blue-ish for victory
                : new Color(0.15f, 0.05f, 0.05f, 0.92f);  // dark red-ish for defeat
        }
        // Style the result text
        if (resultSummaryText != null && isVisible)
        {
            bool isVictory = summary != null && summary.Contains("VICTORY");
            resultSummaryText.color = isVictory
                ? new Color(1f, 0.85f, 0.4f)   // gold for victory
                : new Color(1f, 0.3f, 0.3f);    // red for defeat
            resultSummaryText.fontSize = 24;
            resultSummaryText.alignment = TMPro.TextAlignmentOptions.Center;
        }
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

    private static readonly Color ElementPhysicalColor = new Color(0.70f, 0.70f, 0.75f);
    private static readonly Color ElementFireColor = new Color(0.85f, 0.25f, 0.10f);
    private static readonly Color ElementIceColor = new Color(0.20f, 0.55f, 0.95f);
    private static readonly Color ElementLightningColor = new Color(0.95f, 0.80f, 0.15f);
    private static readonly Color ElementEarthColor = new Color(0.35f, 0.70f, 0.25f);
    private static readonly Color ElementGuardColor = new Color(0.30f, 0.60f, 0.85f);
    private static readonly Color ElementEndTurnColor = new Color(0.75f, 0.30f, 0.30f);

    public static Color GetElementButtonColor(ElementType element)
    {
        return element switch
        {
            ElementType.Fire => ElementFireColor,
            ElementType.Ice => ElementIceColor,
            ElementType.Lightning => ElementLightningColor,
            ElementType.Earth => ElementEarthColor,
            _ => ElementPhysicalColor
        };
    }

    // Element badge icons as simple text symbols
    private static readonly string[] ElementSymbols = { "", "⚔", "🔥", "❄", "⚡", "\U0001F33F", "🌑", "✨" };
    public static string GetElementSymbol(ElementType element)
    {
        int idx = (int)element;
        return idx >= 0 && idx < ElementSymbols.Length ? ElementSymbols[idx] : "";
    }

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

    public void SetEnemyStatusText(CharacterData enemy)
    {
        if (enemyStatusText == null) return;
        enemyStatusText.text = enemy.currentStatusEffect == StatusEffectType.None
            ? "Status: None"
            : $"Status: {enemy.currentStatusEffect} ({enemy.statusTurnsRemaining} turns)";

        // Update status overlays
        bool hasBurn = enemy.currentStatusEffect == StatusEffectType.Burn;
        bool hasStun = enemy.currentStatusEffect == StatusEffectType.Stun;
        if (burnOverlay != null)
        {
            burnOverlay.gameObject.SetActive(hasBurn);
            if (hasBurn) StartCoroutine(PulseOverlay(burnOverlay, 0.5f, new Color(1f, 0.3f, 0.1f, 0.3f)));
        }
        if (stunOverlay != null)
        {
            stunOverlay.gameObject.SetActive(hasStun);
            if (hasStun) StartCoroutine(PulseOverlay(stunOverlay, 0.3f, new Color(0.3f, 0.5f, 1f, 0.3f)));
        }
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
        if (brokenOverlay != null)
            brokenOverlay.gameObject.SetActive(enemy.isBroken);
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

    public Vector3 GetPlayerSpriteWorldPosition()
    {
        if (playerSpriteImage != null)
            return playerSpriteImage.rectTransform.position;
        return Vector3.zero;
    }

    public Vector3 GetEnemySpriteWorldPosition()
    {
        if (enemySpriteImage != null)
            return enemySpriteImage.rectTransform.position;
        return Vector3.zero;
    }

    public Transform GetProjectileParent()
    {
        Canvas canvas = playerSpriteImage != null ? playerSpriteImage.GetComponentInParent<Canvas>() : GetComponentInParent<Canvas>();
        return canvas != null ? canvas.transform : transform;
    }

    public void SetPauseVisible(bool visible)
    {
        if (pausePanel != null) pausePanel.SetActive(visible);
    }

    public void SetupPauseListeners(UnityEngine.Events.UnityAction onResume, UnityEngine.Events.UnityAction onQuit)
    {
        if (resumeButton != null) { resumeButton.onClick.RemoveAllListeners(); resumeButton.onClick.AddListener(onResume); }
        if (quitButton != null) { quitButton.onClick.RemoveAllListeners(); quitButton.onClick.AddListener(onQuit); }
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

    /// <summary>Pulses an overlay image between transparent and tinted for status visual feedback.</summary>
    private IEnumerator PulseOverlay(Image overlay, float speed, Color tint)
    {
        if (overlay == null) yield break;
        float t = 0f;
        while (overlay.gameObject.activeSelf)
        {
            float alpha = Mathf.Abs(Mathf.Sin(t * Mathf.PI * speed)) * 0.5f + 0.1f;
            overlay.color = new Color(tint.r, tint.g, tint.b, alpha);
            t += Time.deltaTime;
            yield return null;
        }
        overlay.color = Color.clear;
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
        if (!ProgressState.IsSkillUnlocked(skill.skillName))
            return $"{skill.skillName}: Locked — complete earlier stages to unlock.";
        string line = $"{skill.skillName}: {skill.power} power, {skill.apCost} AP, {skill.elementType}.";
        if (skill.HasStatusEffect()) line += $" Applies {skill.statusEffectType}.";
        if (!string.IsNullOrWhiteSpace(skill.description)) line += $" {skill.description}";
        return line;
    }

    private void AddBattleLogEntry(string message, int maxEntries)
    {
        if (string.IsNullOrWhiteSpace(message) || message == "Battle Start!") return;
        battleLogSequence++;
        string formatted = FormatLogMessage(message);
        battleLogEntries.Add($"{battleLogSequence}. {formatted}");
        while (battleLogEntries.Count > maxEntries)
            battleLogEntries.RemoveAt(0);
        RefreshBattleLogText();
    }

    /// <summary>Formats a message for the battle log with special prefixes for important events.</summary>
    private string FormatLogMessage(string message)
    {
        // Detect important events and prepend short, styled prefixes
        string lower = message.ToLowerInvariant();

        if (lower.Contains("guarded") || lower.Contains("guards"))
            return "🛡 Guarded!";
        if (lower.Contains("break!"))
            return "💥 BREAK!";
        if (lower.Contains("weakness"))
            return "⚡ Weakness Hit!";
        if (lower.Contains("victory") || lower.Contains("victory!"))
            return "🏆 Victory!";
        if (lower.Contains("defeat") || lower.Contains("defeated"))
            return "💀 Defeated!";
        if (lower.Contains("guards. next enemy attack damage is reduced"))
            return "🛡 Guarded! Damage reduced.";
        if (lower.Contains("is stunn"))
            return "⏳ Enemy STUNNED! Skips turn.";
        if (lower.Contains("burn damage"))
            return "🔥 Burn: " + ExtractFirstNumber(message) + " dmg";
        if (lower.Contains("shield") && lower.Contains("active"))
            return "🛡️ Shield Active!";
        if (lower.Contains("enraged"))
            return "⚠️ ENRAGED!";
        if (lower.Contains("level up"))
            return "⬆ Level Up!";
        if (lower.Contains("is locked"))
            return "🔒 " + message;
        if (lower.Contains("not enough ap"))
            return "⚠️ Not enough AP";
        if (lower.Contains("no items"))
            return "⚠️ No items available";

        // Item effects: must come before generic "uses" check
        if (lower.Contains("restores hp") || lower.Contains("restores hp."))
            return "❤️ +" + ExtractFirstNumber(message) + " HP";
        if (lower.Contains("restores ap") || lower.Contains("restores ap."))
            return "💧 +" + ExtractFirstNumber(message) + " AP";
        if (lower.Contains("uses shield") || lower.Contains("shield active"))
            return "🛡️ Shield Active!";

        // Generic skill use: "Hero uses Slash! Slime takes 22 damage."
        // Format as "⚔ 22 dmg" for physical, "🔥 30 dmg" for elemental
        if (lower.Contains(" uses "))
        {
            string formatted = ShortenSkillMessage(message);
            if (!string.IsNullOrEmpty(formatted)) return formatted;
        }

        // Enemy attack messages
        if (lower.Contains("takes") && lower.Contains("damage") && !lower.Contains("uses"))
        {
            int dmg = ExtractFirstNumber(message);
            if (dmg > 0) return $"💥 {dmg} dmg";
        }

        // Item usage
        if (lower.Contains("restores") && lower.Contains("hp"))
            return "❤️ +" + ExtractFirstNumber(message) + " HP";
        if (lower.Contains("restores") && lower.Contains("ap"))
            return "💎 +" + ExtractFirstNumber(message) + " AP";

        // Fallback: shorten to just the number
        return ShortenPlainMessage(message);
    }

    private string ShortenSkillMessage(string message)
    {
        // Pattern: "Hero uses Slash! Slime takes 22 damage. (Physical | Physical)"
        // We want: "⚔ 22 dmg"
        int dmg = ExtractLastNumber(message);
        if (dmg <= 0) return null;

        string lower = message.ToLowerInvariant();
        if (lower.Contains("physical"))
            return $"⚔ {dmg} dmg";
        if (lower.Contains("fire"))
            return $"🔥 {dmg} dmg";
        if (lower.Contains("ice"))
            return $"❄ {dmg} dmg";
        if (lower.Contains("lightning"))
            return $"⚡ {dmg} dmg";
        if (lower.Contains("earth"))
            return $"🪨 {dmg} dmg";
        return $"• {dmg} dmg";
    }

    private string ShortenPlainMessage(string message)
    {
        // Just take the first meaningful part, strip long explanations
        int dmg = ExtractFirstNumber(message);
        if (dmg > 0) return $"💥 {dmg} dmg";
        if (message.Length > 50) return message.Substring(0, 47) + "...";
        return message;
    }

    private static int ExtractFirstNumber(string text)
    {
        if (string.IsNullOrEmpty(text)) return 0;
        var match = System.Text.RegularExpressions.Regex.Match(text, @"(\d+)");
        return match.Success ? int.Parse(match.Groups[1].Value) : 0;
    }

    private static int ExtractLastNumber(string text)
    {
        if (string.IsNullOrEmpty(text)) return 0;
        var matches = System.Text.RegularExpressions.Regex.Matches(text, @"(\d+)");
        if (matches.Count == 0) return 0;
        return int.Parse(matches[matches.Count - 1].Value);
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

    // --- VFX / Feedback ---

    /// <summary>Shows a floating damage number near the enemy.</summary>
    public void ShowDamageNumber(int damage, bool isWeaknessHit = false)
    {
        Vector3 pos = GetEnemySpriteWorldPosition();
        Transform canvasTf = GetDamagePopupParent();
        if (isWeaknessHit)
            DamagePopup.ShowWeaknessHit(damage, pos, canvasTf);
        else
            DamagePopup.ShowDamage(damage, pos, canvasTf);
    }

    /// <summary>Shows a floating damage number near the player (for enemy attacks).</summary>
    public void ShowDamageNumberOnPlayer(int damage)
    {
        Vector3 pos = GetPlayerSpriteWorldPosition();
        DamagePopup.ShowDamage(damage, pos, GetDamagePopupParent());
    }

    /// <summary>Shows a floating heal number near the player.</summary>
    public void ShowHealNumber(int heal)
    {
        Vector3 pos = GetPlayerSpriteWorldPosition();
        DamagePopup.ShowHeal(heal, pos, GetDamagePopupParent());
    }

    /// <summary>Shows a floating status/buff indicator near the enemy.</summary>
    public void ShowStatusNumber(string text, Color color)
    {
        Vector3 pos = GetEnemySpriteWorldPosition();
        DamagePopup.ShowBuff(text, pos, color, GetDamagePopupParent());
    }

    /// <summary>Shows a floating status/buff indicator near the player.</summary>
    public void ShowBuffOnPlayer(string text, Color color)
    {
        Vector3 pos = GetPlayerSpriteWorldPosition();
        DamagePopup.ShowBuff(text, pos, color, GetDamagePopupParent());
    }

    /// <summary>Shows a BREAK popup near the enemy.</summary>
    public void ShowBreakPopup()
    {
        Vector3 pos = GetEnemySpriteWorldPosition();
        DamagePopup.ShowBreak(pos, GetDamagePopupParent());
    }

    /// <summary>Full-screen white flash for impactful moments.</summary>
    public void ScreenFlash(float duration = 0.15f)
    {
        if (screenFlashImage == null)
        {
            // Create the flash image on demand
            EnsureScreenFlashImage();
        }
        if (screenFlashImage != null)
            StartCoroutine(ScreenFlashRoutine(screenFlashImage, duration));
    }

    private void EnsureScreenFlashImage()
    {
        if (screenFlashImage != null) return;
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null) canvas = FindObjectOfType<Canvas>();
        if (canvas == null) return;

        GameObject flashObj = new GameObject("Screen Flash Image", typeof(RectTransform), typeof(Image));
        flashObj.transform.SetParent(canvas.transform, false);
        RectTransform rt = flashObj.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        screenFlashImage = flashObj.GetComponent<Image>();
        screenFlashImage.color = Color.clear;
        screenFlashImage.raycastTarget = false;
    }

    private IEnumerator ScreenFlashRoutine(Image flashImg, float duration)
    {
        if (flashImg == null) yield break;
        flashImg.color = new Color(1f, 1f, 1f, 0.4f);
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float alpha = Mathf.Lerp(0.4f, 0f, elapsed / duration);
            flashImg.color = new Color(1f, 1f, 1f, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }
        flashImg.color = Color.clear;
    }

    private Transform GetDamagePopupParent()
    {
        if (cachedCanvasTransform == null)
        {
            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas == null) canvas = FindObjectOfType<Canvas>();
            if (canvas != null) cachedCanvasTransform = canvas.transform;
        }
        return cachedCanvasTransform;
    }

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
