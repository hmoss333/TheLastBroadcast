using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] int health;


    public void Hurt(int value)
    {
        health -= value;
        print($"{gameObject.name} health = {health}");

        if (health <= 0)
        {
            print($"{gameObject.name} has been destroyed");
            gameObject.SetActive(false);
        }
    }
}
