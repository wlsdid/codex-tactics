using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Core battle state machine and turn flow.
/// UI rendering is handled by BattleUI. Data loading by StageData helpers.
/// </summary>
public class BattleManager : MonoBehaviour
{
    [Header("Battle State")]
    [SerializeField] private BattleState currentState;

    [Header("Balance Config (optional)")]
    [SerializeField] private BattleBalanceConfig balanceConfig;

    [Header("Player")]
    [SerializeField] private string playerName = "Hero";

    [Header("Skills")]
    [SerializeField] private string basicSkillName = "Slash";
    [SerializeField] private string fireSkillName = "Fire Bolt";
    [SerializeField] private string iceSkillName = "Ice Lance";
    [SerializeField] private string lightningSkillName = "Lightning Strike";
    [SerializeField] private string earthSkillName = "Earth Wall";

    [Header("Stage Encounters")]
    [SerializeField] private List<StageData> stageEncounters = new List<StageData>();

    [Header("UI (assigned via Inspector)")]
    [SerializeField] private BattleUI battleUI;

    [NonSerialized] private int currentStageIndex;

    // Runtime state (overwritten by stage data)
    private string enemyName = "Slime";
    private int enemyMaxHp = 80;
    private ElementType enemyWeakness = ElementType.Fire;
    private EnemyPatternData enemyPattern = new EnemyPatternData();

    private CharacterData player;
    private CharacterData enemy;
    private SkillData basicAttackSkill;
    private SkillData fireSkill;
    private SkillData iceSkill;
    private SkillData lightningSkill;
    private SkillData earthSkill;
    private bool playerIsGuarding;
    private int enemyTurnCount;
    private int speedState = 1;
    private bool autoBattleEnabled;
    private ScreenFade screenFade;
    private ScreenShake cachedShake;
    public bool DebugAutoBattleEnabled => autoBattleEnabled;
    public int DebugPlayerLevel => playerLevel;
    public int DebugPlayerXp => playerXp; // 1=1x, 2=2x
    private int totalDamageDealt;
    private int totalDamageTaken;
    private int guardUseCount;
    private int skillsUsedCount;
    private int totalGoldEarned;
    private int playerShieldAmount;
    private List<ItemData> playerItems;
    private int selectedItemIndex = -1;
    private int playerLevel = 1;
    private int playerXp;
    private int xpToNextLevel = 100;
    private bool currentBattleRewardClaimed;
    private readonly HashSet<int> rewardedStageIndexes = new HashSet<int>();
    private readonly HashSet<int> bonusRewardedStageIndexes = new HashSet<int>();
    private bool loadedProgressState;

    // Bonus tracking
    private readonly HashSet<string> skillsUsedNames = new HashSet<string>();
    private bool guardedStrongAttack;
    private bool usedItems;

    // Enrage state
    private bool isEnraged;

    // --- Debug pass-throughs (for auto-tester) ---
    public string DebugPlayerHpText => battleUI != null ? battleUI.DebugPlayerHpText : "";
    public string DebugPlayerApText => battleUI != null ? battleUI.DebugPlayerApText : "";
    public string DebugEnemyHpText => battleUI != null ? battleUI.DebugEnemyHpText : "";
    public float DebugPlayerHpBarValue => battleUI != null ? battleUI.DebugPlayerHpBarValue : -1f;
    public float DebugPlayerHpBarMaxValue => battleUI != null ? battleUI.DebugPlayerHpBarMaxValue : -1f;
    public float DebugPlayerApBarValue => battleUI != null ? battleUI.DebugPlayerApBarValue : -1f;
    public float DebugPlayerApBarMaxValue => battleUI != null ? battleUI.DebugPlayerApBarMaxValue : -1f;
    public float DebugEnemyHpBarValue => battleUI != null ? battleUI.DebugEnemyHpBarValue : -1f;
    public float DebugEnemyHpBarMaxValue => battleUI != null ? battleUI.DebugEnemyHpBarMaxValue : -1f;
    public string DebugMessageText => battleUI != null ? battleUI.DebugMessageText : "";
    public string DebugSkillHelpText => battleUI != null ? battleUI.DebugSkillHelpText : "";
    public string DebugBattleLogText => battleUI != null ? battleUI.DebugBattleLogText : "";
    public string DebugResultSummaryText => battleUI != null ? battleUI.DebugResultSummaryText : "";
    public string DebugPlayerStatusText => battleUI != null ? battleUI.DebugPlayerStatusText : "";
    public string DebugEnemyStatusText => battleUI != null ? battleUI.DebugEnemyStatusText : "";
    public string DebugEnemyIntentText => battleUI != null ? battleUI.DebugEnemyIntentText : "";
    public string DebugEnemyBreakText => battleUI != null ? battleUI.DebugEnemyBreakText : "";
    public float DebugEnemyBreakBarValue => battleUI != null ? battleUI.DebugEnemyBreakBarValue : -1f;
    public float DebugEnemyBreakBarMaxValue => battleUI != null ? battleUI.DebugEnemyBreakBarMaxValue : -1f;
    public string DebugImpactText => battleUI != null ? battleUI.DebugImpactText : "";
    public string DebugRunStatusText => battleUI != null ? battleUI.DebugRunStatusText : "";
    public string DebugStageText => battleUI != null ? battleUI.DebugStageText : "";
    public string DebugStageObjectiveText => battleUI != null ? battleUI.DebugStageObjectiveText : "";
    public string DebugStageProgressText => battleUI != null ? battleUI.DebugStageProgressText : "";
    public bool DebugRetryButtonVisible => battleUI != null && battleUI.DebugRetryButtonVisible;
    public bool DebugRetryButtonInteractable => battleUI != null && battleUI.DebugRetryButtonInteractable;
    public bool DebugContinueButtonVisible => battleUI != null && battleUI.DebugContinueButtonVisible;
    public bool DebugContinueButtonInteractable => battleUI != null && battleUI.DebugContinueButtonInteractable;
    public bool DebugStageSelectButtonVisible => battleUI != null && battleUI.DebugStageSelectButtonVisible;
    public bool DebugStageSelectButtonInteractable => battleUI != null && battleUI.DebugStageSelectButtonInteractable;
    public bool DebugResultSummaryPanelVisible => battleUI != null && battleUI.DebugResultSummaryPanelVisible;
    public int DebugTotalDamageDealt => totalDamageDealt;
    public int DebugTotalDamageTaken => totalDamageTaken;
    public int DebugGuardUseCount => guardUseCount;
    public int DebugSkillsUsedCount => skillsUsedCount;
    public int DebugTotalGoldEarned => totalGoldEarned;
    public string DebugItemsText => playerItems != null ? string.Join(" | ", playerItems.ConvertAll(i => $"{i.itemName}x{i.quantity}")) : "";

    // --- Config helpers ---
    private ScreenShake GetOrCacheShake()
    {
        if (cachedShake == null && Camera.main != null)
            cachedShake = Camera.main.GetComponent<ScreenShake>();
        return cachedShake;
    }

