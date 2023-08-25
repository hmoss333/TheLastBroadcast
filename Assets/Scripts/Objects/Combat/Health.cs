using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] int health;
    [SerializeField] bool shockEffect, isHit;
    [SerializeField] float cooldownTime = 1f;
    CharacterController character;

    private void Start()
    {
        character = GetComponent<CharacterController>();
    }

    public void Hurt(int value, bool stagger)
    {
        if (!isHit)
        {
            isHit = true;
            health -= value;
            print($"{gameObject.name} health = {health}");

            if (shockEffect)
            {
                CamEffectController.instance.ShockEffect(0.25f);
            }

            if (health <= 0)
            {
                if (character != null)
                    character.dead = true;
            }
            else
            {
                if (character != null && stagger == true)
                    character.hurt = true;

                if (character.tag == "Player")
                    ScrollHealth.instance.toggled = true;

                StartCoroutine(HitCooldown(cooldownTime));
            }
        }
    }

    public void ModifyHealth(int value)
    {
        health += value;
        print($"{gameObject.name} health = {health}");

        if (shockEffect)
        {
            CamEffectController.instance.ShockEffect(0.25f);
        }

        if (character != null && health <= 0)
        {
            character.dead = true;
        }
    }

    IEnumerator HitCooldown(float timer)
    {
        yield return new WaitForSeconds(timer);

        isHit = false;
    }

    public int CurrentHealth()
    {
        return health;
    }

    public void SetHealth(int value)
    {
        health = value;
    }
}
