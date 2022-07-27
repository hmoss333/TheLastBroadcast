using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TransmitterController : InteractObject
{
    public static TransmitterController instance;

    [SerializeField] GameObject focusPoint;
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
        base.Interact();

        PlayerController.instance.ToggleAvatar();
        CameraController.instance.SetTarget(interacting ? focusPoint : PlayerController.instance.gameObject);
        CameraController.instance.FocusTarget();

        if (active)
        {
            //base.Interact();

            //PlayerController.instance.ToggleAvatar();
            //CameraController.instance.SetTarget(interacting ? focusPoint : PlayerController.instance.gameObject);
            //CameraController.instance.FocusTarget();

            //TODO
            SaveDataController.instance.EnableStation(sceneToActivate);
            active = false;
        }
    }
}