    private int CfgPlayerMaxHp => balanceConfig != null ? balanceConfig.playerMaxHp : 100;
    private int CfgPlayerAttack => balanceConfig != null ? balanceConfig.playerAttack : 20;
    private int CfgPlayerMaxAp => balanceConfig != null ? balanceConfig.playerMaxAp : 3;
    private int CfgPlayerApRecovery => balanceConfig != null ? balanceConfig.playerApRecoveryPerTurn : 2;
    private int CfgBasicSkillPower => balanceConfig != null ? balanceConfig.basicSkillPower : 20;
    private int CfgBasicSkillApCost => balanceConfig != null ? balanceConfig.basicSkillApCost : 0;
    private int CfgFireSkillPower => balanceConfig != null ? balanceConfig.fireSkillPower : 30;
    private int CfgFireSkillApCost => balanceConfig != null ? balanceConfig.fireSkillApCost : 2;
    private int CfgIceSkillPower => balanceConfig != null ? balanceConfig.iceSkillPower : 25;
    private int CfgIceSkillApCost => balanceConfig != null ? balanceConfig.iceSkillApCost : 1;
    private int CfgStunTurnDuration => balanceConfig != null ? balanceConfig.stunTurnDuration : 1;
    private int CfgLightningSkillPower => balanceConfig != null ? balanceConfig.lightningSkillPower : 40;
    private int CfgLightningSkillApCost => balanceConfig != null ? balanceConfig.lightningSkillApCost : 3;
    private int CfgEarthSkillPower => balanceConfig != null ? balanceConfig.earthSkillPower : 22;
    private int CfgEarthSkillApCost => balanceConfig != null ? balanceConfig.earthSkillApCost : 2;
    private int CfgEarthSkillShieldAmount => balanceConfig != null ? balanceConfig.earthSkillShieldAmount : 20;
    private int CfgBurnDamagePerTurn => balanceConfig != null ? balanceConfig.burnDamagePerTurn : 5;
    private int CfgBurnTurnDuration => balanceConfig != null ? balanceConfig.burnTurnDuration : 2;
    private int CfgGuardReductionPercent => balanceConfig != null ? balanceConfig.guardDamageReductionPercent : 50;
    private int CfgSRankRewardGold => balanceConfig != null ? balanceConfig.sRankRewardGold : 150;
    private int CfgARankRewardGold => balanceConfig != null ? balanceConfig.aRankRewardGold : 120;
    private int CfgBRankRewardGold => balanceConfig != null ? balanceConfig.bRankRewardGold : 100;
    private int CfgDefeatRewardGold => balanceConfig != null ? balanceConfig.defeatRewardGold : 0;
    private int CfgMaxBattleLogEntries => balanceConfig != null ? balanceConfig.maxBattleLogEntries : 6;
    private float CfgWeaknessMultiplier => balanceConfig != null ? balanceConfig.weaknessDamageMultiplier : 1.5f;
    private float CfgNeutralMultiplier => balanceConfig != null ? balanceConfig.neutralDamageMultiplier : 1.0f;
    private float CfgBattleSpeed => balanceConfig != null ? balanceConfig.battleSpeedMultiplier : 1.0f;

    // --- Lifecycle ---

    private void Start()
    {
        if (battleUI != null)
        {
            battleUI.SetupButtonListeners(
                OnClickAttackButton, OnClickFireSkillButton, OnClickIceSkillButton, OnClickLightningSkillButton,
                OnClickEarthSkillButton,
                OnClickEndTurnButton, OnClickGuardButton,
                OnClickRetryButton, OnClickContinueButton,
                OnClickStageSelectButton, OnClickSpeedToggle, OnClickAutoBattleToggle,
                OnClickItemButton,
                OnClickPauseButton);
            battleUI.SetupPauseListeners(OnResumeGame, OnClickStageSelectButton);
        }
        InitializeFromStageSelection();
        LoadProgressFromState();
        StartBattle();
    }

    public void DebugLoadEncountersForStage(int stageIndex)
    {
        int clamped = stageIndex < 0 ? 0 : stageIndex;
        stageEncounters = StageData.GetEncountersForStage(clamped);
        currentStageIndex = 0;
    }

    private void InitializeFromStageSelection()
    {
        int idx = StageSelectController.SelectedStageIndex;
        if (idx < 0) idx = 0; // fallback
        stageEncounters = StageData.GetEncountersForStage(idx);
        currentStageIndex = 0;
    }

    public void DebugStartBattleForTest() => StartBattle();
    public void DebugEndBattleForTest(BattleState resultState) => EndBattle(resultState);
    public void DebugResolveEnemyAttackForTest() => ResolveEnemyAttack();
    public void DebugSetPlayerApForTest(int value)
    {
        if (player == null) return;
        player.currentAp = Mathf.Clamp(value, 0, player.maxAp);
        battleUI?.UpdateAllUI(currentState, player, enemy, enemyPattern, enemyTurnCount,
            currentStageIndex, stageEncounters, playerName, enemyName, totalGoldEarned,
            CfgGuardReductionPercent, CfgBurnTurnDuration, playerIsGuarding, "Debug: AP adjusted",
            basicAttackSkill, fireSkill, iceSkill, lightningSkill, earthSkill, CfgMaxBattleLogEntries);
    }
    public void DebugForceEnemyBrokenForTest()
    {
        if (enemy == null) return;
        enemy.currentBreakGauge = 0;
        enemy.isBroken = true;
        battleUI?.UpdateAllUI(currentState, player, enemy, enemyPattern, enemyTurnCount,
            currentStageIndex, stageEncounters, playerName, enemyName, totalGoldEarned,
            CfgGuardReductionPercent, CfgBurnTurnDuration, playerIsGuarding, "Debug: Forced BROKEN",
            basicAttackSkill, fireSkill, iceSkill, lightningSkill, earthSkill, CfgMaxBattleLogEntries);
    }

    // --- Battle flow ---

