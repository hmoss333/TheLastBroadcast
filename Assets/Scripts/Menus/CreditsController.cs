using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsController : MonoBehaviour
{
    private InputMaster inputMaster;
    [SerializeField] public float fadeInTime = 10f;
    [SerializeField] public float fadeOutTime = 2.5f;


    private void Start()
    {
        inputMaster = new InputMaster();
        inputMaster.Enable();

        FadeController.instance.StartFade(0.0f, fadeInTime);
    }

    private void Update()
    {
        if (inputMaster.Player.Pause.triggered)
        {
            SceneManager.LoadSceneAsync("MainMenu");
            FadeController.instance.StartFade(1.0f, fadeOutTime);
        }
    }
}
