using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>Runtime 3v3 battle state. Every combatant has independent CharacterData state.</summary>
public class BattleManager : MonoBehaviour
{
    private enum PresentationKind { Attack, Fire, Ice, Lightning, Guard, Earth }
    [SerializeField] private BattleState currentState;
    [SerializeField] private BattleBalanceConfig balanceConfig;
    [SerializeField] private List<StageData> stageEncounters = new List<StageData>();
    [SerializeField] private BattleUI battleUI;

    // No singleton player/enemy compatibility state: these are the combat model.
    public List<CharacterData> playerParty { get; private set; } = new List<CharacterData>();
    public List<CharacterData> enemyParty { get; private set; } = new List<CharacterData>();
    public int SelectedPlayerIndex { get; private set; } = -1;
    public int SelectedEnemyIndex { get; private set; } = -1;
    public IReadOnlyCollection<int> ActedPlayerIndices => actedPlayerIndices;

    private readonly HashSet<int> actedPlayerIndices = new HashSet<int>();
    private readonly Dictionary<CharacterData, bool> guarding = new Dictionary<CharacterData, bool>();
    private readonly Dictionary<CharacterData, int> earthShield = new Dictionary<CharacterData, int>();
    private readonly List<EnemyPatternData> enemyPatterns = new List<EnemyPatternData>();
    private int currentStageIndex, enemyTurnCount, totalGoldEarned, totalDamageDealt, totalDamageTaken, guardUseCount, skillsUsedCount;
    private bool rewardClaimed;
    private string message = "";
    private string resultSummary = "";
    private SkillData slash, fire, ice, lightning, earth;
    private SkillData pendingTargetSkill;
    private bool isActionResolving, presentationManual, presentationImpactApplied;
    private bool presentationReducesBreak, presentationResetsBreak;
    private int presentationActorIndex = -1, presentationTargetIndex = -1, presentationDamage, impactApplicationCount;
    private PresentationKind presentationKind;
    private SkillData presentationSkill;
    private string presentationCompletionText = "";
    private Coroutine presentationRoutine;

    public BattleState DebugState => currentState;
    public int DebugPlayerPartyCount => playerParty.Count;
    public int DebugEnemyPartyCount => enemyParty.Count;
    public int DebugSelectedPlayerIndex => SelectedPlayerIndex;
    public int DebugSelectedEnemyIndex => SelectedEnemyIndex;
    public bool DebugHasActed(int index) => actedPlayerIndices.Contains(index);
    public int DebugTotalGoldEarned => totalGoldEarned;
    public int DebugTotalDamageDealt => totalDamageDealt;
    public int DebugTotalDamageTaken => totalDamageTaken;
    public int DebugGuardUseCount => guardUseCount;
    public int DebugSkillsUsedCount => skillsUsedCount;
    public string DebugMessageText => battleUI != null ? battleUI.DebugMessageText : message;
    public string DebugResultSummaryText => battleUI != null && !string.IsNullOrEmpty(battleUI.DebugResultSummaryText) ? battleUI.DebugResultSummaryText : resultSummary;
    public string DebugPartyState => battleUI != null ? battleUI.DebugPartyState : "";
    public string DebugTargetState => battleUI != null ? battleUI.DebugTargetState : "";
    public string DebugStageText => battleUI != null ? battleUI.DebugStageText : "";
    public int DebugEnemyTurnCount => enemyTurnCount;
    public bool DebugIsPresentationLocked => isActionResolving;
    public int DebugImpactApplicationCount => impactApplicationCount;
    public void DebugSetPresentationManualForTest(bool manual) { presentationManual = manual; }
    public void DebugAdvancePresentationToImpactForTest() { ApplyPresentationImpact(); }
    public void DebugCompletePresentationForTest() { if (isActionResolving) { ApplyPresentationImpact(); CompleteActionPresentation(); } }
    public void DebugClearEnemyTargetForTest() { SelectedEnemyIndex = -1; pendingTargetSkill = null; message = "Select a target"; RefreshUI(); }
    public void DebugEnterEnemyTurnForTest() { currentState = BattleState.EnemyTurn; RefreshUI(); }

