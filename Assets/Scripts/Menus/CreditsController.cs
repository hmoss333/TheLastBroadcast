using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsController : MonoBehaviour
{
    InputMaster inputMaster;

    [SerializeField] private float fadeInTime = 10f;
    [SerializeField] private float fadeOutTime = 2.5f;

    Coroutine fadeSceneRoutine;

    private void Awake()
    {
        inputMaster = new InputMaster();
        inputMaster.Enable();
    }

    private void Start()
    {
        FadeController.instance.StartFade(0.0f, fadeInTime);
    }

    private void Update()
    {
        if (inputMaster.Player.Pause.triggered || inputMaster.Player.Interact.triggered)
        {
            if (fadeSceneRoutine == null)
                fadeSceneRoutine = StartCoroutine(FadeScene(fadeOutTime));
        }
    }

    IEnumerator FadeScene(float fadeTime)
    {
        FadeController.instance.StartFade(1.0f, fadeOutTime);

        while (FadeController.instance.isFading)
            yield return null;

        SceneManager.LoadSceneAsync("MainMenu");

        fadeSceneRoutine = null;
    }
}
