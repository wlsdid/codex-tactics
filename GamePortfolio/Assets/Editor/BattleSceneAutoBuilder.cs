using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem.UI;
#endif

public static class BattleSceneAutoBuilder
{
    private const string ScenePath = "Assets/Scenes/BattleScene.unity";

    [MenuItem("Tools/Tactical Requiem/Create Battle Test Scene")]
    public static void CreateBattleTestScene()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        scene.name = "BattleScene";

        Camera camera = CreateCamera();
        camera.gameObject.AddComponent<ScreenShake>();
        Canvas canvas = CreateCanvas(camera);
        CreateEventSystem();

        // A dedicated 16:9 sprite fills the 1280x720 reference canvas without stretching.
        Image battleStageBackdropPanel = CreateSpritePanel(canvas.transform, "Battle Stage Backdrop Panel", "Assets/Art/BattleBackgrounds/forest_tactical_lane_night.png", Vector2.zero, new Vector2(1280, 720));
        battleStageBackdropPanel.color = Color.white;
        battleStageBackdropPanel.preserveAspect = true;
        battleStageBackdropPanel.raycastTarget = false;

        // Keep only the real 3v3 battlefield slots. Legacy side rails and decorative lighting overlays are not generated.
        CreateBattlefieldUnitStandees(canvas.transform);

        Image topStatusPanel = CreatePanel(canvas.transform, "Top Status Panel", new Vector2(0, 330), new Vector2(1040, 30), new Color(0.010f, 0.014f, 0.024f, 0.52f));
        topStatusPanel.raycastTarget = false;

        Button playerSelectButton = null;
        Image playerSelectionHighlight = null;

        TMP_Text titleText = CreateText(canvas.transform, "Title Text", "TACTICAL REQUIEM", new Vector2(-500, 334), new Vector2(270, 30), TextAlignmentOptions.Left);
        titleText.fontSize = 16;
        titleText.fontStyle = FontStyles.Bold;

        TMP_Text runStatusText = null;
        TMP_Text battleGuideText = null;
        Image stageChip = CreatePanel(canvas.transform, "Stage Chip Panel", new Vector2(-235, 284), new Vector2(270, 24), new Color(0.032f, 0.026f, 0.014f, 0.54f));
        Image stageChipEdge = CreatePanel(canvas.transform, "Stage Chip Top Edge", new Vector2(-235, 296), new Vector2(248, 2), new Color(1.0f, 0.78f, 0.38f, 0.32f));
        stageChip.raycastTarget = false;
        stageChipEdge.raycastTarget = false;
        TMP_Text stageText = CreateText(canvas.transform, "Stage Text", "BATTLE PREP", new Vector2(-235, 284), new Vector2(250, 20), TextAlignmentOptions.Center);
        stageText.fontSize = 12;
        stageText.fontStyle = FontStyles.Bold;
        stageText.color = new Color(0.92f, 0.86f, 0.55f);
        TMP_Text stageObjectiveText = null;
        Image stageProgressChip = CreatePanel(canvas.transform, "Stage Progress Chip Panel", new Vector2(235, 284), new Vector2(150, 20), new Color(0.018f, 0.026f, 0.040f, 0.42f));
        stageProgressChip.raycastTarget = false;
        TMP_Text stageProgressText = CreateText(canvas.transform, "Stage Progress Text", "TURN QUEUE", new Vector2(235, 284), new Vector2(136, 16), TextAlignmentOptions.Center);
        stageProgressText.fontSize = 8;
        stageProgressText.color = new Color(0.72f, 0.90f, 1.0f, 0.86f);


        // Legacy selected-unit rails are not generated; the six battlefield slot labels own name/HP presentation.
        TMP_Text playerHpText = null;
        Slider playerHpSlider = null;
        TMP_Text playerApText = null;
        Slider playerApSlider = null;
        TMP_Text playerStatusText = null;
        TMP_Text playerShieldText = null;
        Image playerSpriteImage = null;
        TMP_Text enemyHpText = null;
        Slider enemyHpSlider = null;
        TMP_Text enemyStatusText = null;
        TMP_Text enemyIntentText = null;
        TMP_Text enemyBreakText = null;
        Slider enemyBreakSlider = null;
        Image enemySpriteImage = null;
        Image burnOverlay = null;
        Image stunOverlay = null;
        Image brokenOverlay = null;
        TMP_Text messageText = CreateText(canvas.transform, "Message Text", "Battle Start!", new Vector2(100, 304), new Vector2(460, 18), TextAlignmentOptions.Center);
        messageText.fontSize = 10;
        messageText.color = new Color(1.0f, 0.94f, 0.72f, 0.86f);
        TMP_Text impactText = CreateText(canvas.transform, "Impact Text", "", new Vector2(462, 304), new Vector2(210, 16), TextAlignmentOptions.Right);
        impactText.fontSize = 8;
        impactText.color = Color.clear;
        TMP_Text demoRouteText = null;
        TMP_Text captureRehearsalText = null;
        Image skillDetailPanel = null;
        TMP_Text skillHelpText = null;
        Image resultSummaryPanel = CreatePanel(canvas.transform, "Result Summary Panel", new Vector2(0, -42), new Vector2(620, 230), new Color(0.03f, 0.04f, 0.06f, 0.92f));
        resultSummaryPanel.gameObject.SetActive(false);
        TMP_Text resultSummaryText = CreateText(canvas.transform, "Result Summary Text", "Result Summary", new Vector2(0, -42), new Vector2(580, 195), TextAlignmentOptions.TopLeft);
        resultSummaryText.fontSize = 18;
        resultSummaryText.color = new Color(1.0f, 0.92f, 0.58f);
        resultSummaryText.gameObject.SetActive(false);
        // ── Command Preview Panel ──
        Image commandPreviewPanel = CreatePanel(canvas.transform, "Command Preview Panel", new Vector2(300, -276), new Vector2(260, 36), new Color(0.04f, 0.06f, 0.12f, 0.94f));
        commandPreviewPanel.gameObject.SetActive(false);
        TMP_Text commandPreviewText = CreateText(canvas.transform, "Command Preview Text", "", new Vector2(300, -276), new Vector2(240, 28), TextAlignmentOptions.Left);
        commandPreviewText.fontSize = 9;
        commandPreviewText.color = new Color(0.92f, 0.88f, 0.82f);
        commandPreviewText.gameObject.SetActive(false);
        Image battleLogPanel = CreatePanel(canvas.transform, "Battle Log Panel", new Vector2(-95, -80), new Vector2(520, 150), new Color(0.05f, 0.06f, 0.09f, 0.88f));
        TMP_Text battleLogTitleText = CreateText(canvas.transform, "Battle Log Title Text", "Recent Actions", new Vector2(-95, -24), new Vector2(480, 30), TextAlignmentOptions.Left);
        battleLogTitleText.fontSize = 20;
        battleLogTitleText.color = new Color(0.96f, 0.92f, 0.68f);
        TMP_Text battleLogText = CreateText(canvas.transform, "Battle Log Text", "Recent Actions\nNo actions yet.", new Vector2(-95, -92), new Vector2(480, 95), TextAlignmentOptions.TopLeft);
        battleLogText.fontSize = 16;
        battleLogText.color = new Color(0.82f, 0.86f, 0.95f);
        battleLogPanel.raycastTarget = false;
        battleLogPanel.gameObject.SetActive(false);
        battleLogTitleText.gameObject.SetActive(false);
        battleLogText.gameObject.SetActive(false);

        TMP_Text commandHintText = null;
        Image referenceSkillDetailPanel = null;
        TMP_Text referenceSkillDetailText = null;
        Image enemyIntentCardPanel = null;
        TMP_Text enemyIntentCardText = null;

        Image actionCommandPanel = CreatePanel(canvas.transform, "Context Command Dock", new Vector2(0, -310), new Vector2(1120, 84), new Color(0.018f, 0.028f, 0.055f, 0.88f));
        Outline dockOutline = actionCommandPanel.gameObject.AddComponent<Outline>();
        dockOutline.effectColor = new Color(0.35f, 0.46f, 0.58f, 0.90f);
        dockOutline.effectDistance = new Vector2(1f, -1f);
        TMP_Text selectedUnitText = CreateText(actionCommandPanel.transform, "Actor Command Summary", "Paladin  HP 100/100  AP 3/3", new Vector2(-405, 0), new Vector2(280, 48), TextAlignmentOptions.Left);
        selectedUnitText.fontSize = 16;
        selectedUnitText.color = new Color(0.90f, 0.93f, 0.98f);

        Button attackButton = CreateCenteredButton(actionCommandPanel.transform, "Attack Button", "ATTACK", new Vector2(-190, 0), new Vector2(130, 48));
        Button skillMenuButton = CreateCenteredButton(actionCommandPanel.transform, "Skill Menu Button", "SKILL", new Vector2(-45, 0), new Vector2(130, 48));
        Button guardButton = CreateCenteredButton(actionCommandPanel.transform, "Guard Button", "GUARD", new Vector2(100, 0), new Vector2(130, 48));
        Button endTurnButton = CreateCenteredButton(actionCommandPanel.transform, "End Turn Button", "END TURN", new Vector2(265, 0), new Vector2(160, 48));
        StyleContextButton(attackButton, new Color(0.30f, 0.075f, 0.085f, 0.96f));
        StyleContextButton(skillMenuButton, new Color(0.085f, 0.105f, 0.16f, 0.96f));
        StyleContextButton(guardButton, new Color(0.045f, 0.24f, 0.25f, 0.96f));
        StyleContextButton(endTurnButton, new Color(0.11f, 0.13f, 0.18f, 0.96f));

        Image skillSubmenuPanel = CreatePanel(actionCommandPanel.transform, "Skill Submenu", new Vector2(115, 0), new Vector2(820, 64), Color.clear);
        skillSubmenuPanel.raycastTarget = false;
        Button fireSkillButton = CreateCenteredButton(skillSubmenuPanel.transform, "Fire Skill Button", "Fire Bolt\nAP 2", new Vector2(-330, 0), new Vector2(130, 48));
        Button iceSkillButton = CreateCenteredButton(skillSubmenuPanel.transform, "Ice Lance Button", "Ice Lance\nAP 1", new Vector2(-187, 0), new Vector2(130, 48));
        Button earthSkillButton = CreateCenteredButton(skillSubmenuPanel.transform, "Earth Wall Button", "Earth Wall\nAP 2", new Vector2(-44, 0), new Vector2(130, 48));
        Button lightningSkillButton = CreateCenteredButton(skillSubmenuPanel.transform, "Lightning Strike Button", "Lightning Strike\nAP 3", new Vector2(116, 0), new Vector2(166, 48));
        Button skillBackButton = CreateCenteredButton(skillSubmenuPanel.transform, "Skill Back Button", "BACK", new Vector2(270, 0), new Vector2(100, 48));
        StyleContextButton(fireSkillButton, new Color(0.34f, 0.065f, 0.075f, 0.96f));
        StyleContextButton(iceSkillButton, new Color(0.27f, 0.075f, 0.10f, 0.96f));
        StyleContextButton(lightningSkillButton, new Color(0.31f, 0.085f, 0.12f, 0.96f));
        StyleContextButton(earthSkillButton, new Color(0.035f, 0.28f, 0.27f, 0.96f));
        StyleContextButton(skillBackButton, new Color(0.10f, 0.12f, 0.17f, 0.96f));
        TMP_Text skillDescriptionText = CreateText(canvas.transform, "Skill Hover Description", "", new Vector2(120, -252), new Vector2(760, 24), TextAlignmentOptions.Center);
        skillDescriptionText.fontSize = 16;
        skillDescriptionText.color = new Color(0.96f, 0.90f, 0.70f);
        skillDescriptionText.gameObject.SetActive(false);

        actionCommandPanel.gameObject.SetActive(false);
        skillSubmenuPanel.gameObject.SetActive(false);
        selectedUnitText.gameObject.SetActive(false);
        Button retryButton = CreateButton(canvas.transform, "Retry Button", "Retry", new Vector2(170, 145), new Vector2(140, 48));
        retryButton.gameObject.SetActive(false);
        Button continueButton = CreateButton(canvas.transform, "Continue Button", "Continue", new Vector2(320, 145), new Vector2(150, 48));
        continueButton.gameObject.SetActive(false);
        // Create the label child that shows "Continue" by default, will be changed to "Next Encounter" at runtime
        TMP_Text continueButtonLabel = continueButton.GetComponentInChildren<TMP_Text>();
        Button stageSelectButton = CreateButton(canvas.transform, "Stage Select Button", "Stage Select", new Vector2(-505, 28), new Vector2(120, 40));
        Button speedToggleButton = CreateButton(canvas.transform, "Speed Toggle Button", "1x", new Vector2(520, 672), new Vector2(52, 30));
        Button autoBattleButton = CreateButton(canvas.transform, "Auto Battle Button", "Auto", new Vector2(458, 672), new Vector2(58, 30));
        Button itemButton = CreateCenteredButton(canvas.transform, "Item Button", "ITEM", new Vector2(314, -254), new Vector2(62, 22));
        itemButton.gameObject.SetActive(false);
        Button pauseButton = CreateButton(canvas.transform, "Pause Button", "II", new Vector2(582, 672), new Vector2(52, 30));
        Button battleLogToggleButton = CreateButton(canvas.transform, "Battle Log Toggle Button", "Log", new Vector2(360, 674), new Vector2(60, 24));
        TMP_Text battleLogToggleLabel = battleLogToggleButton.GetComponentInChildren<TMP_Text>();
        stageSelectButton.gameObject.SetActive(false);
        speedToggleButton.gameObject.SetActive(false);
        autoBattleButton.gameObject.SetActive(false);
        pauseButton.gameObject.SetActive(false);
        battleLogToggleButton.gameObject.SetActive(false);

        // Screen flash image (hidden by default, used by BattleUI for impact feedback)
        Image screenFlashImage = CreateScreenFlashImage(canvas.transform, "Screen Flash Image");

        // ── Turn Banner ──
        GameObject turnBannerObj = new GameObject("Turn Banner Panel", typeof(RectTransform), typeof(Image), typeof(CanvasGroup));
        turnBannerObj.transform.SetParent(canvas.transform, false);
        Image turnBannerBg = turnBannerObj.GetComponent<Image>();
        turnBannerBg.color = new Color(0.03f, 0.04f, 0.08f, 0.90f);
        turnBannerBg.raycastTarget = false;
        RectTransform turnBannerRt = turnBannerObj.GetComponent<RectTransform>();
        turnBannerRt.sizeDelta = new Vector2(500, 80);
        turnBannerRt.anchoredPosition = new Vector2(0, 0);
        turnBannerObj.SetActive(false);

        TMP_Text turnBannerText = CreateText(turnBannerObj.transform, "Turn Banner Text", "", new Vector2(0, 0), new Vector2(480, 70), TextAlignmentOptions.Center);
        turnBannerText.fontSize = 36;
        turnBannerText.fontStyle = FontStyles.Bold;
        turnBannerText.color = Color.white;

        // Pause overlay panel — premium dark
        Image pausePanel = CreatePanel(canvas.transform, "Pause Panel", new Vector2(0, 0), new Vector2(600, 400), new Color(0.03f, 0.04f, 0.08f, 0.95f));
        pausePanel.gameObject.SetActive(false);
        Button resumeButton = CreateButton(canvas.transform, "Resume Button", "Resume", new Vector2(0, 40), new Vector2(260, 60));
        resumeButton.gameObject.SetActive(false);
        Button quitToSelectButton = CreateButton(canvas.transform, "Quit To Select Button", "Quit to Stage Select", new Vector2(0, -40), new Vector2(260, 60));
        quitToSelectButton.gameObject.SetActive(false);

        GameObject battleManagerObject = new GameObject("BattleManager");
        BattleManager battleManager = battleManagerObject.AddComponent<BattleManager>();
        BattleUI battleUI = battleManagerObject.AddComponent<BattleUI>();

        // Ensure AudioManager exists (singleton, DontDestroyOnLoad)
        if (Object.FindObjectOfType<AudioManager>() == null)
        {
            new GameObject("AudioManager", typeof(AudioManager));
        }

        // Ensure ScreenFade exists
        if (Object.FindObjectOfType<ScreenFade>() == null)
        {
            new GameObject("ScreenFade", typeof(ScreenFade));
        }

