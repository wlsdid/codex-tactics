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
    private bool playerIsGuarding;
    private int enemyTurnCount;
    private int totalDamageDealt;
    private int totalDamageTaken;
    private int guardUseCount;
    private int skillsUsedCount;
    private int totalGoldEarned;
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
    public string DebugRunStatusText => battleUI != null ? battleUI.DebugRunStatusText : "";
    public string DebugStageText => battleUI != null ? battleUI.DebugStageText : "";
    public string DebugStageObjectiveText => battleUI != null ? battleUI.DebugStageObjectiveText : "";
    public string DebugStageProgressText => battleUI != null ? battleUI.DebugStageProgressText : "";
    public bool DebugRetryButtonVisible => battleUI != null && battleUI.DebugRetryButtonVisible;
    public bool DebugRetryButtonInteractable => battleUI != null && battleUI.DebugRetryButtonInteractable;
    public bool DebugContinueButtonVisible => battleUI != null && battleUI.DebugContinueButtonVisible;
    public bool DebugContinueButtonInteractable => battleUI != null && battleUI.DebugContinueButtonInteractable;
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
    private int CfgBurnDamagePerTurn => balanceConfig != null ? balanceConfig.burnDamagePerTurn : 3;
    private int CfgBurnTurnDuration => balanceConfig != null ? balanceConfig.burnTurnDuration : 2;
    private int CfgGuardReductionPercent => balanceConfig != null ? balanceConfig.guardDamageReductionPercent : 50;
    private int CfgSRankRewardGold => balanceConfig != null ? balanceConfig.sRankRewardGold : 150;
    private int CfgARankRewardGold => balanceConfig != null ? balanceConfig.aRankRewardGold : 120;
    private int CfgBRankRewardGold => balanceConfig != null ? balanceConfig.bRankRewardGold : 100;
    private int CfgDefeatRewardGold => balanceConfig != null ? balanceConfig.defeatRewardGold : 0;
    private int CfgMaxBattleLogEntries => balanceConfig != null ? balanceConfig.maxBattleLogEntries : 6;

    // --- Lifecycle ---

    private void Start()
    {
        if (battleUI != null)
        {
            battleUI.SetupButtonListeners(
                OnClickAttackButton, OnClickFireSkillButton,
                OnClickEndTurnButton, OnClickGuardButton,
                OnClickRetryButton, OnClickContinueButton);
        }
        StartBattle();
    }

    public void DebugStartBattleForTest() => StartBattle();
    public void DebugEndBattleForTest(BattleState resultState) => EndBattle(resultState);
    public void DebugResolveEnemyAttackForTest() => ResolveEnemyAttack();

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
        basicAttackSkill.description = "Reliable no-cost physical attack.";
        fireSkill.description = "Costs AP, hits the enemy weakness, and applies Burn.";

        playerIsGuarding = false;
        enemyTurnCount = 0;
        totalDamageDealt = 0;
        totalDamageTaken = 0;
        guardUseCount = 0;
        skillsUsedCount = 0;
        currentBattleRewardClaimed = false;

        battleUI?.StartNewBattle();
        battleUI?.UpdateAllUI(currentState, player, enemy, enemyPattern, enemyTurnCount,
            currentStageIndex, stageEncounters, playerName, enemyName, totalGoldEarned,
            CfgGuardReductionPercent, CfgBurnTurnDuration, playerIsGuarding, "Battle Start!",
            basicAttackSkill, fireSkill, CfgMaxBattleLogEntries);

        StartPlayerTurn();
    }

    private void StartPlayerTurn()
    {
        currentState = BattleState.PlayerTurn;
        if (player != null) player.RecoverAp(CfgPlayerApRecovery);
        battleUI?.UpdateActionButtons(player, basicAttackSkill, fireSkill, currentState);
        battleUI?.UpdateAllUI(currentState, player, enemy, enemyPattern, enemyTurnCount,
            currentStageIndex, stageEncounters, playerName, enemyName, totalGoldEarned,
            CfgGuardReductionPercent, CfgBurnTurnDuration, playerIsGuarding,
            $"Player Turn: recovered {CfgPlayerApRecovery} AP. Choose an action.",
            basicAttackSkill, fireSkill, CfgMaxBattleLogEntries);
    }

    // --- Player actions (public for button binding & testing) ---

    public void OnClickAttackButton() => UsePlayerSkill(basicAttackSkill);
    public void OnClickFireSkillButton() => UsePlayerSkill(fireSkill);
    public void OnClickEndTurnButton() => EndPlayerTurn();
    public void OnClickGuardButton() => GuardAndEndPlayerTurn();

    public void OnClickRetryButton()
    {
        if (currentState != BattleState.Victory && currentState != BattleState.Defeat) return;
        StopAllCoroutines();
        StartBattle();
    }

    public void OnClickContinueButton()
    {
        if (currentState != BattleState.Victory || !HasNextStage()) return;
        StopAllCoroutines();
        currentStageIndex++;
        StartBattle();
    }

    private void EndPlayerTurn()
    {
        if (currentState != BattleState.PlayerTurn) return;
        battleUI?.SetActionButtonsInteractable(false);
        battleUI?.UpdateAllUI(currentState, player, enemy, enemyPattern, enemyTurnCount,
            currentStageIndex, stageEncounters, playerName, enemyName, totalGoldEarned,
            CfgGuardReductionPercent, CfgBurnTurnDuration, playerIsGuarding,
            $"{player?.characterName} skipped the turn. You can recover more AP next turn.",
            basicAttackSkill, fireSkill, CfgMaxBattleLogEntries);
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
            basicAttackSkill, fireSkill, CfgMaxBattleLogEntries);
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
                basicAttackSkill, fireSkill, CfgMaxBattleLogEntries);
            battleUI?.UpdateActionButtons(player, basicAttackSkill, fireSkill, currentState);
            return;
        }

        battleUI?.SetActionButtonsInteractable(false);
        skillsUsedCount++;
        int damage = CalculateSkillDamage(enemy, skill);
        int before = enemy.currentHp;
        enemy.TakeDamage(damage);
        TrackDamageDealt(before);

        if (skill.statusEffectType == StatusEffectType.Burn)
            enemy.ApplyStatusEffect(StatusEffectType.Burn, CfgBurnTurnDuration);

        string msg = BuildSkillMessage(skill, damage);
        battleUI?.UpdateAllUI(currentState, player, enemy, enemyPattern, enemyTurnCount,
            currentStageIndex, stageEncounters, playerName, enemyName, totalGoldEarned,
            CfgGuardReductionPercent, CfgBurnTurnDuration, playerIsGuarding, msg,
            basicAttackSkill, fireSkill, CfgMaxBattleLogEntries);

        if (enemy.IsDead()) { EndBattle(BattleState.Victory); return; }
        if (Application.isPlaying) StartCoroutine(EnemyTurnRoutine());
    }

    private IEnumerator EnemyTurnRoutine()
    {
        currentState = BattleState.EnemyTurn;

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
                basicAttackSkill, fireSkill, CfgMaxBattleLogEntries);
            if (enemy.IsDead()) { yield return new WaitForSeconds(1.0f); EndBattle(BattleState.Victory); yield break; }
            yield return new WaitForSeconds(1.0f);
        }
        yield return new WaitForSeconds(1.0f);

        ResolveEnemyAttack();
        if (player != null && player.IsDead()) { EndBattle(BattleState.Defeat); yield break; }
        yield return new WaitForSeconds(1.0f);
        StartPlayerTurn();
    }

    private void ResolveEnemyAttack()
    {
        EnsureEnemyPattern();
        enemyTurnCount++;
        int damage = enemyPattern.GetDamageForTurn(enemyTurnCount);
        bool isStrong = enemyPattern.IsStrongAttackTurn(enemyTurnCount);

        if (playerIsGuarding)
        {
            damage = Mathf.Max(1, damage * (100 - CfgGuardReductionPercent) / 100);
            playerIsGuarding = false;
        }

        int before = player != null ? player.currentHp : 0;
        player?.TakeDamage(damage);
        TrackDamageTaken(before);

        string msg;
        if (isStrong)
            msg = $"{enemy?.characterName} uses {enemyPattern.strongAttackName} on turn {enemyTurnCount}! {player?.characterName}{(playerIsGuarding ? " guards and" : "")} takes {damage} damage.";
        else
            msg = $"{enemy?.characterName} {enemyPattern.normalAttackMessageVerb}! {player?.characterName}{(playerIsGuarding ? " guards and" : "")} takes {damage} damage.";

        battleUI?.UpdateAllUI(currentState, player, enemy, enemyPattern, enemyTurnCount,
            currentStageIndex, stageEncounters, playerName, enemyName, totalGoldEarned,
            CfgGuardReductionPercent, CfgBurnTurnDuration, playerIsGuarding, msg,
            basicAttackSkill, fireSkill, CfgMaxBattleLogEntries);
    }

    private void EndBattle(BattleState resultState)
    {
        currentState = resultState;
        battleUI?.SetActionButtonsInteractable(false);
        battleUI?.SetRetryButtonVisible(true);
        battleUI?.SetContinueButtonVisible(resultState == BattleState.Victory && HasNextStage());

        string resultSummary = BuildResultSummaryText(resultState);
        battleUI?.SetResultSummaryVisible(true, resultSummary);

        string msg = resultState == BattleState.Victory
            ? battleUI?.BuildVictoryGuideMessage(currentStageIndex, stageEncounters) ?? "Victory!"
            : "Defeat... Try again.";
        battleUI?.UpdateAllUI(currentState, player, enemy, enemyPattern, enemyTurnCount,
            currentStageIndex, stageEncounters, playerName, enemyName, totalGoldEarned,
            CfgGuardReductionPercent, CfgBurnTurnDuration, playerIsGuarding, msg,
            basicAttackSkill, fireSkill, CfgMaxBattleLogEntries);
    }

    // --- Damage tracking ---

    private int CalculateSkillDamage(CharacterData target, SkillData skill)
    {
        int dmg = skill.power;
        if (skill.elementType != ElementType.None && skill.elementType == target.weaknessElement)
            dmg += 10;
        return dmg;
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
        string msg = $"{player.characterName} uses {skill.skillName}! {enemy.characterName} takes {damage} damage. ({skill.elementType})";
        if (skill.HasStatusEffect()) msg += $" Extra effect: {skill.statusEffectType} for {CfgBurnTurnDuration} turns.";
        return msg;
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
