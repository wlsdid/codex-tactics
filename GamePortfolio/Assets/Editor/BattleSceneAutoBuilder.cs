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

        TMP_Text titleText = CreateText(canvas.transform, "Title Text", "Turn-Based Battle Test", new Vector2(0, 250), new Vector2(800, 60), TextAlignmentOptions.Center);
        titleText.fontSize = 36;

        TMP_Text playerHpText = CreateText(canvas.transform, "Player HP Text", "Hero HP: 100/100", new Vector2(-360, 130), new Vector2(420, 50), TextAlignmentOptions.Left);
        Slider playerHpSlider = CreateHpSlider(canvas.transform, "Player HP Slider", new Vector2(-360, 100), new Vector2(420, 22), new Color(0.22f, 0.72f, 0.38f));
        TMP_Text playerApText = CreateText(canvas.transform, "Player AP Text", "AP: 3/3", new Vector2(-360, 75), new Vector2(420, 45), TextAlignmentOptions.Left);
        Slider playerApSlider = CreateHpSlider(canvas.transform, "Player AP Slider", new Vector2(-360, 50), new Vector2(420, 18), new Color(0.26f, 0.56f, 1.0f));
        TMP_Text playerStatusText = CreateText(canvas.transform, "Player Status Text", "Status: Ready", new Vector2(-360, 25), new Vector2(420, 40), TextAlignmentOptions.Left);
        playerStatusText.fontSize = 22;
        playerStatusText.color = new Color(0.78f, 1.0f, 0.76f);
        TMP_Text enemyHpText = CreateText(canvas.transform, "Enemy HP Text", "Slime HP: 80/80", new Vector2(360, 130), new Vector2(420, 50), TextAlignmentOptions.Right);
        Slider enemyHpSlider = CreateHpSlider(canvas.transform, "Enemy HP Slider", new Vector2(360, 100), new Vector2(420, 22), new Color(0.82f, 0.22f, 0.24f));
        TMP_Text enemyStatusText = CreateText(canvas.transform, "Enemy Status Text", "Status: None", new Vector2(360, 75), new Vector2(420, 45), TextAlignmentOptions.Right);
        TMP_Text enemyIntentText = CreateText(canvas.transform, "Enemy Intent Text", "Next Enemy: Normal Attack (15)", new Vector2(360, 45), new Vector2(420, 45), TextAlignmentOptions.Right);
        enemyIntentText.fontSize = 22;
        enemyIntentText.color = new Color(1.0f, 0.78f, 0.42f);
        TMP_Text messageText = CreateText(canvas.transform, "Message Text", "Battle Start!", new Vector2(0, -75), new Vector2(900, 100), TextAlignmentOptions.Center);
        TMP_Text skillHelpText = CreateText(canvas.transform, "Skill Help Text", "Skill Help", new Vector2(0, 15), new Vector2(900, 95), TextAlignmentOptions.TopLeft);
        skillHelpText.fontSize = 18;
        skillHelpText.color = new Color(0.72f, 0.90f, 1.0f);
        Image resultSummaryPanel = CreatePanel(canvas.transform, "Result Summary Panel", new Vector2(0, -145), new Vector2(940, 130), new Color(0.06f, 0.07f, 0.10f, 0.86f));
        resultSummaryPanel.gameObject.SetActive(false);
        TMP_Text resultSummaryText = CreateText(canvas.transform, "Result Summary Text", "Result Summary", new Vector2(0, -145), new Vector2(900, 105), TextAlignmentOptions.TopLeft);
        resultSummaryText.fontSize = 20;
        resultSummaryText.color = new Color(1.0f, 0.92f, 0.58f);
        resultSummaryText.gameObject.SetActive(false);
        TMP_Text battleLogText = CreateText(canvas.transform, "Battle Log Text", "Battle Log", new Vector2(0, -220), new Vector2(900, 150), TextAlignmentOptions.TopLeft);
        battleLogText.fontSize = 20;
        battleLogText.color = new Color(0.82f, 0.86f, 0.95f);

        Button attackButton = CreateButton(canvas.transform, "Attack Button", "Attack", new Vector2(-330, 85), new Vector2(180, 65));
        Button fireSkillButton = CreateButton(canvas.transform, "Fire Skill Button", "Fire Skill", new Vector2(-110, 85), new Vector2(180, 65));
        Button guardButton = CreateButton(canvas.transform, "Guard Button", "Guard", new Vector2(110, 85), new Vector2(180, 65));
        Button endTurnButton = CreateButton(canvas.transform, "End Turn Button", "End Turn", new Vector2(330, 85), new Vector2(180, 65));
        Button retryButton = CreateButton(canvas.transform, "Retry Button", "Retry", new Vector2(0, -325), new Vector2(220, 70));
        retryButton.gameObject.SetActive(false);

        GameObject battleManagerObject = new GameObject("BattleManager");
        BattleManager battleManager = battleManagerObject.AddComponent<BattleManager>();

        SerializedObject serializedBattleManager = new SerializedObject(battleManager);
        SetObjectReference(serializedBattleManager, "playerHpText", playerHpText);
        SetObjectReference(serializedBattleManager, "playerHpSlider", playerHpSlider);
        SetObjectReference(serializedBattleManager, "playerApText", playerApText);
        SetObjectReference(serializedBattleManager, "playerApSlider", playerApSlider);
        SetObjectReference(serializedBattleManager, "playerStatusText", playerStatusText);
        SetObjectReference(serializedBattleManager, "enemyHpText", enemyHpText);
        SetObjectReference(serializedBattleManager, "enemyHpSlider", enemyHpSlider);
        SetObjectReference(serializedBattleManager, "enemyStatusText", enemyStatusText);
        SetObjectReference(serializedBattleManager, "enemyIntentText", enemyIntentText);
        SetObjectReference(serializedBattleManager, "messageText", messageText);
        SetObjectReference(serializedBattleManager, "skillHelpText", skillHelpText);
        SetObjectReference(serializedBattleManager, "battleLogText", battleLogText);
        SetObjectReference(serializedBattleManager, "resultSummaryText", resultSummaryText);
        SetObjectReference(serializedBattleManager, "resultSummaryPanel", resultSummaryPanel.gameObject);
        SetObjectReference(serializedBattleManager, "attackButton", attackButton);
        SetObjectReference(serializedBattleManager, "fireSkillButton", fireSkillButton);
        SetObjectReference(serializedBattleManager, "guardButton", guardButton);
        SetObjectReference(serializedBattleManager, "endTurnButton", endTurnButton);
        SetObjectReference(serializedBattleManager, "retryButton", retryButton);
        serializedBattleManager.ApplyModifiedPropertiesWithoutUndo();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, ScenePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Selection.activeGameObject = battleManagerObject;
        EditorUtility.DisplayDialog(
            "BattleScene Created",
            "Assets/Scenes/BattleScene.unity created!\n\nPress Play to test Attack / Fire Skill / Guard / End Turn / Retry.",
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
        Button guardButton = FindButton("Guard Button");
        Button endTurnButton = FindButton("End Turn Button");
        Button retryButton = FindButtonIncludingInactive("Retry Button");
        Slider playerHpSlider = FindSlider("Player HP Slider");
        Slider playerApSlider = FindSlider("Player AP Slider");
        Slider enemyHpSlider = FindSlider("Enemy HP Slider");
        TMP_Text playerStatusText = FindText("Player Status Text");
        TMP_Text skillHelpText = FindText("Skill Help Text");
        TMP_Text enemyStatusText = FindText("Enemy Status Text");
        TMP_Text enemyIntentText = FindText("Enemy Intent Text");
        TMP_Text resultSummaryText = FindTextIncludingInactive("Result Summary Text");
        Image resultSummaryPanel = FindImageIncludingInactive("Result Summary Panel");

        AppendCheck(ref passed, ref report, "Player Status text exists", playerStatusText != null);
        AppendCheck(ref passed, ref report, "Skill Help text exists", skillHelpText != null);
        AppendCheck(ref passed, ref report, "Enemy Status text exists", enemyStatusText != null);
        AppendCheck(ref passed, ref report, "Enemy Intent text exists", enemyIntentText != null);
        AppendCheck(ref passed, ref report, "Result Summary text exists", resultSummaryText != null);
        AppendCheck(ref passed, ref report, "Result Summary panel exists", resultSummaryPanel != null);
        AppendCheck(ref passed, ref report, "Result Summary panel is configured but initially hidden", IsPanelLikelyConfigured(resultSummaryPanel) && resultSummaryPanel != null && !resultSummaryPanel.gameObject.activeSelf);
        AppendCheck(ref passed, ref report, "Attack button exists", attackButton != null);
        AppendCheck(ref passed, ref report, "Fire Skill button exists", fireSkillButton != null);
        AppendCheck(ref passed, ref report, "Guard button exists", guardButton != null);
        AppendCheck(ref passed, ref report, "End Turn button exists", endTurnButton != null);
        AppendCheck(ref passed, ref report, "Retry button exists", retryButton != null);
        AppendCheck(ref passed, ref report, "Player HP slider exists", playerHpSlider != null);
        AppendCheck(ref passed, ref report, "Player AP slider exists", playerApSlider != null);
        AppendCheck(ref passed, ref report, "Enemy HP slider exists", enemyHpSlider != null);
        AppendCheck(ref passed, ref report, "Attack button is visible", IsButtonLikelyVisible(attackButton));
        AppendCheck(ref passed, ref report, "Fire Skill button is visible", IsButtonLikelyVisible(fireSkillButton));
        AppendCheck(ref passed, ref report, "Guard button is visible", IsButtonLikelyVisible(guardButton));
        AppendCheck(ref passed, ref report, "End Turn button is visible", IsButtonLikelyVisible(endTurnButton));
        AppendCheck(ref passed, ref report, "Retry button is configured but initially hidden", IsButtonLikelyConfigured(retryButton) && retryButton != null && !retryButton.gameObject.activeSelf);
        AppendCheck(ref passed, ref report, "Player HP slider is configured", IsSliderLikelyConfigured(playerHpSlider));
        AppendCheck(ref passed, ref report, "Player AP slider is configured", IsSliderLikelyConfigured(playerApSlider));
        AppendCheck(ref passed, ref report, "Enemy HP slider is configured", IsSliderLikelyConfigured(enemyHpSlider));

        if (battleManager != null)
        {
            SerializedObject serializedBattleManager = new SerializedObject(battleManager);
            AppendCheck(ref passed, ref report, "Player HP text linked", HasObjectReference(serializedBattleManager, "playerHpText"));
            AppendCheck(ref passed, ref report, "Player HP slider linked", HasObjectReference(serializedBattleManager, "playerHpSlider"));
            AppendCheck(ref passed, ref report, "Player AP text linked", HasObjectReference(serializedBattleManager, "playerApText"));
            AppendCheck(ref passed, ref report, "Player AP slider linked", HasObjectReference(serializedBattleManager, "playerApSlider"));
            AppendCheck(ref passed, ref report, "Player Status text linked", HasObjectReference(serializedBattleManager, "playerStatusText"));
            AppendCheck(ref passed, ref report, "Enemy HP text linked", HasObjectReference(serializedBattleManager, "enemyHpText"));
            AppendCheck(ref passed, ref report, "Enemy HP slider linked", HasObjectReference(serializedBattleManager, "enemyHpSlider"));
            AppendCheck(ref passed, ref report, "Enemy Status text linked", HasObjectReference(serializedBattleManager, "enemyStatusText"));
            AppendCheck(ref passed, ref report, "Enemy Intent text linked", HasObjectReference(serializedBattleManager, "enemyIntentText"));
            AppendCheck(ref passed, ref report, "Message text linked", HasObjectReference(serializedBattleManager, "messageText"));
            AppendCheck(ref passed, ref report, "Skill Help text linked", HasObjectReference(serializedBattleManager, "skillHelpText"));
            AppendCheck(ref passed, ref report, "Battle Log text linked", HasObjectReference(serializedBattleManager, "battleLogText"));
            AppendCheck(ref passed, ref report, "Result Summary text linked", HasObjectReference(serializedBattleManager, "resultSummaryText"));
            AppendCheck(ref passed, ref report, "Result Summary panel linked", HasObjectReference(serializedBattleManager, "resultSummaryPanel"));
            AppendCheck(ref passed, ref report, "Attack button linked", HasObjectReference(serializedBattleManager, "attackButton"));
            AppendCheck(ref passed, ref report, "Fire Skill button linked", HasObjectReference(serializedBattleManager, "fireSkillButton"));
            AppendCheck(ref passed, ref report, "Guard button linked", HasObjectReference(serializedBattleManager, "guardButton"));
            AppendCheck(ref passed, ref report, "End Turn button linked", HasObjectReference(serializedBattleManager, "endTurnButton"));
            AppendCheck(ref passed, ref report, "Retry button linked", HasObjectReference(serializedBattleManager, "retryButton"));
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

    private static bool IsPanelLikelyConfigured(Image panelImage)
    {
        if (panelImage == null)
        {
            return false;
        }

        RectTransform rectTransform = panelImage.GetComponent<RectTransform>();
        return rectTransform != null && rectTransform.sizeDelta.x >= 900f && rectTransform.sizeDelta.y >= 100f && panelImage.color.a > 0.5f;
    }

    private static bool HasObjectReference(SerializedObject serializedObject, string propertyName)
    {
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
