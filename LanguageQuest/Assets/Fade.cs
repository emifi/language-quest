using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Fade : MonoBehaviour
{
    Color color;
    FadeType fadeType;
    float delay;
    float fadeTime;
    Image image;
    // Start is called before the first frame update
    void Start()
    {
        image = gameObject.GetComponentInChildren<Image>();
        image.color = color;
        FadeIn(Color.black, 1.0f, 3.0f);
    }
    // Fades the screen in, given a starting color
    public void FadeIn(Color startColor, float delay, float fadeTime) {
        StartCoroutine(FadeRoutine(startColor, Color.clear, delay, fadeTime));
    }
    // Fades the screen out, given the target color
    public void FadeOut(Color endColor, float delay, float fadeTime) {
        StartCoroutine(FadeRoutine(Color.clear, endColor, delay, fadeTime));
    }

    IEnumerator FadeRoutine(Color beginColor, Color endColor, float delay, float fadeTime) {
        PlayerInteraction.disableInteraction();
        image.color = beginColor;
        yield return new WaitForSeconds(delay);
        float t = 0.0f;
        while (t < fadeTime) {
            image.color = Color.Lerp(beginColor, endColor, t/fadeTime);
            t += Time.deltaTime;
            yield return null;
        }
        PlayerInteraction.enableInteraction();
    }
    
}

public enum FadeType{In,Out};