    private int CfgPlayerHp => balanceConfig != null ? balanceConfig.playerMaxHp : 100;
    private int CfgPlayerAttack => balanceConfig != null ? balanceConfig.playerAttack : 20;
    private int CfgPlayerAp => balanceConfig != null ? balanceConfig.playerMaxAp : 3;
    private int CfgRecover => balanceConfig != null ? balanceConfig.playerApRecoveryPerTurn : 2;
    private int CfgGuard => balanceConfig != null ? balanceConfig.guardDamageReductionPercent : 50;
    private int CfgShield => balanceConfig != null ? balanceConfig.earthSkillShieldAmount : 20;
    private int CfgBurn => balanceConfig != null ? balanceConfig.burnDamagePerTurn : 5;
    private int CfgBurnTurns => balanceConfig != null ? balanceConfig.burnTurnDuration : 2;
    private int CfgStunTurns => balanceConfig != null ? balanceConfig.stunTurnDuration : 1;

    private void Start()
    {
        if (stageEncounters == null || stageEncounters.Count == 0)
        {
            int selectedStage = StageSelectController.SelectedStageIndex;
            stageEncounters = StageData.GetEncountersForStage(selectedStage >= 0 ? selectedStage : 0);
        }
        StartBattle();
    }
    public void DebugStartBattleForTest() { StartBattle(); }
    public void DebugLoadEncountersForStage(int stageIndex) { stageEncounters = StageData.GetEncountersForStage(stageIndex); currentStageIndex = 0; }
    public void DebugSetCurrentHpForTest(bool isPlayer, int index, int hp) { List<CharacterData> list = isPlayer ? playerParty : enemyParty; if (index >= 0 && index < list.Count) { list[index].currentHp = Mathf.Max(0, hp); RefreshUI(); } }
    public void DebugSetPlayerApForTest(int index, int ap) { if (index >= 0 && index < playerParty.Count) playerParty[index].currentAp = Mathf.Clamp(ap, 0, playerParty[index].maxAp); RefreshUI(); }
    public bool DebugIsGuarding(int index) => index >= 0 && index < playerParty.Count && guarding.TryGetValue(playerParty[index], out bool value) && value;
    public int DebugShield(int index) => index >= 0 && index < playerParty.Count && earthShield.TryGetValue(playerParty[index], out int value) ? value : 0;

    private void StartBattle()
    {
        CancelActionPresentation();
        if (stageEncounters == null || stageEncounters.Count == 0) stageEncounters = StageData.GetEncountersForStage(0);
        currentStageIndex = Mathf.Clamp(currentStageIndex, 0, stageEncounters.Count - 1);
        StageData stage = stageEncounters[currentStageIndex];
        playerParty = new List<CharacterData> {
            MakePlayer("Paladin", CfgPlayerHp, CfgPlayerAttack, BattleVisualId.HeroPaladin), MakePlayer("Cleric", CfgPlayerHp + 20, Mathf.Max(1, CfgPlayerAttack - 4), BattleVisualId.GuardianCleric), MakePlayer("Ranger", Mathf.Max(1, CfgPlayerHp - 15), CfgPlayerAttack + 3, BattleVisualId.ScoutRanger) };
        enemyParty = new List<CharacterData>(); enemyPatterns.Clear();
        foreach (EnemyData definition in stage.enemies.Take(3)) { enemyParty.Add(new CharacterData(definition.enemyName, definition.maxHp, definition.pattern.normalAttackDamage, definition.weakness, 0, definition.visualId)); enemyPatterns.Add(definition.pattern); }
        guarding.Clear(); earthShield.Clear(); actedPlayerIndices.Clear(); pendingTargetSkill = null; SelectedPlayerIndex = -1; SelectedEnemyIndex = FirstLiving(enemyParty);
        enemyTurnCount = totalDamageDealt = totalDamageTaken = guardUseCount = skillsUsedCount = 0; rewardClaimed = false; resultSummary = "";
        slash = new SkillData("Slash", balanceConfig != null ? balanceConfig.basicSkillPower : 20, balanceConfig != null ? balanceConfig.basicSkillApCost : 0, ElementType.Physical, StatusEffectType.None);
        fire = new SkillData("Fire Bolt", balanceConfig != null ? balanceConfig.fireSkillPower : 30, balanceConfig != null ? balanceConfig.fireSkillApCost : 2, ElementType.Fire, StatusEffectType.Burn);
        ice = new SkillData("Ice Lance", balanceConfig != null ? balanceConfig.iceSkillPower : 25, balanceConfig != null ? balanceConfig.iceSkillApCost : 1, ElementType.Ice, StatusEffectType.Stun);
        lightning = new SkillData("Lightning Strike", balanceConfig != null ? balanceConfig.lightningSkillPower : 40, balanceConfig != null ? balanceConfig.lightningSkillApCost : 3, ElementType.Lightning, StatusEffectType.None);
        earth = new SkillData("Earth Wall", balanceConfig != null ? balanceConfig.earthSkillPower : 22, balanceConfig != null ? balanceConfig.earthSkillApCost : 2, ElementType.Earth, StatusEffectType.None);
        currentState = BattleState.PlayerTurn; message = "Player phase: select a living party unit and target.";
        if (battleUI != null) { battleUI.BindBattleManager(this); battleUI.StartNewBattle(); } RefreshUI();
    }
    private CharacterData MakePlayer(string name, int hp, int attack, BattleVisualId visualId) { return new CharacterData(name, hp, attack, ElementType.None, CfgPlayerAp, visualId); }

