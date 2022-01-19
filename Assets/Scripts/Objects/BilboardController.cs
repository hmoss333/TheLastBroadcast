using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BilboardController : InteractObject
{
    public static BilboardController instance;

    [SerializeField] GameObject focusPoint;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }

    public override void Interact()
    {
        base.Interact();

        PlayerController.instance.ToggleAvatar();
        CameraController.instance.SetTarget(interacting ? focusPoint : PlayerController.instance.gameObject);
        CameraController.instance.FocusTarget();
    }
}
