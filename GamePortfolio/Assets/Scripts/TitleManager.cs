using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Simple title screen manager. Creates UI programmatically and handles scene transitions.
/// </summary>
public class TitleManager : MonoBehaviour
{
    private const string BattleScenePath = "Assets/Scenes/BattleScene.unity";

    private void Awake()
    {
        CreateTitleUI();
    }

    private void CreateTitleUI()
    {
        // Create Canvas
        GameObject canvasObj = new GameObject("Title Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasObj.AddComponent<GraphicRaycaster>();

        // Dark background
        GameObject bgObj = new GameObject("Background");
        bgObj.transform.SetParent(canvasObj.transform, false);
        Image bg = bgObj.AddComponent<Image>();
        bg.color = new Color(0.05f, 0.06f, 0.09f, 1f);
        RectTransform bgRt = bgObj.GetComponent<RectTransform>();
        bgRt.anchorMin = Vector2.zero;
        bgRt.anchorMax = Vector2.one;
        bgRt.offsetMin = Vector2.zero;
        bgRt.offsetMax = Vector2.zero;

        // Title text
        GameObject titleObj = new GameObject("Title Text");
        titleObj.transform.SetParent(canvasObj.transform, false);
        TMP_Text title = titleObj.AddComponent<TextMeshProUGUI>();
        title.text = "Codex Tactics";
        title.fontSize = 56;
        title.fontStyle = FontStyles.Bold;
        title.alignment = TextAlignmentOptions.Center;
        title.color = new Color(0.92f, 0.86f, 0.55f);
        RectTransform titleRt = titleObj.GetComponent<RectTransform>();
        titleRt.sizeDelta = new Vector2(800, 80);
        titleRt.anchoredPosition = new Vector2(0, 80);

        // Subtitle
        GameObject subObj = new GameObject("Subtitle Text");
        subObj.transform.SetParent(canvasObj.transform, false);
        TMP_Text subtitle = subObj.AddComponent<TextMeshProUGUI>();
        subtitle.text = "A 2D Turn-Based RPG Prototype";
        subtitle.fontSize = 22;
        subtitle.alignment = TextAlignmentOptions.Center;
        subtitle.color = new Color(0.72f, 0.90f, 1.0f);
        RectTransform subRt = subObj.GetComponent<RectTransform>();
        subRt.sizeDelta = new Vector2(600, 40);
        subRt.anchoredPosition = new Vector2(0, 30);

        // Start button
        GameObject btnObj = new GameObject("Start Button");
        btnObj.transform.SetParent(canvasObj.transform, false);
        Image btnImage = btnObj.AddComponent<Image>();
        btnImage.color = new Color(0.15f, 0.20f, 0.30f);
        Button btn = btnObj.AddComponent<Button>();
        btn.targetGraphic = btnImage;
        btn.onClick.AddListener(OnStartGame);

        RectTransform btnRt = btnObj.GetComponent<RectTransform>();
        btnRt.sizeDelta = new Vector2(260, 60);
        btnRt.anchoredPosition = new Vector2(0, -60);

        GameObject btnTextObj = new GameObject("Button Text");
        btnTextObj.transform.SetParent(btnObj.transform, false);
        TMP_Text btnText = btnTextObj.AddComponent<TextMeshProUGUI>();
        btnText.text = "Start Game";
        btnText.fontSize = 26;
        btnText.alignment = TextAlignmentOptions.Center;
        btnText.color = Color.white;
        RectTransform btnTextRt = btnTextObj.GetComponent<RectTransform>();
        btnTextRt.anchorMin = Vector2.zero;
        btnTextRt.anchorMax = Vector2.one;
        btnTextRt.offsetMin = Vector2.zero;
        btnTextRt.offsetMax = Vector2.zero;

        // Version text
        GameObject verObj = new GameObject("Version Text");
        verObj.transform.SetParent(canvasObj.transform, false);
        TMP_Text version = verObj.AddComponent<TextMeshProUGUI>();
        version.text = "v0.2 · Portfolio Prototype";
        version.fontSize = 14;
        version.alignment = TextAlignmentOptions.Center;
        version.color = new Color(0.45f, 0.50f, 0.60f);
        RectTransform verRt = verObj.GetComponent<RectTransform>();
        verRt.sizeDelta = new Vector2(400, 30);
        verRt.anchoredPosition = new Vector2(0, -200);
    }

    private void OnStartGame()
    {
        // Load battle scene by name — must be in Build Settings
        SceneManager.LoadScene("BattleScene");
    }
}
