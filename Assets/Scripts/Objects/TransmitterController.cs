using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class TransmitterController : InteractObject
{
    [SerializeField] GameObject focusPoint;
    [SerializeField] enum AbilityToGive { Tune, Invisibility, Rats };
    [SerializeField] AbilityToGive abilityToGive;
    [SerializeField] string sceneToActivate;


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
                SaveDataController.instance.SaveObjectData(SceneManager.GetActiveScene().name); //Save object states
                if (!SaveDataController.instance.GetSaveData().abilities.radio_special)
                    SaveDataController.instance.GiveAbility("radio_special"); //If radio_special has not already been unlocked, set to true
                SaveDataController.instance.SaveFile();
            }

            StartCoroutine(LoadBroadcastRoom());
        }
    }

    IEnumerator LoadBroadcastRoom()
    {
        yield return new WaitForSeconds(1f);

        CamEffectController.instance.effectOn = true;

        yield return new WaitForSeconds(2.5f); //change this to wait while any dialogue is playing

        FadeController.instance.StartFade(1.0f, 1f);

        while (FadeController.instance.isFading)
            yield return null;

        CamEffectController.instance.effectOn = false;
        SaveDataController.instance.SetSavePoint("BroadcastRoom", 0);
        PlayerController.instance.ToggleAvatar();
        SceneManager.LoadSceneAsync("BroadcastRoom");
    }
}