    private void StartBattle()
    {
        currentState = BattleState.Start;
        if (!loadedProgressState) LoadProgressFromState();
        EnsureStageEncounters();
        ApplyCurrentStageData();
        EnsureEnemyPattern();

        player = new CharacterData(playerName, CfgPlayerMaxHp + EquipmentManager.TotalHpBonus, CfgPlayerAttack + EquipmentManager.TotalAttackBonus, ElementType.None, CfgPlayerMaxAp + EquipmentManager.TotalApBonus);
        int diffHp = Mathf.RoundToInt(enemyMaxHp * ProgressState.DifficultyHpMultiplier);
        int diffAtk = Mathf.RoundToInt(enemyPattern.normalAttackDamage * ProgressState.DifficultyDamageMultiplier);
        enemy = new CharacterData(enemyName, diffHp, diffAtk, enemyWeakness);
        enemy.maxBreakGauge = 2 * ProgressState.DifficultyBreakGaugeMultiplier;
        enemy.currentBreakGauge = enemy.maxBreakGauge;
        basicAttackSkill = new SkillData(basicSkillName, CfgBasicSkillPower, CfgBasicSkillApCost, ElementType.Physical, StatusEffectType.None);
        fireSkill = new SkillData(fireSkillName, CfgFireSkillPower, CfgFireSkillApCost, ElementType.Fire, StatusEffectType.Burn);
        iceSkill = new SkillData(iceSkillName, CfgIceSkillPower, CfgIceSkillApCost, ElementType.Ice, StatusEffectType.Stun);
        lightningSkill = new SkillData(lightningSkillName, CfgLightningSkillPower, CfgLightningSkillApCost, ElementType.Lightning, StatusEffectType.None);
        earthSkill = new SkillData(earthSkillName, CfgEarthSkillPower, CfgEarthSkillApCost, ElementType.Earth, StatusEffectType.None);
        basicAttackSkill.description = "Reliable no-cost physical attack.";
        fireSkill.description = "Costs AP, hits the enemy weakness, and applies Burn.";
        iceSkill.description = "1 AP, Ice element, applies Stun for 1 turn.";
        lightningSkill.description = "3 AP, Lightning element, high damage.";
        earthSkill.description = "2 AP, Earth element, applies Shield reducing next incoming damage.";

        playerIsGuarding = false;
        enemyTurnCount = 0;
        totalDamageDealt = 0;
        totalDamageTaken = 0;
        guardUseCount = 0;
        skillsUsedCount = 0;
        currentBattleRewardClaimed = false;
        playerShieldAmount = 0;
        selectedItemIndex = -1;
        skillsUsedNames.Clear();
        guardedStrongAttack = false;
        usedItems = false;
        isEnraged = false;

        // Initialize player inventory
        playerItems = new List<ItemData>
        {
            new ItemData { itemName = "Potion", description = "Restore 30 HP.", effectType = ItemEffectType.HealHp, effectValue = 30, quantity = 3 },
            new ItemData { itemName = "Hi-Potion", description = "Restore 60 HP.", effectType = ItemEffectType.HealHp, effectValue = 60, quantity = 2 },
            new ItemData { itemName = "Ether", description = "Restore 2 AP.", effectType = ItemEffectType.RestoreAp, effectValue = 2, quantity = 2 },
            new ItemData { itemName = "Full Ether", description = "Restore all AP.", effectType = ItemEffectType.RestoreAp, effectValue = 99, quantity = 1 }
        };

        // Set up element-aware sprites
        bool isBossEncounter = stageEncounters != null && currentStageIndex == 1;
        battleUI?.ClearCachedSprites();
        battleUI?.SetupPlaceholderSprites(enemyWeakness, isBossEncounter);

        battleUI?.StartNewBattle();
        battleUI?.SetImpactText("Impact: Ready");
        battleUI?.SetPlayerShieldText(0);

        // Apply stage modifier after UI is ready (enemy/player already created)
        ApplyStageModifier();

        // Screen fade in
        if (screenFade == null) screenFade = FindObjectOfType<ScreenFade>();
        if (screenFade != null) screenFade.FadeIn(0.3f, null);

        // Start BGM
        AudioManager.Instance?.PlayBattleBgm();

        var stageData = GetCurrentStageData();
        string startMsg = stageData != null && !string.IsNullOrEmpty(stageData.encounterDescription)
            ? $"Battle Start!\n{stageData.encounterDescription}"
            : "Battle Start!";
        if (!string.IsNullOrEmpty(stageModifierActivationMessage))
            startMsg += $"\n\n⚠ {stageModifierActivationMessage}";

        battleUI?.UpdateAllUI(currentState, player, enemy, enemyPattern, enemyTurnCount,
            currentStageIndex, stageEncounters, playerName, enemyName, totalGoldEarned,
            CfgGuardReductionPercent, CfgBurnTurnDuration, playerIsGuarding, startMsg,
            basicAttackSkill, fireSkill, iceSkill, lightningSkill, earthSkill, CfgMaxBattleLogEntries);

        StartPlayerTurn();
    }

    private void StartPlayerTurn()
    {
        currentState = BattleState.PlayerTurn;
        if (player != null) player.RecoverAp(CfgPlayerApRecovery);
        battleUI?.UpdateActionButtons(player, basicAttackSkill, fireSkill, iceSkill, lightningSkill, earthSkill, currentState);
        battleUI?.UpdateAllUI(currentState, player, enemy, enemyPattern, enemyTurnCount,
            currentStageIndex, stageEncounters, playerName, enemyName, totalGoldEarned,
            CfgGuardReductionPercent, CfgBurnTurnDuration, playerIsGuarding,
            $"Player Turn: recovered {CfgPlayerApRecovery} AP. Choose an action.",
            basicAttackSkill, fireSkill, iceSkill, lightningSkill, earthSkill, CfgMaxBattleLogEntries);
        if (autoBattleEnabled) ExecuteAutoAction();
    }

    private void LoadProgressFromState()
    {
        playerLevel = Mathf.Max(1, ProgressState.PlayerLevel);
        playerXp = Mathf.Max(0, ProgressState.PlayerXp);
        totalGoldEarned = Mathf.Max(0, ProgressState.TotalGold);
        loadedProgressState = true;
    }

    private void SyncProgressToState()
    {
        ProgressState.PlayerLevel = Mathf.Max(1, playerLevel);
        ProgressState.PlayerXp = Mathf.Max(0, playerXp);
        ProgressState.TotalGold = Mathf.Max(0, totalGoldEarned);
    }

    // --- Player actions (public for button binding & testing) ---

    public void OnClickAttackButton() => UsePlayerSkill(basicAttackSkill);
    public void OnClickFireSkillButton() => UsePlayerSkill(fireSkill);
    public void OnClickIceSkillButton() => UsePlayerSkill(iceSkill);
    public void OnClickLightningSkillButton() => UsePlayerSkill(lightningSkill);
    public void OnClickEarthSkillButton() => UsePlayerSkill(earthSkill);
    public void OnClickEndTurnButton() => EndPlayerTurn();
    public void OnClickGuardButton() => GuardAndEndPlayerTurn();

    public void OnClickRetryButton()
    {
        if (currentState != BattleState.Victory && currentState != BattleState.Defeat) return;
        StopAllCoroutines();
        StartBattle();
    }

    public void OnClickAutoBattleToggle()
    {
        autoBattleEnabled = !autoBattleEnabled;
        battleUI?.SetAutoBattleIndicator(autoBattleEnabled);
        if (autoBattleEnabled && currentState == BattleState.PlayerTurn)
            ExecuteAutoAction();
    }

    private void ExecuteAutoAction()
    {
        if (currentState != BattleState.PlayerTurn || player == null || enemy == null) return;
        if (enemy.IsDead()) { EndBattle(BattleState.Victory); return; }

        // AI decision tree (ordered by priority):
        // 1. Guard if incoming strong attack
        bool strongAttackIncoming = enemyPattern.IsStrongAttackTurn(enemyTurnCount + 1);
        if (strongAttackIncoming && playerIsGuarding == false)
        {
            GuardAndEndPlayerTurn();
            return;
        }

        // 2. Use Potion if HP is critical (< 30%)
        var potion = playerItems?.Find(i => i.itemName == "Potion" && i.quantity > 0);
        if (potion != null && player != null && player.currentHp < player.maxHp * 0.3f)
        {
            UseItem(potion);
            return;
        }

        // 3. Use Ether if no AP for any elemental skill
        var ether = playerItems?.Find(i => i.itemName == "Ether" && i.quantity > 0);
        if (ether != null && player != null && player.currentAp < 1 && !player.HasEnoughAp(fireSkill.apCost))
        {
            UseItem(ether);
            return;
        }

        // 4. Use weakness skill if enemy has break gauge remaining
        SkillData weaknessSkill = GetWeaknessSkill();
        if (weaknessSkill != null && IsSkillAvailable(weaknessSkill) && !enemy.isBroken && player.HasEnoughAp(weaknessSkill.apCost))
        {
            UsePlayerSkill(weaknessSkill);
            return;
        }

        // 3. Use Lightning Strike if enough AP
        if (IsSkillAvailable(lightningSkill) && player.HasEnoughAp(lightningSkill.apCost))
        {
            UsePlayerSkill(lightningSkill);
            return;
        }

        // 4. Use Ice Lance if enough AP
        if (IsSkillAvailable(iceSkill) && player.HasEnoughAp(iceSkill.apCost))
        {
            UsePlayerSkill(iceSkill);
            return;
        }

        // 5. Use Earth Wall if shield not active and HP is low (defensive)
        if (IsSkillAvailable(earthSkill) && playerShieldAmount <= 0 && player != null && player.currentHp < player.maxHp * 0.6f && player.HasEnoughAp(earthSkill.apCost))
        {
            UsePlayerSkill(earthSkill);
            return;
        }

        // 6. Use Fire Bolt if enough AP
        if (IsSkillAvailable(fireSkill) && player.HasEnoughAp(fireSkill.apCost))
        {
            UsePlayerSkill(fireSkill);
            return;
        }

        // 7. Default: basic attack
        OnClickAttackButton();
    }

