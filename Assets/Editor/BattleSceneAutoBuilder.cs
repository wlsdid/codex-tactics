using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

        TMP_Text playerHpText = CreateText(canvas.transform, "Player HP Text", "Hero HP: 100/100", new Vector2(40, -40), new Vector2(360, 50), TextAlignmentOptions.Left);
        TMP_Text playerApText = CreateText(canvas.transform, "Player AP Text", "AP: 3/3", new Vector2(40, -95), new Vector2(260, 45), TextAlignmentOptions.Left);
        TMP_Text enemyHpText = CreateText(canvas.transform, "Enemy HP Text", "Slime HP: 80/80", new Vector2(-40, -40), new Vector2(360, 50), TextAlignmentOptions.Right);
        TMP_Text enemyStatusText = CreateText(canvas.transform, "Enemy Status Text", "Status: None", new Vector2(-40, -95), new Vector2(360, 45), TextAlignmentOptions.Right);
        TMP_Text messageText = CreateText(canvas.transform, "Message Text", "전투 시작!", new Vector2(0, -230), new Vector2(720, 90), TextAlignmentOptions.Center);

        Button attackButton = CreateButton(canvas.transform, "Attack Button", "Attack", new Vector2(-230, 90), new Vector2(180, 60));
        Button fireSkillButton = CreateButton(canvas.transform, "Fire Skill Button", "Fire Skill", new Vector2(0, 90), new Vector2(180, 60));
        Button endTurnButton = CreateButton(canvas.transform, "End Turn Button", "End Turn", new Vector2(230, 90), new Vector2(180, 60));

        GameObject battleManagerObject = new GameObject("BattleManager");
        BattleManager battleManager = battleManagerObject.AddComponent<BattleManager>();

        SerializedObject serializedBattleManager = new SerializedObject(battleManager);
        SetObjectReference(serializedBattleManager, "playerHpText", playerHpText);
        SetObjectReference(serializedBattleManager, "playerApText", playerApText);
        SetObjectReference(serializedBattleManager, "enemyHpText", enemyHpText);
        SetObjectReference(serializedBattleManager, "enemyStatusText", enemyStatusText);
        SetObjectReference(serializedBattleManager, "messageText", messageText);
        SetObjectReference(serializedBattleManager, "attackButton", attackButton);
        SetObjectReference(serializedBattleManager, "fireSkillButton", fireSkillButton);
        SetObjectReference(serializedBattleManager, "endTurnButton", endTurnButton);
        serializedBattleManager.ApplyModifiedPropertiesWithoutUndo();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, ScenePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Selection.activeGameObject = battleManagerObject;
        EditorUtility.DisplayDialog(
            "BattleScene 생성 완료",
            "Assets/Scenes/BattleScene.unity 생성 완료!\n\nPlay 버튼을 눌러 Attack / Fire Skill / End Turn을 테스트하세요.",
            "확인"
        );
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
        eventSystemObject.AddComponent<StandaloneInputModule>();
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
