using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TriggerSceneChange : MonoBehaviour
{
    [SerializeField] string sceneToLoad;
    [SerializeField] bool additive;
    [SerializeField] float fadeTime = 1f, transitionDelay = 0f;
    Coroutine changeScene;
    AudioSource musicSource;
    float targetVolume, t = 0;


    private void Start()
    {
        try
        {
            musicSource = AudioController.instance.audioSource;
        }
        catch { print($"Unable to locate {AudioController.instance}"); }
    }

    private void Update()
    {
        try
        {
            if (changeScene != null)
            {
                t += Time.deltaTime / 100f;
                musicSource.volume = Mathf.Lerp(musicSource.volume, targetVolume, t);
            }
        }
        catch { }
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
        yield return new WaitForSeconds(transitionDelay);

        FadeController.instance.StartFade(1.0f, fadeTime);

        while (FadeController.instance.isFading)
            yield return null;

        targetVolume = 0;

        if (!SceneInitController.instance.GetScenesToIgnore().Contains(sceneToLoad))
        {
            try { SaveDataController.instance.SetSavePoint(sceneToLoad, 0); } catch { };
            try { PlayerController.instance.ToggleAvatar(); } catch { };
            InputController.instance.inputMaster.Player.Disable();
        }

        SceneManager.LoadSceneAsync(sceneToLoad, additive ? LoadSceneMode.Additive : LoadSceneMode.Single);
    }
}