    private SkillData GetWeaknessSkill()
    {
        if (enemy == null) return null;
        ElementType weak = enemy.weaknessElement;
        if (weak == ElementType.None) return null;
        if (iceSkill.elementType == weak) return iceSkill;
        if (fireSkill.elementType == weak) return fireSkill;
        if (lightningSkill.elementType == weak) return lightningSkill;
        if (earthSkill.elementType == weak) return earthSkill;
        return null;
    }

    public void OnClickSpeedToggle()
    {
        speedState = speedState >= 2 ? 1 : 2;
        battleUI?.UpdateSpeedLabel(speedState);
    }

    private bool IsSkillAvailable(SkillData skill)
    {
        return skill != null && ProgressState.IsSkillUnlocked(skill.skillName);
    }

    public void OnClickItemButton()
    {
        if (currentState != BattleState.PlayerTurn || player == null) return;
        if (playerItems == null || playerItems.Count == 0 || playerItems.TrueForAll(i => i.quantity <= 0))
        {
            battleUI?.UpdateAllUI(currentState, player, enemy, enemyPattern, enemyTurnCount,
                currentStageIndex, stageEncounters, playerName, enemyName, totalGoldEarned,
                CfgGuardReductionPercent, CfgBurnTurnDuration, playerIsGuarding,
                "No items available.",
                basicAttackSkill, fireSkill, iceSkill, lightningSkill, earthSkill, CfgMaxBattleLogEntries);
            return;
        }
        // Cycle to next available item
        selectedItemIndex = (selectedItemIndex + 1) % playerItems.Count;
        int attempts = 0;
        while (playerItems[selectedItemIndex].quantity <= 0 && attempts < playerItems.Count)
        {
            selectedItemIndex = (selectedItemIndex + 1) % playerItems.Count;
            attempts++;
        }
        if (playerItems[selectedItemIndex].quantity <= 0)
        {
            selectedItemIndex = -1;
            return;
        }
        UseItem(playerItems[selectedItemIndex]);
    }

    public void OnClickPauseButton()
    {
        if (currentState == BattleState.Victory || currentState == BattleState.Defeat) return;
        battleUI?.SetPauseVisible(true);
        battleUI?.SetActionButtonsInteractable(false);
    }

    public void OnResumeGame()
    {
        battleUI?.SetPauseVisible(false);
        if (currentState == BattleState.PlayerTurn)
            battleUI?.SetActionButtonsInteractable(true);
    }

    public void OnClickContinueButton()
    {
        if (currentState != BattleState.Victory || !HasNextStage()) return;
        StopAllCoroutines();
        if (screenFade == null) screenFade = FindObjectOfType<ScreenFade>();
        if (screenFade != null)
            screenFade.FadeOut(0.3f, () => { currentStageIndex++; StartBattle(); });
        else
        {
            currentStageIndex++;
            StartBattle();
        }
    }

    public void OnClickStageSelectButton()
    {
        StopAllCoroutines();
        battleUI?.SetPauseVisible(false);
        UnityEngine.SceneManagement.SceneManager.LoadScene("StageSelectScene");
    }

    private void EndPlayerTurn()
    {
        if (currentState != BattleState.PlayerTurn) return;
        battleUI?.SetActionButtonsInteractable(false);
        battleUI?.UpdateAllUI(currentState, player, enemy, enemyPattern, enemyTurnCount,
            currentStageIndex, stageEncounters, playerName, enemyName, totalGoldEarned,
            CfgGuardReductionPercent, CfgBurnTurnDuration, playerIsGuarding,
            $"{player?.characterName} skipped the turn. You can recover more AP next turn.",
            basicAttackSkill, fireSkill, iceSkill, lightningSkill, earthSkill, CfgMaxBattleLogEntries);
        if (Application.isPlaying) StartCoroutine(EnemyTurnRoutine());
    }

    private void UseItem(ItemData item)
    {
        if (currentState != BattleState.PlayerTurn || player == null || item == null) return;
        item.quantity--;
        usedItems = true;
        AudioManager.Instance?.PlayItemSfx();

        string msg;
        switch (item.effectType)
        {
            case ItemEffectType.HealHp:
                int heal = Mathf.Min(item.effectValue, player.maxHp - player.currentHp);
                player.currentHp += heal;
                msg = $"{player.characterName} uses {item.itemName}! Restores {heal} HP.";
                // Heal popup on player
                battleUI?.ShowHealNumber(heal);
                AudioManager.Instance?.PlayHealSfx();
                break;
            case ItemEffectType.RestoreAp:
                int apGain = Mathf.Min(item.effectValue, player.maxAp - player.currentAp);
                player.currentAp += apGain;
                msg = $"{player.characterName} uses {item.itemName}! Restores {apGain} AP.";
                // AP restore buff popup on player
                if (battleUI != null)
                    battleUI.ShowBuffOnPlayer($"+{apGain} AP", new Color(0.3f, 0.6f, 1f));
                break;
            default:
                msg = $"{player.characterName} uses {item.itemName}.";
                break;
        }

        battleUI?.SetActionButtonsInteractable(false);
        battleUI?.SetImpactText($"Impact: Used {item.itemName}");
        battleUI?.UpdateAllUI(currentState, player, enemy, enemyPattern, enemyTurnCount,
            currentStageIndex, stageEncounters, playerName, enemyName, totalGoldEarned,
            CfgGuardReductionPercent, CfgBurnTurnDuration, playerIsGuarding, msg,
            basicAttackSkill, fireSkill, iceSkill, lightningSkill, earthSkill, CfgMaxBattleLogEntries);

        if (Application.isPlaying) StartCoroutine(EnemyTurnRoutine());
    }

    private void GuardAndEndPlayerTurn()
    {
        if (currentState != BattleState.PlayerTurn) return;
        playerIsGuarding = true;
        guardUseCount++;
        battleUI?.SetActionButtonsInteractable(false);
        // Guard visual feedback: screen flash + guard icon flash
        battleUI?.ScreenFlash(0.08f);
        battleUI?.FlashPlayerDamage(); // reuse flash routine to give a blue/white shimmer
        string guardMsg = $"{player?.characterName} guards. Next enemy attack damage is reduced.";
        battleUI?.UpdateAllUI(currentState, player, enemy, enemyPattern, enemyTurnCount,
            currentStageIndex, stageEncounters, playerName, enemyName, totalGoldEarned,
            CfgGuardReductionPercent, CfgBurnTurnDuration, playerIsGuarding, guardMsg,
            basicAttackSkill, fireSkill, iceSkill, lightningSkill, earthSkill, CfgMaxBattleLogEntries);
        if (Application.isPlaying) StartCoroutine(EnemyTurnRoutine());
    }

