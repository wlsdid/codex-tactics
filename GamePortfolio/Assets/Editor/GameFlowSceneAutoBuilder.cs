using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem.UI;
#endif

public static class GameFlowSceneAutoBuilder
{
    private const string TitleScenePath = "Assets/Scenes/TitleScene.unity";
    private const string StageSelectScenePath = "Assets/Scenes/StageSelectScene.unity";

    // ── Create ──

    [MenuItem("Tools/Codex Tactics/Create Game Flow Scenes")]
    public static void CreateGameFlowScenes()
    {
        CreateTitleScene();
        CreateStageSelectScene();
        EnsureSceneInBuildSettings(TitleScenePath, GameSceneFlow.TitleSceneName);
        EnsureSceneInBuildSettings(StageSelectScenePath, GameSceneFlow.StageSelectSceneName);
        EnsureSceneInBuildSettings("Assets/Scenes/BattleScene.unity", GameSceneFlow.BattleSceneName);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Game Flow Scenes Created", "TitleScene and StageSelectScene created.\n\nBuild settings updated.", "OK");
    }

    private static void CreateTitleScene()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        scene.name = GameSceneFlow.TitleSceneName;

        Camera camera = CreateCamera();
        Canvas canvas = CreateCanvas(camera);
        CreateEventSystem();

        Image bgPanel = CreatePanel(canvas.transform, "Background Panel", Vector2.zero, new Vector2(1200, 800), new Color(0.04f, 0.04f, 0.07f, 1f));
        bgPanel.raycastTarget = false;

        TMP_Text titleText = CreateText(canvas.transform, "Title Text", "Codex Tactics", new Vector2(0, 100), new Vector2(800, 80), TextAlignmentOptions.Center);
        titleText.fontSize = 56;
        titleText.color = new Color(0.92f, 0.78f, 0.38f);

        TMP_Text subtitleText = CreateText(canvas.transform, "Subtitle Text", "Tactical Break RPG Prototype", new Vector2(0, 30), new Vector2(600, 50), TextAlignmentOptions.Center);
        subtitleText.fontSize = 26;
        subtitleText.color = new Color(0.72f, 0.85f, 1.0f);

        Button startButton = CreateButton(canvas.transform, "Start Game Button", "Start Game", new Vector2(0, -80), new Vector2(280, 70));