    public bool SelectPlayerUnit(int index)
    {
        if (isActionResolving || currentState != BattleState.PlayerTurn || index < 0 || index >= playerParty.Count || playerParty[index].IsDead() || actedPlayerIndices.Contains(index)) return false;
        pendingTargetSkill = null; SelectedPlayerIndex = index; SelectedEnemyIndex = -1; message = $"Selected {playerParty[index].characterName}.";
        if (battleUI != null) battleUI.CloseSkillSubmenu();
        RefreshUI(); return true;
    }
    public bool SelectEnemyTarget(int index)
    {
        if (isActionResolving || currentState != BattleState.PlayerTurn || index < 0 || index >= enemyParty.Count || enemyParty[index].IsDead()) return false;
        SelectedEnemyIndex = index; message = $"Target: {enemyParty[index].characterName}";
        if (pendingTargetSkill != null)
        {
            SkillData queued = pendingTargetSkill;
            pendingTargetSkill = null;
            UseSkill(queued);
        }
        else RefreshUI();
        return true;
    }
    public void OnClickPlayerUnit() { SelectPlayerUnit(FirstAvailablePlayer()); }
    public void OnClickAttackButton()
    {
        if (!CanActorAct()) return;
        if (!HasLivingTarget()) { pendingTargetSkill = slash; message = "Select a target"; RefreshUI(); return; }
        UseSkill(slash);
    }
    public void OnClickFireSkillButton() { BeginTargetedSkill(fire); }
    public void OnClickIceSkillButton() { BeginTargetedSkill(ice); }
    public void OnClickLightningSkillButton() { BeginTargetedSkill(lightning); }
    public void OnClickEarthSkillButton()
    {
        if (!CanUseSkill(earth)) return;
        UseSkill(earth);
    }
    public void OnClickGuardButton()
    {
        if (!CanActorAct()) return;
        CharacterData actor = playerParty[SelectedPlayerIndex];
        BeginActionPresentation(PresentationKind.Guard, null, SelectedPlayerIndex, -1, 0, $"{actor.characterName} guards.");
    }
    public void OnClickEndTurnButton() { if (!isActionResolving && currentState == BattleState.PlayerTurn) { pendingTargetSkill = null; EndPlayerPhase(); } }
    public void OnClickRetryButton() { StartBattle(); }
    public void OnClickContinueButton() { if (currentState == BattleState.Victory && currentStageIndex + 1 < stageEncounters.Count) { currentStageIndex++; StartBattle(); } }
    public void OnClickAutoBattleToggle() { if (SelectPlayerUnit(FirstAvailablePlayer())) { SelectEnemyTarget(FirstLiving(enemyParty)); OnClickAttackButton(); } }
    public void OnClickSpeedToggle() { }
    public void OnClickItemButton() { }
    public void OnClickPauseButton() { }
    public void OnResumeGame() { }
    public void OnClickStageSelectButton() { }

