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

        TMP_Text headerText = CreateText(canvas.transform, "Header Text", "Select Stage", new Vector2(0, 280), new Vector2(600, 60), TextAlignmentOptions.Center);
        headerText.fontSize = 40;
        headerText.color = new Color(0.92f, 0.78f, 0.38f);

        // Stage cards — 6 cards in 2 rows of 3
        int cardCount = 6;
        float cardStartX = -260f;
        float cardY = 180f;
        float cardSpacingX = 260f;
        float cardSpacingY = -170f;
        Vector2 cardSize = new Vector2(230, 160);

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

        string[] cardElements = { "🔥", "🌿", "🌍", "⚡", "🌑", "✨" };
        string[] cardDifficulties = { "★", "★", "★★", "★★", "★★★", "★★★" };

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
            TMP_Text nameText = CreateText(cardBtn.transform, $"Stage {i + 1} Name Text", cardNames[i], new Vector2(0, 45), new Vector2(200, 30), TextAlignmentOptions.Center);
            nameText.fontSize = 20;
            nameText.color = i == 0 ? Color.white : new Color(0.5f, 0.5f, 0.5f);

            // Element icon + difficulty text (single line)
            string eleDiffStr = $"{cardElements[i]} {cardDifficulties[i]}";
            TMP_Text eleDiffText = CreateText(cardBtn.transform, $"Stage {i + 1} EleDiff Text", eleDiffStr, new Vector2(0, 15), new Vector2(200, 24), TextAlignmentOptions.Center);
            eleDiffText.fontSize = 18;
            eleDiffText.color = i == 0 ? new Color(0.82f, 0.86f, 0.95f) : new Color(0.5f, 0.5f, 0.5f);

            // Description text (short for card preview)
            TMP_Text shortDescText = CreateText(cardBtn.transform, $"Stage {i + 1} Desc Text", cardDescs[i], new Vector2(0, -10), new Vector2(200, 24), TextAlignmentOptions.Top);
            shortDescText.fontSize = 14;
            shortDescText.color = i == 0 ? new Color(0.72f, 0.72f, 0.72f) : new Color(0.4f, 0.4f, 0.4f);

            // Status text
            string statusLabel = i == 0 ? "▶ Available" : "🔒 Locked";
            Color statusColor = i == 0 ? new Color(0.38f, 1f, 0.42f) : new Color(1f, 0.5f, 0.5f);
            TMP_Text statusText = CreateText(cardBtn.transform, $"Stage {i + 1} Status Text", statusLabel, new Vector2(0, -50), new Vector2(200, 28), TextAlignmentOptions.Center);
            statusText.fontSize = 18;
            statusText.color = statusColor;
            statusTexts[i] = statusText;
        }

        // Description panel — expanded for rich info
        Image descPanel = CreatePanel(canvas.transform, "Description Panel", new Vector2(0, -140), new Vector2(700, 120), new Color(0.06f, 0.06f, 0.10f, 0.88f));
        descPanel.raycastTarget = false;
        TMP_Text stageNameText = CreateText(canvas.transform, "Stage Name Text", "Stage 1-1: Slime Scout", new Vector2(0, -110), new Vector2(660, 28), TextAlignmentOptions.Center);
        stageNameText.fontSize = 22;
        stageNameText.color = new Color(0.92f, 0.78f, 0.38f);
        TMP_Text descText = CreateText(canvas.transform, "Stage Description Text",
            "A basic encounter against slimes.\nLearn the combat basics: Attack, Guard, Fire Skill, and Break.\nDefeat the Slime Scout to advance.\n\nEncounters: Slime → Slime King\nElement: 🔥 Fire | Difficulty: ★\nReward: 50 Gold / 30 XP\nStatus: ▶ Available — Click Start Battle",
            new Vector2(0, -155), new Vector2(660, 80), TextAlignmentOptions.TopLeft);
        descText.fontSize = 16;
        descText.color = new Color(0.82f, 0.82f, 0.92f);

        Button startBattleButton = CreateButton(canvas.transform, "Start Battle Button", "Start Battle", new Vector2(-120, -280), new Vector2(260, 65));
        Button backButton = CreateButton(canvas.transform, "Back Button", "Back", new Vector2(120, -280), new Vector2(260, 65));

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
            StageSelectController controller = Object.FindObjectOfType<StageSelectController>();
            AppendCheck(ref passed, ref report, "StageSelectController exists", controller != null);
            if (controller != null)
            {
                AppendCheck(ref passed, ref report, "Stage 1 Card button exists", controller.DebugStage1CardButtonExists);
                AppendCheck(ref passed, ref report, "Stage 1 Card is interactable (unlocked)", controller.DebugStage1CardInteractable);
                AppendCheck(ref passed, ref report, "Stage 1 status text exists", !string.IsNullOrEmpty(controller.DebugStage1StatusText));
                AppendCheck(ref passed, ref report, "Stage 1 status shows ▶ Available", controller.DebugStage1StatusText == "▶ Available");
                AppendCheck(ref passed, ref report, "Stage 2 Card button exists (unlock-ready)", controller.DebugStage2CardButtonExists);
                AppendCheck(ref passed, ref report, "Stage 2 Card button is non-interactive (locked)", !controller.DebugStage2CardInteractable);
                AppendCheck(ref passed, ref report, "Stage 2 status shows 🔒 Locked", controller.DebugStage2StatusText == "🔒 Locked");
                AppendCheck(ref passed, ref report, "Stage 3 Card button exists", controller.DebugStage3CardButtonExists);
                AppendCheck(ref passed, ref report, "Stage 4 Card button exists", controller.DebugStage4CardButtonExists);
                AppendCheck(ref passed, ref report, "Stage 5 Card button exists", controller.DebugStage5CardButtonExists);
                AppendCheck(ref passed, ref report, "Stage 6 Card button exists", controller.DebugStage6CardButtonExists);
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