        // Create GameSceneFlow and wire Start button (persistent listener for serialization)
        GameObject flowObject = new GameObject("GameSceneFlow");
        GameSceneFlow flow = flowObject.AddComponent<GameSceneFlow>();
        UnityEventTools.AddPersistentListener(startButton.onClick, flow.LoadStageSelect);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, TitleScenePath);
    }

    private static void CreateStageSelectScene()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        scene.name = GameSceneFlow.StageSelectSceneName;

        Camera camera = CreateCamera();
        Canvas canvas = CreateCanvas(camera);
        CreateEventSystem();

        Image bgPanel = CreatePanel(canvas.transform, "Background Panel", Vector2.zero, new Vector2(1200, 800), new Color(0.05f, 0.05f, 0.08f, 1f));
        bgPanel.raycastTarget = false;

        TMP_Text headerText = CreateText(canvas.transform, "Header Text", "Select Stage", new Vector2(0, 260), new Vector2(600, 60), TextAlignmentOptions.Center);
        headerText.fontSize = 40;
        headerText.color = new Color(0.92f, 0.78f, 0.38f);

        // Stage Card 1 — Available (clickable Button)
        Button stage1CardBtn = CreateCardButton(canvas.transform, "Stage Card 1", new Vector2(-200, 80), new Vector2(300, 200),
            new Color(0.07f, 0.07f, 0.12f, 0.92f), new Color(0.15f, 0.20f, 0.35f, 0.95f));
        TMP_Text stage1Text = CreateText(stage1CardBtn.transform, "Stage 1 Name Text", "Slime Scout Route", new Vector2(0, 55), new Vector2(260, 40), TextAlignmentOptions.Center);
        stage1Text.fontSize = 24;
        stage1Text.color = new Color(1f, 1f, 1f);
        TMP_Text stage1Desc = CreateText(stage1CardBtn.transform, "Stage 1 Desc Text", "Basic encounter. Slimes patrol\nthe forest path.", new Vector2(0, 0), new Vector2(260, 60), TextAlignmentOptions.Top);
        stage1Desc.fontSize = 18;
        stage1Desc.color = new Color(0.72f, 0.72f, 0.72f);
        TMP_Text stage1Status = CreateText(stage1CardBtn.transform, "Stage 1 Status Text", "Available", new Vector2(0, -55), new Vector2(260, 40), TextAlignmentOptions.Center);
        stage1Status.fontSize = 20;
        stage1Status.color = new Color(0.38f, 1f, 0.42f);

        // Stage Card 2 — Locked (Button with interactable=false for future unlock)
        Button stage2CardBtn = CreateCardButton(canvas.transform, "Stage Card 2", new Vector2(200, 80), new Vector2(300, 200),
            new Color(0.04f, 0.04f, 0.07f, 0.92f), new Color(0.04f, 0.04f, 0.07f, 0.92f));
        stage2CardBtn.interactable = false;
        TMP_Text stage2Text = CreateText(stage2CardBtn.transform, "Stage 2 Name Text", "Wolf Ambush", new Vector2(0, 55), new Vector2(260, 40), TextAlignmentOptions.Center);
        stage2Text.fontSize = 24;
        stage2Text.color = new Color(0.5f, 0.5f, 0.5f);
        TMP_Text stage2Desc = CreateText(stage2CardBtn.transform, "Stage 2 Desc Text", "Wolf packs hunt in the\nmoonlit clearing.", new Vector2(0, 0), new Vector2(260, 60), TextAlignmentOptions.Top);
        stage2Desc.fontSize = 18;
        stage2Desc.color = new Color(0.5f, 0.5f, 0.5f);
        TMP_Text stage2Status = CreateText(stage2CardBtn.transform, "Stage 2 Status Text", "Locked", new Vector2(0, -55), new Vector2(260, 40), TextAlignmentOptions.Center);
        stage2Status.fontSize = 20;
        stage2Status.color = new Color(1f, 0.5f, 0.5f);

        // Description panel — expanded for stage name + description
        Image descPanel = CreatePanel(canvas.transform, "Description Panel", new Vector2(0, -120), new Vector2(700, 100), new Color(0.06f, 0.06f, 0.10f, 0.88f));
        TMP_Text stageNameText = CreateText(canvas.transform, "Stage Name Text", "Stage 1-1: Slime Scout", new Vector2(0, -100), new Vector2(660, 30), TextAlignmentOptions.Center);
        stageNameText.fontSize = 22;
        stageNameText.color = new Color(0.92f, 0.78f, 0.38f);
        TMP_Text descText = CreateText(canvas.transform, "Stage Description Text", "A basic encounter against slimes.\nLearn the combat basics: Attack, Guard, Fire Skill, and Break.\nDefeat the Slime Scout to advance.", new Vector2(0, -130), new Vector2(660, 60), TextAlignmentOptions.Top);
        descText.fontSize = 18;
        descText.color = new Color(0.82f, 0.82f, 0.92f);

        Button startBattleButton = CreateButton(canvas.transform, "Start Battle Button", "Start Battle", new Vector2(-120, -250), new Vector2(260, 65));
        Button backButton = CreateButton(canvas.transform, "Back Button", "Back", new Vector2(120, -250), new Vector2(260, 65));

        // Create StageSelectController and wire everything
        GameObject controllerObj = new GameObject("StageSelectController");
        StageSelectController controller = controllerObj.AddComponent<StageSelectController>();

        SerializedObject serializedController = new SerializedObject(controller);
        SetObjectReference(serializedController, "stage1CardButton", stage1CardBtn);
        SetObjectReference(serializedController, "stage1CardBg", stage1CardBtn.GetComponent<Image>());
        SetObjectReference(serializedController, "stage2CardButton", stage2CardBtn);
        SetObjectReference(serializedController, "stage2CardBg", stage2CardBtn.GetComponent<Image>());
        SetObjectReference(serializedController, "stage2StatusText", stage2Status);
        SetObjectReference(serializedController, "stageNameText", stageNameText);
        SetObjectReference(serializedController, "stageDescriptionText", descText);
        SetObjectReference(serializedController, "startBattleButton", startBattleButton);
        SetObjectReference(serializedController, "backButton", backButton);
        serializedController.ApplyModifiedPropertiesWithoutUndo();

        // Init button states (controller.Start sets these, but ensure initial state in builder too)
        startBattleButton.interactable = false;
        stage1CardBtn.interactable = true;

        // Create GameSceneFlow (kept for scene name constants)
        GameObject flowObject = new GameObject("GameSceneFlow");
        flowObject.AddComponent<GameSceneFlow>();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, StageSelectScenePath);
    }

    // ── Validate ──

    [MenuItem("Tools/Codex Tactics/Validate Game Flow Scenes")]
    public static void ValidateGameFlowScenes()
    {
        bool passed = true;
        string report = "Game Flow Scenes Auto Test\n\n";

        // Check TitleScene file exists
        bool titleExists = System.IO.File.Exists(TitleScenePath);
        AppendCheck(ref passed, ref report, "TitleScene file exists", titleExists);

        // Check StageSelectScene file exists
        bool stageSelectExists = System.IO.File.Exists(StageSelectScenePath);
        AppendCheck(ref passed, ref report, "StageSelectScene file exists", stageSelectExists);

        if (titleExists)
        {
            EditorSceneManager.OpenScene(TitleScenePath, OpenSceneMode.Single);
            AppendCheck(ref passed, ref report, "TitleScene has Canvas", Object.FindObjectOfType<Canvas>() != null);
            AppendCheck(ref passed, ref report, "TitleScene has EventSystem", Object.FindObjectOfType<EventSystem>() != null);
            Button startBtn = FindButtonInScene("Start Game Button");
            AppendCheck(ref passed, ref report, "TitleScene has Start Game Button", startBtn != null);
            AppendCheck(ref passed, ref report, "TitleScene has GameSceneFlow component", Object.FindObjectOfType<GameSceneFlow>() != null);
            if (startBtn != null)
                AppendCheck(ref passed, ref report, "Start Game Button has LoadStageSelect persistent listener", VerifyButtonPersistentListener(startBtn, "LoadStageSelect"));
        }

        if (stageSelectExists)
        {
            EditorSceneManager.OpenScene(StageSelectScenePath, OpenSceneMode.Single);
            AppendCheck(ref passed, ref report, "StageSelectScene has Canvas", Object.FindObjectOfType<Canvas>() != null);
            AppendCheck(ref passed, ref report, "StageSelectScene has EventSystem", Object.FindObjectOfType<EventSystem>() != null);
            AppendCheck(ref passed, ref report, "StageSelectScene has GameSceneFlow component", Object.FindObjectOfType<GameSceneFlow>() != null);
            StageSelectController controller = Object.FindObjectOfType<StageSelectController>();
            AppendCheck(ref passed, ref report, "StageSelectController exists", controller != null);
            if (controller != null)
            {
                AppendCheck(ref passed, ref report, "Stage 1 Card button exists", controller.DebugStage1CardButtonExists);
                AppendCheck(ref passed, ref report, "Stage 1 Card is interactable (unlocked)", controller.DebugStage1CardInteractable);
                AppendCheck(ref passed, ref report, "Stage 2 Card button is non-interactive (locked)", !controller.DebugStage2CardInteractable);
                AppendCheck(ref passed, ref report, "Stage 2 Card button exists (unlock-ready)", controller.DebugStage2CardButtonExists);
                AppendCheck(ref passed, ref report, "Stage 2 status shows Locked", controller.DebugStage2StatusText == "Locked");
                AppendCheck(ref passed, ref report, "Stage Name text exists", controller.DebugStageNameTextExists);
                AppendCheck(ref passed, ref report, "Stage Description text exists", controller.DebugStageDescriptionTextExists);
                AppendCheck(ref passed, ref report, "Start Battle button exists", controller.DebugStartBattleButtonExists);
                AppendCheck(ref passed, ref report, "Start Battle button starts disabled (no selection)", !controller.DebugStartBattleButtonInteractable);
                AppendCheck(ref passed, ref report, "Back button exists", controller.DebugBackButtonExists);
            }
        }

        // Check Build Settings registration
        bool titleInBuild = false, stageInBuild = false, battleInBuild = false;
        foreach (var bs in EditorBuildSettings.scenes)
        {
            if (bs.path == TitleScenePath) titleInBuild = true;
            if (bs.path == StageSelectScenePath) stageInBuild = true;
            if (bs.path == "Assets/Scenes/BattleScene.unity") battleInBuild = true;
        }
        AppendCheck(ref passed, ref report, "TitleScene in Build Settings", titleInBuild);
        AppendCheck(ref passed, ref report, "StageSelectScene in Build Settings", stageInBuild);
        AppendCheck(ref passed, ref report, "BattleScene in Build Settings", battleInBuild);

        report += passed ? "\nRESULT: PASS" : "\nRESULT: FAIL";
        EditorUtility.DisplayDialog(passed ? "Game Flow Test Passed" : "Game Flow Test Failed", report, "OK");
    }

    // ── Build Settings ──

    private static void EnsureSceneInBuildSettings(string scenePath, string sceneName)
    {
        var buildScenes = EditorBuildSettings.scenes;
        foreach (var bs in buildScenes)
        {
            if (bs.path == scenePath)
                return; // Already registered
        }
        var newList = new System.Collections.Generic.List<EditorBuildSettingsScene>(buildScenes);
        newList.Add(new EditorBuildSettingsScene(scenePath, true));
        EditorBuildSettings.scenes = newList.ToArray();
    }

    // ── Helpers ──

    private static Button FindButtonInScene(string objectName)
    {
        GameObject obj = GameObject.Find(objectName);
        return obj != null ? obj.GetComponent<Button>() : null;
    }

    private static bool VerifyButtonPersistentListener(Button button, string expectedMethod)
    {
        int count = button.onClick.GetPersistentEventCount();
        if (count == 0) return false;
        for (int i = 0; i < count; i++)
        {
            if (button.onClick.GetPersistentMethodName(i) == expectedMethod &&
                button.onClick.GetPersistentTarget(i) != null)
                return true;
        }
        return false;
    }

    private static Camera CreateCamera()
    {
        GameObject camObj = new GameObject("Main Camera");
        Camera cam = camObj.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.04f, 0.04f, 0.07f, 1f);
        cam.orthographic = true;
        cam.orthographicSize = 5f;
        cam.nearClipPlane = 0.1f;
        cam.farClipPlane = 100f;
        cam.depth = -1;
        camObj.tag = "MainCamera";
        return cam;
    }

    private static Canvas CreateCanvas(Camera camera)
    {
        GameObject canvasObj = new GameObject("Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = camera;
        canvas.planeDistance = 10f;
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1200, 800);
        scaler.matchWidthOrHeight = 0.5f;
        canvasObj.AddComponent<GraphicRaycaster>();
        return canvas;
    }

    private static void CreateEventSystem()
    {
        GameObject esObj = new GameObject("EventSystem");
        esObj.AddComponent<EventSystem>();
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        esObj.AddComponent<InputSystemUIInputModule>();
#else
        esObj.AddComponent<StandaloneInputModule>();
#endif
    }

    private static Image CreatePanel(Transform parent, string name, Vector2 position, Vector2 size, Color color)
    {
        GameObject obj = new GameObject(name, typeof(RectTransform), typeof(Image));
        obj.transform.SetParent(parent, false);
        RectTransform rt = obj.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = position;
        rt.sizeDelta = size;
        Image img = obj.GetComponent<Image>();
        img.color = color;
        return img;
    }

    private static TMP_Text CreateText(Transform parent, string name, string content, Vector2 position, Vector2 size, TextAlignmentOptions alignment)
    {
        GameObject obj = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
        obj.transform.SetParent(parent, false);
        RectTransform rt = obj.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = position;
        rt.sizeDelta = size;
        TMP_Text text = obj.GetComponent<TextMeshProUGUI>();
        text.text = content;
        text.fontSize = 22;
        text.color = Color.white;
        text.alignment = alignment;
        return text;
    }

    private static Button CreateButton(Transform parent, string name, string label, Vector2 position, Vector2 size)
    {
        GameObject obj = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
        obj.transform.SetParent(parent, false);
        RectTransform rt = obj.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = position;
        rt.sizeDelta = size;

        Image img = obj.GetComponent<Image>();
        img.color = new Color(0.15f, 0.15f, 0.25f, 0.95f);

        // Button text as child
        GameObject textObj = new GameObject("Button Text", typeof(RectTransform), typeof(TextMeshProUGUI));
        textObj.transform.SetParent(obj.transform, false);
        RectTransform textRt = textObj.GetComponent<RectTransform>();
        textRt.anchorMin = Vector2.zero;
        textRt.anchorMax = Vector2.one;
        textRt.sizeDelta = Vector2.zero;
        TMP_Text buttonText = textObj.GetComponent<TextMeshProUGUI>();
        buttonText.text = label;
        buttonText.fontSize = 28;
        buttonText.color = new Color(0.92f, 0.88f, 0.82f);
        buttonText.alignment = TextAlignmentOptions.Center;

        Button btn = obj.GetComponent<Button>();
        btn.targetGraphic = img;
        return btn;
    }

    private static Button CreateCardButton(Transform parent, string name, Vector2 position, Vector2 size, Color bgColor, Color selectedColor)
    {
        GameObject obj = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
        obj.transform.SetParent(parent, false);
        RectTransform rt = obj.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = position;
        rt.sizeDelta = size;

        Image img = obj.GetComponent<Image>();
        img.color = bgColor;

        Button btn = obj.GetComponent<Button>();
        btn.targetGraphic = img;
        ColorBlock cb = btn.colors;
        cb.selectedColor = selectedColor;
        btn.colors = cb;
        return btn;
    }

    private static void SetObjectReference(SerializedObject so, string fieldName, Object obj)
    {
        if (obj == null) return;
        SerializedProperty prop = so.FindProperty(fieldName);
        if (prop != null)
            prop.objectReferenceValue = obj;
    }

    private static void AppendCheck(ref bool passed, ref string report, string label, bool condition)
    {
        report += condition ? "[OK] " : "[FAIL] ";
        report += label + "\n";
        if (!condition) passed = false;
    }
}