    private void UsePlayerSkill(SkillData skill)
    {
        if (currentState != BattleState.PlayerTurn || player == null || enemy == null || skill == null) return;
        if (!IsSkillAvailable(skill))
        {
            battleUI?.UpdateAllUI(currentState, player, enemy, enemyPattern, enemyTurnCount,
                currentStageIndex, stageEncounters, playerName, enemyName, totalGoldEarned,
                CfgGuardReductionPercent, CfgBurnTurnDuration, playerIsGuarding,
                $"{skill.skillName} is locked. Clear earlier stages to unlock it.",
                basicAttackSkill, fireSkill, iceSkill, lightningSkill, earthSkill, CfgMaxBattleLogEntries);
            battleUI?.UpdateActionButtons(player, basicAttackSkill, fireSkill, iceSkill, lightningSkill, earthSkill, currentState);
            return;
        }
        if (!player.SpendAp(skill.apCost))
        {
            battleUI?.UpdateAllUI(currentState, player, enemy, enemyPattern, enemyTurnCount,
                currentStageIndex, stageEncounters, playerName, enemyName, totalGoldEarned,
                CfgGuardReductionPercent, CfgBurnTurnDuration, playerIsGuarding,
                $"Not enough AP. {skill.skillName} needs {skill.apCost} AP.",
                basicAttackSkill, fireSkill, iceSkill, lightningSkill, earthSkill, CfgMaxBattleLogEntries);
            battleUI?.UpdateActionButtons(player, basicAttackSkill, fireSkill, iceSkill, lightningSkill, earthSkill, currentState);
            return;
        }

        battleUI?.SetActionButtonsInteractable(false);
        skillsUsedCount++;
        if (skill != null) skillsUsedNames.Add(skill.skillName);
        int damage = CalculateSkillDamage(enemy, skill);
        bool wasBroken = enemy != null && enemy.isBroken;
        if (wasBroken)
        {
            damage = Mathf.RoundToInt(damage * 1.5f);
        }
        int before = enemy.currentHp;
        enemy.TakeDamage(damage);
        TrackDamageDealt(before);

        if (skill.statusEffectType == StatusEffectType.Burn)
        {
            enemy.ApplyStatusEffect(StatusEffectType.Burn, CfgBurnTurnDuration);
            AudioManager.Instance?.PlayBurnSfx();
        }
        else if (skill.statusEffectType == StatusEffectType.Stun)
        {
            enemy.ApplyStatusEffect(StatusEffectType.Stun, CfgStunTurnDuration);
            AudioManager.Instance?.PlayStunSfx();
        }

        // Break gauge: reduce on weakness hit
        if (!wasBroken && skill.elementType != ElementType.None && enemy != null && skill.elementType == enemy.weaknessElement)
        {
            enemy.ReduceBreakGauge(1);
        }

        // Reset Break after consuming it with an attack
        if (wasBroken)
        {
            enemy.ResetBreakGauge();
        }

        // Earth Wall: apply Shield to player
        if (skill == earthSkill)
        {
            playerShieldAmount = CfgEarthSkillShieldAmount;
            AudioManager.Instance?.PlayShieldSfx();
            // Show shield buff popup on player
            if (battleUI != null)
                battleUI.ShowBuffOnPlayer($"Shield +{CfgEarthSkillShieldAmount}", new Color(0.3f, 0.7f, 1f));
        }

        // Check enrage: enemy enrages when HP drops below 30%
        if (!isEnraged && enemy != null && !enemy.IsDead() && enemy.currentHp < enemy.maxHp * 0.3f)
        {
            isEnraged = true;
            string enrageMsg = $"{enemy.characterName} is ENRAGED! Its attacks intensify!";
            // Add to battle log via the message
            battleUI?.UpdateAllUI(currentState, player, enemy, enemyPattern, enemyTurnCount,
                currentStageIndex, stageEncounters, playerName, enemyName, totalGoldEarned,
                CfgGuardReductionPercent, CfgBurnTurnDuration, playerIsGuarding, enrageMsg,
                basicAttackSkill, fireSkill, iceSkill, lightningSkill, earthSkill, CfgMaxBattleLogEntries);
        }

        string msg = BuildSkillMessage(skill, damage);
        // Append shield info for Earth Wall
        if (skill == earthSkill)
        {
            msg += $" Shield active! ({CfgEarthSkillShieldAmount} damage absorbed next hit).";
        }

        // Set impact text based on skill - pass wasBroken for correct text before reset
        string impact = BuildImpactText(skill, damage, wasBroken);
        // Append shield info for Earth Wall
        if (skill == earthSkill)
        {
            impact += $" | Shield {CfgEarthSkillShieldAmount} applied";
        }
        battleUI?.SetImpactText(impact);
        battleUI?.SetPlayerShieldText(playerShieldAmount);
        battleUI?.FlashEnemyDamage();

        // VFX: damage popup
        bool isWeaknessHit = skill.elementType != ElementType.None && enemy != null && skill.elementType == enemy.weaknessElement;
        battleUI?.ShowDamageNumber(damage, isWeaknessHit);

        // VFX: screen shake on skill use (stronger for lightning/high-damage)
        var shake = GetOrCacheShake();
        if (shake != null)
        {
            float shMag = skill == lightningSkill ? 0.12f : 0.06f;
            shake.Shake(0.12f, shMag);
        }

        // VFX: screen flash on break or weakness hit
        if (wasBroken || isWeaknessHit)
        {
            battleUI?.ScreenFlash(0.12f);
        }
        if (wasBroken)
        {
            battleUI?.ShowBreakPopup();
            AudioManager.Instance?.PlayBreakSfx();
        }

        // Skill projectile
        if (battleUI != null)
        {
            Vector3 start = battleUI.GetPlayerSpriteWorldPosition();
            Vector3 end = battleUI.GetEnemySpriteWorldPosition();
            SkillProjectile.Spawn(skill.elementType, start, end, battleUI.GetProjectileParent());
        }
        // Play sound effects
        if (skill == basicAttackSkill)
            AudioManager.Instance?.PlayAttackSfx();
        else
            AudioManager.Instance?.PlaySkillSfx();

        battleUI?.UpdateAllUI(currentState, player, enemy, enemyPattern, enemyTurnCount,
            currentStageIndex, stageEncounters, playerName, enemyName, totalGoldEarned,
            CfgGuardReductionPercent, CfgBurnTurnDuration, playerIsGuarding, msg,
            basicAttackSkill, fireSkill, iceSkill, lightningSkill, earthSkill, CfgMaxBattleLogEntries);

        if (enemy.IsDead()) { EndBattle(BattleState.Victory); return; }
        if (Application.isPlaying) StartCoroutine(EnemyTurnRoutine());
    }

    private WaitForSeconds WaitForBattleTick(float seconds = 1.0f)
    {
        return new WaitForSeconds(seconds / Mathf.Max(0.1f, CfgBattleSpeed * speedState));
    }

