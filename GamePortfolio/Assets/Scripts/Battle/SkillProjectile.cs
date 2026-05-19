using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Spawns and animates element-themed projectiles with hit sparks and screen shake.
/// </summary>
public class SkillProjectile : MonoBehaviour
{
    [SerializeField] private float duration = 0.3f;
    private ElementType element;
    private static ScreenShake cachedShake;
    private static Canvas cachedCanvas;
    private static Transform cachedCanvasTransform;

    private static Canvas GetOrCacheCanvas()
    {
        if (cachedCanvas == null)
        {
            cachedCanvas = FindObjectOfType<Canvas>();
            cachedCanvasTransform = cachedCanvas != null ? cachedCanvas.transform : null;
        }
        return cachedCanvas;
    }

    private static ScreenShake GetOrCacheShake()
    {
        if (cachedShake == null && Camera.main != null)
            cachedShake = Camera.main.GetComponent<ScreenShake>();
        return cachedShake;
    }

    public static void Spawn(ElementType element, Vector3 start, Vector3 end, Transform parent)
    {
        GameObject obj = new GameObject($"Projectile_{element}");
        obj.transform.SetParent(parent, false);
        obj.transform.position = start;

        Image img = obj.AddComponent<Image>();
        img.color = GetElementColor(element);
        img.raycastTarget = false;

        RectTransform rt = obj.GetComponent<RectTransform>();

        switch (element)
        {
            case ElementType.Lightning:
                rt.sizeDelta = new Vector2(6, 28);
                break;
            case ElementType.Ice:
                rt.sizeDelta = new Vector2(16, 16);
                break;
            case ElementType.Earth:
                rt.sizeDelta = new Vector2(22, 22);
                break;
            default:
                rt.sizeDelta = new Vector2(18, 18);
                break;
        }
        rt.position = start;

        SkillProjectile proj = obj.AddComponent<SkillProjectile>();
        proj.element = element;
        proj.StartCoroutine(proj.MoveRoutine(start, end));
    }

    private IEnumerator MoveRoutine(Vector3 start, Vector3 end)
    {
        float elapsed = 0f;
        Vector3 direction = (end - start).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            t = t * t * (3f - 2f * t); // smoothstep

            // Arc trajectory for fire and earth
            float arcHeight = (element == ElementType.Fire || element == ElementType.Earth)
                ? Mathf.Sin(t * Mathf.PI) * 40f : 0f;

            Vector3 basePos = Vector3.Lerp(start, end, t);
            transform.position = new Vector3(basePos.x, basePos.y + arcHeight, basePos.z);

            // Rotation
            switch (element)
            {
                case ElementType.Fire:
                    transform.rotation = Quaternion.Euler(0, 0, elapsed * 720f); // Fast spin
                    break;
                case ElementType.Lightning:
                    transform.rotation = Quaternion.Euler(0, 0, angle + 90f); // Point forward
                    break;
                case ElementType.Ice:
                    transform.rotation = Quaternion.Euler(0, 0, -elapsed * 180f); // Slow spin
                    break;
                case ElementType.Earth:
                    transform.rotation = Quaternion.Euler(0, elapsed * 360f, 0); // Side spin
                    break;
            }

            // Scale pulse for lightning
            if (element == ElementType.Lightning)
            {
                float pulse = 0.8f + 0.2f * Mathf.Sin(t * Mathf.PI * 4f);
                transform.localScale = new Vector3(1f, pulse, 1f);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = end;

        // Spawn hit impact spark
        SpawnHitSpark(end, element);

        // Small screen shake on impact
        ScreenShake shake = GetOrCacheShake();
        if (shake != null)
        {
            float shMag = element == ElementType.Lightning ? 0.10f : element == ElementType.Fire ? 0.08f : 0.05f;
            shake.Shake(0.08f, shMag);
        }

        Destroy(gameObject, 0.05f);
    }

    private static void SpawnHitSpark(Vector3 position, ElementType element)
    {
        GetOrCacheCanvas();
        if (cachedCanvasTransform == null) return;

        GameObject spark = new GameObject("Hit Spark", typeof(RectTransform), typeof(Image));
        spark.transform.SetParent(cachedCanvasTransform, false);
        spark.transform.position = position;

        Image sparkImg = spark.GetComponent<Image>();
        sparkImg.color = GetElementColor(element);
        sparkImg.raycastTarget = false;

        RectTransform rt = spark.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(8, 8);

        spark.AddComponent<SkillProjectile>().StartCoroutine(SparkFadeRoutine(spark, rt, element));
    }

    private static IEnumerator SparkFadeRoutine(GameObject spark, RectTransform rt, ElementType element)
    {
        Image img = spark.GetComponent<Image>();
        float expandSize = element == ElementType.Lightning ? 40f : element == ElementType.Fire ? 30f : 24f;
        float startSize = 8f;
        float duration = 0.2f;
        float elapsed = 0f;

        // Expand outward multiple smaller sparks
        int sparkCount = element == ElementType.Fire ? 5 : 3;
        for (int i = 0; i < sparkCount; i++)
        {
            SpawnSubSpark(spark.transform.position, element, i, sparkCount);
        }

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float size = Mathf.Lerp(startSize, expandSize, t);
            rt.sizeDelta = new Vector2(size, size);
            img.color = new Color(img.color.r, img.color.g, img.color.b, 1f - t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(spark);
    }

    private static void SpawnSubSpark(Vector3 origin, ElementType element, int index, int count)
    {
        GetOrCacheCanvas();
        if (cachedCanvasTransform == null) return;

        GameObject sub = new GameObject("Sub Spark", typeof(RectTransform), typeof(Image));
        sub.transform.SetParent(cachedCanvasTransform, false);
        sub.transform.position = origin;

        Image subImg = sub.GetComponent<Image>();
        subImg.color = GetElementColor(element);
        subImg.raycastTarget = false;

        RectTransform subRt = sub.GetComponent<RectTransform>();
        subRt.sizeDelta = new Vector2(4, 4);

        float angle = (360f / count) * index;
        Vector3 dir = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0);
        float distance = 20f + index * 5f;
        Vector3 targetPos = origin + dir * distance;

        sub.AddComponent<SkillProjectile>().StartCoroutine(SubSparkRoutine(sub, targetPos));
    }

    private static IEnumerator SubSparkRoutine(GameObject sub, Vector3 target)
    {
        Image img = sub.GetComponent<Image>();
        Vector3 start = sub.transform.position;
        float duration = 0.15f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            sub.transform.position = Vector3.Lerp(start, target, t);
            img.color = new Color(img.color.r, img.color.g, img.color.b, 1f - t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(sub);
    }

    private static readonly Color FireColor = new Color(1f, 0.4f, 0.1f);
    private static readonly Color IceColor = new Color(0.3f, 0.6f, 1f);
    private static readonly Color LightningColor = new Color(1f, 0.9f, 0.2f);
    private static readonly Color EarthColor = new Color(0.5f, 0.8f, 0.3f);
    private static readonly Color PhysicalColor = new Color(0.8f, 0.8f, 0.8f);

    private static Color GetElementColor(ElementType element) => element switch
    {
        ElementType.Fire => FireColor,
        ElementType.Ice => IceColor,
        ElementType.Lightning => LightningColor,
        ElementType.Earth => EarthColor,
        _ => PhysicalColor
    };
}
