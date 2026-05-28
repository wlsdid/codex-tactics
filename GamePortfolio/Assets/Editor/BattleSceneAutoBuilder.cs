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
        Image battleStageBackdropPanel = CreatePanel(canvas.transform, "Battle Stage Backdrop Panel", new Vector2(0, 12), new Vector2(900, 520), new Color(0.018f, 0.030f, 0.026f, 0.88f));
        Image battleStageFloorPanel = CreatePanel(canvas.transform, "Battle Stage Floor Panel", new Vector2(0, -112), new Vector2(720, 170), new Color(0.075f, 0.115f, 0.090f, 0.52f));
        Image topGoldDividerPanel = CreatePanel(canvas.transform, "Top Gold Divider Panel", new Vector2(0, 274), new Vector2(1220, 3), new Color(0.95f, 0.72f, 0.34f, 0.68f));
        Image commandGoldDividerPanel = CreatePanel(canvas.transform, "Command Gold Divider Panel", new Vector2(330, -232), new Vector2(560, 3), new Color(0.95f, 0.72f, 0.34f, 0.72f));
        battleStageBackdropPanel.raycastTarget = false;
        battleStageFloorPanel.raycastTarget = false;
        topGoldDividerPanel.raycastTarget = false;
        commandGoldDividerPanel.raycastTarget = false;
        CreateTacticalGrid(canvas.transform);
        CreateFieldVignette(canvas.transform);
        CreateBattlefieldUnitStandees(canvas.transform);

        // Premium dark panels — slim overlay style, leaving the battlefield visible.
        Image topStatusPanel = CreatePanel(canvas.transform, "Top Status Panel", new Vector2(0, 320), new Vector2(1220, 72), new Color(0.018f, 0.025f, 0.040f, 0.92f));
        Image playerCardPanel = CreatePanel(canvas.transform, "Player Card Panel", new Vector2(-530, 22), new Vector2(250, 560), new Color(0.026f, 0.030f, 0.048f, 0.92f));
        Image enemyCardPanel = CreatePanel(canvas.transform, "Enemy Card Panel", new Vector2(548, 25), new Vector2(185, 455), new Color(0.040f, 0.025f, 0.045f, 0.90f));
        Image battleCenterPanel = CreatePanel(canvas.transform, "Battle Center Panel", new Vector2(0, 236), new Vector2(690, 74), new Color(0.020f, 0.028f, 0.045f, 0.76f));
        Image commandBarPanel = CreatePanel(canvas.transform, "Command Bar Panel", new Vector2(362, -303), new Vector2(520, 98), new Color(0.032f, 0.032f, 0.045f, 0.94f));
        Image partyRosterPanel = CreatePanel(canvas.transform, "Party Roster Panel", new Vector2(-530, -28), new Vector2(224, 350), new Color(0.014f, 0.018f, 0.030f, 0.72f));
        partyRosterPanel.raycastTarget = false;
        CreatePartyRosterSlots(canvas.transform);
        CreateEnemyRosterSlots(canvas.transform);
        topStatusPanel.raycastTarget = false;
        playerCardPanel.raycastTarget = false;
        enemyCardPanel.raycastTarget = false;
        battleCenterPanel.raycastTarget = false;
        commandBarPanel.raycastTarget = false;

        TMP_Text titleText = CreateText(canvas.transform, "Title Text", "* Codex Tactics  x34", new Vector2(-470, 326), new Vector2(330, 42), TextAlignmentOptions.Left);
        titleText.fontSize = 21;
        titleText.fontStyle = FontStyles.Bold;

        TMP_Text runStatusText = CreateText(canvas.transform, "Run Status Text", "Run Status: Stage 1 In Progress", new Vector2(140, 332), new Vector2(620, 28), TextAlignmentOptions.Left);
        runStatusText.fontSize = 15;
        runStatusText.color = new Color(0.76f, 1.0f, 0.82f);

        TMP_Text battleGuideText = CreateText(canvas.transform, "Battle Guide Text", "Mission: read enemy intent, spend AP, guard heavy attacks, then chain skills for clear.", new Vector2(140, 304), new Vector2(720, 28), TextAlignmentOptions.Left);
        battleGuideText.fontSize = 15;
        battleGuideText.color = new Color(0.90f, 0.95f, 1.0f);

        TMP_Text stageText = CreateText(canvas.transform, "Stage Text", "Stage 1-1  /  Slime Scout", new Vector2(-268, 236), new Vector2(300, 36), TextAlignmentOptions.Left);
        stageText.fontSize = 20;
        stageText.color = new Color(0.92f, 0.86f, 0.55f);
        TMP_Text stageObjectiveText = CreateText(canvas.transform, "Stage Objective Text", "Objective: Defeat Slime Scout", new Vector2(38, 236), new Vector2(390, 26), TextAlignmentOptions.Left);
        stageObjectiveText.fontSize = 15;
        stageObjectiveText.color = new Color(1.0f, 0.94f, 0.72f);
        TMP_Text stageProgressText = CreateText(canvas.transform, "Stage Progress Text", "Progress: Encounter 1/2 | Active", new Vector2(270, 214), new Vector2(330, 24), TextAlignmentOptions.Right);
        stageProgressText.fontSize = 14;
        stageProgressText.color = new Color(0.72f, 0.90f, 1.0f);

        TMP_Text playerHpText = CreateText(canvas.transform, "Player HP Text", "Hero HP: 100/100 (100%)", new Vector2(-486, 178), new Vector2(170, 26), TextAlignmentOptions.Left);
        playerHpText.fontSize = 15;
        TMP_Text playerCardTitleText = CreateText(canvas.transform, "Player Card Title Text", "ALLY UNIT  /  HERO", new Vector2(-530, 250), new Vector2(210, 24), TextAlignmentOptions.Center);
        playerCardTitleText.fontSize = 16;
        playerCardTitleText.fontStyle = FontStyles.Bold;
        playerCardTitleText.color = new Color(0.92f, 0.86f, 0.55f);
        // Portrait border frames — subtle dark outline
        CreatePortraitFrame(canvas.transform, "Player Portrait Frame", new Vector2(-592, 200), new Vector2(72, 72));
        CreatePortraitPixelAccent(canvas.transform, "Player", new Vector2(-592, 200), new Color(0.38f, 0.78f, 1.0f, 0.88f));
        Image playerSpriteImage = CreatePortrait(canvas.transform, "Player Sprite", new Vector2(-592, 200), new Vector2(58, 58));
        Slider playerHpSlider = CreateHpSlider(canvas.transform, "Player HP Slider", new Vector2(-486, 157), new Vector2(170, 14), new Color(0.22f, 0.72f, 0.38f));
        TMP_Text playerApText = CreateText(canvas.transform, "Player AP Text", "AP: 3/3 (100%)", new Vector2(-486, 136), new Vector2(170, 24), TextAlignmentOptions.Left);
        playerApText.fontSize = 15;
        Slider playerApSlider = CreateHpSlider(canvas.transform, "Player AP Slider", new Vector2(-486, 118), new Vector2(170, 12), new Color(0.26f, 0.56f, 1.0f));
        TMP_Text playerStatusText = CreateText(canvas.transform, "Player Status Text", "Status: Ready", new Vector2(-486, 96), new Vector2(170, 24), TextAlignmentOptions.Left);
        playerStatusText.fontSize = 15;
        playerStatusText.color = new Color(0.78f, 1.0f, 0.76f);
        TMP_Text playerShieldText = CreateText(canvas.transform, "Player Shield Text", "", new Vector2(-486, 76), new Vector2(170, 24), TextAlignmentOptions.Left);
        playerShieldText.fontSize = 14;
        playerShieldText.color = new Color(0.45f, 0.78f, 1.0f);
        TMP_Text enemyHpText = CreateText(canvas.transform, "Enemy HP Text", "Slime HP: 80/80 (100%)", new Vector2(545, 175), new Vector2(150, 26), TextAlignmentOptions.Right);
        enemyHpText.fontSize = 14;
        TMP_Text enemyCardTitleText = CreateText(canvas.transform, "Enemy Card Title Text", "ENEMY", new Vector2(548, 235), new Vector2(150, 24), TextAlignmentOptions.Center);
        enemyCardTitleText.fontSize = 15;
        enemyCardTitleText.fontStyle = FontStyles.Bold;
        enemyCardTitleText.color = new Color(1.0f, 0.64f, 0.48f);
        TMP_Text versusDividerText = CreateText(canvas.transform, "Versus Divider Text", "BATTLE LINE", new Vector2(0, 150), new Vector2(220, 34), TextAlignmentOptions.Center);
        versusDividerText.fontSize = 18;
        versusDividerText.fontStyle = FontStyles.Bold;
        versusDividerText.color = new Color(0.96f, 0.78f, 0.36f);
        // Portrait border frames — subtle dark outline
        CreatePortraitFrame(canvas.transform, "Enemy Portrait Frame", new Vector2(505, 198), new Vector2(70, 70));
        CreatePortraitPixelAccent(canvas.transform, "Enemy", new Vector2(505, 198), new Color(1.0f, 0.45f, 0.24f, 0.88f));
        Image enemySpriteImage = CreatePortrait(canvas.transform, "Enemy Sprite", new Vector2(505, 198), new Vector2(56, 56));
        Image burnOverlay = CreateStatusOverlay(canvas.transform, "Burn Overlay", new Vector2(505, 198), new Vector2(56, 56));
        Image stunOverlay = CreateStatusOverlay(canvas.transform, "Stun Overlay", new Vector2(505, 198), new Vector2(56, 56));
        Image brokenOverlay = CreateStatusOverlay(canvas.transform, "Broken Overlay", new Vector2(505, 198), new Vector2(56, 56));
        burnOverlay.gameObject.SetActive(false);
        stunOverlay.gameObject.SetActive(false);
        brokenOverlay.gameObject.SetActive(false);
        Slider enemyHpSlider = CreateHpSlider(canvas.transform, "Enemy HP Slider", new Vector2(545, 154), new Vector2(150, 14), new Color(0.82f, 0.22f, 0.24f));
        TMP_Text enemyStatusText = CreateText(canvas.transform, "Enemy Status Text", "Status: None", new Vector2(545, 132), new Vector2(150, 24), TextAlignmentOptions.Right);
        enemyStatusText.fontSize = 14;
        TMP_Text enemyIntentText = CreateText(canvas.transform, "Enemy Intent Text", "Next: Normal Attack (15)", new Vector2(545, 108), new Vector2(150, 36), TextAlignmentOptions.Right);
        enemyIntentText.fontSize = 14;
        enemyIntentText.color = new Color(1.0f, 0.78f, 0.42f);
        TMP_Text enemyBreakText = CreateText(canvas.transform, "Enemy Break Text", "Break: 2/2", new Vector2(545, 80), new Vector2(150, 24), TextAlignmentOptions.Right);
        enemyBreakText.fontSize = 14;
        enemyBreakText.color = new Color(1.0f, 0.58f, 0.82f);
        Slider enemyBreakSlider = CreateHpSlider(canvas.transform, "Enemy Break Slider", new Vector2(545, 62), new Vector2(150, 12), new Color(0.92f, 0.36f, 0.72f));
        TMP_Text messageText = CreateText(canvas.transform, "Message Text", "Battle Start!", new Vector2(80, 258), new Vector2(390, 28), TextAlignmentOptions.Center);
        messageText.fontSize = 16;
        TMP_Text impactText = CreateText(canvas.transform, "Impact Text", "Impact: Ready", new Vector2(0, 218), new Vector2(420, 28), TextAlignmentOptions.Center);
        impactText.fontSize = 16;
        impactText.color = new Color(1.0f, 0.84f, 0.36f);
        TMP_Text skillHelpText = CreateText(canvas.transform, "Skill Help Text", "Skill Help", new Vector2(-220, -308), new Vector2(320, 52), TextAlignmentOptions.TopLeft);
        skillHelpText.fontSize = 9;
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

        Button attackButton = CreateButton(canvas.transform, "Attack Button", "Attack", new Vector2(130, 82), new Vector2(125, 48));
        Button fireSkillButton = CreateButton(canvas.transform, "Fire Skill Button", "Fire", new Vector2(265, 82), new Vector2(125, 48));
        Button iceSkillButton = CreateButton(canvas.transform, "Ice Lance Button", "Ice", new Vector2(400, 82), new Vector2(125, 48));
        Button lightningSkillButton = CreateButton(canvas.transform, "Lightning Strike Button", "Lightning", new Vector2(130, 28), new Vector2(125, 42));
        Button earthSkillButton = CreateButton(canvas.transform, "Earth Wall Button", "Wall", new Vector2(265, 28), new Vector2(125, 42));
        Button guardButton = CreateButton(canvas.transform, "Guard Button", "Guard", new Vector2(400, 28), new Vector2(125, 42));
        Button endTurnButton = CreateButton(canvas.transform, "End Turn Button", "BATTLE START >>", new Vector2(350, 148), new Vector2(220, 52));
        Button retryButton = CreateButton(canvas.transform, "Retry Button", "Retry", new Vector2(170, 145), new Vector2(140, 48));
        retryButton.gameObject.SetActive(false);
        Button continueButton = CreateButton(canvas.transform, "Continue Button", "Continue", new Vector2(320, 145), new Vector2(150, 48));
        continueButton.gameObject.SetActive(false);
        // Create the label child that shows "Continue" by default, will be changed to "Next Encounter" at runtime
        TMP_Text continueButtonLabel = continueButton.GetComponentInChildren<TMP_Text>();
        Button stageSelectButton = CreateButton(canvas.transform, "Stage Select Button", "Stage Select", new Vector2(-505, 28), new Vector2(120, 40));
        Button speedToggleButton = CreateButton(canvas.transform, "Speed Toggle Button", "1x", new Vector2(520, 672), new Vector2(52, 30));
        Button autoBattleButton = CreateButton(canvas.transform, "Auto Battle Button", "Auto", new Vector2(458, 672), new Vector2(58, 30));
        Button itemButton = CreateButton(canvas.transform, "Item Button", "Items", new Vector2(555, 24), new Vector2(100, 40));
        Button pauseButton = CreateButton(canvas.transform, "Pause Button", "II", new Vector2(582, 672), new Vector2(52, 30));
        Button battleLogToggleButton = CreateButton(canvas.transform, "Battle Log Toggle Button", "Log", new Vector2(555, 72), new Vector2(100, 38));
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
        SetObjectReference(serializedBattleUI, "enemyHpText", enemyHpText);
        SetObjectReference(serializedBattleUI, "enemyHpSlider", enemyHpSlider);
        SetObjectReference(serializedBattleUI, "enemyStatusText", enemyStatusText);
        SetObjectReference(serializedBattleUI, "enemyIntentText", enemyIntentText);
        SetObjectReference(serializedBattleUI, "enemyBreakText", enemyBreakText);
        SetObjectReference(serializedBattleUI, "enemyBreakSlider", enemyBreakSlider);
        SetObjectReference(serializedBattleUI, "enemySpriteImage", enemySpriteImage);
        SetObjectReference(serializedBattleUI, "burnOverlay", burnOverlay);
        SetObjectReference(serializedBattleUI, "stunOverlay", stunOverlay);
        SetObjectReference(serializedBattleUI, "brokenOverlay", brokenOverlay);
        SetObjectReference(serializedBattleUI, "runStatusText", runStatusText);
        SetObjectReference(serializedBattleUI, "stageText", stageText);
        SetObjectReference(serializedBattleUI, "stageObjectiveText", stageObjectiveText);
        SetObjectReference(serializedBattleUI, "stageProgressText", stageProgressText);
        SetObjectReference(serializedBattleUI, "messageText", messageText);
        SetObjectReference(serializedBattleUI, "impactText", impactText);
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
            "Assets/Scenes/BattleScene.unity created!\n\nPress Play to test Attack / Fire Skill / Ice Lance / Lightning Strike / Earth Wall / Guard / End Turn / Continue / Retry.",
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

        Button attackButton = FindButton("Attack Button");
        Button fireSkillButton = FindButton("Fire Skill Button");
        Button iceSkillButton = FindButton("Ice Lance Button");
        Button lightningSkillButton = FindButton("Lightning Strike Button");
        Button earthSkillButton = FindButton("Earth Wall Button");
        Button guardButton = FindButton("Guard Button");
        Button endTurnButton = FindButton("End Turn Button");
        Button retryButton = FindButtonIncludingInactive("Retry Button");
        Button continueButton = FindButtonIncludingInactive("Continue Button");
        Button stageSelectButton = FindButtonIncludingInactive("Stage Select Button");
        Button speedToggleButton = FindButtonIncludingInactive("Speed Toggle Button");
        Button autoBattleButton = FindButtonIncludingInactive("Auto Battle Button");
        Button itemButton = FindButtonIncludingInactive("Item Button");
        Button battleLogToggleButton = FindButtonIncludingInactive("Battle Log Toggle Button");
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
        Image enemyRosterSlot1 = FindImage("Enemy Roster Slot 1");
        Image partyRosterMiniSprite1 = FindImage("Party Roster Mini Sprite 1");
        Image enemyRosterMiniSprite1 = FindImage("Enemy Roster Mini Sprite 1");
        Image tacticalGridTile = FindImage("Tactical Grid Tile 1-1");
        Image skillActionArc = FindImage("Skill Action Arc");
        Image heroStandeeBody = FindImage("Hero Standee Body");
        Image heroStandeeBlade = FindImage("Hero Standee Blade");
        Image enemyStandeeBody = FindImage("Enemy Standee Body");
        Image enemyStandeeCrown = FindImage("Enemy Standee Crown");
        Image commandHeaderPanel = FindImage("Command Header Panel");
        TMP_Text commandHeaderText = FindText("Command Header Text");
        Image skillTierBadge = FindImage("Skill Tier Badge");
        TMP_Text resultSummaryText = FindTextIncludingInactive("Result Summary Text");
        Image resultSummaryPanel = FindImageIncludingInactive("Result Summary Panel");
        Image commandPreviewPanel = FindImageIncludingInactive("Command Preview Panel");
        TMP_Text commandPreviewText = FindTextIncludingInactive("Command Preview Text");
        Image turnBannerPanel = FindImageIncludingInactive("Turn Banner Panel");
        TMP_Text turnBannerText = FindTextIncludingInactive("Turn Banner Text");
        TMP_Text impactText = FindText("Impact Text");

        AppendCheck(ref passed, ref report, "Battle stage backdrop exists", battleStageBackdropPanel != null);
        AppendCheck(ref passed, ref report, "Battle stage backdrop has premium dark RPG styling", IsDecorativePanelLikelyConfigured(battleStageBackdropPanel, 850f, 500f));
        AppendCheck(ref passed, ref report, "Battle stage floor glow exists", battleStageFloorPanel != null);
        AppendCheck(ref passed, ref report, "Battle stage floor glow is readable", IsDecorativePanelLikelyConfigured(battleStageFloorPanel, 680f, 150f));
        AppendCheck(ref passed, ref report, "Top gold divider exists", topGoldDividerPanel != null && IsDecorativePanelLikelyConfigured(topGoldDividerPanel, 1000f, 3f));
        AppendCheck(ref passed, ref report, "Command gold divider exists", commandGoldDividerPanel != null && IsDecorativePanelLikelyConfigured(commandGoldDividerPanel, 520f, 3f));
        AppendCheck(ref passed, ref report, "Tactical grid tile exists", IsDecorativePanelLikelyConfigured(tacticalGridTile, 80f, 35f));
        AppendCheck(ref passed, ref report, "Skill action arc exists", IsDecorativePanelLikelyConfigured(skillActionArc, 450f, 4f));
        AppendCheck(ref passed, ref report, "Hero chibi pixel standee exists", IsSpriteImageLikelyConfigured(heroStandeeBody, 200f, 270f) && IsDecorativePanelLikelyConfigured(heroStandeeBlade, 6f, 62f));
        AppendCheck(ref passed, ref report, "Enemy chibi pixel standee exists", IsSpriteImageLikelyConfigured(enemyStandeeBody, 230f, 280f) && IsDecorativePanelLikelyConfigured(enemyStandeeCrown, 58f, 8f));
        AppendCheck(ref passed, ref report, "Premium command header exists", IsDecorativePanelLikelyConfigured(commandHeaderPanel, 240f, 24f) && IsNameplateTextLikelyConfigured(commandHeaderText, "COMMAND", "CHAIN"));
        AppendCheck(ref passed, ref report, "Skill tier badge exists", IsDecorativePanelLikelyConfigured(skillTierBadge, 56f, 20f));
        AppendCheck(ref passed, ref report, "Party roster panel exists", partyRosterPanel != null && IsDecorativePanelLikelyConfigured(partyRosterPanel, 210f, 330f));
        AppendCheck(ref passed, ref report, "Party roster slots exist", IsDecorativePanelLikelyConfigured(partyRosterSlot1, 200f, 50f));
        AppendCheck(ref passed, ref report, "Enemy roster slots exist", IsDecorativePanelLikelyConfigured(enemyRosterSlot1, 150f, 50f));
        AppendCheck(ref passed, ref report, "Party roster pixel mini sprites exist", IsSpriteImageLikelyConfigured(partyRosterMiniSprite1, 36f, 42f));
        AppendCheck(ref passed, ref report, "Enemy roster pixel mini sprites exist", IsSpriteImageLikelyConfigured(enemyRosterMiniSprite1, 34f, 38f));
        AppendCheck(ref passed, ref report, "Player card title exists", IsNameplateTextLikelyConfigured(playerCardTitleText, "ALLY", "HERO"));
        AppendCheck(ref passed, ref report, "Enemy card title exists", IsNameplateTextLikelyConfigured(enemyCardTitleText, "ENEMY", "ENEMY"));
        AppendCheck(ref passed, ref report, "Battle line divider text exists", IsNameplateTextLikelyConfigured(versusDividerText, "BATTLE", "LINE"));
        AppendCheck(ref passed, ref report, "Player portrait pixel accents exist", IsPortraitAccentLikelyConfigured(playerPortraitPixelAccent1) && IsPortraitAccentLikelyConfigured(playerPortraitPixelAccent4));
        AppendCheck(ref passed, ref report, "Enemy portrait pixel accents exist", IsPortraitAccentLikelyConfigured(enemyPortraitPixelAccent1) && IsPortraitAccentLikelyConfigured(enemyPortraitPixelAccent4));
        AppendCheck(ref passed, ref report, "Top Status panel exists", topStatusPanel != null);
        AppendCheck(ref passed, ref report, "Top Status panel has premium dark RPG styling", IsProfessionalPanelLikelyConfigured(topStatusPanel, 1150f, 65f));
        AppendCheck(ref passed, ref report, "Player Card panel exists", playerCardPanel != null);
        AppendCheck(ref passed, ref report, "Player Card panel has premium dark RPG styling", IsProfessionalPanelLikelyConfigured(playerCardPanel, 240f, 540f));
        AppendCheck(ref passed, ref report, "Enemy Card panel exists", enemyCardPanel != null);
        AppendCheck(ref passed, ref report, "Enemy Card panel has premium dark RPG styling", IsProfessionalPanelLikelyConfigured(enemyCardPanel, 175f, 430f));
        AppendCheck(ref passed, ref report, "Battle Center panel exists", battleCenterPanel != null);
        AppendCheck(ref passed, ref report, "Battle Center panel has premium dark RPG styling", IsProfessionalPanelLikelyConfigured(battleCenterPanel, 650f, 70f));
        AppendCheck(ref passed, ref report, "Command Bar panel exists", commandBarPanel != null);
        AppendCheck(ref passed, ref report, "Command Bar panel has premium dark RPG styling", IsProfessionalPanelLikelyConfigured(commandBarPanel, 500f, 90f));
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
        AppendCheck(ref passed, ref report, "Skill Help text exists", skillHelpText != null);
        AppendCheck(ref passed, ref report, "Runtime labels skip raycast for UI performance", IsTextRaycastOptimized(runStatusText, battleGuideText, stageText, stageObjectiveText, stageProgressText, playerHpText, playerApText, enemyHpText, skillHelpText, messageText, impactText));
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
        AppendCheck(ref passed, ref report, "Attack button is visible", IsButtonLikelyVisible(attackButton));
        AppendCheck(ref passed, ref report, "Fire Skill button is visible", IsButtonLikelyVisible(fireSkillButton));
        AppendCheck(ref passed, ref report, "Guard button is visible", IsButtonLikelyVisible(guardButton));
        AppendCheck(ref passed, ref report, "End Turn button is visible", IsButtonLikelyVisible(endTurnButton));
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
            && color.a >= 0.75f
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

    private static bool IsBattleGuideTextLikelyConfigured(TMP_Text guideText)
    {
        if (guideText == null)
        {
            return false;
        }

        RectTransform rectTransform = guideText.GetComponent<RectTransform>();
        string text = guideText.text;
        return rectTransform != null
            && rectTransform.sizeDelta.x >= 700f
            && text.Contains("Mission")
            && text.Contains("enemy intent")
            && text.Contains("AP")
            && text.Contains("guard")
            && text.Contains("skills");
    }

    private static bool IsStageTextLikelyConfigured(TMP_Text stageText)
    {
        if (stageText == null)
        {
            return false;
        }

        RectTransform rectTransform = stageText.GetComponent<RectTransform>();
        return rectTransform != null
            && rectTransform.sizeDelta.x >= 280f
            && stageText.text.Contains("Stage 1-1")
            && stageText.text.Contains("Slime Scout");
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
            && text.Contains("Run Status")
            && text.Contains("Stage 1 In Progress");
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
            && text.Contains("Objective")
            && text.Contains("Defeat")
            && text.Contains("Slime Scout");
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
            && text.Contains("Progress")
            && text.Contains("Encounter 1/2")
            && text.Contains("Active");
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

    private static Image CreatePortrait(Transform parent, string name, Vector2 anchoredPosition, Vector2 size)
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
        image.color = new Color(0.10f, 0.11f, 0.16f, 0.7f);
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
                Vector2 pos = new Vector2(-260 + col * 92, -118 + row * 46);
                Image tile = CreatePanel(parent, $"Tactical Grid Tile {row + 1}-{col + 1}", pos, new Vector2(84, 38), new Color(0.06f, 0.13f, 0.09f, 0.38f));
                tile.raycastTarget = false;
            }
        }

        Image allyMarker = CreatePanel(parent, "Ally Formation Marker", new Vector2(-286, -42), new Vector2(34, 72), new Color(0.28f, 1.0f, 0.48f, 0.38f));
        Image enemyMarker = CreatePanel(parent, "Enemy Formation Marker", new Vector2(238, 52), new Vector2(34, 72), new Color(1.0f, 0.12f, 0.65f, 0.42f));
        Image actionArc = CreatePanel(parent, "Skill Action Arc", new Vector2(-6, 112), new Vector2(470, 5), new Color(1.0f, 0.78f, 0.50f, 0.58f));
        allyMarker.raycastTarget = false;
        enemyMarker.raycastTarget = false;
        actionArc.raycastTarget = false;
        actionArc.rectTransform.localRotation = Quaternion.Euler(0, 0, -12f);
        _ = lineColor;
    }

    private static void CreateFieldVignette(Transform parent)
    {
        CreatePanel(parent, "Forest Shadow Left", new Vector2(-300, -10), new Vector2(170, 470), new Color(0.010f, 0.030f, 0.020f, 0.34f));
        CreatePanel(parent, "Forest Shadow Right", new Vector2(310, -20), new Vector2(190, 440), new Color(0.010f, 0.025f, 0.020f, 0.38f));
        CreatePanel(parent, "Stage Glow Firefly 1", new Vector2(-210, 150), new Vector2(8, 8), new Color(0.72f, 1.0f, 0.42f, 0.70f));
        CreatePanel(parent, "Stage Glow Firefly 2", new Vector2(188, 118), new Vector2(7, 7), new Color(0.72f, 1.0f, 0.42f, 0.65f));
        CreatePanel(parent, "Stage Glow Firefly 3", new Vector2(-18, -184), new Vector2(6, 6), new Color(0.72f, 1.0f, 0.42f, 0.55f));
    }

    private static void CreateBattlefieldUnitStandees(Transform parent)
    {
        // Original chibi pixel standees inspired by the user's reference direction:
        // big head, tiny body, readable silhouette, dark outline, limited fantasy palette.
        // These are generated project assets, not copied character art.
        CreatePanel(parent, "Hero Standee Shadow", new Vector2(-206, -116), new Vector2(112, 18), new Color(0.0f, 0.0f, 0.0f, 0.34f));
        CreatePanel(parent, "Hero Standee Aura", new Vector2(-205, -24), new Vector2(132, 172), new Color(0.25f, 0.60f, 1.0f, 0.13f));
        Image heroBody = CreateSpritePanel(parent, "Hero Standee Body", "Assets/Art/Generated/chibi_hero_original.png", new Vector2(-204, -2), new Vector2(220, 293));
        Image heroBlade = CreatePanel(parent, "Hero Standee Blade", new Vector2(-158, -18), new Vector2(8, 70), new Color(0.88f, 0.94f, 1.0f, 0.42f));
        heroBlade.rectTransform.localRotation = Quaternion.Euler(0, 0, -18f);
        heroBody.raycastTarget = false;
        heroBlade.raycastTarget = false;

        CreatePanel(parent, "Enemy Standee Shadow", new Vector2(230, -110), new Vector2(146, 22), new Color(0.0f, 0.0f, 0.0f, 0.38f));
        CreatePanel(parent, "Enemy Standee Aura", new Vector2(232, -24), new Vector2(166, 184), new Color(0.82f, 0.20f, 1.0f, 0.13f));
        Image enemyBody = CreateSpritePanel(parent, "Enemy Standee Body", "Assets/Art/Generated/chibi_enemy_original.png", new Vector2(232, -2), new Vector2(252, 306));
        Image enemyCrown = CreatePanel(parent, "Enemy Standee Crown", new Vector2(232, 82), new Vector2(78, 10), new Color(1.0f, 0.66f, 0.20f, 0.42f));
        enemyBody.raycastTarget = false;
        enemyCrown.raycastTarget = false;
    }

    private static Image CreatePixelBlock(Transform parent, string name, Vector2 anchoredPosition, Vector2 size, Color color)
    {
        Image block = CreatePanel(parent, name, anchoredPosition, size, color);
        block.raycastTarget = false;
        return block;
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

        Image commandGlowLeft = CreatePanel(parent, "Command Glow Left", new Vector2(106, -303), new Vector2(4, 88), new Color(0.95f, 0.72f, 0.34f, 0.68f));
        Image commandGlowRight = CreatePanel(parent, "Command Glow Right", new Vector2(618, -303), new Vector2(4, 88), new Color(0.95f, 0.72f, 0.34f, 0.68f));

        headerPanel.raycastTarget = false;
        skillTierBadge.raycastTarget = false;
        commandGlowLeft.raycastTarget = false;
        commandGlowRight.raycastTarget = false;
    }

    private static void CreatePartyRosterSlots(Transform parent)
    {
        Color[] slotColors =
        {
            new Color(0.14f, 0.36f, 0.25f, 0.82f),
            new Color(0.16f, 0.18f, 0.25f, 0.78f),
            new Color(0.12f, 0.17f, 0.24f, 0.76f),
            new Color(0.11f, 0.18f, 0.15f, 0.76f),
            new Color(0.16f, 0.30f, 0.20f, 0.84f)
        };

        for (int i = 0; i < slotColors.Length; i++)
        {
            float y = 40 - i * 64;
            Image slot = CreatePanel(parent, $"Party Roster Slot {i + 1}", new Vector2(-530, y), new Vector2(212, 54), slotColors[i]);
            slot.raycastTarget = false;
            CreatePanel(parent, $"Party Roster Portrait Chip {i + 1}", new Vector2(-610, y), new Vector2(44, 44), new Color(0.06f, 0.08f, 0.11f, 0.90f));
            string spritePath = i == 0 ? "Assets/Art/Generated/chibi_hero_original.png" : "Assets/Art/Generated/chibi_ally_guardian.png";
            Image miniSprite = CreateSpritePanel(parent, $"Party Roster Mini Sprite {i + 1}", spritePath, new Vector2(-610, y + 1), new Vector2(42, 50));
            miniSprite.raycastTarget = false;
            TMP_Text label = CreateText(parent, $"Party Roster Label {i + 1}", i == 0 ? "Hero  100" : $"Ally {i + 1}  {100 - i * 8}", new Vector2(-500, y + 4), new Vector2(126, 20), TextAlignmentOptions.Left);
            label.fontSize = 12;
            label.color = i == 0 ? new Color(0.78f, 1.0f, 0.68f) : new Color(0.82f, 0.86f, 0.94f);
        }
    }

    private static void CreateEnemyRosterSlots(Transform parent)
    {
        for (int i = 0; i < 5; i++)
        {
            float y = -4 - i * 58;
            Color color = i == 4 ? new Color(0.72f, 0.05f, 0.12f, 0.86f) : new Color(0.07f, 0.075f, 0.10f, 0.76f);
            Image slot = CreatePanel(parent, $"Enemy Roster Slot {i + 1}", new Vector2(548, y), new Vector2(160, 50), color);
            slot.raycastTarget = false;
            CreatePanel(parent, $"Enemy Roster Portrait Chip {i + 1}", new Vector2(485, y), new Vector2(38, 38), new Color(0.08f, 0.04f, 0.10f, 0.88f));
            string spritePath = i == 4 ? "Assets/Art/Generated/chibi_enemy_original.png" : "Assets/Art/Generated/chibi_enemy_raider.png";
            Image miniSprite = CreateSpritePanel(parent, $"Enemy Roster Mini Sprite {i + 1}", spritePath, new Vector2(485, y + 1), new Vector2(38, 44));
            miniSprite.raycastTarget = false;
            TMP_Text label = CreateText(parent, $"Enemy Roster Label {i + 1}", i == 0 ? "Raider 80" : i == 4 ? "Boss" : $"Enemy {i + 1}", new Vector2(562, y), new Vector2(112, 20), TextAlignmentOptions.Right);
            label.fontSize = 12;
            label.color = i == 4 ? new Color(1.0f, 0.78f, 0.78f) : new Color(0.82f, 0.86f, 0.94f);
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
        image.color = new Color(0.14f, 0.17f, 0.28f);

        Button button = buttonObject.AddComponent<Button>();
        ColorBlock colors = button.colors;
        colors.normalColor = new Color(0.14f, 0.17f, 0.28f);
        colors.highlightedColor = new Color(0.22f, 0.28f, 0.42f);
        colors.pressedColor = new Color(0.08f, 0.10f, 0.16f);
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
