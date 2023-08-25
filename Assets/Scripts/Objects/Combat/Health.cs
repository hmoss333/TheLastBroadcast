using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int currentHealth { get; private set; }
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
            currentHealth -= value;
            print($"{gameObject.name} health = {currentHealth}");

            if (shockEffect)
            {
                CamEffectController.instance.ShockEffect(0.25f);
            }

            if (currentHealth <= 0)
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
        currentHealth += value;
        print($"{gameObject.name} health = {currentHealth}");

        if (shockEffect)
        {
            CamEffectController.instance.ShockEffect(0.25f);
        }

        if (character != null && currentHealth <= 0)
        {
            character.dead = true;
        }
    }

    IEnumerator HitCooldown(float timer)
    {
        yield return new WaitForSeconds(timer);

        isHit = false;
    }

    public void SetHealth(int value)
    {
        currentHealth = value;
    }
}
