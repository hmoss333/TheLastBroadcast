using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class BilboardController : MonoBehaviour
{
    public static BilboardController instance;

    [Header("Radio Values")]
    [SerializeField] TMP_Text radioText;
    [SerializeField] GameObject radioDial;

    [Header("Scene Buttons")]
    [SerializeField] GameObject site1Button;
    [SerializeField] GameObject site2Button;
    [SerializeField] GameObject site3Button;
    [SerializeField] GameObject site4Button;
    [SerializeField] GameObject site5Button;

    Coroutine ls;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    private void Start()
    {
        FadeController.instance.StartFade(0f, 1f); //TODO move this to a scene controller, if possible
    }

    public void LoadScene(string sceneToLoad)
    {
        if (ls == null)
            ls = StartCoroutine(LoadSceneRoutine(sceneToLoad));
    }

    IEnumerator LoadSceneRoutine(string sceneToLoad)
    {
        print($"Loading scene {sceneToLoad}");
        SaveDataController.instance.SetSavePoint(sceneToLoad);
        FadeController.instance.StartFade(1f, 1f);

        while (FadeController.instance.isFading)
            yield return null;

        SceneManager.LoadSceneAsync(sceneToLoad);

        ls = null;
    }
}
