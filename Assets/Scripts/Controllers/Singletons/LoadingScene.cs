using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LoadingScene : MonoBehaviour
{
    public GameObject loadingScreen;
    public Image loadingBarFill;
    public TextMeshProUGUI loadingText;

    Coroutine loadSceneRoutine;

    private void Start()
    {
        loadingScreen.SetActive(false);
    }

    public void LoadScene (string sceneName)
    {
        if (loadSceneRoutine == null)
            loadSceneRoutine = StartCoroutine(LoadSceneAsync(sceneName));
    }

    IEnumerator LoadSceneAsync(string sceneName)
    {
        loadingText.text = "Loading...";

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        loadingScreen.SetActive(true);

        while (!operation.isDone)
        {
            float progressValue = Mathf.Clamp01(operation.progress / 0.9f);
            loadingBarFill.fillAmount = progressValue;

            // Check if the load has finished
            if (operation.progress >= 0.9f)
            {
                //Change the Text to show the Scene is ready
                string continueText = "Press {Interact} to continue.";
                loadingText.text = InputTextConverter.instance.GenerateText(continueText);
                //Wait to you press the interact key to activate the Scene
                if (InputController.instance.inputMaster.Player.Interact.triggered)
                    operation.allowSceneActivation = true;
            }

            yield return null;
        }

        loadSceneRoutine = null;
    }
}
