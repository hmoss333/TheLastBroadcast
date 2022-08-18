using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] int health;
    [SerializeField] bool shockEffect;
    //[SerializeField] Animator animator;

    //private void Start()
    //{
    //    animator = GetComponentInChildren<Animator>();
    //}

    public void Hurt(int value)
    {
        health -= value;
        print($"{gameObject.name} health = {health}");
        //animator.SetTrigger("isHurt");

        if (shockEffect)
        {
            CamEffectController.instance.ShockEffect(0.25f);
        }

        if (health <= 0)
        {
            print($"{gameObject.name} has been destroyed");
            gameObject.SetActive(false);
        }
    }
}
