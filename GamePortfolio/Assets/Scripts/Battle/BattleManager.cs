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
    public bool DebugAutoBattleEnabled => autoBattleEnabled;
    public int DebugPlayerLevel => playerLevel;
    public int DebugPlayerXp => playerXp; // 1=1x, 2=2x
    private int totalDamageDealt;
    private int totalDamageTaken;
    private int guardUseCount;
    private int skillsUsedCount;
    private int totalGoldEarned;
    private int playerShieldAmount;
    private int playerLevel = 1;
    private int playerXp;
    private int xpToNextLevel = 100;
    private bool currentBattleRewardClaimed;
    private readonly HashSet<int> rewardedStageIndexes = new HashSet<int>();

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

    // --- Config helpers ---
    private int CfgPlayerMaxHp => balanceConfig != null ? balanceConfig.playerMaxHp : 100;
    private int CfgPlayerMaxAp => balanceConfig != null ? balanceConfig.playerMaxAp : 3;
    private int CfgPlayerApRecovery => balanceConfig != null ? balanceConfig.playerApRecoveryPerTurn : 1;
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
    private int CfgEarthSkillShieldAmount => balanceConfig != null ? balanceConfig.earthSkillShieldAmount : 15;
    private int CfgBurnDamagePerTurn => balanceConfig != null ? balanceConfig.burnDamagePerTurn : 3;
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
                OnClickStageSelectButton, OnClickSpeedToggle, OnClickAutoBattleToggle);
        }
        InitializeFromStageSelection();
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
        EnsureStageEncounters();
        ApplyCurrentStageData();
        EnsureEnemyPattern();

        player = new CharacterData(playerName, CfgPlayerMaxHp, 20, ElementType.None, CfgPlayerMaxAp);
        enemy = new CharacterData(enemyName, enemyMaxHp, enemyPattern.normalAttackDamage, enemyWeakness);
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

        battleUI?.StartNewBattle();
        battleUI?.SetImpactText("Impact: Ready");
        battleUI?.SetPlayerShieldText(0);

        var stageData = GetCurrentStageData();
        string startMsg = stageData != null && !string.IsNullOrEmpty(stageData.encounterDescription)
            ? $"Battle Start!\n{stageData.encounterDescription}"
            : "Battle Start!";

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

        // 2. Use weakness skill if enemy has break gauge remaining
        SkillData weaknessSkill = GetWeaknessSkill();
        if (weaknessSkill != null && !enemy.isBroken && player.HasEnoughAp(weaknessSkill.apCost))
        {
            UsePlayerSkill(weaknessSkill);
            return;
        }

        // 3. Use Lightning Strike if enough AP
        if (player.HasEnoughAp(lightningSkill.apCost))
        {
            UsePlayerSkill(lightningSkill);
            return;
        }

        // 4. Use Ice Lance if enough AP
        if (player.HasEnoughAp(iceSkill.apCost))
        {
            UsePlayerSkill(iceSkill);
            return;
        }

        // 5. Use Earth Wall if shield not active and HP is low (defensive)
        if (playerShieldAmount <= 0 && player != null && player.currentHp < player.maxHp * 0.6f && player.HasEnoughAp(earthSkill.apCost))
        {
            UsePlayerSkill(earthSkill);
            return;
        }

        // 6. Use Fire Bolt if enough AP
        if (player.HasEnoughAp(fireSkill.apCost))
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

    public void OnClickContinueButton()
    {
        if (currentState != BattleState.Victory || !HasNextStage()) return;
        StopAllCoroutines();
        currentStageIndex++;
        StartBattle();
    }

    public void OnClickStageSelectButton()
    {
        if (currentState != BattleState.Victory && currentState != BattleState.Defeat) return;
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

    private void GuardAndEndPlayerTurn()
    {
        if (currentState != BattleState.PlayerTurn) return;
        playerIsGuarding = true;
        guardUseCount++;
        battleUI?.SetActionButtonsInteractable(false);
        battleUI?.UpdateAllUI(currentState, player, enemy, enemyPattern, enemyTurnCount,
            currentStageIndex, stageEncounters, playerName, enemyName, totalGoldEarned,
            CfgGuardReductionPercent, CfgBurnTurnDuration, playerIsGuarding,
            $"{player?.characterName} guards. Next enemy attack damage is reduced.",
            basicAttackSkill, fireSkill, iceSkill, lightningSkill, earthSkill, CfgMaxBattleLogEntries);
        if (Application.isPlaying) StartCoroutine(EnemyTurnRoutine());
    }

    private void UsePlayerSkill(SkillData skill)
    {
        if (currentState != BattleState.PlayerTurn || player == null || enemy == null) return;
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
            enemy.ApplyStatusEffect(StatusEffectType.Burn, CfgBurnTurnDuration);
        else if (skill.statusEffectType == StatusEffectType.Stun)
            enemy.ApplyStatusEffect(StatusEffectType.Stun, CfgStunTurnDuration);

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

        battleUI?.UpdateAllUI(currentState, player, enemy, enemyPattern, enemyTurnCount,
            currentStageIndex, stageEncounters, playerName, enemyName, totalGoldEarned,
            CfgGuardReductionPercent, CfgBurnTurnDuration, playerIsGuarding, msg,
            basicAttackSkill, fireSkill, iceSkill, lightningSkill, earthSkill, CfgMaxBattleLogEntries);

        if (enemy.IsDead()) { EndBattle(BattleState.Victory); return; }
        if (Application.isPlaying) StartCoroutine(EnemyTurnRoutine());
    }

    private WaitForSeconds WaitForBattleTick(float seconds = 1.0f)
    {
        return new WaitForSeconds(seconds / Mathf.Max(0.1f, CfgBattleSpeed));
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
        int damage = enemyPattern.GetDamageForTurn(enemyTurnCount);
        bool isStrong = enemyPattern.IsStrongAttackTurn(enemyTurnCount);

        bool wasGuarding = playerIsGuarding;
        if (wasGuarding)
        {
            damage = Mathf.Max(1, damage * (100 - CfgGuardReductionPercent) / 100);
            playerIsGuarding = false;
        }

        // Shield absorbs damage before it reaches the player
        bool shieldAbsorbed = playerShieldAmount > 0;
        if (shieldAbsorbed)
        {
            int absorbed = Mathf.Min(playerShieldAmount, damage);
            damage = Mathf.Max(1, damage - absorbed);
            playerShieldAmount = 0;
        }

        int before = player != null ? player.currentHp : 0;
        player?.TakeDamage(damage);
        TrackDamageTaken(before);

        string msg;
        if (isStrong)
            msg = $"{enemy?.characterName} uses {enemyPattern.strongAttackName} on turn {enemyTurnCount}! {player?.characterName}{(wasGuarding ? " guards and" : "")}{(shieldAbsorbed ? " shield absorbs some damage and" : "")} takes {damage} damage.";
        else
            msg = $"{enemy?.characterName} {enemyPattern.normalAttackMessageVerb}! {player?.characterName}{(wasGuarding ? " guards and" : "")}{(shieldAbsorbed ? " shield absorbs some damage and" : "")} takes {damage} damage.";

        string impactText = wasGuarding
            ? $"Impact: Guard reduced incoming damage to {damage}"
            : shieldAbsorbed
                ? $"Impact: Shield absorbed {CfgEarthSkillShieldAmount} damage, reduced to {damage}"
                : $"Impact: {enemy?.characterName} dealt {damage} damage";
        battleUI?.SetImpactText(impactText);
        battleUI?.SetPlayerShieldText(playerShieldAmount);
        battleUI?.FlashPlayerDamage();
        if (isStrong)
        {
            var shake = Camera.main != null ? Camera.main.GetComponent<ScreenShake>() : null;
            if (shake != null) shake.Shake();
        }

        battleUI?.UpdateAllUI(currentState, player, enemy, enemyPattern, enemyTurnCount,
            currentStageIndex, stageEncounters, playerName, enemyName, totalGoldEarned,
            CfgGuardReductionPercent, CfgBurnTurnDuration, playerIsGuarding, msg,
            basicAttackSkill, fireSkill, iceSkill, lightningSkill, earthSkill, CfgMaxBattleLogEntries);
    }

    private void EndBattle(BattleState resultState)
    {
        currentState = resultState;
        battleUI?.SetActionButtonsInteractable(false);
        battleUI?.SetRetryButtonVisible(true);
        bool hasNext = HasNextStage();
        battleUI?.SetContinueButtonVisible(resultState == BattleState.Victory && hasNext);
        if (resultState == BattleState.Victory && hasNext)
        {
            battleUI?.SetContinueButtonLabel("Next Encounter");
        }
        battleUI?.SetStageSelectButtonVisible(true);

        // Mark stage as completed when all encounters are cleared
        // Award XP on stage clear
        if (resultState == BattleState.Victory && !HasNextStage())
        {
            int gainedXp = 50 + (StageSelectController.SelectedStageIndex + 1) * 30;
            playerXp += gainedXp;
            if (playerXp >= xpToNextLevel)
            {
                playerXp -= xpToNextLevel;
                playerLevel++;
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
            SaveManager.Save();
        }

        string resultSummary = BuildResultSummaryText(resultState);
        battleUI?.SetResultSummaryVisible(true, resultSummary);

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
