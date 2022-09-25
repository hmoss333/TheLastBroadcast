using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Intro_TransmitterController : InteractObject
{
    [SerializeField] GameObject focusPoint;
    [SerializeField] string nextScene;


    public override void Interact()
    {
        if (active)
        {
            base.Interact();

            PlayerController.instance.ToggleAvatar();
            CameraController.instance.SetTarget(interacting ? focusPoint : PlayerController.instance.gameObject);
            CameraController.instance.FocusTarget();
            active = false;

            StartCoroutine(LoadRoom(nextScene));
        }
    }

    IEnumerator LoadRoom(string sceneToLoad)
    {
        yield return new WaitForSeconds(1f);

        CamEffectController.instance.SetEffectValues(true);

        yield return new WaitForSeconds(2.5f); //change this to wait while any dialogue is playing

        FadeController.instance.StartFade(1.0f, 7.5f);

        while (FadeController.instance.isFading)
            yield return null;

        CamEffectController.instance.SetEffectValues(false);
        SaveDataController.instance.SetSavePoint(sceneToLoad, 0);
        PlayerController.instance.ToggleAvatar();
        SceneManager.LoadSceneAsync(sceneToLoad);
    }
}