    private IEnumerator EnemyTurnRoutine()
    {
        currentState = BattleState.EnemyTurn;

        // Check Stun first - if stunned, enemy skips this turn
        if (enemy != null && enemy.HasStatusEffect(StatusEffectType.Stun))
        {
            battleUI?.UpdateAllUI(currentState, player, enemy, enemyPattern, enemyTurnCount,
                currentStageIndex, stageEncounters, playerName, enemyName, totalGoldEarned,
                CfgGuardReductionPercent, CfgBurnTurnDuration, playerIsGuarding,
                $"{enemy.characterName} is STUNNED! Skips this turn.",
                basicAttackSkill, fireSkill, iceSkill, lightningSkill, earthSkill, CfgMaxBattleLogEntries);
            enemy.ReduceStatusTurn();
            yield return WaitForBattleTick();
            StartPlayerTurn();
            yield break;
        }

        if (enemy != null && enemy.HasStatusEffect(StatusEffectType.Burn))
        {
            int before = enemy.currentHp;
            enemy.TakeDamage(CfgBurnDamagePerTurn);
            TrackDamageDealt(before);
            enemy.ReduceStatusTurn();
            battleUI?.UpdateAllUI(currentState, player, enemy, enemyPattern, enemyTurnCount,
                currentStageIndex, stageEncounters, playerName, enemyName, totalGoldEarned,
                CfgGuardReductionPercent, CfgBurnTurnDuration, playerIsGuarding,
                $"{enemy.characterName} takes {CfgBurnDamagePerTurn} burn damage.",
                basicAttackSkill, fireSkill, iceSkill, lightningSkill, earthSkill, CfgMaxBattleLogEntries);
            // VFX: burn damage popup
            battleUI?.ShowDamageNumber(CfgBurnDamagePerTurn);
            if (enemy.IsDead()) { yield return WaitForBattleTick(); EndBattle(BattleState.Victory); yield break; }
            yield return WaitForBattleTick();
        }
        yield return WaitForBattleTick();

        ResolveEnemyAttack();
        if (player != null && player.IsDead()) { EndBattle(BattleState.Defeat); yield break; }
        yield return WaitForBattleTick();
        StartPlayerTurn();
    }

    private void ResolveEnemyAttack()
    {
        EnsureEnemyPattern();
        enemyTurnCount++;
        int baseDamage = enemyPattern.GetDamageForTurn(enemyTurnCount);
        int damage = Mathf.RoundToInt(baseDamage * ProgressState.DifficultyDamageMultiplier);
        if (isEnraged) damage = Mathf.RoundToInt(damage * 1.5f);
        bool isStrong = enemyPattern.IsStrongAttackTurn(enemyTurnCount);

        bool wasGuarding = playerIsGuarding;
        if (wasGuarding)
        {
            damage = Mathf.Max(1, damage * (100 - CfgGuardReductionPercent) / 100);
            playerIsGuarding = false;
            if (isStrong) guardedStrongAttack = true;
        }

        // Shield absorbs damage before it reaches the player
        bool shieldAbsorbed = playerShieldAmount > 0;
        if (shieldAbsorbed)
        {
            int absorbed = Mathf.Min(playerShieldAmount, damage);
            damage = Mathf.Max(0, damage - absorbed);
            playerShieldAmount = 0;
        }

        int before = player != null ? player.currentHp : 0;
        player?.TakeDamage(damage);
        TrackDamageTaken(before);

        string msg;
        if (isStrong)
            msg = $"{(isEnraged ? "ENRAGED! " : "")}{enemy?.characterName} uses {enemyPattern.strongAttackName} on turn {enemyTurnCount}! {player?.characterName}{(wasGuarding ? " guards and" : "")}{(shieldAbsorbed ? " shield absorbs some damage and" : "")} takes {damage} damage.";
        else
            msg = $"{(isEnraged ? "ENRAGED! " : "")}{enemy?.characterName} {enemyPattern.normalAttackMessageVerb}! {player?.characterName}{(wasGuarding ? " guards and" : "")}{(shieldAbsorbed ? " shield absorbs some damage and" : "")} takes {damage} damage.";

        string impactText = wasGuarding
            ? $"Impact: Guard reduced incoming damage to {damage}"
            : shieldAbsorbed
                ? $"Impact: Shield absorbed damage, reduced to {damage}"
                : $"Impact: {enemy?.characterName} dealt {damage} damage{(isEnraged ? " (ENRAGED!)" : "")}";
        battleUI?.SetImpactText(impactText);
        battleUI?.SetPlayerShieldText(playerShieldAmount);
        battleUI?.FlashPlayerDamage();
        AudioManager.Instance?.PlayDamageSfx();
        if (wasGuarding) AudioManager.Instance?.PlayGuardSfx();

        // Screen flash on all enemy attacks
        battleUI?.ScreenFlash(wasGuarding ? 0.06f : isStrong ? 0.15f : 0.10f);

        if (isStrong)
        {
            var shake = GetOrCacheShake();
            if (shake != null) shake.Shake();
        }
        // Damage popup on player (not enemy)
        battleUI?.ShowDamageNumberOnPlayer(damage);

        // Guard visual feedback in impact text
        if (wasGuarding)
        {
            battleUI?.ShowBuffOnPlayer("🛡 Guarded!", new Color(0.3f, 0.7f, 1f));
        }

        battleUI?.UpdateAllUI(currentState, player, enemy, enemyPattern, enemyTurnCount,
            currentStageIndex, stageEncounters, playerName, enemyName, totalGoldEarned,
            CfgGuardReductionPercent, CfgBurnTurnDuration, playerIsGuarding, msg,
            basicAttackSkill, fireSkill, iceSkill, lightningSkill, earthSkill, CfgMaxBattleLogEntries);

        // Storm Surge: periodic hazard damage every 3 enemy turns
        if (enemyTurnCount > 0 && enemyTurnCount % 3 == 0 && player != null)
        {
            var currentStageData = GetCurrentStageData();
            if (currentStageData != null && currentStageData.stageModifier == StageModifierType.StormSurge)
            {
                int surgeDamage = 8;
                int beforeSurge = player.currentHp;
                player.TakeDamage(surgeDamage);
                totalDamageTaken += Mathf.Min(surgeDamage, beforeSurge > 0 ? beforeSurge : surgeDamage);

                string surgeMsg = $"⚡ Storm Surge strikes! Player takes {surgeDamage} lightning damage.";
                battleUI?.SetImpactText($"Impact: Storm Surge dealt {surgeDamage} hazard damage");
                battleUI?.ScreenFlash(0.12f);
                battleUI?.ShowDamageNumberOnPlayer(surgeDamage);

                battleUI?.UpdateAllUI(currentState, player, enemy, enemyPattern, enemyTurnCount,
                    currentStageIndex, stageEncounters, playerName, enemyName, totalGoldEarned,
                    CfgGuardReductionPercent, CfgBurnTurnDuration, playerIsGuarding, surgeMsg,
                    basicAttackSkill, fireSkill, iceSkill, lightningSkill, earthSkill, CfgMaxBattleLogEntries);

                if (player.currentHp <= 0)
                {
                    EndBattle(BattleState.Defeat);
                    return;
                }
            }
        }

        // Void Drain: drain AP every 2 enemy turns
        if (enemyTurnCount > 0 && enemyTurnCount % 2 == 0 && player != null)
        {
            var currentStageData = GetCurrentStageData();
            if (currentStageData != null && currentStageData.stageModifier == StageModifierType.VoidDrain)
            {
                string drainMsg;
                string drainImpact;
                if (player.currentAp >= 1)
                {
                    player.currentAp -= 1;
                    drainMsg = $"🌑 Void Drain saps 1 AP from {playerName}.";
                    drainImpact = "Impact: Void Drain reduced AP";
                }
                else
                {
                    int beforeDrain = player.currentHp;
                    player.TakeDamage(5);
                    totalDamageTaken += Mathf.Min(5, beforeDrain > 0 ? beforeDrain : 5);
                    drainMsg = $"🌑 Void Drain lashes out! {playerName} takes 5 shadow damage.";
                    drainImpact = "Impact: Void Drain dealt 5 hazard damage";
                    battleUI?.ShowDamageNumberOnPlayer(5);
                }

                battleUI?.SetImpactText(drainImpact);
                battleUI?.ScreenFlash(0.1f);
                battleUI?.UpdateAllUI(currentState, player, enemy, enemyPattern, enemyTurnCount,
                    currentStageIndex, stageEncounters, playerName, enemyName, totalGoldEarned,
                    CfgGuardReductionPercent, CfgBurnTurnDuration, playerIsGuarding, drainMsg,
                    basicAttackSkill, fireSkill, iceSkill, lightningSkill, earthSkill, CfgMaxBattleLogEntries);

                if (player.currentHp <= 0)
                {
                    EndBattle(BattleState.Defeat);
                    return;
                }
            }
        }
    }

