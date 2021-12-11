using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ObscuringItemFader : MonoBehaviour
{
    [SerializeField] private ItemFaderSettings itemFaderSettings;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void FadeOut()
    {
        StartCoroutine(FadeOutCoroutine());
    }

    public void FadeIn()
    {
        StartCoroutine(FadeInCoroutine());
    }

    private IEnumerator FadeOutCoroutine()
    {
        float currentAlpha = spriteRenderer.color.a;
        float distance = currentAlpha - itemFaderSettings.targetAlpha;

        while (currentAlpha - itemFaderSettings.targetAlpha > 0.01f)
        {
            currentAlpha = currentAlpha - distance / itemFaderSettings.fadeOutSeconds * Time.deltaTime;
            spriteRenderer.color = new Color(1, 1, 1, currentAlpha);
            yield return null;
        }

        spriteRenderer.color = new Color(1, 1, 1, itemFaderSettings.targetAlpha);
    }

    private IEnumerator FadeInCoroutine()
    {
        float currentAlpha = spriteRenderer.color.a;
        float distance = 1 - currentAlpha;

        while (1 - currentAlpha > 0.01f)
        {
            currentAlpha = currentAlpha + distance / itemFaderSettings.fadeInSeconds * Time.deltaTime;
            spriteRenderer.color = new Color(1, 1, 1, currentAlpha);
            yield return null;
        }

        spriteRenderer.color = new Color(1, 1, 1, 1);
    }
}
