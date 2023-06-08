using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : InteractObject
{
    [SerializeField] int healthAdd;

    public override void Interact()
    {
        Health playerHealth = PlayerController.instance.gameObject.GetComponent<Health>();
        if (playerHealth.CurrentHealth() < SaveDataController.instance.saveData.maxHealth)
        {
            playerHealth.Hurt(-healthAdd, false); //beautiful, makes perfect sense
            Destroy(gameObject);
        }

        PlayerController.instance.SetState(PlayerController.States.idle);
    }
}