    private bool CanActorAct() => !isActionResolving && currentState == BattleState.PlayerTurn && SelectedPlayerIndex >= 0 && SelectedPlayerIndex < playerParty.Count && !playerParty[SelectedPlayerIndex].IsDead() && !actedPlayerIndices.Contains(SelectedPlayerIndex);
    private bool HasLivingTarget() => SelectedEnemyIndex >= 0 && SelectedEnemyIndex < enemyParty.Count && !enemyParty[SelectedEnemyIndex].IsDead();
    private bool CanUseSkill(SkillData skill)
    {
        if (!CanActorAct() || skill == null) return false;
        CharacterData actor = playerParty[SelectedPlayerIndex];
        if (!ProgressState.IsSkillUnlocked(skill.skillName)) { message = $"{skill.skillName} is locked."; RefreshUI(); return false; }
        if (!actor.HasEnoughAp(skill.apCost)) { message = $"{actor.characterName} lacks AP."; RefreshUI(); return false; }
        return true;
    }
    private void BeginTargetedSkill(SkillData skill)
    {
        if (!CanUseSkill(skill)) return;
        pendingTargetSkill = skill;
        SelectedEnemyIndex = -1;
        message = "Select a target";
        RefreshUI();
        if (battleUI != null) battleUI.SetPendingSkillDescription(DescribeSkill(skill));
    }
    private static string DescribeSkill(SkillData skill)
    {
        if (skill == null) return "";
        if (skill.statusEffectType == StatusEffectType.Burn) return "Fire damage; applies Burn to the selected target.";
        if (skill.statusEffectType == StatusEffectType.Stun) return "Ice damage; applies Stun to the selected target.";
        if (skill.skillName == "Lightning Strike") return "Heavy lightning damage to the selected target.";
        return "";
    }
    private void UseSkill(SkillData skill)
    {
        if (!CanUseSkill(skill)) return;
        CharacterData actor = playerParty[SelectedPlayerIndex];
        if (skill == earth)
        {
            if (!actor.SpendAp(skill.apCost)) return;
            BeginActionPresentation(PresentationKind.Earth, skill, SelectedPlayerIndex, -1, 0, $"{actor.characterName} raises an Earth Wall."); return;
        }
        if (!HasLivingTarget()) { pendingTargetSkill = skill; message = "Select a target"; RefreshUI(); return; }
        CharacterData target = enemyParty[SelectedEnemyIndex];
        if (!actor.SpendAp(skill.apCost)) { message = $"{actor.characterName} lacks AP."; RefreshUI(); return; }
        int damage = skill.power;
        bool reducesBreak = skill.elementType == target.weaknessElement;
        bool resetsBreak = target.isBroken || (reducesBreak && target.currentBreakGauge <= 1);
        if (reducesBreak) damage = Mathf.RoundToInt(damage * (balanceConfig != null ? balanceConfig.weaknessDamageMultiplier : 1.5f));
        if (resetsBreak) damage *= 2;
        PresentationKind kind = skill == slash ? PresentationKind.Attack : skill == fire ? PresentationKind.Fire : skill == ice ? PresentationKind.Ice : PresentationKind.Lightning;
        BeginActionPresentation(kind, skill, SelectedPlayerIndex, SelectedEnemyIndex, damage, $"{actor.characterName} used {skill.skillName} on {target.characterName} for {damage}.", reducesBreak, resetsBreak);
    }

    private void BeginActionPresentation(PresentationKind kind, SkillData skill, int actorIndex, int targetIndex, int damage, string completionText, bool reducesBreak = false, bool resetsBreak = false)
    {
        if (isActionResolving) return;
        isActionResolving = true; presentationImpactApplied = false; impactApplicationCount = 0;
        presentationKind = kind; presentationSkill = skill; presentationActorIndex = actorIndex; presentationTargetIndex = targetIndex;
        presentationDamage = damage; presentationCompletionText = completionText; presentationReducesBreak = reducesBreak; presentationResetsBreak = resetsBreak;
        RefreshUI();
        if (battleUI != null)
        {
            battleUI.SetActionPresentationLocked(true);
            BattleVisualId actorVisual = playerParty[actorIndex].visualId;
            BattleVisualId targetVisual = targetIndex >= 0 ? enemyParty[targetIndex].visualId : actorVisual;
            battleUI.BeginActionFeedback(kind.ToString(), actorVisual, targetVisual);
        }
        if (!presentationManual)
        {
            if (Application.isPlaying) presentationRoutine = StartCoroutine(ActionPresentationRoutine());
            else DebugCompletePresentationForTest();
        }
    }

    private IEnumerator ActionPresentationRoutine()
    {
        yield return new WaitForSeconds(0.22f);
        ApplyPresentationImpact();
        yield return new WaitForSeconds(0.23f);
        CompleteActionPresentation();
    }

