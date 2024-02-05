using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsController : MonoBehaviour
{
    InputMaster inputMaster;

    [SerializeField] private float fadeInTime = 10f;
    [SerializeField] private float fadeOutTime = 2.5f;

    Coroutine unloadSceneRoutine;

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
            SceneManager.LoadSceneAsync("MainMenu");
            FadeController.instance.StartFade(1.0f, fadeOutTime);
        }
    }
}
