using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Simple title screen manager. Creates UI programmatically and handles scene transitions.
/// </summary>
public class TitleManager : MonoBehaviour
{
    private const string BattleScenePath = "Assets/Scenes/BattleScene.unity";

    private void Awake()
    {
        CreateTitleUI();
        if (AudioManager.Instance != null)
            AudioManager.Instance.CrossfadeTitle();
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

        // Create background with floating particles
        CreateStarParticles(canvasObj.transform);

        // Dark background overlay for readability
        GameObject bgObj = new GameObject("Background");
        bgObj.transform.SetParent(canvasObj.transform, false);
        Image bg = bgObj.AddComponent<Image>();
        bg.color = new Color(0.05f, 0.06f, 0.09f, 0.85f);
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

        // Settings button
        GameObject settingsObj = new GameObject("Settings Button");
        settingsObj.transform.SetParent(canvasObj.transform, false);
        Image settingsImage = settingsObj.AddComponent<Image>();
        settingsImage.color = new Color(0.10f, 0.15f, 0.25f);
        Button settingsBtn = settingsObj.AddComponent<Button>();
        settingsBtn.targetGraphic = settingsImage;
        settingsBtn.onClick.AddListener(OnSettingsClicked);

        RectTransform settingsRt = settingsObj.GetComponent<RectTransform>();
        settingsRt.sizeDelta = new Vector2(200, 40);
        settingsRt.anchoredPosition = new Vector2(0, -215);

        GameObject settingsTextObj = new GameObject("Settings Text");
        settingsTextObj.transform.SetParent(settingsObj.transform, false);
        TMP_Text settingsText = settingsTextObj.AddComponent<TextMeshProUGUI>();
        settingsText.text = "⚙ Settings";
        settingsText.fontSize = 18;
        settingsText.alignment = TextAlignmentOptions.Center;
        settingsText.color = new Color(0.72f, 0.90f, 1.0f);
        RectTransform settingsTextRt = settingsTextObj.GetComponent<RectTransform>();
        settingsTextRt.anchorMin = Vector2.zero;
        settingsTextRt.anchorMax = Vector2.one;
        settingsTextRt.offsetMin = Vector2.zero;
        settingsTextRt.offsetMax = Vector2.zero;

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
        verRt.anchoredPosition = new Vector2(0, -260);
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

    private void OnSettingsClicked()
    {
        var flow = FindFirstObjectByType<GameSceneFlow>();
        if (flow != null)
            flow.LoadSettings();
        else
            SceneManager.LoadScene("SettingsScene");
    }

    // ── Visual polish ──

    private void Start()
    {
        StartCoroutine(TitleAnimationRoutine());
    }

    private IEnumerator TitleAnimationRoutine()
    {
        // Fade in
        CanvasGroup cg = GetComponentInChildren<CanvasGroup>();
        if (cg == null)
        {
            GameObject canvas = GameObject.Find("Title Canvas");
            if (canvas != null) cg = canvas.AddComponent<CanvasGroup>();
        }
        if (cg != null)
        {
            cg.alpha = 0f;
            float elapsed = 0f;
            while (elapsed < 0.5f)
            {
                cg.alpha = Mathf.Lerp(0f, 1f, elapsed / 0.5f);
                elapsed += Time.deltaTime;
                yield return null;
            }
            cg.alpha = 1f;
        }

        // Continuous gentle title float
        Transform titleTransform = GameObject.Find("Title Text")?.transform;
        Transform startBtnTransform = GameObject.Find("Start Button")?.transform;

        while (true)
        {
            float t = Time.time;
            if (titleTransform != null)
            {
                float floatOffset = Mathf.Sin(t * 0.8f) * 4f;
                titleTransform.localPosition = new Vector3(0, 80 + floatOffset, 0);
            }
            if (startBtnTransform != null)
            {
                float glow = 0.85f + 0.15f * (Mathf.Sin(t * 1.2f) * 0.5f + 0.5f);
                Image btnImg = startBtnTransform.GetComponent<Image>();
                if (btnImg != null)
                    btnImg.color = new Color(0.15f * glow, 0.20f * glow, 0.30f * glow);
            }
            yield return new WaitForSeconds(0.05f);
        }
    }

    /// <summary>Creates floating star/dot particles in the background.</summary>
    private void CreateStarParticles(Transform parent)
    {
        for (int i = 0; i < 40; i++)
        {
            GameObject star = new GameObject($"Star_{i}", typeof(RectTransform), typeof(Image));
            star.transform.SetParent(parent, false);
            Image starImg = star.GetComponent<Image>();
            starImg.color = new Color(1f, 1f, 1f, Random.Range(0.1f, 0.4f));
            starImg.raycastTarget = false;

            RectTransform rt = star.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(Random.Range(1.5f, 3f), Random.Range(1.5f, 3f));
            rt.anchoredPosition = new Vector2(
                Random.Range(-960f, 960f),
                Random.Range(-540f, 540f)
            );

            StarParticle sp = star.AddComponent<StarParticle>();
            sp.driftSpeed = Random.Range(2f, 8f);
            sp.twinkleSpeed = Random.Range(0.3f, 1.5f);
            sp.startAlpha = starImg.color.a;
        }
    }
}

/// <summary>Helper for floating star particles on the title screen.</summary>
public class StarParticle : MonoBehaviour
{
    public float driftSpeed = 4f;
    public float twinkleSpeed = 1f;
    public float startAlpha = 0.3f;
    private Image img;
    private Vector3 startPos;

    private void Start()
    {
        img = GetComponent<Image>();
        startPos = transform.localPosition;
    }

    private void Update()
    {
        // Gentle drift
        float driftX = Mathf.Sin(Time.time * driftSpeed * 0.1f + startPos.x * 0.01f) * 8f;
        float driftY = Mathf.Sin(Time.time * driftSpeed * 0.05f + startPos.y * 0.01f) * 5f;
        transform.localPosition = startPos + new Vector3(driftX, driftY, 0);

        // Twinkle
        if (img != null)
        {
            float twinkle = Mathf.Sin(Time.time * twinkleSpeed + startPos.x) * 0.5f + 0.5f;
            img.color = new Color(1f, 1f, 1f, startAlpha * (0.5f + twinkle * 0.5f));
        }
    }
}