    private void EndBattle(BattleState resultState)
    {
        currentState = resultState;
        battleUI?.SetActionButtonsInteractable(false);
        battleUI?.SetRetryButtonVisible(true);
        // Screen fade + brief delay for dramatic effect
        if (screenFade == null) screenFade = FindObjectOfType<ScreenFade>();
        if (screenFade != null)
        {
            screenFade.FadeOut(0.2f, () =>
            {
                ShowBattleResult(resultState);
                if (screenFade != null) screenFade.FadeIn(0.15f, null);
            });
        }
        else
        {
            ShowBattleResult(resultState);
        }
    }

    private void ShowBattleResult(BattleState resultState)
    {
        // Audio feedback
        if (resultState == BattleState.Victory)
        {
            AudioManager.Instance?.PlayVictoryBgm();
            AudioManager.Instance?.PlayVictorySfx();
        }
        else
        {
            AudioManager.Instance?.StopBgm();
            AudioManager.Instance?.PlayDefeatSfx();
        }
        bool hasNext = HasNextStage();
        battleUI?.SetContinueButtonVisible(resultState == BattleState.Victory && hasNext);
        if (resultState == BattleState.Victory && hasNext)
        {
            battleUI?.SetContinueButtonLabel("▶ Continue to Next Encounter");
        }
        battleUI?.SetStageSelectButtonVisible(true);

        // Mark stage as completed when all encounters are cleared
        // Award XP on stage clear
        bool finalStageCleared = resultState == BattleState.Victory && !HasNextStage();
        if (finalStageCleared)
        {
            int gainedXp = 50 + (StageSelectController.SelectedStageIndex + 1) * 30;
            playerXp += gainedXp;
            if (playerXp >= xpToNextLevel)
            {
                playerXp -= xpToNextLevel;
                playerLevel++;
                AudioManager.Instance?.PlayLevelUpSfx();
                xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * 1.5f);
                if (player != null) player.maxHp += 20;
                string lvlMsg = $"Level Up! Now Level {playerLevel}. Max HP increased to {player?.maxHp}.";
                battleUI?.UpdateAllUI(currentState, player, enemy, enemyPattern, enemyTurnCount,
                    currentStageIndex, stageEncounters, playerName, enemyName, totalGoldEarned,
                    CfgGuardReductionPercent, CfgBurnTurnDuration, playerIsGuarding, lvlMsg,
                    basicAttackSkill, fireSkill, iceSkill, lightningSkill, earthSkill, CfgMaxBattleLogEntries);
            }
            int stageIdx = StageSelectController.SelectedStageIndex;
            if (stageIdx >= 0) ProgressState.MarkStageCompleted(stageIdx);
        }

        // Evaluate stage bonuses on victory
        if (resultState == BattleState.Victory && !currentBattleRewardClaimed && !bonusRewardedStageIndexes.Contains(currentStageIndex))
        {
            var (bonuses, bonusGold) = StageBonusEvaluator.Evaluate(
                totalDamageTaken, enemyTurnCount, skillsUsedNames,
                guardedStrongAttack, usedItems, 5);
            totalGoldEarned += bonusGold;
            bonusRewardedStageIndexes.Add(currentStageIndex);
            if (bonuses.Count > 0 && battleUI != null)
            {
                string bonusMsg = "Bonuses: " + string.Join(", ", bonuses.ConvertAll(b => $"{StageBonusEvaluator.GetBonusName(b)}(+{StageBonusEvaluator.GetBonusGold(b)}g)"));
                battleUI?.UpdateAllUI(currentState, player, enemy, enemyPattern, enemyTurnCount,
                    currentStageIndex, stageEncounters, playerName, enemyName, totalGoldEarned,
                    CfgGuardReductionPercent, CfgBurnTurnDuration, playerIsGuarding, bonusMsg,
                    basicAttackSkill, fireSkill, iceSkill, lightningSkill, earthSkill, CfgMaxBattleLogEntries);
            }
        }

        string resultSummary = BuildResultSummaryText(resultState);
        battleUI?.SetResultSummaryVisible(true, resultSummary);
        SyncProgressToState();
        if (finalStageCleared) SaveManager.Save();

