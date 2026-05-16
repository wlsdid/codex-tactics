using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Spawns and animates a projectile from player to enemy on skill use.
/// Color and size vary by element type.
/// </summary>
public class SkillProjectile : MonoBehaviour
{
    [SerializeField] private float duration = 0.3f;

    private static readonly Color FireColor = new Color(1f, 0.4f, 0.1f);
    private static readonly Color IceColor = new Color(0.3f, 0.6f, 1f);
    private static readonly Color LightningColor = new Color(1f, 0.9f, 0.2f);
    private static readonly Color EarthColor = new Color(0.5f, 0.8f, 0.3f);
    private static readonly Color PhysicalColor = new Color(0.8f, 0.8f, 0.8f);

    public static void Spawn(ElementType element, Vector3 start, Vector3 end, Transform parent)
    {
        GameObject obj = new GameObject($"Projectile_{element}");
        obj.transform.SetParent(parent, false);
        obj.transform.position = start;

        Image img = obj.AddComponent<Image>();
        img.color = GetElementColor(element);
        img.raycastTarget = false;

        RectTransform rt = obj.GetComponent<RectTransform>();
        rt.sizeDelta = element == ElementType.Lightning ? new Vector2(6, 24) : new Vector2(20, 20);
        rt.anchoredPosition3D = start;

        SkillProjectile proj = obj.AddComponent<SkillProjectile>();
        proj.StartCoroutine(proj.MoveRoutine(start, end));
    }

    private IEnumerator MoveRoutine(Vector3 start, Vector3 end)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            t = t * t * (3f - 2f * t); // smoothstep
            transform.position = Vector3.Lerp(start, end, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = end;
        Destroy(gameObject, 0.05f);
    }

    private static Color GetElementColor(ElementType element) => element switch
    {
        ElementType.Fire => FireColor,
        ElementType.Ice => IceColor,
        ElementType.Lightning => LightningColor,
        ElementType.Earth => EarthColor,
        _ => PhysicalColor
    };
}
