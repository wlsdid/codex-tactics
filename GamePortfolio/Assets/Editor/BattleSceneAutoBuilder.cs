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
        Canvas canvas = CreateCanvas(camera);
        CreateEventSystem();

        Image topStatusPanel = CreatePanel(canvas.transform, "Top Status Panel", new Vector2(0, 205), new Vector2(1160, 150), new Color(0.055f, 0.050f, 0.075f, 0.92f));
        Image playerCardPanel = CreatePanel(canvas.transform, "Player Card Panel", new Vector2(-410, 65), new Vector2(340, 285), new Color(0.060f, 0.055f, 0.085f, 0.90f));
        Image enemyCardPanel = CreatePanel(canvas.transform, "Enemy Card Panel", new Vector2(410, 65), new Vector2(340, 285), new Color(0.075f, 0.050f, 0.070f, 0.90f));
        Image battleCenterPanel = CreatePanel(canvas.transform, "Battle Center Panel", new Vector2(0, -65), new Vector2(520, 245), new Color(0.045f, 0.052f, 0.078f, 0.88f));
        Image commandBarPanel = CreatePanel(canvas.transform, "Command Bar Panel", new Vector2(0, -275), new Vector2(1020, 112), new Color(0.050f, 0.045f, 0.065f, 0.94f));
        topStatusPanel.raycastTarget = false;
        playerCardPanel.raycastTarget = false;
        enemyCardPanel.raycastTarget = false;
        battleCenterPanel.raycastTarget = false;
        commandBarPanel.raycastTarget = false;

        TMP_Text titleText = CreateText(canvas.transform, "Title Text", "Codex Tactics", new Vector2(0, 255), new Vector2(800, 50), TextAlignmentOptions.Center);
        titleText.fontSize = 34;

        TMP_Text runStatusText = CreateText(canvas.transform, "Run Status Text", "Run Status: Stage 1 In Progress", new Vector2(0, 220), new Vector2(800, 34), TextAlignmentOptions.Center);
        runStatusText.fontSize = 20;
        runStatusText.color = new Color(0.76f, 1.0f, 0.82f);

        TMP_Text battleGuideText = CreateText(canvas.transform, "Battle Guide Text", "Battle Guide: Attack to deal damage | Fire Skill applies Burn | Guard before Heavy Slam | Watch Enemy Intent | Continue after Victory | Final Clear completes Stage 1 | Retry current fight", new Vector2(0, 190), new Vector2(1120, 40), TextAlignmentOptions.Center);
        battleGuideText.fontSize = 18;
        battleGuideText.color = new Color(0.90f, 0.95f, 1.0f);

        TMP_Text stageText = CreateText(canvas.transform, "Stage Text", "Stage 1-1: Slime Scout", new Vector2(0, 165), new Vector2(800, 40), TextAlignmentOptions.Center);
        stageText.fontSize = 24;
        stageText.color = new Color(0.92f, 0.86f, 0.55f);
        TMP_Text stageObjectiveText = CreateText(canvas.transform, "Stage Objective Text", "Objective: Defeat Slime Scout", new Vector2(0, 135), new Vector2(900, 32), TextAlignmentOptions.Center);
        stageObjectiveText.fontSize = 18;
        stageObjectiveText.color = new Color(1.0f, 0.94f, 0.72f);
        TMP_Text stageProgressText = CreateText(canvas.transform, "Stage Progress Text", "Progress: Encounter 1/2 | Active", new Vector2(0, 112), new Vector2(900, 28), TextAlignmentOptions.Center);
        stageProgressText.fontSize = 17;
        stageProgressText.color = new Color(0.72f, 0.90f, 1.0f);

        TMP_Text playerHpText = CreateText(canvas.transform, "Player HP Text", "Hero HP: 100/100 (100%)", new Vector2(-360, 95), new Vector2(420, 50), TextAlignmentOptions.Left);
        Image playerSpriteImage = CreatePortrait(canvas.transform, "Player Sprite", new Vector2(-360, 200), new Vector2(100, 100));
        Slider playerHpSlider = CreateHpSlider(canvas.transform, "Player HP Slider", new Vector2(-360, 80), new Vector2(420, 22), new Color(0.22f, 0.72f, 0.38f));
        TMP_Text playerApText = CreateText(canvas.transform, "Player AP Text", "AP: 3/3 (100%)", new Vector2(-360, 55), new Vector2(420, 45), TextAlignmentOptions.Left);
        Slider playerApSlider = CreateHpSlider(canvas.transform, "Player AP Slider", new Vector2(-360, 30), new Vector2(420, 18), new Color(0.26f, 0.56f, 1.0f));
        TMP_Text playerStatusText = CreateText(canvas.transform, "Player Status Text", "Status: Ready", new Vector2(-360, 5), new Vector2(420, 40), TextAlignmentOptions.Left);
        playerStatusText.fontSize = 22;
        playerStatusText.color = new Color(0.78f, 1.0f, 0.76f);
        TMP_Text enemyHpText = CreateText(canvas.transform, "Enemy HP Text", "Slime HP: 80/80 (100%)", new Vector2(360, 110), new Vector2(420, 50), TextAlignmentOptions.Right);
        Image enemySpriteImage = CreatePortrait(canvas.transform, "Enemy Sprite", new Vector2(360, 200), new Vector2(100, 100));
        Slider enemyHpSlider = CreateHpSlider(canvas.transform, "Enemy HP Slider", new Vector2(360, 80), new Vector2(420, 22), new Color(0.82f, 0.22f, 0.24f));
        TMP_Text enemyStatusText = CreateText(canvas.transform, "Enemy Status Text", "Status: None", new Vector2(360, 55), new Vector2(420, 45), TextAlignmentOptions.Right);
        TMP_Text enemyIntentText = CreateText(canvas.transform, "Enemy Intent Text", "Next Enemy: Normal Attack (15)", new Vector2(360, 25), new Vector2(420, 45), TextAlignmentOptions.Right);
        enemyIntentText.fontSize = 22;
        enemyIntentText.color = new Color(1.0f, 0.78f, 0.42f);
        TMP_Text enemyBreakText = CreateText(canvas.transform, "Enemy Break Text", "Break: 2/2", new Vector2(360, 0), new Vector2(420, 40), TextAlignmentOptions.Right);
        enemyBreakText.fontSize = 22;
        enemyBreakText.color = new Color(1.0f, 0.58f, 0.82f);
        Slider enemyBreakSlider = CreateHpSlider(canvas.transform, "Enemy Break Slider", new Vector2(360, -20), new Vector2(420, 18), new Color(0.92f, 0.36f, 0.72f));
        TMP_Text messageText = CreateText(canvas.transform, "Message Text", "Battle Start!", new Vector2(0, -75), new Vector2(900, 100), TextAlignmentOptions.Center);
        TMP_Text impactText = CreateText(canvas.transform, "Impact Text", "Impact: Ready", new Vector2(0, -25), new Vector2(900, 45), TextAlignmentOptions.Center);
        impactText.fontSize = 22;
        impactText.color = new Color(1.0f, 0.84f, 0.36f);
        TMP_Text skillHelpText = CreateText(canvas.transform, "Skill Help Text", "Skill Help", new Vector2(0, 15), new Vector2(900, 95), TextAlignmentOptions.TopLeft);
        skillHelpText.fontSize = 18;
        skillHelpText.color = new Color(0.72f, 0.90f, 1.0f);
        Image resultSummaryPanel = CreatePanel(canvas.transform, "Result Summary Panel", new Vector2(0, -145), new Vector2(940, 130), new Color(0.06f, 0.07f, 0.10f, 0.86f));
        resultSummaryPanel.gameObject.SetActive(false);
        TMP_Text resultSummaryText = CreateText(canvas.transform, "Result Summary Text", "Result Summary", new Vector2(0, -145), new Vector2(900, 105), TextAlignmentOptions.TopLeft);
        resultSummaryText.fontSize = 20;
        resultSummaryText.color = new Color(1.0f, 0.92f, 0.58f);
        resultSummaryText.gameObject.SetActive(false);
        Image battleLogPanel = CreatePanel(canvas.transform, "Battle Log Panel", new Vector2(0, -245), new Vector2(940, 150), new Color(0.05f, 0.06f, 0.09f, 0.78f));
        TMP_Text battleLogTitleText = CreateText(canvas.transform, "Battle Log Title Text", "Recent Actions", new Vector2(0, -185), new Vector2(900, 30), TextAlignmentOptions.Left);
        battleLogTitleText.fontSize = 20;
        battleLogTitleText.color = new Color(0.96f, 0.92f, 0.68f);
        TMP_Text battleLogText = CreateText(canvas.transform, "Battle Log Text", "Recent Actions\nNo actions yet.", new Vector2(0, -255), new Vector2(900, 105), TextAlignmentOptions.TopLeft);
        battleLogText.fontSize = 20;
        battleLogText.color = new Color(0.82f, 0.86f, 0.95f);
        battleLogPanel.raycastTarget = false;

        Button attackButton = CreateButton(canvas.transform, "Attack Button", "Attack", new Vector2(-330, 85), new Vector2(180, 65));
        Button fireSkillButton = CreateButton(canvas.transform, "Fire Skill Button", "Fire Skill", new Vector2(-110, 85), new Vector2(180, 65));
        Button iceSkillButton = CreateButton(canvas.transform, "Ice Lance Button", "Ice Lance", new Vector2(-330, 15), new Vector2(180, 55));
        Button lightningSkillButton = CreateButton(canvas.transform, "Lightning Strike Button", "Lightning Strike", new Vector2(-110, 15), new Vector2(180, 55));
        Button guardButton = CreateButton(canvas.transform, "Guard Button", "Guard", new Vector2(110, 85), new Vector2(180, 65));
        Button endTurnButton = CreateButton(canvas.transform, "End Turn Button", "End Turn", new Vector2(330, 85), new Vector2(180, 65));
        Button retryButton = CreateButton(canvas.transform, "Retry Button", "Retry", new Vector2(-130, -325), new Vector2(220, 70));
        retryButton.gameObject.SetActive(false);
        Button continueButton = CreateButton(canvas.transform, "Continue Button", "Continue", new Vector2(130, -325), new Vector2(220, 70));
        continueButton.gameObject.SetActive(false);
        // Create the label child that shows "Continue" by default, will be changed to "Next Encounter" at runtime
        TMP_Text continueButtonLabel = continueButton.GetComponentInChildren<TMP_Text>();
        Button stageSelectButton = CreateButton(canvas.transform, "Stage Select Button", "Stage Select", new Vector2(-410, -325), new Vector2(220, 70));
        stageSelectButton.gameObject.SetActive(false);

        GameObject battleManagerObject = new GameObject("BattleManager");
        BattleManager battleManager = battleManagerObject.AddComponent<BattleManager>();
        BattleUI battleUI = battleManagerObject.AddComponent<BattleUI>();

        SerializedObject serializedBattleUI = new SerializedObject(battleUI);
        SetObjectReference(serializedBattleUI, "playerHpText", playerHpText);
        SetObjectReference(serializedBattleUI, "playerHpSlider", playerHpSlider);
        SetObjectReference(serializedBattleUI, "playerApText", playerApText);
        SetObjectReference(serializedBattleUI, "playerApSlider", playerApSlider);
        SetObjectReference(serializedBattleUI, "playerStatusText", playerStatusText);
        SetObjectReference(serializedBattleUI, "playerSpriteImage", playerSpriteImage);
        SetObjectReference(serializedBattleUI, "enemyHpText", enemyHpText);
        SetObjectReference(serializedBattleUI, "enemyHpSlider", enemyHpSlider);
        SetObjectReference(serializedBattleUI, "enemyStatusText", enemyStatusText);
        SetObjectReference(serializedBattleUI, "enemyIntentText", enemyIntentText);
        SetObjectReference(serializedBattleUI, "enemyBreakText", enemyBreakText);
        SetObjectReference(serializedBattleUI, "enemyBreakSlider", enemyBreakSlider);
        SetObjectReference(serializedBattleUI, "enemySpriteImage", enemySpriteImage);
        SetObjectReference(serializedBattleUI, "runStatusText", runStatusText);
        SetObjectReference(serializedBattleUI, "stageText", stageText);
        SetObjectReference(serializedBattleUI, "stageObjectiveText", stageObjectiveText);
        SetObjectReference(serializedBattleUI, "stageProgressText", stageProgressText);
        SetObjectReference(serializedBattleUI, "messageText", messageText);
        SetObjectReference(serializedBattleUI, "impactText", impactText);
        SetObjectReference(serializedBattleUI, "skillHelpText", skillHelpText);
        SetObjectReference(serializedBattleUI, "battleLogText", battleLogText);
        SetObjectReference(serializedBattleUI, "resultSummaryText", resultSummaryText);
        SetObjectReference(serializedBattleUI, "resultSummaryPanel", resultSummaryPanel.gameObject);
        SetObjectReference(serializedBattleUI, "attackButton", attackButton);
        SetObjectReference(serializedBattleUI, "fireSkillButton", fireSkillButton);
        SetObjectReference(serializedBattleUI, "iceSkillButton", iceSkillButton);
        SetObjectReference(serializedBattleUI, "lightningSkillButton", lightningSkillButton);
        SetObjectReference(serializedBattleUI, "guardButton", guardButton);
        SetObjectReference(serializedBattleUI, "endTurnButton", endTurnButton);
        SetObjectReference(serializedBattleUI, "retryButton", retryButton);
        SetObjectReference(serializedBattleUI, "continueButton", continueButton);
        SetObjectReference(serializedBattleUI, "stageSelectButton", stageSelectButton);
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
            "Assets/Scenes/BattleScene.unity created!\n\nPress Play to test Attack / Fire Skill / Guard / End Turn / Continue / Retry.",
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
        Button guardButton = FindButton("Guard Button");
        Button endTurnButton = FindButton("End Turn Button");
        Button retryButton = FindButtonIncludingInactive("Retry Button");
        Button continueButton = FindButtonIncludingInactive("Continue Button");
        Button stageSelectButton = FindButtonIncludingInactive("Stage Select Button");
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
        TMP_Text battleLogTitleText = FindText("Battle Log Title Text");
        TMP_Text battleLogText = FindText("Battle Log Text");
        Image battleLogPanel = FindImage("Battle Log Panel");
        Image topStatusPanel = FindImage("Top Status Panel");
        Image playerCardPanel = FindImage("Player Card Panel");
        Image enemyCardPanel = FindImage("Enemy Card Panel");
        Image battleCenterPanel = FindImage("Battle Center Panel");
        Image commandBarPanel = FindImage("Command Bar Panel");
        TMP_Text resultSummaryText = FindTextIncludingInactive("Result Summary Text");
        Image resultSummaryPanel = FindImageIncludingInactive("Result Summary Panel");
        TMP_Text impactText = FindText("Impact Text");

        AppendCheck(ref passed, ref report, "Top Status panel exists", topStatusPanel != null);
        AppendCheck(ref passed, ref report, "Top Status panel has premium dark RPG styling", IsProfessionalPanelLikelyConfigured(topStatusPanel, 1100f, 120f));
        AppendCheck(ref passed, ref report, "Player Card panel exists", playerCardPanel != null);
        AppendCheck(ref passed, ref report, "Player Card panel has premium dark RPG styling", IsProfessionalPanelLikelyConfigured(playerCardPanel, 300f, 250f));
        AppendCheck(ref passed, ref report, "Enemy Card panel exists", enemyCardPanel != null);
        AppendCheck(ref passed, ref report, "Enemy Card panel has premium dark RPG styling", IsProfessionalPanelLikelyConfigured(enemyCardPanel, 300f, 250f));
        AppendCheck(ref passed, ref report, "Battle Center panel exists", battleCenterPanel != null);
        AppendCheck(ref passed, ref report, "Battle Center panel has premium dark RPG styling", IsProfessionalPanelLikelyConfigured(battleCenterPanel, 480f, 230f));
        AppendCheck(ref passed, ref report, "Command Bar panel exists", commandBarPanel != null);
        AppendCheck(ref passed, ref report, "Command Bar panel has premium dark RPG styling", IsProfessionalPanelLikelyConfigured(commandBarPanel, 980f, 110f));
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
        AppendCheck(ref passed, ref report, "Enemy Status text exists", enemyStatusText != null);
        AppendCheck(ref passed, ref report, "Enemy Intent text exists", enemyIntentText != null);
        AppendCheck(ref passed, ref report, "Enemy Break text exists", enemyBreakText != null);
        AppendCheck(ref passed, ref report, "Enemy Break slider exists", enemyBreakSlider != null);
        AppendCheck(ref passed, ref report, "Battle Log title exists", battleLogTitleText != null);
        AppendCheck(ref passed, ref report, "Battle Log text exists", battleLogText != null);
        AppendCheck(ref passed, ref report, "Battle Log panel exists", battleLogPanel != null);
        AppendCheck(ref passed, ref report, "Battle Log panel is readable", IsBattleLogPanelLikelyConfigured(battleLogPanel));
        AppendCheck(ref passed, ref report, "Battle Log starts with recent-actions placeholder", IsBattleLogTextLikelyConfigured(battleLogText));
        AppendCheck(ref passed, ref report, "Result Summary text exists", resultSummaryText != null);
        AppendCheck(ref passed, ref report, "Result Summary panel exists", resultSummaryPanel != null);
        AppendCheck(ref passed, ref report, "Result Summary panel is configured but initially hidden", IsPanelLikelyConfigured(resultSummaryPanel) && resultSummaryPanel != null && !resultSummaryPanel.gameObject.activeSelf);
        AppendCheck(ref passed, ref report, "Attack button exists", attackButton != null);
        AppendCheck(ref passed, ref report, "Fire Skill button exists", fireSkillButton != null);
        AppendCheck(ref passed, ref report, "Ice Lance button exists", iceSkillButton != null);
        AppendCheck(ref passed, ref report, "Lightning Strike button exists", lightningSkillButton != null);
        AppendCheck(ref passed, ref report, "Guard button exists", guardButton != null);
        AppendCheck(ref passed, ref report, "End Turn button exists", endTurnButton != null);
        AppendCheck(ref passed, ref report, "Retry button exists", retryButton != null);
        AppendCheck(ref passed, ref report, "Continue button exists", continueButton != null);
        AppendCheck(ref passed, ref report, "Stage Select button exists", stageSelectButton != null);
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
            AppendCheck(ref passed, ref report, "Battle Log text linked", HasObjectReference(serializedBattleUI, "battleLogText"));
            AppendCheck(ref passed, ref report, "Result Summary text linked", HasObjectReference(serializedBattleUI, "resultSummaryText"));
            AppendCheck(ref passed, ref report, "Result Summary panel linked", HasObjectReference(serializedBattleUI, "resultSummaryPanel"));
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
        return button.gameObject.activeSelf && IsButtonLikelyConfigured(button) && rectTransform.anchoredPosition.y > 0;
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

    private static bool IsBattleLogPanelLikelyConfigured(Image panelImage)
    {
        if (panelImage == null)
        {
            return false;
        }

        RectTransform rectTransform = panelImage.GetComponent<RectTransform>();
        return rectTransform != null && rectTransform.sizeDelta.x >= 900f && rectTransform.sizeDelta.y >= 140f && panelImage.color.a > 0.5f;
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
            && color.a >= 0.70f
            && color.r <= 0.16f
            && color.g <= 0.16f
            && color.b <= 0.22f;
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
            && rectTransform.sizeDelta.x >= 1000f
            && text.Contains("Attack")
            && text.Contains("Fire Skill")
            && text.Contains("Burn")
            && text.Contains("Guard")
            && text.Contains("Enemy Intent")
            && text.Contains("Continue")
            && text.Contains("Final Clear")
            && text.Contains("Retry");
    }

    private static bool IsStageTextLikelyConfigured(TMP_Text stageText)
    {
        if (stageText == null)
        {
            return false;
        }

        RectTransform rectTransform = stageText.GetComponent<RectTransform>();
        return rectTransform != null
            && rectTransform.sizeDelta.x >= 700f
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
            && rectTransform.sizeDelta.x >= 700f
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
            && rectTransform.sizeDelta.x >= 850f
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
            && rectTransform.sizeDelta.x >= 850f
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
            && rectTransform.sizeDelta.x >= 850f
            && rectTransform.sizeDelta.y >= 100f
            && logText.text.Contains("Recent Actions")
            && logText.text.Contains("No actions yet.");
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
        image.color = new Color(0.15f, 0.15f, 0.20f, 0.6f);
        image.raycastTarget = false;
        return image;
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
        backgroundImage.color = new Color(0.10f, 0.11f, 0.14f);

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
        image.color = new Color(0.18f, 0.22f, 0.32f);

        Button button = buttonObject.AddComponent<Button>();
        ColorBlock colors = button.colors;
        colors.normalColor = new Color(0.18f, 0.22f, 0.32f);
        colors.highlightedColor = new Color(0.28f, 0.34f, 0.48f);
        colors.pressedColor = new Color(0.12f, 0.14f, 0.20f);
        colors.disabledColor = new Color(0.08f, 0.08f, 0.10f, 0.5f);
        button.colors = colors;
        button.targetGraphic = image;

        TMP_Text label = CreateText(buttonObject.transform, "Label", labelText, Vector2.zero, size, TextAlignmentOptions.Center);
        label.fontSize = 24;
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
