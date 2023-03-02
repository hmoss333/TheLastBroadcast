using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntennaController : InteractObject
{
    public override void Interact()
    {
        if (active && !hasActivated)
        {
            base.Interact();

            PlayerController.instance.ToggleAvatar();
            CameraController.instance.SetTarget(interacting ? focusPoint : PlayerController.instance.gameObject);
            CameraController.instance.FocusTarget();
        }

        //staticSource.mute = !interacting;
    }

    public override void EndInteract()
    {
        //if (activated)
        //{
        hasActivated = true;
        SaveDataController.instance.SaveObjectData(SaveDataController.instance.saveData.currentScene);
        //}
    }
}
