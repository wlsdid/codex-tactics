using System.Reflection;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class BattleAutoTestRunner
{
    [MenuItem("Tools/Tactical Requiem/Run Battle Logic Auto Test")]
    public static void RunBattleLogicAutoTest()
    {
        bool passed = true;
        string report = "Battle Logic Auto Test (3v3)\n\n";
        ProgressState.Reset();
        GameObject root = new GameObject("3v3 Battle Test");
        BattleManager battle = root.AddComponent<BattleManager>();
        BattleUI ui = root.AddComponent<BattleUI>();
        SetPrivateField(battle, "battleUI", ui);
        ConfigureBattlefieldSlots(ui);

        battle.DebugStartBattleForTest();
        Check(ref passed, ref report, "exact 3+3 startup names", battle.DebugPlayerPartyCount == 3 && battle.DebugEnemyPartyCount == 3 && battle.playerParty[0].characterName == "Paladin" && battle.playerParty[1].characterName == "Cleric" && battle.playerParty[2].characterName == "Ranger" && battle.enemyParty[0].characterName == "Goblin" && battle.enemyParty[1].characterName == "Skeleton" && battle.enemyParty[2].characterName == "Orc Berserker");
        Check(ref passed, ref report, "stage 1 title matches ruins formation", StageData.CreateStage1Normal().BuildDisplayName() == "Stage 1-1: Ruins Patrol");
        Check(ref passed, ref report, "stage presets contain three distinct EnemyData instances", StageData.CreateStage1Normal().enemies.Count == 3 && !ReferenceEquals(StageData.CreateStage1Normal().enemies[0], StageData.CreateStage1Normal().enemies[1]) && StageData.GetEncountersForStage(5).TrueForAll(stage => stage.enemies.Count == 3));
        Check(ref passed, ref report, "full party state reaches UI", ui.DebugPartyState.Contains("Paladin") && ui.DebugPartyState.Contains("Cleric") && ui.DebugPartyState.Contains("Ranger") && ui.DebugPartyState.Contains("P[") && ui.DebugPartyState.Contains("E["));
        Check(ref passed, ref report, "startup visual IDs are explicit and not name/order derived", battle.playerParty[0].visualId == BattleVisualId.HeroPaladin && battle.playerParty[1].visualId == BattleVisualId.GuardianCleric && battle.playerParty[2].visualId == BattleVisualId.ScoutRanger && battle.enemyParty[0].visualId == BattleVisualId.Goblin && battle.enemyParty[1].visualId == BattleVisualId.Skeleton && battle.enemyParty[2].visualId == BattleVisualId.Orc);
        Check(ref passed, ref report, "six slot bodies resolve six distinct extracted sprite names", ui.DebugAllySlotSpriteName(0) == "ally_paladin" && ui.DebugAllySlotSpriteName(1) == "ally_cleric" && ui.DebugAllySlotSpriteName(2) == "ally_ranger" && ui.DebugEnemySlotSpriteName(0) == "enemy_orc" && ui.DebugEnemySlotSpriteName(1) == "enemy_skeleton" && ui.DebugEnemySlotSpriteName(2) == "enemy_goblin");
        Check(ref passed, ref report, "slot UI resolves visual IDs rather than party indexes", ui.DebugAllySlotCount == 3 && ui.DebugEnemySlotCount == 3 && ui.DebugAllySlotState(0).Contains("Paladin 100/100") && ui.DebugAllySlotState(1).Contains("Cleric 120/120") && ui.DebugAllySlotState(2).Contains("Ranger 85/85") && ui.DebugEnemySlotState(0).Contains("Orc Berserker") && ui.DebugEnemySlotState(1).Contains("Skeleton") && ui.DebugEnemySlotState(2).Contains("Goblin") && !ui.DebugAllySlotState(0).Contains(" HP "));
        Check(ref passed, ref report, "command dock starts hidden without an actor", !ui.DebugCommandDockVisible && !ui.DebugBasicCommandsVisible && !ui.DebugSkillSubmenuVisible);
        int noActorEnemyHp = battle.enemyParty[0].currentHp;
        ui.DebugClickAttackButton();
        Check(ref passed, ref report, "actor-unselected ATTACK is blocked", battle.DebugSelectedPlayerIndex == -1 && battle.enemyParty[0].currentHp == noActorEnemyHp && !battle.DebugHasActed(0) && !battle.DebugHasActed(1) && !battle.DebugHasActed(2));
        RecordState(ref report, "NO_ACTOR_BLOCK", $"actor={battle.DebugSelectedPlayerIndex}; enemy0Hp={noActorEnemyHp}->{battle.enemyParty[0].currentHp}; acted=0,0,0; dock={ui.DebugCommandDockVisible}");
        bool selectedActor = battle.SelectPlayerUnit(1);
        ui.StyleSkillButtons();
        Check(ref passed, ref report, "actionable actor opens contextual command dock", selectedActor && battle.DebugSelectedEnemyIndex == -1 && ui.DebugCommandDockVisible && ui.DebugBasicCommandsVisible && !ui.DebugSkillSubmenuVisible);
        Check(ref passed, ref report, "basic command labels remain exact and unprefixed", ui.DebugBasicCommandLabels == "ATTACK|SKILL|GUARD|END TURN");

        Check(ref passed, ref report, "living unused party selection opens contextual commands", battle.SelectPlayerUnit(1) && battle.DebugSelectedPlayerIndex == 1 && ui.DebugAllySlotSelected(1) && ui.DebugActiveAllyIndicatorCount == 1 && ui.DebugCommandDockVisible && ui.DebugBasicCommandsVisible && ui.DebugActorSummaryText == "Cleric  HP 120/120  AP 3/3");
        Check(ref passed, ref report, "enemy target selection preserves actor command dock", battle.SelectEnemyTarget(0) && ui.DebugCommandDockVisible && ui.DebugBasicCommandsVisible && battle.DebugSelectedPlayerIndex == 1);
        bool dynamicTargets = battle.SelectEnemyTarget(0) && battle.DebugMessageText == "Target: Goblin" && battle.SelectEnemyTarget(1) && battle.DebugMessageText == "Target: Skeleton" && battle.SelectEnemyTarget(2) && battle.DebugMessageText == "Target: Orc Berserker";
        Check(ref passed, ref report, "target message follows each selected CharacterData", dynamicTargets && battle.DebugSelectedEnemyIndex == 2 && ui.DebugEnemySlotTargeted(0) && ui.DebugActiveEnemyIndicatorCount == 1);
        int targetHp = battle.enemyParty[2].currentHp;
        ui.DebugClickAttackButton();
        Check(ref passed, ref report, "ATTACK button is wired to selected actor and target", battle.enemyParty[2].currentHp < targetHp && battle.enemyParty[0].currentHp == battle.enemyParty[0].maxHp && battle.enemyParty[1].currentHp == battle.enemyParty[1].maxHp && battle.DebugHasActed(1) && !ui.DebugCommandDockVisible);
        Check(ref passed, ref report, "repeat action is blocked in same player phase", !battle.SelectPlayerUnit(1) && battle.DebugHasActed(1));
        RecordState(ref report, "DONE_BLOCK", $"actor1Acted={battle.DebugHasActed(1)}; reselect=false; dock={ui.DebugCommandDockVisible}");
        battle.DebugSetCurrentHpForTest(true, 2, 0);
        Check(ref passed, ref report, "dead party slot becomes unavailable and command dock stays closed", !battle.SelectPlayerUnit(2) && !ui.DebugAllySlotInteractable(2) && ui.DebugAllySlotState(2).Contains("DEAD") && !ui.DebugCommandDockVisible);
        RecordState(ref report, "DEAD_BLOCK", $"actor2Hp={battle.playerParty[2].currentHp}; slotInteractable={ui.DebugAllySlotInteractable(2)}; dock={ui.DebugCommandDockVisible}");
        battle.DebugSetCurrentHpForTest(false, 1, 0);
        Check(ref passed, ref report, "dead enemy slot becomes unavailable and visibly dead", !battle.SelectEnemyTarget(1) && !ui.DebugEnemySlotInteractable(1) && ui.DebugEnemySlotState(1).Contains("DEAD"));

        battle.DebugStartBattleForTest();
        battle.SelectPlayerUnit(0); battle.DebugClearEnemyTargetForTest();
        int goblinHpBeforeTarget = battle.enemyParty[0].currentHp;
        ui.DebugClickAttackButton();
        Check(ref passed, ref report, "ATTACK without target requests target and preserves actor", battle.DebugMessageText == "Select a target" && !battle.DebugHasActed(0) && ui.DebugCommandDockVisible && battle.enemyParty[0].currentHp == goblinHpBeforeTarget);
        RecordState(ref report, "ATTACK_WAIT", $"actor=0; target={battle.DebugSelectedEnemyIndex}; goblinHp={goblinHpBeforeTarget}->{battle.enemyParty[0].currentHp}; acted={battle.DebugHasActed(0)}; message={battle.DebugMessageText}");
        battle.SelectEnemyTarget(0);
        Check(ref passed, ref report, "target selection executes pending ATTACK", battle.enemyParty[0].currentHp < goblinHpBeforeTarget && battle.DebugHasActed(0) && !ui.DebugCommandDockVisible);
        RecordState(ref report, "ATTACK_RESOLVE", $"actor=0; target=0; goblinHp={goblinHpBeforeTarget}->{battle.enemyParty[0].currentHp}; acted={battle.DebugHasActed(0)}; dock={ui.DebugCommandDockVisible}");

        battle.DebugStartBattleForTest();
        battle.SelectPlayerUnit(1); int guardTargetBefore = battle.DebugSelectedEnemyIndex; ui.DebugClickGuardButton();
        Check(ref passed, ref report, "GUARD applies only to selected actor and completes action without target", guardTargetBefore == -1 && battle.DebugIsGuarding(1) && !battle.DebugIsGuarding(0) && !battle.DebugIsGuarding(2) && battle.DebugHasActed(1) && !ui.DebugCommandDockVisible);
        RecordState(ref report, "GUARD_SELF", $"targetBefore={guardTargetBefore}; guard=0,{battle.DebugIsGuarding(1)},0; acted1={battle.DebugHasActed(1)}");

        battle.DebugStartBattleForTest();
        battle.SelectPlayerUnit(2); ui.DebugClickSkillMenuButton();
        Check(ref passed, ref report, "SKILL opens compact submenu and hides basic commands", ui.DebugSkillSubmenuVisible && !ui.DebugBasicCommandsVisible);
        Check(ref passed, ref report, "skill submenu shows only names and AP costs without truncation", ui.DebugSkillCommandLabels == "Fire Bolt/AP 2|Ice Lance/AP 1|Earth Wall/AP 2|Lightning Strike/AP 3|BACK");
        Check(ref passed, ref report, "four skill buttons expose pointer hover descriptions", ui.DebugSkillHoverTriggerCount == 4);
        int backActor = battle.DebugSelectedPlayerIndex; int backAp = battle.playerParty[backActor].currentAp;
        ui.DebugClickSkillBackButton();
        Check(ref passed, ref report, "BACK preserves selected actor and AP while returning to basic commands", !ui.DebugSkillSubmenuVisible && ui.DebugBasicCommandsVisible && battle.DebugSelectedPlayerIndex == backActor && battle.playerParty[backActor].currentAp == backAp && !battle.DebugHasActed(backActor));
        RecordState(ref report, "BACK_PRESERVE", $"actor={backActor}->{battle.DebugSelectedPlayerIndex}; ap={backAp}->{battle.playerParty[backActor].currentAp}; acted={battle.DebugHasActed(backActor)}; basic={ui.DebugBasicCommandsVisible}");

        ProgressState.Reset();
        battle.DebugStartBattleForTest(); battle.SelectPlayerUnit(0); ui.DebugClickSkillMenuButton();
        Check(ref passed, ref report, "locked skills are disabled", !ui.DebugFireSkillInteractable && !ui.DebugIceSkillInteractable && !ui.DebugEarthSkillInteractable && !ui.DebugLightningSkillInteractable);
        UnlockAllSkills();

        battle.DebugStartBattleForTest(); battle.SelectPlayerUnit(2); battle.DebugSetPlayerApForTest(2, 0); ui.DebugClickSkillMenuButton();
        int apBlockHp = battle.enemyParty[0].currentHp;
        Check(ref passed, ref report, "insufficient AP skill is dimmed and click-blocked", !ui.DebugIceSkillInteractable && !ui.DebugClickIceSkillButton() && !battle.DebugHasActed(2) && battle.playerParty[2].currentAp == 0 && battle.enemyParty[0].currentHp == apBlockHp);
        RecordState(ref report, "AP_BLOCK", $"actor=2; ap={battle.playerParty[2].currentAp}; iceInteractable={ui.DebugIceSkillInteractable}; enemy0Hp={apBlockHp}->{battle.enemyParty[0].currentHp}; acted={battle.DebugHasActed(2)}");

        battle.DebugStartBattleForTest(); battle.SelectPlayerUnit(2); battle.DebugClearEnemyTargetForTest(); ui.DebugClickSkillMenuButton();
        int rangerAp = battle.playerParty[2].currentAp; int skeletonHpBeforeSkill = battle.enemyParty[1].currentHp;
        Check(ref passed, ref report, "Ice Lance button arms target selection without spending AP or dealing damage", ui.DebugClickIceSkillButton() && battle.DebugMessageText == "Select a target" && ui.DebugCommandDockVisible && !battle.DebugHasActed(2) && battle.playerParty[2].currentAp == rangerAp && battle.enemyParty[1].currentHp == skeletonHpBeforeSkill && !battle.enemyParty[1].HasStatusEffect(StatusEffectType.Stun));
        Check(ref passed, ref report, "selected skill exposes one concise description", ui.DebugSkillDescriptionVisible && ui.DebugSkillDescriptionText.Contains("Stun") && ui.DebugSkillDescriptionText.Length <= 60);
        RecordState(ref report, "SKILL_WAIT", $"actor=2; target={battle.DebugSelectedEnemyIndex}; rangerAp={rangerAp}->{battle.playerParty[2].currentAp}; skeletonHp={skeletonHpBeforeSkill}->{battle.enemyParty[1].currentHp}; stun={battle.enemyParty[1].HasStatusEffect(StatusEffectType.Stun)}; acted={battle.DebugHasActed(2)}");
        battle.SelectEnemyTarget(1);
        Check(ref passed, ref report, "Ice Lance spends selected actor AP, damages and stuns selected target", battle.playerParty[2].currentAp < rangerAp && battle.enemyParty[1].currentHp < skeletonHpBeforeSkill && battle.enemyParty[1].HasStatusEffect(StatusEffectType.Stun) && !battle.enemyParty[0].HasStatusEffect(StatusEffectType.Stun) && battle.DebugHasActed(2) && !ui.DebugCommandDockVisible);
        RecordState(ref report, "SKILL_RESOLVE", $"actor=2; target=1; rangerAp={rangerAp}->{battle.playerParty[2].currentAp}; skeletonHp={skeletonHpBeforeSkill}->{battle.enemyParty[1].currentHp}; stun={battle.enemyParty[1].HasStatusEffect(StatusEffectType.Stun)}; acted={battle.DebugHasActed(2)}");

        battle.DebugStartBattleForTest(); battle.SelectPlayerUnit(1); ui.DebugClickSkillMenuButton();
        int clericAp = battle.playerParty[1].currentAp; int earthTargetBefore = battle.DebugSelectedEnemyIndex;
        Check(ref passed, ref report, "Earth Wall targets actor self and completes action without enemy target", earthTargetBefore == -1 && ui.DebugClickEarthSkillButton() && battle.playerParty[1].currentAp < clericAp && battle.DebugShield(1) > 0 && battle.DebugShield(0) == 0 && battle.DebugShield(2) == 0 && battle.DebugHasActed(1));
        RecordState(ref report, "EARTH_WALL_SELF", $"targetBefore={earthTargetBefore}; clericAp={clericAp}->{battle.playerParty[1].currentAp}; shields={battle.DebugShield(0)},{battle.DebugShield(1)},{battle.DebugShield(2)}; acted1={battle.DebugHasActed(1)}");

        battle.DebugStartBattleForTest(); battle.SelectPlayerUnit(0);
        bool actorsStillAvailable = !battle.DebugHasActed(0) && !battle.DebugHasActed(1) && !battle.DebugHasActed(2);
        int enemyTurns = battle.DebugEnemyTurnCount; ui.DebugClickEndTurnButton();
        Check(ref passed, ref report, "END TURN immediately resolves enemy phase while actors remain", actorsStillAvailable && battle.DebugEnemyTurnCount == enemyTurns + 1 && battle.DebugSelectedPlayerIndex == -1 && !ui.DebugCommandDockVisible);
        RecordState(ref report, "END_TURN", $"remainingBefore={actorsStillAvailable}; enemyTurns={enemyTurns}->{battle.DebugEnemyTurnCount}; stateAfter={battle.DebugState}; actorAfter={battle.DebugSelectedPlayerIndex}");
        battle.DebugStartBattleForTest(); battle.SelectPlayerUnit(0); battle.DebugEnterEnemyTurnForTest();
        Check(ref passed, ref report, "enemy turn hides and disables every command", !ui.DebugCommandDockVisible && !ui.DebugAnyCommandInteractable);
        RecordState(ref report, "ENEMY_TURN_BLOCK", $"state={battle.DebugState}; dock={ui.DebugCommandDockVisible}; anyInteractable={ui.DebugAnyCommandInteractable}");

        battle.DebugStartBattleForTest();
        battle.SelectPlayerUnit(0); battle.OnClickFireSkillButton(); battle.SelectEnemyTarget(2);
        Check(ref passed, ref report, "burn applies only to selected enemy", battle.enemyParty[2].HasStatusEffect(StatusEffectType.Burn) && !battle.enemyParty[0].HasStatusEffect(StatusEffectType.Burn) && !battle.enemyParty[1].HasStatusEffect(StatusEffectType.Burn));
        battle.DebugStartBattleForTest();
        battle.SelectPlayerUnit(0); battle.OnClickIceSkillButton(); battle.SelectEnemyTarget(1);
        Check(ref passed, ref report, "stun applies only to selected enemy", battle.enemyParty[1].HasStatusEffect(StatusEffectType.Stun) && !battle.enemyParty[0].HasStatusEffect(StatusEffectType.Stun) && !battle.enemyParty[2].HasStatusEffect(StatusEffectType.Stun));
        battle.DebugStartBattleForTest();
        battle.SelectPlayerUnit(0); battle.OnClickEarthSkillButton();
        Check(ref passed, ref report, "Earth shield is per acting unit", battle.DebugShield(0) > 0 && battle.DebugShield(1) == 0 && battle.DebugShield(2) == 0);
        battle.DebugStartBattleForTest();
        battle.SelectPlayerUnit(0); battle.OnClickGuardButton();
        int heroHp = battle.playerParty[0].currentHp; int guardianHp = battle.playerParty[1].currentHp;
        battle.DebugResolveEnemyAttackForTest();
        Check(ref passed, ref report, "guard is per-unit and enemy targets living lowest index", battle.playerParty[0].currentHp < heroHp && battle.playerParty[1].currentHp == guardianHp && !battle.DebugIsGuarding(0));

        battle.DebugStartBattleForTest();
        battle.DebugSetPresentationManualForTest(true);
        battle.SelectPlayerUnit(0); battle.SelectEnemyTarget(0);
        int impactHpBefore = battle.enemyParty[0].currentHp;
        int impactEnemyTurnsBefore = battle.DebugEnemyTurnCount;
        battle.OnClickAttackButton();
        Check(ref passed, ref report, "ATTACK presentation locks input before impact", battle.DebugIsPresentationLocked && battle.enemyParty[0].currentHp == impactHpBefore && !battle.DebugHasActed(0) && !ui.DebugAnyCommandInteractable);
        battle.OnClickAttackButton(); battle.OnClickEndTurnButton(); battle.SelectPlayerUnit(1); battle.SelectEnemyTarget(1);
        Check(ref passed, ref report, "presentation lock blocks duplicate command, END TURN and reselection", battle.enemyParty[0].currentHp == impactHpBefore && battle.DebugEnemyTurnCount == impactEnemyTurnsBefore && battle.DebugSelectedPlayerIndex == 0 && battle.DebugSelectedEnemyIndex == 0);
        battle.DebugAdvancePresentationToImpactForTest();
        int impactHpAfter = battle.enemyParty[0].currentHp;
        Check(ref passed, ref report, "ATTACK applies damage exactly once at impact", impactHpAfter < impactHpBefore && battle.DebugImpactApplicationCount == 1 && !battle.DebugHasActed(0));
        battle.DebugAdvancePresentationToImpactForTest();
        Check(ref passed, ref report, "duplicate impact completion cannot apply damage twice", battle.enemyParty[0].currentHp == impactHpAfter && battle.DebugImpactApplicationCount == 1);
        battle.DebugCompletePresentationForTest();
        Check(ref passed, ref report, "ATTACK presentation completion marks actor DONE and unlocks battle", !battle.DebugIsPresentationLocked && battle.DebugHasActed(0) && !ui.DebugCommandDockVisible);
        RecordState(ref report, "ATTACK_PRESENTATION", $"hp={impactHpBefore}->{impactHpAfter}; impacts={battle.DebugImpactApplicationCount}; acted={battle.DebugHasActed(0)}; locked={battle.DebugIsPresentationLocked}");
        battle.DebugSetPresentationManualForTest(false);

        UnlockAllSkills();
        battle.DebugStartBattleForTest(); battle.DebugSetPresentationManualForTest(true);
        battle.SelectPlayerUnit(0); int fireApBefore = battle.playerParty[0].currentAp; int fireHpBefore = battle.enemyParty[0].currentHp;
        battle.OnClickFireSkillButton(); battle.SelectEnemyTarget(0);
        Check(ref passed, ref report, "Fire Bolt locks during flight and spends AP without early damage", battle.DebugIsPresentationLocked && battle.playerParty[0].currentAp == fireApBefore - 2 && battle.enemyParty[0].currentHp == fireHpBefore);
        battle.DebugAdvancePresentationToImpactForTest();
        Check(ref passed, ref report, "Fire Bolt impact damages once and applies Burn popup", battle.enemyParty[0].currentHp < fireHpBefore && battle.enemyParty[0].HasStatusEffect(StatusEffectType.Burn) && battle.DebugImpactApplicationCount == 1 && ui.DebugFeedbackKind == "Fire" && ui.DebugFeedbackPopup == "BURN");
        battle.DebugCompletePresentationForTest();
        Check(ref passed, ref report, "Fire Bolt completes actor action", battle.DebugHasActed(0) && !battle.DebugIsPresentationLocked);
        Check(ref passed, ref report, "Burn persistent overlay returns after transient feedback", ui.DebugEnemyOverlayActive(2));

        battle.DebugStartBattleForTest(); battle.DebugSetPresentationManualForTest(true);
        battle.enemyParty[0].weaknessElement = ElementType.Fire; battle.enemyParty[0].currentBreakGauge = 1; battle.enemyParty[0].isBroken = false;
        battle.SelectPlayerUnit(0); battle.OnClickFireSkillButton(); battle.SelectEnemyTarget(0);
        Check(ref passed, ref report, "weakness Break gauge does not change before impact", battle.enemyParty[0].currentBreakGauge == 1 && !battle.enemyParty[0].isBroken);
        battle.DebugAdvancePresentationToImpactForTest();
        Check(ref passed, ref report, "weakness Break reduction and reset occur atomically at impact", battle.enemyParty[0].currentBreakGauge == battle.enemyParty[0].maxBreakGauge && !battle.enemyParty[0].isBroken);
        battle.DebugCompletePresentationForTest();

        battle.DebugStartBattleForTest(); battle.DebugSetPresentationManualForTest(true);
        battle.SelectPlayerUnit(2); int iceApBefore = battle.playerParty[2].currentAp; int iceHpBefore = battle.enemyParty[1].currentHp;
        battle.OnClickIceSkillButton(); battle.SelectEnemyTarget(1); battle.DebugAdvancePresentationToImpactForTest();
        Check(ref passed, ref report, "Ice Lance impact spends AP, damages once and applies Stun popup", battle.playerParty[2].currentAp == iceApBefore - 1 && battle.enemyParty[1].currentHp < iceHpBefore && battle.enemyParty[1].HasStatusEffect(StatusEffectType.Stun) && battle.DebugImpactApplicationCount == 1 && ui.DebugFeedbackPopup == "STUN");
        battle.DebugCompletePresentationForTest();

        battle.DebugStartBattleForTest(); battle.DebugSetPresentationManualForTest(true);
        battle.SelectPlayerUnit(0); int lightningApBefore = battle.playerParty[0].currentAp; int lightningHpBefore = battle.enemyParty[2].currentHp;
        battle.OnClickLightningSkillButton(); battle.SelectEnemyTarget(2); battle.DebugAdvancePresentationToImpactForTest();
        Check(ref passed, ref report, "Lightning Strike impact spends AP and damages exactly once", battle.playerParty[0].currentAp == lightningApBefore - 3 && battle.enemyParty[2].currentHp < lightningHpBefore && battle.DebugImpactApplicationCount == 1 && ui.DebugFeedbackKind == "Lightning");
        battle.DebugCompletePresentationForTest();

        battle.DebugStartBattleForTest(); battle.DebugSetPresentationManualForTest(true);
        battle.SelectPlayerUnit(1); battle.OnClickGuardButton();
        Check(ref passed, ref report, "GUARD waits for feedback impact", battle.DebugIsPresentationLocked && !battle.DebugIsGuarding(1));
        battle.DebugAdvancePresentationToImpactForTest();
        Check(ref passed, ref report, "GUARD impact applies only to selected actor", battle.DebugIsGuarding(1) && !battle.DebugIsGuarding(0) && !battle.DebugIsGuarding(2) && ui.DebugFeedbackPopup == "GUARD");
        battle.DebugCompletePresentationForTest();

        battle.DebugStartBattleForTest(); battle.DebugSetPresentationManualForTest(true);
        battle.SelectPlayerUnit(1); int wallApBefore = battle.playerParty[1].currentAp; battle.OnClickEarthSkillButton();
        battle.DebugAdvancePresentationToImpactForTest();
        Check(ref passed, ref report, "Earth Wall impact needs no enemy and shields only selected actor", battle.DebugSelectedEnemyIndex == -1 && battle.playerParty[1].currentAp == wallApBefore - 2 && battle.DebugShield(1) == 20 && battle.DebugShield(0) == 0 && battle.DebugShield(2) == 0 && ui.DebugFeedbackPopup == "SHIELD +20");
        battle.DebugCompletePresentationForTest();

        battle.DebugStartBattleForTest(); battle.DebugSetPresentationManualForTest(true);
        battle.DebugSetCurrentHpForTest(false, 1, 0); battle.DebugSetCurrentHpForTest(false, 2, 0); battle.DebugSetCurrentHpForTest(false, 0, 1);
        battle.SelectPlayerUnit(0); battle.SelectEnemyTarget(0); battle.OnClickAttackButton();
        Check(ref passed, ref report, "lethal presentation caches target before indicator is disabled", ui.DebugHasCachedFeedbackTarget);
        battle.DebugAdvancePresentationToImpactForTest();
        Check(ref passed, ref report, "lethal impact keeps cached target feedback after death", battle.enemyParty[0].IsDead() && ui.DebugHasCachedFeedbackTarget && ui.DebugFeedbackPopup == "-20");
        Check(ref passed, ref report, "last target death waits for presentation completion before Victory", battle.enemyParty[0].IsDead() && battle.DebugState == BattleState.PlayerTurn && battle.DebugIsPresentationLocked);
        battle.DebugCompletePresentationForTest();
        Check(ref passed, ref report, "last target death enters Victory after presentation", battle.DebugState == BattleState.Victory && battle.DebugResultSummaryText.Contains("Victory"));

        battle.DebugStartBattleForTest(); battle.DebugSetPresentationManualForTest(true);
        battle.SelectPlayerUnit(0); battle.SelectEnemyTarget(0); battle.OnClickAttackButton(); battle.DebugCompletePresentationForTest();
        Check(ref passed, ref report, "next living actor is selectable after presentation completes", battle.SelectPlayerUnit(1) && battle.DebugSelectedPlayerIndex == 1);
        battle.DebugSetPresentationManualForTest(false);

        int goldBeforeVictorySequence = battle.DebugTotalGoldEarned;
        battle.DebugStartBattleForTest();
        battle.DebugSetCurrentHpForTest(false, 0, 1);
        battle.SelectPlayerUnit(0); battle.SelectEnemyTarget(0); battle.OnClickAttackButton();
        Check(ref passed, ref report, "dead enemy does not end fight while enemies live", battle.enemyParty[0].IsDead() && battle.DebugState != BattleState.Victory && battle.DebugEnemyPartyCount == 3);
        battle.DebugSetCurrentHpForTest(false, 1, 1); battle.DebugSetCurrentHpForTest(false, 2, 1);
        battle.SelectPlayerUnit(1); battle.SelectEnemyTarget(1); battle.OnClickAttackButton();
        battle.SelectPlayerUnit(2); battle.SelectEnemyTarget(2); battle.OnClickAttackButton();
        RecordState(ref report, "VICTORY_DIAG", $"state={battle.DebugState}; locked={battle.DebugIsPresentationLocked}; acted={battle.DebugHasActed(0)},{battle.DebugHasActed(1)},{battle.DebugHasActed(2)}; hp={battle.enemyParty[0].currentHp},{battle.enemyParty[1].currentHp},{battle.enemyParty[2].currentHp}; reward={battle.DebugTotalGoldEarned}");
        Check(ref passed, ref report, "victory only after all enemies die", battle.DebugState == BattleState.Victory && battle.enemyParty.TrueForAll(unit => unit.IsDead()) && battle.DebugResultSummaryText.Contains("Victory") && battle.DebugTotalGoldEarned == goldBeforeVictorySequence + 150);
        int reward = battle.DebugTotalGoldEarned; battle.OnClickRetryButton();
        Check(ref passed, ref report, "retry preserves reward flow without duplicate reward", battle.DebugTotalGoldEarned == reward && battle.DebugState == BattleState.PlayerTurn);

        battle.DebugStartBattleForTest();
        battle.DebugSetCurrentHpForTest(true, 0, 0); battle.DebugSetCurrentHpForTest(true, 1, 0); battle.DebugSetCurrentHpForTest(true, 2, 0);
        battle.DebugResolveEnemyAttackForTest();
        Check(ref passed, ref report, "defeat only after all party members die", battle.DebugState == BattleState.Defeat && battle.playerParty.TrueForAll(unit => unit.IsDead()));
        Check(ref passed, ref report, "UI exposes selection debug strings", ui.DebugTargetState.Contains("actor=") && ui.DebugTargetState.Contains("target="));

        Object.DestroyImmediate(root);
        report += passed ? "\nRESULT: PASS" : "\nRESULT: FAIL";
        Debug.Log(report);
        if (!passed) throw new System.Exception(report);
    }

    [MenuItem("Tools/Tactical Requiem/Run Enemy Turn QA")]
    public static void RunEnemyTurnQA()
    {
        bool passed = true;
        string report = "Enemy Turn Intent + Sequential Presentation QA\n\n";
        ProgressState.Reset();
        GameObject root = new GameObject("Enemy Turn QA");
        BattleManager battle = root.AddComponent<BattleManager>();
        BattleUI ui = root.AddComponent<BattleUI>();
        SetPrivateField(battle, "battleUI", ui); ConfigureBattlefieldSlots(ui); battle.DebugStartBattleForTest();

        Check(ref passed, ref report, "three living enemies expose real first-turn ATTACK intents", battle.DebugEnemyIntent(0) == "ATTACK → Paladin" && battle.DebugEnemyIntent(1) == "ATTACK → Paladin" && battle.DebugEnemyIntent(2) == "ATTACK → Paladin");
        Check(ref passed, ref report, "intent labels map to all visual slots at >=14px", ui.DebugEnemySlotIntent(0).Contains("Paladin") && ui.DebugEnemySlotIntent(1).Contains("Paladin") && ui.DebugEnemySlotIntent(2).Contains("Paladin") && ui.DebugEnemyIntentFontSize(0) >= 14f && ui.DebugEnemyIntentFontSize(1) >= 14f && ui.DebugEnemyIntentFontSize(2) >= 14f);
        battle.DebugSetCurrentHpForTest(true, 0, 0);
        Check(ref passed, ref report, "intent target updates when the planned target dies", battle.DebugEnemyIntent(0) == "ATTACK → Cleric" && battle.DebugEnemyIntent(1) == "ATTACK → Cleric" && battle.DebugEnemyIntent(2) == "ATTACK → Cleric");

        battle.DebugStartBattleForTest(); battle.DebugSetEnemyTurnManualForTest(true); battle.DebugSetPlayerApForTest(0, 0);
        int hpBefore = battle.playerParty[0].currentHp; battle.DebugBeginEnemyTurnForTest();
        Check(ref passed, ref report, "enemy phase locks commands before impact", battle.DebugState == BattleState.EnemyTurn && battle.DebugEnemyActorIndex == 0 && battle.playerParty[0].currentHp == hpBefore && !ui.DebugAnyCommandInteractable && !ui.DebugCommandDockVisible);
        battle.DebugAdvanceEnemyTurnToImpactForTest(); int hpAfter = battle.playerParty[0].currentHp;
        Check(ref passed, ref report, "intent target equals actual target and HP changes exactly at impact", hpAfter < hpBefore && battle.DebugEnemyActualTarget(0) == 0 && battle.DebugEnemyActionCount(0) == 1 && battle.DebugEnemyImpactCount == 1 && battle.DebugEnemyIntent(0) == "DONE");
        battle.DebugAdvanceEnemyTurnToImpactForTest();
        Check(ref passed, ref report, "duplicate impact hook is idempotent", battle.playerParty[0].currentHp == hpAfter && battle.DebugEnemyImpactCount == 1 && battle.DebugEnemyActionCount(0) == 1);
        battle.DebugCompleteCurrentEnemyActionForTest(); battle.DebugAdvanceEnemyTurnToImpactForTest(); battle.DebugCompleteCurrentEnemyActionForTest(); battle.DebugAdvanceEnemyTurnToImpactForTest(); battle.DebugCompleteCurrentEnemyActionForTest();
        Check(ref passed, ref report, "three enemies act once in order then return to PlayerTurn", battle.DebugEnemyActionCount(0) == 1 && battle.DebugEnemyActionCount(1) == 1 && battle.DebugEnemyActionCount(2) == 1 && battle.DebugState == BattleState.PlayerTurn);
        Check(ref passed, ref report, "AP recovery and PlayerTurn transition occur exactly once", battle.playerParty[0].currentAp == 2 && battle.DebugPlayerTurnRecoveryCount == 1 && ui.DebugTurnBannerText == "PLAYER TURN");

        battle.DebugStartBattleForTest(); battle.DebugSetEnemyTurnManualForTest(true); battle.DebugSetCurrentHpForTest(false, 0, 0); battle.DebugApplyStatusForTest(false, 1, StatusEffectType.Stun, 1); battle.DebugBeginEnemyTurnForTest();
        Check(ref passed, ref report, "dead first enemy is skipped before selecting actor", battle.DebugEnemyActorIndex == 1 && string.IsNullOrEmpty(battle.DebugEnemyIntent(0)) && string.IsNullOrEmpty(battle.DebugEnemyIntent(1)));
        battle.DebugAdvanceEnemyTurnToImpactForTest(); battle.DebugCompleteCurrentEnemyActionForTest();
        Check(ref passed, ref report, "stunned enemy shows STUNNED and does not attack", battle.DebugEnemyActionCount(1) == 0 && ui.DebugFeedbackPopup == "STUNNED");
        battle.DebugAdvanceEnemyTurnToImpactForTest(); battle.DebugCompleteCurrentEnemyActionForTest();
        Check(ref passed, ref report, "later living enemy still attacks after dead/stun skips", battle.DebugEnemyActionCount(2) == 1 && battle.DebugState == BattleState.PlayerTurn);

        battle.DebugStartBattleForTest(); battle.DebugSetEnemyTurnManualForTest(true); battle.DebugApplyStatusForTest(false, 0, StatusEffectType.Burn, 2); int burnHp = battle.enemyParty[0].currentHp; battle.DebugBeginEnemyTurnForTest(); battle.DebugAdvanceEnemyTurnToImpactForTest();
        Check(ref passed, ref report, "Burn ticks at resolution then surviving enemy attacks", battle.enemyParty[0].currentHp == burnHp - 5 && battle.DebugEnemyActionCount(0) == 1 && battle.DebugEnemyImpactCount == 1);

        battle.DebugStartBattleForTest(); battle.DebugSetEnemyTurnManualForTest(true); battle.DebugSetCurrentHpForTest(false, 0, 5); battle.DebugApplyStatusForTest(false, 0, StatusEffectType.Burn, 1); battle.DebugBeginEnemyTurnForTest(); battle.DebugAdvanceEnemyTurnToImpactForTest();
        Check(ref passed, ref report, "Burn lethal tick suppresses that enemy attack", battle.enemyParty[0].IsDead() && battle.DebugEnemyActionCount(0) == 0 && string.IsNullOrEmpty(battle.DebugEnemyIntent(0)) && ui.DebugFeedbackPopup == "BURN -5");

        battle.DebugStartBattleForTest(); battle.DebugSetEnemyTurnManualForTest(true); battle.SelectPlayerUnit(0); battle.OnClickGuardButton(); int guardedHp = battle.playerParty[0].currentHp; battle.DebugBeginEnemyTurnForTest(); battle.DebugAdvanceEnemyTurnToImpactForTest();
        Check(ref passed, ref report, "Guard halves final damage and is consumed by one attack", guardedHp - battle.playerParty[0].currentHp == 7 && !battle.DebugIsGuarding(0) && ui.DebugFeedbackPopup == "GUARD");

        UnlockAllSkills(); battle.DebugStartBattleForTest(); battle.DebugSetEnemyTurnManualForTest(true); battle.SelectPlayerUnit(0); battle.OnClickEarthSkillButton(); int shieldHp = battle.playerParty[0].currentHp; battle.DebugBeginEnemyTurnForTest(); battle.DebugAdvanceEnemyTurnToImpactForTest();
        Check(ref passed, ref report, "Earth Wall reports absorbed amount and retains remaining shield", battle.playerParty[0].currentHp == shieldHp && battle.DebugShield(0) == 5 && ui.DebugFeedbackPopup == "BLOCK 15");

        battle.DebugStartBattleForTest(); battle.DebugSetEnemyTurnManualForTest(true); for (int i = 0; i < 3; i++) battle.DebugSetCurrentHpForTest(true, i, 1); battle.DebugBeginEnemyTurnForTest(); battle.DebugCompleteEnemyTurnForTest();
        Check(ref passed, ref report, "enemy attacks stop on party wipe and enter Defeat", battle.DebugState == BattleState.Defeat && battle.playerParty.TrueForAll(unit => unit.IsDead()));

        battle.DebugStartBattleForTest(); battle.DebugSetEnemyTurnManualForTest(true); battle.DebugBeginEnemyTurnForTest(); battle.DebugAdvanceEnemyTurnToImpactForTest(); battle.DebugStartBattleForTest();
        Check(ref passed, ref report, "restart cleans enemy coroutine/VFX lock and restores fresh intents", battle.DebugState == BattleState.PlayerTurn && !battle.DebugIsEnemyTurnResolving && !ui.DebugActionPresentationLocked && battle.DebugEnemyIntent(0) == "ATTACK → Paladin");

        Object.DestroyImmediate(root); report += passed ? "\nRESULT: PASS" : "\nRESULT: FAIL"; Debug.Log(report); if (!passed) throw new System.Exception(report);
    }

    private static void ConfigureBattlefieldSlots(BattleUI ui)
    {
        Image[] allyBodies = new Image[3]; Slider[] allyHp = new Slider[3]; TMP_Text[] allyHpText = new TMP_Text[3]; TMP_Text[] allyStatus = new TMP_Text[3]; Image[] allyOverlays = new Image[3]; Image[] allyIndicators = new Image[3]; Button[] allyButtons = new Button[3];
        Image[] enemyBodies = new Image[3]; Slider[] enemyHp = new Slider[3]; TMP_Text[] enemyHpText = new TMP_Text[3]; TMP_Text[] enemyStatus = new TMP_Text[3]; Image[] enemyOverlays = new Image[3]; Image[] enemyIndicators = new Image[3]; Button[] enemyButtons = new Button[3];
        for (int i = 0; i < 3; i++)
        {
            CreateSlot("Ally Test Slot " + i, out allyBodies[i], out allyHp[i], out allyHpText[i], out allyStatus[i], out allyOverlays[i], out allyIndicators[i], out allyButtons[i]);
            CreateSlot("Enemy Test Slot " + i, out enemyBodies[i], out enemyHp[i], out enemyHpText[i], out enemyStatus[i], out enemyOverlays[i], out enemyIndicators[i], out enemyButtons[i]);
        }
        SetPrivateField(ui, "allySlotBodies", allyBodies); SetPrivateField(ui, "allySlotHpSliders", allyHp); SetPrivateField(ui, "allySlotHpTexts", allyHpText); SetPrivateField(ui, "allySlotStatusTexts", allyStatus); SetPrivateField(ui, "allySlotStatusOverlays", allyOverlays); SetPrivateField(ui, "allySlotIndicators", allyIndicators); SetPrivateField(ui, "allySlotButtons", allyButtons);
        SetPrivateField(ui, "enemySlotBodies", enemyBodies); SetPrivateField(ui, "enemySlotHpSliders", enemyHp); SetPrivateField(ui, "enemySlotHpTexts", enemyHpText); SetPrivateField(ui, "enemySlotStatusTexts", enemyStatus); SetPrivateField(ui, "enemySlotStatusOverlays", enemyOverlays); SetPrivateField(ui, "enemySlotIndicators", enemyIndicators); SetPrivateField(ui, "enemySlotButtons", enemyButtons);
        TMP_Text messageText = new GameObject("Battle Test Message", typeof(RectTransform), typeof(TextMeshProUGUI)).GetComponent<TMP_Text>();
        messageText.transform.SetParent(ui.transform, false);
        SetPrivateField(ui, "messageText", messageText);
        GameObject turnBanner = new GameObject("Turn Banner Test", typeof(RectTransform), typeof(Image)); turnBanner.transform.SetParent(ui.transform, false);
        TMP_Text turnBannerText = new GameObject("Turn Banner Text Test", typeof(RectTransform), typeof(TextMeshProUGUI)).GetComponent<TMP_Text>(); turnBannerText.transform.SetParent(turnBanner.transform, false);
        turnBanner.SetActive(false); SetPrivateField(ui, "turnBannerPanel", turnBanner); SetPrivateField(ui, "turnBannerText", turnBannerText);
        GameObject commandDock = new GameObject("Command Dock Test", typeof(RectTransform), typeof(Image)); commandDock.transform.SetParent(ui.transform, false); commandDock.SetActive(false);
        GameObject skillSubmenu = new GameObject("Skill Submenu Test", typeof(RectTransform)); skillSubmenu.transform.SetParent(commandDock.transform, false); skillSubmenu.SetActive(false);
        TMP_Text actorSummary = new GameObject("Actor Summary Test", typeof(RectTransform), typeof(TextMeshProUGUI)).GetComponent<TMP_Text>(); actorSummary.transform.SetParent(commandDock.transform, false);
        TMP_Text skillDescription = new GameObject("Skill Description Test", typeof(RectTransform), typeof(TextMeshProUGUI)).GetComponent<TMP_Text>(); skillDescription.transform.SetParent(ui.transform, false); skillDescription.gameObject.SetActive(false);
        SetPrivateField(ui, "actionCommandPanel", commandDock);
        SetPrivateField(ui, "skillSubmenuPanel", skillSubmenu);
        SetPrivateField(ui, "selectedUnitText", actorSummary);
        SetPrivateField(ui, "skillDescriptionText", skillDescription);
        SetPrivateField(ui, "attackButton", CreateTestButton("Attack Test", commandDock.transform));
        SetPrivateField(ui, "skillMenuButton", CreateTestButton("Skill Menu Test", commandDock.transform));
        SetPrivateField(ui, "guardButton", CreateTestButton("Guard Test", commandDock.transform));
        SetPrivateField(ui, "endTurnButton", CreateTestButton("End Turn Test", commandDock.transform));
        SetPrivateField(ui, "fireSkillButton", CreateTestButton("Fire Test", skillSubmenu.transform));
        SetPrivateField(ui, "iceSkillButton", CreateTestButton("Ice Test", skillSubmenu.transform));
        SetPrivateField(ui, "lightningSkillButton", CreateTestButton("Lightning Test", skillSubmenu.transform));
        SetPrivateField(ui, "earthSkillButton", CreateTestButton("Earth Test", skillSubmenu.transform));
        SetPrivateField(ui, "skillBackButton", CreateTestButton("Back Test", skillSubmenu.transform));
        SetPrivateField(ui, "paladinBattleSprite", AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/BattleUnits/ally_paladin.png"));
        SetPrivateField(ui, "clericBattleSprite", AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/BattleUnits/ally_cleric.png"));
        SetPrivateField(ui, "rangerBattleSprite", AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/BattleUnits/ally_ranger.png"));
        SetPrivateField(ui, "goblinBattleSprite", AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/BattleUnits/enemy_goblin.png"));
        SetPrivateField(ui, "skeletonBattleSprite", AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/BattleUnits/enemy_skeleton.png"));
        SetPrivateField(ui, "orcBattleSprite", AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/BattleUnits/enemy_orc.png"));
    }

    private static void CreateSlot(string name, out Image body, out Slider hp, out TMP_Text hpText, out TMP_Text status, out Image overlay, out Image indicator, out Button button)
    {
        GameObject bodyObject = new GameObject(name + " Body", typeof(RectTransform), typeof(Image), typeof(Button));
        body = bodyObject.GetComponent<Image>(); button = bodyObject.GetComponent<Button>();
        hp = new GameObject(name + " HP", typeof(RectTransform), typeof(Slider)).GetComponent<Slider>();
        hpText = new GameObject(name + " HP Text", typeof(RectTransform), typeof(TextMeshProUGUI)).GetComponent<TMP_Text>();
        status = new GameObject(name + " Status", typeof(RectTransform), typeof(TextMeshProUGUI)).GetComponent<TMP_Text>();
        overlay = new GameObject(name + " Overlay", typeof(RectTransform), typeof(Image)).GetComponent<Image>();
        indicator = new GameObject(name + " Indicator", typeof(RectTransform), typeof(Image)).GetComponent<Image>();
        overlay.gameObject.SetActive(false); indicator.gameObject.SetActive(false);
    }

    private static void UnlockAllSkills()
    {
        for (int stage = 0; stage < 4; stage++) ProgressState.MarkStageCompleted(stage);
    }

    private static Button CreateTestButton(string name, Transform parent)
    {
        Button button = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button)).GetComponent<Button>();
        button.transform.SetParent(parent, false);
        TMP_Text label = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI)).GetComponent<TMP_Text>();
        label.transform.SetParent(button.transform, false);
        label.text = TestButtonLabel(name);
        return button;
    }

    private static string TestButtonLabel(string name)
    {
        switch (name)
        {
            case "Attack Button": return "ATTACK";
            case "Skill Menu Button": return "SKILL";
            case "Guard Button": return "GUARD";
            case "End Turn Button": return "END TURN";
            case "Fire Skill Button": return "Fire Bolt\nAP 2";
            case "Ice Lance Button": return "Ice Lance\nAP 1";
            case "Earth Wall Button": return "Earth Wall\nAP 2";
            case "Lightning Strike Button": return "Lightning Strike\nAP 3";
            case "Skill Back Button": return "BACK";
            default: return name;
        }
    }

    private static void SetPrivateField(object target, string fieldName, object value)
    {
        FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        if (field != null) field.SetValue(target, value);
    }
    private static void RecordState(ref string report, string label, string state)
    {
        report += $"[STATE] {label}: {state}\n";
    }

    private static void Check(ref bool passed, ref string report, string label, bool condition)
    {
        report += (condition ? "[OK] " : "[FAIL] ") + label + "\n";
        if (!condition) passed = false;
    }
}
