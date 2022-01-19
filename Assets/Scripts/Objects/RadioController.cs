using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadioController : InteractObject
{
    public static RadioController instance;

    //[SerializeField] GameObject radioPrefab;
    [SerializeField] GameObject focusPoint;

    public float powerLevel;
    public float antennaLevel;
    public int stationSetting;
    public int channel;

    [SerializeField] AudioSource staticSource;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }

    public override void Interact()
    {
        if (activated)
        {
            base.Interact();

            PlayerController.instance.ToggleAvatar();
            CameraController.instance.SetTarget(interacting ? focusPoint : PlayerController.instance.gameObject);
            CameraController.instance.FocusTarget();
        }

        staticSource.mute = !interacting;
    }
}
