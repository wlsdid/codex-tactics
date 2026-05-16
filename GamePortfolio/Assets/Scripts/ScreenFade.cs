using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages screen fade transitions (black fade in/out) between scenes or encounters.
/// Attach to a persistent GameObject or create on demand.
/// </summary>
public class ScreenFade : MonoBehaviour
{
    private Image fadeImage;

    private void Awake()
    {
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
