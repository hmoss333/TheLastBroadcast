using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class DestructableObject : SaveObject
{
    private Health health;
    

    private void Start()
    {
        health = GetComponent<Health>();

        gameObject.SetActive(!hasActivated);
    }

    private void Update()
    {
        if (health.currentHealth <= 0 && !hasActivated)
        {
            SetHasActivated();
            gameObject.SetActive(false);
        }
    }
}
