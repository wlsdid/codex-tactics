using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// Animated floating damage/heal/buff text popup.
/// Attaches to a world-space GameObject and auto-destroys after animation.
/// </summary>
public class DamagePopup : MonoBehaviour
{
    [SerializeField] private float upwardSpeed = 40f;
    [SerializeField] private float fadeDuration = 0.8f;
    [SerializeField] private float scaleDuration = 0.2f;

    private TMP_Text label;
    private Color baseColor;
    private float elapsed;

    private void Awake()
    {
        label = GetComponent<TMP_Text>();
        if (label == null)
            label = gameObject.AddComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        elapsed = 0f;
        if (label != null)
        {
            baseColor = label.color;
            label.transform.localScale = Vector3.one * 1.5f;
            StartCoroutine(ScaleIn());
        }
    }

    private IEnumerator ScaleIn()
    {
        float t = 0f;
        while (t < scaleDuration)
        {
            float s = Mathf.Lerp(1.5f, 1f, t / scaleDuration);
            label.transform.localScale = Vector3.one * s;
            t += Time.deltaTime;
            yield return null;
        }
        label.transform.localScale = Vector3.one;
    }

    private void Update()
    {
        elapsed += Time.deltaTime;
        // Float upward
        transform.Translate(Vector3.up * upwardSpeed * Time.deltaTime);
        // Fade out
        if (elapsed > fadeDuration * 0.5f && label != null)
        {
            float alpha = Mathf.Lerp(1f, 0f, (elapsed - fadeDuration * 0.5f) / (fadeDuration * 0.5f));
            label.color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
        }
        if (elapsed >= fadeDuration)
            Destroy(gameObject);
    }

    // --- Static spawn helpers ---

    /// <summary>Spawns a damage popup at the given world position on the given canvas.</summary>
    public static void ShowDamage(int damage, Vector3 worldPosition, Transform canvasTransform)
    {
        ShowPopup(damage.ToString(), worldPosition, new Color(1f, 0.25f, 0.2f), 36, canvasTransform);
    }

    /// <summary>Spawns a heal popup at the given world position.</summary>
    public static void ShowHeal(int heal, Vector3 worldPosition, Transform canvasTransform)
    {
        ShowPopup($"+{heal}", worldPosition, new Color(0.3f, 1f, 0.4f), 32, canvasTransform);
    }

    /// <summary>Spawns a shield/buff popup.</summary>
    public static void ShowBuff(string text, Vector3 worldPosition, Color color, Transform canvasTransform)
    {
        ShowPopup(text, worldPosition, color, 28, canvasTransform);
    }

    /// <summary>Spawns a weakness hit popup.</summary>
    public static void ShowWeaknessHit(int damage, Vector3 worldPosition, Transform canvasTransform)
    {
        ShowPopup($"WEAKNESS! {damage}", worldPosition, new Color(1f, 0.85f, 0.2f), 38, canvasTransform);
    }

    /// <summary>Spawns a BREAK popup.</summary>
    public static void ShowBreak(Vector3 worldPosition, Transform canvasTransform)
    {
        ShowPopup("BREAK!", worldPosition, new Color(1f, 0.3f, 0.8f), 42, canvasTransform);
    }

    private static void ShowPopup(string text, Vector3 worldPosition, Color color, int fontSize, Transform canvasTransform)
    {
        if (canvasTransform == null) return;
        GameObject obj = new GameObject($"DamagePopup_{text}");
        obj.transform.SetParent(canvasTransform, false);
        obj.transform.position = worldPosition;

        DamagePopup popup = obj.AddComponent<DamagePopup>();
        TMP_Text tmp = obj.GetComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.color = color;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontStyle = FontStyles.Bold;
        tmp.raycastTarget = false;
    }
}
