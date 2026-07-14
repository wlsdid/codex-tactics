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

    [MenuItem("Tools/Tactical Requiem/Create Game Flow Scenes")]
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

        Image bgPanel = CreatePanel(canvas.transform, "Background Panel", Vector2.zero, new Vector2(1200, 800), new Color(0.025f, 0.026f, 0.045f, 1f));
        bgPanel.raycastTarget = false;
        CreateTitleShowcaseFrame(canvas.transform);
        CreateTitleCommercialAccents(canvas.transform);
        CreateTitleTrailerStoryboard(canvas.transform);

        TMP_Text titleText = CreateText(canvas.transform, "Title Text", "Tactical Requiem", new Vector2(0, 135), new Vector2(860, 92), TextAlignmentOptions.Center);
        titleText.fontSize = 64;
        titleText.fontStyle = FontStyles.Bold;
        titleText.color = new Color(0.96f, 0.78f, 0.34f);

        TMP_Text subtitleText = CreateText(canvas.transform, "Subtitle Text", "A short tactical break RPG vertical slice", new Vector2(0, 70), new Vector2(720, 40), TextAlignmentOptions.Center);
        subtitleText.fontSize = 24;
        subtitleText.color = new Color(0.76f, 0.90f, 1.0f);

        TMP_Text pitchText = CreateText(canvas.transform, "Title Pitch Text", "Click Hero -> chain skills -> Guard the heavy hit -> finish with rank rewards", new Vector2(0, 28), new Vector2(760, 34), TextAlignmentOptions.Center);
        pitchText.fontSize = 17;
        pitchText.color = new Color(0.90f, 0.92f, 1.0f);

        Button startButton = CreateButton(canvas.transform, "Start Game Button", "Start Game", new Vector2(0, -115), new Vector2(300, 64));
        startButton.GetComponent<Image>().color = new Color(0.20f, 0.15f, 0.08f, 0.96f);

        // Create GameSceneFlow and wire Start button (persistent listener for serialization)
        GameObject flowObject = new GameObject("GameSceneFlow");
        GameSceneFlow flow = flowObject.AddComponent<GameSceneFlow>();
        UnityEventTools.AddPersistentListener(startButton.onClick, flow.LoadStageSelect);

        TacticalTypography.ApplyToLoadedScene();
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
        CreateStageSelectPremiumPreview(canvas.transform);
        CreateStageSelectCampaignBriefing(canvas.transform);

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

            CreateStageCardThumbnail(cardBtn.transform, i, cardElements[i], i == 0);

            // Stage name text
            TMP_Text nameText = CreateText(cardBtn.transform, $"Stage {i + 1} Name Text", cardNames[i], new Vector2(0, 46), new Vector2(190, 24), TextAlignmentOptions.Center);
            nameText.fontSize = 16;
            nameText.fontStyle = FontStyles.Bold;
            nameText.color = i == 0 ? Color.white : new Color(0.58f, 0.58f, 0.62f);

            // Element icon + difficulty text (single line)
            string eleDiffStr = $"{cardElements[i]} {cardDifficulties[i]}";
            TMP_Text eleDiffText = CreateText(cardBtn.transform, $"Stage {i + 1} EleDiff Text", eleDiffStr, new Vector2(-52, -18), new Vector2(86, 22), TextAlignmentOptions.Center);
            eleDiffText.fontSize = 14;
            eleDiffText.fontStyle = FontStyles.Bold;
            eleDiffText.color = i == 0 ? new Color(0.95f, 0.86f, 0.55f) : new Color(0.5f, 0.5f, 0.5f);

            // Description text (short for card preview)
            TMP_Text shortDescText = CreateText(cardBtn.transform, $"Stage {i + 1} Desc Text", cardDescs[i], new Vector2(36, -18), new Vector2(104, 28), TextAlignmentOptions.TopLeft);
            shortDescText.fontSize = 11;
            shortDescText.color = i == 0 ? new Color(0.78f, 0.82f, 0.92f) : new Color(0.42f, 0.42f, 0.46f);

            // Status text
            string statusLabel = i == 0 ? "NEXT" : "LOCKED";
            Color statusColor = i == 0 ? new Color(0.38f, 1f, 0.42f) : new Color(1f, 0.5f, 0.5f);
            TMP_Text statusText = CreateText(cardBtn.transform, $"Stage {i + 1} Status Text", statusLabel, new Vector2(0, -50), new Vector2(190, 22), TextAlignmentOptions.Center);
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

        TacticalTypography.ApplyToLoadedScene();
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, StageSelectScenePath);
    }

    // ── Validate ──

    [MenuItem("Tools/Tactical Requiem/Validate Game Flow Scenes")]
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
            AppendCheck(ref passed, ref report, "TitleScene has premium frame", HasSceneObject("Title Premium Frame Panel"));
            AppendCheck(ref passed, ref report, "TitleScene has battlefield silhouette", HasSceneObject("Title Battlefield Silhouette Panel"));
            AppendCheck(ref passed, ref report, "TitleScene has party silhouette", HasSceneObject("Title Party Silhouette Hero"));
            AppendCheck(ref passed, ref report, "TitleScene has reviewer pitch text", HasSceneObject("Title Pitch Text"));
            AppendCheck(ref passed, ref report, "TitleScene has commercial logo crest and feature chips", HasSceneObject("Title Logo Crest Panel") && HasSceneObject("Title Feature Chip 1 Panel") && HasSceneObject("Title Feature Chip 3 Text"));
            AppendCheck(ref passed, ref report, "TitleScene has trailer storyboard carousel", HasSceneObject("Title Trailer Shot 1 Panel") && HasSceneObject("Title Trailer Shot 3 Text") && HasSceneObject("Title Demo Pillar 2 Panel"));
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
            AppendCheck(ref passed, ref report, "StageSelect has premium map preview rail", HasSceneObject("Stage Select Map Preview Panel") && HasSceneObject("Stage Select Route Line Panel"));
            AppendCheck(ref passed, ref report, "StageSelect has reward chip row", HasSceneObject("Stage Select Reward Gold Chip") && HasSceneObject("Stage Select Reward XP Chip"));
            AppendCheck(ref passed, ref report, "StageSelect keeps the lower detail panel clear of overlapping strategic chips", !HasSceneObject("Stage Select Strategy Strip Panel") && !HasSceneObject("Stage Select Party Loadout Chip") && !HasSceneObject("Stage Select Enemy Forecast Chip"));
            AppendCheck(ref passed, ref report, "StageSelect has commercial campaign briefing", HasSceneObject("Stage Select Campaign Briefing Panel") && HasSceneObject("Stage Select Risk Gauge Fill Panel") && HasSceneObject("Stage Select Sponsor Tag Text"));
            AppendCheck(ref passed, ref report, "StageSelect primary buttons have premium bevel material", HasSceneObject("Start Battle Button Top Highlight") && HasSceneObject("Start Battle Button Gold Edge"));
            AppendCheck(ref passed, ref report, "StageSelect cards have premium bevel material", HasSceneObject("Stage Card 1 Top Highlight") && HasSceneObject("Stage Card 1 Gold Edge"));
            AppendCheck(ref passed, ref report, "StageSelect cards have thumbnail art frames", HasSceneObject("Stage 1 Thumbnail Frame Panel") && HasSceneObject("Stage 1 Thumbnail Sky Panel"));
            AppendCheck(ref passed, ref report, "StageSelect locked cards have dimmed thumbnail treatment", HasSceneObject("Stage 2 Thumbnail Lock Veil Panel"));
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

    private static void CreateTitleShowcaseFrame(Transform parent)
    {
        Image frame = CreatePanel(parent, "Title Premium Frame Panel", new Vector2(0, 0), new Vector2(980, 560), new Color(0.030f, 0.034f, 0.060f, 0.84f));
        frame.raycastTarget = false;
        Image topGold = CreatePanel(parent, "Title Top Gold Divider Panel", new Vector2(0, 224), new Vector2(820, 5), new Color(0.90f, 0.66f, 0.24f, 0.92f));
        Image bottomGold = CreatePanel(parent, "Title Bottom Gold Divider Panel", new Vector2(0, -194), new Vector2(680, 4), new Color(0.78f, 0.50f, 0.18f, 0.84f));
        topGold.raycastTarget = false;
        bottomGold.raycastTarget = false;

        Image battlefield = CreatePanel(parent, "Title Battlefield Silhouette Panel", new Vector2(0, -6), new Vector2(720, 160), new Color(0.018f, 0.040f, 0.040f, 0.70f));
        battlefield.raycastTarget = false;
        Image floorGlow = CreatePanel(parent, "Title Floor Glow Panel", new Vector2(0, -68), new Vector2(560, 46), new Color(0.16f, 0.28f, 0.20f, 0.36f));
        floorGlow.raycastTarget = false;

        Image hero = CreatePanel(parent, "Title Party Silhouette Hero", new Vector2(-142, -24), new Vector2(42, 92), new Color(0.25f, 0.62f, 1.0f, 0.52f));
        Image guardian = CreatePanel(parent, "Title Party Silhouette Guardian", new Vector2(-76, -38), new Vector2(54, 72), new Color(0.38f, 0.80f, 0.66f, 0.42f));
        Image enemy = CreatePanel(parent, "Title Enemy Silhouette Boss", new Vector2(142, -26), new Vector2(92, 104), new Color(0.90f, 0.25f, 0.62f, 0.40f));
        hero.raycastTarget = false;
        guardian.raycastTarget = false;
        enemy.raycastTarget = false;
    }

    private static void CreateTitleCommercialAccents(Transform parent)
    {
        Image logoGlow = CreatePanel(parent, "Title Logo Glow Panel", new Vector2(0, 134), new Vector2(620, 86), new Color(0.95f, 0.62f, 0.20f, 0.12f));
        Image crest = CreatePanel(parent, "Title Logo Crest Panel", new Vector2(0, 202), new Vector2(118, 30), new Color(0.90f, 0.66f, 0.24f, 0.72f));
        Image crestShade = CreatePanel(parent, "Title Logo Crest Shade Panel", new Vector2(0, 196), new Vector2(92, 6), new Color(0.0f, 0.0f, 0.0f, 0.28f));
        Image leftOrnament = CreatePanel(parent, "Title Left Ornament Line", new Vector2(-292, 118), new Vector2(180, 3), new Color(0.78f, 0.54f, 0.20f, 0.52f));
        Image rightOrnament = CreatePanel(parent, "Title Right Ornament Line", new Vector2(292, 118), new Vector2(180, 3), new Color(0.78f, 0.54f, 0.20f, 0.52f));
        logoGlow.raycastTarget = false;
        crest.raycastTarget = false;
        crestShade.raycastTarget = false;
        leftOrnament.raycastTarget = false;
        rightOrnament.raycastTarget = false;

        string[] chipLabels = { "TACTICAL COMMAND", "5 MIN VERTICAL SLICE", "PORTFOLIO READY LOOP" };
        for (int i = 0; i < chipLabels.Length; i++)
        {
            float x = -250f + i * 250f;
            Image chip = CreatePanel(parent, $"Title Feature Chip {i + 1} Panel", new Vector2(x, -164), new Vector2(210, 28), new Color(0.018f, 0.028f, 0.046f, 0.72f));
            Image edge = CreatePanel(parent, $"Title Feature Chip {i + 1} Gold Edge", new Vector2(x, -150), new Vector2(168, 2), new Color(0.92f, 0.70f, 0.34f, 0.48f));
            TMP_Text label = CreateText(parent, $"Title Feature Chip {i + 1} Text", chipLabels[i], new Vector2(x, -164), new Vector2(194, 20), TextAlignmentOptions.Center);
            label.fontSize = 10;
            label.fontStyle = FontStyles.Bold;
            label.color = new Color(0.86f, 0.92f, 1.0f, 0.86f);
            label.raycastTarget = false;
            chip.raycastTarget = false;
            edge.raycastTarget = false;
        }
    }

    private static void CreateTitleTrailerStoryboard(Transform parent)
    {
        Image rail = CreatePanel(parent, "Title Trailer Storyboard Rail Panel", new Vector2(0, -238), new Vector2(760, 74), new Color(0.010f, 0.016f, 0.028f, 0.62f));
        Image railEdge = CreatePanel(parent, "Title Trailer Storyboard Gold Edge", new Vector2(0, -202), new Vector2(680, 2), new Color(0.92f, 0.70f, 0.34f, 0.42f));
        rail.raycastTarget = false;
        railEdge.raycastTarget = false;

        string[] shots = { "01 SELECT", "02 BREAK", "03 RANK" };
        string[] captions = { "Pick route", "Chain AP burst", "Claim reward" };
        Color[] colors =
        {
            new Color(0.25f, 0.54f, 1.0f, 0.22f),
            new Color(1.0f, 0.42f, 0.26f, 0.24f),
            new Color(0.95f, 0.76f, 0.34f, 0.24f)
        };

        for (int i = 0; i < shots.Length; i++)
        {
            float x = -248f + i * 248f;
            Image shot = CreatePanel(parent, $"Title Trailer Shot {i + 1} Panel", new Vector2(x, -238), new Vector2(202, 52), new Color(0.018f, 0.024f, 0.040f, 0.78f));
            Image glow = CreatePanel(parent, $"Title Trailer Shot {i + 1} Glow Panel", new Vector2(x - 56, -238), new Vector2(54, 38), colors[i]);
            TMP_Text label = CreateText(parent, $"Title Trailer Shot {i + 1} Text", shots[i] + " / " + captions[i], new Vector2(x + 12, -238), new Vector2(164, 22), TextAlignmentOptions.Center);
            label.fontSize = 10;
            label.fontStyle = FontStyles.Bold;
            label.color = new Color(0.88f, 0.94f, 1.0f, 0.86f);
            shot.raycastTarget = false;
            glow.raycastTarget = false;
            label.raycastTarget = false;
        }

        for (int i = 0; i < 3; i++)
        {
            Image pillar = CreatePanel(parent, $"Title Demo Pillar {i + 1} Panel", new Vector2(-84 + i * 84, -74), new Vector2(38, 76 + i * 10), new Color(0.95f, 0.70f, 0.30f, 0.10f));
            pillar.raycastTarget = false;
        }
    }

    private static void CreateStageCardThumbnail(Transform parent, int index, string elementLabel, bool unlocked)
    {
        Color elementColor = GetStageElementColor(elementLabel);
        Image frame = CreatePanel(parent, $"Stage {index + 1} Thumbnail Frame Panel", new Vector2(0, 7), new Vector2(188, 58), new Color(0.015f, 0.018f, 0.030f, 0.88f));
        Image sky = CreatePanel(parent, $"Stage {index + 1} Thumbnail Sky Panel", new Vector2(0, 13), new Vector2(176, 34), new Color(elementColor.r, elementColor.g, elementColor.b, unlocked ? 0.24f : 0.10f));
        Image ground = CreatePanel(parent, $"Stage {index + 1} Thumbnail Ground Panel", new Vector2(0, -10), new Vector2(176, 14), new Color(0.04f, 0.05f, 0.055f, unlocked ? 0.88f : 0.55f));
        Image accent = CreatePanel(parent, $"Stage {index + 1} Thumbnail Element Accent Panel", new Vector2(-74, 12), new Vector2(22, 28), new Color(elementColor.r, elementColor.g, elementColor.b, unlocked ? 0.82f : 0.32f));
        frame.raycastTarget = false;
        sky.raycastTarget = false;
        ground.raycastTarget = false;
        accent.raycastTarget = false;

        if (!unlocked)
        {
            Image veil = CreatePanel(parent, $"Stage {index + 1} Thumbnail Lock Veil Panel", new Vector2(0, 7), new Vector2(188, 58), new Color(0.0f, 0.0f, 0.0f, 0.38f));
            veil.raycastTarget = false;
        }
    }

    private static Color GetStageElementColor(string elementLabel)
    {
        switch (elementLabel)
        {
            case "FIRE": return new Color(1.0f, 0.38f, 0.18f, 1f);
            case "NAT": return new Color(0.30f, 0.82f, 0.42f, 1f);
            case "EARTH": return new Color(0.75f, 0.55f, 0.28f, 1f);
            case "LIT": return new Color(0.95f, 0.88f, 0.24f, 1f);
            case "DARK": return new Color(0.55f, 0.28f, 0.88f, 1f);
            case "LIGHT": return new Color(0.95f, 0.88f, 0.62f, 1f);
            default: return new Color(0.55f, 0.72f, 1.0f, 1f);
        }
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

    private static void CreateStageSelectPremiumPreview(Transform parent)
    {
        Image mapPanel = CreatePanel(parent, "Stage Select Map Preview Panel", new Vector2(-430, -196), new Vector2(230, 118), new Color(0.014f, 0.020f, 0.034f, 0.86f));
        Image mapSky = CreatePanel(parent, "Stage Select Map Sky Glow Panel", new Vector2(-430, -178), new Vector2(202, 62), new Color(0.12f, 0.22f, 0.38f, 0.48f));
        Image routeLine = CreatePanel(parent, "Stage Select Route Line Panel", new Vector2(-430, -216), new Vector2(170, 3), new Color(0.95f, 0.72f, 0.36f, 0.74f));
        TMP_Text mapTitle = CreateText(parent, "Stage Select Map Preview Label Text", "SCOUT ROUTE", new Vector2(-430, -147), new Vector2(198, 20), TextAlignmentOptions.Center);
        mapTitle.fontSize = 12;
        mapTitle.fontStyle = FontStyles.Bold;
        mapTitle.color = new Color(0.90f, 0.84f, 0.58f);

        Image goldChip = CreatePanel(parent, "Stage Select Reward Gold Chip", new Vector2(330, -154), new Vector2(150, 30), new Color(0.18f, 0.12f, 0.05f, 0.88f));
        TMP_Text goldText = CreateText(parent, "Stage Select Reward Gold Text", "GOLD 100-150", new Vector2(330, -154), new Vector2(136, 22), TextAlignmentOptions.Center);
        goldText.fontSize = 12;
        goldText.color = new Color(1.0f, 0.82f, 0.42f);

        Image xpChip = CreatePanel(parent, "Stage Select Reward XP Chip", new Vector2(330, -190), new Vector2(150, 30), new Color(0.05f, 0.12f, 0.20f, 0.88f));
        TMP_Text xpText = CreateText(parent, "Stage Select Reward XP Text", "XP 80", new Vector2(330, -190), new Vector2(136, 22), TextAlignmentOptions.Center);
        xpText.fontSize = 12;
        xpText.color = new Color(0.72f, 0.90f, 1.0f);

        Image modifierChip = CreatePanel(parent, "Stage Select Modifier Chip", new Vector2(330, -226), new Vector2(150, 30), new Color(0.10f, 0.07f, 0.16f, 0.88f));
        TMP_Text modifierText = CreateText(parent, "Stage Select Modifier Text", "FIELD: TUTORIAL", new Vector2(330, -226), new Vector2(136, 22), TextAlignmentOptions.Center);
        modifierText.fontSize = 11;
        modifierText.color = new Color(0.86f, 0.78f, 1.0f);

        mapPanel.raycastTarget = false;
        mapSky.raycastTarget = false;
        routeLine.raycastTarget = false;
        goldChip.raycastTarget = false;
        xpChip.raycastTarget = false;
        modifierChip.raycastTarget = false;
    }

    private static void CreateStageSelectCampaignBriefing(Transform parent)
    {
        Image panel = CreatePanel(parent, "Stage Select Campaign Briefing Panel", new Vector2(0, -105), new Vector2(812, 34), new Color(0.012f, 0.018f, 0.032f, 0.58f));
        Image leftEdge = CreatePanel(parent, "Stage Select Campaign Briefing Left Edge", new Vector2(-404, -105), new Vector2(3, 28), new Color(0.90f, 0.68f, 0.32f, 0.52f));
        TMP_Text label = CreateText(parent, "Stage Select Sponsor Tag Text", "CLIENT BRIEF / polished short demo: tactical choice -> break -> ranked reward", new Vector2(-136, -105), new Vector2(500, 18), TextAlignmentOptions.Left);
        label.fontSize = 11;
        label.fontStyle = FontStyles.Bold;
        label.color = new Color(0.86f, 0.92f, 1.0f, 0.84f);

        Image riskBack = CreatePanel(parent, "Stage Select Risk Gauge Back Panel", new Vector2(292, -105), new Vector2(170, 10), new Color(0.04f, 0.04f, 0.052f, 0.82f));
        Image riskFill = CreatePanel(parent, "Stage Select Risk Gauge Fill Panel", new Vector2(248, -105), new Vector2(82, 6), new Color(1.0f, 0.58f, 0.30f, 0.78f));
        TMP_Text riskText = CreateText(parent, "Stage Select Risk Gauge Text", "RISK 48%", new Vector2(388, -105), new Vector2(82, 16), TextAlignmentOptions.Left);
        riskText.fontSize = 10;
        riskText.color = new Color(1.0f, 0.78f, 0.52f, 0.90f);

        panel.raycastTarget = false;
        leftEdge.raycastTarget = false;
        riskBack.raycastTarget = false;
        riskFill.raycastTarget = false;
        label.raycastTarget = false;
        riskText.raycastTarget = false;
    }

    private static void CreateStageSelectStrategicInfoStrip(Transform parent)
    {
        Image strip = CreatePanel(parent, "Stage Select Strategy Strip Panel", new Vector2(0, -285), new Vector2(760, 44), new Color(0.014f, 0.020f, 0.034f, 0.78f));
        Image topEdge = CreatePanel(parent, "Stage Select Strategy Strip Top Edge", new Vector2(0, -263), new Vector2(680, 2), new Color(0.86f, 0.64f, 0.28f, 0.46f));
        strip.raycastTarget = false;
        topEdge.raycastTarget = false;

        CreateStrategyChip(parent, "Stage Select Party Loadout", "PARTY / PALADIN + MAGE + GUARD", new Vector2(-250, -285), new Color(0.06f, 0.12f, 0.20f, 0.86f), new Color(0.42f, 0.74f, 1.0f, 0.52f));
        CreateStrategyChip(parent, "Stage Select Enemy Forecast", "ENEMY / SLIME KING + BREAK", new Vector2(0, -285), new Color(0.16f, 0.06f, 0.08f, 0.86f), new Color(1.0f, 0.44f, 0.52f, 0.52f));
        CreateStrategyChip(parent, "Stage Select Clear Target", "TARGET / A RANK + 150G", new Vector2(250, -285), new Color(0.15f, 0.10f, 0.04f, 0.86f), new Color(1.0f, 0.76f, 0.34f, 0.52f));
    }

    private static void CreateStrategyChip(Transform parent, string baseName, string label, Vector2 position, Color bgColor, Color edgeColor)
    {
        Image chip = CreatePanel(parent, baseName + " Chip", position, new Vector2(224, 28), bgColor);
        Image edge = CreatePanel(parent, baseName + " Edge", new Vector2(position.x - 108, position.y), new Vector2(3, 20), edgeColor);
        TMP_Text text = CreateText(parent, baseName + " Text", label, position, new Vector2(204, 18), TextAlignmentOptions.Center);
        text.fontSize = 9;
        text.fontStyle = FontStyles.Bold;
        text.color = new Color(0.88f, 0.92f, 1.0f, 0.88f);
        text.raycastTarget = false;
        chip.raycastTarget = false;
        edge.raycastTarget = false;
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
        img.color = new Color(0.075f, 0.082f, 0.120f, 0.96f);

        Image topHighlight = CreatePanel(obj.transform, name + " Top Highlight", new Vector2(0, size.y * 0.34f), new Vector2(Mathf.Max(8f, size.x - 18f), 3f), new Color(1.0f, 0.84f, 0.48f, 0.42f));
        Image bottomShade = CreatePanel(obj.transform, name + " Bottom Shade", new Vector2(0, -size.y * 0.34f), new Vector2(Mathf.Max(8f, size.x - 16f), 4f), new Color(0.0f, 0.0f, 0.0f, 0.36f));
        Image goldEdge = CreatePanel(obj.transform, name + " Gold Edge", new Vector2(0, 0), new Vector2(Mathf.Max(8f, size.x - 12f), 2f), new Color(0.92f, 0.66f, 0.28f, 0.34f));
        topHighlight.raycastTarget = false;
        bottomShade.raycastTarget = false;
        goldEdge.raycastTarget = false;

        // Button text as child
        GameObject textObj = new GameObject("Button Text", typeof(RectTransform), typeof(TextMeshProUGUI));
        textObj.transform.SetParent(obj.transform, false);
        RectTransform textRt = textObj.GetComponent<RectTransform>();
        textRt.anchorMin = Vector2.zero;
        textRt.anchorMax = Vector2.one;
        textRt.sizeDelta = Vector2.zero;
        TMP_Text buttonText = textObj.GetComponent<TextMeshProUGUI>();
        buttonText.text = label;
        buttonText.fontSize = size.y <= 56f ? 20 : 24;
        buttonText.fontStyle = FontStyles.Bold;
        buttonText.color = new Color(0.96f, 0.90f, 0.72f);
        buttonText.alignment = TextAlignmentOptions.Center;

        Button btn = obj.GetComponent<Button>();
        btn.targetGraphic = img;
        ColorBlock cb = btn.colors;
        cb.normalColor = new Color(0.075f, 0.082f, 0.120f, 0.96f);
        cb.highlightedColor = new Color(0.20f, 0.16f, 0.09f, 0.98f);
        cb.pressedColor = new Color(0.045f, 0.050f, 0.080f, 1.0f);
        cb.disabledColor = new Color(0.06f, 0.07f, 0.10f, 0.45f);
        btn.colors = cb;
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

        Image topHighlight = CreatePanel(obj.transform, name + " Top Highlight", new Vector2(0, size.y * 0.40f), new Vector2(Mathf.Max(8f, size.x - 20f), 3f), new Color(1.0f, 0.82f, 0.42f, 0.34f));
        Image bottomShade = CreatePanel(obj.transform, name + " Bottom Shade", new Vector2(0, -size.y * 0.40f), new Vector2(Mathf.Max(8f, size.x - 18f), 4f), new Color(0.0f, 0.0f, 0.0f, 0.32f));
        Image goldEdge = CreatePanel(obj.transform, name + " Gold Edge", new Vector2(0, 0), new Vector2(Mathf.Max(8f, size.x - 14f), 2f), new Color(0.92f, 0.66f, 0.28f, 0.24f));
        topHighlight.raycastTarget = false;
        bottomShade.raycastTarget = false;
        goldEdge.raycastTarget = false;

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
