using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorController : InteractObject
{
    [Header("Equipment References")]
    [SerializeField] TranscieverController transciever;
    [SerializeField] AntennaController antenna;

    public override void Interact()
    {
        if (active && !hasActivated)
        {
            base.Interact();
        }
    }

    public override void StartInteract()
    {
        //base.StartInteract();
        PlayerController.instance.ToggleAvatar();
        CameraController.instance.SetTarget(interacting ? focusPoint : PlayerController.instance.gameObject);
        CameraController.instance.FocusTarget();

        transciever.Activate();
        antenna.Activate();

        UIController.instance.SetDialogueText("Power has been restored");
        UIController.instance.ToggleDialogueUI(true);
    }

    public override void EndInteract()
    {
        //if (activated)
        //{
        PlayerController.instance.ToggleAvatar();
        CameraController.instance.SetTarget(interacting ? focusPoint : PlayerController.instance.gameObject);
        CameraController.instance.FocusTarget();

        hasActivated = true;
        SaveDataController.instance.SaveObjectData(SaveDataController.instance.saveData.currentScene);

        UIController.instance.ToggleDialogueUI(false);
        //}
    }
}
