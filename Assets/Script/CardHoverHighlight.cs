using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))] // swap to Collider if this is a 3D project
public class CardHoverHighlight : MonoBehaviour
{
    [SerializeField] private SpriteRenderer glowOverlay; // separate child sprite, starts invisible
    [SerializeField] private float maxGlowAlpha = 0.6f;
    [SerializeField] private float transitionSpeed = 10f;

    private Coroutine fadeRoutine;

    private void Awake()
    {
        if (glowOverlay != null)
        {
            Color c = glowOverlay.color;
            c.a = 0f;
            glowOverlay.color = c;
        }
    }

    private void OnMouseEnter()
    {
        FadeTo(maxGlowAlpha);
    }

    private void OnMouseExit()
    {
        FadeTo(0f);
    }

    private void FadeTo(float targetAlpha)
    {
        if (glowOverlay == null) return;

        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeRoutine(targetAlpha));
    }

    private IEnumerator FadeRoutine(float targetAlpha)
    {
        while (Mathf.Abs(glowOverlay.color.a - targetAlpha) > 0.01f)
        {
            Color c = glowOverlay.color;
            c.a = Mathf.Lerp(c.a, targetAlpha, Time.deltaTime * transitionSpeed);
            glowOverlay.color = c;
            yield return null;
        }

        Color final = glowOverlay.color;
        final.a = targetAlpha;
        glowOverlay.color = final;
    }
}