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

public static class StageSelectSceneAutoBuilder
{
    private const string ScenePath = "Assets/Scenes/StageSelectScene.unity";

    [MenuItem("Tools/Codex Tactics/Create Stage Select Scene")]
    public static void CreateStageSelectScene()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        scene.name = "StageSelectScene";

        Camera camera = CreateCamera();
        Canvas canvas = CreateCanvas(camera);
        CreateEventSystem();

        // Title
        TMP_Text titleText = CreateText(canvas.transform, "Title Text", "Select Stage", new Vector2(0, 240), new Vector2(800, 60), TextAlignmentOptions.Center);
        titleText.fontSize = 40;

        // Stage cards — 6 cards in 2 rows of 3
        int cardCount = 6;
        float cardStartX = -320f;
        float cardY = 165f;
        float cardSpacingX = 320f;
        float cardSpacingY = -190f;
        Vector2 cardSize = new Vector2(280, 170);

        Button[] cardButtons = new Button[cardCount];
        Image[] cardBgs = new Image[cardCount];
        TMP_Text[] cardLabels = new TMP_Text[cardCount];
        TMP_Text[] statusTexts = new TMP_Text[cardCount];

        for (int i = 0; i < cardCount; i++)
        {
            int row = i / 3;
            int col = i % 3;
            float x = cardStartX + col * cardSpacingX;
            float y = cardY + row * cardSpacingY;

            Image panel = CreatePanel(canvas.transform, $"Stage Card {i + 1}", new Vector2(x, y), cardSize, new Color(0.07f, 0.07f, 0.12f, 0.92f));
            cardBgs[i] = panel;

            Button btn = panel.gameObject.AddComponent<Button>();
            btn.targetGraphic = panel;
            cardButtons[i] = btn;
            btn.interactable = i == 0; // Only first unlocked by default

            string[] cardLabelsData = { "Stage 1\nSlime", "Stage 2\nWolf", "Stage 3\nGolem", "Stage 4\nStorm", "Stage 5\nShadow", "Stage 6\nLight" };
            TMP_Text label = CreateText(canvas.transform, $"Stage Label {i + 1}", cardLabelsData[i], new Vector2(x, y + 25), new Vector2(260, 60), TextAlignmentOptions.Center);
            label.fontSize = 22;
            label.color = new Color(0.92f, 0.86f, 0.55f);
            cardLabels[i] = label;

            TMP_Text status = CreateText(canvas.transform, $"Stage Status {i + 1}", i == 0 ? "Available" : "Locked", new Vector2(x, y - 55), new Vector2(260, 30), TextAlignmentOptions.Center);
            status.fontSize = 16;
            status.color = new Color(0.72f, 0.90f, 1.0f);
            statusTexts[i] = status;
        }

        // Description panel
        Image descPanel = CreatePanel(canvas.transform, "Description Panel", new Vector2(0, -180), new Vector2(700, 90), new Color(0.06f, 0.07f, 0.10f, 0.86f));
        descPanel.raycastTarget = false;
        TMP_Text stageNameText = CreateText(canvas.transform, "Stage Name Text", "Select a stage", new Vector2(0, -160), new Vector2(660, 30), TextAlignmentOptions.Center);
        stageNameText.fontSize = 22;
        stageNameText.color = new Color(0.92f, 0.86f, 0.55f);
        TMP_Text stageDescText = CreateText(canvas.transform, "Stage Description Text", "", new Vector2(0, -190), new Vector2(660, 60), TextAlignmentOptions.TopLeft);
        stageDescText.fontSize = 16;
        stageDescText.color = new Color(0.82f, 0.86f, 0.95f);

        // Buttons
        Button startButton = CreateButton(canvas.transform, "Start Battle Button", "Start Battle", new Vector2(-160, -285), new Vector2(260, 60));
        startButton.interactable = false;
        Button backButton = CreateButton(canvas.transform, "Back Button", "Back to Title", new Vector2(160, -285), new Vector2(260, 60));

        // StageSelectController
        GameObject controllerObj = new GameObject("StageSelectController");
        StageSelectController controller = controllerObj.AddComponent<StageSelectController>();

