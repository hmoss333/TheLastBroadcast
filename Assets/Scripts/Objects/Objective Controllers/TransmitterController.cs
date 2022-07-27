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
                SaveDataController.instance.EnableStation(sceneToActivate);
                SaveDataController.instance.SaveFile();
            }
        }
    }
}
