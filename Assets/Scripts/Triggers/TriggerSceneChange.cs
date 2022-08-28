using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TriggerSceneChange : MonoBehaviour
{
    [SerializeField] string sceneToLoad;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            StartCoroutine(LoadScene(sceneToLoad));
        }
    }

    IEnumerator LoadScene(string sceneToLoad)
    {
        FadeController.instance.StartFade(1.0f, 1f);

        while (FadeController.instance.isFading)
            yield return null;

        SaveDataController.instance.SetSavePoint(sceneToLoad, 0);
        PlayerController.instance.ToggleAvatar();
        SceneManager.LoadSceneAsync(sceneToLoad);
    }
}
