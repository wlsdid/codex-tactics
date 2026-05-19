using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Batch routine to create the Settings scene and register it in Build Settings.
/// Invoked via Unity -executeMethod.
/// </summary>
public static class BatchSettingsBuilder
{
    public static void BuildAndRegister()
    {
        Debug.Log("=== BatchSettingsBuilder: Starting ===");

        // Step 1: Create Settings scene in-memory (don't open other scenes)
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        SceneManager.SetActiveScene(scene);

        string settingsScenePath = "Assets/Scenes/SettingsScene.unity";

        // Create the Settings scene UI elements
        // ── Main Canvas ──
        GameObject canvasObj = new GameObject("Settings Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasObj.AddComponent<GraphicRaycaster>();

        // ── Dark background ──
        GameObject bgObj = new GameObject("Background", typeof(RectTransform));
        bgObj.transform.SetParent(canvasObj.transform, false);
        Image bg = bgObj.AddComponent<Image>();
        bg.color = new Color(0.05f, 0.06f, 0.09f, 0.85f);
        RectTransform bgRt = bgObj.GetComponent<RectTransform>();
        bgRt.anchorMin = Vector2.zero;
        bgRt.anchorMax = Vector2.one;
        bgRt.offsetMin = Vector2.zero;
        bgRt.offsetMax = Vector2.zero;

        // ── Title ──
        GameObject titleObj = new GameObject("Title Text", typeof(RectTransform));
        titleObj.transform.SetParent(canvasObj.transform, false);
        TMP_Text title = titleObj.AddComponent<TextMeshProUGUI>();
        title.text = "Settings";
        title.fontSize = 42;
        title.fontStyle = FontStyles.Bold;
        title.alignment = TextAlignmentOptions.Center;
        title.color = new Color(0.92f, 0.86f, 0.55f);
        RectTransform titleRt = titleObj.GetComponent<RectTransform>();
        titleRt.sizeDelta = new Vector2(400, 60);
        titleRt.anchoredPosition = new Vector2(0, 280);

        // ── Settings Panel ──
        GameObject panelObj = new GameObject("Settings Panel", typeof(RectTransform));
        panelObj.transform.SetParent(canvasObj.transform, false);
        Image panelImg = panelObj.AddComponent<Image>();
        panelImg.color = new Color(0.08f, 0.10f, 0.18f, 0.95f);
        RectTransform panelRt = panelObj.GetComponent<RectTransform>();
        panelRt.sizeDelta = new Vector2(520, 400);
        panelRt.anchoredPosition = Vector2.zero;

        // ── BGM Volume ──
        CreateSlider(panelObj.transform, "BgmSlider", "BGM Volume", 120f, 0.3f);

        // ── SFX Volume ──
        CreateSlider(panelObj.transform, "SfxSlider", "SFX Volume", 40f, 0.5f);

        // ── Slider value labels ──
        GameObject bgmValLabel = new GameObject("BgmValueLabel", typeof(RectTransform));
        bgmValLabel.transform.SetParent(panelObj.transform, false);
        TMP_Text bgmValText = bgmValLabel.AddComponent<TextMeshProUGUI>();
        bgmValText.text = "30%";
        bgmValText.fontSize = 18;
        bgmValText.alignment = TextAlignmentOptions.Right;
        bgmValText.color = new Color(0.85f, 0.85f, 0.90f);
        RectTransform bgmValRt = bgmValLabel.GetComponent<RectTransform>();
        bgmValRt.sizeDelta = new Vector2(60, 30);
        bgmValRt.anchoredPosition = new Vector2(200, 120);

        GameObject sfxValLabel = new GameObject("SfxValueLabel", typeof(RectTransform));
        sfxValLabel.transform.SetParent(panelObj.transform, false);
        TMP_Text sfxValText = sfxValLabel.AddComponent<TextMeshProUGUI>();
        sfxValText.text = "50%";
        sfxValText.fontSize = 18;
        sfxValText.alignment = TextAlignmentOptions.Right;
        sfxValText.color = new Color(0.85f, 0.85f, 0.90f);
        RectTransform sfxValRt = sfxValLabel.GetComponent<RectTransform>();
        sfxValRt.sizeDelta = new Vector2(60, 30);
        sfxValRt.anchoredPosition = new Vector2(200, 40);

        // ── Reset Save Data Button ──
        GameObject resetBtnObj = new GameObject("Reset Data Button", typeof(RectTransform));
        resetBtnObj.transform.SetParent(panelObj.transform, false);
        Image resetBtnImage = resetBtnObj.AddComponent<Image>();
        resetBtnImage.color = new Color(0.35f, 0.10f, 0.12f, 0.90f);
        Button resetBtn = resetBtnObj.AddComponent<Button>();
        resetBtn.targetGraphic = resetBtnImage;
        ColorBlock resetColors = resetBtn.colors;
        resetColors.normalColor = new Color(0.35f, 0.10f, 0.12f, 0.90f);
        resetColors.highlightedColor = new Color(0.50f, 0.15f, 0.18f, 0.90f);
        resetColors.pressedColor = new Color(0.25f, 0.08f, 0.10f, 0.95f);
        resetBtn.colors = resetColors;
        RectTransform resetBtnRt = resetBtnObj.GetComponent<RectTransform>();
        resetBtnRt.sizeDelta = new Vector2(340, 50);
        resetBtnRt.anchoredPosition = new Vector2(0, -80);

        GameObject resetBtnTextObj = new GameObject("Reset Button Text", typeof(RectTransform));
        resetBtnTextObj.transform.SetParent(resetBtnObj.transform, false);
        TMP_Text resetBtnText = resetBtnTextObj.AddComponent<TextMeshProUGUI>();
        resetBtnText.text = "Reset Save Data";
        resetBtnText.fontSize = 20;
        resetBtnText.fontStyle = FontStyles.Bold;
        resetBtnText.alignment = TextAlignmentOptions.Center;
        resetBtnText.color = Color.white;
        RectTransform resetBtnTextRt = resetBtnTextObj.GetComponent<RectTransform>();
        resetBtnTextRt.sizeDelta = new Vector2(340, 50);
        resetBtnTextRt.anchoredPosition = Vector2.zero;

        // ── Back Button ──
        GameObject backBtnObj = new GameObject("Back Button", typeof(RectTransform));
        backBtnObj.transform.SetParent(canvasObj.transform, false);
        Image backBtnImage = backBtnObj.AddComponent<Image>();
        backBtnImage.color = new Color(0.15f, 0.20f, 0.30f, 0.95f);
        Button backBtn = backBtnObj.AddComponent<Button>();
        backBtn.targetGraphic = backBtnImage;
        RectTransform backBtnRt = backBtnObj.GetComponent<RectTransform>();
        backBtnRt.sizeDelta = new Vector2(200, 50);
        backBtnRt.anchoredPosition = new Vector2(0, -330);

        GameObject backBtnTextObj = new GameObject("Back Button Text", typeof(RectTransform));
        backBtnTextObj.transform.SetParent(backBtnObj.transform, false);
        TMP_Text backBtnText = backBtnTextObj.AddComponent<TextMeshProUGUI>();
        backBtnText.text = "Back to Title";
        backBtnText.fontSize = 22;
        backBtnText.fontStyle = FontStyles.Bold;
        backBtnText.alignment = TextAlignmentOptions.Center;
        backBtnText.color = new Color(0.72f, 0.90f, 1.0f);
        RectTransform backBtnTextRt = backBtnTextObj.GetComponent<RectTransform>();
        backBtnTextRt.sizeDelta = new Vector2(200, 50);
        backBtnTextRt.anchoredPosition = Vector2.zero;

        // ── Settings Controller ──
        GameObject controllerObj = new GameObject("SettingsController");
        SettingsController controller = controllerObj.AddComponent<SettingsController>();
        controller.backButton = backBtn;
        controller.resetDataButton = resetBtn;

        // Find sliders and value labels
        foreach (Transform child in panelObj.transform)
        {
            if (child.name == "BgmSlider")
            {
                Slider s = child.GetComponentInChildren<Slider>(true);
                if (s != null) controller.bgmSlider = s;
            }
            else if (child.name == "SfxSlider")
            {
                Slider s = child.GetComponentInChildren<Slider>(true);
                if (s != null) controller.sfxSlider = s;
            }
            else if (child.name == "BgmValueLabel")
            {
                controller.TrySetBgmValueLabel(child.GetComponent<TMP_Text>());
            }
            else if (child.name == "SfxValueLabel")
            {
                controller.TrySetSfxValueLabel(child.GetComponent<TMP_Text>());
            }
        }

        // ── Save scene ──
        EditorSceneManager.SaveScene(scene, settingsScenePath);
        Debug.Log($"Settings scene created at: {settingsScenePath}");

        // Step 2: Add to Build Settings
        bool found = false;
        foreach (var editorScene in EditorBuildSettings.scenes)
        {
            if (editorScene.path == settingsScenePath)
            {
                found = true;
                break;
            }
        }

        if (!found)
        {
            Debug.Log("Adding SettingsScene to Build Settings...");
            var scenes = EditorBuildSettings.scenes;
            var newList = new System.Collections.Generic.List<EditorBuildSettingsScene>(scenes);
            newList.Add(new EditorBuildSettingsScene(settingsScenePath, true));
            EditorBuildSettings.scenes = newList.ToArray();
            Debug.Log("SettingsScene added to Build Settings.");
        }
        else
        {
            Debug.Log("SettingsScene already in Build Settings.");
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("=== BatchSettingsBuilder: Complete ===");
    }

    private static void CreateSlider(Transform parent, string name, string label, float yPos, float defaultValue)
    {
        // Container row
        GameObject rowObj = new GameObject(name, typeof(RectTransform));
        rowObj.transform.SetParent(parent, false);
        RectTransform rowRt = rowObj.GetComponent<RectTransform>();
        rowRt.sizeDelta = new Vector2(400, 50);
        rowRt.anchoredPosition = new Vector2(0, yPos);

        // Label
        GameObject labelObj = new GameObject("Label", typeof(RectTransform));
        labelObj.transform.SetParent(rowObj.transform, false);
        TMP_Text labelText = labelObj.AddComponent<TextMeshProUGUI>();
        labelText.text = label;
        labelText.fontSize = 20;
        labelText.alignment = TextAlignmentOptions.Left;
        labelText.color = new Color(0.85f, 0.85f, 0.90f);
        RectTransform labelRt = labelObj.GetComponent<RectTransform>();
        labelRt.sizeDelta = new Vector2(140, 30);
        labelRt.anchoredPosition = new Vector2(-150, 0);

        // Slider
        GameObject sliderObj = new GameObject("SliderCore", typeof(RectTransform));
        sliderObj.transform.SetParent(rowObj.transform, false);
        Slider slider = sliderObj.AddComponent<Slider>();
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = defaultValue;
        slider.direction = Slider.Direction.LeftToRight;
        slider.fillRect = null;
        RectTransform sliderRt = sliderObj.GetComponent<RectTransform>();
        sliderRt.sizeDelta = new Vector2(200, 30);
        sliderRt.anchoredPosition = new Vector2(10, 0);

        // Slider background
        GameObject sliderBgObj = new GameObject("Background", typeof(RectTransform));
        sliderBgObj.transform.SetParent(sliderObj.transform, false);
        Image sliderBg = sliderBgObj.AddComponent<Image>();
        sliderBg.color = new Color(0.15f, 0.18f, 0.30f, 0.85f);
        RectTransform sliderBgRt = sliderBgObj.GetComponent<RectTransform>();
        sliderBgRt.anchorMin = new Vector2(0, 0.25f);
        sliderBgRt.anchorMax = new Vector2(1, 0.75f);
        sliderBgRt.offsetMin = Vector2.zero;
        sliderBgRt.offsetMax = Vector2.zero;

        // Fill area
        GameObject fillAreaObj = new GameObject("Fill Area", typeof(RectTransform));
        fillAreaObj.transform.SetParent(sliderObj.transform, false);
        RectTransform fillAreaRt = fillAreaObj.GetComponent<RectTransform>();
        fillAreaRt.anchorMin = new Vector2(0, 0.25f);
        fillAreaRt.anchorMax = new Vector2(1, 0.75f);
        fillAreaRt.offsetMin = new Vector2(10, 0);
        fillAreaRt.offsetMax = new Vector2(-10, 0);

        GameObject fillObj = new GameObject("Fill", typeof(RectTransform));
        fillObj.transform.SetParent(fillAreaObj.transform, false);
        Image fillImg = fillObj.AddComponent<Image>();
        fillImg.color = new Color(0.30f, 0.55f, 0.85f);
        RectTransform fillRt = fillObj.GetComponent<RectTransform>();
        fillRt.anchorMin = new Vector2(0, 0);
        fillRt.anchorMax = new Vector2(1, 1);
        fillRt.offsetMin = Vector2.zero;
        fillRt.offsetMax = Vector2.zero;
        slider.fillRect = fillRt;

        // Handle
        GameObject handleAreaObj = new GameObject("Handle Slide Area", typeof(RectTransform));
        handleAreaObj.transform.SetParent(sliderObj.transform, false);
        RectTransform handleAreaRt = handleAreaObj.GetComponent<RectTransform>();
        handleAreaRt.anchorMin = new Vector2(0, 0);
        handleAreaRt.anchorMax = new Vector2(1, 1);
        handleAreaRt.offsetMin = Vector2.zero;
        handleAreaRt.offsetMax = Vector2.zero;

        GameObject handleObj = new GameObject("Handle", typeof(RectTransform));
        handleObj.transform.SetParent(handleAreaObj.transform, false);
        Image handleImg = handleObj.AddComponent<Image>();
        handleImg.color = new Color(0.85f, 0.90f, 1.0f);
        RectTransform handleRt = handleObj.GetComponent<RectTransform>();
        handleRt.sizeDelta = new Vector2(20, 20);
        slider.handleRect = handleRt;
        slider.targetGraphic = handleImg;
    }
}
