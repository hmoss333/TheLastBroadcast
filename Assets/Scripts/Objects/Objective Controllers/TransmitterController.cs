using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class TransmitterController : InteractObject
{
    public static TransmitterController instance;

    [SerializeField] GameObject focusPoint;
    [SerializeField] enum AbilityToGive { invisibility, remote, control };
    [SerializeField] AbilityToGive abilityToGive;
    [SerializeField] string sceneToActivate;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    public override void Interact()
    {
        if (active)
        {
            base.Interact();

            PlayerController.instance.ToggleAvatar();
            CameraController.instance.SetTarget(interacting ? focusPoint : PlayerController.instance.gameObject);
            CameraController.instance.FocusTarget();

            if (!hasActivated)
            {
                Debug.Log($"Actived transmitter for {sceneToActivate} station");
                hasActivated = true;
                active = false;
                SaveDataController.instance.EnableStation(sceneToActivate); //Activate new scene station
                SaveDataController.instance.GiveRadioAbility(abilityToGive.ToString()); //Give new ability station
                SaveDataController.instance.SaveFile();
                string currentScene = SaveDataController.instance.saveData.currentScene;
                SaveDataController.instance.SaveObjectData(currentScene);
            }

            StartCoroutine(LoadBroadcastRoom());
        }
    }

    IEnumerator LoadBroadcastRoom()
    {
        yield return new WaitForSeconds(2.5f); //change this to wait while any dialogue is playing

        FadeController.instance.StartFade(1.0f, 1f);

        while (FadeController.instance.isFading)
            yield return null;

        SaveDataController.instance.SetSavePoint("BroadcastRoom", 0);
        PlayerController.instance.ToggleAvatar();
        //SceneInitController.instance.SaveInteractObjs();
        SceneManager.LoadSceneAsync("BroadcastRoom");
    }
}
