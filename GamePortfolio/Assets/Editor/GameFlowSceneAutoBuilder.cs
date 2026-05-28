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

        CreateStageSelectShowcaseFrame(canvas.transform);

        TMP_Text headerText = CreateText(canvas.transform, "Header Text", "Select Stage", new Vector2(0, 305), new Vector2(600, 54), TextAlignmentOptions.Center);
        headerText.fontSize = 36;
        headerText.color = new Color(0.92f, 0.78f, 0.38f);

        // Stage cards — 6 cards in 2 rows of 3
        int cardCount = 6;
        float cardStartX = -260f;
        float cardY = 150f;
        float cardSpacingX = 260f;
        float cardSpacingY = -155f;
        Vector2 cardSize = new Vector2(220, 135);

        string[] cardNames = {
            "Slime Scout Route",
            "Wolf Ambush",
            "Golem Depths",
            "Storm Peaks",
            "Shadow Realm",
            "Sanctuary of Radiance"
        };

        string[] cardDescs = {
            "Basic slime encounter",
            "Wolf pack ambush",
            "Ancient golem depths",
            "Lightning storm peaks",
            "Shadow realm void",
            "Sanctuary of light"
        };

        string[] cardElements = { "FIRE", "NAT", "EARTH", "LIT", "DARK", "LIGHT" };
        string[] cardDifficulties = { "D1", "D1", "D2", "D2", "D3", "D3" };

        Button[] cardButtons = new Button[cardCount];
        Image[] cardBgs = new Image[cardCount];
        TMP_Text[] statusTexts = new TMP_Text[cardCount];

        for (int i = 0; i < cardCount; i++)
        {
            int row = i / 3;
            int col = i % 3;
            float x = cardStartX + col * cardSpacingX;
            float y = cardY + row * cardSpacingY;

            // Card background + button
            Button cardBtn = CreateCardButton(canvas.transform, $"Stage Card {i + 1}", new Vector2(x, y), cardSize,
                new Color(0.07f, 0.07f, 0.12f, 0.92f), new Color(0.15f, 0.20f, 0.35f, 0.95f));
            cardBtn.interactable = i == 0; // Only Stage 1 interactive by default
            cardButtons[i] = cardBtn;
            cardBgs[i] = cardBtn.GetComponent<Image>();

            // Stage name text
            TMP_Text nameText = CreateText(cardBtn.transform, $"Stage {i + 1} Name Text", cardNames[i], new Vector2(0, 38), new Vector2(190, 28), TextAlignmentOptions.Center);
            nameText.fontSize = 18;
            nameText.color = i == 0 ? Color.white : new Color(0.5f, 0.5f, 0.5f);

            // Element icon + difficulty text (single line)
            string eleDiffStr = $"{cardElements[i]} {cardDifficulties[i]}";
            TMP_Text eleDiffText = CreateText(cardBtn.transform, $"Stage {i + 1} EleDiff Text", eleDiffStr, new Vector2(0, 12), new Vector2(190, 22), TextAlignmentOptions.Center);
            eleDiffText.fontSize = 16;
            eleDiffText.color = i == 0 ? new Color(0.82f, 0.86f, 0.95f) : new Color(0.5f, 0.5f, 0.5f);

            // Description text (short for card preview)
            TMP_Text shortDescText = CreateText(cardBtn.transform, $"Stage {i + 1} Desc Text", cardDescs[i], new Vector2(0, -10), new Vector2(190, 22), TextAlignmentOptions.Top);
            shortDescText.fontSize = 13;
            shortDescText.color = i == 0 ? new Color(0.72f, 0.72f, 0.72f) : new Color(0.4f, 0.4f, 0.4f);

            // Status text
            string statusLabel = i == 0 ? "NEXT" : "LOCKED";
            Color statusColor = i == 0 ? new Color(0.38f, 1f, 0.42f) : new Color(1f, 0.5f, 0.5f);
            TMP_Text statusText = CreateText(cardBtn.transform, $"Stage {i + 1} Status Text", statusLabel, new Vector2(0, -43), new Vector2(190, 24), TextAlignmentOptions.Center);
            statusText.fontSize = 16;
            statusText.color = statusColor;
            statusTexts[i] = statusText;
        }

        // Description panel — expanded for rich info
        Image descPanel = CreatePanel(canvas.transform, "Description Panel", new Vector2(0, -200), new Vector2(760, 135), new Color(0.06f, 0.06f, 0.10f, 0.88f));
        descPanel.raycastTarget = false;
        TMP_Text stageNameText = CreateText(canvas.transform, "Stage Name Text", "Stage 1-1: Slime Scout", new Vector2(0, -155), new Vector2(710, 26), TextAlignmentOptions.Center);
        stageNameText.fontSize = 20;
        stageNameText.color = new Color(0.92f, 0.78f, 0.38f);
        TMP_Text descText = CreateText(canvas.transform, "Stage Description Text",
            "A basic encounter against slimes.\nEncounters: Slime -> Slime King\nElement: FIRE Fire | Difficulty: D1\nReward: Rank 100-150G / 80 XP\nModifier: Tutorial Field\nGear: ATK +15\nStatus: NEXT - Click Start Battle",
            new Vector2(0, -212), new Vector2(710, 96), TextAlignmentOptions.TopLeft);
        descText.fontSize = 13;
        descText.color = new Color(0.82f, 0.82f, 0.92f);

        Button startBattleButton = CreateButton(canvas.transform, "Start Battle Button", "Start Battle", new Vector2(-120, -340), new Vector2(260, 54));
        Button backButton = CreateButton(canvas.transform, "Back Button", "Back", new Vector2(120, -340), new Vector2(260, 54));

        // Create StageSelectController and wire everything
        GameObject controllerObj = new GameObject("StageSelectController");
        StageSelectController controller = controllerObj.AddComponent<StageSelectController>();

        SerializedObject serializedController = new SerializedObject(controller);
        SetObjectReference(serializedController, "stage1CardButton", cardButtons[0]);
        SetObjectReference(serializedController, "stage2CardButton", cardButtons[1]);
        SetObjectReference(serializedController, "stage3CardButton", cardButtons[2]);
        SetObjectReference(serializedController, "stage4CardButton", cardButtons[3]);
        SetObjectReference(serializedController, "stage5CardButton", cardButtons[4]);
        SetObjectReference(serializedController, "stage6CardButton", cardButtons[5]);
        SetObjectReference(serializedController, "stage1CardBg", cardBgs[0]);
        SetObjectReference(serializedController, "stage2CardBg", cardBgs[1]);
        SetObjectReference(serializedController, "stage3CardBg", cardBgs[2]);
        SetObjectReference(serializedController, "stage4CardBg", cardBgs[3]);
        SetObjectReference(serializedController, "stage5CardBg", cardBgs[4]);
        SetObjectReference(serializedController, "stage6CardBg", cardBgs[5]);
        SetObjectReference(serializedController, "stage1StatusText", statusTexts[0]);
        SetObjectReference(serializedController, "stage2StatusText", statusTexts[1]);
        SetObjectReference(serializedController, "stage3StatusText", statusTexts[2]);
        SetObjectReference(serializedController, "stage4StatusText", statusTexts[3]);
        SetObjectReference(serializedController, "stage5StatusText", statusTexts[4]);
        SetObjectReference(serializedController, "stage6StatusText", statusTexts[5]);
        SetObjectReference(serializedController, "stageNameText", stageNameText);
        SetObjectReference(serializedController, "stageDescriptionText", descText);
        SetObjectReference(serializedController, "startBattleButton", startBattleButton);
        SetObjectReference(serializedController, "backButton", backButton);
        serializedController.ApplyModifiedPropertiesWithoutUndo();

        // Init button states
        startBattleButton.interactable = false;
        cardButtons[0].interactable = true;

        // Create GameSceneFlow
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
            AppendCheck(ref passed, ref report, "StageSelect showcase frame exists", HasSceneObject("Stage Select Showcase Frame Panel"));
            AppendCheck(ref passed, ref report, "StageSelect top glow exists", HasSceneObject("Stage Select Top Glow Panel"));
            AppendCheck(ref passed, ref report, "StageSelect card rail exists", HasSceneObject("Stage Card Rail Panel"));
            AppendCheck(ref passed, ref report, "StageSelect gold dividers exist", HasSceneObject("Stage Select Top Gold Divider Panel") && HasSceneObject("Stage Select Bottom Gold Divider Panel"));
            AppendCheck(ref passed, ref report, "StageSelect description divider exists", HasSceneObject("Description Gold Divider Panel"));
            AppendCheck(ref passed, ref report, "StageSelect chapter label exists", HasSceneObject("Stage Select Chapter Label Text"));
            StageSelectController controller = Object.FindObjectOfType<StageSelectController>();
            AppendCheck(ref passed, ref report, "StageSelectController exists", controller != null);
            if (controller != null)
            {
                AppendCheck(ref passed, ref report, "Stage 1 Card button exists", controller.DebugStage1CardButtonExists);
                AppendCheck(ref passed, ref report, "Stage 1 Card is interactable (unlocked)", controller.DebugStage1CardInteractable);
                AppendCheck(ref passed, ref report, "Stage 1 status text exists", !string.IsNullOrEmpty(controller.DebugStage1StatusText));
                AppendCheck(ref passed, ref report, "Stage 1 status shows NEXT (first unlocked + incomplete)", controller.DebugStage1StatusText == "NEXT");
                AppendCheck(ref passed, ref report, "Stage 2 Card button exists (unlock-ready)", controller.DebugStage2CardButtonExists);
                AppendCheck(ref passed, ref report, "Stage 2 Card button is non-interactive (locked)", !controller.DebugStage2CardInteractable);
                AppendCheck(ref passed, ref report, "Stage 2 status shows LOCKED", controller.DebugStage2StatusText == "LOCKED");
                AppendCheck(ref passed, ref report, "Stage 3 Card button exists", controller.DebugStage3CardButtonExists);
                AppendCheck(ref passed, ref report, "Stage 4 Card button exists", controller.DebugStage4CardButtonExists);
                AppendCheck(ref passed, ref report, "Stage 5 Card button exists", controller.DebugStage5CardButtonExists);
                AppendCheck(ref passed, ref report, "Stage 6 Card button exists", controller.DebugStage6CardButtonExists);
                AppendCheck(ref passed, ref report, "Stage Name text exists", controller.DebugStageNameTextExists);
                AppendCheck(ref passed, ref report, "Stage Description text exists", controller.DebugStageDescriptionTextExists);
                AppendCheck(ref passed, ref report, "Start Battle button exists", controller.DebugStartBattleButtonExists);
                AppendCheck(ref passed, ref report, "Start Battle button starts disabled (no selection)", !controller.DebugStartBattleButtonInteractable);
                AppendCheck(ref passed, ref report, "Back button exists", controller.DebugBackButtonExists);
                // Select Stage 1 to verify modifier info appears in description panel
                controller.DebugSelectStage(0);
                AppendCheck(ref passed, ref report, "Stage 1 description starts with Modifier: header", controller.DebugStageDescriptionText.Contains("Modifier:"));
                AppendCheck(ref passed, ref report, "Stage 1 description shows Tutorial Field modifier name", controller.DebugStageDescriptionText.Contains("Tutorial Field"));
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

    private static bool HasSceneObject(string objectName)
    {
        return GameObject.Find(objectName) != null;
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

    private static void CreateStageSelectShowcaseFrame(Transform parent)
    {
        Image frame = CreatePanel(parent, "Stage Select Showcase Frame Panel", new Vector2(0, 20), new Vector2(1080, 620), new Color(0.025f, 0.027f, 0.045f, 0.82f));
        frame.raycastTarget = false;

        Image topGlow = CreatePanel(parent, "Stage Select Top Glow Panel", new Vector2(0, 330), new Vector2(920, 30), new Color(0.16f, 0.22f, 0.40f, 0.45f));
        topGlow.raycastTarget = false;

        Image topDivider = CreatePanel(parent, "Stage Select Top Gold Divider Panel", new Vector2(0, 238), new Vector2(900, 4), new Color(0.82f, 0.62f, 0.24f, 0.95f));
        topDivider.raycastTarget = false;

        Image cardRail = CreatePanel(parent, "Stage Card Rail Panel", new Vector2(0, 72), new Vector2(880, 320), new Color(0.035f, 0.040f, 0.070f, 0.70f));
        cardRail.raycastTarget = false;

        Image bottomDivider = CreatePanel(parent, "Stage Select Bottom Gold Divider Panel", new Vector2(0, -92), new Vector2(760, 3), new Color(0.72f, 0.48f, 0.18f, 0.88f));
        bottomDivider.raycastTarget = false;

        Image descDivider = CreatePanel(parent, "Description Gold Divider Panel", new Vector2(0, -122), new Vector2(620, 3), new Color(0.82f, 0.62f, 0.24f, 0.90f));
        descDivider.raycastTarget = false;

        TMP_Text chapterText = CreateText(parent, "Stage Select Chapter Label Text", "CHAPTER 1 - TUTORIAL FRONT", new Vector2(0, 258), new Vector2(680, 24), TextAlignmentOptions.Center);
        chapterText.fontSize = 16;
        chapterText.color = new Color(0.70f, 0.82f, 1.0f, 0.92f);
        chapterText.raycastTarget = false;
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
