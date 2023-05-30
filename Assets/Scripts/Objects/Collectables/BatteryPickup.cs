using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryPickup : InteractObject
{
    //[SerializeField] int healthAdd;

    public override void Interact()
    {
        //Health playerHealth = PlayerController.instance.gameObject.GetComponent<Health>();
        //if (playerHealth.CurrentHealth() < 5)
        //{
        //    playerHealth.Hurt(-healthAdd, false);
        //    this.gameObject.SetActive(false);
        //}

        PlayerController.instance.SetState(PlayerController.States.idle);
    }
}
