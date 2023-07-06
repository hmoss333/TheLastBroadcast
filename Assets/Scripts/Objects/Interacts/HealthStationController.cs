using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthStationController : InteractObject
{
    [SerializeField] private float waitTime;
    private float tempTime = 0f;
    private Health playerHealth;

    private void Start()
    {
        playerHealth = PlayerController.instance.gameObject.GetComponent<Health>();
    }

    private void Update()
    {
        if (interacting && playerHealth.CurrentHealth() < 5)
        {
            tempTime += Time.deltaTime;
            if (tempTime >= waitTime)
            {
                int currentHealth = playerHealth.CurrentHealth();
                //print($"Player health = {currentHealth++}");
                playerHealth.SetHealth(currentHealth++);
                CamEffectController.instance.ShockEffect(0.25f);
                tempTime = 0;
            }
        }
    }

    public override void Interact()
    {
        base.Interact();
    }
}
