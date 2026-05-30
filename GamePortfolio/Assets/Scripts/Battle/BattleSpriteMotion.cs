using System.Collections;
using UnityEngine;

/// <summary>
/// Lightweight UI sprite motion for portfolio GIF readability.
/// Adds a gentle idle bob and a short shove-back hit reaction without Animator assets.
/// </summary>
public class BattleSpriteMotion : MonoBehaviour
{
    [SerializeField] private float idleBobPixels = 4f;
    [SerializeField] private float idleBobSpeed = 1.6f;
    [SerializeField] private float idlePhaseOffset = 0f;
    [SerializeField] private float hitMovePixels = 18f;
    [SerializeField] private float hitSquashAmount = 0.08f;
    [SerializeField] private bool hitMovesLeft = true;

    private RectTransform rectTransform;
    private Vector2 baseAnchoredPosition;
    private Vector3 baseScale;
    private Coroutine hitRoutine;
    private float hitOffset;

    public string DebugProfile => BuildDebugProfile(idleBobPixels, idleBobSpeed, hitMovePixels, hitSquashAmount);

    public static string BuildDebugProfile(float bobPixels, float bobSpeed, float hitPixels, float squashAmount)
    {
        return $"Bob={bobPixels:0.#}px Speed={bobSpeed:0.#} Hit={hitPixels:0.#}px Squash={squashAmount:0.##}";
    }

    private void Awake()
    {
        CacheBaseTransform();
    }

    private void OnEnable()
    {
        CacheBaseTransform();
    }

    private void OnDisable()
    {
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = baseAnchoredPosition;
            rectTransform.localScale = baseScale;
        }
        hitOffset = 0f;
        hitRoutine = null;
    }

    private void Update()
    {
        if (rectTransform == null)
            CacheBaseTransform();
        if (rectTransform == null)
            return;

        float bob = Mathf.Sin((Time.time + idlePhaseOffset) * idleBobSpeed * Mathf.PI * 2f) * idleBobPixels;
        rectTransform.anchoredPosition = baseAnchoredPosition + new Vector2(hitOffset, bob);
    }

    public void Configure(float bobPixels, float bobSpeed, float phaseOffset, float hitPixels, float squashAmount, bool moveLeftOnHit)
    {
        idleBobPixels = bobPixels;
        idleBobSpeed = bobSpeed;
        idlePhaseOffset = phaseOffset;
        hitMovePixels = hitPixels;
        hitSquashAmount = squashAmount;
        hitMovesLeft = moveLeftOnHit;
        CacheBaseTransform();
    }

    public void PlayHitReaction()
    {
        if (!isActiveAndEnabled)
            return;
        if (hitRoutine != null)
            StopCoroutine(hitRoutine);
        hitRoutine = StartCoroutine(HitReactionRoutine());
    }

    private IEnumerator HitReactionRoutine()
    {
        float direction = hitMovesLeft ? -1f : 1f;
        float duration = 0.18f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float punch = Mathf.Sin(t * Mathf.PI);
            hitOffset = direction * hitMovePixels * punch;
            if (rectTransform != null)
                rectTransform.localScale = new Vector3(baseScale.x * (1f + hitSquashAmount * punch), baseScale.y * (1f - hitSquashAmount * punch), baseScale.z);
            elapsed += Time.deltaTime;
            yield return null;
        }

        hitOffset = 0f;
        if (rectTransform != null)
            rectTransform.localScale = baseScale;
        hitRoutine = null;
    }

    private void CacheBaseTransform()
    {
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
            return;
        baseAnchoredPosition = rectTransform.anchoredPosition;
        baseScale = rectTransform.localScale;
    }
}
