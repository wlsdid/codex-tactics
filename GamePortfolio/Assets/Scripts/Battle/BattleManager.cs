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
    private int currentStageIndex, selectedStageIndex, enemyTurnCount, totalGoldEarned, totalDamageDealt, totalDamageTaken, guardUseCount, skillsUsedCount;
    private bool rewardClaimed;
    private bool resultPending, stageSelectRequested, resultNavigationLocked;
    private int resultEntryCount, rewardGrantCount;
    private readonly HashSet<int> rewardedEncounterKeys = new HashSet<int>();
    private BattleState pendingResultState;
    private BattleResultData lastResultData;
    private Coroutine resultRoutine;
    private string requestedSceneName = "";
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
    private Coroutine enemyTurnRoutine;
    private readonly List<string> enemyIntents = new List<string>();
    private readonly List<int> enemyActionCounts = new List<int>();
    private readonly List<int> enemyActualTargets = new List<int>();
    private bool enemyTurnManual, enemyImpactApplied, enemyStatusResolved, playerTurnRecovered;
    private int enemyActorIndex = -1, enemyTargetIndex = -1, enemyPendingDamage;
    private int enemyAbsorbedDamage, enemyFinalDamage, enemyImpactCount, playerTurnRecoveryCount;

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
    public bool DebugResultPending => resultPending;
    public int DebugResultEntryCount => resultEntryCount;
    public int DebugRewardGrantCount => rewardGrantCount;
    public int DebugEncounterIndex => currentStageIndex;
    public BattleResultData DebugResultData => lastResultData;
    public bool DebugStageSelectRequested => stageSelectRequested;
    public string DebugRequestedSceneName => requestedSceneName;
    public string DebugMessageText => battleUI != null ? battleUI.DebugMessageText : message;
    public string DebugResultSummaryText => battleUI != null && !string.IsNullOrEmpty(battleUI.DebugResultSummaryText) ? battleUI.DebugResultSummaryText : resultSummary;
    public string DebugPartyState => battleUI != null ? battleUI.DebugPartyState : "";
    public string DebugTargetState => battleUI != null ? battleUI.DebugTargetState : "";
    public string DebugStageText => battleUI != null ? battleUI.DebugStageText : "";
    public int DebugEnemyTurnCount => enemyTurnCount;
    public bool DebugIsPresentationLocked => isActionResolving;
    public int DebugImpactApplicationCount => impactApplicationCount;
    public bool DebugIsEnemyTurnResolving => enemyTurnRoutine != null || (currentState == BattleState.EnemyTurn && enemyActorIndex >= 0);
    public int DebugEnemyActorIndex => enemyActorIndex;
    public int DebugEnemyTargetIndex => enemyTargetIndex;
    public int DebugEnemyImpactCount => enemyImpactCount;
    public int DebugPlayerTurnRecoveryCount => playerTurnRecoveryCount;
    public string DebugEnemyIntent(int index) => index >= 0 && index < enemyIntents.Count ? enemyIntents[index] : "";
    public int DebugEnemyActionCount(int index) => index >= 0 && index < enemyActionCounts.Count ? enemyActionCounts[index] : 0;
    public int DebugEnemyActualTarget(int index) => index >= 0 && index < enemyActualTargets.Count ? enemyActualTargets[index] : -1;
    public void DebugSetPresentationManualForTest(bool manual) { presentationManual = manual; }
    public void DebugAdvancePresentationToImpactForTest() { ApplyPresentationImpact(); }
    public void DebugCompletePresentationForTest() { if (isActionResolving) { ApplyPresentationImpact(); CompleteActionPresentation(); } }
    public void DebugClearEnemyTargetForTest() { SelectedEnemyIndex = -1; pendingTargetSkill = null; message = "Select a target"; RefreshUI(); }
    public void DebugEnterEnemyTurnForTest() { currentState = BattleState.EnemyTurn; RefreshUI(); }
    public void DebugSetEnemyTurnManualForTest(bool manual) { enemyTurnManual = manual; }
    public void DebugBeginEnemyTurnForTest() { if (currentState == BattleState.PlayerTurn) EndPlayerPhase(); }
    public void DebugAdvanceEnemyTurnToImpactForTest() { AdvanceEnemyTurnToImpact(); }
    public void DebugCompleteCurrentEnemyActionForTest() { CompleteCurrentEnemyAction(); }
    public void DebugCompleteEnemyTurnForTest() { while (currentState == BattleState.EnemyTurn && !resultPending) { AdvanceEnemyTurnToImpact(); CompleteCurrentEnemyAction(); } }
    public void DebugApplyStatusForTest(bool isPlayer, int index, StatusEffectType status, int turns) { List<CharacterData> list = isPlayer ? playerParty : enemyParty; if (index >= 0 && index < list.Count) { list[index].ApplyStatusEffect(status, turns); RefreshUI(); } }

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
            selectedStageIndex = selectedStage >= 0 ? selectedStage : 0;
            stageEncounters = StageData.GetEncountersForStage(selectedStageIndex);
        }
        StartBattle();
    }
    public void DebugStartBattleForTest() { StartBattle(); }
    public void DebugLoadEncountersForStage(int stageIndex) { selectedStageIndex = stageIndex; stageEncounters = StageData.GetEncountersForStage(stageIndex); currentStageIndex = 0; rewardedEncounterKeys.Clear(); rewardGrantCount = 0; }
    public void DebugConfigureStageForTest(int stageIndex, int encounterIndex) { selectedStageIndex = stageIndex; stageEncounters = StageData.GetEncountersForStage(stageIndex); currentStageIndex = Mathf.Clamp(encounterIndex, 0, stageEncounters.Count - 1); rewardedEncounterKeys.Clear(); rewardGrantCount = 0; StartBattle(); }
    public void DebugTryEnterResultForTest() { CheckForBattleEnd(); }
    public void DebugShowPendingResultForTest() { CompleteBattleEnd(); }
    public void DebugSetCurrentHpForTest(bool isPlayer, int index, int hp) { List<CharacterData> list = isPlayer ? playerParty : enemyParty; if (index >= 0 && index < list.Count) { list[index].currentHp = Mathf.Max(0, hp); RefreshEnemyIntents(); RefreshUI(); } }
    public void DebugSetPlayerApForTest(int index, int ap) { if (index >= 0 && index < playerParty.Count) playerParty[index].currentAp = Mathf.Clamp(ap, 0, playerParty[index].maxAp); RefreshUI(); }
    public bool DebugIsGuarding(int index) => index >= 0 && index < playerParty.Count && guarding.TryGetValue(playerParty[index], out bool value) && value;
    public int DebugShield(int index) => index >= 0 && index < playerParty.Count && earthShield.TryGetValue(playerParty[index], out int value) ? value : 0;

    private void StartBattle()
    {
        if (resultRoutine != null) StopCoroutine(resultRoutine);
        resultRoutine = null; resultPending = false; resultEntryCount = 0; pendingResultState = BattleState.PlayerTurn; resultNavigationLocked = false;
        stageSelectRequested = false; requestedSceneName = "";
        CancelActionPresentation();
        CancelEnemyTurn();
        if (stageEncounters == null || stageEncounters.Count == 0) stageEncounters = StageData.GetEncountersForStage(0);
        currentStageIndex = Mathf.Clamp(currentStageIndex, 0, stageEncounters.Count - 1);
        StageData stage = stageEncounters[currentStageIndex];
        playerParty = new List<CharacterData> {
            MakePlayer("Paladin", CfgPlayerHp, CfgPlayerAttack, BattleVisualId.HeroPaladin), MakePlayer("Cleric", CfgPlayerHp + 20, Mathf.Max(1, CfgPlayerAttack - 4), BattleVisualId.GuardianCleric), MakePlayer("Ranger", Mathf.Max(1, CfgPlayerHp - 15), CfgPlayerAttack + 3, BattleVisualId.ScoutRanger) };
        enemyParty = new List<CharacterData>(); enemyPatterns.Clear();
        foreach (EnemyData definition in stage.enemies.Take(3)) { enemyParty.Add(new CharacterData(definition.enemyName, definition.maxHp, definition.pattern.normalAttackDamage, definition.weakness, 0, definition.visualId)); enemyPatterns.Add(definition.pattern); }
        guarding.Clear(); earthShield.Clear(); actedPlayerIndices.Clear(); pendingTargetSkill = null; SelectedPlayerIndex = -1; SelectedEnemyIndex = FirstLiving(enemyParty);
        enemyTurnCount = totalDamageDealt = totalDamageTaken = guardUseCount = skillsUsedCount = 0; rewardClaimed = rewardedEncounterKeys.Contains(EncounterRewardKey()); resultSummary = ""; lastResultData = default;
        enemyImpactCount = playerTurnRecoveryCount = 0; playerTurnRecovered = false;
        enemyIntents.Clear(); enemyActionCounts.Clear(); enemyActualTargets.Clear();
        for (int i = 0; i < enemyParty.Count; i++) { enemyIntents.Add(""); enemyActionCounts.Add(0); enemyActualTargets.Add(-1); }
        slash = new SkillData("Slash", balanceConfig != null ? balanceConfig.basicSkillPower : 20, balanceConfig != null ? balanceConfig.basicSkillApCost : 0, ElementType.Physical, StatusEffectType.None);
        fire = new SkillData("Fire Bolt", balanceConfig != null ? balanceConfig.fireSkillPower : 30, balanceConfig != null ? balanceConfig.fireSkillApCost : 2, ElementType.Fire, StatusEffectType.Burn);
        ice = new SkillData("Ice Lance", balanceConfig != null ? balanceConfig.iceSkillPower : 25, balanceConfig != null ? balanceConfig.iceSkillApCost : 1, ElementType.Ice, StatusEffectType.Stun);
        lightning = new SkillData("Lightning Strike", balanceConfig != null ? balanceConfig.lightningSkillPower : 40, balanceConfig != null ? balanceConfig.lightningSkillApCost : 3, ElementType.Lightning, StatusEffectType.None);
        earth = new SkillData("Earth Wall", balanceConfig != null ? balanceConfig.earthSkillPower : 22, balanceConfig != null ? balanceConfig.earthSkillApCost : 2, ElementType.Earth, StatusEffectType.None);
        currentState = BattleState.PlayerTurn; message = "Player phase: select a living party unit and target."; RefreshEnemyIntents();
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
    public void OnClickRetryButton() { if (!resultNavigationLocked && (currentState == BattleState.Victory || currentState == BattleState.Defeat)) { resultNavigationLocked = true; StartBattle(); } }
    public void OnClickContinueButton() { if (!resultNavigationLocked && currentState == BattleState.Victory && currentStageIndex + 1 < stageEncounters.Count) { resultNavigationLocked = true; currentStageIndex++; StartBattle(); } }
    public void OnClickAutoBattleToggle() { if (SelectPlayerUnit(FirstAvailablePlayer())) { SelectEnemyTarget(FirstLiving(enemyParty)); OnClickAttackButton(); } }
    public void OnClickSpeedToggle() { }
    public void OnClickItemButton() { }
    public void OnClickPauseButton() { }
    public void OnResumeGame() { }
    public void OnClickStageSelectButton()
    {
        if (resultNavigationLocked || (currentState != BattleState.Victory && currentState != BattleState.Defeat)) return;
        resultNavigationLocked = true;
        stageSelectRequested = true; requestedSceneName = GameSceneFlow.StageSelectSceneName;
        if (Application.isPlaying) UnityEngine.SceneManagement.SceneManager.LoadScene(GameSceneFlow.StageSelectSceneName);
    }

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
        pendingTargetSkill = null; actedPlayerIndices.Add(SelectedPlayerIndex); message = text; if (AllDead(enemyParty)) { BeginBattleEnd(BattleState.Victory); return; }
        SelectedPlayerIndex = -1; SelectedEnemyIndex = FirstLiving(enemyParty); if (FirstAvailablePlayer() < 0) EndPlayerPhase(); else RefreshUI();
    }
    private void EndPlayerPhase()
    {
        if (currentState != BattleState.PlayerTurn || enemyTurnRoutine != null) return;
        if (AllDead(playerParty)) { BeginBattleEnd(BattleState.Defeat); return; }
        currentState = BattleState.EnemyTurn; enemyTurnCount++; playerTurnRecovered = false; enemyActorIndex = -1;
        SelectedPlayerIndex = SelectedEnemyIndex = -1; RefreshEnemyIntents(); RefreshUI();
        if (battleUI != null) { battleUI.SetActionPresentationLocked(true); battleUI.ShowTurnBanner("ENEMY TURN", new Color(1f, 0.48f, 0.28f), 0.35f); }
        BeginNextEnemyAction();
        if (!enemyTurnManual && Application.isPlaying) enemyTurnRoutine = StartCoroutine(EnemyTurnRoutine());
        else if (!enemyTurnManual) ResolveEnemyTurnImmediately();
    }

    private IEnumerator EnemyTurnRoutine()
    {
        yield return new WaitForSeconds(0.75f);
        while (currentState == BattleState.EnemyTurn && enemyActorIndex >= 0)
        {
            CharacterData enemy = enemyParty[enemyActorIndex];
            if (enemy.HasStatusEffect(StatusEffectType.Burn))
            {
                ApplyBurnTick(enemyActorIndex); yield return new WaitForSeconds(0.18f);
                if (resultPending) yield break;
                if (enemy.IsDead()) { CompleteCurrentEnemyAction(); yield return new WaitForSeconds(0.18f); continue; }
            }
            if (enemy.HasStatusEffect(StatusEffectType.Stun))
            {
                enemy.ReduceStatusTurn(); enemyIntents[enemyActorIndex] = ""; message = $"{enemy.characterName} is stunned.";
                if (battleUI != null) battleUI.ShowEnemyStatusPopup(enemy.visualId, "STUNNED", new Color(0.35f, 0.78f, 1f));
                RefreshUI(); yield return new WaitForSeconds(0.58f); CompleteCurrentEnemyAction(); yield return new WaitForSeconds(0.18f); continue;
            }
            PrepareEnemyAttack();
            if (battleUI != null) battleUI.BeginEnemyActionFeedback(enemy.visualId, playerParty[enemyTargetIndex].visualId);
            yield return new WaitForSeconds(0.32f);
            ApplyEnemyAttackImpact();
            yield return new WaitForSeconds(0.30f);
            CompleteCurrentEnemyAction();
            yield return new WaitForSeconds(0.20f);
        }
        if (currentState == BattleState.EnemyTurn) ReturnToPlayerTurn();
    }

    private bool BeginNextEnemyAction()
    {
        if (currentState != BattleState.EnemyTurn) return false;
        int next = enemyActorIndex + 1;
        while (next < enemyParty.Count && enemyParty[next].IsDead()) { enemyIntents[next] = ""; next++; }
        if (next >= enemyParty.Count) return false;
        enemyActorIndex = next; enemyTargetIndex = -1; enemyImpactApplied = enemyStatusResolved = false; enemyPendingDamage = enemyFinalDamage = enemyAbsorbedDamage = 0;
        RefreshUI(); return true;
    }

    private void PrepareEnemyAttack()
    {
        if (enemyActorIndex < 0 || enemyActorIndex >= enemyParty.Count) return;
        enemyTargetIndex = FirstLiving(playerParty);
        if (enemyTargetIndex < 0) { BeginBattleEnd(BattleState.Defeat); return; }
        EnemyPatternData pattern = enemyPatterns[enemyActorIndex];
        enemyPendingDamage = pattern.GetDamageForTurn(enemyTurnCount);
        enemyActualTargets[enemyActorIndex] = enemyTargetIndex;
        enemyIntents[enemyActorIndex] = BuildEnemyIntent(enemyActorIndex, enemyTargetIndex);
        RefreshUI();
    }

    private void AdvanceEnemyTurnToImpact()
    {
        if (currentState != BattleState.EnemyTurn) return;
        if (enemyActorIndex < 0 && !BeginNextEnemyAction()) { ReturnToPlayerTurn(); return; }
        CharacterData enemy = enemyParty[enemyActorIndex];
        if (enemy.IsDead()) return;
        if (!enemyStatusResolved && enemy.HasStatusEffect(StatusEffectType.Burn))
        {
            ApplyBurnTick(enemyActorIndex);
            if (resultPending || enemy.IsDead()) return;
        }
        if (!enemyStatusResolved && enemy.HasStatusEffect(StatusEffectType.Stun))
        {
            enemy.ReduceStatusTurn(); enemyIntents[enemyActorIndex] = ""; message = $"{enemy.characterName} is stunned.";
            if (battleUI != null) battleUI.ShowEnemyStatusPopup(enemy.visualId, "STUNNED", new Color(0.35f, 0.78f, 1f));
            enemyStatusResolved = true; RefreshUI(); return;
        }
        enemyStatusResolved = true;
        if (enemyTargetIndex < 0) PrepareEnemyAttack();
        if (currentState != BattleState.EnemyTurn) return;
        if (battleUI != null && !enemyImpactApplied) battleUI.BeginEnemyActionFeedback(enemy.visualId, playerParty[enemyTargetIndex].visualId);
        ApplyEnemyAttackImpact();
    }

    private void ApplyBurnTick(int index)
    {
        CharacterData enemy = enemyParty[index];
        int hpBefore = enemy.currentHp;
        enemy.TakeDamage(CfgBurn);
        int burnDamage = Mathf.Max(0, hpBefore - enemy.currentHp);
        totalDamageDealt += burnDamage;
        enemy.ReduceStatusTurn(); enemyStatusResolved = true; message = $"{enemy.characterName} takes Burn damage.";
        if (battleUI != null) battleUI.ShowEnemyStatusPopup(enemy.visualId, $"BURN -{burnDamage}", new Color(1f, 0.35f, 0.12f));
        if (enemy.IsDead()) enemyIntents[index] = "";
        RefreshUI();
        if (AllDead(enemyParty)) BeginBattleEnd(BattleState.Victory);
    }

    private void ApplyEnemyAttackImpact()
    {
        if (enemyImpactApplied || enemyActorIndex < 0 || enemyTargetIndex < 0 || currentState != BattleState.EnemyTurn) return;
        enemyImpactApplied = true; enemyImpactCount++; CharacterData target = playerParty[enemyTargetIndex];
        int damage = enemyPendingDamage; int absorbed = 0; bool guarded = false;
        if (earthShield.TryGetValue(target, out int shield) && shield > 0) { absorbed = Mathf.Min(shield, damage); damage -= absorbed; earthShield[target] = shield - absorbed; }
        if (guarding.TryGetValue(target, out bool isGuarding) && isGuarding) { damage = Mathf.FloorToInt(damage * (100 - CfgGuard) / 100f); guarding[target] = false; guarded = true; }
        enemyAbsorbedDamage = absorbed; enemyFinalDamage = damage; target.TakeDamage(damage); totalDamageTaken += damage;
        enemyActionCounts[enemyActorIndex]++; enemyActualTargets[enemyActorIndex] = enemyTargetIndex; enemyIntents[enemyActorIndex] = "DONE";
        message = $"{enemyParty[enemyActorIndex].characterName} attacks {target.characterName} for {damage}.";
        RefreshUI();
        if (battleUI != null) battleUI.ShowEnemyAttackImpact(damage, absorbed, guarded);
        if (AllDead(playerParty)) BeginBattleEnd(BattleState.Defeat);
    }

    private void CompleteCurrentEnemyAction()
    {
        if (currentState != BattleState.EnemyTurn) return;
        if (battleUI != null) battleUI.EndEnemyActionFeedback();
        int completed = enemyActorIndex; enemyActorIndex = -1; enemyTargetIndex = -1; enemyImpactApplied = enemyStatusResolved = false;
        int next = completed + 1; while (next < enemyParty.Count && enemyParty[next].IsDead()) next++;
        if (next >= enemyParty.Count) { ReturnToPlayerTurn(); return; }
        enemyActorIndex = next - 1; BeginNextEnemyAction();
    }

    private void ResolveEnemyTurnImmediately()
    {
        while (currentState == BattleState.EnemyTurn && enemyActorIndex >= 0) { AdvanceEnemyTurnToImpact(); CompleteCurrentEnemyAction(); }
        if (currentState == BattleState.EnemyTurn) ReturnToPlayerTurn();
    }

    private void ReturnToPlayerTurn()
    {
        if (currentState != BattleState.EnemyTurn || playerTurnRecovered) return;
        playerTurnRecovered = true; playerTurnRecoveryCount++; enemyTurnRoutine = null; currentState = BattleState.PlayerTurn; actedPlayerIndices.Clear();
        foreach (CharacterData unit in playerParty) if (!unit.IsDead()) unit.RecoverAp(CfgRecover);
        SelectedPlayerIndex = -1; SelectedEnemyIndex = FirstLiving(enemyParty); message = "Player phase: select a living party unit and target."; RefreshEnemyIntents(); RefreshUI();
        if (battleUI != null) { battleUI.CleanupActionFeedback(); battleUI.ShowTurnBanner("PLAYER TURN", new Color(0.42f, 0.86f, 1f), 0.35f); }
    }

    private void RefreshEnemyIntents()
    {
        while (enemyIntents.Count < enemyParty.Count) enemyIntents.Add("");
        int target = FirstLiving(playerParty);
        for (int i = 0; i < enemyParty.Count; i++)
        {
            CharacterData enemy = enemyParty[i];
            if (enemy.IsDead() || enemy.HasStatusEffect(StatusEffectType.Stun) || target < 0) enemyIntents[i] = "";
            else if (currentState == BattleState.PlayerTurn) enemyIntents[i] = BuildEnemyIntent(i, target);
        }
    }

    private string BuildEnemyIntent(int enemyIndex, int targetIndex)
    {
        EnemyPatternData pattern = enemyPatterns[enemyIndex]; int turn = enemyTurnCount + (currentState == BattleState.PlayerTurn ? 1 : 0);
        string action = pattern.IsStrongAttackTurn(turn) ? "HEAVY" : "ATTACK";
        return $"{action} → {playerParty[targetIndex].characterName}";
    }

    public void DebugResolveEnemyAttackForTest() { bool old = enemyTurnManual; enemyTurnManual = false; if (currentState != BattleState.PlayerTurn) currentState = BattleState.PlayerTurn; EndPlayerPhase(); enemyTurnManual = old; }

    private void CancelEnemyTurn()
    {
        if (enemyTurnRoutine != null) StopCoroutine(enemyTurnRoutine);
        enemyTurnRoutine = null; enemyActorIndex = enemyTargetIndex = -1; enemyImpactApplied = false; playerTurnRecovered = false;
        if (battleUI != null) { battleUI.EndEnemyActionFeedback(); battleUI.SetActionPresentationLocked(false); battleUI.HideTurnBanner(); }
    }
    private static bool AllDead(List<CharacterData> party) => party.Count > 0 && party.All(c => c.IsDead());
    private static int FirstLiving(List<CharacterData> party) => party.FindIndex(c => !c.IsDead());
    private int FirstAvailablePlayer()
    {
        for (int i = 0; i < playerParty.Count; i++)
            if (!playerParty[i].IsDead() && !actedPlayerIndices.Contains(i)) return i;
        return -1;
    }

    private void CheckForBattleEnd()
    {
        if (AllDead(enemyParty)) BeginBattleEnd(BattleState.Victory);
        else if (AllDead(playerParty)) BeginBattleEnd(BattleState.Defeat);
    }

    private void BeginBattleEnd(BattleState result)
    {
        if (resultPending || currentState == BattleState.Victory || currentState == BattleState.Defeat) return;
        resultPending = true; pendingResultState = result; resultEntryCount++; isActionResolving = true;
        if (enemyTurnRoutine != null) { StopCoroutine(enemyTurnRoutine); enemyTurnRoutine = null; }
        enemyActorIndex = enemyTargetIndex = -1;
        if (presentationRoutine != null) { StopCoroutine(presentationRoutine); presentationRoutine = null; }
        if (battleUI != null) { battleUI.EndActionFeedback(); battleUI.SetActionPresentationLocked(true); battleUI.SetResultSummaryVisible(false, ""); battleUI.SetRetryButtonVisible(false); battleUI.SetContinueButtonVisible(false); battleUI.SetStageSelectButtonVisible(false); }
        SelectedPlayerIndex = SelectedEnemyIndex = -1; pendingTargetSkill = null;
        if (Application.isPlaying) resultRoutine = StartCoroutine(DelayedBattleEnd());
        RefreshUI();
    }

    private IEnumerator DelayedBattleEnd()
    {
        yield return new WaitForSeconds(0.45f);
        CompleteBattleEnd();
    }

    private void CompleteBattleEnd()
    {
        if (!resultPending) return;
        BattleState result = pendingResultState; resultPending = false; resultRoutine = null; isActionResolving = false;
        currentState = result; message = result == BattleState.Victory ? "Victory: all enemies defeated." : "Defeat: all party members defeated.";
        string rank = BattleResultEvaluator.BuildRank(result, enemyTurnCount, totalDamageTaken, balanceConfig);
        int rewardGold = result == BattleState.Victory ? BattleResultEvaluator.BuildRewardGold(rank, balanceConfig != null ? balanceConfig.sRankRewardGold : 150, balanceConfig != null ? balanceConfig.aRankRewardGold : 120, balanceConfig != null ? balanceConfig.bRankRewardGold : 100, 0) : 0;
        int rewardXp = result == BattleState.Victory ? 50 + (selectedStageIndex + 1) * 30 : 0;
        int rewardKey = EncounterRewardKey();
        bool rewardGrantedNow = false;
        if (result == BattleState.Victory && !rewardClaimed && !rewardedEncounterKeys.Contains(rewardKey))
        {
            totalGoldEarned += rewardGold; ProgressState.TotalGold += rewardGold; ProgressState.PlayerXp += rewardXp; rewardClaimed = true; rewardGrantCount++; rewardGrantedNow = true;
            rewardedEncounterKeys.Add(rewardKey);
            if (currentStageIndex + 1 >= stageEncounters.Count) ProgressState.MarkStageCompleted(selectedStageIndex);
            SaveManager.Save();
        }
        lastResultData = new BattleResultData { resultLabel = result == BattleState.Victory ? "Victory" : "Defeat", enemyTurns = enemyTurnCount + 1, damageDealt = totalDamageDealt, damageTaken = totalDamageTaken, guardUses = guardUseCount, skillsUsed = skillsUsedCount, paceLabel = BattleResultEvaluator.BuildPaceLabel(result, enemyTurnCount, balanceConfig), survivalLabel = $"{playerParty.Count(c => !c.IsDead())}/{playerParty.Count}", rank = rank, rewardGold = rewardGrantedNow ? rewardGold : 0, rewardXp = rewardGrantedNow ? rewardXp : 0, totalGold = ProgressState.TotalGold, resultTip = result == BattleState.Victory ? "All enemies defeated." : "Try a different target order.", partySize = playerParty.Count, survivors = playerParty.Count(c => !c.IsDead()), partyRemainingHp = playerParty.Sum(c => Mathf.Max(0, c.currentHp)), enemyWiped = AllDead(enemyParty) };
        resultSummary = lastResultData.BuildSummaryText();
        if (battleUI != null)
        {
            battleUI.CleanupActionFeedback();
            battleUI.SetResultSummaryVisible(true, resultSummary); battleUI.SetRetryButtonVisible(true); battleUI.SetContinueButtonVisible(result == BattleState.Victory && currentStageIndex + 1 < stageEncounters.Count); battleUI.SetStageSelectButtonVisible(result == BattleState.Defeat || (result == BattleState.Victory && currentStageIndex + 1 >= stageEncounters.Count));
            battleUI.SetActionPresentationLocked(true);
        }
        RefreshUI();
    }
    private int EncounterRewardKey() => selectedStageIndex * 100 + currentStageIndex;
    private void RefreshUI()
    {
        if (battleUI == null) return; StageData stage = stageEncounters != null && currentStageIndex < stageEncounters.Count ? stageEncounters[currentStageIndex] : null;
        battleUI.UpdatePartyUI(currentState, playerParty, enemyParty, SelectedPlayerIndex, SelectedEnemyIndex, actedPlayerIndices, message, stage, enemyTurnCount, guarding, earthShield, enemyIntents, enemyActorIndex, enemyTargetIndex);
        CharacterData selectedActor = SelectedPlayerIndex >= 0 && SelectedPlayerIndex < playerParty.Count ? playerParty[SelectedPlayerIndex] : null;
        battleUI.UpdateCommandDock(currentState, selectedActor, SelectedPlayerIndex >= 0 && actedPlayerIndices.Contains(SelectedPlayerIndex), slash, fire, ice, lightning, earth);
    }
}