        SerializedObject serializedBattleUI = new SerializedObject(battleUI);
        SetObjectReference(serializedBattleUI, "playerHpText", playerHpText);
        SetObjectReference(serializedBattleUI, "playerHpSlider", playerHpSlider);
        SetObjectReference(serializedBattleUI, "playerApText", playerApText);
        SetObjectReference(serializedBattleUI, "playerApSlider", playerApSlider);
        SetObjectReference(serializedBattleUI, "playerStatusText", playerStatusText);
        SetObjectReference(serializedBattleUI, "playerShieldText", playerShieldText);
        SetObjectReference(serializedBattleUI, "playerSpriteImage", playerSpriteImage);
        SetObjectReference(serializedBattleUI, "referencePlayerSprite", LoadPixelSprite("Assets/Art/Generated/chibi_hero_original.png"));
        SetObjectReference(serializedBattleUI, "enemyHpText", enemyHpText);
        SetObjectReference(serializedBattleUI, "enemyHpSlider", enemyHpSlider);
        SetObjectReference(serializedBattleUI, "enemyStatusText", enemyStatusText);
        SetObjectReference(serializedBattleUI, "enemyIntentText", enemyIntentText);
        SetObjectReference(serializedBattleUI, "enemyBreakText", enemyBreakText);
        SetObjectReference(serializedBattleUI, "enemyBreakSlider", enemyBreakSlider);
        SetObjectReference(serializedBattleUI, "enemySpriteImage", enemySpriteImage);
        SetObjectReference(serializedBattleUI, "enemyStandeeImage", FindImage("Enemy Standee Body"));
        SetObjectReference(serializedBattleUI, "heroStandeeImage", FindImage("Hero Standee Body"));
        SetObjectReference(serializedBattleUI, "heroFormationFocusRing", FindImage("Hero Base Ring Panel"));
        SetObjectReference(serializedBattleUI, "enemyFormationTargetRing", FindImage("Enemy Base Ring Panel"));
        SetObjectArrayReferences(serializedBattleUI, "allySlotBodies", FindImage("Ally Slot 1 Body"), FindImage("Ally Slot 2 Body"), FindImage("Ally Slot 3 Body"));
        SetObjectArrayReferences(serializedBattleUI, "allySlotHpSliders", FindSlider("Ally Slot 1 HP Slider"), FindSlider("Ally Slot 2 HP Slider"), FindSlider("Ally Slot 3 HP Slider"));
        SetObjectArrayReferences(serializedBattleUI, "allySlotHpTexts", FindText("Ally Slot 1 HP Text"), FindText("Ally Slot 2 HP Text"), FindText("Ally Slot 3 HP Text"));
        SetObjectArrayReferences(serializedBattleUI, "allySlotStatusTexts", FindText("Ally Slot 1 Status Text"), FindText("Ally Slot 2 Status Text"), FindText("Ally Slot 3 Status Text"));
        SetObjectArrayReferences(serializedBattleUI, "allySlotStatusOverlays", FindImageIncludingInactive("Ally Slot 1 Status Overlay"), FindImageIncludingInactive("Ally Slot 2 Status Overlay"), FindImageIncludingInactive("Ally Slot 3 Status Overlay"));
        SetObjectArrayReferences(serializedBattleUI, "allySlotIndicators", FindImageIncludingInactive("Ally Slot 1 Indicator"), FindImageIncludingInactive("Ally Slot 2 Indicator"), FindImageIncludingInactive("Ally Slot 3 Indicator"));
        SetObjectArrayReferences(serializedBattleUI, "allySlotButtons", FindButton("Ally Slot 1 Body"), FindButton("Ally Slot 2 Body"), FindButton("Ally Slot 3 Body"));
        SetObjectArrayReferences(serializedBattleUI, "enemySlotBodies", FindImage("Enemy Slot 1 Body"), FindImage("Enemy Slot 2 Body"), FindImage("Enemy Slot 3 Body"));
        SetObjectArrayReferences(serializedBattleUI, "enemySlotHpSliders", FindSlider("Enemy Slot 1 HP Slider"), FindSlider("Enemy Slot 2 HP Slider"), FindSlider("Enemy Slot 3 HP Slider"));
        SetObjectArrayReferences(serializedBattleUI, "enemySlotHpTexts", FindText("Enemy Slot 1 HP Text"), FindText("Enemy Slot 2 HP Text"), FindText("Enemy Slot 3 HP Text"));
        SetObjectArrayReferences(serializedBattleUI, "enemySlotStatusTexts", FindText("Enemy Slot 1 Status Text"), FindText("Enemy Slot 2 Status Text"), FindText("Enemy Slot 3 Status Text"));
        SetObjectArrayReferences(serializedBattleUI, "enemySlotStatusOverlays", FindImageIncludingInactive("Enemy Slot 1 Status Overlay"), FindImageIncludingInactive("Enemy Slot 2 Status Overlay"), FindImageIncludingInactive("Enemy Slot 3 Status Overlay"));
        SetObjectArrayReferences(serializedBattleUI, "enemySlotIndicators", FindImageIncludingInactive("Enemy Slot 1 Indicator"), FindImageIncludingInactive("Enemy Slot 2 Indicator"), FindImageIncludingInactive("Enemy Slot 3 Indicator"));
        SetObjectArrayReferences(serializedBattleUI, "enemySlotButtons", FindButton("Enemy Slot 1 Body"), FindButton("Enemy Slot 2 Body"), FindButton("Enemy Slot 3 Body"));
        // Right legacy roster is deliberately absent; enemy names remain in CharacterData and battlefield slot labels.
        SetObjectReference(serializedBattleUI, "referenceEnemySprite", LoadPixelSprite("Assets/Art/Generated/chibi_enemy_original.png"));
        SetObjectReference(serializedBattleUI, "referenceGoblinSprite", LoadPixelSprite("Assets/Art/Generated/chibi_enemy_original.png"));
        SetObjectReference(serializedBattleUI, "referenceSkeletonSprite", LoadPixelSprite("Assets/Art/Generated/chibi_enemy_raider.png"));
        SetObjectReference(serializedBattleUI, "referenceOrcSprite", LoadPixelSprite("Assets/Art/Generated/chibi_enemy_raider.png"));
        SetObjectReference(serializedBattleUI, "referenceLichSprite", LoadPixelSprite("Assets/Art/Generated/chibi_enemy_original.png"));
        SetObjectReference(serializedBattleUI, "referenceGolemSprite", LoadPixelSprite("Assets/Art/Generated/chibi_enemy_raider.png"));
        SetObjectReference(serializedBattleUI, "referenceDarkKnightSprite", LoadPixelSprite("Assets/Art/Generated/chibi_enemy_raider.png"));
        SetObjectReference(serializedBattleUI, "paladinBattleSprite", LoadPixelSprite("Assets/Art/BattleUnits/ally_paladin.png"));
        SetObjectReference(serializedBattleUI, "clericBattleSprite", LoadPixelSprite("Assets/Art/BattleUnits/ally_cleric.png"));
        SetObjectReference(serializedBattleUI, "rangerBattleSprite", LoadPixelSprite("Assets/Art/BattleUnits/ally_ranger.png"));
        SetObjectReference(serializedBattleUI, "goblinBattleSprite", LoadPixelSprite("Assets/Art/BattleUnits/enemy_goblin.png"));
        SetObjectReference(serializedBattleUI, "skeletonBattleSprite", LoadPixelSprite("Assets/Art/BattleUnits/enemy_skeleton.png"));
        SetObjectReference(serializedBattleUI, "orcBattleSprite", LoadPixelSprite("Assets/Art/BattleUnits/enemy_orc.png"));
        SetObjectReference(serializedBattleUI, "burnOverlay", burnOverlay);
        SetObjectReference(serializedBattleUI, "stunOverlay", stunOverlay);
        SetObjectReference(serializedBattleUI, "brokenOverlay", brokenOverlay);
        SetObjectReference(serializedBattleUI, "runStatusText", runStatusText);
        SetObjectReference(serializedBattleUI, "stageText", stageText);
        SetObjectReference(serializedBattleUI, "stageObjectiveText", stageObjectiveText);
        SetObjectReference(serializedBattleUI, "stageProgressText", stageProgressText);
        SetObjectReference(serializedBattleUI, "messageText", messageText);
        SetObjectReference(serializedBattleUI, "impactText", impactText);
        SetObjectReference(serializedBattleUI, "captureRehearsalText", captureRehearsalText);
        SetObjectReference(serializedBattleUI, "skillHelpText", skillHelpText);
        SetObjectReference(serializedBattleUI, "battleLogPanel", battleLogPanel.gameObject);
        SetObjectReference(serializedBattleUI, "battleLogTitleText", battleLogTitleText);
        SetObjectReference(serializedBattleUI, "battleLogText", battleLogText);
        SetObjectReference(serializedBattleUI, "battleLogToggleButton", battleLogToggleButton);
        SetObjectReference(serializedBattleUI, "battleLogToggleLabel", battleLogToggleLabel);
        SetObjectReference(serializedBattleUI, "resultSummaryText", resultSummaryText);
        SetObjectReference(serializedBattleUI, "resultSummaryPanel", resultSummaryPanel.gameObject);
        SetObjectReference(serializedBattleUI, "commandPreviewPanel", commandPreviewPanel.gameObject);
        SetObjectReference(serializedBattleUI, "commandPreviewText", commandPreviewText);
        SetObjectReference(serializedBattleUI, "turnBannerPanel", turnBannerObj);
        SetObjectReference(serializedBattleUI, "turnBannerText", turnBannerText);
        SetObjectReference(serializedBattleUI, "actionCommandPanel", actionCommandPanel.gameObject);
        SetObjectReference(serializedBattleUI, "skillSubmenuPanel", skillSubmenuPanel.gameObject);
        SetObjectReference(serializedBattleUI, "skillMenuButton", skillMenuButton);
        SetObjectReference(serializedBattleUI, "skillDescriptionText", skillDescriptionText);
        SetObjectReference(serializedBattleUI, "playerSelectButton", playerSelectButton);
        SetObjectReference(serializedBattleUI, "playerSelectionHighlight", playerSelectionHighlight);
        SetObjectReference(serializedBattleUI, "selectedUnitText", selectedUnitText);
        SetObjectReference(serializedBattleUI, "attackButton", attackButton);
        SetObjectReference(serializedBattleUI, "fireSkillButton", fireSkillButton);
        SetObjectReference(serializedBattleUI, "iceSkillButton", iceSkillButton);
        SetObjectReference(serializedBattleUI, "lightningSkillButton", lightningSkillButton);
        SetObjectReference(serializedBattleUI, "earthSkillButton", earthSkillButton);
        SetObjectReference(serializedBattleUI, "skillBackButton", skillBackButton);
        SetObjectReference(serializedBattleUI, "guardButton", guardButton);
        SetObjectReference(serializedBattleUI, "endTurnButton", endTurnButton);
        SetObjectReference(serializedBattleUI, "retryButton", retryButton);
        SetObjectReference(serializedBattleUI, "continueButton", continueButton);
        SetObjectReference(serializedBattleUI, "stageSelectButton", stageSelectButton);
        SetObjectReference(serializedBattleUI, "speedToggleButton", speedToggleButton);
        SetObjectReference(serializedBattleUI, "autoBattleButton", autoBattleButton);
        SetObjectReference(serializedBattleUI, "itemButton", itemButton);
        SetObjectReference(serializedBattleUI, "pauseButton", pauseButton);
        SetObjectReference(serializedBattleUI, "pausePanel", pausePanel.gameObject);
        SetObjectReference(serializedBattleUI, "resumeButton", resumeButton);
        SetObjectReference(serializedBattleUI, "quitButton", quitToSelectButton);
        SetObjectReference(serializedBattleUI, "screenFlashImage", screenFlashImage);
        serializedBattleUI.ApplyModifiedPropertiesWithoutUndo();

        // Link BattleUI to BattleManager
        SerializedObject serializedBattleManager = new SerializedObject(battleManager);
        SetObjectReference(serializedBattleManager, "battleUI", battleUI);
        serializedBattleManager.ApplyModifiedPropertiesWithoutUndo();

