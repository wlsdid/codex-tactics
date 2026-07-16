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

        Check(ref passed, ref report, "living unused party selection succeeds", battle.SelectPlayerUnit(1) && battle.DebugSelectedPlayerIndex == 1 && ui.DebugAllySlotSelected(1) && ui.DebugActiveAllyIndicatorCount == 1);
        bool dynamicTargets = battle.SelectEnemyTarget(0) && battle.DebugMessageText == "Target: Goblin" && battle.SelectEnemyTarget(1) && battle.DebugMessageText == "Target: Skeleton" && battle.SelectEnemyTarget(2) && battle.DebugMessageText == "Target: Orc Berserker";
        Check(ref passed, ref report, "target message follows each selected CharacterData", dynamicTargets && battle.DebugSelectedEnemyIndex == 2 && ui.DebugEnemySlotTargeted(0) && ui.DebugActiveEnemyIndicatorCount == 1);
        int targetHp = battle.enemyParty[2].currentHp;
        battle.OnClickAttackButton();
        Check(ref passed, ref report, "selected target alone receives attack", battle.enemyParty[2].currentHp < targetHp && battle.enemyParty[0].currentHp == battle.enemyParty[0].maxHp && battle.enemyParty[1].currentHp == battle.enemyParty[1].maxHp);
        Check(ref passed, ref report, "repeat action is blocked in same player phase", !battle.SelectPlayerUnit(1) && battle.DebugHasActed(1));
        battle.DebugSetCurrentHpForTest(true, 2, 0);
        Check(ref passed, ref report, "dead party slot becomes unavailable and visibly dead", !battle.SelectPlayerUnit(2) && !ui.DebugAllySlotInteractable(2) && ui.DebugAllySlotState(2).Contains("DEAD"));
        battle.DebugSetCurrentHpForTest(false, 1, 0);
        Check(ref passed, ref report, "dead enemy slot becomes unavailable and visibly dead", !battle.SelectEnemyTarget(1) && !ui.DebugEnemySlotInteractable(1) && ui.DebugEnemySlotState(1).Contains("DEAD"));

        battle.DebugStartBattleForTest();
        battle.SelectPlayerUnit(0); battle.SelectEnemyTarget(2); battle.OnClickFireSkillButton();
        Check(ref passed, ref report, "burn applies only to selected enemy", battle.enemyParty[2].HasStatusEffect(StatusEffectType.Burn) && !battle.enemyParty[0].HasStatusEffect(StatusEffectType.Burn) && !battle.enemyParty[1].HasStatusEffect(StatusEffectType.Burn));
        battle.DebugStartBattleForTest();
        battle.SelectPlayerUnit(0); battle.SelectEnemyTarget(1); battle.OnClickIceSkillButton();
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
        battle.DebugSetCurrentHpForTest(false, 0, 1);
        battle.SelectPlayerUnit(0); battle.SelectEnemyTarget(0); battle.OnClickAttackButton();
        Check(ref passed, ref report, "dead enemy does not end fight while enemies live", battle.enemyParty[0].IsDead() && battle.DebugState != BattleState.Victory && battle.DebugEnemyPartyCount == 3);
        battle.DebugSetCurrentHpForTest(false, 1, 1); battle.DebugSetCurrentHpForTest(false, 2, 1);
        battle.SelectPlayerUnit(1); battle.SelectEnemyTarget(1); battle.OnClickAttackButton();
        battle.SelectPlayerUnit(2); battle.SelectEnemyTarget(2); battle.OnClickAttackButton();
        Check(ref passed, ref report, "victory only after all enemies die", battle.DebugState == BattleState.Victory && battle.enemyParty.TrueForAll(unit => unit.IsDead()) && battle.DebugResultSummaryText.Contains("Victory") && battle.DebugTotalGoldEarned == 150);
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

    private static void SetPrivateField(object target, string fieldName, object value)
    {
        FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        if (field != null) field.SetValue(target, value);
    }
    private static void Check(ref bool passed, ref string report, string label, bool condition)
    {
        report += (condition ? "[OK] " : "[FAIL] ") + label + "\n";
        if (!condition) passed = false;
    }
}
