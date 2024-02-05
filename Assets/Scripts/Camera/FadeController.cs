using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FadeController : MonoBehaviour
{
    public static FadeController instance;

    [SerializeField] Image objectToFade;
    public bool isFading { get; private set; }
    Coroutine fadeRoutine, fadeTextRoutine;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }

    public void StartFade(float aValue, float aTime)
    {
        if (fadeRoutine != null)
        {
            StopCoroutine(fadeRoutine);
            fadeRoutine = null;
        }

        fadeRoutine = StartCoroutine(FadeTo(aValue, aTime));
    }

    public void StartFadeText(TextMeshProUGUI fadeGUI, float aValue, float aTime)
    {
        if (fadeTextRoutine == null)
            fadeTextRoutine = StartCoroutine(FadeText(fadeGUI, aValue, aTime));
    }

    IEnumerator FadeTo(float aValue, float aTime)
    {
        isFading = true;

        float alpha = objectToFade.color.a;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            Color newColor = new Color(0, 0, 0, Mathf.Lerp(alpha, aValue, t));
            objectToFade.color = newColor;
            yield return null;
        }

        isFading = false;

        fadeRoutine = null;
    }

    IEnumerator FadeText(TextMeshProUGUI fadeGUI, float aValue, float aTime)
    {
        isFading = true;

        float alpha = fadeGUI.color.a;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            Color newColor = new Color(fadeGUI.color.r, fadeGUI.color.g, fadeGUI.color.b, Mathf.Lerp(alpha, aValue, t));
            fadeGUI.color = newColor;
            yield return null;
        }

        isFading = false;

        fadeTextRoutine = null;
    }
}