        TacticalTypography.ApplyToLoadedScene();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, ScenePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Selection.activeGameObject = battleManagerObject;
        EditorUtility.DisplayDialog(
            "BattleScene Created",
            "Assets/Scenes/BattleScene.unity created!\n\nPress Play, click an allied battlefield unit, then test Attack / Skills / Guard / End Turn.",
            "OK"
        );
    }

    private static void ValidateBattleSlotIntegration()
    {
        bool passed = true;
        string report = "BattleScene 3v3 Slot Integration Test\n\n";
        if (!System.IO.File.Exists(ScenePath))
        {
            EditorUtility.DisplayDialog("BattleScene Test Failed", "BattleScene file does not exist.", "OK");
            throw new System.InvalidOperationException("BattleScene file does not exist.");
        }
        EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        BattleManager battle = Object.FindFirstObjectByType<BattleManager>();
        BattleUI ui = battle != null ? battle.GetComponent<BattleUI>() : null;
        AppendCheck(ref passed, ref report, "BattleManager and BattleUI exist", battle != null && ui != null);
        SerializedObject serializedUi = ui != null ? new SerializedObject(ui) : null;
        string[] fields = { "allySlotBodies", "allySlotHpSliders", "allySlotHpTexts", "allySlotStatusOverlays", "allySlotIndicators", "allySlotButtons", "enemySlotBodies", "enemySlotHpSliders", "enemySlotHpTexts", "enemySlotStatusOverlays", "enemySlotIndicators", "enemySlotButtons" };
        bool arraysExact = serializedUi != null;
        foreach (string field in fields) arraysExact &= HasExactlyThreeObjectReferences(serializedUi, field);
        AppendCheck(ref passed, ref report, "all six battlefield slots have complete exact 3-entry metadata", arraysExact);
        AppendCheck(ref passed, ref report, "obsolete support formation method is absent", typeof(BattleSceneAutoBuilder).GetMethod("CreateBattlefieldSupportFormation", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static) == null && FindImageIncludingInactive("Ally Formation Unit 1 Body") == null && FindImageIncludingInactive("Enemy Formation Unit 1 Body") == null);

        Image backdrop = FindImageIncludingInactive("Battle Stage Backdrop Panel");
        const string backdropAssetPath = "Assets/Art/BattleBackgrounds/forest_tactical_lane_night.png";
        string backdropPath = backdrop != null && backdrop.sprite != null ? AssetDatabase.GetAssetPath(backdrop.sprite) : string.Empty;
        TextureImporter backdropImporter = AssetImporter.GetAtPath(backdropAssetPath) as TextureImporter;
        AppendCheck(ref passed, ref report, "battle uses only the tactical lane 1920x1080 background sprite", backdrop != null && backdropPath == backdropAssetPath && backdrop.sprite.texture.width == 1920 && backdrop.sprite.texture.height == 1080 && backdrop.preserveAspect && backdrop.rectTransform.sizeDelta == new Vector2(1280f, 720f));
        AppendCheck(ref passed, ref report, "background importer is point uncompressed clamp with mipmaps off", backdropImporter != null && backdropImporter.textureType == TextureImporterType.Sprite && backdropImporter.spriteImportMode == SpriteImportMode.Single && backdropImporter.filterMode == FilterMode.Point && backdropImporter.textureCompression == TextureImporterCompression.Uncompressed && !backdropImporter.mipmapEnabled && backdropImporter.wrapMode == TextureWrapMode.Clamp);
        AppendCheck(ref passed, ref report, "legacy side and bottom HUD objects are physically absent", FindImageIncludingInactive("Party Roster Panel") == null && FindTextIncludingInactive("Player Card Title Text") == null && FindImageIncludingInactive("Player HP Chip Panel") == null && FindImageIncludingInactive("Player AP Chip Panel") == null && FindTextIncludingInactive("Enemy Card Title Text") == null && FindImageIncludingInactive("Enemy HP Chip Panel") == null && FindImageIncludingInactive("Enemy Roster Slot 1") == null);
        AppendCheck(ref passed, ref report, "battlefield slots show only compact name and HP labels", FindTextIncludingInactive("Ally Slot 1 HP Text") != null && FindTextIncludingInactive("Enemy Slot 1 HP Text") != null && FindTextIncludingInactive("Ally Slot 1 Status Text") == null && FindTextIncludingInactive("Enemy Slot 1 Status Text") == null);
        AppendCheck(ref passed, ref report, "spotlight beam floor and color-grade overlays are physically absent", FindImageIncludingInactive("Battle Stage Color Grade Panel") == null && FindImageIncludingInactive("Battle Stage Floor Panel") == null && FindImageIncludingInactive("Moonlight Beam Panel") == null && FindImageIncludingInactive("Hero Cinematic Spotlight") == null && FindImageIncludingInactive("Enemy Cinematic Spotlight") == null && FindImageIncludingInactive("Floor Specular Highlight Panel") == null);

        Image commandDock = FindImageIncludingInactive("Context Command Dock");
        GameObject skillSubmenu = FindImageIncludingInactive("Skill Submenu")?.gameObject;
        TMP_Text actorSummary = FindTextIncludingInactive("Actor Command Summary");
        TMP_Text skillDescription = FindTextIncludingInactive("Skill Hover Description");
        Button skillMenuButton = FindButtonIncludingInactive("Skill Menu Button");
        Button skillBackButton = FindButtonIncludingInactive("Skill Back Button");
        Button contextualAttack = FindButtonIncludingInactive("Attack Button");
        Button contextualGuard = FindButtonIncludingInactive("Guard Button");
        Button contextualEndTurn = FindButtonIncludingInactive("End Turn Button");
        Button contextualFire = FindButtonIncludingInactive("Fire Skill Button");
        Button contextualIce = FindButtonIncludingInactive("Ice Lance Button");
        Button contextualEarth = FindButtonIncludingInactive("Earth Wall Button");
        Button contextualLightning = FindButtonIncludingInactive("Lightning Strike Button");
        AppendCheck(ref passed, ref report, "compact contextual dock and skill submenu objects exist", commandDock != null && skillSubmenu != null && actorSummary != null && skillDescription != null && skillMenuButton != null && skillBackButton != null && contextualAttack != null && contextualGuard != null && contextualEndTurn != null && contextualFire != null && contextualIce != null && contextualEarth != null && contextualLightning != null);
        AppendCheck(ref passed, ref report, "all four skill buttons remain active under the hidden submenu", contextualFire != null && contextualFire.gameObject.activeSelf && contextualIce != null && contextualIce.gameObject.activeSelf && contextualEarth != null && contextualEarth.gameObject.activeSelf && contextualLightning != null && contextualLightning.gameObject.activeSelf);
        AppendCheck(ref passed, ref report, "command dock stays bottom-center and no taller than 90px", commandDock != null && commandDock.rectTransform.sizeDelta.y <= 90f && commandDock.rectTransform.anchoredPosition.y <= -280f && Mathf.Abs(commandDock.rectTransform.anchoredPosition.x) < 1f && commandDock.color.a >= 0.85f && commandDock.color.a <= 0.90f);
        AppendCheck(ref passed, ref report, "context command buttons use 44-52px height and readable labels", HasCompactReadableButton(contextualAttack) && HasCompactReadableButton(skillMenuButton) && HasCompactReadableButton(contextualGuard) && HasCompactReadableButton(contextualEndTurn) && HasCompactReadableButton(FindButtonIncludingInactive("Fire Skill Button")) && HasCompactReadableButton(FindButtonIncludingInactive("Ice Lance Button")) && HasCompactReadableButton(FindButtonIncludingInactive("Earth Wall Button")) && HasCompactReadableButton(FindButtonIncludingInactive("Lightning Strike Button")) && HasCompactReadableButton(skillBackButton));

        Image paladinBody = FindImage("Ally Slot 1 Body");
        Image clericBody = FindImage("Ally Slot 2 Body");
        Image rangerBody = FindImage("Ally Slot 3 Body");
        Image orcBody = FindImage("Enemy Slot 1 Body");
        Image skeletonBody = FindImage("Enemy Slot 2 Body");
        Image goblinBody = FindImage("Enemy Slot 3 Body");
        AppendCheck(ref passed, ref report, "front middle rear perspective scales are 105 95 85 percent", paladinBody != null && clericBody != null && rangerBody != null && goblinBody != null && skeletonBody != null && orcBody != null && Mathf.Approximately(paladinBody.rectTransform.sizeDelta.y, 168f) && Mathf.Approximately(clericBody.rectTransform.sizeDelta.y, 152f) && Mathf.Approximately(rangerBody.rectTransform.sizeDelta.y, 136f) && Mathf.Approximately(orcBody.rectTransform.sizeDelta.y, 168f) && Mathf.Approximately(skeletonBody.rectTransform.sizeDelta.y, 152f) && Mathf.Approximately(goblinBody.rectTransform.sizeDelta.y, 136f) && goblinBody.rectTransform.sizeDelta.y < orcBody.rectTransform.sizeDelta.y);
        AppendCheck(ref passed, ref report, "formation follows the new lower-left to upper-right tactical lane", paladinBody != null && paladinBody.rectTransform.anchoredPosition == new Vector2(-80f, 12f) && clericBody.rectTransform.anchoredPosition == new Vector2(-205f, -52f) && rangerBody.rectTransform.anchoredPosition == new Vector2(-340f, -112f) && orcBody.rectTransform.anchoredPosition == new Vector2(82f, 32f) && skeletonBody.rectTransform.anchoredPosition == new Vector2(220f, 94f) && goblinBody.rectTransform.anchoredPosition == new Vector2(350f, 150f));
        bool preserved = paladinBody != null && clericBody != null && rangerBody != null && goblinBody != null && skeletonBody != null && orcBody != null && paladinBody.preserveAspect && clericBody.preserveAspect && rangerBody.preserveAspect && goblinBody.preserveAspect && skeletonBody.preserveAspect && orcBody.preserveAspect;
        AppendCheck(ref passed, ref report, "all six bodies preserve aspect ratio", preserved);
        bool hpWidths = HasHpWidthRatio("Ally Slot 1", paladinBody) && HasHpWidthRatio("Ally Slot 2", clericBody) && HasHpWidthRatio("Ally Slot 3", rangerBody) && HasHpWidthRatio("Enemy Slot 1", orcBody) && HasHpWidthRatio("Enemy Slot 2", skeletonBody) && HasHpWidthRatio("Enemy Slot 3", goblinBody);
        AppendCheck(ref passed, ref report, "all HP bars are 80-100 percent of body rect width", hpWidths);
        AppendCheck(ref passed, ref report, "selection indicators render above bodies and below HP bars spatially", IsIndicatorLayeredAtFeet("Ally Slot 1", paladinBody) && IsIndicatorLayeredAtFeet("Ally Slot 2", clericBody) && IsIndicatorLayeredAtFeet("Ally Slot 3", rangerBody) && IsIndicatorLayeredAtFeet("Enemy Slot 1", orcBody) && IsIndicatorLayeredAtFeet("Enemy Slot 2", skeletonBody) && IsIndicatorLayeredAtFeet("Enemy Slot 3", goblinBody));
        if (battle != null && ui != null)
        {
            battle.DebugStartBattleForTest();
            AppendCheck(ref passed, ref report, "runtime does not recreate a right-side enemy element badge", FindTextIncludingInactive("Enemy Element Badge") == null);
            AppendCheck(ref passed, ref report, "runtime starts with visual IDs for all six exact roles", battle.DebugPlayerPartyCount == 3 && battle.DebugEnemyPartyCount == 3 && battle.playerParty[0].visualId == BattleVisualId.HeroPaladin && battle.playerParty[1].visualId == BattleVisualId.GuardianCleric && battle.playerParty[2].visualId == BattleVisualId.ScoutRanger && battle.enemyParty[0].visualId == BattleVisualId.Goblin && battle.enemyParty[1].visualId == BattleVisualId.Skeleton && battle.enemyParty[2].visualId == BattleVisualId.Orc);
            AppendCheck(ref passed, ref report, "six live slot bodies use the exact distinct BattleUnits sprites", ui.DebugAllySlotSpriteName(0) == "ally_paladin" && ui.DebugAllySlotSpriteName(1) == "ally_cleric" && ui.DebugAllySlotSpriteName(2) == "ally_ranger" && ui.DebugEnemySlotSpriteName(0) == "enemy_orc" && ui.DebugEnemySlotSpriteName(1) == "enemy_skeleton" && ui.DebugEnemySlotSpriteName(2) == "enemy_goblin");
            AppendCheck(ref passed, ref report, "slot labels exactly match CharacterData names without legacy Slime or HP word", ui.DebugAllySlotState(0).Contains("Paladin 100/100") && ui.DebugAllySlotState(1).Contains("Cleric 120/120") && ui.DebugAllySlotState(2).Contains("Ranger 85/85") && ui.DebugEnemySlotState(0).Contains("Orc Berserker") && ui.DebugEnemySlotState(1).Contains("Skeleton") && ui.DebugEnemySlotState(2).Contains("Goblin") && !ui.DebugPartyState.Contains("Slime") && !ui.DebugAllySlotState(0).Contains(" HP "));
            FindButton("Ally Slot 2 Body")?.onClick.Invoke();
            FindButton("Enemy Slot 3 Body")?.onClick.Invoke();
            AppendCheck(ref passed, ref report, "visual slot buttons retain one actor and one target ring bound to CharacterData", battle.DebugSelectedPlayerIndex == 1 && battle.DebugSelectedEnemyIndex == 0 && ui.DebugAllySlotSelected(1) && ui.DebugEnemySlotTargeted(2) && ui.DebugActiveAllyIndicatorCount == 1 && ui.DebugActiveEnemyIndicatorCount == 1 && battle.DebugMessageText == "Target: Goblin");
            battle.DebugSetCurrentHpForTest(true, 1, 0);
            battle.DebugSetCurrentHpForTest(false, 2, 0);
            AppendCheck(ref passed, ref report, "dead slots are non-interactable and render DEAD", !ui.DebugAllySlotInteractable(1) && !ui.DebugEnemySlotInteractable(0) && ui.DebugAllySlotState(1).Contains("DEAD") && ui.DebugEnemySlotState(0).Contains("DEAD"));
        }
        report += passed ? "\nRESULT: PASS" : "\nRESULT: FAIL";
        Debug.Log(report);
        EditorUtility.DisplayDialog(passed ? "BattleScene Test Passed" : "BattleScene Test Failed", report, "OK");
        if (!passed) throw new System.InvalidOperationException(report);
    }

    private static bool HasCompactReadableButton(Button button)
    {
        if (button == null) return false;
        float height = button.GetComponent<RectTransform>().sizeDelta.y;
        TMP_Text label = button.GetComponentInChildren<TMP_Text>(true);
        return height >= 44f && height <= 52f && label != null && label.fontSize >= 16f;
    }

    private static bool HasHpWidthRatio(string slotName, Image body)
    {
        Slider hp = FindSlider(slotName + " HP Slider");
        if (body == null || hp == null || body.rectTransform.sizeDelta.x <= 0f) return false;
        float ratio = hp.GetComponent<RectTransform>().sizeDelta.x / body.rectTransform.sizeDelta.x;
        return ratio >= 0.80f && ratio <= 1.0f;
    }

    private static bool IsIndicatorLayeredAtFeet(string slotName, Image body)
    {
        Image indicator = FindImageIncludingInactive(slotName + " Indicator");
        Slider hp = FindSlider(slotName + " HP Slider");
        if (body == null || indicator == null || hp == null) return false;
        float bodyBottom = body.rectTransform.anchoredPosition.y - body.rectTransform.sizeDelta.y * 0.5f;
        float indicatorY = indicator.rectTransform.anchoredPosition.y;
        float hpY = hp.GetComponent<RectTransform>().anchoredPosition.y;
        return indicator.transform.GetSiblingIndex() > body.transform.GetSiblingIndex() && Mathf.Abs(indicatorY - bodyBottom) <= 12f && indicatorY > hpY + 6f;
    }

    private static bool HasExactlyThreeObjectReferences(SerializedObject target, string field)
    {
        SerializedProperty property = target != null ? target.FindProperty(field) : null;
        if (property == null || !property.isArray || property.arraySize != 3) return false;
        for (int i = 0; i < 3; i++) if (property.GetArrayElementAtIndex(i).objectReferenceValue == null) return false;
        return true;
    }

    [MenuItem("Tools/Tactical Requiem/Validate Battle Test Scene")]
    public static void ValidateBattleTestScene()
    {
        ValidateBattleSlotIntegration();
#if false

        if (!System.IO.File.Exists(ScenePath))
        {
            EditorUtility.DisplayDialog("BattleScene Test Failed", "BattleScene file does not exist.\n\nRun Tools > Tactical Requiem > Create Battle Test Scene first.", "OK");
            return;
        }

        EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);

        bool passed = true;
        string report = "BattleScene Auto Test\n\n";

        BattleManager battleManager = Object.FindObjectOfType<BattleManager>();
        Canvas canvas = Object.FindObjectOfType<Canvas>();
        EventSystem eventSystem = Object.FindObjectOfType<EventSystem>();
        Camera mainCamera = Camera.main;

        AppendCheck(ref passed, ref report, "Main Camera exists", mainCamera != null);
        AppendCheck(ref passed, ref report, "Canvas exists", canvas != null);
        AppendCheck(ref passed, ref report, "EventSystem exists", eventSystem != null);
        AppendCheck(ref passed, ref report, "BattleManager exists", battleManager != null);

        Button attackButton = FindButtonIncludingInactive("Attack Button");
        Button fireSkillButton = FindButtonIncludingInactive("Fire Skill Button");
        Button iceSkillButton = FindButtonIncludingInactive("Ice Lance Button");
        Button lightningSkillButton = FindButtonIncludingInactive("Lightning Strike Button");
        Button earthSkillButton = FindButtonIncludingInactive("Earth Wall Button");
        Button guardButton = FindButtonIncludingInactive("Guard Button");
        Button endTurnButton = FindButtonIncludingInactive("End Turn Button");
        Button retryButton = FindButtonIncludingInactive("Retry Button");
        Button continueButton = FindButtonIncludingInactive("Continue Button");
        Button stageSelectButton = FindButtonIncludingInactive("Stage Select Button");
        Button speedToggleButton = FindButtonIncludingInactive("Speed Toggle Button");
        Button autoBattleButton = FindButtonIncludingInactive("Auto Battle Button");
        Button itemButton = FindButtonIncludingInactive("Item Button");
        Button battleLogToggleButton = FindButtonIncludingInactive("Battle Log Toggle Button");
        Button playerSelectButton = FindButtonIncludingInactive("Player Select Button");
        Image attackButtonGoldEdge = FindImageIncludingInactive("Attack Button Gold Edge");
        Image fireButtonTopHighlight = FindImageIncludingInactive("Fire Skill Button Top Highlight");
        Image continueButtonGoldEdge = FindImageIncludingInactive("Continue Button Gold Edge");
        Image actionCommandPanel = FindImageIncludingInactive("Action Command Panel");
        Image playerSelectionHighlight = FindImageIncludingInactive("Player Selection Highlight");
        TMP_Text selectedUnitText = FindTextIncludingInactive("Selected Unit Text");
        Image playerSpriteImage = FindImage("Player Sprite");
        Image enemySpriteImage = FindImage("Enemy Sprite");
        TMP_Text playerHpText = FindText("Player HP Text");
        TMP_Text playerApText = FindText("Player AP Text");
        TMP_Text enemyHpText = FindText("Enemy HP Text");
        Slider playerHpSlider = FindSlider("Player HP Slider");
        Slider playerApSlider = FindSlider("Player AP Slider");
        Slider enemyHpSlider = FindSlider("Enemy HP Slider");
        TMP_Text battleGuideText = FindText("Battle Guide Text");
        TMP_Text runStatusText = FindText("Run Status Text");
        TMP_Text stageText = FindText("Stage Text");
        TMP_Text stageObjectiveText = FindText("Stage Objective Text");
        TMP_Text stageProgressText = FindText("Stage Progress Text");
        TMP_Text playerStatusText = FindText("Player Status Text");
        TMP_Text skillHelpText = FindText("Skill Help Text");
        TMP_Text enemyStatusText = FindText("Enemy Status Text");
        TMP_Text enemyIntentText = FindText("Enemy Intent Text");
        TMP_Text enemyBreakText = FindText("Enemy Break Text");
        Slider enemyBreakSlider = FindSlider("Enemy Break Slider");
        TMP_Text messageText = FindText("Message Text");
        TMP_Text battleLogTitleText = FindTextIncludingInactive("Battle Log Title Text");
        TMP_Text battleLogText = FindTextIncludingInactive("Battle Log Text");
        Image battleLogPanel = FindImageIncludingInactive("Battle Log Panel");
        Image battleStageBackdropPanel = FindImage("Battle Stage Backdrop Panel");
        Image battleStageFloorPanel = FindImage("Battle Stage Floor Panel");
        Image topGoldDividerPanel = FindImage("Top Gold Divider Panel");
        Image commandGoldDividerPanel = FindImage("Command Gold Divider Panel");
        TMP_Text playerCardTitleText = FindText("Player Card Title Text");
        TMP_Text enemyCardTitleText = FindText("Enemy Card Title Text");
        TMP_Text versusDividerText = FindText("Versus Divider Text");
        Image playerPortraitPixelAccent1 = FindImage("Player Portrait Pixel Accent 1");
        Image playerPortraitPixelAccent4 = FindImage("Player Portrait Pixel Accent 4");
        Image enemyPortraitPixelAccent1 = FindImage("Enemy Portrait Pixel Accent 1");
        Image enemyPortraitPixelAccent4 = FindImage("Enemy Portrait Pixel Accent 4");
        Image topStatusPanel = FindImage("Top Status Panel");
        Image playerCardPanel = FindImage("Player Card Panel");
        Image enemyCardPanel = FindImage("Enemy Card Panel");
        Image battleCenterPanel = FindImage("Battle Center Panel");
        Image commandBarPanel = FindImage("Command Bar Panel");
        Image partyRosterPanel = FindImage("Party Roster Panel");
        Image partyRosterSlot1 = FindImage("Party Roster Slot 1");
        Image enemyRosterSlot1 = FindImageIncludingInactive("Enemy Roster Slot 1");
        Image partyRosterMiniSprite1 = FindImage("Party Roster Mini Sprite 1");
        Image partyRosterMiniSpriteShadow1 = FindImage("Party Roster Mini Sprite Shadow 1");
        Image partyRosterMiniSpriteEdge1 = FindImage("Party Roster Mini Sprite Edge Accent 1");
        Image enemyRosterMiniSprite1 = FindImageIncludingInactive("Enemy Roster Mini Sprite 1");
        Image enemyRosterMiniSpriteShadow1 = FindImageIncludingInactive("Enemy Roster Mini Sprite Shadow 1");
        Image enemyRosterMiniSpriteEdge1 = FindImageIncludingInactive("Enemy Roster Mini Sprite Edge Accent 1");
        Image tacticalGridTile = FindImage("Tactical Grid Tile 1-1");
        Image skillActionArc = FindImage("Skill Action Arc");
        Image heroStandeeBody = FindImage("Hero Standee Body");
        Image heroStandeeBlade = FindImage("Hero Standee Blade");
        Image enemyStandeeBody = FindImage("Enemy Standee Body");
        Image allyFormationUnit1 = FindImage("Ally Formation Unit 1 Body");
        Image allyFormationUnit2 = FindImage("Ally Formation Unit 2 Body");
        Image enemyFormationUnit1 = FindImage("Enemy Formation Unit 1 Body");
        Image enemyFormationUnit2 = FindImage("Enemy Formation Unit 2 Body");
        Image enemyStandeeCrown = FindImage("Enemy Standee Crown");
        Image commandHeaderPanel = FindImageIncludingInactive("Command Header Panel");
        TMP_Text commandHeaderText = FindTextIncludingInactive("Command Header Text");
        Image skillTierBadge = FindImageIncludingInactive("Skill Tier Badge");
        TMP_Text resultSummaryText = FindTextIncludingInactive("Result Summary Text");
        Image resultSummaryPanel = FindImageIncludingInactive("Result Summary Panel");
        Image commandPreviewPanel = FindImageIncludingInactive("Command Preview Panel");
        TMP_Text commandPreviewText = FindTextIncludingInactive("Command Preview Text");
        TMP_Text commandHintText = FindText("Command Hint Text");
        Image referenceSkillDetailPanel = FindImageIncludingInactive("Reference Skill Detail Panel");
        TMP_Text referenceSkillDetailText = FindTextIncludingInactive("Reference Skill Detail Text");
        Image enemyIntentCardPanel = FindImage("Enemy Intent Card Panel");
        TMP_Text enemyIntentCardText = FindText("Enemy Intent Card Text");
        Image turnBannerPanel = FindImageIncludingInactive("Turn Banner Panel");
        TMP_Text turnBannerText = FindTextIncludingInactive("Turn Banner Text");
        TMP_Text impactText = FindText("Impact Text");
        Image demoRoutePanel = FindImage("Demo Route Panel");
        TMP_Text demoRouteText = FindText("Demo Route Text");
        Image captureRehearsalPanel = FindImage("Capture Rehearsal Panel");
        TMP_Text captureRehearsalText = FindText("Capture Rehearsal Text");
        Image distantForestSilhouette = FindImage("Distant Forest Silhouette Panel");
        Image forestRuinsGroundRidge = FindImage("Forest Ruins Ground Ridge");
        Image forestRuinsLeftPillar = FindImage("Forest Ruins Left Pillar");
        Image forestRuinsRightObelisk = FindImage("Forest Ruins Right Obelisk");
        Image forestRuinsFallenSlab = FindImage("Forest Ruins Fallen Slab");
        Image moonlightBeam = FindImage("Moonlight Beam Panel");
        Image foregroundFog = FindImage("Foreground Fog Panel");
        Image heroCinematicSpotlight = FindImage("Hero Cinematic Spotlight");
        Image enemyCinematicSpotlight = FindImage("Enemy Cinematic Spotlight");
        Image centerClashGlow = FindImage("Center Clash Glow Panel");
        Image floorSpecularHighlight = FindImage("Floor Specular Highlight Panel");
        Image foregroundTreeLeft = FindImage("Foreground Tree Pillar Left Panel");
        Image foregroundTreeRight = FindImage("Foreground Tree Pillar Right Panel");
        Image lowerFogBand = FindImage("Lower Battle Fog Band Panel");
        Image upperCanopyShadow = FindImage("Upper Canopy Shadow Panel");
        Image heroBaseRing = FindImage("Hero Base Ring Panel");
        Image enemyBaseRing = FindImage("Enemy Base Ring Panel");
        Image heroStandeeShadow = FindImage("Hero Standee Shadow");
        Image enemyStandeeShadow = FindImage("Enemy Standee Shadow");
        Image heroStandeeAura = FindImage("Hero Standee Aura");
        Image enemyStandeeAura = FindImage("Enemy Standee Aura");
        Image heroContactGlow = FindImage("Hero Contact Glow Panel");
        Image enemyContactGlow = FindImage("Enemy Contact Glow Panel");
        Image heroStandeeRimLight = FindImage("Hero Standee Rim Light");
        Image enemyStandeeRimLight = FindImage("Enemy Standee Rim Light");
        Image centerActionSlashTrail = FindImage("Center Action Slash Trail");
        Image progressSkillCard1 = FindImage("Progress Skill Card 1");
        Image progressSkillIconFrame1 = FindImage("Progress Skill Icon Frame 1");
        Image progressSkillCardTopHighlight1 = FindImage("Progress Skill Card Top Highlight 1");
        Image progressTurnDial = FindImage("Progress Turn Dial");
        Image progressBottomPortrait1 = FindImage("Progress Bottom Portrait Sprite 1");
        Image battleLetterboxTop = FindImage("Battle Letterbox Top Panel");
        Image battleLetterboxBottom = FindImage("Battle Letterbox Bottom Panel");
        Image battlefieldInnerFrame = FindImage("Battlefield Inner Gold Frame Panel");
        Image heroLandingTile = FindImage("Hero Premium Landing Tile Panel");
        Image enemyLandingTile = FindImage("Enemy Premium Landing Tile Panel");
        Image fieldDepthBloom = FindImage("Field Depth Bloom Panel");
        Image topStatusPanelTopGloss = FindImage("Top Status Panel Top Gloss");
        Image playerCardPanelLeftRim = FindImage("Player Card Panel Left Rim");
        Image commandBarPanelTopShade = FindImage("Command Bar Panel Top Shade");
        Image runStatusChip = FindImage("Run Status Chip Panel");
        Image stageChip = FindImage("Stage Chip Panel");
        Image bottomResourceStrip = FindImage("Bottom Resource Strip Panel");
        Image playerHpChipPanel = FindImage("Player HP Chip Panel");
        Image playerApChipPanel = FindImage("Player AP Chip Panel");
        Image commandHintChipPanel = FindImage("Command Hint Chip Panel");
        Image skillDetailPanel = FindImage("Skill Detail Panel");
        Image enemyHpChipPanel = FindImage("Enemy HP Chip Panel");
        Image enemyStatusChipPanel = FindImage("Enemy Status Chip Panel");
        Image enemyIntentChipPanel = FindImage("Enemy Intent Chip Panel");
        Image enemyBreakChipPanel = FindImage("Enemy Break Chip Panel");
        Image enemyBreakChipPinkEdge = FindImage("Enemy Break Chip Pink Edge");
        Image rainStreak1 = FindImage("Battle Rain Streak 1");
        Image parallaxLeaf3 = FindImage("Battle Parallax Leaf 3");
        Image bossPhasePanel = FindImage("Boss Phase Telegraph Panel");
        TMP_Text bossPhaseText = FindText("Boss Phase Telegraph Text");
        Image comboMeterPanel = FindImage("Combo Meter Panel");
        Image comboMeterFill = FindImage("Combo Meter Fill Panel");
        Image turnTimelineRail = FindImage("Turn Timeline Rail Panel");
        Image turnTimelineNode1 = FindImage("Turn Timeline Node 1");
        TMP_Text damagePreviewText = FindText("Damage Preview Text");

        AppendCheck(ref passed, ref report, "Battle stage backdrop exists", battleStageBackdropPanel != null);
        AppendCheck(ref passed, ref report, "Battle stage backdrop has premium dark RPG styling", IsDecorativePanelLikelyConfigured(battleStageBackdropPanel, 1100f, 560f));
        AppendCheck(ref passed, ref report, "Battle stage floor glow exists", battleStageFloorPanel != null);
        AppendCheck(ref passed, ref report, "Central battlefield floor remains broad without a blocky board", IsDecorativePanelLikelyConfigured(battleStageFloorPanel, 820f, 180f));
        AppendCheck(ref passed, ref report, "Battle screen omits cinematic letterbox framing", IsHiddenForCapture(battleLetterboxTop) && IsHiddenForCapture(battleLetterboxBottom));
        AppendCheck(ref passed, ref report, "Battlefield omits a decorative inner frame", IsHiddenForCapture(battlefieldInnerFrame));
        AppendCheck(ref passed, ref report, "Capture view suppresses rectangular landing tiles", IsHiddenForCapture(heroLandingTile) && IsHiddenForCapture(enemyLandingTile));
        AppendCheck(ref passed, ref report, "Capture view suppresses rectangular depth-bloom overlay", IsHiddenForCapture(fieldDepthBloom));
        AppendCheck(ref passed, ref report, "Battlefield has layered forest silhouette", IsDecorativePanelLikelyConfigured(distantForestSilhouette, 680f, 70f));
        AppendCheck(ref passed, ref report, "Capture avoids blocky foreground prop overlays", IsHiddenForCapture(forestRuinsGroundRidge) && IsHiddenForCapture(forestRuinsLeftPillar) && IsHiddenForCapture(forestRuinsRightObelisk) && IsHiddenForCapture(forestRuinsFallenSlab));
        AppendCheck(ref passed, ref report, "Capture view suppresses rectangular moonlight beam", IsHiddenForCapture(moonlightBeam));
        AppendCheck(ref passed, ref report, "Battlefield has foreground fog layer", IsReadableContrastAccent(foregroundFog, 0.18f, 0.24f));
        AppendCheck(ref passed, ref report, "Capture view suppresses rectangular character spotlights", IsHiddenForCapture(heroCinematicSpotlight) && IsHiddenForCapture(enemyCinematicSpotlight));
        AppendCheck(ref passed, ref report, "Capture view suppresses rectangular clash highlights", IsHiddenForCapture(centerClashGlow) && IsHiddenForCapture(floorSpecularHighlight));
        AppendCheck(ref passed, ref report, "Capture avoids side prop overlays that compete with the formation", IsHiddenForCapture(foregroundTreeLeft) && IsHiddenForCapture(foregroundTreeRight) && IsHiddenForCapture(lowerFogBand) && IsReadableContrastAccent(upperCanopyShadow, 0.14f, 0.24f));
        AppendCheck(ref passed, ref report, "Capture view suppresses nonessential weather/parallax overlays", IsHiddenForCapture(rainStreak1) && IsHiddenForCapture(parallaxLeaf3));
        AppendCheck(ref passed, ref report, "Battlefield unit base rings align to landing tiles", IsDecorativePanelLikelyConfigured(heroBaseRing, 100f, 16f) && IsDecorativePanelLikelyConfigured(enemyBaseRing, 112f, 18f));
        AppendCheck(ref passed, ref report, "Battlefield selection rings are restrained and readable", IsReadableContrastAccent(heroBaseRing, 0.38f, 0.56f) && IsReadableContrastAccent(enemyBaseRing, 0.38f, 0.56f));
        AppendCheck(ref passed, ref report, "Battlefield standee grounding shadows are readable", IsReadableContrastAccent(heroStandeeShadow, 0.40f, 0.50f) && IsReadableContrastAccent(enemyStandeeShadow, 0.42f, 0.52f));
        AppendCheck(ref passed, ref report, "Battlefield standee aura stays nearly transparent behind readable sprites", IsReadableContrastAccent(heroStandeeAura, 0.04f, 0.07f) && IsReadableContrastAccent(enemyStandeeAura, 0.04f, 0.07f));
        AppendCheck(ref passed, ref report, "Battlefield units retain contact glow without rectangular rim bars", IsReadableContrastAccent(heroContactGlow, 0.22f, 0.30f) && IsReadableContrastAccent(enemyContactGlow, 0.20f, 0.30f) && IsHiddenForCapture(heroStandeeRimLight) && IsHiddenForCapture(enemyStandeeRimLight));
        AppendCheck(ref passed, ref report, "Capture view suppresses rectangular center slash trail", IsHiddenForCapture(centerActionSlashTrail));
        AppendCheck(ref passed, ref report, "Top lane omits long decorative dividers", topGoldDividerPanel != null && topGoldDividerPanel.rectTransform.sizeDelta.x <= 200f && IsHiddenForCapture(topGoldDividerPanel));
        AppendCheck(ref passed, ref report, "Command lane omits long decorative dividers", commandGoldDividerPanel != null && commandGoldDividerPanel.rectTransform.sizeDelta.x <= 200f && IsHiddenForCapture(commandGoldDividerPanel));
        AppendCheck(ref passed, ref report, "Capture view suppresses decorative tactical grid", IsHiddenForCapture(tacticalGridTile));
        AppendCheck(ref passed, ref report, "Capture view suppresses nonessential action arc", IsHiddenForCapture(skillActionArc));
        AppendCheck(ref passed, ref report, "Battlefield presents a readable diagonal 3v3 formation", IsSpriteImageLikelyConfigured(heroStandeeBody, 130f, 165f) && IsSpriteImageLikelyConfigured(enemyStandeeBody, 140f, 165f) && IsSpriteImageLikelyConfigured(allyFormationUnit1, 88f, 118f) && IsSpriteImageLikelyConfigured(allyFormationUnit2, 88f, 118f) && IsSpriteImageLikelyConfigured(enemyFormationUnit1, 88f, 118f) && IsSpriteImageLikelyConfigured(enemyFormationUnit2, 88f, 118f));
        AppendCheck(ref passed, ref report, "Hero scaled pixel standee remains readable without blade bar overlay", IsSpriteImageLikelyConfigured(heroStandeeBody, 130f, 165f) && IsHiddenForCapture(heroStandeeBlade));
        AppendCheck(ref passed, ref report, "Enemy scaled pixel standee is grounded on ring", IsSpriteImageLikelyConfigured(enemyStandeeBody, 140f, 165f) && IsReadableContrastAccent(enemyStandeeCrown, 0.20f, 0.30f));
        AppendCheck(ref passed, ref report, "StageData enemy visual variants use extracted reference sprites", StageData.CreateStage1Normal().enemies[0].visualVariant == EnemyVisualVariant.Goblin && StageData.CreateStage1Boss().enemies[0].visualVariant == EnemyVisualVariant.Skeleton && StageData.CreateStage3Normal().enemies[0].visualVariant == EnemyVisualVariant.Golem && StageData.CreateStage5Normal().enemies[0].visualVariant == EnemyVisualVariant.Lich);
        AppendCheck(ref passed, ref report, "Battle portraits have idle bob and hit reaction motion", HasBattleSpriteMotion(playerSpriteImage) && HasBattleSpriteMotion(enemySpriteImage));
        AppendCheck(ref passed, ref report, "Battlefield standees have idle bob motion", HasBattleSpriteMotion(heroStandeeBody) && HasBattleSpriteMotion(enemyStandeeBody));
        AppendCheck(ref passed, ref report, "Premium command header exists", IsDecorativePanelLikelyConfigured(commandHeaderPanel, 240f, 24f) && IsNameplateTextLikelyConfigured(commandHeaderText, "COMMAND", "CHAIN"));
        AppendCheck(ref passed, ref report, "Skill tier badge exists", IsDecorativePanelLikelyConfigured(skillTierBadge, 56f, 20f));
        AppendCheck(ref passed, ref report, "Left ally rail stays compact with portrait and short status", partyRosterPanel != null && partyRosterPanel.rectTransform.sizeDelta.x <= 160f && partyRosterSlot1 != null && partyRosterSlot1.rectTransform.sizeDelta.x <= 152f && partyRosterMiniSprite1 != null && partyRosterMiniSprite1.rectTransform.sizeDelta.x <= 40f);
        AppendCheck(ref passed, ref report, "Enemy roster slots exist", IsDecorativePanelLikelyConfigured(enemyRosterSlot1, 150f, 50f));
        AppendCheck(ref passed, ref report, "Party roster mini sprites remain readable at compact scale", IsSpriteImageLikelyConfigured(partyRosterMiniSprite1, 32f, 38f));
        AppendCheck(ref passed, ref report, "Party roster mini-sprite crop frame and shadow are readable", IsReadableContrastAccent(partyRosterMiniSpriteShadow1, 0.26f, 0.34f) && IsReadableContrastAccent(partyRosterMiniSpriteEdge1, 0.30f, 0.48f));
        AppendCheck(ref passed, ref report, "Player roster select button exists", IsButtonLikelyConfigured(playerSelectButton));
        AppendCheck(ref passed, ref report, "Player selection highlight starts hidden", playerSelectionHighlight != null && !playerSelectionHighlight.gameObject.activeSelf);
        AppendCheck(ref passed, ref report, "Bounded three-action command strip starts hidden until ally click", IsOverlayPanelLikelyConfigured(actionCommandPanel, 320f, 70f) && actionCommandPanel != null && !actionCommandPanel.gameObject.activeSelf);
        AppendCheck(ref passed, ref report, "Selected unit prompt starts hidden with command UI", selectedUnitText != null && !selectedUnitText.gameObject.activeSelf);
        AppendCheck(ref passed, ref report, "Enemy roster high-density mini sprites exist", IsSpriteImageLikelyConfigured(enemyRosterMiniSprite1, 32f, 40f));
        AppendCheck(ref passed, ref report, "Enemy roster mini-sprite crop frame and shadow are readable", IsReadableContrastAccent(enemyRosterMiniSpriteShadow1, 0.36f, 0.52f) && IsReadableContrastAccent(enemyRosterMiniSpriteEdge1, 0.62f, 0.82f));
        AppendCheck(ref passed, ref report, "Player card title is a compact party header", playerCardTitleText != null && playerCardTitleText.text.Contains("PARTY"));
        AppendCheck(ref passed, ref report, "Enemy card title exists", IsNameplateTextLikelyConfigured(enemyCardTitleText, "ENEMY", "ENEMY"));
        AppendCheck(ref passed, ref report, "Battle line divider text removed from center field", versusDividerText != null && string.IsNullOrEmpty(versusDividerText.text));
        AppendCheck(ref passed, ref report, "Player portrait pixel accents exist", IsPortraitAccentLikelyConfigured(playerPortraitPixelAccent1) && IsPortraitAccentLikelyConfigured(playerPortraitPixelAccent4));
        AppendCheck(ref passed, ref report, "Enemy portrait pixel accents exist", IsPortraitAccentLikelyConfigured(enemyPortraitPixelAccent1) && IsPortraitAccentLikelyConfigured(enemyPortraitPixelAccent4));
        AppendCheck(ref passed, ref report, "Top Status panel exists", topStatusPanel != null);
        AppendCheck(ref passed, ref report, "Top Status panel is a compact peripheral status line", IsProfessionalPanelLikelyConfigured(topStatusPanel, 1000f, 28f) && topStatusPanel.rectTransform.sizeDelta.y <= 32f);
        AppendCheck(ref passed, ref report, "Capture removes nonessential panel gloss and rim accents", IsHiddenForCapture(topStatusPanelTopGloss) && IsHiddenForCapture(playerCardPanelLeftRim) && IsHiddenForCapture(commandBarPanelTopShade));
        AppendCheck(ref passed, ref report, "Top lane hides redundant run chips and retains stage lane", IsHiddenForCapture(runStatusChip) && IsReadableContrastAccent(stageChip, 0.50f, 0.58f));
        AppendCheck(ref passed, ref report, "Player Card panel exists", playerCardPanel != null);
        AppendCheck(ref passed, ref report, "Player Card panel remains a compact secondary rail", IsProfessionalPanelLikelyConfigured(playerCardPanel, 120f, 160f) && playerCardPanel.rectTransform.sizeDelta.x <= 130f);
        AppendCheck(ref passed, ref report, "Enemy Card panel exists", enemyCardPanel != null);
        AppendCheck(ref passed, ref report, "Enemy Card panel remains a compact secondary rail", IsProfessionalPanelLikelyConfigured(enemyCardPanel, 120f, 150f) && enemyCardPanel.rectTransform.sizeDelta.x <= 130f);
        AppendCheck(ref passed, ref report, "Battle Center panel exists", battleCenterPanel != null);
        AppendCheck(ref passed, ref report, "Top lane consolidates stage, current turn, and queue above formation", battleCenterPanel != null && battleCenterPanel.rectTransform.sizeDelta.x >= 500f && battleCenterPanel.rectTransform.sizeDelta.y <= 24f && battleCenterPanel.color.a >= 0.30f);
        AppendCheck(ref passed, ref report, "Command Bar panel exists", commandBarPanel != null);
        AppendCheck(ref passed, ref report, "Command Bar panel is a compact contextual strip inside a 1080 capture", IsProfessionalPanelLikelyConfigured(commandBarPanel, 400f, 70f) && commandBarPanel.rectTransform.anchoredPosition.y >= -300f);
        AppendCheck(ref passed, ref report, "Bottom strip prioritizes selected unit resources without overflow", IsHiddenForCapture(bottomResourceStrip) && IsReadableContrastAccent(playerHpChipPanel, 0.28f, 0.34f) && IsReadableContrastAccent(playerApChipPanel, 0.28f, 0.34f));
        AppendCheck(ref passed, ref report, "Battle Guide text exists", battleGuideText != null);
        AppendCheck(ref passed, ref report, "Battle Guide text is compact for capture readability", IsBattleGuideTextLikelyConfigured(battleGuideText));
        AppendCheck(ref passed, ref report, "Run Status text exists", runStatusText != null);
        AppendCheck(ref passed, ref report, "Run Status text shows the current stage run", IsRunStatusTextLikelyConfigured(runStatusText));
        AppendCheck(ref passed, ref report, "Stage text exists", stageText != null);
        AppendCheck(ref passed, ref report, "Stage text starts at the first encounter", IsStageTextLikelyConfigured(stageText));
        AppendCheck(ref passed, ref report, "Stage Objective text exists", stageObjectiveText != null);
        AppendCheck(ref passed, ref report, "Stage Objective text explains the first objective", IsStageObjectiveTextLikelyConfigured(stageObjectiveText));
        AppendCheck(ref passed, ref report, "Stage Progress text exists", stageProgressText != null);
        AppendCheck(ref passed, ref report, "Stage Progress text serves as a compact turn queue", stageProgressText != null && stageProgressText.text.Contains("TURN") && stageProgressText.rectTransform.sizeDelta.x >= 130f);
        AppendCheck(ref passed, ref report, "Player Status text exists", playerStatusText != null);
        AppendCheck(ref passed, ref report, "Impact text exists", impactText != null);
        AppendCheck(ref passed, ref report, "Capture view suppresses reviewer route chip", IsHiddenForCapture(demoRoutePanel) && IsHiddenForCapture(demoRouteText));
        AppendCheck(ref passed, ref report, "Capture view suppresses rehearsal chip", IsHiddenForCapture(captureRehearsalPanel) && IsHiddenForCapture(captureRehearsalText));
        AppendCheck(ref passed, ref report, "Skill Help text exists", skillHelpText != null);
        AppendCheck(ref passed, ref report, "Runtime labels skip raycast for UI performance", IsTextRaycastOptimized(runStatusText, battleGuideText, stageText, stageObjectiveText, stageProgressText, playerHpText, playerApText, enemyHpText, enemyStatusText, enemyIntentText, enemyBreakText, skillHelpText, messageText, impactText, demoRouteText, captureRehearsalText));
        AppendCheck(ref passed, ref report, "Enemy Status text exists", enemyStatusText != null);
        AppendCheck(ref passed, ref report, "Enemy Intent text exists", enemyIntentText != null);
        AppendCheck(ref passed, ref report, "Enemy Break text exists", enemyBreakText != null);
        AppendCheck(ref passed, ref report, "Enemy Break slider exists", enemyBreakSlider != null);
        AppendCheck(ref passed, ref report, "Enemy card resources are framed as compact chips", IsReadableContrastAccent(enemyHpChipPanel, 0.28f, 0.34f) && IsReadableContrastAccent(enemyStatusChipPanel, 0.24f, 0.30f));
        AppendCheck(ref passed, ref report, "Enemy intent and break hierarchy uses tinted chips", IsReadableContrastAccent(enemyIntentChipPanel, 0.26f, 0.32f) && IsReadableContrastAccent(enemyBreakChipPanel, 0.24f, 0.30f) && IsReadableContrastAccent(enemyBreakChipPinkEdge, 0.30f, 0.38f));
        AppendCheck(ref passed, ref report, "Battle Log title exists", battleLogTitleText != null);
        AppendCheck(ref passed, ref report, "Battle Log text exists", battleLogText != null);
        AppendCheck(ref passed, ref report, "Battle Log panel exists", battleLogPanel != null);
        AppendCheck(ref passed, ref report, "Battle Log panel is readable", IsBattleLogPanelLikelyConfigured(battleLogPanel));
        AppendCheck(ref passed, ref report, "Battle Log panel starts collapsed", battleLogPanel != null && !battleLogPanel.gameObject.activeSelf);
        AppendCheck(ref passed, ref report, "Battle Log title starts collapsed", battleLogTitleText != null && !battleLogTitleText.gameObject.activeSelf);
        AppendCheck(ref passed, ref report, "Battle Log text starts collapsed", battleLogText != null && !battleLogText.gameObject.activeSelf);
        AppendCheck(ref passed, ref report, "Battle Log starts with recent-actions placeholder", IsBattleLogTextLikelyConfigured(battleLogText));
        AppendCheck(ref passed, ref report, "Result Summary text exists", resultSummaryText != null);
        AppendCheck(ref passed, ref report, "Result Summary panel exists", resultSummaryPanel != null);
        AppendCheck(ref passed, ref report, "Result Summary panel is configured but initially hidden", IsOverlayPanelLikelyConfigured(resultSummaryPanel, 600f, 200f) && resultSummaryPanel != null && !resultSummaryPanel.gameObject.activeSelf);
        AppendCheck(ref passed, ref report, "Command Preview panel exists", commandPreviewPanel != null);
        AppendCheck(ref passed, ref report, "Command Preview panel starts hidden and fits current skill detail", IsOverlayPanelLikelyConfigured(commandPreviewPanel, 490f, 50f) && commandPreviewPanel != null && commandPreviewPanel.rectTransform.anchoredPosition.y >= -280f && !commandPreviewPanel.gameObject.activeSelf);
        AppendCheck(ref passed, ref report, "Command Preview text exists", commandPreviewText != null);
        AppendCheck(ref passed, ref report, "Bottom command strip has visible select-unit hint", IsCommandHintTextLikelyConfigured(commandHintText));
        AppendCheck(ref passed, ref report, "Bottom command hint is framed as a subdued chip", IsReadableContrastAccent(commandHintChipPanel, 0.20f, 0.30f));
        AppendCheck(ref passed, ref report, "Current skill detail stays inside the bottom viewport", IsOverlayPanelLikelyConfigured(skillDetailPanel, 490f, 50f) && skillDetailPanel.rectTransform.anchoredPosition.y >= -280f);
        AppendCheck(ref passed, ref report, "Capture view suppresses reference-only skill detail", IsHiddenForCapture(referenceSkillDetailPanel) && IsHiddenForCapture(referenceSkillDetailText));
        AppendCheck(ref passed, ref report, "Right rail uses the live intent row instead of a duplicate intent card", IsHiddenForCapture(enemyIntentCardPanel) && IsHiddenForCapture(enemyIntentCardText));
        AppendCheck(ref passed, ref report, "Capture view suppresses reference combat readouts", IsHiddenForCapture(bossPhasePanel) && IsHiddenForCapture(bossPhaseText) && IsHiddenForCapture(comboMeterPanel) && IsHiddenForCapture(comboMeterFill) && IsHiddenForCapture(turnTimelineRail) && IsHiddenForCapture(turnTimelineNode1) && IsHiddenForCapture(damagePreviewText));
        AppendCheck(ref passed, ref report, "Capture view suppresses right reference skill cards", IsHiddenForCapture(progressSkillCard1) && IsHiddenForCapture(progressSkillIconFrame1) && IsHiddenForCapture(progressSkillCardTopHighlight1));
        AppendCheck(ref passed, ref report, "Bottom right duplicate battle-start CTA removed", FindImage("Progress Battle Start Panel") == null && FindText("Progress Battle Start Text") == null);
        AppendCheck(ref passed, ref report, "Capture view suppresses reference turn dial", IsHiddenForCapture(progressTurnDial));
        AppendCheck(ref passed, ref report, "Capture view suppresses reference portrait strip", IsHiddenForCapture(progressBottomPortrait1));
        AppendCheck(ref passed, ref report, "Turn Banner panel exists", turnBannerPanel != null);
        AppendCheck(ref passed, ref report, "Turn Banner panel starts hidden", turnBannerPanel != null && !turnBannerPanel.gameObject.activeSelf);
        AppendCheck(ref passed, ref report, "Turn Banner text exists", turnBannerText != null);
        AppendCheck(ref passed, ref report, "Attack button exists", attackButton != null);
        AppendCheck(ref passed, ref report, "Fire Skill button exists", fireSkillButton != null);
        AppendCheck(ref passed, ref report, "Ice Lance button exists", iceSkillButton != null);
        AppendCheck(ref passed, ref report, "Lightning Strike button exists", lightningSkillButton != null);
        AppendCheck(ref passed, ref report, "Earth Wall button exists", earthSkillButton != null);
        AppendCheck(ref passed, ref report, "Guard button exists", guardButton != null);
        AppendCheck(ref passed, ref report, "End Turn button exists", endTurnButton != null);
        AppendCheck(ref passed, ref report, "Retry button exists", retryButton != null);
        AppendCheck(ref passed, ref report, "Continue button exists", continueButton != null);
        AppendCheck(ref passed, ref report, "Stage Select button exists", stageSelectButton != null);
        AppendCheck(ref passed, ref report, "Speed Toggle button exists", speedToggleButton != null);
        AppendCheck(ref passed, ref report, "Auto Battle button exists", autoBattleButton != null);
        AppendCheck(ref passed, ref report, "Item button exists", itemButton != null);
        AppendCheck(ref passed, ref report, "Battle Log toggle button exists", battleLogToggleButton != null);
        AppendCheck(ref passed, ref report, "Battle Log toggle button is configured", IsButtonLikelyConfigured(battleLogToggleButton));
        AppendCheck(ref passed, ref report, "Command/result buttons have premium bevel material", IsDecorativePanelLikelyConfigured(attackButtonGoldEdge, 40f, 2f) && IsDecorativePanelLikelyConfigured(fireButtonTopHighlight, 38f, 2f) && IsDecorativePanelLikelyConfigured(continueButtonGoldEdge, 118f, 2f));
        AppendCheck(ref passed, ref report, "Player HP text includes percentage", IsResourceTextLikelyConfigured(playerHpText, "Hero HP", "100%"));
        AppendCheck(ref passed, ref report, "Player AP text includes percentage", IsResourceTextLikelyConfigured(playerApText, "AP", "100%"));
        AppendCheck(ref passed, ref report, "Enemy HP text includes percentage", IsResourceTextLikelyConfigured(enemyHpText, "Slime HP", "100%"));
        AppendCheck(ref passed, ref report, "Player HP slider exists", playerHpSlider != null);
        AppendCheck(ref passed, ref report, "Player AP slider exists", playerApSlider != null);
        AppendCheck(ref passed, ref report, "Enemy HP slider exists", enemyHpSlider != null);
        AppendCheck(ref passed, ref report, "Attack button starts hidden until ally click", IsButtonLikelyConfigured(attackButton) && attackButton != null && !attackButton.gameObject.activeSelf);
        AppendCheck(ref passed, ref report, "Fire Skill button starts hidden until ally click", IsButtonLikelyConfigured(fireSkillButton) && fireSkillButton != null && !fireSkillButton.gameObject.activeSelf);
        AppendCheck(ref passed, ref report, "Guard button starts hidden until ally click", IsButtonLikelyConfigured(guardButton) && guardButton != null && !guardButton.gameObject.activeSelf);
        AppendCheck(ref passed, ref report, "End Turn button starts hidden until ally click", IsButtonLikelyConfigured(endTurnButton) && endTurnButton != null && !endTurnButton.gameObject.activeSelf);
        AppendCheck(ref passed, ref report, "Retry button is configured but initially hidden", IsButtonLikelyConfigured(retryButton) && retryButton != null && !retryButton.gameObject.activeSelf);
        AppendCheck(ref passed, ref report, "Continue button is configured but initially hidden", IsButtonLikelyConfigured(continueButton) && continueButton != null && !continueButton.gameObject.activeSelf);
        AppendCheck(ref passed, ref report, "Player HP slider is configured", IsSliderLikelyConfigured(playerHpSlider));
        AppendCheck(ref passed, ref report, "Player AP slider is configured", IsSliderLikelyConfigured(playerApSlider));
        AppendCheck(ref passed, ref report, "Enemy HP slider is configured", IsSliderLikelyConfigured(enemyHpSlider));

        if (battleManager != null)
        {
            BattleUI battleUI = battleManager.GetComponent<BattleUI>();
            SerializedObject serializedBattleUI = battleUI != null ? new SerializedObject(battleUI) : null;
            AppendCheck(ref passed, ref report, "BattleUI component exists on BattleManager object", battleUI != null);
            AppendCheck(ref passed, ref report, "Action command panel linked", HasObjectReference(serializedBattleUI, "actionCommandPanel"));
            AppendCheck(ref passed, ref report, "Player select button linked", HasObjectReference(serializedBattleUI, "playerSelectButton"));
            AppendCheck(ref passed, ref report, "Player selection highlight linked", HasObjectReference(serializedBattleUI, "playerSelectionHighlight"));
            AppendCheck(ref passed, ref report, "Selected unit text linked", HasObjectReference(serializedBattleUI, "selectedUnitText"));
            AppendCheck(ref passed, ref report, "Player HP text linked", HasObjectReference(serializedBattleUI, "playerHpText"));
            AppendCheck(ref passed, ref report, "Player HP slider linked", HasObjectReference(serializedBattleUI, "playerHpSlider"));
            AppendCheck(ref passed, ref report, "Player AP text linked", HasObjectReference(serializedBattleUI, "playerApText"));
            AppendCheck(ref passed, ref report, "Player AP slider linked", HasObjectReference(serializedBattleUI, "playerApSlider"));
            AppendCheck(ref passed, ref report, "Player Status text linked", HasObjectReference(serializedBattleUI, "playerStatusText"));
            AppendCheck(ref passed, ref report, "Enemy HP text linked", HasObjectReference(serializedBattleUI, "enemyHpText"));
            AppendCheck(ref passed, ref report, "Enemy HP slider linked", HasObjectReference(serializedBattleUI, "enemyHpSlider"));
            AppendCheck(ref passed, ref report, "Enemy Status text linked", HasObjectReference(serializedBattleUI, "enemyStatusText"));
            AppendCheck(ref passed, ref report, "Enemy Intent text linked", HasObjectReference(serializedBattleUI, "enemyIntentText"));
            AppendCheck(ref passed, ref report, "Enemy Break text linked", HasObjectReference(serializedBattleUI, "enemyBreakText"));
            AppendCheck(ref passed, ref report, "Enemy Break slider linked", HasObjectReference(serializedBattleUI, "enemyBreakSlider"));
            AppendCheck(ref passed, ref report, "Enemy standee sprite linked for runtime visual variants", HasObjectReference(serializedBattleUI, "enemyStandeeImage"));
            AppendCheck(ref passed, ref report, "Enemy reference sprite variants linked", HasObjectReference(serializedBattleUI, "referenceGoblinSprite") && HasObjectReference(serializedBattleUI, "referenceSkeletonSprite") && HasObjectReference(serializedBattleUI, "referenceOrcSprite") && HasObjectReference(serializedBattleUI, "referenceLichSprite") && HasObjectReference(serializedBattleUI, "referenceGolemSprite") && HasObjectReference(serializedBattleUI, "referenceDarkKnightSprite"));
            AppendCheck(ref passed, ref report, "Run Status text linked", HasObjectReference(serializedBattleUI, "runStatusText"));
            AppendCheck(ref passed, ref report, "Stage text linked", HasObjectReference(serializedBattleUI, "stageText"));
            AppendCheck(ref passed, ref report, "Stage Objective text linked", HasObjectReference(serializedBattleUI, "stageObjectiveText"));
            AppendCheck(ref passed, ref report, "Stage Progress text linked", HasObjectReference(serializedBattleUI, "stageProgressText"));
            AppendCheck(ref passed, ref report, "Message text linked", HasObjectReference(serializedBattleUI, "messageText"));
            AppendCheck(ref passed, ref report, "Impact text linked", HasObjectReference(serializedBattleUI, "impactText"));
            AppendCheck(ref passed, ref report, "Capture rehearsal text linked", HasObjectReference(serializedBattleUI, "captureRehearsalText"));
            AppendCheck(ref passed, ref report, "Skill Help text linked", HasObjectReference(serializedBattleUI, "skillHelpText"));
            AppendCheck(ref passed, ref report, "Battle Log panel linked", HasObjectReference(serializedBattleUI, "battleLogPanel"));
            AppendCheck(ref passed, ref report, "Battle Log title linked", HasObjectReference(serializedBattleUI, "battleLogTitleText"));
            AppendCheck(ref passed, ref report, "Battle Log text linked", HasObjectReference(serializedBattleUI, "battleLogText"));
            AppendCheck(ref passed, ref report, "Battle Log toggle button linked", HasObjectReference(serializedBattleUI, "battleLogToggleButton"));
            AppendCheck(ref passed, ref report, "Battle Log toggle label linked", HasObjectReference(serializedBattleUI, "battleLogToggleLabel"));
            AppendCheck(ref passed, ref report, "Result Summary text linked", HasObjectReference(serializedBattleUI, "resultSummaryText"));
            AppendCheck(ref passed, ref report, "Result Summary panel linked", HasObjectReference(serializedBattleUI, "resultSummaryPanel"));
            AppendCheck(ref passed, ref report, "Command Preview panel linked", HasObjectReference(serializedBattleUI, "commandPreviewPanel"));
            AppendCheck(ref passed, ref report, "Command Preview text linked", HasObjectReference(serializedBattleUI, "commandPreviewText"));
            AppendCheck(ref passed, ref report, "Turn Banner panel linked", HasObjectReference(serializedBattleUI, "turnBannerPanel"));
            AppendCheck(ref passed, ref report, "Turn Banner text linked", HasObjectReference(serializedBattleUI, "turnBannerText"));
            AppendCheck(ref passed, ref report, "Attack button linked", HasObjectReference(serializedBattleUI, "attackButton"));
            AppendCheck(ref passed, ref report, "Fire Skill button linked", HasObjectReference(serializedBattleUI, "fireSkillButton"));
            AppendCheck(ref passed, ref report, "Guard button linked", HasObjectReference(serializedBattleUI, "guardButton"));
            AppendCheck(ref passed, ref report, "End Turn button linked", HasObjectReference(serializedBattleUI, "endTurnButton"));
            AppendCheck(ref passed, ref report, "Retry button linked", HasObjectReference(serializedBattleUI, "retryButton"));
            AppendCheck(ref passed, ref report, "Continue button linked", HasObjectReference(serializedBattleUI, "continueButton"));
        }

        report += passed ? "\nRESULT: PASS" : "\nRESULT: FAIL";
        EditorUtility.DisplayDialog(passed ? "BattleScene Test Passed" : "BattleScene Test Failed", report, "OK");
