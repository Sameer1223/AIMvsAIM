using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DamageFlash : MonoBehaviour
{
    public Image damageVignette;
    public float flashDuration = 0.3f;
    public float maxAlpha = 0.4f;

    private Coroutine flashCoroutine;

    public void Flash()
    {
        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);

        flashCoroutine = StartCoroutine(FlashVignette());
    }

    private IEnumerator FlashVignette()
    {
        float elapsed = 0f;

        // Fade in
        while (elapsed < flashDuration / 2f)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0.2f, maxAlpha, elapsed / (flashDuration / 2f));
            SetAlpha(alpha);
            yield return null;
        }

        // Fade out
        elapsed = 0f;
        while (elapsed < flashDuration / 2f)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(maxAlpha, 0f, elapsed / (flashDuration / 2f));
            SetAlpha(alpha);
            yield return null;
        }

        SetAlpha(0f);
    }

    private void SetAlpha(float a)
    {
        if (damageVignette != null)
        {
            Color c = damageVignette.color;
            c.a = a;
            damageVignette.color = c;
        }
    }
    
    public void Reset()
    {
        SetAlpha(0f);
    }
}