        string msg = resultState == BattleState.Victory
            ? battleUI?.BuildVictoryGuideMessage(currentStageIndex, stageEncounters) ?? "Victory!"
            : "Defeat... Try again.";
        battleUI?.UpdateAllUI(currentState, player, enemy, enemyPattern, enemyTurnCount,
            currentStageIndex, stageEncounters, playerName, enemyName, totalGoldEarned,
            CfgGuardReductionPercent, CfgBurnTurnDuration, playerIsGuarding, msg,
            basicAttackSkill, fireSkill, iceSkill, lightningSkill, earthSkill, CfgMaxBattleLogEntries);
    }

    // --- Damage tracking ---

    private int CalculateSkillDamage(CharacterData target, SkillData skill)
    {
        float multiplier = CfgNeutralMultiplier;
        if (skill.elementType != ElementType.None && skill.elementType == target.weaknessElement)
            multiplier = CfgWeaknessMultiplier;
        return Mathf.RoundToInt(skill.power * multiplier);
    }

    /// <summary>Returns a short label and the calculated multiplier for the given skill/target pair.</summary>
    private (string label, float multiplier) GetElementEffectiveness(CharacterData target, SkillData skill)
    {
        if (skill.elementType == ElementType.None)
            return ("Physical", 1f);
        if (skill.elementType == target.weaknessElement)
            return ("Weakness", CfgWeaknessMultiplier);
        return ("Neutral", CfgNeutralMultiplier);
    }

    private void TrackDamageDealt(int beforeHp)
    {
        if (enemy == null) return;
        totalDamageDealt += Mathf.Max(0, beforeHp - enemy.currentHp);
    }

    private void TrackDamageTaken(int beforeHp)
    {
        if (player == null) return;
        totalDamageTaken += Mathf.Max(0, beforeHp - player.currentHp);
    }

    private string BuildSkillMessage(SkillData skill, int damage)
    {
        if (player == null || enemy == null) return "";
        var (effLabel, _) = GetElementEffectiveness(enemy, skill);
        string msg = $"{player.characterName} uses {skill.skillName}! {enemy.characterName} takes {damage} damage. ({skill.elementType} | {effLabel})";
        if (skill.HasStatusEffect())
        {
            int dur = skill.statusEffectType == StatusEffectType.Burn ? CfgBurnTurnDuration : CfgStunTurnDuration;
            msg += $" Extra effect: {skill.statusEffectType} for {dur} turns.";
        }
        return msg;
    }

    private string BuildImpactText(SkillData skill, int damage, bool wasBrokenBeforeHit = false)
    {
        if (skill == null) return "Impact: Ready";
        var (effLabel, effMultiplier) = GetElementEffectiveness(enemy, skill);
        string impact = $"Impact: {skill.skillName} dealt {damage} damage";
        if (wasBrokenBeforeHit)
        {
            impact += " | Break bonus consumed";
        }
        else if (skill.elementType != ElementType.None && enemy != null && skill.elementType == enemy.weaknessElement)
        {
            impact += $" | Weakness x{effMultiplier:0.0}";
            if (enemy.isBroken)
                impact += " | BREAK!";
            else
                impact += $" | Break {enemy.currentBreakGauge}/{enemy.maxBreakGauge}";
        }
        else if (skill.elementType != ElementType.None && skill.elementType != ElementType.Physical && Mathf.Abs(effMultiplier - 1.0f) < 0.01f)
        {
            impact += $" | Neutral x{effMultiplier:0.0}";
        }
        else if (skill.elementType == ElementType.Physical)
        {
            impact += " | Physical";
        }
        if (skill.HasStatusEffect())
            impact += $" | {skill.statusEffectType} applied";
        return impact;
    }

    // --- Results ---

    private string BuildResultSummaryText(BattleState resultState)
    {
        if (player == null || enemy == null) return "";
        EnsureEnemyPattern();

        var data = BuildBattleResultData(resultState);
        if (resultState == BattleState.Victory) ClaimBattleReward(data.rewardGold);
        data.totalGold = totalGoldEarned;
        return BattleResultPresenter.BuildSummaryText(data);
    }

    private BattleResultData BuildBattleResultData(BattleState resultState)
    {
        EnsureEnemyPattern();
        string rank = BattleResultEvaluator.BuildRank(resultState, enemyTurnCount, totalDamageTaken, balanceConfig);
        string lastPattern = BattleResultEvaluator.BuildLastEnemyPatternLabel(enemyTurnCount, enemyPattern);
        string pace = BattleResultEvaluator.BuildPaceLabel(resultState, enemyTurnCount, balanceConfig);

        return new BattleResultData
        {
            resultLabel = resultState == BattleState.Victory ? "Victory" : "Defeat",
            enemyTurns = enemyTurnCount,
            playerName = player.characterName,
            playerCurrentHp = player.currentHp,
            playerMaxHp = player.maxHp,
            playerCurrentAp = player.currentAp,
            playerMaxAp = player.maxAp,
            enemyName = enemy.characterName,
            enemyCurrentHp = enemy.currentHp,
            enemyMaxHp = enemy.maxHp,
            damageDealt = totalDamageDealt,
            damageTaken = totalDamageTaken,
            guardUses = guardUseCount,
            skillsUsed = skillsUsedCount,
            paceLabel = pace,
            survivalLabel = BattleResultEvaluator.BuildSurvivalLabel(player.currentHp, player.maxHp),
            rank = rank,
            rewardGold = BattleResultEvaluator.BuildRewardGold(rank, CfgSRankRewardGold, CfgARankRewardGold, CfgBRankRewardGold, CfgDefeatRewardGold),
            totalGold = totalGoldEarned,
            resultTip = BattleResultEvaluator.BuildResultTip(rank, lastPattern, enemyPattern.strongAttackName),
            lastEnemyPattern = lastPattern
        };
    }

    private void ClaimBattleReward(int rewardGold)
    {
        if (currentBattleRewardClaimed || rewardedStageIndexes.Contains(currentStageIndex)) return;
        totalGoldEarned += Mathf.Max(0, rewardGold);
        rewardedStageIndexes.Add(currentStageIndex);
        currentBattleRewardClaimed = true;
    }

    // --- Stage data ---

    private void EnsureStageEncounters()
    {
        if (stageEncounters == null) stageEncounters = new List<StageData>();
        if (stageEncounters.Count == 0)
        {
            stageEncounters.Add(StageData.CreateStage1Normal());
            stageEncounters.Add(StageData.CreateStage1Boss());
        }
        currentStageIndex = Mathf.Clamp(currentStageIndex, 0, stageEncounters.Count - 1);
    }

    private void ApplyCurrentStageData()
    {
        var stage = GetCurrentStageData();
        if (stage?.enemy == null) return;
        enemyName = stage.enemy.enemyName;
        enemyMaxHp = stage.enemy.maxHp;
        enemyWeakness = stage.enemy.weakness;
        enemyPattern = stage.enemy.pattern;
    }

    private void ApplyStageModifier()
    {
        var stage = GetCurrentStageData();
        if (stage == null) return;
        StageModifierType modifier = stage.stageModifier;
        string modifierMsg = "";

        switch (modifier)
        {
            case StageModifierType.PackPressure:
                if (enemyPattern != null)
                {
                    enemyPattern.strongAttackEveryTurns = Mathf.Max(2, enemyPattern.strongAttackEveryTurns - 1);
                }
                modifierMsg = "⚠ Stage Modifier: Pack Pressure — Enemy uses strong attacks more frequently!";
                break;

            case StageModifierType.Stoneguard:
                if (enemy != null)
                {
                    enemy.maxBreakGauge += 1;
                    enemy.currentBreakGauge = enemy.maxBreakGauge;
                }
                modifierMsg = "⚠ Stage Modifier: Stoneguard — Enemy starts with reinforced break defense!";
                break;

            case StageModifierType.TutorialField:
                modifierMsg = "Stage Modifier: Tutorial Field — A safe training ground.";
                break;

            case StageModifierType.StormSurge:
                modifierMsg = "⚠ Stage Modifier: Storm Surge — Residual lightning strikes every 3 turns.";
                break;

            case StageModifierType.VoidDrain:
                modifierMsg = "⚠ Stage Modifier: Void Drain — Shadow energy drains AP over time.";
                break;

            case StageModifierType.RadiantTrial:
                if (enemyPattern != null)
                    enemyPattern.strongAttackEveryTurns = Mathf.Max(2, enemyPattern.strongAttackEveryTurns - 1);
                if (enemy != null)
                {
                    enemy.maxBreakGauge += 1;
                    enemy.currentBreakGauge = enemy.maxBreakGauge;
                }
                modifierMsg = "⚠ Stage Modifier: Radiant Trial — The ultimate trial. Enemies are relentless.";
                break;

            default:
                return;
        }

        // Store message for display on first UI update
        stageModifierActivationMessage = modifierMsg;
        // Note: modifier message is displayed via startMsg in StartBattle(), not as a separate log entry
    }

    private string stageModifierActivationMessage = "";

    private StageData GetCurrentStageData()
    {
        EnsureStageEncounters();
        return stageEncounters[currentStageIndex];
    }

    private bool HasNextStage()
    {
        EnsureStageEncounters();
        return currentStageIndex < stageEncounters.Count - 1;
    }

    private void EnsureEnemyPattern()
    {
        if (enemyPattern == null) enemyPattern = new EnemyPatternData();
    }
}
