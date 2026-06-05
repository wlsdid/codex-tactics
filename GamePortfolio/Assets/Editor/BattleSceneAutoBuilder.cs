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

    [MenuItem("Tools/Codex Tactics/Create Battle Test Scene")]
    public static void CreateBattleTestScene()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        scene.name = "BattleScene";

        Camera camera = CreateCamera();
        camera.gameObject.AddComponent<ScreenShake>();
        Canvas canvas = CreateCanvas(camera);
        CreateEventSystem();

        // Reference-inspired tactical RPG layout: left party stack, open center stage, right enemy stack, compact bottom commands.
        Image battleStageBackdropPanel = CreateSpritePanel(canvas.transform, "Battle Stage Backdrop Panel", "Assets/Art/Generated/polished_forest_battle_bg.png", new Vector2(0, 18), new Vector2(1220, 600));
        battleStageBackdropPanel.color = new Color(0.88f, 0.94f, 1.0f, 0.96f);
        Image battleStageFloorPanel = CreatePanel(canvas.transform, "Battle Stage Floor Panel", new Vector2(0, -104), new Vector2(900, 220), new Color(0.055f, 0.145f, 0.135f, 0.42f));
        Image topGoldDividerPanel = CreatePanel(canvas.transform, "Top Gold Divider Panel", new Vector2(0, 276), new Vector2(1220, 3), new Color(1.0f, 0.78f, 0.42f, 0.62f));
        Image commandGoldDividerPanel = CreatePanel(canvas.transform, "Command Gold Divider Panel", new Vector2(80, -258), new Vector2(990, 2), new Color(1.0f, 0.80f, 0.45f, 0.58f));
        battleStageBackdropPanel.raycastTarget = false;
        battleStageFloorPanel.raycastTarget = false;
        topGoldDividerPanel.raycastTarget = false;
        commandGoldDividerPanel.raycastTarget = false;
        CreateBattlefieldDepthLayers(canvas.transform);
        CreateFieldVignette(canvas.transform);
        CreateTacticalGrid(canvas.transform);
        CreateBattlefieldUnitStandees(canvas.transform);

        // Premium dark panels — slim overlay style, leaving the battlefield visible.
        Image topStatusPanel = CreatePanel(canvas.transform, "Top Status Panel", new Vector2(0, 326), new Vector2(1220, 62), new Color(0.010f, 0.014f, 0.024f, 0.72f));
        Image playerCardPanel = CreatePanel(canvas.transform, "Player Card Panel", new Vector2(-508, 54), new Vector2(300, 455), new Color(0.010f, 0.012f, 0.020f, 0.66f));
        Image enemyCardPanel = CreatePanel(canvas.transform, "Enemy Card Panel", new Vector2(526, 56), new Vector2(245, 390), new Color(0.014f, 0.012f, 0.018f, 0.58f));
        Image battleCenterPanel = CreatePanel(canvas.transform, "Battle Center Panel", new Vector2(0, 238), new Vector2(570, 80), new Color(0.010f, 0.014f, 0.024f, 0.40f));
        Image commandBarPanel = CreatePanel(canvas.transform, "Command Bar Panel", new Vector2(0, -318), new Vector2(1220, 96), new Color(0.010f, 0.012f, 0.018f, 0.88f));
        Image partyRosterPanel = CreatePanel(canvas.transform, "Party Roster Panel", new Vector2(-508, 28), new Vector2(296, 386), new Color(0.006f, 0.008f, 0.014f, 0.48f));
        partyRosterPanel.raycastTarget = false;
        CreateProgressReferenceTopControls(canvas.transform);
        CreatePartyRosterSlots(canvas.transform);
        CreateProgressReferenceSkillCards(canvas.transform);
        CreateProgressReferenceBottomHud(canvas.transform);
        Image playerSelectionHighlight = CreatePanel(canvas.transform, "Player Selection Highlight", new Vector2(-508, 160), new Vector2(292, 70), new Color(0.95f, 0.86f, 0.64f, 0.24f));
        playerSelectionHighlight.gameObject.SetActive(false);
        Button playerSelectButton = CreateCenteredButton(canvas.transform, "Player Select Button", "Click Hero", new Vector2(-508, 160), new Vector2(292, 70));
        playerSelectButton.GetComponent<Image>().color = new Color(0.10f, 0.18f, 0.24f, 0.12f);
        TMP_Text playerSelectLabel = playerSelectButton.GetComponentInChildren<TMP_Text>();
        if (playerSelectLabel != null)
        {
            playerSelectLabel.text = "";
            playerSelectLabel.fontSize = 11;
            playerSelectLabel.color = new Color(0.96f, 0.92f, 0.68f, 0.92f);
            playerSelectLabel.alignment = TextAlignmentOptions.BottomRight;
        }
        CreateEnemyRosterSlots(canvas.transform);
        topStatusPanel.raycastTarget = false;
        playerCardPanel.raycastTarget = false;
        enemyCardPanel.raycastTarget = false;
        battleCenterPanel.raycastTarget = false;
        commandBarPanel.raycastTarget = false;

        TMP_Text titleText = CreateText(canvas.transform, "Title Text", "* Codex Tactics  x334", new Vector2(-470, 326), new Vector2(330, 42), TextAlignmentOptions.Left);
        titleText.fontSize = 21;
        titleText.fontStyle = FontStyles.Bold;

        TMP_Text runStatusText = CreateText(canvas.transform, "Run Status Text", "1) Break enemy posture, then open them from behind.", new Vector2(-20, 334), new Vector2(650, 26), TextAlignmentOptions.Left);
        runStatusText.fontSize = 13;
        runStatusText.color = new Color(0.76f, 1.0f, 0.82f);

        TMP_Text battleGuideText = CreateText(canvas.transform, "Battle Guide Text", "Push targets into allies to deal bonus damage equal to 25% max HP.", new Vector2(-20, 306), new Vector2(680, 24), TextAlignmentOptions.Left);
        battleGuideText.fontSize = 12;
        battleGuideText.color = new Color(0.90f, 0.95f, 1.0f);

        TMP_Text stageText = CreateText(canvas.transform, "Stage Text", "BATTLE PREP", new Vector2(-210, 252), new Vector2(250, 28), TextAlignmentOptions.Center);
        stageText.fontSize = 17;
        stageText.color = new Color(0.92f, 0.86f, 0.55f);
        TMP_Text stageObjectiveText = CreateText(canvas.transform, "Stage Objective Text", "Formation grid / enemy intent preview", new Vector2(70, 252), new Vector2(390, 22), TextAlignmentOptions.Left);
        stageObjectiveText.fontSize = 12;
        stageObjectiveText.color = new Color(1.0f, 0.94f, 0.72f);
        TMP_Text stageProgressText = CreateText(canvas.transform, "Stage Progress Text", "Turn Cost 3 / Chain slots ready", new Vector2(360, 226), new Vector2(330, 20), TextAlignmentOptions.Right);
        stageProgressText.fontSize = 11;
        stageProgressText.color = new Color(0.72f, 0.90f, 1.0f);

        TMP_Text playerHpText = CreateText(canvas.transform, "Player HP Text", "Hero HP: 100/100 (100%)", new Vector2(-330, -560), new Vector2(170, 20), TextAlignmentOptions.Left);
        playerHpText.fontSize = 10;
        TMP_Text playerCardTitleText = CreateText(canvas.transform, "Player Card Title Text", "ALLY UNIT  /  HERO", new Vector2(-530, 250), new Vector2(210, 24), TextAlignmentOptions.Center);
        playerCardTitleText.fontSize = 16;
        playerCardTitleText.fontStyle = FontStyles.Bold;
        playerCardTitleText.color = new Color(0.92f, 0.86f, 0.55f);
        // Portrait border frames — subtle dark outline
        CreatePortraitFrame(canvas.transform, "Player Portrait Frame", new Vector2(-592, 200), new Vector2(72, 72));
        CreatePortraitPixelAccent(canvas.transform, "Player", new Vector2(-592, 200), new Color(0.38f, 0.78f, 1.0f, 0.88f));
        Image playerSpriteImage = CreatePortrait(canvas.transform, "Player Sprite", new Vector2(-592, 200), new Vector2(58, 58), "Assets/Art/ReferenceSprites/reference_paladin_full.png");
        ConfigureBattleSpriteMotion(playerSpriteImage, 3.5f, 1.45f, 0f, 14f, 0.06f, false);
        Slider playerHpSlider = CreateHpSlider(canvas.transform, "Player HP Slider", new Vector2(-330, -578), new Vector2(170, 8), new Color(0.22f, 0.72f, 0.38f));
        TMP_Text playerApText = CreateText(canvas.transform, "Player AP Text", "AP: 3/3 (100%)", new Vector2(-160, -560), new Vector2(170, 20), TextAlignmentOptions.Left);
        playerApText.fontSize = 10;
        Slider playerApSlider = CreateHpSlider(canvas.transform, "Player AP Slider", new Vector2(-160, -578), new Vector2(170, 8), new Color(0.26f, 0.56f, 1.0f));
        TMP_Text playerStatusText = CreateText(canvas.transform, "Player Status Text", "Status: Ready", new Vector2(-245, -600), new Vector2(300, 20), TextAlignmentOptions.Left);
        playerStatusText.fontSize = 10;
        playerStatusText.color = new Color(0.78f, 1.0f, 0.76f);
        TMP_Text playerShieldText = CreateText(canvas.transform, "Player Shield Text", "", new Vector2(-245, -620), new Vector2(300, 18), TextAlignmentOptions.Left);
        playerShieldText.fontSize = 14;
        playerShieldText.color = new Color(0.45f, 0.78f, 1.0f);
        TMP_Text enemyHpText = CreateText(canvas.transform, "Enemy HP Text", "Slime HP: 80/80 (100%)", new Vector2(540, 252), new Vector2(160, 22), TextAlignmentOptions.Right);
        enemyHpText.fontSize = 10;
        TMP_Text enemyCardTitleText = CreateText(canvas.transform, "Enemy Card Title Text", "ENEMY", new Vector2(548, 235), new Vector2(150, 24), TextAlignmentOptions.Center);
        enemyCardTitleText.fontSize = 15;
        enemyCardTitleText.fontStyle = FontStyles.Bold;
        enemyCardTitleText.color = new Color(1.0f, 0.64f, 0.48f);
        TMP_Text versusDividerText = CreateText(canvas.transform, "Versus Divider Text", "BATTLE LINE", new Vector2(0, 146), new Vector2(220, 24), TextAlignmentOptions.Center);
        versusDividerText.fontSize = 14;
        versusDividerText.fontStyle = FontStyles.Bold;
        versusDividerText.color = new Color(0.96f, 0.78f, 0.36f, 0.42f);
        // Portrait border frames — subtle dark outline
        CreatePortraitFrame(canvas.transform, "Enemy Portrait Frame", new Vector2(505, 198), new Vector2(70, 70));
        CreatePortraitPixelAccent(canvas.transform, "Enemy", new Vector2(505, 198), new Color(1.0f, 0.45f, 0.24f, 0.88f));
        Image enemySpriteImage = CreatePortrait(canvas.transform, "Enemy Sprite", new Vector2(505, 198), new Vector2(56, 56), "Assets/Art/ReferenceSprites/reference_goblin_full.png");
        ConfigureBattleSpriteMotion(enemySpriteImage, 4.5f, 1.25f, 0.35f, 18f, 0.08f, true);
        Image burnOverlay = CreateStatusOverlay(canvas.transform, "Burn Overlay", new Vector2(505, 198), new Vector2(56, 56));
        Image stunOverlay = CreateStatusOverlay(canvas.transform, "Stun Overlay", new Vector2(505, 198), new Vector2(56, 56));
        Image brokenOverlay = CreateStatusOverlay(canvas.transform, "Broken Overlay", new Vector2(505, 198), new Vector2(56, 56));
        burnOverlay.gameObject.SetActive(false);
        stunOverlay.gameObject.SetActive(false);
        brokenOverlay.gameObject.SetActive(false);
        Slider enemyHpSlider = CreateHpSlider(canvas.transform, "Enemy HP Slider", new Vector2(540, 234), new Vector2(150, 8), new Color(0.82f, 0.22f, 0.24f));
        TMP_Text enemyStatusText = CreateText(canvas.transform, "Enemy Status Text", "Status: None", new Vector2(540, 218), new Vector2(160, 18), TextAlignmentOptions.Right);
        enemyStatusText.fontSize = 10;
        TMP_Text enemyIntentText = CreateText(canvas.transform, "Enemy Intent Text", "Next: Normal Attack (15)", new Vector2(540, 200), new Vector2(160, 32), TextAlignmentOptions.Right);
        enemyIntentText.fontSize = 10;
        enemyIntentText.color = new Color(1.0f, 0.78f, 0.42f);
        TMP_Text enemyBreakText = CreateText(canvas.transform, "Enemy Break Text", "Break: 2/2", new Vector2(540, 174), new Vector2(160, 20), TextAlignmentOptions.Right);
        enemyBreakText.fontSize = 10;
        enemyBreakText.color = new Color(1.0f, 0.58f, 0.82f);
        Slider enemyBreakSlider = CreateHpSlider(canvas.transform, "Enemy Break Slider", new Vector2(540, 158), new Vector2(150, 8), new Color(0.92f, 0.36f, 0.72f));
        TMP_Text messageText = CreateText(canvas.transform, "Message Text", "Battle Start!", new Vector2(0, 286), new Vector2(420, 22), TextAlignmentOptions.Center);
        messageText.fontSize = 12;
        messageText.color = new Color(1.0f, 0.94f, 0.72f, 0.86f);
        TMP_Text impactText = CreateText(canvas.transform, "Impact Text", "IMPACT READY", new Vector2(262, 254), new Vector2(176, 18), TextAlignmentOptions.Center);
        impactText.fontSize = 9;
        impactText.color = new Color(1.0f, 0.84f, 0.36f, 0.82f);
        Image demoRoutePanel = CreatePanel(canvas.transform, "Demo Route Panel", new Vector2(-248, 222), new Vector2(236, 22), new Color(0.025f, 0.034f, 0.052f, 0.38f));
        demoRoutePanel.raycastTarget = false;
        TMP_Text demoRouteText = CreateText(canvas.transform, "Demo Route Text", "PATH  HERO / FIRE / GUARD / RESULT / RETRY", new Vector2(-248, 222), new Vector2(220, 16), TextAlignmentOptions.Center);
        demoRouteText.fontSize = 7;
        demoRouteText.color = new Color(0.96f, 0.92f, 0.68f, 0.62f);
        Image captureRehearsalPanel = CreatePanel(canvas.transform, "Capture Rehearsal Panel", new Vector2(262, 222), new Vector2(184, 22), new Color(0.030f, 0.045f, 0.070f, 0.40f));
        captureRehearsalPanel.raycastTarget = false;
        TMP_Text captureRehearsalText = CreateText(canvas.transform, "Capture Rehearsal Text", "SHOT 1/5  CLICK HERO", new Vector2(262, 222), new Vector2(170, 16), TextAlignmentOptions.Center);
        captureRehearsalText.fontSize = 7;
        captureRehearsalText.color = new Color(0.72f, 0.90f, 1.0f, 0.64f);
        TMP_Text skillHelpText = CreateText(canvas.transform, "Skill Help Text", "Skill Help", new Vector2(-420, -620), new Vector2(250, 28), TextAlignmentOptions.TopLeft);
        skillHelpText.fontSize = 5;
        skillHelpText.color = new Color(0.72f, 0.90f, 1.0f);
        Image resultSummaryPanel = CreatePanel(canvas.transform, "Result Summary Panel", new Vector2(0, -42), new Vector2(620, 230), new Color(0.03f, 0.04f, 0.06f, 0.92f));
        resultSummaryPanel.gameObject.SetActive(false);
        TMP_Text resultSummaryText = CreateText(canvas.transform, "Result Summary Text", "Result Summary", new Vector2(0, -42), new Vector2(580, 195), TextAlignmentOptions.TopLeft);
        resultSummaryText.fontSize = 18;
        resultSummaryText.color = new Color(1.0f, 0.92f, 0.58f);
        resultSummaryText.gameObject.SetActive(false);
        // ── Command Preview Panel ──
        Image commandPreviewPanel = CreatePanel(canvas.transform, "Command Preview Panel", new Vector2(10, -244), new Vector2(500, 48), new Color(0.04f, 0.06f, 0.12f, 0.92f));
        commandPreviewPanel.gameObject.SetActive(false);
        TMP_Text commandPreviewText = CreateText(canvas.transform, "Command Preview Text", "Select a skill to preview", new Vector2(10, -244), new Vector2(475, 42), TextAlignmentOptions.MidlineLeft);
        commandPreviewText.fontSize = 15;
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

        CreatePremiumCommandFrame(canvas.transform);
        TMP_Text commandHintText = CreateText(canvas.transform, "Command Hint Text", "Select Hero to open contextual commands.", new Vector2(240, -620), new Vector2(320, 18), TextAlignmentOptions.Center);
        commandHintText.fontSize = 9;
        commandHintText.color = new Color(0.96f, 0.92f, 0.68f);
        Image referenceSkillDetailPanel = CreatePanel(canvas.transform, "Reference Skill Detail Panel", new Vector2(522, -238), new Vector2(210, 82), new Color(0.045f, 0.034f, 0.052f, 0.92f));
        referenceSkillDetailPanel.raycastTarget = false;
        TMP_Text referenceSkillDetailText = CreateText(canvas.transform, "Reference Skill Detail Text", "SELECTED SKILL\nFIRE / AP2 / Break +1", new Vector2(522, -238), new Vector2(188, 62), TextAlignmentOptions.Center);
        referenceSkillDetailText.fontSize = 12;
        referenceSkillDetailText.color = new Color(1.0f, 0.84f, 0.48f);
        referenceSkillDetailPanel.gameObject.SetActive(false);
        referenceSkillDetailText.gameObject.SetActive(false);
        Image enemyIntentCardPanel = CreatePanel(canvas.transform, "Enemy Intent Card Panel", new Vector2(522, -152), new Vector2(210, 54), new Color(0.08f, 0.035f, 0.045f, 0.88f));
        enemyIntentCardPanel.raycastTarget = false;
        TMP_Text enemyIntentCardText = CreateText(canvas.transform, "Enemy Intent Card Text", "INTENT: Revenge / Shield", new Vector2(522, -152), new Vector2(188, 30), TextAlignmentOptions.Center);
        enemyIntentCardText.fontSize = 11;
        enemyIntentCardText.color = new Color(1.0f, 0.70f, 0.42f);

        Image actionCommandPanel = CreatePanel(canvas.transform, "Action Command Panel", new Vector2(335, -302), new Vector2(560, 92), new Color(0.030f, 0.038f, 0.060f, 0.96f));
        TMP_Text selectedUnitText = CreateText(canvas.transform, "Selected Unit Text", "Selected: Hero / AP 3", new Vector2(160, -274), new Vector2(210, 22), TextAlignmentOptions.Left);
        selectedUnitText.fontSize = 12;
        selectedUnitText.color = new Color(0.96f, 0.92f, 0.68f);
        selectedUnitText.gameObject.SetActive(false);

        Button attackButton = CreateCenteredButton(canvas.transform, "Attack Button", "ATK", new Vector2(158, -314), new Vector2(72, 30));
        Button fireSkillButton = CreateCenteredButton(canvas.transform, "Fire Skill Button", "FIRE", new Vector2(236, -314), new Vector2(72, 30));
        Button iceSkillButton = CreateCenteredButton(canvas.transform, "Ice Lance Button", "ICE", new Vector2(314, -314), new Vector2(72, 30));
        Button lightningSkillButton = CreateCenteredButton(canvas.transform, "Lightning Strike Button", "LIT", new Vector2(392, -314), new Vector2(72, 30));
        Button earthSkillButton = CreateCenteredButton(canvas.transform, "Earth Wall Button", "EARTH", new Vector2(470, -314), new Vector2(72, 30));
        Button guardButton = CreateCenteredButton(canvas.transform, "Guard Button", "GUARD", new Vector2(548, -314), new Vector2(78, 30));
        Button endTurnButton = CreateCenteredButton(canvas.transform, "End Turn Button", "END", new Vector2(548, -274), new Vector2(78, 28));
        actionCommandPanel.gameObject.SetActive(false);
        attackButton.gameObject.SetActive(false);
        fireSkillButton.gameObject.SetActive(false);
        iceSkillButton.gameObject.SetActive(false);
        lightningSkillButton.gameObject.SetActive(false);
        earthSkillButton.gameObject.SetActive(false);
        guardButton.gameObject.SetActive(false);
        endTurnButton.gameObject.SetActive(false);
        Button retryButton = CreateButton(canvas.transform, "Retry Button", "Retry", new Vector2(170, 145), new Vector2(140, 48));
        retryButton.gameObject.SetActive(false);
        Button continueButton = CreateButton(canvas.transform, "Continue Button", "Continue", new Vector2(320, 145), new Vector2(150, 48));
        continueButton.gameObject.SetActive(false);
        // Create the label child that shows "Continue" by default, will be changed to "Next Encounter" at runtime
        TMP_Text continueButtonLabel = continueButton.GetComponentInChildren<TMP_Text>();
        Button stageSelectButton = CreateButton(canvas.transform, "Stage Select Button", "Stage Select", new Vector2(-505, 28), new Vector2(120, 40));
        Button speedToggleButton = CreateButton(canvas.transform, "Speed Toggle Button", "1x", new Vector2(520, 672), new Vector2(52, 30));
        Button autoBattleButton = CreateButton(canvas.transform, "Auto Battle Button", "Auto", new Vector2(458, 672), new Vector2(58, 30));
        Button itemButton = CreateCenteredButton(canvas.transform, "Item Button", "ITEM", new Vector2(470, -274), new Vector2(72, 28));
        itemButton.gameObject.SetActive(false);
        Button pauseButton = CreateButton(canvas.transform, "Pause Button", "II", new Vector2(582, 672), new Vector2(52, 30));
        Button battleLogToggleButton = CreateButton(canvas.transform, "Battle Log Toggle Button", "Log", new Vector2(360, 674), new Vector2(60, 24));
        TMP_Text battleLogToggleLabel = battleLogToggleButton.GetComponentInChildren<TMP_Text>();
        stageSelectButton.gameObject.SetActive(false);

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
        SetObjectReference(serializedBattleUI, "referencePlayerSprite", LoadPixelSprite("Assets/Art/ReferenceSprites/reference_paladin_full.png"));
        SetObjectReference(serializedBattleUI, "enemyHpText", enemyHpText);
        SetObjectReference(serializedBattleUI, "enemyHpSlider", enemyHpSlider);
        SetObjectReference(serializedBattleUI, "enemyStatusText", enemyStatusText);
        SetObjectReference(serializedBattleUI, "enemyIntentText", enemyIntentText);
        SetObjectReference(serializedBattleUI, "enemyBreakText", enemyBreakText);
        SetObjectReference(serializedBattleUI, "enemyBreakSlider", enemyBreakSlider);
        SetObjectReference(serializedBattleUI, "enemySpriteImage", enemySpriteImage);
        SetObjectReference(serializedBattleUI, "referenceEnemySprite", LoadPixelSprite("Assets/Art/ReferenceSprites/reference_goblin_full.png"));
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
        SetObjectReference(serializedBattleUI, "playerSelectButton", playerSelectButton);
        SetObjectReference(serializedBattleUI, "playerSelectionHighlight", playerSelectionHighlight);
        SetObjectReference(serializedBattleUI, "selectedUnitText", selectedUnitText);
        SetObjectReference(serializedBattleUI, "attackButton", attackButton);
        SetObjectReference(serializedBattleUI, "fireSkillButton", fireSkillButton);
        SetObjectReference(serializedBattleUI, "iceSkillButton", iceSkillButton);
        SetObjectReference(serializedBattleUI, "lightningSkillButton", lightningSkillButton);
        SetObjectReference(serializedBattleUI, "earthSkillButton", earthSkillButton);
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

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, ScenePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Selection.activeGameObject = battleManagerObject;
        EditorUtility.DisplayDialog(
            "BattleScene Created",
            "Assets/Scenes/BattleScene.unity created!\n\nPress Play, click Hero in the party roster, then test Attack / Skills / Guard / End Turn.",
            "OK"
        );
    }

    [MenuItem("Tools/Codex Tactics/Validate Battle Test Scene")]
    public static void ValidateBattleTestScene()
    {
        if (!System.IO.File.Exists(ScenePath))
        {
            EditorUtility.DisplayDialog("BattleScene Test Failed", "BattleScene file does not exist.\n\nRun Tools > Codex Tactics > Create Battle Test Scene first.", "OK");
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
        Image enemyRosterMiniSprite1 = FindImageIncludingInactive("Enemy Roster Mini Sprite 1");
        Image tacticalGridTile = FindImage("Tactical Grid Tile 1-1");
        Image skillActionArc = FindImage("Skill Action Arc");
        Image heroStandeeBody = FindImage("Hero Standee Body");
        Image heroStandeeBlade = FindImage("Hero Standee Blade");
        Image enemyStandeeBody = FindImage("Enemy Standee Body");
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
        Image moonlightBeam = FindImage("Moonlight Beam Panel");
        Image foregroundFog = FindImage("Foreground Fog Panel");
        Image heroBaseRing = FindImage("Hero Base Ring Panel");
        Image enemyBaseRing = FindImage("Enemy Base Ring Panel");
        Image progressSkillCard1 = FindImage("Progress Skill Card 1");
        Image progressBattleStartPanel = FindImage("Progress Battle Start Panel");
        TMP_Text progressBattleStartText = FindText("Progress Battle Start Text");
        Image progressTurnDial = FindImage("Progress Turn Dial");
        Image progressBottomPortrait1 = FindImage("Progress Bottom Portrait Sprite 1");

        AppendCheck(ref passed, ref report, "Battle stage backdrop exists", battleStageBackdropPanel != null);
        AppendCheck(ref passed, ref report, "Battle stage backdrop has premium dark RPG styling", IsDecorativePanelLikelyConfigured(battleStageBackdropPanel, 1100f, 560f));
        AppendCheck(ref passed, ref report, "Battle stage floor glow exists", battleStageFloorPanel != null);
        AppendCheck(ref passed, ref report, "Battle stage floor glow is readable", IsDecorativePanelLikelyConfigured(battleStageFloorPanel, 820f, 220f));
        AppendCheck(ref passed, ref report, "Battlefield has layered forest silhouette", IsDecorativePanelLikelyConfigured(distantForestSilhouette, 680f, 70f));
        AppendCheck(ref passed, ref report, "Battlefield has moonlight beam depth", IsDecorativePanelLikelyConfigured(moonlightBeam, 110f, 330f));
        AppendCheck(ref passed, ref report, "Battlefield has foreground fog layer", IsDecorativePanelLikelyConfigured(foregroundFog, 650f, 34f));
        AppendCheck(ref passed, ref report, "Battlefield unit base rings exist", IsDecorativePanelLikelyConfigured(heroBaseRing, 120f, 18f) && IsDecorativePanelLikelyConfigured(enemyBaseRing, 140f, 20f));
        AppendCheck(ref passed, ref report, "Top gold divider exists", topGoldDividerPanel != null && IsDecorativePanelLikelyConfigured(topGoldDividerPanel, 1000f, 3f));
        AppendCheck(ref passed, ref report, "Command gold divider exists", commandGoldDividerPanel != null && IsDecorativePanelLikelyConfigured(commandGoldDividerPanel, 900f, 2f));
        AppendCheck(ref passed, ref report, "Tactical grid tile exists", IsDecorativePanelLikelyConfigured(tacticalGridTile, 70f, 32f));
        AppendCheck(ref passed, ref report, "Skill action arc exists", IsDecorativePanelLikelyConfigured(skillActionArc, 450f, 4f));
        AppendCheck(ref passed, ref report, "Hero mature high-density pixel standee exists", IsSpriteImageLikelyConfigured(heroStandeeBody, 165f, 220f) && IsDecorativePanelLikelyConfigured(heroStandeeBlade, 6f, 56f));
        AppendCheck(ref passed, ref report, "Enemy mature high-density pixel standee exists", IsSpriteImageLikelyConfigured(enemyStandeeBody, 185f, 218f) && IsDecorativePanelLikelyConfigured(enemyStandeeCrown, 52f, 8f));
        AppendCheck(ref passed, ref report, "Battle portraits have idle bob and hit reaction motion", HasBattleSpriteMotion(playerSpriteImage) && HasBattleSpriteMotion(enemySpriteImage));
        AppendCheck(ref passed, ref report, "Battlefield standees have idle bob motion", HasBattleSpriteMotion(heroStandeeBody) && HasBattleSpriteMotion(enemyStandeeBody));
        AppendCheck(ref passed, ref report, "Premium command header exists", IsDecorativePanelLikelyConfigured(commandHeaderPanel, 240f, 24f) && IsNameplateTextLikelyConfigured(commandHeaderText, "COMMAND", "CHAIN"));
        AppendCheck(ref passed, ref report, "Skill tier badge exists", IsDecorativePanelLikelyConfigured(skillTierBadge, 56f, 20f));
        AppendCheck(ref passed, ref report, "Party roster panel exists", partyRosterPanel != null && IsDecorativePanelLikelyConfigured(partyRosterPanel, 280f, 360f));
        AppendCheck(ref passed, ref report, "Party roster slots exist", IsDecorativePanelLikelyConfigured(partyRosterSlot1, 280f, 66f));
        AppendCheck(ref passed, ref report, "Enemy roster slots exist", IsDecorativePanelLikelyConfigured(enemyRosterSlot1, 150f, 50f));
        AppendCheck(ref passed, ref report, "Party roster high-density mini sprites exist", IsSpriteImageLikelyConfigured(partyRosterMiniSprite1, 30f, 38f));
        AppendCheck(ref passed, ref report, "Player roster select button exists", IsButtonLikelyConfigured(playerSelectButton));
        AppendCheck(ref passed, ref report, "Player selection highlight starts hidden", playerSelectionHighlight != null && !playerSelectionHighlight.gameObject.activeSelf);
        AppendCheck(ref passed, ref report, "Action command panel starts hidden until ally click", IsOverlayPanelLikelyConfigured(actionCommandPanel, 540f, 88f) && actionCommandPanel != null && !actionCommandPanel.gameObject.activeSelf);
        AppendCheck(ref passed, ref report, "Selected unit prompt starts hidden with command UI", selectedUnitText != null && !selectedUnitText.gameObject.activeSelf);
        AppendCheck(ref passed, ref report, "Enemy roster high-density mini sprites exist", IsSpriteImageLikelyConfigured(enemyRosterMiniSprite1, 30f, 36f));
        AppendCheck(ref passed, ref report, "Player card title exists", IsNameplateTextLikelyConfigured(playerCardTitleText, "ALLY", "HERO"));
        AppendCheck(ref passed, ref report, "Enemy card title exists", IsNameplateTextLikelyConfigured(enemyCardTitleText, "ENEMY", "ENEMY"));
        AppendCheck(ref passed, ref report, "Battle line divider text exists", IsNameplateTextLikelyConfigured(versusDividerText, "BATTLE", "LINE"));
        AppendCheck(ref passed, ref report, "Player portrait pixel accents exist", IsPortraitAccentLikelyConfigured(playerPortraitPixelAccent1) && IsPortraitAccentLikelyConfigured(playerPortraitPixelAccent4));
        AppendCheck(ref passed, ref report, "Enemy portrait pixel accents exist", IsPortraitAccentLikelyConfigured(enemyPortraitPixelAccent1) && IsPortraitAccentLikelyConfigured(enemyPortraitPixelAccent4));
        AppendCheck(ref passed, ref report, "Top Status panel exists", topStatusPanel != null);
        AppendCheck(ref passed, ref report, "Top Status panel has premium dark RPG styling", IsProfessionalPanelLikelyConfigured(topStatusPanel, 1150f, 58f));
        AppendCheck(ref passed, ref report, "Player Card panel exists", playerCardPanel != null);
        AppendCheck(ref passed, ref report, "Player Card panel has premium dark RPG styling", IsProfessionalPanelLikelyConfigured(playerCardPanel, 280f, 420f));
        AppendCheck(ref passed, ref report, "Enemy Card panel exists", enemyCardPanel != null);
        AppendCheck(ref passed, ref report, "Enemy Card panel has premium dark RPG styling", IsProfessionalPanelLikelyConfigured(enemyCardPanel, 230f, 360f));
        AppendCheck(ref passed, ref report, "Battle Center panel exists", battleCenterPanel != null);
        AppendCheck(ref passed, ref report, "Battle Center panel has premium dark RPG styling", IsDecorativePanelLikelyConfigured(battleCenterPanel, 520f, 56f));
        AppendCheck(ref passed, ref report, "Command Bar panel exists", commandBarPanel != null);
        AppendCheck(ref passed, ref report, "Command Bar panel has premium dark RPG styling", IsProfessionalPanelLikelyConfigured(commandBarPanel, 1100f, 88f));
        AppendCheck(ref passed, ref report, "Battle Guide text exists", battleGuideText != null);
        AppendCheck(ref passed, ref report, "Battle Guide text explains main controls", IsBattleGuideTextLikelyConfigured(battleGuideText));
        AppendCheck(ref passed, ref report, "Run Status text exists", runStatusText != null);
        AppendCheck(ref passed, ref report, "Run Status text shows the current stage run", IsRunStatusTextLikelyConfigured(runStatusText));
        AppendCheck(ref passed, ref report, "Stage text exists", stageText != null);
        AppendCheck(ref passed, ref report, "Stage text starts at the first encounter", IsStageTextLikelyConfigured(stageText));
        AppendCheck(ref passed, ref report, "Stage Objective text exists", stageObjectiveText != null);
        AppendCheck(ref passed, ref report, "Stage Objective text explains the first objective", IsStageObjectiveTextLikelyConfigured(stageObjectiveText));
        AppendCheck(ref passed, ref report, "Stage Progress text exists", stageProgressText != null);
        AppendCheck(ref passed, ref report, "Stage Progress text shows encounter count", IsStageProgressTextLikelyConfigured(stageProgressText));
        AppendCheck(ref passed, ref report, "Player Status text exists", playerStatusText != null);
        AppendCheck(ref passed, ref report, "Impact text exists", impactText != null);
        AppendCheck(ref passed, ref report, "Demo route chip exists", IsDecorativePanelLikelyConfigured(demoRoutePanel, 220f, 18f));
        AppendCheck(ref passed, ref report, "Demo route chip shows compact reviewer path", IsDemoRouteTextLikelyConfigured(demoRouteText));
        AppendCheck(ref passed, ref report, "Capture rehearsal chip exists", IsDecorativePanelLikelyConfigured(captureRehearsalPanel, 170f, 18f));
        AppendCheck(ref passed, ref report, "Capture rehearsal chip starts with compact step prompt", IsCaptureRehearsalTextLikelyConfigured(captureRehearsalText));
        AppendCheck(ref passed, ref report, "Skill Help text exists", skillHelpText != null);
        AppendCheck(ref passed, ref report, "Runtime labels skip raycast for UI performance", IsTextRaycastOptimized(runStatusText, battleGuideText, stageText, stageObjectiveText, stageProgressText, playerHpText, playerApText, enemyHpText, skillHelpText, messageText, impactText, demoRouteText, captureRehearsalText));
        AppendCheck(ref passed, ref report, "Enemy Status text exists", enemyStatusText != null);
        AppendCheck(ref passed, ref report, "Enemy Intent text exists", enemyIntentText != null);
        AppendCheck(ref passed, ref report, "Enemy Break text exists", enemyBreakText != null);
        AppendCheck(ref passed, ref report, "Enemy Break slider exists", enemyBreakSlider != null);
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
        AppendCheck(ref passed, ref report, "Command Preview panel starts hidden", commandPreviewPanel != null && !commandPreviewPanel.gameObject.activeSelf);
        AppendCheck(ref passed, ref report, "Command Preview text exists", commandPreviewText != null);
        AppendCheck(ref passed, ref report, "Bottom command strip has visible select-unit hint", IsCommandHintTextLikelyConfigured(commandHintText));
        AppendCheck(ref passed, ref report, "Reference-style skill detail card exists", IsDecorativePanelLikelyConfigured(referenceSkillDetailPanel, 160f, 76f) && IsNameplateTextLikelyConfigured(referenceSkillDetailText, "SKILL", "AP2"));
        AppendCheck(ref passed, ref report, "Reference-style enemy intent card exists", IsDecorativePanelLikelyConfigured(enemyIntentCardPanel, 190f, 50f) && IsNameplateTextLikelyConfigured(enemyIntentCardText, "INTENT", "Revenge"));
        AppendCheck(ref passed, ref report, "Progress-reference right skill cards exist", IsDecorativePanelLikelyConfigured(progressSkillCard1, 230f, 78f));
        AppendCheck(ref passed, ref report, "Progress-reference battle start CTA exists", IsDecorativePanelLikelyConfigured(progressBattleStartPanel, 200f, 46f) && IsNameplateTextLikelyConfigured(progressBattleStartText, "BATTLE", "START"));
        AppendCheck(ref passed, ref report, "Progress-reference bottom turn dial exists", IsDecorativePanelLikelyConfigured(progressTurnDial, 80f, 80f));
        AppendCheck(ref passed, ref report, "Progress-reference bottom portrait strip exists", IsSpriteImageLikelyConfigured(progressBottomPortrait1, 54f, 54f));
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
            && color.a >= 0.50f
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
            && accentImage.color.a >= 0.4f;
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
            && rectTransform.sizeDelta.x >= 640f
            && text.Contains("Push")
            && text.Contains("targets")
            && text.Contains("bonus damage");
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
            && rectTransform.sizeDelta.x >= 200f
            && text.Contains("PATH")
            && text.Contains("HERO")
            && text.Contains("FIRE")
            && text.Contains("GUARD")
            && text.Contains("RESULT")
            && text.Contains("RETRY");
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
            && rectTransform.sizeDelta.x >= 160f
            && text.Contains("SHOT")
            && text.Contains("1/5")
            && text.Contains("CLICK HERO");
    }

    private static bool IsStageTextLikelyConfigured(TMP_Text stageText)
    {
        if (stageText == null)
        {
            return false;
        }

        RectTransform rectTransform = stageText.GetComponent<RectTransform>();
        return rectTransform != null
            && rectTransform.sizeDelta.x >= 240f
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
            && rectTransform.sizeDelta.x >= 600f
            && text.Contains("Break enemy posture")
            && text.Contains("behind");
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
            && rectTransform.sizeDelta.x >= 380f
            && text.Contains("Formation")
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
            && rectTransform.sizeDelta.x >= 320f
            && text.Contains("Turn Cost")
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
                || importer.textureCompression != TextureImporterCompression.Uncompressed;

            if (needsReimport)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.filterMode = FilterMode.Point;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.mipmapEnabled = false;
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
        Color lineColor = new Color(0.35f, 0.80f, 0.55f, 0.18f);
        for (int row = 0; row < 4; row++)
        {
            for (int col = 0; col < 6; col++)
            {
                Vector2 pos = new Vector2(-238 + col * 88 + row * 28, -118 + row * 38);
                Image tile = CreatePanel(parent, $"Tactical Grid Tile {row + 1}-{col + 1}", pos, new Vector2(78, 32), new Color(0.18f, 0.76f, 0.74f, 0.35f));
                tile.rectTransform.localRotation = Quaternion.Euler(0, 0, -10f);
                tile.raycastTarget = false;
            }
        }

        Image allyMarker = CreatePanel(parent, "Ally Formation Marker", new Vector2(-286, -42), new Vector2(30, 68), new Color(0.42f, 1.0f, 0.78f, 0.26f));
        Image enemyMarker = CreatePanel(parent, "Enemy Formation Marker", new Vector2(238, 52), new Vector2(30, 68), new Color(1.0f, 0.26f, 0.86f, 0.28f));
        Image actionArc = CreatePanel(parent, "Skill Action Arc", new Vector2(-6, 112), new Vector2(470, 4), new Color(1.0f, 0.80f, 0.48f, 0.44f));
        allyMarker.raycastTarget = false;
        enemyMarker.raycastTarget = false;
        actionArc.raycastTarget = false;
        actionArc.rectTransform.localRotation = Quaternion.Euler(0, 0, -12f);
        _ = lineColor;
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
        Image distantForest = CreatePanel(parent, "Distant Forest Silhouette Panel", new Vector2(0, 92), new Vector2(820, 80), new Color(0.010f, 0.040f, 0.034f, 0.38f));
        Image moonlight = CreatePanel(parent, "Moonlight Beam Panel", new Vector2(96, 28), new Vector2(112, 430), new Color(0.46f, 0.64f, 0.88f, 0.36f));
        Image fog = CreatePanel(parent, "Foreground Fog Panel", new Vector2(0, -172), new Vector2(740, 44), new Color(0.62f, 0.78f, 0.70f, 0.36f));
        Image rearHorizon = CreatePanel(parent, "Rear Horizon Gold Line Panel", new Vector2(0, 58), new Vector2(620, 3), new Color(0.86f, 0.62f, 0.24f, 0.32f));
        distantForest.raycastTarget = false;
        moonlight.raycastTarget = false;
        fog.raycastTarget = false;
        rearHorizon.raycastTarget = false;
        moonlight.rectTransform.localRotation = Quaternion.Euler(0, 0, -11f);
    }

    private static void CreateBattlefieldUnitStandees(Transform parent)
    {
        // User-provided reference sprites are now used directly for the battlefield standees.
        // Keep the surrounding rings/aura original so the portfolio scene has a readable tactical frame.
        CreatePanel(parent, "Hero Standee Shadow", new Vector2(-206, -112), new Vector2(94, 16), new Color(0.0f, 0.0f, 0.0f, 0.34f));
        CreatePanel(parent, "Hero Base Ring Panel", new Vector2(-206, -103), new Vector2(128, 20), new Color(0.34f, 0.72f, 1.0f, 0.38f));
        CreatePanel(parent, "Hero Standee Aura", new Vector2(-205, -28), new Vector2(112, 148), new Color(0.25f, 0.60f, 1.0f, 0.13f));
        Image heroBody = CreateSpritePanel(parent, "Hero Standee Body", "Assets/Art/ReferenceSprites/reference_paladin_full.png", new Vector2(-204, -8), new Vector2(176, 220));
        ConfigureBattleSpriteMotion(heroBody, 5f, 1.1f, 0.15f, 20f, 0.05f, false);
        Image heroBlade = CreatePanel(parent, "Hero Standee Blade", new Vector2(-166, -24), new Vector2(7, 60), new Color(0.88f, 0.94f, 1.0f, 0.38f));
        heroBlade.rectTransform.localRotation = Quaternion.Euler(0, 0, -18f);
        heroBody.raycastTarget = false;
        heroBlade.raycastTarget = false;

        CreatePanel(parent, "Enemy Standee Shadow", new Vector2(230, -108), new Vector2(122, 20), new Color(0.0f, 0.0f, 0.0f, 0.38f));
        CreatePanel(parent, "Enemy Base Ring Panel", new Vector2(230, -99), new Vector2(148, 22), new Color(1.0f, 0.36f, 0.70f, 0.38f));
        CreatePanel(parent, "Enemy Standee Aura", new Vector2(232, -26), new Vector2(136, 156), new Color(0.82f, 0.20f, 1.0f, 0.13f));
        Image enemyBody = CreateSpritePanel(parent, "Enemy Standee Body", "Assets/Art/ReferenceSprites/reference_goblin_full.png", new Vector2(232, -18), new Vector2(190, 224));
        ConfigureBattleSpriteMotion(enemyBody, 6f, 0.95f, 0.45f, 24f, 0.07f, true);
        Image enemyCrown = CreatePanel(parent, "Enemy Standee Crown", new Vector2(232, 66), new Vector2(60, 9), new Color(1.0f, 0.66f, 0.20f, 0.38f));
        enemyBody.raycastTarget = false;
        enemyCrown.raycastTarget = false;
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

    private static void CreateProgressReferenceTopControls(Transform parent)
    {
        CreatePanel(parent, "Reference Top Icon Coin", new Vector2(-610, 326), new Vector2(26, 26), new Color(0.92f, 0.78f, 0.38f, 0.82f));
        CreatePanel(parent, "Reference Top Buff Icon 1", new Vector2(-584, 288), new Vector2(28, 28), new Color(0.20f, 0.52f, 0.22f, 0.88f));
        CreatePanel(parent, "Reference Top Buff Icon 2", new Vector2(-548, 288), new Vector2(28, 28), new Color(0.18f, 0.48f, 0.24f, 0.88f));

        TMP_Text autoIcon = CreateText(parent, "Reference Auto Icon Text", "AUTO", new Vector2(398, 326), new Vector2(64, 28), TextAlignmentOptions.Center);
        autoIcon.fontSize = 14;
        autoIcon.fontStyle = FontStyles.Bold;
        autoIcon.color = new Color(0.92f, 0.92f, 0.86f);
        TMP_Text speedIcon = CreateText(parent, "Reference Speed Icon Text", "x2", new Vector2(490, 326), new Vector2(48, 28), TextAlignmentOptions.Center);
        speedIcon.fontSize = 18;
        speedIcon.fontStyle = FontStyles.Bold;
        TMP_Text pauseIcon = CreateText(parent, "Reference Pause Icon Text", "II", new Vector2(590, 326), new Vector2(48, 28), TextAlignmentOptions.Center);
        pauseIcon.fontSize = 24;
        pauseIcon.fontStyle = FontStyles.Bold;
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
            float y = 174 - i * 112;
            Image card = CreatePanel(parent, $"Progress Skill Card {i + 1}", new Vector2(530, y), new Vector2(245, 84), new Color(0.010f, 0.010f, 0.016f, 0.52f));
            string iconPath = i == 0 ? "Assets/Art/Generated/skill_revenge_icon.png" : i == 1 ? "Assets/Art/Generated/skill_shield_icon.png" : "Assets/Art/Generated/skill_holy_icon.png";
            Image iconGlow = CreatePanel(parent, $"Progress Skill Icon Glow {i + 1}", new Vector2(438, y), new Vector2(82, 82), new Color(1.0f, 0.82f, 0.36f, 0.13f));
            Image icon = CreateSpritePanel(parent, $"Progress Skill Icon {i + 1}", iconPath, new Vector2(438, y), new Vector2(72, 72));
            TMP_Text title = CreateText(parent, $"Progress Skill Title {i + 1}", names[i], new Vector2(548, y + 17), new Vector2(138, 28), TextAlignmentOptions.Left);
            title.fontSize = 16;
            title.fontStyle = FontStyles.Bold;
            title.color = new Color(0.92f, 0.86f, 0.70f);
            TMP_Text body = CreateText(parent, $"Progress Skill Body {i + 1}", desc[i], new Vector2(548, y - 16), new Vector2(138, 38), TextAlignmentOptions.Left);
            body.fontSize = 10;
            body.color = new Color(0.76f, 0.74f, 0.68f);
            card.raycastTarget = false;
            icon.raycastTarget = false;
            iconGlow.raycastTarget = false;
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
            "Assets/Art/ReferenceSprites/reference_cleric_full.png",
            "Assets/Art/ReferenceSprites/reference_archmage_full.png",
            "Assets/Art/ReferenceSprites/reference_bard_full.png",
            "Assets/Art/ReferenceSprites/reference_ranger_full.png"
        };

        for (int i = 0; i < portraitPaths.Length; i++)
        {
            float x = 178 + i * 78;
            CreatePanel(parent, $"Progress Bottom Portrait Frame {i + 1}", new Vector2(x, -312), new Vector2(64, 64), new Color(0.72f, 0.68f, 0.60f, 0.42f));
            Image portrait = CreateSpritePanel(parent, $"Progress Bottom Portrait Sprite {i + 1}", portraitPaths[i], new Vector2(x, -312), new Vector2(58, 58));
            CreatePanel(parent, $"Progress Bottom Portrait Mp {i + 1}", new Vector2(x, -350), new Vector2(60, 4), new Color(0.42f, 0.68f, 1.0f, 0.90f));
            portrait.raycastTarget = false;
        }

        Image battleStartPanel = CreatePanel(parent, "Progress Battle Start Panel", new Vector2(520, -230), new Vector2(210, 48), new Color(0.88f, 0.84f, 0.76f, 0.94f));
        TMP_Text battleStartText = CreateText(parent, "Progress Battle Start Text", "BATTLE START", new Vector2(520, -230), new Vector2(194, 38), TextAlignmentOptions.Center);
        battleStartText.fontSize = 20;
        battleStartText.fontStyle = FontStyles.Bold;
        battleStartText.color = new Color(0.20f, 0.17f, 0.13f);
        battleStartPanel.raycastTarget = false;

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
        string[] names = { "PALADIN", "CLERIC", "ARCHMAGE", "BARD", "RANGER" };
        int[] hp = { 253, 139, 253, 252, 254 };
        int[] mp = { 130, 94, 180, 170, 150 };
        string[] sprites =
        {
            "Assets/Art/ReferenceSprites/reference_paladin_full.png",
            "Assets/Art/ReferenceSprites/reference_cleric_full.png",
            "Assets/Art/ReferenceSprites/reference_archmage_full.png",
            "Assets/Art/ReferenceSprites/reference_bard_full.png",
            "Assets/Art/ReferenceSprites/reference_ranger_full.png"
        };

        for (int i = 0; i < names.Length; i++)
        {
            float y = 160 - i * 76;
            Color slotColor = i == 0 ? new Color(0.12f, 0.10f, 0.055f, 0.70f) : new Color(0.010f, 0.012f, 0.020f, 0.48f);
            Image slot = CreatePanel(parent, $"Party Roster Slot {i + 1}", new Vector2(-508, y), new Vector2(292, 70), slotColor);
            slot.raycastTarget = false;
            CreatePanel(parent, $"Party Roster Portrait Chip {i + 1}", new Vector2(-610, y), new Vector2(70, 64), new Color(0.02f, 0.024f, 0.032f, 0.78f));
            if (i == 0) CreatePanel(parent, "Party Roster Selected Gold Rim", new Vector2(-508, y + 35), new Vector2(286, 3), new Color(1.0f, 0.78f, 0.38f, 0.72f));
            Image miniSprite = CreateSpritePanel(parent, $"Party Roster Mini Sprite {i + 1}", sprites[i], new Vector2(-610, y), new Vector2(66, 60));
            miniSprite.raycastTarget = false;
            TMP_Text level = CreateText(parent, $"Party Roster Level {i + 1}", (i == 0 ? 13 : 114 + i * 47).ToString(), new Vector2(-644, y - 24), new Vector2(34, 20), TextAlignmentOptions.Center);
            level.fontSize = 14;
            level.fontStyle = FontStyles.Bold;
            TMP_Text label = CreateText(parent, $"Party Roster Label {i + 1}", names[i], new Vector2(-512, y + 18), new Vector2(126, 24), TextAlignmentOptions.Left);
            label.fontSize = 18;
            label.fontStyle = FontStyles.Bold;
            label.color = i == 0 ? new Color(0.96f, 0.88f, 0.64f) : new Color(0.86f, 0.84f, 0.76f);
            TMP_Text stat = CreateText(parent, $"Party Roster Stat {i + 1}", $"HP {hp[i]}/190\nMP {mp[i]}", new Vector2(-498, y - 12), new Vector2(88, 36), TextAlignmentOptions.Left);
            stat.fontSize = 10;
            stat.color = new Color(0.82f, 0.82f, 0.76f);
            CreatePanel(parent, $"Party Roster HP Bar {i + 1}", new Vector2(-420, y - 4), new Vector2(72, 5), new Color(0.36f, 0.86f, 0.38f, 0.90f));
            CreatePanel(parent, $"Party Roster MP Bar {i + 1}", new Vector2(-420, y - 22), new Vector2(82, 5), new Color(0.38f, 0.62f, 1.0f, 0.90f));
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
            Image chip = CreatePanel(parent, $"Enemy Roster Portrait Chip {i + 1}", new Vector2(485, y), new Vector2(38, 38), new Color(0.08f, 0.04f, 0.10f, 0.88f));
            string spritePath = i == 0
                ? "Assets/Art/ReferenceSprites/reference_goblin_full.png"
                : i == 1
                    ? "Assets/Art/ReferenceSprites/reference_skeleton_full.png"
                    : "Assets/Art/ReferenceSprites/reference_dark_knight_full.png";
            Image miniSprite = CreateSpritePanel(parent, $"Enemy Roster Mini Sprite {i + 1}", spritePath, new Vector2(485, y + 1), new Vector2(32, 38));
            miniSprite.raycastTarget = false;
            string enemyLabel = i == 0 ? "Goblin 80" : i == 1 ? "Skeleton" : "Dark Knight";
            TMP_Text label = CreateText(parent, $"Enemy Roster Label {i + 1}", enemyLabel, new Vector2(562, y), new Vector2(112, 20), TextAlignmentOptions.Right);
            label.fontSize = 12;
            label.color = i == 2 ? new Color(1.0f, 0.78f, 0.78f) : new Color(0.82f, 0.86f, 0.94f);
            slot.gameObject.SetActive(false);
            chip.gameObject.SetActive(false);
            miniSprite.gameObject.SetActive(false);
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
        label.enableWordWrapping = false;
        label.overflowMode = TextOverflowModes.Ellipsis;
        label.raycastTarget = false;

        return button;
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
