using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : InteractObject
{
    [SerializeField] int healthAdd;

    public override void Interact()
    {
        Health playerHealth = PlayerController.instance.gameObject.GetComponent<Health>();
        int maxHealth = SaveDataController.instance.saveData.maxHealth;
        if (playerHealth.CurrentHealth() < maxHealth)
        {
            if (playerHealth.CurrentHealth() + healthAdd > maxHealth)
                playerHealth.SetHealth(maxHealth);
            else
                playerHealth.Hurt(-healthAdd, false); //beautiful, makes perfect sense

            Destroy(gameObject);
        }

        PlayerController.instance.SetState(PlayerController.States.idle);
    }
}
