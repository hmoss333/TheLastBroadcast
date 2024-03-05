using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class CreditsController : MonoBehaviour
{
    [SerializeField] private float fadeInTime = 10f;
    [SerializeField] private float fadeOutTime = 2.5f;
    [SerializeField] TMP_Text inputIconText;
    string inputText;

    Coroutine fadeSceneRoutine;

    private void Awake()
    {
        if (inputIconText)
            inputText = inputIconText.text;
    }

    private void Start()
    {
        FadeController.instance.StartFade(0.0f, fadeInTime);
    }

    private void Update()
    {
        //If Input text exists, update key reference
        if (inputIconText)
        {
            string returnText = InputTextConverter.instance.GenerateText(inputText);
            inputIconText.text = returnText;
        }

        //If input is registered, start scene transition
        if (InputController.instance.inputMaster.Player.Pause.triggered || InputController.instance.inputMaster.Player.Interact.triggered)
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
