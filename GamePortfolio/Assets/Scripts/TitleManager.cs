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

        // Hero title text
        GameObject heroTitleObj = new GameObject("Hero Title");
        heroTitleObj.transform.SetParent(canvasObj.transform, false);
        TMP_Text heroTitle = heroTitleObj.AddComponent<TextMeshProUGUI>();
        heroTitle.text = $"★ {PlaceholderSpriteGenerator.HeroName} — {PlaceholderSpriteGenerator.HeroTitle} ★";
        heroTitle.fontSize = 18;
        heroTitle.fontStyle = FontStyles.Italic;
        heroTitle.alignment = TextAlignmentOptions.Center;
        heroTitle.color = new Color(0.50f, 0.75f, 1.0f);
        RectTransform heroRt = heroTitleObj.GetComponent<RectTransform>();
        heroRt.sizeDelta = new Vector2(800, 30);
        heroRt.anchoredPosition = new Vector2(0, -5);

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
        subRt.anchoredPosition = new Vector2(0, -40);

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

        // Reset Progress button
        GameObject resetObj = new GameObject("Reset Button");
        resetObj.transform.SetParent(canvasObj.transform, false);
        Image resetImage = resetObj.AddComponent<Image>();
        resetImage.color = new Color(0.12f, 0.12f, 0.18f);
        Button resetBtn = resetObj.AddComponent<Button>();
        resetBtn.targetGraphic = resetImage;
        resetBtn.onClick.AddListener(OnResetProgress);

        RectTransform resetRt = resetObj.GetComponent<RectTransform>();
        resetRt.sizeDelta = new Vector2(200, 40);
        resetRt.anchoredPosition = new Vector2(0, -120);

        GameObject resetTextObj = new GameObject("Reset Text");
        resetTextObj.transform.SetParent(resetObj.transform, false);
        TMP_Text resetText = resetTextObj.AddComponent<TextMeshProUGUI>();
        resetText.text = "Reset Progress";
        resetText.fontSize = 18;
        resetText.alignment = TextAlignmentOptions.Center;
        resetText.color = new Color(0.65f, 0.30f, 0.30f);
        RectTransform resetTextRt = resetTextObj.GetComponent<RectTransform>();
        resetTextRt.anchorMin = Vector2.zero;
        resetTextRt.anchorMax = Vector2.one;
        resetTextRt.offsetMin = Vector2.zero;
        resetTextRt.offsetMax = Vector2.zero;

        // Difficulty toggle
        GameObject diffObj = new GameObject("Difficulty Button");
        diffObj.transform.SetParent(canvasObj.transform, false);
        Image diffImage = diffObj.AddComponent<Image>();
        diffImage.color = new Color(0.12f, 0.15f, 0.22f);
        Button diffBtn = diffObj.AddComponent<Button>();
        diffBtn.targetGraphic = diffImage;
        diffBtn.onClick.AddListener(OnToggleDifficulty);

        RectTransform diffRt = diffObj.GetComponent<RectTransform>();
        diffRt.sizeDelta = new Vector2(200, 40);
        diffRt.anchoredPosition = new Vector2(0, -170);

        GameObject diffTextObj = new GameObject("Difficulty Text");
        diffTextObj.transform.SetParent(diffObj.transform, false);
        TMP_Text diffText = diffTextObj.AddComponent<TextMeshProUGUI>();
        diffText.name = "Difficulty Label";
        diffText.text = $"Mode: {ProgressState.DifficultyLabel}";
        diffText.fontSize = 18;
        diffText.alignment = TextAlignmentOptions.Center;
        diffText.color = new Color(0.72f, 0.90f, 1.0f);
        RectTransform diffTextRt = diffTextObj.GetComponent<RectTransform>();
        diffTextRt.anchorMin = Vector2.zero;
        diffTextRt.anchorMax = Vector2.one;
        diffTextRt.offsetMin = Vector2.zero;
        diffTextRt.offsetMax = Vector2.zero;

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
        SaveManager.Load();
        SceneManager.LoadScene("StageSelectScene");
    }

    private void OnResetProgress()
    {
        SaveManager.ResetSave();
    }

    private void OnToggleDifficulty()
    {
        ProgressState.DifficultyMode = ProgressState.DifficultyMode == 0 ? 1 : 0;
        // Update the button label
        var label = GameObject.Find("Difficulty Label")?.GetComponent<TMPro.TMP_Text>();
        if (label != null) label.text = $"Mode: {ProgressState.DifficultyLabel}";
    }
}
