using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransmitterController : InteractObject
{
    public static TransmitterController instance;

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
        }
    }

    public override void Interact()
    {
        if (active)
        {
            base.Interact();
        }
    }
}
