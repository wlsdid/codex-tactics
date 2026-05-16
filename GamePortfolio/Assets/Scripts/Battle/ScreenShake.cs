using System.Collections;
using UnityEngine;

/// <summary>
/// Simple screen shake utility for battle feedback.
/// Attached to the Main Camera, called by BattleManager on strong hits.
/// </summary>
public class ScreenShake : MonoBehaviour
{
    [SerializeField] private float shakeDuration = 0.15f;
    [SerializeField] private float shakeMagnitude = 0.1f;

    private Vector3 originalPosition;

    private void Awake()
    {
        originalPosition = transform.localPosition;
    }

    public void Shake(float duration = -1f, float magnitude = -1f)
    {
        float dur = duration > 0f ? duration : shakeDuration;
        float mag = magnitude > 0f ? magnitude : shakeMagnitude;
        StopAllCoroutines();
        StartCoroutine(ShakeRoutine(dur, mag));
    }

    private IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float offsetX = Random.Range(-1f, 1f) * magnitude;
            float offsetY = Random.Range(-1f, 1f) * magnitude;
            transform.localPosition = originalPosition + new Vector3(offsetX, offsetY, 0f);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = originalPosition;
    }
}
