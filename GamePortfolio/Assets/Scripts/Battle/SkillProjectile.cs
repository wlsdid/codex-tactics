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
    private Image projectileImage;
    private RectTransform projectileRect;
    private RectTransform trailRect;
    private Image trailImage;
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
        Vector2 projectileSize = GetProjectileSize(element);
        rt.sizeDelta = projectileSize;

        GameObject trail = new GameObject("Projectile Trail", typeof(RectTransform), typeof(Image));
        trail.transform.SetParent(obj.transform, false);
        trail.transform.SetAsFirstSibling();
        RectTransform trailRt = trail.GetComponent<RectTransform>();
        trailRt.anchorMin = new Vector2(0.5f, 0.5f);
        trailRt.anchorMax = new Vector2(0.5f, 0.5f);
        trailRt.pivot = new Vector2(0.5f, 0.5f);
        trailRt.anchoredPosition = Vector2.zero;
        trailRt.sizeDelta = projectileSize + GetTrailPadding(element);
        Image trailImg = trail.GetComponent<Image>();
        Color trailColor = GetElementColor(element);
        trailColor.a = GetTrailAlpha(element);
        trailImg.color = trailColor;
        trailImg.raycastTarget = false;

        rt.position = start;

        SkillProjectile proj = obj.AddComponent<SkillProjectile>();
        proj.element = element;
        proj.duration = GetProjectileDuration(element);
        proj.projectileImage = img;
        proj.projectileRect = rt;
        proj.trailRect = trailRt;
        proj.trailImage = trailImg;
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

            UpdateProjectileReadability(t);

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
            shake.Shake(GetShakeDuration(element), GetShakeMagnitude(element));
        }

        Destroy(gameObject, 0.05f);
    }

    private void UpdateProjectileReadability(float t)
    {
        if (projectileRect == null) return;

        float breathe = 1f + Mathf.Sin(t * Mathf.PI * GetPulseFrequency(element)) * GetPulseAmount(element);
        projectileRect.localScale = new Vector3(breathe, breathe, 1f);

        if (projectileImage != null)
        {
            Color baseColor = GetElementColor(element);
            float alpha = Mathf.Lerp(0.85f, 1f, Mathf.Sin(t * Mathf.PI));
            projectileImage.color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
        }

        if (trailRect != null)
        {
            float trailPulse = 1f + Mathf.Sin(t * Mathf.PI * 2f) * 0.12f;
            trailRect.localScale = new Vector3(trailPulse, trailPulse, 1f);
        }

        if (trailImage != null)
        {
            Color trailColor = GetElementColor(element);
            trailColor.a = Mathf.Lerp(GetTrailAlpha(element) * 0.45f, GetTrailAlpha(element), Mathf.Sin(t * Mathf.PI));
            trailImage.color = trailColor;
        }
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

        SpawnImpactRing(position, element);
        spark.AddComponent<SkillProjectile>().StartCoroutine(SparkFadeRoutine(spark, rt, element));
    }

    private static IEnumerator SparkFadeRoutine(GameObject spark, RectTransform rt, ElementType element)
    {
        Image img = spark.GetComponent<Image>();
        float expandSize = GetImpactSparkSize(element);
        float startSize = 8f;
        float duration = GetImpactDuration(element);
        float elapsed = 0f;

        // Expand outward multiple smaller sparks
        int sparkCount = GetBurstSparkCount(element);
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
        float distance = GetSubSparkDistance(element) + index * 5f;
        Vector3 targetPos = origin + dir * distance;

        sub.AddComponent<SkillProjectile>().StartCoroutine(SubSparkRoutine(sub, targetPos, element));
    }

    private static IEnumerator SubSparkRoutine(GameObject sub, Vector3 target, ElementType element)
    {
        Image img = sub.GetComponent<Image>();
        Vector3 start = sub.transform.position;
        float duration = GetImpactDuration(element) * 0.75f;
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

    private static void SpawnImpactRing(Vector3 position, ElementType element)
    {
        GetOrCacheCanvas();
        if (cachedCanvasTransform == null) return;

        GameObject ring = new GameObject($"Impact Ring {element}", typeof(RectTransform), typeof(Image));
        ring.transform.SetParent(cachedCanvasTransform, false);
        ring.transform.position = position;

        Image ringImg = ring.GetComponent<Image>();
        Color color = GetElementColor(element);
        color.a = GetImpactRingAlpha(element);
        ringImg.color = color;
        ringImg.raycastTarget = false;

        RectTransform ringRt = ring.GetComponent<RectTransform>();
        ringRt.sizeDelta = new Vector2(12f, 12f);

        ring.AddComponent<SkillProjectile>().StartCoroutine(ImpactRingRoutine(ring, ringRt, element));
    }

    private static IEnumerator ImpactRingRoutine(GameObject ring, RectTransform rt, ElementType element)
    {
        Image img = ring.GetComponent<Image>();
        float duration = GetImpactDuration(element) * 1.25f;
        float endSize = GetImpactRingSize(element);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float eased = 1f - Mathf.Pow(1f - t, 2f);
            rt.sizeDelta = new Vector2(Mathf.Lerp(12f, endSize, eased), Mathf.Lerp(12f, endSize, eased));
            img.color = new Color(img.color.r, img.color.g, img.color.b, Mathf.Lerp(GetImpactRingAlpha(element), 0f, t));
            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(ring);
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

    private static Vector2 GetProjectileSize(ElementType element) => element switch
    {
        ElementType.Lightning => new Vector2(7f, 34f),
        ElementType.Ice => new Vector2(18f, 18f),
        ElementType.Earth => new Vector2(24f, 24f),
        ElementType.Fire => new Vector2(20f, 20f),
        _ => new Vector2(18f, 18f)
    };

    private static Vector2 GetTrailPadding(ElementType element) => element switch
    {
        ElementType.Lightning => new Vector2(10f, 18f),
        ElementType.Ice => new Vector2(14f, 14f),
        ElementType.Earth => new Vector2(12f, 12f),
        ElementType.Fire => new Vector2(18f, 18f),
        _ => new Vector2(12f, 12f)
    };

    private static float GetProjectileDuration(ElementType element) => element switch
    {
        ElementType.Lightning => 0.20f,
        ElementType.Fire => 0.26f,
        ElementType.Ice => 0.34f,
        ElementType.Earth => 0.38f,
        _ => 0.28f
    };

    private static float GetPulseFrequency(ElementType element) => element == ElementType.Lightning ? 6f : 3f;
    private static float GetPulseAmount(ElementType element) => element == ElementType.Ice ? 0.10f : 0.18f;
    private static float GetTrailAlpha(ElementType element) => element == ElementType.Lightning ? 0.34f : 0.24f;
    private static float GetImpactDuration(ElementType element) => element == ElementType.Lightning ? 0.16f : element == ElementType.Earth ? 0.24f : 0.20f;
    private static float GetImpactSparkSize(ElementType element) => element == ElementType.Lightning ? 56f : element == ElementType.Fire ? 46f : element == ElementType.Earth ? 42f : 34f;
    private static float GetImpactRingSize(ElementType element) => element == ElementType.Lightning ? 86f : element == ElementType.Fire ? 72f : element == ElementType.Earth ? 64f : 54f;
    private static float GetImpactRingAlpha(ElementType element) => element == ElementType.Lightning ? 0.48f : 0.36f;
    private static float GetShakeDuration(ElementType element) => element == ElementType.Lightning ? 0.12f : element == ElementType.Earth ? 0.11f : 0.09f;
    private static float GetShakeMagnitude(ElementType element) => element == ElementType.Lightning ? 0.14f : element == ElementType.Fire ? 0.11f : element == ElementType.Earth ? 0.10f : 0.07f;
    private static float GetSubSparkDistance(ElementType element) => element == ElementType.Lightning ? 32f : element == ElementType.Fire ? 28f : 22f;

    private static int GetBurstSparkCount(ElementType element) => element switch
    {
        ElementType.Fire => 8,
        ElementType.Lightning => 7,
        ElementType.Earth => 6,
        ElementType.Ice => 5,
        _ => 4
    };

    public static string DebugImpactProfile(ElementType element)
    {
        return $"Element={element}; Projectile={GetProjectileSize(element)}; Trail={GetTrailPadding(element)}; Sparks={GetBurstSparkCount(element)}; Ring={GetImpactRingSize(element)}; Shake={GetShakeMagnitude(element):0.00}";
    }
}
