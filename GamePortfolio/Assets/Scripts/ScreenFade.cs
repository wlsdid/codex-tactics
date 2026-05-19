using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages screen fade transitions (black fade in/out) between scenes or encounters.
/// Singleton pattern for easy access. Attach to a persistent GameObject.
/// </summary>
public class ScreenFade : MonoBehaviour
{
    private Image fadeImage;
    private static ScreenFade instance;
    public static ScreenFade Instance => instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        // Create full-screen black overlay
        GameObject canvasObj = new GameObject("Fade Canvas");
        canvasObj.transform.SetParent(transform);
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        GameObject imgObj = new GameObject("Fade Image");
        imgObj.transform.SetParent(canvasObj.transform, false);
        fadeImage = imgObj.AddComponent<Image>();
        fadeImage.color = Color.black;
        fadeImage.raycastTarget = false;
        RectTransform rt = imgObj.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        fadeImage.gameObject.SetActive(false);
    }

    public void FadeOut(float duration, System.Action onComplete)
    {
        StartCoroutine(FadeRoutine(0f, 1f, duration, onComplete));
    }

    public void FadeIn(float duration, System.Action onComplete)
    {
        StartCoroutine(FadeRoutine(1f, 0f, duration, onComplete));
    }

    /// <summary>Fade to black, load a scene, then fade back in.</summary>
    public void TransitionToScene(string sceneName, float fadeDuration = 0.5f)
    {
        StartCoroutine(TransitionRoutine(sceneName, fadeDuration));
    }

    private IEnumerator TransitionRoutine(string sceneName, float duration)
    {
        fadeImage.gameObject.SetActive(true);
        // Fade to black
        float elapsed = 0f;
        while (elapsed < duration * 0.5f)
        {
            float alpha = Mathf.Lerp(0f, 1f, elapsed / (duration * 0.5f));
            fadeImage.color = new Color(0f, 0f, 0f, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }
        fadeImage.color = new Color(0f, 0f, 0f, 1f);

        // Load scene
        SceneManager.LoadScene(sceneName);

        // Fade back in
        elapsed = 0f;
        while (elapsed < duration * 0.5f)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsed / (duration * 0.5f));
            fadeImage.color = new Color(0f, 0f, 0f, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }
        fadeImage.color = new Color(0f, 0f, 0f, 0f);
        fadeImage.gameObject.SetActive(false);
    }

    private IEnumerator FadeRoutine(float from, float to, float duration, System.Action onComplete)
    {
        fadeImage.gameObject.SetActive(true);
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float alpha = Mathf.Lerp(from, to, elapsed / duration);
            fadeImage.color = new Color(0f, 0f, 0f, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }
        fadeImage.color = new Color(0f, 0f, 0f, to);
        if (to <= 0f) fadeImage.gameObject.SetActive(false);
        onComplete?.Invoke();
    }
}
