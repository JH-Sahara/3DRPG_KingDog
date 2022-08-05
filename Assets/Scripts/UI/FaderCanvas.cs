using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaderCanvas : MonoBehaviour
{
    CanvasGroup canvasGroup;
    public float fadeInTime;
    public float fadeOutTime;

    private void Awake() {
        canvasGroup = GetComponent<CanvasGroup>();
        DontDestroyOnLoad(gameObject);
    }

    public IEnumerator FadeIn(float time)
    {
        while (canvasGroup.alpha < 1f)
        {
            canvasGroup.alpha += Time.deltaTime / time;
            yield return null;
        }
    }

    public IEnumerator FadeOut(float time)
    {
        while (canvasGroup.alpha > 0f)
        {
            canvasGroup.alpha -= Time.deltaTime / time;
            yield return null;
        }
        Destroy(gameObject);
    }

    public IEnumerator FadeInOut()
    {
        yield return FadeIn(fadeInTime);
        yield return FadeOut(fadeOutTime);
    }
}
