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
        staticSource.volume = 1;
        staticSource.Play();

        yield return new WaitForSeconds(startDelayTime);

        musicSource.volume = 0;
        musicSource.Play();

        //Update Audio
        //staticSource.volume = Mathf.Lerp(1, 0.5f, 3f);
        //musicSource.volume = Mathf.Lerp(0, 1, 3f);

        quoteText.gameObject.SetActive(true);
        sigText.gameObject.SetActive(true);
        FadeController.instance.StartFade(0, 2.5f);

        yield return new WaitForSeconds(quoteDelayTime);

        FadeController.instance.StartFade(1, 2.5f);
        while (FadeController.instance.isFading)
            yield return null;

        quoteText.gameObject.SetActive(false);
        sigText.gameObject.SetActive(false);

        yield return new WaitForSeconds(titleDelayTime);

        //Update Audio
        //staticSource.volume = Mathf.Lerp(0.5f, 1f, 3f);
        //musicSource.volume = Mathf.Lerp(1, 0, 3f);

        titleText.gameObject.SetActive(true);
        FadeController.instance.StartFade(0, 2.5f);

        while (FadeController.instance.isFading)
            yield return null;

        yield return new WaitForSeconds(glitchDelayTime);

        CamEffectController.instance.SetEffectValues(true);

        yield return new WaitForSeconds(titleDisplayTime);

        FadeController.instance.StartFade(1, 2.5f);

        while (FadeController.instance.isFading)
            yield return null;

        titleText.gameObject.SetActive(false);

        yield return new WaitForSeconds(sceneDelayTime);

        SceneManager.LoadScene(2); //should link directly into starting scene
    }
}
