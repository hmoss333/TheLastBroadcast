using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class TitleScreenController : MonoBehaviour
{
    [SerializeField] AudioSource musicSource;
    [SerializeField] Camera MainCam, UICam;
    [SerializeField] PanCamera panCamera;
    [SerializeField] TextMeshProUGUI titleText, quoteText, sigText;
    [SerializeField] float fadeSpeed, startDelayTime, quoteDelayTime, glitchDelayTime, titleDelayTime, titleDisplayTime, sceneDelayTime;
    private float targetVolume, t = 0f;
    [SerializeField] string sceneToLoad;
    [SerializeField] LoadingScene loadingScreen;


    // Start is called before the first frame update
    void Start()
    {
        MainCam.enabled = false;
        UICam.enabled = true;
        StartCoroutine(TitleScreenRoutine());
    }

    private void Update()
    {
        t += Time.deltaTime / 100f;
        musicSource.volume = Mathf.Lerp(musicSource.volume, targetVolume, t);
    }

    IEnumerator TitleScreenRoutine()
    {
        yield return new WaitForSeconds(startDelayTime);

        //Start music audio
        musicSource.volume = 0;
        musicSource.Play();
        t = 0;
        targetVolume = 1f;//0.5f; //0.15f;


        //Fade In Quote/Signature text
        FadeController.instance.StartFadeText(quoteText, 1, 2f);
        while (FadeController.instance.isFading)
            yield return null;
        FadeController.instance.StartFadeText(sigText, 1, 2f);
        while (FadeController.instance.isFading)
            yield return null;


        yield return new WaitForSeconds(quoteDelayTime);


        //Fade Out Quote/Signature text
        FadeController.instance.StartFadeText(quoteText, 0.00f, 1f);
        while (FadeController.instance.isFading)
            yield return null;
        FadeController.instance.StartFadeText(sigText, 0.00f, 1f);
        while (FadeController.instance.isFading)
            yield return null;


        yield return new WaitForSeconds(1f);


        //Start PanCamera script
        panCamera.TogglePanning(true);
        MainCam.enabled = true;
        FadeController.instance.StartFade(0, 2.5f);
        while (FadeController.instance.isFading)
            yield return null;
        UICam.enabled = false;
        while (panCamera.isPanning)
            yield return null;

        MainCam.enabled = false;
        UICam.enabled = true;

        yield return new WaitForSeconds(titleDelayTime);


        //Fade In Title text
        FadeController.instance.StartFadeText(titleText, 1, 2.5f);
        while (FadeController.instance.isFading)
            yield return null;


        //Audio Pitch Changes
        //while (musicSource.pitch >= 0.25f)
        //{
        //    musicSource.pitch -= Time.deltaTime;
        //    yield return null;
        //}
        //while (musicSource.pitch <= 1.5f)
        //{
        //    musicSource.pitch += Time.deltaTime;
        //    yield return null;
        //}


        //Delay and activate glitch effect
        yield return new WaitForSeconds(glitchDelayTime);
        CamEffectController.instance.SetEffectState(true);


        //Audio Pitch Down
        while (musicSource.pitch >= 0.125f)
        {
            musicSource.pitch -= 0.125f * Time.deltaTime;
            yield return null;
        }


        yield return new WaitForSeconds(titleDisplayTime);

        //Fade Out Title Screen
        t = 0;
        targetVolume = 0f;
        FadeController.instance.StartFadeText(titleText, 0, 2.5f);
        while (FadeController.instance.isFading)
            yield return null;

        //Fade Out Static and Music audio
        yield return new WaitForSeconds(sceneDelayTime);

        //Load next scene
        //SceneManager.LoadSceneAsync(sceneToLoad);
        loadingScreen.LoadScene(sceneToLoad);
    }
}
