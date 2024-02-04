using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TriggerSceneChange : MonoBehaviour
{
    [SerializeField] string sceneToLoad;
    [SerializeField] float fadeTime = 1f, transitionDelay = 0f;
    Coroutine changeScene;
    AudioSource musicSource;
    float targetVolume, t = 0;


    private void Start()
    {
        musicSource = AudioController.instance.audioSource;
    }

    private void Update()
    {
        if (changeScene != null)
        {
            t += Time.deltaTime / 100f;
            musicSource.volume = Mathf.Lerp(musicSource.volume, targetVolume, t);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            ChangeScene(sceneToLoad);
        }
    }

    public void ChangeScene(string sceneToLoad)
    {
        if (changeScene == null)
        {
            changeScene = StartCoroutine(LoadScene(sceneToLoad));
        }
    }

    IEnumerator LoadScene(string sceneToLoad)
    {
        FadeController.instance.StartFade(1.0f, fadeTime);

        while (FadeController.instance.isFading)
            yield return null;

        yield return new WaitForSeconds(transitionDelay);

        targetVolume = 0;

        if (!SceneInitController.instance.GetScenesToIgnore().Contains(sceneToLoad))
        {
            try { SaveDataController.instance.SetSavePoint(sceneToLoad, 0); } catch { };
            try { PlayerController.instance.ToggleAvatar(); } catch { };
        }

        SceneManager.LoadSceneAsync(sceneToLoad);
    }
}
