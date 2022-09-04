using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class TitleScreenController : MonoBehaviour
{
    [SerializeField] AudioSource staticSource;
    [SerializeField] AudioSource musicSource;

    [SerializeField] TextMeshPro titleText;
    [SerializeField] TextMeshProUGUI quoteText, sigText;

    [SerializeField] float fadeSpeed, startDelayTime, musicDelayTime, quoteDelayTime, glitchDelayTime, titleDelayTime, titleDisplayTime, sceneDelayTime;


    // Start is called before the first frame update
    void Start()
    {
        titleText.gameObject.SetActive(false);
        quoteText.gameObject.SetActive(false);
        sigText.gameObject.SetActive(false);

        StartCoroutine(TitleScreenRoutine());
    }

    IEnumerator TitleScreenRoutine()
    {
        //Start static audio
        staticSource.volume = 0;
        staticSource.Play();
        StartCoroutine(FadeAudio(staticSource, 1, startDelayTime));

        yield return new WaitForSeconds(startDelayTime);

        //Start music audio
        musicSource.volume = 0;
        musicSource.Play();
        StartCoroutine(FadeAudio(musicSource, 1, musicDelayTime));

        //Fade In Quote/Signature text
        quoteText.gameObject.SetActive(true);
        sigText.gameObject.SetActive(true);
        FadeController.instance.StartFade(0, 2.5f);

        yield return new WaitForSeconds(quoteDelayTime);

        //Fade Out Quote/Signature text
        FadeController.instance.StartFade(1, 2.5f);
        while (FadeController.instance.isFading)
            yield return null;
        quoteText.gameObject.SetActive(false);
        sigText.gameObject.SetActive(false);

        yield return new WaitForSeconds(titleDelayTime);

        //Fade In Title text
        titleText.gameObject.SetActive(true);
        FadeController.instance.StartFade(0, 2.5f);

        while (FadeController.instance.isFading)
            yield return null;

        //Delay and activate glitch effect
        yield return new WaitForSeconds(glitchDelayTime);
        CamEffectController.instance.SetEffectValues(true);

        yield return new WaitForSeconds(titleDisplayTime);

        //Fade Out Title Screen
        FadeController.instance.StartFade(1, 2.5f);
        while (FadeController.instance.isFading)
            yield return null;
        titleText.gameObject.SetActive(false);

        //Fade Out Static and Music audio
        StartCoroutine(FadeAudio(musicSource, 0, sceneDelayTime));
        //StartCoroutine(FadeAudio(staticSource, 0, sceneDelayTime));
        yield return new WaitForSeconds(sceneDelayTime);

        //Load next scene
        SceneManager.LoadSceneAsync(2); //should link directly into starting scene
    }

    IEnumerator FadeAudio(AudioSource audioSource, float aValue, float aTime)
    {
        float delta = audioSource.volume;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            delta = Mathf.Lerp(delta, aValue, t);
            audioSource.volume = delta;
            yield return null;
        }
    }
}