#endif
    }

    private static Button FindButton(string objectName)
    {
        GameObject buttonObject = GameObject.Find(objectName);
        return buttonObject != null ? buttonObject.GetComponent<Button>() : null;
    }

    private static Button FindButtonIncludingInactive(string objectName)
    {
        Button[] buttons = Resources.FindObjectsOfTypeAll<Button>();
        foreach (Button button in buttons)
        {
            if (button.gameObject.name == objectName)
            {
                return button;
            }
        }

        return null;
    }

    private static TMP_Text FindText(string objectName)
    {
        GameObject textObject = GameObject.Find(objectName);
        return textObject != null ? textObject.GetComponent<TMP_Text>() : null;
    }

    private static Slider FindSlider(string objectName)
    {
        GameObject sliderObject = GameObject.Find(objectName);
        return sliderObject != null ? sliderObject.GetComponent<Slider>() : null;
    }

    private static Image FindImage(string objectName)
    {
        GameObject imageObject = GameObject.Find(objectName);
        return imageObject != null ? imageObject.GetComponent<Image>() : null;
    }

    private static TMP_Text FindTextIncludingInactive(string objectName)
    {
        TMP_Text[] texts = Resources.FindObjectsOfTypeAll<TMP_Text>();
        foreach (TMP_Text text in texts)
        {
            if (text.gameObject.name == objectName)
            {
                return text;
            }
        }

        return null;
    }

    private static Image FindImageIncludingInactive(string objectName)
    {
        Image[] images = Resources.FindObjectsOfTypeAll<Image>();
        foreach (Image image in images)
        {
            if (image.gameObject.name == objectName)
            {
                return image;
            }
        }

        return null;
    }

    private static bool IsButtonLikelyVisible(Button button)
    {
        if (button == null)
        {
            return false;
        }

        RectTransform rectTransform = button.GetComponent<RectTransform>();
        return button.gameObject.activeSelf && IsButtonLikelyConfigured(button) && rectTransform.anchoredPosition.y >= 20f;
    }

    private static bool IsButtonLikelyConfigured(Button button)
    {
        if (button == null)
        {
            return false;
        }

        RectTransform rectTransform = button.GetComponent<RectTransform>();
        Image image = button.GetComponent<Image>();
        return rectTransform != null && image != null && button.targetGraphic == image && rectTransform.sizeDelta.x > 0 && rectTransform.sizeDelta.y > 0;
    }

    private static bool IsSliderLikelyConfigured(Slider slider)
    {
        if (slider == null)
        {
            return false;
        }

        RectTransform rectTransform = slider.GetComponent<RectTransform>();
        return slider.gameObject.activeSelf && rectTransform != null && rectTransform.sizeDelta.x > 0 && slider.fillRect != null && slider.targetGraphic != null;
    }

    private static bool IsResourceTextLikelyConfigured(TMP_Text resourceText, string label, string percentageToken)
    {
        if (resourceText == null)
        {
            return false;
        }

        return resourceText.text.Contains(label)
            && resourceText.text.Contains("/")
            && resourceText.text.Contains(percentageToken);
    }

    private static bool IsPanelLikelyConfigured(Image panelImage)
    {
        if (panelImage == null)
        {
            return false;
        }

        RectTransform rectTransform = panelImage.GetComponent<RectTransform>();
        return rectTransform != null && rectTransform.sizeDelta.x >= 900f && rectTransform.sizeDelta.y >= 100f && panelImage.color.a > 0.5f;
    }

    private static bool IsOverlayPanelLikelyConfigured(Image panelImage, float minimumWidth, float minimumHeight)
    {
        if (panelImage == null)
        {
            return false;
        }

        RectTransform rectTransform = panelImage.GetComponent<RectTransform>();
        return rectTransform != null
            && rectTransform.sizeDelta.x >= minimumWidth
            && rectTransform.sizeDelta.y >= minimumHeight
            && panelImage.color.a > 0.5f;
    }

    private static bool IsBattleLogPanelLikelyConfigured(Image panelImage)
    {
        if (panelImage == null)
        {
            return false;
        }

        RectTransform rectTransform = panelImage.GetComponent<RectTransform>();
        return rectTransform != null && rectTransform.sizeDelta.x >= 500f && rectTransform.sizeDelta.y >= 140f && panelImage.color.a > 0.5f;
    }

    private static bool IsProfessionalPanelLikelyConfigured(Image panelImage, float minimumWidth, float minimumHeight)
    {
        if (panelImage == null)
        {
            return false;
        }

        RectTransform rectTransform = panelImage.GetComponent<RectTransform>();
        Color color = panelImage.color;
        return rectTransform != null
            && rectTransform.sizeDelta.x >= minimumWidth
            && rectTransform.sizeDelta.y >= minimumHeight
            && color.a >= 0.40f
            && color.r <= 0.14f
            && color.g <= 0.14f
            && color.b <= 0.20f;
    }

    private static bool IsDecorativePanelLikelyConfigured(Image panelImage, float minimumWidth, float minimumHeight)
    {
        if (panelImage == null)
        {
            return false;
        }

        RectTransform rectTransform = panelImage.GetComponent<RectTransform>();
        Color color = panelImage.color;
        return rectTransform != null
            && rectTransform.sizeDelta.x >= minimumWidth
            && rectTransform.sizeDelta.y >= minimumHeight
            && color.a >= 0.35f;
    }

    private static bool IsCompactPartyRosterLikelyConfigured(Image rosterPanel, Image rosterSlot, Image miniSprite)
    {
        if (rosterPanel == null || rosterSlot == null || miniSprite == null)
        {
            return false;
        }

        RectTransform panelRect = rosterPanel.GetComponent<RectTransform>();
        RectTransform slotRect = rosterSlot.GetComponent<RectTransform>();
        RectTransform spriteRect = miniSprite.GetComponent<RectTransform>();
        return panelRect != null
            && slotRect != null
            && spriteRect != null
            && panelRect.sizeDelta.x >= 190f
            && panelRect.sizeDelta.x <= 205f
            && panelRect.sizeDelta.y >= 250f
            && panelRect.sizeDelta.y <= 270f
            && slotRect.sizeDelta.x >= 190f
            && slotRect.sizeDelta.x <= 200f
            && slotRect.sizeDelta.y >= 40f
            && slotRect.sizeDelta.y <= 45f
            && spriteRect.sizeDelta.x >= 32f
            && spriteRect.sizeDelta.x <= 40f
            && spriteRect.sizeDelta.y >= 38f
            && spriteRect.sizeDelta.y <= 44f;
    }

    private static bool IsHiddenForCapture(Graphic graphic)
    {
        return graphic != null && graphic.color.a <= 0.01f;
    }

    private static bool IsReadableContrastAccent(Image panelImage, float minimumAlpha, float maximumAlpha)
    {
        if (panelImage == null)
        {
            return false;
        }

        RectTransform rectTransform = panelImage.GetComponent<RectTransform>();
        Color color = panelImage.color;
        return rectTransform != null
            && rectTransform.sizeDelta.x > 0f
            && rectTransform.sizeDelta.y > 0f
            && color.a >= minimumAlpha
            && color.a <= maximumAlpha;
    }

    private static void ReduceCaptureNoise(Transform root)
    {
        string[] suppressedNames =
        {
            "Demo Route", "Capture Rehearsal", "Boss Phase Telegraph", "Combo Meter", "Turn Timeline", "Damage Preview",
            "Progress Skill", "Progress Turn", "Progress Bottom", "Reference Skill", "Tactical Grid Tile", "Skill Action Arc",
            "Rain Streak", "Parallax Leaf", "Fire Element", "Battle Guide", "Stage Objective",
            "Hero Cinematic Spotlight", "Enemy Cinematic Spotlight", "Field Depth Bloom", "Center Clash Glow", "Floor Specular Highlight",
            "Moonlight Beam", "Center Action Slash Trail", "Hero Premium Landing Tile", "Enemy Premium Landing Tile", "Hero Standee Blade", "Hero Standee Rim Light", "Enemy Standee Rim Light",
            "Forest Ruins", "Cinematic Left", "Cinematic Right", "Foreground Tree", "Lower Battle Fog", "Tactical Grid", "Skill Action Arc",
            "Professional Top", "Companion System", "Enemy Intent Card", "Reference Skill", "Command Glow", "Bottom Resource Strip", "Progress Chain",
            "Ally Formation Marker", "Enemy Formation Marker", "Top Gold Divider", "Command Gold Divider", "Rear Horizon Gold",
            "Battle Letterbox", "Battlefield Inner Gold", "Center Field Composition", "Top Status Panel Top Gloss", "Top Status Panel Bottom Shade",
            "Command Bar Panel Top Shade", "Command Bar Panel Bottom Depth", "Player Card Panel Left Rim", "Professional Top Right", "Stage Chip Top Edge", "Command Hint Chip Gold Edge", "Forest Shadow",
            "Party Roster Slot 2", "Party Roster Slot 3", "Party Roster Portrait Chip 2", "Party Roster Portrait Chip 3",
            "Party Roster Mini Sprite Shadow 2", "Party Roster Mini Sprite Shadow 3", "Party Roster Mini Sprite Crop Frame 2", "Party Roster Mini Sprite Crop Frame 3",
            "Party Roster Mini Sprite 2", "Party Roster Mini Sprite 3", "Party Roster Mini Sprite Edge Accent 2", "Party Roster Mini Sprite Edge Accent 3",
            "Party Roster Label 2", "Party Roster Label 3", "Party Roster Stat 2", "Party Roster Stat 3", "Party Roster Selected Gold Rim"
        };

        foreach (Image image in root.GetComponentsInChildren<Image>(true))
        {
            if (NameContainsAny(image.gameObject.name, suppressedNames))
            {
                Color color = image.color;
                color.a = 0f;
                image.color = color;
                image.raycastTarget = false;
            }
        }

        foreach (TMP_Text text in root.GetComponentsInChildren<TMP_Text>(true))
        {
            if (NameContainsAny(text.gameObject.name, suppressedNames))
            {
                Color color = text.color;
                color.a = 0f;
                text.color = color;
                text.raycastTarget = false;
            }
        }
    }

    private static bool NameContainsAny(string name, string[] fragments)
    {
        foreach (string fragment in fragments)
        {
            if (name.Contains(fragment))
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsSpriteImageLikelyConfigured(Image spriteImage, float minimumWidth, float minimumHeight)
    {
        if (spriteImage == null)
        {
            return false;
        }

        RectTransform rectTransform = spriteImage.GetComponent<RectTransform>();
        return rectTransform != null
            && rectTransform.sizeDelta.x >= minimumWidth
            && rectTransform.sizeDelta.y >= minimumHeight
            && spriteImage.sprite != null
            && spriteImage.color.a >= 0.95f;
    }

    private static bool HasBattleSpriteMotion(Image spriteImage)
    {
        if (spriteImage == null)
        {
            return false;
        }

        BattleSpriteMotion motion = spriteImage.GetComponent<BattleSpriteMotion>();
        return motion != null
            && motion.DebugProfile.Contains("Bob=")
            && motion.DebugProfile.Contains("Hit=")
            && motion.DebugProfile.Contains("Squash=");
    }

    private static bool IsNameplateTextLikelyConfigured(TMP_Text text, string firstToken, string secondToken)
    {
        if (text == null)
        {
            return false;
        }

        RectTransform rectTransform = text.GetComponent<RectTransform>();
        string value = text.text;
        return rectTransform != null
            && rectTransform.sizeDelta.x >= 100f
            && value.Contains(firstToken)
            && value.Contains(secondToken);
    }

    private static bool IsPortraitAccentLikelyConfigured(Image accentImage)
    {
        if (accentImage == null)
        {
            return false;
        }

        RectTransform rectTransform = accentImage.GetComponent<RectTransform>();
        return rectTransform != null
            && rectTransform.sizeDelta.x >= 8f
            && rectTransform.sizeDelta.y >= 8f
            && accentImage.color.a >= 0.3f;
    }

    private static bool IsCommandHintTextLikelyConfigured(TMP_Text hintText)
    {
        if (hintText == null)
        {
            return false;
        }

        RectTransform rectTransform = hintText.GetComponent<RectTransform>();
        string text = hintText.text;
        return rectTransform != null
            && rectTransform.sizeDelta.x >= 300f
            && text.Contains("Select Hero")
            && text.Contains("commands");
    }

    private static bool IsBattleGuideTextLikelyConfigured(TMP_Text guideText)
    {
        if (guideText == null)
        {
            return false;
        }

        RectTransform rectTransform = guideText.GetComponent<RectTransform>();
        string text = guideText.text;
        return rectTransform != null
            && rectTransform.sizeDelta.x >= 180f
            && text.Length <= 24
            && text.Contains("Push")
            && text.Contains("+25%")
            && text.Contains("HP");
    }

    private static bool IsDemoRouteTextLikelyConfigured(TMP_Text routeText)
    {
        if (routeText == null)
        {
            return false;
        }

        RectTransform rectTransform = routeText.GetComponent<RectTransform>();
        string text = routeText.text;
        return rectTransform != null
            && rectTransform.sizeDelta.x >= 160f
            && text.Length <= 28
            && text.Contains("HERO")
            && text.Contains("FIRE")
            && text.Contains("WIN");
    }

    private static bool IsCaptureRehearsalTextLikelyConfigured(TMP_Text rehearsalText)
    {
        if (rehearsalText == null)
        {
            return false;
        }

        RectTransform rectTransform = rehearsalText.GetComponent<RectTransform>();
        string text = rehearsalText.text;
        return rectTransform != null
            && rectTransform.sizeDelta.x >= 120f
            && text.Length <= 10
            && text.Contains("1/5")
            && text.Contains("SHOT");
    }

    private static bool IsStageTextLikelyConfigured(TMP_Text stageText)
    {
        if (stageText == null)
        {
            return false;
        }

        RectTransform rectTransform = stageText.GetComponent<RectTransform>();
        return rectTransform != null
            && rectTransform.sizeDelta.x >= 210f
            && stageText.text.Contains("BATTLE")
            && stageText.text.Contains("PREP");
    }

    private static bool IsRunStatusTextLikelyConfigured(TMP_Text runStatusText)
    {
        if (runStatusText == null)
        {
            return false;
        }

        RectTransform rectTransform = runStatusText.GetComponent<RectTransform>();
        string text = runStatusText.text;
        return rectTransform != null
            && rectTransform.sizeDelta.x >= 150f
            && text.Length <= 24
            && text.Contains("Run");
    }

    private static bool IsStageObjectiveTextLikelyConfigured(TMP_Text objectiveText)
    {
        if (objectiveText == null)
        {
            return false;
        }

        RectTransform rectTransform = objectiveText.GetComponent<RectTransform>();
        string text = objectiveText.text;
        return rectTransform != null
            && rectTransform.sizeDelta.x >= 140f
            && text.Length <= 18
            && text.Contains("Grid")
            && text.Contains("intent");
    }

    private static bool IsStageProgressTextLikelyConfigured(TMP_Text progressText)
    {
        if (progressText == null)
        {
            return false;
        }

        RectTransform rectTransform = progressText.GetComponent<RectTransform>();
        string text = progressText.text;
        return rectTransform != null
            && rectTransform.sizeDelta.x >= 110f
            && text.Length <= 16
            && text.Contains("Cost")
            && text.Contains("Chain");
    }

    private static bool IsBattleLogTextLikelyConfigured(TMP_Text logText)
    {
        if (logText == null)
        {
            return false;
        }

        RectTransform rectTransform = logText.GetComponent<RectTransform>();
        return rectTransform != null
            && rectTransform.sizeDelta.x >= 470f
            && rectTransform.sizeDelta.y >= 90f
            && logText.text.Contains("Recent Actions")
            && logText.text.Contains("No actions yet.");
    }

    private static bool IsTextRaycastOptimized(params TMP_Text[] texts)
    {
        if (texts == null || texts.Length == 0)
        {
            return false;
        }

        foreach (TMP_Text text in texts)
        {
            if (text == null || text.raycastTarget)
            {
                return false;
            }
        }

        return true;
    }

    private static bool HasObjectReference(SerializedObject serializedObject, string propertyName)
    {
        if (serializedObject == null)
        {
            return false;
        }

        SerializedProperty property = serializedObject.FindProperty(propertyName);
        return property != null && property.objectReferenceValue != null;
    }

    private static void AppendCheck(ref bool passed, ref string report, string label, bool condition)
    {
        report += condition ? "[OK] " : "[FAIL] ";
        report += label + "\n";

        if (!condition)
        {
            passed = false;
        }
    }

    private static Camera CreateCamera()
    {
        GameObject cameraObject = new GameObject("Main Camera");
        Camera camera = cameraObject.AddComponent<Camera>();
        camera.tag = "MainCamera";
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0.08f, 0.09f, 0.12f);
        camera.orthographic = true;
        camera.orthographicSize = 5.0f;
        camera.transform.position = new Vector3(0, 0, -10);
        return camera;
    }

    private static Canvas CreateCanvas(Camera camera)
    {
        GameObject canvasObject = new GameObject("Canvas");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = camera;
        canvas.planeDistance = 1;

        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280, 720);
        scaler.matchWidthOrHeight = 0.5f;

        canvasObject.AddComponent<GraphicRaycaster>();
        return canvas;
    }

    private static void CreateEventSystem()
    {
        GameObject eventSystemObject = new GameObject("EventSystem");
        eventSystemObject.AddComponent<EventSystem>();
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        eventSystemObject.AddComponent<InputSystemUIInputModule>();
#else
        eventSystemObject.AddComponent<StandaloneInputModule>();
#endif
    }

    private static TMP_Text CreateText(Transform parent, string name, string text, Vector2 anchoredPosition, Vector2 size, TextAlignmentOptions alignment)
    {
        GameObject textObject = new GameObject(name);
        textObject.transform.SetParent(parent, false);

        RectTransform rectTransform = textObject.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = size;

        TextMeshProUGUI label = textObject.AddComponent<TextMeshProUGUI>();
        label.text = text;
        label.fontSize = 28;
        label.color = Color.white;
        label.alignment = alignment;
        label.enableWordWrapping = true;
        // Static and runtime-updated labels should not participate in pointer hit tests.
        // This keeps the GraphicRaycaster focused on actual Buttons and reduces UI event overhead.
        label.raycastTarget = false;
        return label;
    }

    private static Image CreatePanel(Transform parent, string name, Vector2 anchoredPosition, Vector2 size, Color color)
    {
        GameObject panelObject = new GameObject(name);
        panelObject.transform.SetParent(parent, false);

        RectTransform rectTransform = panelObject.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = size;

        Image image = panelObject.AddComponent<Image>();
        image.color = color;
        image.raycastTarget = false;
        return image;
    }

    private static Image CreateSpritePanel(Transform parent, string name, string assetPath, Vector2 anchoredPosition, Vector2 size)
    {
        GameObject panelObject = new GameObject(name);
        panelObject.transform.SetParent(parent, false);

        RectTransform rectTransform = panelObject.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = size;

        Image image = panelObject.AddComponent<Image>();
        image.sprite = LoadPixelSprite(assetPath);
        image.color = Color.white;
        image.preserveAspect = true;
        image.raycastTarget = false;
        return image;
    }

    private static Sprite LoadPixelSprite(string assetPath)
    {
        TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        if (importer != null)
        {
            bool needsReimport = importer.textureType != TextureImporterType.Sprite
                || importer.spriteImportMode != SpriteImportMode.Single
                || importer.filterMode != FilterMode.Point
                || importer.textureCompression != TextureImporterCompression.Uncompressed
                || importer.mipmapEnabled
                || importer.wrapMode != TextureWrapMode.Clamp;

            if (needsReimport)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.filterMode = FilterMode.Point;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.mipmapEnabled = false;
                importer.wrapMode = TextureWrapMode.Clamp;
                importer.alphaIsTransparency = true;
                importer.SaveAndReimport();
            }
        }

        AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
        return AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
    }

    private static Image CreatePortrait(Transform parent, string name, Vector2 anchoredPosition, Vector2 size, string assetPath = null)
    {
        GameObject portraitObject = new GameObject(name);
        portraitObject.transform.SetParent(parent, false);

        RectTransform rectTransform = portraitObject.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = size;

        Image image = portraitObject.AddComponent<Image>();
        image.sprite = string.IsNullOrEmpty(assetPath) ? null : LoadPixelSprite(assetPath);
        image.color = image.sprite == null ? new Color(0.10f, 0.11f, 0.16f, 0.7f) : Color.white;
        image.preserveAspect = image.sprite != null;
        image.raycastTarget = false;
        return image;
    }

    private static Image CreateStatusOverlay(Transform parent, string name, Vector2 anchoredPosition, Vector2 size)
    {
        GameObject overlayObj = new GameObject(name);
        overlayObj.transform.SetParent(parent, false);

        RectTransform rectTransform = overlayObj.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = size;

        Image image = overlayObj.AddComponent<Image>();
        image.color = Color.clear;
        image.raycastTarget = false;
        return image;
    }

    private static Image CreatePortraitFrame(Transform parent, string name, Vector2 anchoredPosition, Vector2 size)
    {
        GameObject frameObject = new GameObject(name);
        frameObject.transform.SetParent(parent, false);

        RectTransform rectTransform = frameObject.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = size;

        Image image = frameObject.AddComponent<Image>();
        image.color = new Color(0.10f, 0.12f, 0.20f, 0.85f);
        image.raycastTarget = false;
        return image;
    }

    private static void CreatePortraitPixelAccent(Transform parent, string prefix, Vector2 center, Color color)
    {
        Vector2[] offsets =
        {
            new Vector2(-68, 50),
            new Vector2(-52, 66),
            new Vector2(-68, -50),
            new Vector2(68, 50),
            new Vector2(52, -66),
            new Vector2(68, -50)
        };

        for (int i = 0; i < offsets.Length; i++)
        {
            Image accent = CreatePanel(
                parent,
                $"{prefix} Portrait Pixel Accent {i + 1}",
                center + offsets[i],
                i % 2 == 0 ? new Vector2(12, 12) : new Vector2(8, 8),
                color);
            accent.raycastTarget = false;
        }
    }


    private static void CreateTacticalGrid(Transform parent)
    {
        // A grounded isometric floor: muted tiles with visible landing cells.
        // The old bright debug blocks made the characters look like they were floating beside the board.
        Color tileColor = new Color(0.105f, 0.225f, 0.225f, 0.70f);
        Color tileAltColor = new Color(0.080f, 0.180f, 0.195f, 0.74f);
        Color rimColor = new Color(0.58f, 0.78f, 0.62f, 0.24f);
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 6; col++)
            {
                Vector2 pos = new Vector2(-238 + col * 82 + row * 35, -118 + row * 42);
                Color color = (row + col) % 2 == 0 ? tileColor : tileAltColor;
                Image tile = CreatePanel(parent, $"Tactical Grid Tile {row + 1}-{col + 1}", pos, new Vector2(78, 38), color);
                Image rim = CreatePanel(parent, $"Tactical Grid Tile Rim {row + 1}-{col + 1}", pos + new Vector2(0, 18), new Vector2(72, 3), rimColor);
                tile.rectTransform.localRotation = Quaternion.Euler(0, 0, -12f);
                rim.rectTransform.localRotation = Quaternion.Euler(0, 0, -12f);
                tile.raycastTarget = false;
                rim.raycastTarget = false;
            }
        }

        Image allyLandingTile = CreatePanel(parent, "Ally Formation Marker", new Vector2(-206, -105), new Vector2(132, 36), new Color(0.38f, 0.90f, 0.76f, 0.30f));
        Image enemyLandingTile = CreatePanel(parent, "Enemy Formation Marker", new Vector2(230, -99), new Vector2(148, 38), new Color(0.94f, 0.36f, 0.82f, 0.30f));
        Image actionArc = CreatePanel(parent, "Skill Action Arc", new Vector2(-6, 104), new Vector2(360, 2), new Color(1.0f, 0.80f, 0.48f, 0.10f));
        allyLandingTile.rectTransform.localRotation = Quaternion.Euler(0, 0, -12f);
        enemyLandingTile.rectTransform.localRotation = Quaternion.Euler(0, 0, -12f);
        allyLandingTile.raycastTarget = false;
        enemyLandingTile.raycastTarget = false;
        actionArc.raycastTarget = false;
        actionArc.rectTransform.localRotation = Quaternion.Euler(0, 0, -12f);
    }

    private static void CreateCommercialBattlefieldComposition(Transform parent)
    {
        Image topLetterbox = CreatePanel(parent, "Battle Letterbox Top Panel", new Vector2(0, 292), new Vector2(1220, 34), new Color(0.0f, 0.0f, 0.0f, 0.36f));
        Image bottomLetterbox = CreatePanel(parent, "Battle Letterbox Bottom Panel", new Vector2(0, -252), new Vector2(1220, 34), new Color(0.0f, 0.0f, 0.0f, 0.38f));
        Image innerFrame = CreatePanel(parent, "Battlefield Inner Gold Frame Panel", new Vector2(0, -22), new Vector2(840, 3), new Color(1.0f, 0.78f, 0.38f, 0.28f));
        Image bloom = CreatePanel(parent, "Field Depth Bloom Panel", new Vector2(8, -54), new Vector2(430, 118), new Color(0.36f, 0.62f, 0.86f, 0.12f));
        Image heroTile = CreatePanel(parent, "Hero Premium Landing Tile Panel", new Vector2(-206, -104), new Vector2(132, 28), new Color(0.30f, 0.74f, 1.0f, 0.14f));
        Image enemyTile = CreatePanel(parent, "Enemy Premium Landing Tile Panel", new Vector2(230, -102), new Vector2(142, 30), new Color(1.0f, 0.36f, 0.72f, 0.14f));
        Image centerRule = CreatePanel(parent, "Center Field Composition Rule Panel", new Vector2(12, -102), new Vector2(238, 2), new Color(0.92f, 0.72f, 0.36f, 0.18f));

        topLetterbox.raycastTarget = false;
        bottomLetterbox.raycastTarget = false;
        innerFrame.raycastTarget = false;
        bloom.raycastTarget = false;
        heroTile.raycastTarget = false;
        enemyTile.raycastTarget = false;
        centerRule.raycastTarget = false;
        bloom.rectTransform.localRotation = Quaternion.Euler(0, 0, -4f);
        heroTile.rectTransform.localRotation = Quaternion.Euler(0, 0, -4f);
        enemyTile.rectTransform.localRotation = Quaternion.Euler(0, 0, 4f);
    }

    private static void CreateBattlefieldWeatherAndParallax(Transform parent)
    {
        for (int i = 0; i < 7; i++)
        {
            float x = -330f + i * 110f;
            float y = 178f - (i % 3) * 84f;
            Image streak = CreatePanel(parent, $"Battle Rain Streak {i + 1}", new Vector2(x, y), new Vector2(3, 72), new Color(0.58f, 0.74f, 1.0f, 0.10f + i * 0.006f));
            streak.rectTransform.localRotation = Quaternion.Euler(0, 0, -14f);
            streak.raycastTarget = false;
        }

        Color[] leafColors =
        {
            new Color(0.42f, 0.78f, 0.38f, 0.20f),
            new Color(0.72f, 0.54f, 0.24f, 0.22f),
            new Color(0.28f, 0.62f, 0.42f, 0.24f),
            new Color(0.86f, 0.70f, 0.34f, 0.18f)
        };
        for (int i = 0; i < leafColors.Length; i++)
        {
            Image leaf = CreatePanel(parent, $"Battle Parallax Leaf {i + 1}", new Vector2(-270 + i * 180, 126 - i * 34), new Vector2(24, 10), leafColors[i]);
            leaf.rectTransform.localRotation = Quaternion.Euler(0, 0, 18f + i * 17f);
            leaf.raycastTarget = false;
        }
    }

    private static void CreateFieldVignette(Transform parent)
    {
        CreatePanel(parent, "Forest Shadow Left", new Vector2(-382, -6), new Vector2(86, 510), new Color(0.005f, 0.012f, 0.016f, 0.44f));
        CreatePanel(parent, "Forest Shadow Right", new Vector2(410, -12), new Vector2(96, 500), new Color(0.005f, 0.012f, 0.016f, 0.46f));
        CreatePanel(parent, "Stage Glow Firefly 1", new Vector2(-210, 150), new Vector2(8, 8), new Color(0.72f, 1.0f, 0.42f, 0.70f));
        CreatePanel(parent, "Stage Glow Firefly 2", new Vector2(188, 118), new Vector2(7, 7), new Color(0.72f, 1.0f, 0.42f, 0.65f));
        CreatePanel(parent, "Stage Glow Firefly 3", new Vector2(-18, -184), new Vector2(6, 6), new Color(0.72f, 1.0f, 0.42f, 0.55f));
    }

    private static void CreateBattlefieldDepthLayers(Transform parent)
    {
        Image distantForest = CreatePanel(parent, "Distant Forest Silhouette Panel", new Vector2(0, 104), new Vector2(1120, 100), new Color(0.006f, 0.028f, 0.026f, 0.52f));
        Image moonlight = CreatePanel(parent, "Moonlight Beam Panel", new Vector2(82, 34), new Vector2(42, 360), new Color(0.42f, 0.56f, 0.78f, 0.08f));
        Image fog = CreatePanel(parent, "Foreground Fog Panel", new Vector2(0, -202), new Vector2(980, 22), new Color(0.32f, 0.46f, 0.40f, 0.20f));
        Image rearHorizon = CreatePanel(parent, "Rear Horizon Gold Line Panel", new Vector2(0, 58), new Vector2(180, 2), new Color(0.60f, 0.64f, 0.58f, 0.12f));
        distantForest.raycastTarget = false;
        moonlight.raycastTarget = false;
        fog.raycastTarget = false;
        rearHorizon.raycastTarget = false;
        moonlight.rectTransform.localRotation = Quaternion.Euler(0, 0, -11f);
    }

    private static void CreateForestRuinsTerrainProps(Transform parent)
    {
        // Low-contrast environmental props break the flat test-board silhouette without obscuring tactical units.
        Color stone = new Color(0.055f, 0.090f, 0.096f, 0.70f);
        Color stoneLight = new Color(0.14f, 0.22f, 0.20f, 0.42f);
        Color moss = new Color(0.16f, 0.36f, 0.26f, 0.34f);

        Image groundRidge = CreatePanel(parent, "Forest Ruins Ground Ridge", new Vector2(0, -150), new Vector2(760, 30), new Color(0.020f, 0.070f, 0.064f, 0.62f));
        Image leftPillar = CreatePanel(parent, "Forest Ruins Left Pillar", new Vector2(-336, -32), new Vector2(38, 194), stone);
        Image leftCap = CreatePanel(parent, "Forest Ruins Left Pillar Cap", new Vector2(-336, 70), new Vector2(66, 20), stoneLight);
        Image leftMoss = CreatePanel(parent, "Forest Ruins Left Moss", new Vector2(-351, 14), new Vector2(7, 112), moss);
        Image rightObelisk = CreatePanel(parent, "Forest Ruins Right Obelisk", new Vector2(346, -20), new Vector2(42, 168), stone);
        Image rightCap = CreatePanel(parent, "Forest Ruins Right Obelisk Cap", new Vector2(346, 70), new Vector2(70, 18), stoneLight);
        Image rightMoss = CreatePanel(parent, "Forest Ruins Right Moss", new Vector2(330, 6), new Vector2(8, 98), moss);
        Image fallenSlab = CreatePanel(parent, "Forest Ruins Fallen Slab", new Vector2(70, -142), new Vector2(124, 18), stoneLight);
        fallenSlab.rectTransform.localRotation = Quaternion.Euler(0, 0, -7f);

        Image[] props = { groundRidge, leftPillar, leftCap, leftMoss, rightObelisk, rightCap, rightMoss, fallenSlab };
        foreach (Image prop in props)
        {
            prop.raycastTarget = false;
        }
    }

    private static void CreateCinematicBattlefieldLighting(Transform parent)
    {
        Image leftCurtain = CreatePanel(parent, "Cinematic Left Shadow Curtain", new Vector2(-390, 18), new Vector2(180, 520), new Color(0.0f, 0.0f, 0.0f, 0.20f));
        Image rightCurtain = CreatePanel(parent, "Cinematic Right Shadow Curtain", new Vector2(390, 18), new Vector2(180, 520), new Color(0.0f, 0.0f, 0.0f, 0.20f));
        Image heroSpotlight = CreatePanel(parent, "Hero Cinematic Spotlight", new Vector2(-205, -42), new Vector2(170, 190), new Color(0.24f, 0.58f, 1.0f, 0.11f));
        Image enemySpotlight = CreatePanel(parent, "Enemy Cinematic Spotlight", new Vector2(232, -42), new Vector2(180, 198), new Color(1.0f, 0.32f, 0.58f, 0.10f));
        Image centerClashGlow = CreatePanel(parent, "Center Clash Glow Panel", new Vector2(12, -88), new Vector2(260, 64), new Color(1.0f, 0.72f, 0.32f, 0.10f));
        Image floorSpecular = CreatePanel(parent, "Floor Specular Highlight Panel", new Vector2(0, -114), new Vector2(500, 18), new Color(0.92f, 0.78f, 0.48f, 0.12f));

        leftCurtain.raycastTarget = false;
        rightCurtain.raycastTarget = false;
        heroSpotlight.raycastTarget = false;
        enemySpotlight.raycastTarget = false;
        centerClashGlow.raycastTarget = false;
        floorSpecular.raycastTarget = false;
        heroSpotlight.rectTransform.localRotation = Quaternion.Euler(0, 0, -8f);
        enemySpotlight.rectTransform.localRotation = Quaternion.Euler(0, 0, 8f);
    }

    private static void CreateBattlefieldForegroundFraming(Transform parent)
    {
        Image leftTree = CreatePanel(parent, "Foreground Tree Pillar Left Panel", new Vector2(-455, -18), new Vector2(70, 500), new Color(0.0f, 0.010f, 0.012f, 0.28f));
        Image rightTree = CreatePanel(parent, "Foreground Tree Pillar Right Panel", new Vector2(462, -20), new Vector2(74, 500), new Color(0.0f, 0.010f, 0.012f, 0.30f));
        Image lowerFog = CreatePanel(parent, "Lower Battle Fog Band Panel", new Vector2(0, -198), new Vector2(820, 42), new Color(0.36f, 0.50f, 0.46f, 0.18f));
        Image canopy = CreatePanel(parent, "Upper Canopy Shadow Panel", new Vector2(0, 176), new Vector2(840, 54), new Color(0.0f, 0.014f, 0.012f, 0.18f));
        leftTree.raycastTarget = false;
        rightTree.raycastTarget = false;
        lowerFog.raycastTarget = false;
        canopy.raycastTarget = false;
        leftTree.rectTransform.localRotation = Quaternion.Euler(0, 0, -2f);
        rightTree.rectTransform.localRotation = Quaternion.Euler(0, 0, 2f);
    }

    private static void CreateBattlefieldUnitStandees(Transform parent)
    {
        // Exactly three live, clickable CharacterData slots on each diagonal. No stand-in or duplicate formation bodies.
        // Slot index follows stable visual IDs: front is nearest the center, rear is farthest.
        Vector2[] allyPositions = { new Vector2(-80, 12), new Vector2(-205, -52), new Vector2(-340, -112) };
        Vector2[] enemyPositions = { new Vector2(82, 32), new Vector2(220, 94), new Vector2(350, 150) };
        string[] allySprites = { "Assets/Art/BattleUnits/ally_paladin.png", "Assets/Art/BattleUnits/ally_cleric.png", "Assets/Art/BattleUnits/ally_ranger.png" };
        // Physical enemy slots are front Orc, middle Skeleton, rear Goblin; CharacterData resolves by visual id.
        string[] enemySprites = { "Assets/Art/BattleUnits/enemy_orc.png", "Assets/Art/BattleUnits/enemy_skeleton.png", "Assets/Art/BattleUnits/enemy_goblin.png" };
        const float perspectiveBaseHeight = 160f;
        float[] allyHeights = { perspectiveBaseHeight * 1.05f, perspectiveBaseHeight * 0.95f, perspectiveBaseHeight * 0.85f };
        float[] enemyHeights = { perspectiveBaseHeight * 1.05f, perspectiveBaseHeight * 0.95f, perspectiveBaseHeight * 0.85f };
        for (int i = 0; i < 3; i++)
        {
            CreateBattlefieldSlot(parent, true, i, allyPositions[i], allySprites[i], allyHeights[i]);
            CreateBattlefieldSlot(parent, false, i, enemyPositions[i], enemySprites[i], enemyHeights[i]);
        }
        // Move all six functional indicators above every body after the full formation exists.
        for (int i = 1; i <= 3; i++)
        {
            FindImageIncludingInactive("Ally Slot " + i + " Indicator")?.transform.SetAsLastSibling();
            FindImageIncludingInactive("Enemy Slot " + i + " Indicator")?.transform.SetAsLastSibling();
        }
    }

    private static void CreateBattlefieldSlot(Transform parent, bool isAlly, int index, Vector2 position, string spritePath, float bodyHeight)
    {
        string side = isAlly ? "Ally" : "Enemy";
        Color indicatorColor = isAlly ? new Color(0.34f, 0.86f, 1f, 0.90f) : new Color(1f, 0.42f, 0.30f, 0.90f);
        // Source sprites already include their ground shadows. The only added base treatment is a selected-state indicator.
        float bodyBottom = position.y - bodyHeight * 0.5f;
        Sprite sprite = LoadPixelSprite(spritePath);
        float width = sprite != null && sprite.rect.height > 0f ? bodyHeight * sprite.rect.width / sprite.rect.height : bodyHeight;
        Image indicator = CreateEllipseShadow(parent, side + " Slot " + (index + 1) + " Indicator", new Vector2(position.x, bodyBottom + 5f), new Vector2(Mathf.Clamp(width * 0.72f, 68f, 116f), 10f), indicatorColor);
        Image body = CreateSpritePanel(parent, side + " Slot " + (index + 1) + " Body", spritePath, position, new Vector2(width, bodyHeight));
        body.preserveAspect = true;
        Button button = body.gameObject.AddComponent<Button>();
        button.targetGraphic = body;
        Image overlay = CreatePanel(parent, side + " Slot " + (index + 1) + " Status Overlay", position, new Vector2(width, bodyHeight), new Color(0.35f, 0.65f, 1f, 0.40f));
        float hpWidth = width * 0.84f;
        Slider hp = CreateHpSlider(parent, side + " Slot " + (index + 1) + " HP Slider", new Vector2(position.x, bodyBottom - 12f), new Vector2(hpWidth, 6), isAlly ? new Color(0.25f, 0.82f, 0.40f) : new Color(0.52f, 0.10f, 0.12f));
        TMP_Text hpText = CreateText(parent, side + " Slot " + (index + 1) + " HP Text", "", new Vector2(position.x, bodyBottom - 25f), new Vector2(Mathf.Max(150f, width + 24f), 14), TextAlignmentOptions.Center);
        hpText.fontSize = 7;
        indicator.raycastTarget = false;
        overlay.raycastTarget = false;
        hpText.raycastTarget = false;
        // The ring is spatially separated from HP and rendered last so the sprite cannot hide it.
        indicator.transform.SetAsLastSibling();
        indicator.gameObject.SetActive(false);
        overlay.gameObject.SetActive(false);
    }

    private static Image CreateEllipseShadow(Transform parent, string name, Vector2 position, Vector2 size, Color color)
    {
        Image shadow = CreatePanel(parent, name, position, size, color);
        shadow.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
        shadow.type = Image.Type.Simple;
        shadow.raycastTarget = false;
        return shadow;
    }

    private static Image CreatePixelBlock(Transform parent, string name, Vector2 anchoredPosition, Vector2 size, Color color)
    {
        Image block = CreatePanel(parent, name, anchoredPosition, size, color);
        block.raycastTarget = false;
        return block;
    }

    private static void ConfigureBattleSpriteMotion(Image image, float bobPixels, float bobSpeed, float phaseOffset, float hitPixels, float squashAmount, bool moveLeftOnHit)
    {
        if (image == null) return;
        BattleSpriteMotion motion = image.GetComponent<BattleSpriteMotion>();
        if (motion == null)
            motion = image.gameObject.AddComponent<BattleSpriteMotion>();
        motion.Configure(bobPixels, bobSpeed, phaseOffset, hitPixels, squashAmount, moveLeftOnHit);
    }

    private static void CreateCommercialCombatReadouts(Transform parent)
    {
        Image bossPhase = CreatePanel(parent, "Boss Phase Telegraph Panel", new Vector2(0, 182), new Vector2(272, 46), new Color(0.075f, 0.024f, 0.035f, 0.52f));
        Image bossEdge = CreatePanel(parent, "Boss Phase Telegraph Gold Edge", new Vector2(0, 204), new Vector2(232, 2), new Color(1.0f, 0.72f, 0.36f, 0.40f));
        TMP_Text bossText = CreateText(parent, "Boss Phase Telegraph Text", "BOSS BREAK WINDOW / 2 TURNS", new Vector2(0, 182), new Vector2(244, 24), TextAlignmentOptions.Center);
        bossText.fontSize = 11;
        bossText.fontStyle = FontStyles.Bold;
        bossText.color = new Color(1.0f, 0.84f, 0.58f, 0.88f);

        Image comboPanel = CreatePanel(parent, "Combo Meter Panel", new Vector2(0, 136), new Vector2(232, 30), new Color(0.018f, 0.024f, 0.040f, 0.46f));
        Image comboFill = CreatePanel(parent, "Combo Meter Fill Panel", new Vector2(-36, 136), new Vector2(142, 8), new Color(0.96f, 0.50f, 0.28f, 0.62f));
        TMP_Text comboText = CreateText(parent, "Combo Meter Text", "CHAIN 3 / AP BURST", new Vector2(0, 146), new Vector2(210, 15), TextAlignmentOptions.Center);
        comboText.fontSize = 8;
        comboText.color = new Color(0.96f, 0.92f, 0.80f, 0.84f);

        Image damagePreview = CreatePanel(parent, "Damage Preview Panel", new Vector2(0, 96), new Vector2(204, 26), new Color(0.050f, 0.026f, 0.014f, 0.42f));
        TMP_Text damagePreviewText = CreateText(parent, "Damage Preview Text", "DMG 42-57 / CRIT 18%", new Vector2(0, 96), new Vector2(184, 18), TextAlignmentOptions.Center);
        damagePreviewText.fontSize = 9;
        damagePreviewText.color = new Color(1.0f, 0.78f, 0.46f, 0.88f);

        Image timelineRail = CreatePanel(parent, "Turn Timeline Rail Panel", new Vector2(314, 252), new Vector2(270, 22), new Color(0.010f, 0.018f, 0.030f, 0.40f));
        TMP_Text timelineLabel = CreateText(parent, "Turn Timeline Label Text", "TURN ORDER", new Vector2(200, 252), new Vector2(82, 16), TextAlignmentOptions.Left);
        timelineLabel.fontSize = 8;
        timelineLabel.color = new Color(0.76f, 0.88f, 1.0f, 0.74f);
        Color[] nodeColors =
        {
            new Color(0.44f, 0.82f, 1.0f, 0.72f),
            new Color(1.0f, 0.58f, 0.36f, 0.66f),
            new Color(0.52f, 0.92f, 0.56f, 0.62f),
            new Color(0.94f, 0.74f, 0.34f, 0.62f)
        };
        for (int i = 0; i < nodeColors.Length; i++)
        {
            Image node = CreatePanel(parent, $"Turn Timeline Node {i + 1}", new Vector2(268 + i * 42, 252), new Vector2(20, 20), nodeColors[i]);
            node.raycastTarget = false;
        }

        bossPhase.raycastTarget = false;
        bossEdge.raycastTarget = false;
        comboPanel.raycastTarget = false;
        comboFill.raycastTarget = false;
        damagePreview.raycastTarget = false;
        timelineRail.raycastTarget = false;
        bossText.raycastTarget = false;
        comboText.raycastTarget = false;
        damagePreviewText.raycastTarget = false;
        timelineLabel.raycastTarget = false;
    }

    private static void CreatePremiumCommandFrame(Transform parent)
    {
        Image headerPanel = CreatePanel(parent, "Command Header Panel", new Vector2(362, -230), new Vector2(292, 28), new Color(0.09f, 0.075f, 0.045f, 0.92f));
        TMP_Text headerText = CreateText(parent, "Command Header Text", "COMMAND CHAIN", new Vector2(362, -230), new Vector2(270, 24), TextAlignmentOptions.Center);
        headerText.fontSize = 13;
        headerText.fontStyle = FontStyles.Bold;
        headerText.color = new Color(1.0f, 0.84f, 0.42f);

        Image skillTierBadge = CreatePanel(parent, "Skill Tier Badge", new Vector2(558, -230), new Vector2(64, 24), new Color(0.30f, 0.20f, 0.10f, 0.92f));
        TMP_Text skillTierText = CreateText(parent, "Skill Tier Text", "AP3", new Vector2(558, -230), new Vector2(58, 20), TextAlignmentOptions.Center);
        skillTierText.fontSize = 12;
        skillTierText.fontStyle = FontStyles.Bold;
        skillTierText.color = new Color(0.86f, 0.94f, 1.0f);

        Image commandGlowLeft = CreatePanel(parent, "Command Glow Left", new Vector2(66, -300), new Vector2(4, 100), new Color(0.95f, 0.72f, 0.34f, 0.68f));
        Image commandGlowRight = CreatePanel(parent, "Command Glow Right", new Vector2(634, -300), new Vector2(4, 100), new Color(0.95f, 0.72f, 0.34f, 0.68f));

        headerPanel.raycastTarget = false;
        skillTierBadge.raycastTarget = false;
        commandGlowLeft.raycastTarget = false;
        commandGlowRight.raycastTarget = false;
        headerPanel.gameObject.SetActive(false);
        headerText.gameObject.SetActive(false);
        skillTierBadge.gameObject.SetActive(false);
        skillTierText.gameObject.SetActive(false);
        commandGlowLeft.gameObject.SetActive(false);
        commandGlowRight.gameObject.SetActive(false);
    }

    private static void CreateBottomResourceStrip(Transform parent)
    {
        Image strip = CreatePanel(parent, "Bottom Resource Strip Panel", new Vector2(-245, -344), new Vector2(372, 52), new Color(0.006f, 0.008f, 0.014f, 0.24f));
        Image topEdge = CreatePanel(parent, "Bottom Resource Strip Top Edge", new Vector2(-245, -318), new Vector2(348, 2), new Color(1.0f, 0.78f, 0.38f, 0.18f));
        Image bottomDepth = CreatePanel(parent, "Bottom Resource Strip Bottom Depth", new Vector2(-245, -370), new Vector2(348, 3), new Color(0.0f, 0.0f, 0.0f, 0.24f));
        Image resourceSeparator = CreatePanel(parent, "Bottom Resource Strip Separator", new Vector2(-245, -344), new Vector2(2, 36), new Color(0.92f, 0.82f, 0.54f, 0.12f));

        strip.raycastTarget = false;
        topEdge.raycastTarget = false;
        bottomDepth.raycastTarget = false;
        resourceSeparator.raycastTarget = false;
    }

    private static void CreateProfessionalPanelMaterialOverlays(Transform parent)
    {
        Image topGloss = CreatePanel(parent, "Top Status Panel Top Gloss", new Vector2(0, 354), new Vector2(1180, 3), new Color(0.72f, 0.88f, 1.0f, 0.22f));
        Image topBottomShade = CreatePanel(parent, "Top Status Panel Bottom Shade", new Vector2(0, 304), new Vector2(1180, 5), new Color(0.0f, 0.0f, 0.0f, 0.28f));
        Image playerLeftRim = CreatePanel(parent, "Player Card Panel Left Rim", new Vector2(-640, 20), new Vector2(3, 350), new Color(0.95f, 0.78f, 0.42f, 0.22f));
        Image playerInnerShade = CreatePanel(parent, "Player Card Panel Inner Shade", new Vector2(-440, 20), new Vector2(3, 340), new Color(0.0f, 0.0f, 0.0f, 0.20f));
        Image enemyRightRim = CreatePanel(parent, "Enemy Card Panel Right Rim", new Vector2(632, 112), new Vector2(3, 210), new Color(1.0f, 0.45f, 0.28f, 0.20f));
        Image enemyInnerShade = CreatePanel(parent, "Enemy Card Panel Inner Shade", new Vector2(448, 112), new Vector2(3, 200), new Color(0.0f, 0.0f, 0.0f, 0.18f));
        Image commandTopShade = CreatePanel(parent, "Command Bar Panel Top Shade", new Vector2(0, -227), new Vector2(1160, 4), new Color(1.0f, 0.78f, 0.42f, 0.20f));
        Image commandBottomDepth = CreatePanel(parent, "Command Bar Panel Bottom Depth", new Vector2(0, -353), new Vector2(1160, 5), new Color(0.0f, 0.0f, 0.0f, 0.24f));

        topGloss.raycastTarget = false;
        topBottomShade.raycastTarget = false;
        playerLeftRim.raycastTarget = false;
        playerInnerShade.raycastTarget = false;
        enemyRightRim.raycastTarget = false;
        enemyInnerShade.raycastTarget = false;
        commandTopShade.raycastTarget = false;
        commandBottomDepth.raycastTarget = false;
    }

    private static void CreateProfessionalTopHudAccents(Transform parent)
    {
        // Decorative accents only. Do not duplicate runtime AUTO / speed / pause controls,
        // because those already exist as real buttons in the top-right HUD.
        Image coin = CreatePanel(parent, "Professional Top Coin Accent", new Vector2(-610, 326), new Vector2(26, 26), new Color(0.92f, 0.78f, 0.38f, 0.72f));
        Image buff1 = CreatePanel(parent, "Professional Top Buff Accent 1", new Vector2(-584, 288), new Vector2(24, 24), new Color(0.20f, 0.52f, 0.22f, 0.62f));
        Image buff2 = CreatePanel(parent, "Professional Top Buff Accent 2", new Vector2(-552, 288), new Vector2(24, 24), new Color(0.18f, 0.48f, 0.24f, 0.62f));
        Image rightRail = CreatePanel(parent, "Professional Top Right Control Rail", new Vector2(490, 326), new Vector2(230, 2), new Color(1.0f, 0.82f, 0.46f, 0.42f));
        coin.raycastTarget = false;
        buff1.raycastTarget = false;
        buff2.raycastTarget = false;
        rightRail.raycastTarget = false;
    }

    private static void CreateProgressReferenceSkillCards(Transform parent)
    {
        string[] names = { "REVENGE", "BLESSED SHIELD", "HOLY LIGHT" };
        string[] desc = { "Counter blow / 30%", "Guard ally with shield", "Heal and cleanse" };
        Color[] colors =
        {
            new Color(0.48f, 0.08f, 0.07f, 0.86f),
            new Color(0.58f, 0.42f, 0.12f, 0.86f),
            new Color(0.82f, 0.68f, 0.24f, 0.86f)
        };

        for (int i = 0; i < names.Length; i++)
        {
            float y = 178 - i * 78;
            Image card = CreatePanel(parent, $"Progress Skill Card {i + 1}", new Vector2(566, y), new Vector2(172, 58), new Color(0.010f, 0.010f, 0.016f, 0.36f));
            Image topHighlight = CreatePanel(parent, $"Progress Skill Card Top Highlight {i + 1}", new Vector2(566, y + 27), new Vector2(150, 2), new Color(1.0f, 0.78f, 0.38f, 0.28f));
            Image bottomShade = CreatePanel(parent, $"Progress Skill Card Bottom Shade {i + 1}", new Vector2(566, y - 27), new Vector2(150, 3), new Color(0.0f, 0.0f, 0.0f, 0.30f));
            string iconPath = i == 0 ? "Assets/Art/Generated/skill_revenge_icon.png" : i == 1 ? "Assets/Art/Generated/skill_shield_icon.png" : "Assets/Art/Generated/skill_holy_icon.png";
            Image iconGlow = CreatePanel(parent, $"Progress Skill Icon Glow {i + 1}", new Vector2(500, y), new Vector2(50, 50), new Color(1.0f, 0.82f, 0.36f, 0.10f));
            Image iconFrame = CreatePanel(parent, $"Progress Skill Icon Frame {i + 1}", new Vector2(500, y), new Vector2(50, 50), new Color(colors[i].r, colors[i].g, colors[i].b, 0.36f));
            Image icon = CreateSpritePanel(parent, $"Progress Skill Icon {i + 1}", iconPath, new Vector2(500, y), new Vector2(40, 40));
            Image iconEdge = CreatePanel(parent, $"Progress Skill Icon Gold Edge {i + 1}", new Vector2(500, y + 24), new Vector2(44, 2), new Color(1.0f, 0.78f, 0.38f, 0.46f));
            TMP_Text title = CreateText(parent, $"Progress Skill Title {i + 1}", names[i], new Vector2(579, y + 10), new Vector2(92, 18), TextAlignmentOptions.Left);
            title.fontSize = 10;
            title.fontStyle = FontStyles.Bold;
            title.color = new Color(0.92f, 0.86f, 0.70f);
            TMP_Text body = CreateText(parent, $"Progress Skill Body {i + 1}", desc[i], new Vector2(579, y - 11), new Vector2(92, 24), TextAlignmentOptions.Left);
            body.fontSize = 7;
            body.color = new Color(0.76f, 0.74f, 0.68f);
            card.raycastTarget = false;
            topHighlight.raycastTarget = false;
            bottomShade.raycastTarget = false;
            icon.raycastTarget = false;
            iconGlow.raycastTarget = false;
            iconFrame.raycastTarget = false;
            iconEdge.raycastTarget = false;
        }
    }

    private static void CreateProgressReferenceBottomHud(Transform parent)
    {
        TMP_Text unitName = CreateText(parent, "Progress Bottom Unit Name", "FEH", new Vector2(-320, -304), new Vector2(80, 34), TextAlignmentOptions.Left);
        unitName.fontSize = 18;
        unitName.fontStyle = FontStyles.Bold;
        unitName.color = new Color(0.90f, 0.82f, 0.62f);
        CreateSlimStat(parent, "Progress Bottom HP", "HP", "253/180", new Vector2(-250, -292), new Color(0.36f, 0.86f, 0.42f));
        CreateSlimStat(parent, "Progress Bottom MP", "MP", "130/180", new Vector2(-250, -326), new Color(0.40f, 0.62f, 1.0f));

        Image turnDial = CreatePanel(parent, "Progress Turn Dial", new Vector2(0, -314), new Vector2(86, 86), new Color(0.04f, 0.04f, 0.04f, 0.92f));
        TMP_Text turnText = CreateText(parent, "Progress Turn Dial Text", "3", new Vector2(0, -314), new Vector2(76, 76), TextAlignmentOptions.Center);
        turnText.fontSize = 34;
        turnText.fontStyle = FontStyles.Bold;
        turnText.color = new Color(0.98f, 0.86f, 0.58f);
        turnDial.raycastTarget = false;

        string[] portraitPaths =
        {
            "Assets/Art/Generated/chibi_ally_guardian.png",
            "Assets/Art/Generated/chibi_hero_original.png",
            "Assets/Art/Generated/chibi_ally_guardian.png",
            "Assets/Art/Generated/chibi_hero_original.png"
        };

        for (int i = 0; i < portraitPaths.Length; i++)
        {
            float x = 178 + i * 78;
            CreatePanel(parent, $"Progress Bottom Portrait Frame {i + 1}", new Vector2(x, -312), new Vector2(64, 64), new Color(0.72f, 0.68f, 0.60f, 0.42f));
            Image portrait = CreateSpritePanel(parent, $"Progress Bottom Portrait Sprite {i + 1}", portraitPaths[i], new Vector2(x, -312), new Vector2(58, 58));
            CreatePanel(parent, $"Progress Bottom Portrait Mp {i + 1}", new Vector2(x, -350), new Vector2(60, 4), new Color(0.42f, 0.68f, 1.0f, 0.90f));
            portrait.raycastTarget = false;
        }

        for (int i = 0; i < 18; i++)
        {
            float x = -210 + i * 24;
            Image diamond = CreatePanel(parent, $"Progress Chain Diamond {i + 1}", new Vector2(x, -362), new Vector2(12, 12), i < 12 ? new Color(0.78f, 0.74f, 0.66f, 0.86f) : new Color(0.26f, 0.32f, 0.30f, 0.58f));
            diamond.rectTransform.localRotation = Quaternion.Euler(0, 0, 45f);
        }
    }

    private static void CreateSlimStat(Transform parent, string name, string label, string value, Vector2 position, Color fillColor)
    {
        TMP_Text labelText = CreateText(parent, name + " Label", label, position + new Vector2(0, 8), new Vector2(42, 18), TextAlignmentOptions.Left);
        labelText.fontSize = 10;
        labelText.color = new Color(0.66f, 0.66f, 0.62f);
        TMP_Text valueText = CreateText(parent, name + " Value", value, position + new Vector2(104, 8), new Vector2(92, 18), TextAlignmentOptions.Right);
        valueText.fontSize = 12;
        valueText.color = Color.white;
        CreatePanel(parent, name + " Bar Back", position + new Vector2(58, -8), new Vector2(118, 5), new Color(0.10f, 0.10f, 0.10f, 0.86f));
        CreatePanel(parent, name + " Bar Fill", position + new Vector2(36, -8), new Vector2(74, 5), fillColor);
    }

    private static void CreatePartyRosterSlots(Transform parent)
    {
        string[] names = { "HERO", "GUARDIAN", "SCOUT" };
        int[] hp = { 100, 92, 86 };
        string[] sprites =
        {
            "Assets/Art/Generated/chibi_hero_original.png",
            "Assets/Art/Generated/chibi_ally_guardian.png",
            "Assets/Art/Generated/chibi_hero_original.png"
        };

        for (int i = 0; i < names.Length; i++)
        {
            float y = 92 - i * 62;
            Color slotColor = i == 0 ? new Color(0.12f, 0.10f, 0.055f, 0.46f) : new Color(0.010f, 0.012f, 0.020f, 0.34f);
            Image slot = CreatePanel(parent, $"Party Roster Slot {i + 1}", new Vector2(-540, y), new Vector2(150, 52), slotColor);
            slot.raycastTarget = false;
            CreatePanel(parent, $"Party Roster Portrait Chip {i + 1}", new Vector2(-599, y), new Vector2(40, 40), new Color(0.014f, 0.018f, 0.026f, 0.60f));
            CreatePanel(parent, $"Party Roster Mini Sprite Shadow {i + 1}", new Vector2(-599, y - 2), new Vector2(32, 30), new Color(0.0f, 0.0f, 0.0f, 0.30f));
            CreatePanel(parent, $"Party Roster Mini Sprite Crop Frame {i + 1}", new Vector2(-599, y), new Vector2(38, 38), new Color(0.04f, 0.055f, 0.075f, 0.24f));
            if (i == 0) CreatePanel(parent, "Party Roster Selected Gold Rim", new Vector2(-540, y + 25), new Vector2(144, 2), new Color(1.0f, 0.78f, 0.38f, 0.46f));
            Image miniSprite = CreateSpritePanel(parent, $"Party Roster Mini Sprite {i + 1}", sprites[i], new Vector2(-599, y + 1), new Vector2(34, 40));
            miniSprite.raycastTarget = false;
            CreatePanel(parent, $"Party Roster Mini Sprite Edge Accent {i + 1}", new Vector2(-599, y + 20), new Vector2(36, 2), i == 0 ? new Color(1.0f, 0.78f, 0.38f, 0.42f) : new Color(0.45f, 0.86f, 1.0f, 0.34f));
            TMP_Text label = CreateText(parent, $"Party Roster Label {i + 1}", names[i], new Vector2(-530, y + 8), new Vector2(92, 14), TextAlignmentOptions.Left);
            label.fontSize = 9;
            label.fontStyle = FontStyles.Bold;
            label.color = i == 0 ? new Color(0.96f, 0.88f, 0.64f) : new Color(0.86f, 0.84f, 0.76f);
            TMP_Text stat = CreateText(parent, $"Party Roster Stat {i + 1}", $"HP {hp[i]}  READY", new Vector2(-530, y - 9), new Vector2(92, 12), TextAlignmentOptions.Left);
            stat.fontSize = 7;
            stat.color = new Color(0.82f, 0.82f, 0.76f);

        }
    }

    private static void CreateEnemyRosterSlots(Transform parent)
    {
        for (int i = 0; i < 3; i++)
        {
            float y = -100 - i * 58;
            Color color = i == 2 ? new Color(0.72f, 0.05f, 0.12f, 0.82f) : new Color(0.07f, 0.075f, 0.10f, 0.68f);
            Image slot = CreatePanel(parent, $"Enemy Roster Slot {i + 1}", new Vector2(548, y), new Vector2(160, 50), color);
            slot.raycastTarget = false;
            Image chip = CreatePanel(parent, $"Enemy Roster Portrait Chip {i + 1}", new Vector2(485, y), new Vector2(46, 44), new Color(0.035f, 0.018f, 0.045f, 0.92f));
            string spritePath = i == 0
                ? "Assets/Art/Generated/chibi_enemy_original.png"
                : "Assets/Art/Generated/chibi_enemy_raider.png";
            Image shadow = CreatePanel(parent, $"Enemy Roster Mini Sprite Shadow {i + 1}", new Vector2(488, y - 3), new Vector2(36, 36), new Color(0.0f, 0.0f, 0.0f, 0.44f));
            Image cropFrame = CreatePanel(parent, $"Enemy Roster Mini Sprite Crop Frame {i + 1}", new Vector2(485, y), new Vector2(44, 42), new Color(0.12f, 0.045f, 0.09f, 0.36f));
            Image miniSprite = CreateSpritePanel(parent, $"Enemy Roster Mini Sprite {i + 1}", spritePath, new Vector2(486, y + 2), new Vector2(34, 42));
            miniSprite.raycastTarget = false;
            Image edgeAccent = CreatePanel(parent, $"Enemy Roster Mini Sprite Edge Accent {i + 1}", new Vector2(485, y + 21), new Vector2(42, 3), i == 2 ? new Color(1.0f, 0.40f, 0.56f, 0.76f) : new Color(1.0f, 0.52f, 0.34f, 0.68f));
            string enemyLabel = i == 0 ? "Goblin 80" : i == 1 ? "Skeleton" : "Dark Knight";
            TMP_Text label = CreateText(parent, $"Enemy Roster Label {i + 1}", enemyLabel, new Vector2(562, y), new Vector2(112, 20), TextAlignmentOptions.Right);
            label.fontSize = 12;
            label.color = i == 2 ? new Color(1.0f, 0.78f, 0.78f) : new Color(0.82f, 0.86f, 0.94f);
            slot.gameObject.SetActive(false);
            chip.gameObject.SetActive(false);
            shadow.gameObject.SetActive(false);
            cropFrame.gameObject.SetActive(false);
            miniSprite.gameObject.SetActive(false);
            edgeAccent.gameObject.SetActive(false);
            label.gameObject.SetActive(false);
        }
    }

    private static Image CreateScreenFlashImage(Transform parent, string name)
    {
        GameObject flashObj = new GameObject(name, typeof(RectTransform), typeof(Image));
        flashObj.transform.SetParent(parent, false);
        RectTransform rt = flashObj.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        Image img = flashObj.GetComponent<Image>();
        img.color = Color.clear;
        img.raycastTarget = false;
        return img;
    }

    private static Slider CreateHpSlider(Transform parent, string name, Vector2 anchoredPosition, Vector2 size, Color fillColor)
    {
        GameObject sliderObject = new GameObject(name);
        sliderObject.transform.SetParent(parent, false);

        RectTransform sliderRect = sliderObject.AddComponent<RectTransform>();
        sliderRect.anchorMin = new Vector2(0.5f, 0.5f);
        sliderRect.anchorMax = new Vector2(0.5f, 0.5f);
        sliderRect.pivot = new Vector2(0.5f, 0.5f);
        sliderRect.anchoredPosition = anchoredPosition;
        sliderRect.sizeDelta = size;

        Image backgroundImage = sliderObject.AddComponent<Image>();
        backgroundImage.color = new Color(0.06f, 0.07f, 0.10f);

        GameObject fillObject = new GameObject("Fill");
        fillObject.transform.SetParent(sliderObject.transform, false);
        RectTransform fillRect = fillObject.AddComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = new Vector2(2, 2);
        fillRect.offsetMax = new Vector2(-2, -2);
        Image fillImage = fillObject.AddComponent<Image>();
        fillImage.color = fillColor;

        Slider slider = sliderObject.AddComponent<Slider>();
        slider.minValue = 0f;
        slider.maxValue = 100f;
        slider.value = 100f;
        slider.wholeNumbers = true;
        slider.interactable = false;
        slider.transition = Selectable.Transition.None;
        slider.targetGraphic = backgroundImage;
        slider.fillRect = fillRect;
        slider.direction = Slider.Direction.LeftToRight;
        return slider;
    }

    private static Button CreateCenteredButton(Transform parent, string name, string labelText, Vector2 anchoredPosition, Vector2 size)
    {
        Button button = CreateButton(parent, name, labelText, anchoredPosition, size);
        RectTransform rectTransform = button.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = anchoredPosition;
        return button;
    }

    private static void StyleContextButton(Button button, Color baseColor)
    {
        if (button == null) return;
        Transform topHighlight = button.transform.Find(button.name + " Top Highlight");
        Transform goldEdge = button.transform.Find(button.name + " Gold Edge");
        if (topHighlight != null) Object.DestroyImmediate(topHighlight.gameObject);
        if (goldEdge != null) Object.DestroyImmediate(goldEdge.gameObject);
        Image image = button.GetComponent<Image>();
        image.color = baseColor;
        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(1f, 0.92f, 0.70f, 1f);
        colors.selectedColor = new Color(1f, 0.84f, 0.46f, 1f);
        colors.pressedColor = new Color(0.72f, 0.72f, 0.72f, 1f);
        colors.disabledColor = new Color(0.42f, 0.45f, 0.50f, 0.46f);
        colors.colorMultiplier = 1f;
        button.colors = colors;
        TMP_Text label = button.GetComponentInChildren<TMP_Text>(true);
        if (label != null) { label.fontSize = 16f; label.color = new Color(0.94f, 0.96f, 1f); }
    }

    private static Button CreateButton(Transform parent, string name, string labelText, Vector2 anchoredPosition, Vector2 size)
    {
        GameObject buttonObject = new GameObject(name);
        buttonObject.transform.SetParent(parent, false);

        RectTransform rectTransform = buttonObject.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.0f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.0f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = size;

        Image image = buttonObject.AddComponent<Image>();
        image.color = new Color(0.075f, 0.082f, 0.120f, 0.96f);

        Image topHighlight = CreatePanel(buttonObject.transform, name + " Top Highlight", new Vector2(0, size.y * 0.34f), new Vector2(Mathf.Max(8f, size.x - 16f), 2f), new Color(1.0f, 0.84f, 0.48f, 0.42f));
        Image bottomShade = CreatePanel(buttonObject.transform, name + " Bottom Shade", new Vector2(0, -size.y * 0.34f), new Vector2(Mathf.Max(8f, size.x - 14f), 3f), new Color(0.0f, 0.0f, 0.0f, 0.34f));
        Image goldEdge = CreatePanel(buttonObject.transform, name + " Gold Edge", new Vector2(0, 0), new Vector2(Mathf.Max(8f, size.x - 10f), 2f), new Color(0.92f, 0.66f, 0.28f, 0.36f));
        topHighlight.raycastTarget = false;
        bottomShade.raycastTarget = false;
        goldEdge.raycastTarget = false;

        Button button = buttonObject.AddComponent<Button>();
        ColorBlock colors = button.colors;
        colors.normalColor = new Color(0.075f, 0.082f, 0.120f, 0.96f);
        colors.highlightedColor = new Color(0.20f, 0.16f, 0.09f, 0.98f);
        colors.pressedColor = new Color(0.045f, 0.050f, 0.080f, 1.0f);
        colors.disabledColor = new Color(0.06f, 0.07f, 0.10f, 0.45f);
        button.colors = colors;
        button.targetGraphic = image;

        TMP_Text label = CreateText(buttonObject.transform, "Label", labelText, Vector2.zero, size, TextAlignmentOptions.Center);
        label.fontSize = size.y <= 32f ? 16 : 20;
        label.fontStyle = FontStyles.Bold;
        label.color = new Color(0.96f, 0.90f, 0.72f);
        label.enableWordWrapping = false;
        label.overflowMode = TextOverflowModes.Ellipsis;
        label.raycastTarget = false;

        return button;
    }

    private static void SetObjectArrayReferences(SerializedObject serializedObject, string propertyName, params Object[] values)
    {
        SerializedProperty property = serializedObject.FindProperty(propertyName);
        if (property == null || !property.isArray || values == null)
        {
            return;
        }

        property.arraySize = values.Length;
        for (int i = 0; i < values.Length; i++)
        {
            property.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
        }
    }

    private static void SetObjectReference(SerializedObject serializedObject, string propertyName, Object value)
    {
        SerializedProperty property = serializedObject.FindProperty(propertyName);
        if (property != null)
        {
            property.objectReferenceValue = value;
        }
    }
}
