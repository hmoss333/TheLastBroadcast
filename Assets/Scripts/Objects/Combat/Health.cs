using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] int health;
    [SerializeField] bool shockEffect;

    public void Hurt(int value)
    {
        health -= value;
        print($"{gameObject.name} health = {health}");
        CharacterController character = GetComponent<CharacterController>();

        if (shockEffect)
        {
            CamEffectController.instance.ShockEffect(0.25f);
        }

        if (health <= 0)
        {
            if (character != null)
                character.dead = true;
            else
                gameObject.SetActive(false);
        }
        else
        {
            if (character != null)
                character.hurt = true;
        }
    }

    public int CurrentHealth()
    {
        return health;
    }
}
