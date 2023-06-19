using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TriggerSceneChange : MonoBehaviour
{
    [SerializeField] string sceneToLoad;
    [SerializeField] float fadeTime = 1f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            StartCoroutine(LoadScene(sceneToLoad));
        }
    }

    IEnumerator LoadScene(string sceneToLoad)
    {
        FadeController.instance.StartFade(1.0f, fadeTime);

        while (FadeController.instance.isFading)
            yield return null;

        try { SaveDataController.instance.SetSavePoint(sceneToLoad, 0); }
        catch { };
        try { PlayerController.instance.ToggleAvatar(); }
        catch { };
        SceneManager.LoadSceneAsync(sceneToLoad);
    }
}