    private void ApplyPresentationImpact()
    {
        if (!isActionResolving || presentationImpactApplied) return;
        presentationImpactApplied = true; impactApplicationCount++;
        string popup = "";
        int displayedDamage = 0;
        if (presentationKind == PresentationKind.Guard && presentationActorIndex >= 0 && presentationActorIndex < playerParty.Count)
        {
            CharacterData actor = playerParty[presentationActorIndex];
            if (actor != null && !actor.IsDead()) { guarding[actor] = true; guardUseCount++; popup = "GUARD"; }
        }
        else if (presentationKind == PresentationKind.Earth && presentationActorIndex >= 0 && presentationActorIndex < playerParty.Count)
        {
            CharacterData actor = playerParty[presentationActorIndex];
            if (actor != null && !actor.IsDead()) { earthShield[actor] = CfgShield; skillsUsedCount++; popup = $"SHIELD +{CfgShield}"; }
        }
        else if (presentationTargetIndex >= 0 && presentationTargetIndex < enemyParty.Count && presentationSkill != null)
        {
            CharacterData target = enemyParty[presentationTargetIndex];
            if (target != null && !target.IsDead())
            {
                if (presentationReducesBreak) target.ReduceBreakGauge(1);
                if (presentationResetsBreak) target.ResetBreakGauge();
                target.TakeDamage(presentationDamage); displayedDamage = presentationDamage; totalDamageDealt += presentationDamage;
                if (presentationSkill.statusEffectType == StatusEffectType.Burn) { target.ApplyStatusEffect(StatusEffectType.Burn, CfgBurnTurns); popup = "BURN"; }
                if (presentationSkill.statusEffectType == StatusEffectType.Stun) { target.ApplyStatusEffect(StatusEffectType.Stun, CfgStunTurns); popup = "STUN"; }
                if (presentationSkill != slash) skillsUsedCount++;
            }
        }
        RefreshUI();
        if (battleUI != null)
        {
            battleUI.SetActionPresentationLocked(true);
            battleUI.ShowActionImpact(presentationKind.ToString(), displayedDamage, popup);
        }
    }

    private void CompleteActionPresentation()
    {
        if (!isActionResolving) return;
        ApplyPresentationImpact();
        isActionResolving = false; presentationRoutine = null;
        if (battleUI != null) battleUI.EndActionFeedback();
        FinishPlayerAction(presentationCompletionText);
    }

    private void CancelActionPresentation()
    {
        if (presentationRoutine != null) StopCoroutine(presentationRoutine);
        presentationRoutine = null; isActionResolving = false; presentationImpactApplied = false;
        presentationReducesBreak = presentationResetsBreak = false;
        presentationActorIndex = presentationTargetIndex = -1;
        if (battleUI != null) battleUI.CleanupActionFeedback();
    }
    private void FinishPlayerAction(string text)
    {
        pendingTargetSkill = null; actedPlayerIndices.Add(SelectedPlayerIndex); message = text; if (AllDead(enemyParty)) { EndBattle(BattleState.Victory); return; }
        SelectedPlayerIndex = -1; SelectedEnemyIndex = FirstLiving(enemyParty); if (FirstAvailablePlayer() < 0) EndPlayerPhase(); else RefreshUI();
    }
    private void EndPlayerPhase() { if (currentState != BattleState.PlayerTurn) return; currentState = BattleState.EnemyTurn; ResolveEnemyTurn(); }
    private void ResolveEnemyTurn()
    {
        if (AllDead(playerParty)) { EndBattle(BattleState.Defeat); return; }
        enemyTurnCount++;
        foreach (CharacterData enemy in enemyParty)
        {
            if (enemy.IsDead()) continue;
            if (enemy.HasStatusEffect(StatusEffectType.Burn)) { enemy.TakeDamage(CfgBurn); enemy.ReduceStatusTurn(); if (enemy.IsDead()) continue; }
            if (enemy.HasStatusEffect(StatusEffectType.Stun)) { enemy.ReduceStatusTurn(); continue; }
            int targetIndex = FirstLiving(playerParty); if (targetIndex < 0) break; CharacterData target = playerParty[targetIndex]; int damage = enemy.attackPower;
            if (earthShield.TryGetValue(target, out int shield) && shield > 0) { int absorbed = Mathf.Min(shield, damage); damage -= absorbed; earthShield[target] = shield - absorbed; }
            if (guarding.TryGetValue(target, out bool isGuarding) && isGuarding) { damage = Mathf.FloorToInt(damage * (100 - CfgGuard) / 100f); guarding[target] = false; }
            target.TakeDamage(damage); totalDamageTaken += damage; message = $"{enemy.characterName} attacks {target.characterName} for {damage}.";
            if (AllDead(playerParty)) { EndBattle(BattleState.Defeat); return; }
        }
        if (AllDead(enemyParty)) { EndBattle(BattleState.Victory); return; }
        currentState = BattleState.PlayerTurn; actedPlayerIndices.Clear(); foreach (CharacterData unit in playerParty) if (!unit.IsDead()) unit.RecoverAp(CfgRecover); SelectedPlayerIndex = -1; SelectedEnemyIndex = FirstLiving(enemyParty); message = "Player phase: select a living party unit and target."; RefreshUI();
    }
    public void DebugResolveEnemyAttackForTest() { currentState = BattleState.EnemyTurn; ResolveEnemyTurn(); }
    private static bool AllDead(List<CharacterData> party) => party.Count > 0 && party.All(c => c.IsDead());
    private static int FirstLiving(List<CharacterData> party) => party.FindIndex(c => !c.IsDead());
    private int FirstAvailablePlayer()
    {
        for (int i = 0; i < playerParty.Count; i++)
            if (!playerParty[i].IsDead() && !actedPlayerIndices.Contains(i)) return i;
        return -1;
    }

