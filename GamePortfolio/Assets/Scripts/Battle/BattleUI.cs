using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

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
    [SerializeField] private Sprite referencePlayerSprite;

    [Header("Enemy UI")]
    [SerializeField] private TMP_Text enemyHpText;
    [SerializeField] private Slider enemyHpSlider;
    [SerializeField] private TMP_Text enemyStatusText;
    [SerializeField] private TMP_Text enemyIntentText;
    [SerializeField] private TMP_Text enemyBreakText;
    [SerializeField] private Slider enemyBreakSlider;
    [SerializeField] private Image enemySpriteImage;
    [SerializeField] private Image enemyStandeeImage;
    [SerializeField] private Image heroStandeeImage;
    [SerializeField] private Image heroFormationFocusRing;
    [SerializeField] private Image enemyFormationTargetRing;
    [SerializeField] private Image[] enemyRosterMiniSprites;
    [SerializeField] private TMP_Text[] enemyRosterLabels;
    [SerializeField] private Sprite referenceEnemySprite;
    [SerializeField] private Sprite referenceGoblinSprite;
    [SerializeField] private Sprite referenceSkeletonSprite;
    [SerializeField] private Sprite referenceOrcSprite;
    [SerializeField] private Sprite referenceLichSprite;
    [SerializeField] private Sprite referenceGolemSprite;
    [SerializeField] private Sprite referenceDarkKnightSprite;
    [Header("Extracted 3v3 Battle Unit Sprites")]
    [SerializeField] private Sprite paladinBattleSprite;
    [SerializeField] private Sprite clericBattleSprite;
    [SerializeField] private Sprite rangerBattleSprite;
    [SerializeField] private Sprite goblinBattleSprite;
    [SerializeField] private Sprite skeletonBattleSprite;
    [SerializeField] private Sprite orcBattleSprite;
    [SerializeField] private Image burnOverlay;
    [SerializeField] private Image stunOverlay;
    [SerializeField] private Image brokenOverlay;

    [Header("3v3 Battlefield Slots")]
    [SerializeField] private Image[] allySlotBodies;
    [SerializeField] private Slider[] allySlotHpSliders;
    [SerializeField] private TMP_Text[] allySlotHpTexts;
    [SerializeField] private TMP_Text[] allySlotStatusTexts;
    [SerializeField] private Image[] allySlotStatusOverlays;
    [SerializeField] private Image[] allySlotIndicators;
    [SerializeField] private Button[] allySlotButtons;
    [SerializeField] private Image[] enemySlotBodies;
    [SerializeField] private Slider[] enemySlotHpSliders;
    [SerializeField] private TMP_Text[] enemySlotHpTexts;
    [SerializeField] private TMP_Text[] enemySlotStatusTexts;
    [SerializeField] private Image[] enemySlotStatusOverlays;
    [SerializeField] private Image[] enemySlotIndicators;
    [SerializeField] private Button[] enemySlotButtons;
    private BattleManager boundBattleManager;
    private bool skillDescriptionPinned;
    private UnityAction[] allySlotActions;
    private UnityAction[] enemySlotActions;

    [Header("Stage UI")]
    [SerializeField] private TMP_Text runStatusText;
    [SerializeField] private TMP_Text stageText;
    [SerializeField] private TMP_Text stageObjectiveText;
    [SerializeField] private TMP_Text stageProgressText;

    [Header("Message & Help")]
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private TMP_Text impactText;
    [SerializeField] private TMP_Text captureRehearsalText;
    [SerializeField] private TMP_Text skillHelpText;

    [Header("Command Preview")]
    [SerializeField] private GameObject commandPreviewPanel;
    [SerializeField] private TMP_Text commandPreviewText;

    [Header("Battle Log")]
    [SerializeField] private GameObject battleLogPanel;
    [SerializeField] private TMP_Text battleLogTitleText;
    [SerializeField] private TMP_Text battleLogText;
    [SerializeField] private Button battleLogToggleButton;
    [SerializeField] private TMP_Text battleLogToggleLabel;

    [Header("Result")]
    [SerializeField] private TMP_Text resultSummaryText;
    [SerializeField] private GameObject resultSummaryPanel;

    [Header("Character Selection Commands")]
    [SerializeField] private GameObject actionCommandPanel;
    [SerializeField] private GameObject skillSubmenuPanel;
    [SerializeField] private Button playerSelectButton;
    [SerializeField] private Image playerSelectionHighlight;
    [SerializeField] private TMP_Text selectedUnitText;
    [SerializeField] private TMP_Text skillDescriptionText;
    private bool playerUnitSelected;

    [Header("Buttons")]
    [SerializeField] private Button attackButton;
    [SerializeField] private Button skillMenuButton;
    [SerializeField] private Button fireSkillButton;
    [SerializeField] private Button iceSkillButton;
    [SerializeField] private Button lightningSkillButton;
    [SerializeField] private Button earthSkillButton;
    [SerializeField] private Button skillBackButton;
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
    private bool isBattleLogVisible;

    [Header("Pause Menu")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button quitButton;

    [Header("VFX")]
    [SerializeField] private Image screenFlashImage;
    [SerializeField] private GameObject turnBannerPanel;
    [SerializeField] private TMP_Text turnBannerText;
    private Canvas cachedCanvas;
    private Transform cachedCanvasTransform;
    private BattleSpriteMotion playerSpriteMotion;
    private BattleSpriteMotion enemySpriteMotion;
    private Coroutine guardPulseRoutine;
    private Coroutine burnPulseRoutine;
    private Coroutine stunPulseRoutine;
    private bool actionPresentationLocked;
    private string feedbackKind = "", feedbackPopup = "";
    private readonly List<GameObject> transientFeedbackObjects = new List<GameObject>();
    private readonly List<Coroutine> actionFeedbackCoroutines = new List<Coroutine>();
    private Coroutine feedbackLungeRoutine;
    private Coroutine feedbackTargetHitRoutine;
    private RectTransform feedbackActorRect;
    private RectTransform feedbackTargetRect;
    private Vector2 feedbackActorBasePosition;
    private Vector2 feedbackTargetBasePosition;
    private Color feedbackTargetBaseColor = Color.white;
    private TMP_Text[] enemyIntentLabels;
    private TMP_Text speedToggleLabel;
    private Image playerHpFillImage;
    private Image playerApFillImage;
    private Image enemyHpFillImage;

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
    public bool DebugBattleLogPanelVisible => battleLogPanel != null && battleLogPanel.activeSelf;
    public string DebugResultSummaryText => resultSummaryText != null ? resultSummaryText.text : "";
    public string DebugPlayerStatusText => playerStatusText != null ? playerStatusText.text : "";
    public string DebugEnemyStatusText => enemyStatusText != null ? enemyStatusText.text : "";
    public string DebugEnemyIntentText => enemyIntentText != null ? enemyIntentText.text : "";
    public string DebugEnemyBreakText => enemyBreakText != null ? enemyBreakText.text : "";
    public float DebugEnemyBreakBarValue => enemyBreakSlider != null ? enemyBreakSlider.value : -1f;
    public float DebugEnemyBreakBarMaxValue => enemyBreakSlider != null ? enemyBreakSlider.maxValue : -1f;
    public string DebugImpactText => impactText != null ? impactText.text : "";
    public string DebugCaptureRehearsalText => captureRehearsalText != null ? captureRehearsalText.text : "";
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
    public string DebugCommandPreviewText => commandPreviewText != null ? commandPreviewText.text : "";
    public bool DebugCommandPreviewPanelExists => commandPreviewPanel != null;
    public string DebugTurnBannerText => turnBannerText != null ? turnBannerText.text : "";
    public bool DebugTurnBannerPanelExists => turnBannerPanel != null;
    public bool DebugActionCommandPanelVisible => actionCommandPanel != null && actionCommandPanel.activeSelf;
    public bool DebugCommandDockVisible => actionCommandPanel != null && actionCommandPanel.activeSelf;
    public bool DebugBasicCommandsVisible => attackButton != null && skillMenuButton != null && guardButton != null && endTurnButton != null && attackButton.gameObject.activeSelf && skillMenuButton.gameObject.activeSelf && guardButton.gameObject.activeSelf && endTurnButton.gameObject.activeSelf;
    public bool DebugSkillSubmenuVisible => skillSubmenuPanel != null && skillSubmenuPanel.activeSelf;
    public string DebugActorSummaryText => selectedUnitText != null ? selectedUnitText.text : "";
    public string DebugBasicCommandLabels => $"{ButtonLabel(attackButton)}|{ButtonLabel(skillMenuButton)}|{ButtonLabel(guardButton)}|{ButtonLabel(endTurnButton)}";
    public string DebugSkillCommandLabels => $"{ButtonLabel(fireSkillButton).Replace("\n", "/")}|{ButtonLabel(iceSkillButton).Replace("\n", "/")}|{ButtonLabel(earthSkillButton).Replace("\n", "/")}|{ButtonLabel(lightningSkillButton).Replace("\n", "/")}|{ButtonLabel(skillBackButton)}";
    public int DebugSkillHoverTriggerCount => HasSkillHover(fireSkillButton) + HasSkillHover(iceSkillButton) + HasSkillHover(earthSkillButton) + HasSkillHover(lightningSkillButton);
    public bool DebugSkillDescriptionVisible => skillDescriptionText != null && skillDescriptionText.gameObject.activeSelf;
    public string DebugSkillDescriptionText => skillDescriptionText != null ? skillDescriptionText.text : "";
    public bool DebugFireSkillInteractable => fireSkillButton != null && fireSkillButton.interactable;
    public bool DebugIceSkillInteractable => iceSkillButton != null && iceSkillButton.interactable;
    public bool DebugEarthSkillInteractable => earthSkillButton != null && earthSkillButton.interactable;
    public bool DebugLightningSkillInteractable => lightningSkillButton != null && lightningSkillButton.interactable;
    public bool DebugAnyCommandInteractable => IsInteractable(attackButton) || IsInteractable(skillMenuButton) || IsInteractable(guardButton) || IsInteractable(endTurnButton) || IsInteractable(fireSkillButton) || IsInteractable(iceSkillButton) || IsInteractable(earthSkillButton) || IsInteractable(lightningSkillButton);
    public bool DebugActionPresentationLocked => actionPresentationLocked;
    public string DebugFeedbackKind => feedbackKind;
    public string DebugFeedbackPopup => feedbackPopup;
    public bool DebugHasCachedFeedbackTarget => feedbackTargetRect != null;
    public int DebugTransientFeedbackCount => transientFeedbackObjects.Count(item => item != null);
    public float DebugFeedbackActorOffset => feedbackActorRect != null ? Vector2.Distance(feedbackActorRect.anchoredPosition, feedbackActorBasePosition) : 0f;
    public bool DebugHasTransientFeedbackNamed(string objectName) => transientFeedbackObjects.Any(item => item != null && item.name == objectName);
    public bool DebugEnemyOverlayActive(int slot) => enemySlotStatusOverlays != null && slot >= 0 && slot < enemySlotStatusOverlays.Length && enemySlotStatusOverlays[slot] != null && enemySlotStatusOverlays[slot].gameObject.activeSelf;
    public string DebugEnemySlotIntent(int slot) => enemyIntentLabels != null && slot >= 0 && slot < enemyIntentLabels.Length && enemyIntentLabels[slot] != null ? enemyIntentLabels[slot].text : "";
    public float DebugEnemyIntentFontSize(int slot) => enemyIntentLabels != null && slot >= 0 && slot < enemyIntentLabels.Length && enemyIntentLabels[slot] != null ? enemyIntentLabels[slot].fontSize : 0f;
    public void DebugClickAttackButton() => InvokeIfInteractable(attackButton);
    public void DebugClickGuardButton() => InvokeIfInteractable(guardButton);
    public void DebugClickSkillMenuButton() => InvokeIfInteractable(skillMenuButton);
    public void DebugClickSkillBackButton() => InvokeIfInteractable(skillBackButton);
    public void DebugClickEndTurnButton() => InvokeIfInteractable(endTurnButton);
    public bool DebugClickIceSkillButton() => InvokeIfInteractable(iceSkillButton);
    public bool DebugClickEarthSkillButton() => InvokeIfInteractable(earthSkillButton);
    public bool DebugPlayerUnitSelected => playerUnitSelected;
    public string DebugSelectedUnitText => selectedUnitText != null ? selectedUnitText.text : "";
    public string DebugPlayerSpriteMotionProfile => GetSpriteMotionProfile(playerSpriteImage, true);
    public string DebugEnemySpriteMotionProfile => GetSpriteMotionProfile(enemySpriteImage, false);
    public string DebugEnemySpriteName => enemySpriteImage != null && enemySpriteImage.sprite != null ? enemySpriteImage.sprite.name : "";
    public string DebugEnemyStandeeSpriteName => enemyStandeeImage != null && enemyStandeeImage.sprite != null ? enemyStandeeImage.sprite.name : "";
    public string DebugEnemyRosterFirstSpriteName => enemyRosterMiniSprites != null && enemyRosterMiniSprites.Length > 0 && enemyRosterMiniSprites[0] != null && enemyRosterMiniSprites[0].sprite != null ? enemyRosterMiniSprites[0].sprite.name : "";
    public string DebugEnemyRosterFirstLabel => enemyRosterLabels != null && enemyRosterLabels.Length > 0 && enemyRosterLabels[0] != null ? enemyRosterLabels[0].text : "";
    public int DebugAllySlotCount => allySlotBodies != null ? allySlotBodies.Length : 0;
    public int DebugEnemySlotCount => enemySlotBodies != null ? enemySlotBodies.Length : 0;
    public string DebugAllySlotState(int index) => BuildSlotDebugState(allySlotHpTexts, allySlotStatusTexts, allySlotBodies, index);
    public string DebugEnemySlotState(int index) => BuildSlotDebugState(enemySlotHpTexts, enemySlotStatusTexts, enemySlotBodies, index);
    public bool DebugAllySlotSelected(int index) => IsIndicatorActive(allySlotIndicators, index);
    public bool DebugEnemySlotTargeted(int index) => IsIndicatorActive(enemySlotIndicators, index);
    public int DebugActiveAllyIndicatorCount => CountActiveIndicators(allySlotIndicators);
    public int DebugActiveEnemyIndicatorCount => CountActiveIndicators(enemySlotIndicators);
    public bool DebugAllySlotInteractable(int index) => IsButtonInteractable(allySlotButtons, index);
    public bool DebugEnemySlotInteractable(int index) => IsButtonInteractable(enemySlotButtons, index);
    public string DebugAllySlotSpriteName(int index) => GetSlotSpriteName(allySlotBodies, index);
    public string DebugEnemySlotSpriteName(int index) => GetSlotSpriteName(enemySlotBodies, index);
    // Full party/target state is intentionally exposed for headless battle logic verification.
    public string DebugPartyState { get; private set; } = "";
    public string DebugTargetState { get; private set; } = "";

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
        UnityEngine.Events.UnityAction onPause = null,
        UnityEngine.Events.UnityAction onPlayerSelect = null)
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

        WireButton(battleLogToggleButton, ToggleBattleLogVisibility);
        if (onPlayerSelect != null)
            WireButton(playerSelectButton, onPlayerSelect);
    }

    private static void WireButton(Button btn, UnityEngine.Events.UnityAction action)
    {
        if (btn == null || action == null) return;
        btn.onClick.RemoveListener(action);
        btn.onClick.AddListener(action);
    }

    private static string ButtonLabel(Button button)
    {
        TMP_Text label = button != null ? button.GetComponentInChildren<TMP_Text>(true) : null;
        return label != null ? label.text : "";
    }

    private static int HasSkillHover(Button button)
    {
        EventTrigger trigger = button != null ? button.GetComponent<EventTrigger>() : null;
        return trigger != null && trigger.triggers != null && trigger.triggers.Count >= 2 ? 1 : 0;
    }

    private static bool IsInteractable(Button button) => button != null && button.gameObject.activeInHierarchy && button.interactable;

    private static bool InvokeIfInteractable(Button button)
    {
        if (!IsInteractable(button)) return false;
        button.onClick.Invoke();
        return true;
    }

    private void ConfigureSkillHover(Button button, string description)
    {
        if (button == null) return;
        EventTrigger trigger = button.GetComponent<EventTrigger>();
        if (trigger == null) trigger = button.gameObject.AddComponent<EventTrigger>();
        trigger.triggers = new List<EventTrigger.Entry>();
        EventTrigger.Entry enter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        enter.callback.AddListener(_ => ShowSkillDescription(description, false));
        EventTrigger.Entry exit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        exit.callback.AddListener(_ => { if (!skillDescriptionPinned) HideSkillDescription(); });
        trigger.triggers.Add(enter);
        trigger.triggers.Add(exit);
    }

    private void ShowSkillDescription(string description, bool pin)
    {
        if (skillDescriptionText == null) return;
        string concise = description ?? "";
        if (concise.Length > 60) concise = concise.Substring(0, 60);
        skillDescriptionPinned = pin;
        skillDescriptionText.text = concise;
        skillDescriptionText.gameObject.SetActive(!string.IsNullOrEmpty(concise));
    }

    public void SetPendingSkillDescription(string description) => ShowSkillDescription(description, true);

    private void HideSkillDescription()
    {
        if (skillDescriptionText != null) skillDescriptionText.gameObject.SetActive(false);
    }

    private void ClearSkillDescription()
    {
        skillDescriptionPinned = false;
        HideSkillDescription();
    }

    /// <summary>Binds each serialized battlefield button to its exact party index.</summary>
    public void BindBattleManager(BattleManager manager)
    {
        if (boundBattleManager == manager) return;
        UnwireBattlefieldSlots();
        boundBattleManager = manager;
        if (manager == null) return;
        allySlotActions = WireVisualSlots(allySlotButtons, true, manager);
        enemySlotActions = WireVisualSlots(enemySlotButtons, false, manager);
        WireButton(attackButton, manager.OnClickAttackButton);
        WireButton(skillMenuButton, OpenSkillSubmenu);
        WireButton(guardButton, manager.OnClickGuardButton);
        WireButton(endTurnButton, manager.OnClickEndTurnButton);
        WireButton(fireSkillButton, manager.OnClickFireSkillButton);
        WireButton(iceSkillButton, manager.OnClickIceSkillButton);
        WireButton(lightningSkillButton, manager.OnClickLightningSkillButton);
        WireButton(earthSkillButton, manager.OnClickEarthSkillButton);
        WireButton(skillBackButton, CloseSkillSubmenu);
        ConfigureSkillHover(fireSkillButton, "Fire damage; applies Burn to the selected target.");
        ConfigureSkillHover(iceSkillButton, "Ice damage; applies Stun to the selected target.");
        ConfigureSkillHover(earthSkillButton, "Grants a damage shield to the selected actor.");
        ConfigureSkillHover(lightningSkillButton, "Heavy lightning damage to the selected target.");
    }

    private static UnityAction[] WireVisualSlots(Button[] buttons, bool isAlly, BattleManager manager)
    {
        if (buttons == null) return Array.Empty<UnityAction>();
        UnityAction[] actions = new UnityAction[buttons.Length];
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] == null) continue;
            int slotIndex = i;
            actions[i] = () =>
            {
                int partyIndex = FindUnitIndex(isAlly ? manager.playerParty : manager.enemyParty, GetSlotVisualId(isAlly, slotIndex));
                if (partyIndex >= 0)
                {
                    if (isAlly) manager.SelectPlayerUnit(partyIndex);
                    else manager.SelectEnemyTarget(partyIndex);
                }
            };
            buttons[i].onClick.AddListener(actions[i]);
        }
        return actions;
    }

    private void UnwireBattlefieldSlots()
    {
        UnwireIndexedSlots(allySlotButtons, allySlotActions);
        UnwireIndexedSlots(enemySlotButtons, enemySlotActions);
        allySlotActions = null;
        enemySlotActions = null;
    }

    private static void UnwireIndexedSlots(Button[] buttons, UnityAction[] actions)
    {
        if (buttons == null || actions == null) return;
        for (int i = 0; i < buttons.Length && i < actions.Length; i++)
            if (buttons[i] != null && actions[i] != null) buttons[i].onClick.RemoveListener(actions[i]);
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
        if (speedToggleLabel == null)
            speedToggleLabel = speedToggleButton.GetComponentInChildren<TMP_Text>();
        if (speedToggleLabel != null) speedToggleLabel.text = speedState >= 2 ? "2x" : "1x";
    }

    public void StartNewBattle()
    {
        battleLogEntries.Clear();
        battleLogSequence = 0;
        CacheResourceSliderFills();
        RefreshBattleLogText();
        SetBattleLogVisible(false);
        SetRetryButtonVisible(false);
        SetContinueButtonVisible(false);
        SetStageSelectButtonVisible(false);
        SetResultSummaryVisible(false, "");
        ClearCommandPreview();
        HideCharacterCommandMenu();
        ResetCaptureRehearsal();
        SetFormationFocus(false);
        // Cache continue button's child text component if not yet set
        if (continueButtonText == null && continueButton != null)
            continueButtonText = continueButton.GetComponentInChildren<TMP_Text>();
        // Apply element colors to skill buttons
        StyleSkillButtons();
    }

    /// <summary>Applies element-appropriate colors to skill buttons for visual hierarchy.</summary>
    public void StyleSkillButtons()
    {
        StyleContextCommandButton(attackButton, new Color(0.30f, 0.075f, 0.085f, 0.96f), "ATTACK");
        StyleContextCommandButton(skillMenuButton, new Color(0.085f, 0.105f, 0.16f, 0.96f), "SKILL");
        StyleContextCommandButton(guardButton, new Color(0.045f, 0.24f, 0.25f, 0.96f), "GUARD");
        StyleContextCommandButton(endTurnButton, new Color(0.11f, 0.13f, 0.18f, 0.96f), "END TURN");
        StyleContextCommandButton(fireSkillButton, new Color(0.34f, 0.065f, 0.075f, 0.96f), ButtonLabel(fireSkillButton));
        StyleContextCommandButton(iceSkillButton, new Color(0.27f, 0.075f, 0.10f, 0.96f), ButtonLabel(iceSkillButton));
        StyleContextCommandButton(lightningSkillButton, new Color(0.31f, 0.085f, 0.12f, 0.96f), ButtonLabel(lightningSkillButton));
        StyleContextCommandButton(earthSkillButton, new Color(0.035f, 0.28f, 0.27f, 0.96f), ButtonLabel(earthSkillButton));
        StyleContextCommandButton(skillBackButton, new Color(0.10f, 0.12f, 0.17f, 0.96f), "BACK");
        if (itemButton != null)
        {
            Image img = itemButton.GetComponent<Image>();
            if (img != null) img.color = new Color(0.10f, 0.22f, 0.16f, 0.92f);
            TMP_Text lbl = itemButton.GetComponentInChildren<TMP_Text>();
            if (lbl != null) lbl.text = "ITEMS";
        }
    }

    private static void StyleContextCommandButton(Button button, Color color, string exactLabel)
    {
        if (button == null) return;
        Image image = button.GetComponent<Image>();
        if (image != null) image.color = color;
        TMP_Text label = button.GetComponentInChildren<TMP_Text>(true);
        if (label == null) return;
        label.text = exactLabel;
        label.fontSize = 16f;
        RectTransform labelRect = label.rectTransform;
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.pivot = new Vector2(0.5f, 0.5f);
        labelRect.anchoredPosition = Vector2.zero;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;
        label.enableWordWrapping = true;
        label.overflowMode = TextOverflowModes.Overflow;
        label.color = new Color(0.94f, 0.96f, 1f);
    }

    public void SetContinueButtonLabel(string label)
    {
        if (continueButtonText != null) continueButtonText.text = label;
    }

    public void SetupPlaceholderSprites(ElementType enemyElement = ElementType.Fire, bool isBoss = false, EnemyVisualVariant visualVariant = EnemyVisualVariant.Goblin)
    {
        if (playerSpriteImage != null && playerSpriteImage.sprite == null)
            playerSpriteImage.sprite = referencePlayerSprite != null ? referencePlayerSprite : PlaceholderSpriteGenerator.CreateHeroSprite();
        Sprite enemyVisualSprite = SelectReferenceEnemySprite(visualVariant);
        if (enemySpriteImage != null && enemySpriteImage.sprite == null)
            enemySpriteImage.sprite = enemyVisualSprite != null ? enemyVisualSprite : PlaceholderSpriteGenerator.CreateEnemySprite(enemyElement, isBoss);
        ApplyEnemyVisualSprite(enemyVisualSprite, GetEnemyVisualLabel(visualVariant));
        EnsureSpriteMotions();
    }

    /// <summary>Clears cached sprite references so they can be regenerated on next battle.</summary>
    public void ClearCachedSprites()
    {
        if (playerSpriteImage != null) playerSpriteImage.sprite = null;
        if (enemySpriteImage != null) enemySpriteImage.sprite = null;
        if (enemyStandeeImage != null) enemyStandeeImage.sprite = null;
        if (enemyRosterMiniSprites != null)
        {
            for (int i = 0; i < enemyRosterMiniSprites.Length; i++)
            {
                if (enemyRosterMiniSprites[i] != null) enemyRosterMiniSprites[i].sprite = null;
            }
        }
    }


    public void SetEnemyVisuals(StageData stageData, ElementType fallbackElement = ElementType.Fire)
    {
        EnemyVisualVariant visualVariant = stageData != null && stageData.enemies != null && stageData.enemies.Count > 0
            ? stageData.enemies[0].visualVariant
            : EnemyVisualVariant.Goblin;
        Sprite visualSprite = SelectReferenceEnemySprite(visualVariant);
        if (visualSprite == null)
            visualSprite = PlaceholderSpriteGenerator.CreateEnemySprite(fallbackElement, visualVariant == EnemyVisualVariant.DarkKnight || visualVariant == EnemyVisualVariant.Golem || visualVariant == EnemyVisualVariant.Lich);

        if (enemySpriteImage != null && enemySpriteImage.sprite != visualSprite)
            enemySpriteImage.sprite = visualSprite;
        ApplyEnemyVisualSprite(visualSprite, GetEnemyVisualLabel(visualVariant));
        EnsureSpriteMotions();
    }

    private void ApplyEnemyVisualSprite(Sprite visualSprite, string visualLabel)
    {
        if (visualSprite != null && enemyStandeeImage != null && enemyStandeeImage.sprite != visualSprite)
            enemyStandeeImage.sprite = visualSprite;

        if (enemyRosterMiniSprites != null)
        {
            for (int i = 0; i < enemyRosterMiniSprites.Length; i++)
            {
                if (enemyRosterMiniSprites[i] != null && visualSprite != null)
                    enemyRosterMiniSprites[i].sprite = visualSprite;
            }
        }

        if (enemyRosterLabels != null && enemyRosterLabels.Length > 0 && enemyRosterLabels[0] != null)
            SetTextIfChanged(enemyRosterLabels[0], visualLabel);
    }

    private Sprite SelectReferenceEnemySprite(EnemyVisualVariant visualVariant)
    {
        Sprite selected = visualVariant switch
        {
            EnemyVisualVariant.Skeleton => referenceSkeletonSprite,
            EnemyVisualVariant.Orc => referenceOrcSprite,
            EnemyVisualVariant.Lich => referenceLichSprite,
            EnemyVisualVariant.Golem => referenceGolemSprite,
            EnemyVisualVariant.DarkKnight => referenceDarkKnightSprite,
            _ => referenceGoblinSprite
        };
        return selected != null ? selected : referenceEnemySprite;
    }

    private static string GetEnemyVisualLabel(EnemyVisualVariant visualVariant)
    {
        return visualVariant switch
        {
            EnemyVisualVariant.Skeleton => "Skeleton",
            EnemyVisualVariant.Orc => "Orc",
            EnemyVisualVariant.Lich => "Lich",
            EnemyVisualVariant.Golem => "Golem",
            EnemyVisualVariant.DarkKnight => "Dark Knight",
            _ => "Goblin"
        };
    }

    // --- Main Update ---

    /// <summary>Renders the complete 3v3 model; identity, HP, actor ring, and target ring live on battlefield slots.</summary>
    public void UpdatePartyUI(BattleState state, List<CharacterData> party, List<CharacterData> enemies, int selectedPlayerIndex, int selectedEnemyIndex, IReadOnlyCollection<int> actedMembers, string message, StageData stage, int enemyTurnCount, IDictionary<CharacterData, bool> guarding, IDictionary<CharacterData, int> shields, IReadOnlyList<string> enemyIntents = null, int activeEnemyIndex = -1, int warnedPlayerIndex = -1)
    {
        CharacterData selectedPlayer = GetUnit(party, selectedPlayerIndex) ?? GetFirstLiving(party);
        CharacterData selectedEnemy = GetUnit(enemies, selectedEnemyIndex) ?? GetFirstLiving(enemies);
        if (selectedPlayer == null || selectedEnemy == null) return;
        string playerName = selectedPlayer.characterName;
        string enemyName = selectedEnemy.characterName;
        bool isGuarding = guarding != null && guarding.TryGetValue(selectedPlayer, out bool guarded) && guarded;
        SetPlayerHp(selectedPlayer.currentHp, selectedPlayer.maxHp, playerName);
        SetPlayerAp(selectedPlayer.currentAp, selectedPlayer.maxAp);
        SetPlayerStatusText(state, isGuarding);
        SetPlayerShieldText(shields != null && shields.TryGetValue(selectedPlayer, out int shield) ? shield : 0);
        SetEnemyHp(selectedEnemy.currentHp, selectedEnemy.maxHp, enemyName);
        SetEnemyStatusText(selectedEnemy);
        SetEnemyBreakText(selectedEnemy);
        SetEnemyElementLabel(selectedEnemy.weaknessElement);
        SetEnemyIntentText(state, stage != null && stage.enemies.Count > 0 ? stage.enemies[Mathf.Clamp(selectedEnemyIndex, 0, stage.enemies.Count - 1)].pattern : new EnemyPatternData(), enemyTurnCount);
        SetRunStatusText(state, 0, new List<StageData> { stage });
        if (stage != null) { if (stageText != null) stageText.text = stage.BuildDisplayName(); if (stageObjectiveText != null) stageObjectiveText.text = stage.BuildObjectiveText(); if (stageProgressText != null) stageProgressText.text = "3v3"; SetEnemyVisuals(stage, selectedEnemy.weaknessElement); }
        SetMessageText(selectedEnemyIndex >= 0 ? $"Target: {enemyName}" : message);
        DebugPartyState = BuildPartyDebug("P", party, selectedPlayerIndex, actedMembers) + "|" + BuildPartyDebug("E", enemies, selectedEnemyIndex, null);
        DebugTargetState = $"actor={selectedPlayerIndex};target={selectedEnemyIndex};acted={string.Join(",", actedMembers ?? Array.Empty<int>())}";
        UpdateBattlefieldSlots(party, enemies, selectedPlayerIndex, selectedEnemyIndex, actedMembers, guarding, shields);
        UpdateEnemyIntentLabels(state, enemies, enemyIntents, activeEnemyIndex);
    }

    private void UpdateEnemyIntentLabels(BattleState state, List<CharacterData> enemies, IReadOnlyList<string> intents, int activeEnemyIndex)
    {
        EnsureEnemyIntentLabels();
        if (enemyIntentLabels == null) return;
        for (int slot = 0; slot < enemyIntentLabels.Length; slot++)
        {
            TMP_Text label = enemyIntentLabels[slot]; if (label == null) continue;
            int partyIndex = FindUnitIndex(enemies, GetSlotVisualId(false, slot));
            CharacterData enemy = partyIndex >= 0 ? enemies[partyIndex] : null;
            string intent = partyIndex >= 0 && intents != null && partyIndex < intents.Count ? intents[partyIndex] : "";
            bool visible = enemy != null && !enemy.IsDead() && !enemy.HasStatusEffect(StatusEffectType.Stun) && !string.IsNullOrEmpty(intent) && state != BattleState.Victory && state != BattleState.Defeat;
            label.text = visible ? intent : ""; label.gameObject.SetActive(visible);
            label.color = partyIndex == activeEnemyIndex ? new Color(1f, 0.58f, 0.24f) : new Color(1f, 0.82f, 0.48f);
        }
    }

    private void EnsureEnemyIntentLabels()
    {
        if (enemyIntentLabels != null && enemyIntentLabels.Length == 3 && enemyIntentLabels.All(item => item != null)) return;
        enemyIntentLabels = new TMP_Text[3];
        for (int slot = 0; slot < 3; slot++)
        {
            if (!HasSlotAt(enemySlotBodies, slot)) continue;
            Transform existing = enemySlotBodies[slot].transform.Find("Enemy Intent Label");
            TMP_Text label = existing != null ? existing.GetComponent<TMP_Text>() : null;
            if (label == null)
            {
                GameObject obj = new GameObject("Enemy Intent Label", typeof(RectTransform), typeof(TextMeshProUGUI)); obj.transform.SetParent(enemySlotBodies[slot].transform, false); label = obj.GetComponent<TextMeshProUGUI>();
            }
            RectTransform rect = label.rectTransform; rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f); rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(210f, 24f); rect.anchoredPosition = new Vector2(0f, -enemySlotBodies[slot].rectTransform.sizeDelta.y * 0.5f - 38f);
            label.fontSize = 14f; label.fontStyle = FontStyles.Bold; label.alignment = TextAlignmentOptions.Center; label.enableWordWrapping = false; label.overflowMode = TextOverflowModes.Overflow; label.raycastTarget = false;
            enemyIntentLabels[slot] = label;
        }
    }

    private void UpdateBattlefieldSlots(List<CharacterData> party, List<CharacterData> enemies, int selectedPlayerIndex, int selectedEnemyIndex, IReadOnlyCollection<int> actedMembers, IDictionary<CharacterData, bool> guarding, IDictionary<CharacterData, int> shields)
    {
        for (int i = 0; i < 3; i++)
        {
            BattleVisualId allyVisualId = GetSlotVisualId(true, i);
            int allyPartyIndex = FindUnitIndex(party, allyVisualId);
            CharacterData ally = allyPartyIndex >= 0 ? party[allyPartyIndex] : null;
            bool acted = actedMembers != null && actedMembers.Contains(allyPartyIndex);
            bool guarded = ally != null && guarding != null && guarding.TryGetValue(ally, out bool isGuarded) && isGuarded;
            int shield = ally != null && shields != null && shields.TryGetValue(ally, out int value) ? value : 0;
            UpdateBattlefieldSlot(allySlotBodies, allySlotHpSliders, allySlotHpTexts, allySlotStatusTexts, allySlotStatusOverlays, allySlotIndicators, allySlotButtons, i, ally, allyPartyIndex == selectedPlayerIndex, acted, guarded, shield, true);
            ApplyBattleSprite(allySlotBodies, i, allyVisualId);

            BattleVisualId enemyVisualId = GetSlotVisualId(false, i);
            int enemyPartyIndex = FindUnitIndex(enemies, enemyVisualId);
            CharacterData enemy = enemyPartyIndex >= 0 ? enemies[enemyPartyIndex] : null;
            UpdateBattlefieldSlot(enemySlotBodies, enemySlotHpSliders, enemySlotHpTexts, enemySlotStatusTexts, enemySlotStatusOverlays, enemySlotIndicators, enemySlotButtons, i, enemy, enemyPartyIndex == selectedEnemyIndex, false, false, 0, false);
            ApplyBattleSprite(enemySlotBodies, i, enemyVisualId);
        }

        ApplySelectedLegacyPortrait(playerSpriteImage, GetUnit(party, selectedPlayerIndex) ?? GetFirstLiving(party));
        ApplySelectedLegacyPortrait(enemySpriteImage, GetUnit(enemies, selectedEnemyIndex) ?? GetFirstLiving(enemies));
        ApplySelectedLegacyPortrait(enemyStandeeImage, GetUnit(enemies, selectedEnemyIndex) ?? GetFirstLiving(enemies));
        UpdateEnemyRosterSprites(enemies);
    }

    // Physical battlefield order is deliberate; runtime party order is not.
    private static BattleVisualId GetSlotVisualId(bool isAlly, int slotIndex)
    {
        BattleVisualId[] visuals = isAlly
            ? new[] { BattleVisualId.HeroPaladin, BattleVisualId.GuardianCleric, BattleVisualId.ScoutRanger }
            : new[] { BattleVisualId.Orc, BattleVisualId.Skeleton, BattleVisualId.Goblin };
        return visuals[Mathf.Clamp(slotIndex, 0, visuals.Length - 1)];
    }

    private static int FindUnitIndex(List<CharacterData> units, BattleVisualId visualId) => units == null ? -1 : units.FindIndex(unit => unit != null && unit.visualId == visualId);

    private void ApplyBattleSprite(Image[] bodies, int index, BattleVisualId visualId)
    {
        if (!HasSlotAt(bodies, index)) return;
        Sprite sprite = GetBattleSprite(visualId);
        if (sprite != null) bodies[index].sprite = sprite;
        bodies[index].preserveAspect = true;
    }

    private void ApplySelectedLegacyPortrait(Image image, CharacterData unit)
    {
        if (image == null || unit == null) return;
        Sprite sprite = GetBattleSprite(unit.visualId);
        if (sprite != null) image.sprite = sprite;
        image.preserveAspect = true;
    }

    private void UpdateEnemyRosterSprites(List<CharacterData> enemies)
    {
        if (enemyRosterMiniSprites == null) return;
        for (int i = 0; i < enemyRosterMiniSprites.Length; i++)
        {
            BattleVisualId visualId = GetSlotVisualId(false, i);
            Image portrait = enemyRosterMiniSprites[i];
            if (portrait != null) { portrait.sprite = GetBattleSprite(visualId); portrait.preserveAspect = true; }
            int partyIndex = FindUnitIndex(enemies, visualId);
            if (enemyRosterLabels != null && i < enemyRosterLabels.Length && enemyRosterLabels[i] != null && partyIndex >= 0)
                enemyRosterLabels[i].text = enemies[partyIndex].characterName;
        }
    }

    private Sprite GetBattleSprite(BattleVisualId visualId) => visualId switch
    {
        BattleVisualId.HeroPaladin => paladinBattleSprite,
        BattleVisualId.GuardianCleric => clericBattleSprite,
        BattleVisualId.ScoutRanger => rangerBattleSprite,
        BattleVisualId.Goblin => goblinBattleSprite,
        BattleVisualId.Skeleton => skeletonBattleSprite,
        BattleVisualId.Orc => orcBattleSprite,
        _ => null
    };

    private static void UpdateBattlefieldSlot(Image[] bodies, Slider[] hpSliders, TMP_Text[] hpTexts, TMP_Text[] statusTexts, Image[] overlays, Image[] indicators, Button[] buttons, int index, CharacterData unit, bool selected, bool acted, bool guarded, int shield, bool isAlly)
    {
        if (!HasSlotAt(bodies, index)) return;
        bool dead = unit == null || unit.IsDead();
        if (hpSliders != null && index < hpSliders.Length && hpSliders[index] != null)
        {
            hpSliders[index].minValue = 0f;
            hpSliders[index].maxValue = unit != null ? Mathf.Max(1, unit.maxHp) : 1f;
            hpSliders[index].value = unit != null ? unit.currentHp : 0f;
        }
        if (hpTexts != null && index < hpTexts.Length && hpTexts[index] != null)
        {
            hpTexts[index].text = unit == null ? "EMPTY" : $"{unit.characterName} {unit.currentHp}/{unit.maxHp}{(dead ? " DEAD" : string.Empty)}";
            hpTexts[index].fontSize = selected ? 8f : 7f;
        }
        string status = dead ? "DEAD" : unit.currentStatusEffect != StatusEffectType.None ? $"{unit.currentStatusEffect} ({unit.statusTurnsRemaining})" : guarded ? "GUARD" : shield > 0 ? "SHIELD" : acted ? "DONE" : "READY";
        if (statusTexts != null && index < statusTexts.Length && statusTexts[index] != null) statusTexts[index].text = status;
        if (bodies[index] != null) bodies[index].color = dead ? new Color(0.25f, 0.25f, 0.25f, 0.65f) : Color.white;
        if (overlays != null && index < overlays.Length && overlays[index] != null)
        {
            bool showOverlay = !dead && (unit.currentStatusEffect != StatusEffectType.None || guarded || shield > 0 || acted);
            overlays[index].gameObject.SetActive(showOverlay);
            overlays[index].color = unit != null && unit.currentStatusEffect == StatusEffectType.Burn ? new Color(1f, 0.25f, 0.08f, 0.06f) : unit != null && unit.currentStatusEffect == StatusEffectType.Stun ? new Color(0.25f, 0.55f, 1f, 0.06f) : guarded || shield > 0 ? new Color(0.20f, 0.72f, 1f, 0.05f) : new Color(0.65f, 0.65f, 0.65f, 0.04f);
        }
        if (indicators != null && index < indicators.Length && indicators[index] != null) indicators[index].gameObject.SetActive(selected && !dead);
        if (buttons != null && index < buttons.Length && buttons[index] != null) buttons[index].interactable = !dead && (!isAlly || !acted);
    }

    private static bool HasSlotAt(Image[] bodies, int index) => bodies != null && index >= 0 && index < bodies.Length && bodies[index] != null;
    private static string GetSlotSpriteName(Image[] bodies, int index) => HasSlotAt(bodies, index) && bodies[index].sprite != null ? bodies[index].sprite.name : "";
    private static string BuildSlotDebugState(TMP_Text[] hpTexts, TMP_Text[] statusTexts, Image[] bodies, int index)
    {
        if (!HasSlotAt(bodies, index)) return "";
        string hp = hpTexts != null && index < hpTexts.Length && hpTexts[index] != null ? hpTexts[index].text : "";
        string status = statusTexts != null && index < statusTexts.Length && statusTexts[index] != null ? statusTexts[index].text : "";
        return hp + "|" + status + "|visible=" + bodies[index].gameObject.activeInHierarchy;
    }
    private static bool IsIndicatorActive(Image[] indicators, int index) => indicators != null && index >= 0 && index < indicators.Length && indicators[index] != null && indicators[index].gameObject.activeSelf;
    private static int CountActiveIndicators(Image[] indicators)
    {
        if (indicators == null) return 0;
        int count = 0;
        foreach (Image indicator in indicators) if (indicator != null && indicator.gameObject.activeSelf) count++;
        return count;
    }
    private static bool IsButtonInteractable(Button[] buttons, int index) => buttons != null && index >= 0 && index < buttons.Length && buttons[index] != null && buttons[index].interactable;

    private static CharacterData GetUnit(List<CharacterData> units, int index) => units != null && index >= 0 && index < units.Count ? units[index] : null;
    private static CharacterData GetFirstLiving(List<CharacterData> units) => units == null ? null : units.Find(unit => !unit.IsDead());
    private static string BuildPartyDebug(string prefix, List<CharacterData> units, int selectedIndex, IReadOnlyCollection<int> acted)
    {
        if (units == null) return prefix + "[]";
        var entries = new List<string>();
        for (int i = 0; i < units.Count; i++) entries.Add($"{i}:{units[i].characterName}:{units[i].currentHp}:{(units[i].IsDead() ? "dead" : "alive")}{(i == selectedIndex ? ":selected" : "")}{(acted != null && acted.Contains(i) ? ":acted" : "")}");
        return prefix + "[" + string.Join(",", entries) + "]";
    }

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
        CacheResourceSliderFills();
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
        var currentStageData = GetStageData(currentStageIndex, stageEncounters);
        SetEnemyVisuals(currentStageData, enemy.weaknessElement);
        UpdateSkillHelpText(basicSkill, fireSkill, iceSkill, lightningSkill, earthSkill, guardReductionPercent, enemyPattern, currentStageData);
        AddBattleLogEntry(message, maxBattleLogEntries);
    }

    public void UpdateCommandDock(BattleState state, CharacterData actor, bool actorHasActed, SkillData basicSkill, SkillData fireSkill, SkillData iceSkill, SkillData lightningSkill, SkillData earthSkill)
    {
        bool available = state == BattleState.PlayerTurn && actor != null && !actor.IsDead() && !actorHasActed;
        playerUnitSelected = available;
        SetGameObjectActiveIfChanged(actionCommandPanel, available);
        SetGameObjectActiveIfChanged(selectedUnitText != null ? selectedUnitText.gameObject : null, available);
        if (!available)
        {
            CloseSkillSubmenu();
            SetActionButtonsInteractable(false);
            return;
        }

        SetTextIfChanged(selectedUnitText, $"{actor.characterName}  HP {actor.currentHp}/{actor.maxHp}  AP {actor.currentAp}/{actor.maxAp}");
        SetSkillButtonLabel(fireSkillButton, fireSkill);
        SetSkillButtonLabel(iceSkillButton, iceSkill);
        SetSkillButtonLabel(earthSkillButton, earthSkill);
        SetSkillButtonLabel(lightningSkillButton, lightningSkill);
        if (skillSubmenuPanel == null || !skillSubmenuPanel.activeSelf) SetActionCommandButtonsVisible(true);
        UpdateActionButtons(actor, basicSkill, fireSkill, iceSkill, lightningSkill, earthSkill, state);
    }

    public void OpenSkillSubmenu()
    {
        if (!playerUnitSelected || actionCommandPanel == null || !actionCommandPanel.activeSelf) return;
        SetActionCommandButtonsVisible(false);
        SetGameObjectActiveIfChanged(fireSkillButton != null ? fireSkillButton.gameObject : null, true);
        SetGameObjectActiveIfChanged(iceSkillButton != null ? iceSkillButton.gameObject : null, true);
        SetGameObjectActiveIfChanged(earthSkillButton != null ? earthSkillButton.gameObject : null, true);
        SetGameObjectActiveIfChanged(lightningSkillButton != null ? lightningSkillButton.gameObject : null, true);
        SetGameObjectActiveIfChanged(skillBackButton != null ? skillBackButton.gameObject : null, true);
        SetGameObjectActiveIfChanged(skillSubmenuPanel, true);
        ClearSkillDescription();
    }

    public void CloseSkillSubmenu()
    {
        ClearSkillDescription();
        SetGameObjectActiveIfChanged(skillSubmenuPanel, false);
        if (actionCommandPanel != null && actionCommandPanel.activeSelf) SetActionCommandButtonsVisible(true);
        else SetActionCommandButtonsVisible(false);
    }

    public void ShowCharacterCommandMenu(string unitName)
    {
        playerUnitSelected = true;
        SetGameObjectActiveIfChanged(actionCommandPanel, true);
        SetActionCommandButtonsVisible(true);
        if (playerSelectionHighlight != null)
            SetGameObjectActiveIfChanged(playerSelectionHighlight.gameObject, true);
        if (selectedUnitText != null)
            SetGameObjectActiveIfChanged(selectedUnitText.gameObject, true);
        SetTextIfChanged(selectedUnitText, string.IsNullOrWhiteSpace(unitName) ? "Selected: Paladin" : $"Selected: {unitName}");
        SetFormationFocus(true);
    }

    public void HideCharacterCommandMenu()
    {
        playerUnitSelected = false;
        SetGameObjectActiveIfChanged(actionCommandPanel, false);
        SetActionCommandButtonsVisible(false);
        if (playerSelectionHighlight != null)
            SetGameObjectActiveIfChanged(playerSelectionHighlight.gameObject, false);
        if (selectedUnitText != null)
            SetGameObjectActiveIfChanged(selectedUnitText.gameObject, false);
        SetTextIfChanged(selectedUnitText, "Click an ally to command");
        SetFormationFocus(false);
    }

    public void ResetCaptureRehearsal()
    {
        SetCaptureRehearsalText("SHOT 1/5");
    }

    public void MarkCaptureRehearsalHeroSelected()
    {
        SetCaptureRehearsalText("SHOT 2/5");
    }

    public void MarkCaptureRehearsalFireUsed()
    {
        SetCaptureRehearsalText("SHOT 3/5");
    }

    public void MarkCaptureRehearsalGuardUsed()
    {
        SetCaptureRehearsalText("SHOT 4/5");
    }

    public void MarkCaptureRehearsalResultShown()
    {
        SetCaptureRehearsalText("SHOT 5/5");
    }

    public void MarkCaptureRehearsalRetryDone()
    {
        SetCaptureRehearsalText("SHOT OK");
    }

    private void SetCaptureRehearsalText(string text)
    {
        SetTextIfChanged(captureRehearsalText, text);
    }


    private void SetActionCommandButtonsVisible(bool isVisible)
    {
        SetGameObjectActiveIfChanged(attackButton != null ? attackButton.gameObject : null, isVisible);
        SetGameObjectActiveIfChanged(skillMenuButton != null ? skillMenuButton.gameObject : null, isVisible);
        SetGameObjectActiveIfChanged(guardButton != null ? guardButton.gameObject : null, isVisible);
        SetGameObjectActiveIfChanged(endTurnButton != null ? endTurnButton.gameObject : null, isVisible);
        if (isVisible) SetGameObjectActiveIfChanged(skillSubmenuPanel, false);
        SetGameObjectActiveIfChanged(itemButton != null ? itemButton.gameObject : null, false);
    }

    public void SetActionButtonsInteractable(bool isInteractable)
    {
        SetButtonInteractable(attackButton, isInteractable);
        SetButtonInteractable(skillMenuButton, isInteractable);
        SetButtonInteractable(fireSkillButton, isInteractable);
        SetButtonInteractable(iceSkillButton, isInteractable);
        SetButtonInteractable(lightningSkillButton, isInteractable);
        SetButtonInteractable(earthSkillButton, isInteractable);
        SetButtonInteractable(endTurnButton, isInteractable);
        SetButtonInteractable(guardButton, isInteractable);
        SetButtonInteractable(itemButton, isInteractable);
    }

    public void SetActionPresentationLocked(bool locked)
    {
        actionPresentationLocked = locked;
        if (locked)
        {
            SetActionButtonsInteractable(false);
            SetStatusOverlaysVisible(allySlotStatusOverlays, false);
            SetStatusOverlaysVisible(enemySlotStatusOverlays, false);
        }
        SetSlotButtonsInteractable(allySlotButtons, !locked);
        SetSlotButtonsInteractable(enemySlotButtons, !locked);
    }

    private static void SetStatusOverlaysVisible(Image[] overlays, bool visible)
    {
        if (overlays == null) return;
        foreach (Image overlay in overlays) if (overlay != null) overlay.gameObject.SetActive(visible);
    }

    private static void SetSlotButtonsInteractable(Button[] buttons, bool interactable)
    {
        if (buttons == null) return;
        foreach (Button button in buttons) if (button != null) button.interactable = interactable;
    }

    public void UpdateActionButtons(CharacterData player, SkillData basicSkill, SkillData fireSkill, SkillData iceSkill, SkillData lightningSkill, SkillData earthSkill, BattleState currentState)
    {
        bool commandVisible = playerUnitSelected && currentState == BattleState.PlayerTurn;
        SetGameObjectActiveIfChanged(actionCommandPanel, commandVisible);
        if (!commandVisible)
        {
            SetActionButtonsInteractable(false);
            SetActionCommandButtonsVisible(false);
            return;
        }
        bool submenuOpen = skillSubmenuPanel != null && skillSubmenuPanel.activeSelf;
        SetActionCommandButtonsVisible(!submenuOpen);

        SetButtonInteractable(attackButton, true);
        SetButtonInteractable(skillMenuButton, true);
        SetButtonInteractable(fireSkillButton, player.HasEnoughAp(fireSkill.apCost) && ProgressState.IsSkillUnlocked(fireSkill.skillName));
        SetButtonInteractable(iceSkillButton, player.HasEnoughAp(iceSkill.apCost) && ProgressState.IsSkillUnlocked(iceSkill.skillName));
        SetButtonInteractable(lightningSkillButton, player.HasEnoughAp(lightningSkill.apCost) && ProgressState.IsSkillUnlocked(lightningSkill.skillName));
        SetButtonInteractable(earthSkillButton, player.HasEnoughAp(earthSkill.apCost) && ProgressState.IsSkillUnlocked(earthSkill.skillName));
        SetButtonInteractable(endTurnButton, currentState == BattleState.PlayerTurn);
        SetButtonInteractable(guardButton, currentState == BattleState.PlayerTurn);
        SetButtonInteractable(itemButton, currentState == BattleState.PlayerTurn);
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
        {
            resultSummaryPanel.SetActive(isVisible);
            if (isVisible)
            {
                // Animate slide-in from bottom
                RectTransform rt = resultSummaryPanel.GetComponent<RectTransform>();
                if (rt != null) StartCoroutine(SlideInResultPanel(rt));
            }
        }
        // Style the result panel background
        if (resultPanelBackground != null)
        {
            resultPanelBackground.gameObject.SetActive(isVisible);
            bool isVictory = IsVictorySummary(summary);
            resultPanelBackground.color = isVictory
                ? new Color(0.03f, 0.06f, 0.12f, 0.97f)  // deep navy for victory
                : new Color(0.12f, 0.03f, 0.03f, 0.97f);  // deep burgundy for defeat
        }
        // Style the result text for capture readability. The summary is eight compact lines,
        // so keep it small and top-left aligned instead of letting large centered text spill
        // over the characters in README screenshots.
        if (resultSummaryText != null && isVisible)
        {
            bool isVictory = IsVictorySummary(summary);
            resultSummaryText.color = isVictory
                ? new Color(1f, 0.88f, 0.48f)   // gold for victory
                : new Color(1f, 0.46f, 0.46f);    // red for defeat
            resultSummaryText.fontSize = 18;
            resultSummaryText.alignment = TMPro.TextAlignmentOptions.TopLeft;
            resultSummaryText.enableWordWrapping = true;
        }
    }

    private static bool IsVictorySummary(string summary)
    {
        return !string.IsNullOrEmpty(summary)
            && summary.IndexOf("Victory", System.StringComparison.OrdinalIgnoreCase) >= 0;
    }

    public void SetRetryButtonVisible(bool isVisible)
    {
        if (retryButton == null) return;
        retryButton.interactable = isVisible;
        SetGameObjectActiveIfChanged(retryButton.gameObject, isVisible);
    }

    public void SetContinueButtonVisible(bool isVisible)
    {
        if (continueButton == null) return;
        continueButton.interactable = isVisible;
        SetGameObjectActiveIfChanged(continueButton.gameObject, isVisible);
    }

    public void SetStageSelectButtonVisible(bool isVisible)
    {
        if (stageSelectButton == null) return;
        stageSelectButton.interactable = isVisible;
        SetGameObjectActiveIfChanged(stageSelectButton.gameObject, isVisible);
    }

    private static readonly Color ElementPhysicalColor = new Color(0.65f, 0.68f, 0.75f);
    private static readonly Color ElementFireColor = new Color(0.95f, 0.30f, 0.08f);
    private static readonly Color ElementIceColor = new Color(0.15f, 0.55f, 0.98f);
    private static readonly Color ElementLightningColor = new Color(1.0f, 0.82f, 0.10f);
    private static readonly Color ElementEarthColor = new Color(0.30f, 0.72f, 0.22f);
    private static readonly Color ElementGuardColor = new Color(0.25f, 0.60f, 0.95f);
    private static readonly Color ElementEndTurnColor = new Color(0.85f, 0.25f, 0.25f);

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

    // Element badge tags use ASCII so the default TMP font never renders missing-glyph boxes.
    private static readonly string[] ElementSymbols = { "", "PHY", "FIRE", "ICE", "LIT", "NAT", "DARK", "LIGHT" };
    public static string GetElementSymbol(ElementType element)
    {
        int idx = (int)element;
        return idx >= 0 && idx < ElementSymbols.Length ? ElementSymbols[idx] : "";
    }

    private void SetPlayerHp(int current, int max, string name)
    {
        SetTextIfChanged(playerHpText, BuildResourceText($"{name} HP", current, max));
        UpdateResourceSlider(playerHpSlider, current, max);
        SetSliderColorByRatio(playerHpFillImage, current, max, new Color(0.22f, 0.72f, 0.38f), new Color(0.85f, 0.72f, 0.18f), new Color(0.82f, 0.22f, 0.24f));
    }

    private void SetPlayerAp(int current, int max)
    {
        SetTextIfChanged(playerApText, BuildResourceText("AP", current, max));
        UpdateResourceSlider(playerApSlider, current, max);
        SetSliderColorByRatio(playerApFillImage, current, max, new Color(0.26f, 0.56f, 1.0f), new Color(0.26f, 0.86f, 0.76f), new Color(0.92f, 0.56f, 0.18f));
    }

    private void SetEnemyHp(int current, int max, string name)
    {
        SetTextIfChanged(enemyHpText, BuildResourceText($"{name} HP", current, max));
        UpdateResourceSlider(enemyHpSlider, current, max);
        SetSliderColorByRatio(enemyHpFillImage, current, max, new Color(0.22f, 0.72f, 0.38f), new Color(0.85f, 0.72f, 0.18f), new Color(0.82f, 0.22f, 0.24f));
    }

    private void SetPlayerStatusText(BattleState state, bool isGuarding)
    {
        if (playerStatusText == null) return;
        if (state == BattleState.Victory || state == BattleState.Defeat)
            SetTextIfChanged(playerStatusText, "Status: Battle ended");
        else if (isGuarding)
            SetTextIfChanged(playerStatusText, "Status: Guarding");
        else
            SetTextIfChanged(playerStatusText, "Status: Ready");

        // Guard overlay on player sprite
        UpdatePlayerGuardOverlay(isGuarding && state != BattleState.Victory && state != BattleState.Defeat);
    }

    private Image playerGuardOverlay;
    private void UpdatePlayerGuardOverlay(bool show)
    {
        if (playerSpriteImage == null) return;
        if (playerGuardOverlay == null)
        {
            Transform canvasTransform = GetDamagePopupParent();
            if (canvasTransform == null) return;
            GameObject overlayObj = new GameObject("Player Guard Overlay", typeof(RectTransform), typeof(Image));
            overlayObj.transform.SetParent(canvasTransform, false);
            RectTransform rt = overlayObj.GetComponent<RectTransform>();
            // Position near player sprite
            rt.anchorMin = new Vector2(0.0f, 0.85f);
            rt.anchorMax = new Vector2(0.0f, 0.85f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = new Vector2(80, 0);
            rt.sizeDelta = new Vector2(36, 36);
            playerGuardOverlay = overlayObj.GetComponent<Image>();
            playerGuardOverlay.sprite = null;
            playerGuardOverlay.color = new Color(0.3f, 0.7f, 1.0f, 0.7f);
        }
        SetGameObjectActiveIfChanged(playerGuardOverlay.gameObject, show);
        if (show && playerGuardOverlay.gameObject.activeInHierarchy)
            EnsurePulseRunning(ref guardPulseRoutine, playerGuardOverlay, 0.5f, new Color(0.3f, 0.7f, 1.0f, 0.3f));
        else
            StopPulse(ref guardPulseRoutine, playerGuardOverlay);
    }

    public void SetEnemyStatusText(CharacterData enemy)
    {
        if (enemyStatusText == null) return;
        SetTextIfChanged(enemyStatusText, enemy.currentStatusEffect == StatusEffectType.None
            ? "Status: None"
            : $"Status: {enemy.currentStatusEffect} ({enemy.statusTurnsRemaining} turns)");

        // Update status overlays
        bool hasBurn = enemy.currentStatusEffect == StatusEffectType.Burn;
        bool hasStun = enemy.currentStatusEffect == StatusEffectType.Stun;
        if (burnOverlay != null)
        {
            SetGameObjectActiveIfChanged(burnOverlay.gameObject, hasBurn);
            if (hasBurn)
                EnsurePulseRunning(ref burnPulseRoutine, burnOverlay, 0.5f, new Color(1f, 0.3f, 0.1f, 0.3f));
            else
                StopPulse(ref burnPulseRoutine, burnOverlay);
        }
        if (stunOverlay != null)
        {
            SetGameObjectActiveIfChanged(stunOverlay.gameObject, hasStun);
            if (hasStun)
                EnsurePulseRunning(ref stunPulseRoutine, stunOverlay, 0.3f, new Color(0.3f, 0.5f, 1f, 0.3f));
            else
                StopPulse(ref stunPulseRoutine, stunOverlay);
        }
    }

    private void SetEnemyBreakText(CharacterData enemy)
    {
        SetTextIfChanged(enemyBreakText, enemy.DebugBuildBreakText());
        if (enemyBreakSlider != null)
        {
            enemyBreakSlider.minValue = 0f;
            enemyBreakSlider.maxValue = enemy.maxBreakGauge;
            enemyBreakSlider.value = enemy.isBroken ? 0f : enemy.currentBreakGauge;
        }
        if (brokenOverlay != null)
            SetGameObjectActiveIfChanged(brokenOverlay.gameObject, enemy.isBroken);
    }

    private string enemyElementLabel = "";
    private TMP_Text enemyElementBadge;
    public void SetEnemyElementLabel(ElementType element)
    {
        if (element == ElementType.None || element == ElementType.Physical)
            enemyElementLabel = "";
        else
            enemyElementLabel = $"[{element}] ";

        // Enemy identity/status stays on its battlefield slot; do not recreate the removed right-side badge.
        if (enemyElementBadge != null)
            SetGameObjectActiveIfChanged(enemyElementBadge.gameObject, false);
    }

    private void UpdateEnemyElementBadge(ElementType element)
    {
        if (enemyElementBadge == null)
        {
            Transform canvasTransform = GetDamagePopupParent();
            if (canvasTransform == null) return;
            GameObject badgeObj = new GameObject("Enemy Element Badge", typeof(RectTransform), typeof(TextMeshProUGUI));
            badgeObj.transform.SetParent(canvasTransform, false);
            RectTransform rt = badgeObj.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.85f);
            rt.anchorMax = new Vector2(0.5f, 0.85f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = new Vector2(260, -10);
            rt.sizeDelta = new Vector2(100, 30);
            enemyElementBadge = badgeObj.GetComponent<TextMeshProUGUI>();
            enemyElementBadge.fontSize = 18;
            enemyElementBadge.alignment = TextAlignmentOptions.Center;
            enemyElementBadge.fontStyle = FontStyles.Bold;
            enemyElementBadge.raycastTarget = false;
            enemyElementBadge.enableWordWrapping = false;
            enemyElementBadge.overflowMode = TextOverflowModes.Ellipsis;
        }

        if (element == ElementType.None || element == ElementType.Physical)
        {
            SetGameObjectActiveIfChanged(enemyElementBadge.gameObject, false);
            return;
        }

        string icon = GetElementBadgeIcon(element);
        Color color = PlaceholderSpriteGenerator.GetElementColor(element);
        enemyElementBadge.text = $"{icon} {element}";
        enemyElementBadge.color = new Color(color.r * 0.9f + 0.3f, color.g * 0.9f + 0.3f, color.b * 0.9f + 0.3f);
        SetGameObjectActiveIfChanged(enemyElementBadge.gameObject, true);
    }

    private static string GetElementBadgeIcon(ElementType element) => element switch
    {
        ElementType.Fire => "FIRE",
        ElementType.Ice => "ICE",
        ElementType.Lightning => "LIT",
        ElementType.Nature => "NAT",
        ElementType.Earth => "EARTH",
        ElementType.Dark => "DARK",
        ElementType.Light => "LIGHT",
        _ => "ELEM"
    };

    private void SetEnemyIntentText(BattleState state, EnemyPatternData pattern, int turnCount)
    {
        if (enemyIntentText == null) return;
        if (state == BattleState.Victory || state == BattleState.Defeat)
        {
            SetTextIfChanged(enemyIntentText, "Next Enemy: Battle ended");
            return;
        }
        int nextTurn = turnCount + 1;
        SetTextIfChanged(enemyIntentText, pattern.IsStrongAttackTurn(nextTurn)
            ? $"Next Enemy: {enemyElementLabel}{pattern.strongAttackName} ({pattern.strongAttackDamage})"
            : $"Next Enemy: {enemyElementLabel}Normal Attack ({pattern.normalAttackDamage})");
    }

    private void SetRunStatusText(BattleState state, int stageIndex, List<StageData> encounters)
    {
        if (runStatusText == null) return;
        if (state == BattleState.Victory)
            runStatusText.text = HasNextStage(stageIndex, encounters)
                ? "Run: Clear -> Next"
                : "Run: Stage Clear";
        else if (state == BattleState.Defeat)
            runStatusText.text = "Run: Retry";
        else
            runStatusText.text = "Run: Active";
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
        if (current == null) { stageObjectiveText.text = "Goal: Unknown"; return; }

        if (state == BattleState.Victory)
        {
            if (HasNextStage(stageIndex, encounters))
            {
                var next = GetStageData(stageIndex + 1, encounters);
                string nextName = next != null ? next.BuildDisplayName() : "next encounter";
                stageObjectiveText.text = $"Clear: {current.BuildDisplayName()} -> {nextName}";
            }
            else
                stageObjectiveText.text = "Clear: Stage 1";
        }
        else if (state == BattleState.Defeat)
            stageObjectiveText.text = $"Retry: {current.BuildDisplayName()}";
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
            statusLabel = "Clear";
        else if (state == BattleState.Defeat)
            statusLabel = "Retry";
        stageProgressText.text = $"Enc {currentNum}/{count} | {statusLabel}";
    }

    private void SetMessageText(string message)
    {
        SetTextIfChanged(messageText, message);
    }

    public void SetImpactText(string text)
    {
        if (impactText != null)
        {
            impactText.text = text;
            // Color-code impact text based on content
            if (text.Contains("hazard") || text.Contains("Storm Surge") || text.Contains("Void Drain"))
                impactText.color = new Color(0.90f, 0.55f, 0.10f); // Orange for hazards
            else if (text.Contains("Guard") || text.Contains("guarded") || text.Contains("reduced"))
                impactText.color = new Color(0.30f, 0.70f, 1.0f);  // Blue for defense
            else if (text.Contains("FIRE") || text.Contains("Fire") || text.Contains("Burn"))
                impactText.color = new Color(1.0f, 0.35f, 0.15f);  // Red-orange for fire
            else if (text.Contains("ICE") || text.Contains("Ice") || text.Contains("Stun"))
                impactText.color = new Color(0.30f, 0.60f, 1.0f);  // Ice blue
            else if (text.Contains("LIT") || text.Contains("Lightning"))
                impactText.color = new Color(1.0f, 0.85f, 0.15f);  // Yellow-gold
            else if (text.Contains("Heal") || text.Contains("restore"))
                impactText.color = new Color(0.22f, 0.85f, 0.40f); // Green for healing
            else if (text.Contains("BROKEN") || text.Contains("Break bonus"))
                impactText.color = new Color(1.0f, 0.65f, 0.0f);   // Gold for break
            else if (text.Contains("Ready"))
                impactText.color = Color.white;
            else if (text.Contains("dealt") || text.Contains("damage"))
                impactText.color = new Color(0.92f, 0.28f, 0.28f); // Red for damage
            else
                impactText.color = Color.white;

            // Keep the visual top lane limited to stage, current turn, and queue.
            // The event string remains available to the debug API and battle log.
            impactText.color = Color.clear;
        }
    }

    /// <summary>Updates the Command Preview panel with skill info.</summary>
    public void UpdateCommandPreview(string text, Color? textColor = null)
    {
        SetGameObjectActiveIfChanged(commandPreviewPanel, true);
        // The current skill card owns the bottom focus area while it is displayed.
        // Keep the long reference help from competing with its final lines.
        if (skillHelpText != null)
            SetGameObjectActiveIfChanged(skillHelpText.gameObject, false);
        if (commandPreviewText != null)
        {
            SetGameObjectActiveIfChanged(commandPreviewText.gameObject, true);
            SetTextIfChanged(commandPreviewText, text);
            if (textColor.HasValue)
                commandPreviewText.color = textColor.Value;
            else
                commandPreviewText.color = new Color(0.92f, 0.88f, 0.82f);
        }
    }

    /// <summary>Hides the Command Preview panel.</summary>
    public void ClearCommandPreview()
    {
        SetGameObjectActiveIfChanged(commandPreviewPanel, false);
        if (commandPreviewText != null)
            SetGameObjectActiveIfChanged(commandPreviewText.gameObject, false);
        if (skillHelpText != null)
            SetGameObjectActiveIfChanged(skillHelpText.gameObject, false);
    }

    public void SetPlayerShieldText(int shieldAmount)
    {
        SetTextIfChanged(playerShieldText, shieldAmount > 0 ? $"Shield: {shieldAmount}" : "");
    }

    /// <summary>Brief flash effect on enemy sprite when damage is dealt.</summary>
    public void FlashEnemyDamage()
    {
        EnsureSpriteMotions();
        enemySpriteMotion?.PlayHitReaction();
        PlayStandeeMotion(heroStandeeImage, true);
        PlayStandeeMotion(enemyStandeeImage, false, true);
        PulseTargetRing(enemyFormationTargetRing, new Color(1f, 0.34f, 0.44f, 0.82f));
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
        return GetDamagePopupParent() ?? transform;
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
        EnsureSpriteMotions();
        playerSpriteMotion?.PlayHitReaction();
        PlayStandeeMotion(enemyStandeeImage, false);
        PlayStandeeMotion(heroStandeeImage, true, true);
        PulseTargetRing(heroFormationFocusRing, new Color(0.34f, 0.82f, 1f, 0.82f));
        if (playerSpriteImage != null)
            StartCoroutine(FlashRoutine(playerSpriteImage, Color.red, 0.15f));
    }

    private void SetFormationFocus(bool isFocused)
    {
        SetRingColor(heroFormationFocusRing, new Color(0.38f, 0.82f, 1f, isFocused ? 0.80f : 0.42f));
        SetRingColor(enemyFormationTargetRing, new Color(1f, 0.42f, 0.60f, isFocused ? 0.58f : 0.42f));
        if (isFocused)
            PlayStandeeMotion(heroStandeeImage, true);
    }

    private static void SetRingColor(Image ring, Color color)
    {
        if (ring != null)
            ring.color = color;
    }

    private void PulseTargetRing(Image ring, Color color)
    {
        if (ring != null)
            StartCoroutine(FlashRoutine(ring, color, 0.14f));
    }

    private static void PlayStandeeMotion(Image standee, bool towardRight, bool hitReaction = false)
    {
        BattleSpriteMotion motion = standee != null ? standee.GetComponent<BattleSpriteMotion>() : null;
        if (motion == null)
            return;
        if (hitReaction)
            motion.PlayHitReaction();
        else
            motion.PlayAttackLunge(towardRight);
    }

    private void EnsureSpriteMotions()
    {
        playerSpriteMotion = EnsureSpriteMotion(playerSpriteImage, playerSpriteMotion, 3.5f, 1.45f, 0f, 14f, 0.06f, false);
        enemySpriteMotion = EnsureSpriteMotion(enemySpriteImage, enemySpriteMotion, 4.5f, 1.25f, 0.35f, 18f, 0.08f, true);
    }

    private static BattleSpriteMotion EnsureSpriteMotion(Image image, BattleSpriteMotion cachedMotion, float bobPixels, float bobSpeed, float phaseOffset, float hitPixels, float squashAmount, bool moveLeftOnHit)
    {
        if (cachedMotion != null)
            return cachedMotion;
        if (image == null)
            return null;
        BattleSpriteMotion motion = image.GetComponent<BattleSpriteMotion>();
        if (motion == null)
            motion = image.gameObject.AddComponent<BattleSpriteMotion>();
        motion.Configure(bobPixels, bobSpeed, phaseOffset, hitPixels, squashAmount, moveLeftOnHit);
        return motion;
    }

    private static string GetSpriteMotionProfile(Image image, bool playerProfile)
    {
        BattleSpriteMotion motion = image != null ? image.GetComponent<BattleSpriteMotion>() : null;
        if (motion == null)
            return playerProfile
                ? BattleSpriteMotion.BuildDebugProfile(3.5f, 1.45f, 14f, 0.06f)
                : BattleSpriteMotion.BuildDebugProfile(4.5f, 1.25f, 18f, 0.08f);
        return motion.DebugProfile;
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

    private void EnsurePulseRunning(ref Coroutine routine, Image overlay, float speed, Color tint)
    {
        if (routine == null && overlay != null && overlay.gameObject.activeInHierarchy)
            routine = StartCoroutine(PulseOverlay(overlay, speed, tint));
    }

    private void StopPulse(ref Coroutine routine, Image overlay)
    {
        if (routine != null)
        {
            StopCoroutine(routine);
            routine = null;
        }
        if (overlay != null)
            overlay.color = Color.clear;
    }

    private void UpdateSkillHelpText(SkillData basicSkill, SkillData fireSkill, SkillData iceSkill, SkillData lightningSkill, SkillData earthSkill, int guardReduction, EnemyPatternData pattern, StageData stageData)
    {
        if (skillHelpText == null || basicSkill == null || fireSkill == null || iceSkill == null || lightningSkill == null || earthSkill == null) return;
        string attackHelp = BuildCompactSkillHelpLine(basicSkill);
        string fireHelp = BuildCompactSkillHelpLine(fireSkill);
        string iceHelp = BuildCompactSkillHelpLine(iceSkill);
        string lightningHelp = BuildCompactSkillHelpLine(lightningSkill);
        string earthHelp = BuildCompactSkillHelpLine(earthSkill);
        string turnHint = pattern?.BuildPatternHelpText() ?? "Read enemy intent, then spend AP.";
        string modifierHelp = BuildStageModifierHelpLine(stageData);
        SetTextIfChanged(skillHelpText, $"Commands: {attackHelp} | {fireHelp} | {iceHelp}\nMore: {lightningHelp} | {earthHelp} | Guard -{guardReduction}% damage\n{turnHint} / {modifierHelp}");
    }

    private string BuildStageModifierHelpLine(StageData stageData)
    {
        if (stageData == null || stageData.stageModifier == StageModifierType.None)
            return "Stage Modifier: None";
        string modifierName = StageData.GetModifierDisplayName(stageData.stageModifier);
        return string.IsNullOrWhiteSpace(stageData.stageModifierDescription)
            ? $"Stage Modifier: {modifierName}"
            : $"Stage Modifier: {modifierName} — {stageData.stageModifierDescription}";
    }

    private string BuildCompactSkillHelpLine(SkillData skill)
    {
        if (!ProgressState.IsSkillUnlocked(skill.skillName))
            return $"{skill.skillName}: locked";
        string effect = skill.HasStatusEffect() ? $", {skill.statusEffectType}" : "";
        return $"{skill.skillName}: {skill.power}p/{skill.apCost}AP{effect}";
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

    public void AddBattleLogEntry(string message, int maxEntries)
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

        // Keep player turn prompts and generic game flow messages intact
        if (lower.Contains("player turn"))
            return message;
        if (lower.Contains("skipped the turn"))
            return message;
        // Keep stage modifier activation messages intact
        if (lower.Contains("stage modifier"))
            return message;

        // Keep enemy attack messages intact for battle log readability
        if (lower.Contains("hero guards"))
            return message;
        if (lower.Contains("attacks") && lower.Contains("guards"))
            return message;
        if (lower.Contains("attacks") && lower.Contains("takes"))
            return message;

        if (lower.Contains("guards. next enemy attack damage is reduced"))
            return "GUARD: Damage reduced.";
        if (lower.Contains("guarded") || lower.Contains("guards"))
            return "GUARD: Guarded!";
        if (lower.Contains("break!"))
            return "BREAK!";
        if (lower.Contains("weakness"))
            return "WEAK: Weakness Hit!";
        if (lower.Contains("victory") || lower.Contains("victory!"))
            return "WIN: Victory!";
        if (lower.Contains("defeat") || lower.Contains("defeated"))
            return "LOSE: Defeated!";
        if (lower.Contains("is stunn"))
            return "STUN: Enemy skips turn.";
        if (lower.Contains("burn damage"))
            return "BURN: " + ExtractFirstNumber(message) + " dmg";
        if (lower.Contains("shield") && lower.Contains("active"))
            return "SHIELD: Active!";
        if (lower.Contains("enraged"))
            return "ENRAGE!";
        if (lower.Contains("level up"))
            return "LEVEL UP!";
        if (lower.Contains("is locked"))
            return "LOCKED: " + message;
        if (lower.Contains("not enough ap"))
            return "WARN: Not enough AP";
        if (lower.Contains("no items"))
            return "WARN: No items available";

        // Item effects: must come before generic "uses" check
        if (lower.Contains("restores") && lower.Contains("hp"))
            return "HP +" + ExtractFirstNumber(message);
        if (lower.Contains("restores") && lower.Contains("ap"))
            return "AP +" + ExtractFirstNumber(message);
        if (lower.Contains("uses shield") || lower.Contains("shield active"))
            return "SHIELD: Active!";

        // Generic skill use: "Hero uses Slash! Slime takes 22 damage."
        // Format as ASCII tags ("PHY 22 dmg", "FIRE 30 dmg") to avoid TMP missing-glyph boxes.
        if (lower.Contains(" uses "))
        {
            string formatted = ShortenSkillMessage(message);
            if (!string.IsNullOrEmpty(formatted)) return formatted;
        }

        // Enemy attack messages
        if (lower.Contains("takes") && lower.Contains("damage") && !lower.Contains("uses"))
        {
            int dmg = ExtractFirstNumber(message);
            if (dmg > 0) return $"DMG {dmg}";
        }

        // Item usage
        if (lower.Contains("restores") && lower.Contains("hp"))
            return "HP +" + ExtractFirstNumber(message);
        if (lower.Contains("restores") && lower.Contains("ap"))
            return "AP +" + ExtractFirstNumber(message);

        // Fallback: shorten to just the number
        return ShortenPlainMessage(message);
    }

    private string ShortenSkillMessage(string message)
    {
        // Pattern: "Hero uses Slash! Slime takes 22 damage. (Physical | Physical)"
        // We want: "PHY 22 dmg"
        int dmg = ExtractLastNumber(message);
        if (dmg <= 0) return null;

        string lower = message.ToLowerInvariant();
        if (lower.Contains("physical"))
            return $"PHY {dmg} dmg";
        if (lower.Contains("fire"))
            return $"FIRE {dmg} dmg";
        if (lower.Contains("ice"))
            return $"ICE {dmg} dmg";
        if (lower.Contains("lightning"))
            return $"LIT {dmg} dmg";
        if (lower.Contains("earth"))
            return $"EARTH {dmg} dmg";
        return $"• {dmg} dmg";
    }

    private string ShortenPlainMessage(string message)
    {
        // Just take the first meaningful part, strip long explanations
        int dmg = ExtractFirstNumber(message);
        if (dmg > 0) return $"DMG {dmg}";
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
        SetTextIfChanged(battleLogText, battleLogEntries.Count == 0
            ? "Recent Actions\nNo actions yet."
            : "Recent Actions\n" + string.Join("\n", battleLogEntries));
    }

    public void ToggleBattleLogVisibility()
    {
        SetBattleLogVisible(!isBattleLogVisible);
    }

    public void SetBattleLogVisible(bool isVisible)
    {
        isBattleLogVisible = isVisible;
        SetGameObjectActiveIfChanged(battleLogPanel, isVisible);
        if (battleLogTitleText != null)
            SetGameObjectActiveIfChanged(battleLogTitleText.gameObject, isVisible);
        if (battleLogText != null)
            SetGameObjectActiveIfChanged(battleLogText.gameObject, isVisible);
        if (battleLogToggleButton != null)
            battleLogToggleButton.interactable = true;
        if (battleLogToggleLabel == null && battleLogToggleButton != null)
            battleLogToggleLabel = battleLogToggleButton.GetComponentInChildren<TMP_Text>();
        if (battleLogToggleLabel != null)
            SetTextIfChanged(battleLogToggleLabel, isVisible ? "Hide Log" : "Log");
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

    public void BeginActionFeedback(string kind, BattleVisualId actorVisual, BattleVisualId targetVisual)
    {
        feedbackKind = kind ?? "";
        feedbackPopup = "";
        feedbackActorRect = GetSlotBodyRect(allySlotBodies, true, actorVisual);
        feedbackTargetRect = GetSlotBodyRect(enemySlotBodies, false, targetVisual);
        if (feedbackActorRect != null) feedbackActorBasePosition = feedbackActorRect.anchoredPosition;
        if (feedbackTargetRect != null)
        {
            feedbackTargetBasePosition = feedbackTargetRect.anchoredPosition;
            Image targetImage = feedbackTargetRect.GetComponent<Image>();
            feedbackTargetBaseColor = targetImage != null ? targetImage.color : Color.white;
        }
        if (!Application.isPlaying) return;
        if (feedbackActorRect != null)
        {
            if (kind == "Attack") feedbackLungeRoutine = TrackFeedbackCoroutine(ActionLungeRoutine(feedbackActorRect, 48f, 0.45f));
        }
        if ((kind == "Fire" || kind == "Ice") && feedbackActorRect != null && feedbackTargetRect != null)
        {
            Color projectileColor = kind == "Fire" ? new Color(1f, 0.28f, 0.08f, 0.95f) : new Color(0.25f, 0.75f, 1f, 0.95f);
            Image projectile = CreateFeedbackImage($"{kind} Projectile", new Vector2(18f, 18f), projectileColor, feedbackActorRect.position);
            if (projectile != null) TrackFeedbackCoroutine(ProjectileRoutine(projectile, feedbackActorRect.position, feedbackTargetRect.position, 0.22f));
        }
        else if (kind == "Lightning" && feedbackTargetRect != null)
        {
            Image bolt = CreateFeedbackImage("Lightning Flash", new Vector2(12f, 105f), new Color(0.9f, 0.9f, 1f, 0.82f), feedbackTargetRect.position + Vector3.up * 38f);
            if (bolt != null) TrackFeedbackCoroutine(FadeFeedbackRoutine(bolt, 0.28f));
        }
    }

    public void ShowActionImpact(string kind, int damage, string statusPopup)
    {
        feedbackKind = kind ?? "";
        feedbackPopup = !string.IsNullOrEmpty(statusPopup) ? statusPopup : damage > 0 ? $"-{damage}" : "";
        RectTransform targetRect = kind == "Guard" || kind == "Earth" ? feedbackActorRect : feedbackTargetRect;
        if (!Application.isPlaying || targetRect == null) return;
        if (kind == "Guard" || kind == "Earth")
        {
            Color pulseColor = kind == "Guard" ? new Color(0.18f, 0.85f, 0.78f, 0.62f) : new Color(0.48f, 0.62f, 0.32f, 0.68f);
            TMP_Text pulse = CreateFeedbackGlyphAt(targetRect, kind == "Guard" ? "Guard Pulse" : "Earth Wall Pulse", "O", 88f, pulseColor, Vector2.zero);
            if (pulse != null) TrackFeedbackCoroutine(PulseFeedbackRoutine(pulse, 0.48f));
            TMP_Text selfPopup = CreateFeedbackGlyphAt(targetRect, $"{kind} Popup", statusPopup, 28f, pulseColor, new Vector2(0f, 72f));
            if (selfPopup != null) TrackFeedbackCoroutine(PopupFeedbackRoutine(selfPopup, 0.8f));
            return;
        }
        feedbackTargetHitRoutine = TrackFeedbackCoroutine(TargetHitRoutine(targetRect, 0.16f));
        if (damage > 0)
        {
            TMP_Text damagePopup = CreateFeedbackGlyphAt(targetRect, "Damage Popup", $"-{damage}", 28f, new Color(1f, 0.88f, 0.82f), new Vector2(0f, 60f));
            if (damagePopup != null) TrackFeedbackCoroutine(PopupFeedbackRoutine(damagePopup, 0.8f));
        }
        if (!string.IsNullOrEmpty(statusPopup))
        {
            TMP_Text statusText = CreateFeedbackGlyphAt(targetRect, "Status Popup", statusPopup, 28f, kind == "Fire" ? new Color(1f, 0.34f, 0.12f) : new Color(0.35f, 0.8f, 1f), new Vector2(0f, 92f));
            if (statusText != null) TrackFeedbackCoroutine(PopupFeedbackRoutine(statusText, 0.8f));
        }
        Color burstColor = kind == "Fire" ? new Color(1f, 0.26f, 0.05f, 0.72f) : kind == "Ice" ? new Color(0.32f, 0.78f, 1f, 0.72f) : new Color(1f, 0.85f, 0.65f, 0.55f);
        TMP_Text burst = CreateFeedbackGlyphAt(targetRect, $"{kind} Impact Burst", "*", 58f, burstColor, Vector2.zero);
        if (burst != null) TrackFeedbackCoroutine(PulseFeedbackRoutine(burst, 0.32f));
    }

    public void BeginEnemyActionFeedback(BattleVisualId actorVisual, BattleVisualId targetVisual)
    {
        EndEnemyActionFeedback(); feedbackKind = "EnemyAttack"; feedbackPopup = "";
        feedbackActorRect = GetSlotBodyRect(enemySlotBodies, false, actorVisual);
        feedbackTargetRect = GetSlotBodyRect(allySlotBodies, true, targetVisual);
        if (feedbackActorRect != null) feedbackActorBasePosition = feedbackActorRect.anchoredPosition;
        if (feedbackTargetRect != null) { feedbackTargetBasePosition = feedbackTargetRect.anchoredPosition; Image image = feedbackTargetRect.GetComponent<Image>(); feedbackTargetBaseColor = image != null ? image.color : Color.white; }
        SetActionPresentationLocked(true);
        if (!Application.isPlaying) return;
        if (feedbackActorRect != null)
        {
            TMP_Text ring = CreateFeedbackGlyphAt(feedbackActorRect, "Enemy Actor Ring", "O", 92f, new Color(1f, 0.34f, 0.12f, 0.72f), Vector2.zero);
            if (ring != null) TrackFeedbackCoroutine(PulseFeedbackRoutine(ring, 0.70f));
            feedbackLungeRoutine = TrackFeedbackCoroutine(ActionLungeRoutine(feedbackActorRect, -42f, 0.62f));
        }
        if (feedbackTargetRect != null)
        {
            TMP_Text warning = CreateFeedbackGlyphAt(feedbackTargetRect, "Enemy Target Warning", "!", 54f, new Color(1f, 0.20f, 0.16f), new Vector2(0f, 72f));
            if (warning != null) TrackFeedbackCoroutine(PulseFeedbackRoutine(warning, 0.48f));
        }
    }

    public void ShowEnemyAttackImpact(int finalDamage, int absorbed, bool guarded)
    {
        feedbackPopup = absorbed > 0 ? $"BLOCK {absorbed}" : guarded ? "GUARD" : finalDamage > 0 ? $"-{finalDamage}" : "";
        if (!Application.isPlaying || feedbackTargetRect == null) return;
        feedbackTargetHitRoutine = TrackFeedbackCoroutine(TargetHitRoutine(feedbackTargetRect, 0.18f));
        if (finalDamage > 0)
        {
            TMP_Text damage = CreateFeedbackGlyphAt(feedbackTargetRect, "Enemy Damage Popup", $"-{finalDamage}", 30f, new Color(1f, 0.82f, 0.76f), new Vector2(0f, 58f));
            if (damage != null) TrackFeedbackCoroutine(PopupFeedbackRoutine(damage, 0.85f));
        }
        string defense = absorbed > 0 ? $"BLOCK {absorbed}" : guarded ? "GUARD" : "";
        if (!string.IsNullOrEmpty(defense))
        {
            TMP_Text block = CreateFeedbackGlyphAt(feedbackTargetRect, "Enemy Defense Popup", defense, 25f, new Color(0.30f, 0.88f, 1f), new Vector2(0f, 91f));
            if (block != null) TrackFeedbackCoroutine(PopupFeedbackRoutine(block, 0.85f));
        }
        TMP_Text burst = CreateFeedbackGlyphAt(feedbackTargetRect, "Enemy Impact Burst", "*", 62f, new Color(1f, 0.34f, 0.20f, 0.72f), Vector2.zero);
        if (burst != null) TrackFeedbackCoroutine(PulseFeedbackRoutine(burst, 0.34f));
    }

    public void ShowEnemyStatusPopup(BattleVisualId enemyVisual, string popup, Color color)
    {
        RectTransform body = GetSlotBodyRect(enemySlotBodies, false, enemyVisual); feedbackPopup = popup;
        if (!Application.isPlaying || body == null) return;
        TMP_Text text = CreateFeedbackGlyphAt(body, "Enemy Status Tick Popup", popup, 27f, color, new Vector2(0f, 72f));
        if (text != null) TrackFeedbackCoroutine(PopupFeedbackRoutine(text, 0.72f));
    }

    public void EndEnemyActionFeedback()
    {
        if (feedbackLungeRoutine != null) StopCoroutine(feedbackLungeRoutine);
        if (feedbackTargetHitRoutine != null) StopCoroutine(feedbackTargetHitRoutine);
        feedbackLungeRoutine = feedbackTargetHitRoutine = null; RestoreFeedbackActor(); RestoreFeedbackTargetTransform();
    }

    public void EndActionFeedback()
    {
        SetActionPresentationLocked(false);
        if (feedbackLungeRoutine != null) StopCoroutine(feedbackLungeRoutine);
        if (feedbackTargetHitRoutine != null) StopCoroutine(feedbackTargetHitRoutine);
        feedbackLungeRoutine = feedbackTargetHitRoutine = null;
        RestoreFeedbackActor();
        RestoreFeedbackTargetTransform();
    }

    public void CleanupActionFeedback()
    {
        SetActionPresentationLocked(false);
        foreach (Coroutine routine in actionFeedbackCoroutines) if (routine != null) StopCoroutine(routine);
        actionFeedbackCoroutines.Clear();
        feedbackLungeRoutine = feedbackTargetHitRoutine = null;
        RestoreFeedbackActor();
        RestoreFeedbackTargetTransform();
        feedbackTargetRect = null;
        foreach (GameObject item in transientFeedbackObjects) if (item != null) Destroy(item);
        transientFeedbackObjects.Clear();
        feedbackKind = feedbackPopup = "";
    }

    private RectTransform GetSlotBodyRect(Image[] bodies, bool ally, BattleVisualId visual)
    {
        if (bodies == null) return null;
        for (int i = 0; i < bodies.Length; i++) if (GetSlotVisualId(ally, i) == visual && bodies[i] != null) return bodies[i].rectTransform;
        return null;
    }

    private Coroutine TrackFeedbackCoroutine(IEnumerator routine)
    {
        if (routine == null || !Application.isPlaying) return null;
        Coroutine coroutine = StartCoroutine(routine);
        actionFeedbackCoroutines.Add(coroutine);
        return coroutine;
    }

    private Image CreateFeedbackImage(string objectName, Vector2 size, Color color, Vector3 position)
    {
        Transform parent = GetDamagePopupParent();
        if (parent == null) return null;
        GameObject obj = new GameObject(objectName, typeof(RectTransform), typeof(Image));
        obj.transform.SetParent(parent, false);
        RectTransform rect = obj.GetComponent<RectTransform>(); rect.sizeDelta = size; SetFeedbackPosition(rect, position);
        Image image = obj.GetComponent<Image>(); image.color = color; image.raycastTarget = false;
        transientFeedbackObjects.Add(obj); return image;
    }

    private void SetFeedbackPosition(RectTransform rect, Vector3 worldPosition)
    {
        EnsureCanvasCached();
        RectTransform canvasRect = cachedCanvasTransform as RectTransform;
        if (rect == null || canvasRect == null) return;
        Camera camera = cachedCanvas != null && cachedCanvas.renderMode != RenderMode.ScreenSpaceOverlay ? cachedCanvas.worldCamera : null;
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(null, worldPosition);
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, camera, out Vector2 localPoint)) rect.anchoredPosition = localPoint;
    }

    private TMP_Text CreateFeedbackGlyphAt(RectTransform anchor, string objectName, string glyph, float fontSize, Color color, Vector2 localOffset)
    {
        if (anchor == null) return null;
        GameObject obj = new GameObject(objectName, typeof(RectTransform), typeof(TextMeshProUGUI));
        obj.transform.SetParent(anchor, false);
        RectTransform rect = obj.GetComponent<RectTransform>(); rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f); rect.sizeDelta = new Vector2(180f, 120f); rect.anchoredPosition = localOffset;
        TMP_Text text = obj.GetComponent<TextMeshProUGUI>(); text.text = glyph; text.fontSize = fontSize; text.color = color; text.alignment = TextAlignmentOptions.Center; text.raycastTarget = false;
        transientFeedbackObjects.Add(obj); return text;
    }

    private IEnumerator ActionLungeRoutine(RectTransform actor, float pixels, float duration)
    {
        float elapsed = 0f;
        while (actor != null && elapsed < duration)
        {
            float t = elapsed / duration; actor.anchoredPosition = feedbackActorBasePosition + Vector2.right * (Mathf.Sin(t * Mathf.PI) * pixels);
            elapsed += Time.deltaTime; yield return null;
        }
        if (actor != null) actor.anchoredPosition = feedbackActorBasePosition;
    }

    private IEnumerator ProjectileRoutine(Image image, Vector3 start, Vector3 end, float duration)
    {
        float elapsed = 0f;
        while (image != null && elapsed < duration) { SetFeedbackPosition(image.rectTransform, Vector3.Lerp(start, end, elapsed / duration)); elapsed += Time.deltaTime; yield return null; }
        if (image != null)
        {
            SetFeedbackPosition(image.rectTransform, end);
            yield return new WaitForSeconds(0.25f);
            transientFeedbackObjects.Remove(image.gameObject); Destroy(image.gameObject);
        }
    }

    private IEnumerator TargetHitRoutine(RectTransform target, float duration)
    {
        Vector2 origin = target.anchoredPosition; Image image = target.GetComponent<Image>(); Color original = image != null ? image.color : Color.white;
        float elapsed = 0f;
        while (target != null && elapsed < duration) { float shake = Mathf.Sin(elapsed * 120f) * 4f; target.anchoredPosition = origin + Vector2.right * shake; if (image != null) image.color = Color.Lerp(original, new Color(1f, 0.72f, 0.72f), 0.65f); elapsed += Time.deltaTime; yield return null; }
        if (target != null) target.anchoredPosition = origin; if (image != null) image.color = original;
    }

    private IEnumerator PulseFeedbackRoutine(Graphic graphic, float duration)
    {
        float elapsed = 0f; Color color = graphic.color;
        while (graphic != null && elapsed < duration) { float t = elapsed / duration; graphic.rectTransform.localScale = Vector3.one * Mathf.Lerp(0.55f, 1.25f, t); graphic.color = new Color(color.r, color.g, color.b, Mathf.Lerp(color.a, 0f, t)); elapsed += Time.deltaTime; yield return null; }
        if (graphic != null) { transientFeedbackObjects.Remove(graphic.gameObject); Destroy(graphic.gameObject); }
    }

    private IEnumerator PopupFeedbackRoutine(TMP_Text text, float duration)
    {
        float elapsed = 0f; Color color = text.color; Vector2 origin = text.rectTransform.anchoredPosition;
        while (text != null && elapsed < duration)
        {
            float t = elapsed / duration; text.rectTransform.anchoredPosition = origin + Vector2.up * (24f * t); text.color = new Color(color.r, color.g, color.b, t < 0.55f ? color.a : Mathf.Lerp(color.a, 0f, (t - 0.55f) / 0.45f)); elapsed += Time.deltaTime; yield return null;
        }
        if (text != null) { transientFeedbackObjects.Remove(text.gameObject); Destroy(text.gameObject); }
    }

    private IEnumerator FadeFeedbackRoutine(Image image, float duration)
    {
        float elapsed = 0f; Color color = image.color;
        while (image != null && elapsed < duration) { float t = elapsed / duration; image.color = new Color(color.r, color.g, color.b, Mathf.Lerp(color.a, 0f, t)); elapsed += Time.deltaTime; yield return null; }
        if (image != null) { transientFeedbackObjects.Remove(image.gameObject); Destroy(image.gameObject); }
    }

    private void RestoreFeedbackTargetTransform()
    {
        if (feedbackTargetRect == null) return;
        feedbackTargetRect.anchoredPosition = feedbackTargetBasePosition;
        Image targetImage = feedbackTargetRect.GetComponent<Image>();
        if (targetImage != null) targetImage.color = feedbackTargetBaseColor;
    }

    private void RestoreFeedbackActor()
    {
        if (feedbackActorRect != null) feedbackActorRect.anchoredPosition = feedbackActorBasePosition;
        feedbackActorRect = null;
    }

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

    /// <summary>Shows a large turn banner (e.g. PLAYER TURN, ENEMY TURN, VICTORY) with fade animation.</summary>
    public void ShowTurnBanner(string text, Color? textColor = null, float holdDuration = 1.2f)
    {
        if (turnBannerPanel == null || turnBannerText == null) return;
        turnBannerText.text = text;
        turnBannerText.color = textColor ?? Color.white;
        turnBannerPanel.SetActive(true);
        // Scale/bounce animation
        RectTransform rt = turnBannerPanel.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(320f, 44f);
            rt.anchoredPosition = new Vector2(0f, 246f);
            if (turnBannerText != null) { turnBannerText.fontSize = 22f; turnBannerText.enableWordWrapping = false; }
            rt.localScale = Vector3.one * 1.12f;
            StartCoroutine(TurnBannerRoutine(rt, turnBannerPanel, holdDuration));
        }
    }

    private IEnumerator TurnBannerRoutine(RectTransform rt, GameObject panel, float holdDuration)
    {
        // Scale in (bounce)
        float elapsed = 0f;
        float scaleIn = 0.15f;
        while (elapsed < scaleIn)
        {
            float t = elapsed / scaleIn;
            rt.localScale = Vector3.Lerp(Vector3.one * 1.3f, Vector3.one, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        rt.localScale = Vector3.one;

        // Hold
        yield return new WaitForSeconds(holdDuration);

        // Fade out
        elapsed = 0f;
        float fadeOut = 0.25f;
        CanvasGroup cg = panel.GetComponent<CanvasGroup>();
        if (cg == null) cg = panel.AddComponent<CanvasGroup>();
        while (elapsed < fadeOut)
        {
            cg.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeOut);
            elapsed += Time.deltaTime;
            yield return null;
        }
        panel.SetActive(false);
        cg.alpha = 1f;
    }

    /// <summary>Immediately hides the turn banner.</summary>
    public void HideTurnBanner()
    {
        if (turnBannerPanel != null)
            turnBannerPanel.SetActive(false);
    }

    private void EnsureScreenFlashImage()
    {
        if (screenFlashImage != null) return;
        EnsureCanvasCached();
        if (cachedCanvas == null) return;

        GameObject flashObj = new GameObject("Screen Flash Image", typeof(RectTransform), typeof(Image));
        flashObj.transform.SetParent(cachedCanvas.transform, false);
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

    /// <summary>Animates the result panel sliding in from below.</summary>
    private IEnumerator SlideInResultPanel(RectTransform panelRt)
    {
        if (panelRt == null) yield break;
        Vector2 startPos = panelRt.anchoredPosition;
        Vector2 offScreen = new Vector2(startPos.x, startPos.y - 60f);
        panelRt.anchoredPosition = offScreen;
        float duration = 0.25f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            panelRt.anchoredPosition = Vector2.Lerp(offScreen, startPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        panelRt.anchoredPosition = startPos;
    }

    private Transform GetDamagePopupParent()
    {
        EnsureCanvasCached();
        return cachedCanvasTransform;
    }

    private void EnsureCanvasCached()
    {
        if (cachedCanvas != null && cachedCanvasTransform != null) return;
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null) canvas = FindObjectOfType<Canvas>();
        cachedCanvas = canvas;
        cachedCanvasTransform = canvas != null ? canvas.transform : null;
    }

    private void CacheResourceSliderFills()
    {
        if (playerHpFillImage == null)
            playerHpFillImage = playerHpSlider != null ? playerHpSlider.fillRect?.GetComponent<Image>() : null;
        if (playerApFillImage == null)
            playerApFillImage = playerApSlider != null ? playerApSlider.fillRect?.GetComponent<Image>() : null;
        if (enemyHpFillImage == null)
            enemyHpFillImage = enemyHpSlider != null ? enemyHpSlider.fillRect?.GetComponent<Image>() : null;
    }

    private static string BuildResourceText(string label, int currentValue, int maxValue)
    {
        int pct = maxValue > 0 ? Mathf.RoundToInt((float)currentValue / maxValue * 100f) : 0;
        return $"{label}: {currentValue}/{maxValue} ({pct}%)";
    }

    private static void SetSkillButtonLabel(Button button, SkillData skill)
    {
        if (button == null || skill == null) return;
        TMP_Text label = button.GetComponentInChildren<TMP_Text>(true);
        SetTextIfChanged(label, $"{skill.skillName}\nAP {skill.apCost}");
    }

    private static void SetTextIfChanged(TMP_Text target, string value)
    {
        if (target != null && target.text != value)
        {
            target.text = value;
        }
    }

    private static void SetGameObjectActiveIfChanged(GameObject target, bool isActive)
    {
        if (target != null && target.activeSelf != isActive)
        {
            target.SetActive(isActive);
        }
    }

    private static void SetSliderColorByRatio(Image fill, int current, int max, Color highColor, Color midColor, Color lowColor)
    {
        if (fill == null) return;
        float ratio = max > 0 ? (float)current / max : 0f;
        Color nextColor;
        if (ratio > 0.55f)
            nextColor = highColor;
        else if (ratio > 0.25f)
            nextColor = midColor;
        else
            nextColor = lowColor;

        if (fill.color != nextColor)
        {
            fill.color = nextColor;
        }
    }

    private static void UpdateResourceSlider(Slider slider, int currentValue, int maxValue)
    {
        if (slider == null) return;
        float nextValue = Mathf.Clamp(currentValue, 0, maxValue);
        if (slider.minValue != 0f)
            slider.minValue = 0f;
        if (slider.maxValue != maxValue)
            slider.maxValue = maxValue;
        if (slider.value != nextValue)
            slider.value = nextValue;
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
