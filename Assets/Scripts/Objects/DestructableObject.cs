using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableObject : SaveObject
{
    private Health health;

    private void Start()
    {
        health = GetComponent<Health>();
    }

    private void Update()
    {
        if (health.CurrentHealth() <= 0 || hasActivated)
        {
            SetHasActivated();
            gameObject.SetActive(false);
        }
    }
}