        SerializedObject serialized = new SerializedObject(controller);
        SetRef(serialized, "stage1CardButton", cardButtons[0]);
        SetRef(serialized, "stage2CardButton", cardButtons[1]);
        SetRef(serialized, "stage3CardButton", cardButtons[2]);
        SetRef(serialized, "stage4CardButton", cardButtons[3]);
        SetRef(serialized, "stage5CardButton", cardButtons[4]);
        SetRef(serialized, "stage6CardButton", cardButtons[5]);
        SetRef(serialized, "stage1CardBg", cardBgs[0]);
        SetRef(serialized, "stage2CardBg", cardBgs[1]);
        SetRef(serialized, "stage3CardBg", cardBgs[2]);
        SetRef(serialized, "stage4CardBg", cardBgs[3]);
        SetRef(serialized, "stage5CardBg", cardBgs[4]);
        SetRef(serialized, "stage6CardBg", cardBgs[5]);
        SetRef(serialized, "stage2StatusText", statusTexts[1]);
        SetRef(serialized, "stage3StatusText", statusTexts[2]);
        SetRef(serialized, "stage4StatusText", statusTexts[3]);
        SetRef(serialized, "stage5StatusText", statusTexts[4]);
        SetRef(serialized, "stage6StatusText", statusTexts[5]);
        SetRef(serialized, "stageNameText", stageNameText);
        SetRef(serialized, "stageDescriptionText", stageDescText);
        SetRef(serialized, "startBattleButton", startButton);
        SetRef(serialized, "backButton", backButton);
        serialized.ApplyModifiedPropertiesWithoutUndo();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, ScenePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Selection.activeGameObject = controllerObj;
        EditorUtility.DisplayDialog("StageSelectScene Created",
            "Assets/Scenes/StageSelectScene.unity created!\n\nAdd to Build Settings for scene transitions to work.", "OK");
    }

    // --- Helpers ---

    private static Camera CreateCamera()
    {
        GameObject obj = new GameObject("Main Camera");
        Camera cam = obj.AddComponent<Camera>();
        cam.tag = "MainCamera";
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.05f, 0.06f, 0.09f);
        cam.orthographic = true;
        cam.orthographicSize = 5.0f;
        cam.transform.position = new Vector3(0, 0, -10);
        return cam;
    }

    private static Canvas CreateCanvas(Camera camera)
    {
        GameObject obj = new GameObject("Canvas");
        Canvas canvas = obj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = camera;
        canvas.sortingOrder = 100;
        CanvasScaler scaler = obj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        obj.AddComponent<GraphicRaycaster>();
        return canvas;
    }

    private static void CreateEventSystem()
    {
        if (Object.FindObjectOfType<EventSystem>() == null)
        {
            GameObject obj = new GameObject("EventSystem");
            obj.AddComponent<EventSystem>();
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
            obj.AddComponent<InputSystemUIInputModule>();
#else
            obj.AddComponent<StandaloneInputModule>();
#endif
        }
    }

    private static TMP_Text CreateText(Transform parent, string name, string text, Vector2 pos, Vector2 size, TextAlignmentOptions align)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        TMP_Text tmp = obj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 18;
        tmp.alignment = align;
        tmp.color = Color.white;
        RectTransform rt = obj.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        return tmp;
    }

    private static Image CreatePanel(Transform parent, string name, Vector2 pos, Vector2 size, Color color)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        Image img = obj.AddComponent<Image>();
        img.color = color;
        img.raycastTarget = true;
        RectTransform rt = obj.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        return img;
    }

    private static Button CreateButton(Transform parent, string name, string label, Vector2 pos, Vector2 size)
    {
        Image panel = CreatePanel(parent, name, pos, size, new Color(0.15f, 0.20f, 0.30f));
        Button btn = panel.gameObject.AddComponent<Button>();
        btn.targetGraphic = panel;

        TMP_Text text = CreateText(panel.transform, $"{name} Label", label, Vector2.zero, size, TextAlignmentOptions.Center);
        text.fontSize = 22;
        text.color = Color.white;

        return btn;
    }

    private static void SetRef(SerializedObject so, string field, Object value)
    {
        var prop = so.FindProperty(field);
        if (prop != null) prop.objectReferenceValue = value;
    }
}
