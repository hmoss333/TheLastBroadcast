using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryPickup : InteractObject
{
    [SerializeField] int chargeAdd;

    public override void Interact()
    {
        float currentCharge = RadioController.instance.currentCharge;
        float maxCharge = RadioController.instance.maxCharge;

        if (currentCharge < maxCharge)
        {
            RadioController.instance.ModifyCharge(chargeAdd);
            Destroy(gameObject);
        }

        PlayerController.instance.SetState(PlayerController.States.idle);
    }
}