    private void EndBattle(BattleState result)
    {
        if (presentationRoutine != null) { StopCoroutine(presentationRoutine); presentationRoutine = null; }
        isActionResolving = false;
        if (battleUI != null) battleUI.CleanupActionFeedback();
        currentState = result; message = result == BattleState.Victory ? "Victory: all enemies defeated." : "Defeat: all party members defeated.";
        if (result == BattleState.Victory && !rewardClaimed) { totalGoldEarned += 150; rewardClaimed = true; }
        if (battleUI != null)
        {
            CharacterData hero = playerParty.Count > 0 ? playerParty[0] : new CharacterData("Paladin", 1, 0);
            CharacterData firstEnemy = enemyParty.Count > 0 ? enemyParty[0] : new CharacterData("Enemy", 1, 0);
            resultSummary = new BattleResultData { resultLabel = result == BattleState.Victory ? "Victory" : "Defeat", enemyTurns = enemyTurnCount, playerName = hero.characterName, playerCurrentHp = hero.currentHp, playerMaxHp = hero.maxHp, playerCurrentAp = hero.currentAp, playerMaxAp = hero.maxAp, enemyName = firstEnemy.characterName, enemyCurrentHp = firstEnemy.currentHp, enemyMaxHp = firstEnemy.maxHp, damageDealt = totalDamageDealt, damageTaken = totalDamageTaken, guardUses = guardUseCount, skillsUsed = skillsUsedCount, paceLabel = "Party", survivalLabel = $"{playerParty.Count(c => !c.IsDead())}/{playerParty.Count}", rank = result == BattleState.Victory ? "S" : "C", rewardGold = result == BattleState.Victory ? 150 : 0, totalGold = totalGoldEarned, resultTip = result == BattleState.Victory ? "All enemies defeated." : "Keep the party alive.", lastEnemyPattern = "Party turn" }.BuildSummaryText();
            battleUI.SetResultSummaryVisible(true, resultSummary); battleUI.SetRetryButtonVisible(true); battleUI.SetContinueButtonVisible(result == BattleState.Victory && currentStageIndex + 1 < stageEncounters.Count); battleUI.SetStageSelectButtonVisible(true);
        }
        RefreshUI();
    }
    private void RefreshUI()
    {
        if (battleUI == null) return; StageData stage = stageEncounters != null && currentStageIndex < stageEncounters.Count ? stageEncounters[currentStageIndex] : null;
        battleUI.UpdatePartyUI(currentState, playerParty, enemyParty, SelectedPlayerIndex, SelectedEnemyIndex, actedPlayerIndices, message, stage, enemyTurnCount, guarding, earthShield);
        CharacterData selectedActor = SelectedPlayerIndex >= 0 && SelectedPlayerIndex < playerParty.Count ? playerParty[SelectedPlayerIndex] : null;
        battleUI.UpdateCommandDock(currentState, selectedActor, SelectedPlayerIndex >= 0 && actedPlayerIndices.Contains(SelectedPlayerIndex), slash, fire, ice, lightning, earth);
    }
}
