using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class TransmitterController : InteractObject
{
    //[SerializeField] GameObject focusPoint;
    [SerializeField] enum AbilityToGive { Tune, Invisibility, Rats };
    [SerializeField] AbilityToGive abilityToGive;
    [SerializeField] string sceneToActivate;


    public override void Interact()
    {
        if (active && !hasActivated)
        {
            base.Interact();

            PlayerController.instance.ToggleAvatar();
            CameraController.instance.SetTarget(interacting ? focusPoint : PlayerController.instance.gameObject);
            CameraController.instance.FocusTarget();

            CamEffectController.instance.SetEffectValues(interacting);
        }
    }

    public override void StartInteract()
    {
        Debug.Log($"Actived transmitter for {abilityToGive} station");
        SaveDataController.instance.EnableStation(sceneToActivate); //Activate new scene station
        if (!SaveDataController.instance.GetSaveData().abilities.radio_special)
            SaveDataController.instance.GiveAbility("radio_special"); //If radio_special has not already been unlocked, set to true
        SaveDataController.instance.GiveRadioAbility(abilityToGive.ToString()); //Give new ability station
        SaveDataController.instance.SaveFile();
    }

    public override void EndInteract()
    {
        hasActivated = true;
        SaveDataController.instance.SaveObjectData(SceneManager.GetActiveScene().name); //Save object states
    }
}
