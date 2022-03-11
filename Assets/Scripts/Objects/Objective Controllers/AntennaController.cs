using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AntennaController : InteractObject
{
    public static AntennaController instance;

    [SerializeField] GameObject focusPoint;
    [SerializeField] TextMeshPro displayText;
    public float value;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    private void Update()
    {
        if (interacting)
        {
            float xInput = Input.GetAxis("Horizontal");
            value += (float)(xInput * Time.deltaTime);
            //value = (float)System.Math.Round(value, 2);

            displayText.text = ((float)System.Math.Round(value, 2)).ToString();
        }
    }

    public void SetDisplayColor(Color colorToSet)
    {
        displayText.color = colorToSet;
    }

    public override void Interact()
    {
        if (active)
        {
            base.Interact();

            PlayerController.instance.ToggleAvatar();
            CameraController.instance.SetTarget(interacting ? focusPoint : PlayerController.instance.gameObject);
            CameraController.instance.FocusTarget();
        }
    }
}
