using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableObject : MonoBehaviour
{
    [SerializeField] int health;

    private void Update()
    {
        if (health <= 0)
        {
            this.gameObject.SetActive(false);
        }
    }

    public virtual void Hit()
    {
        health--;

        StartCoroutine(DamageFlash());
    }

    public virtual void Interact()
    {
        Debug.Log("Interacting with " + this.gameObject.name);
    }

    IEnumerator DamageFlash()
    {
        GetComponent<Renderer>().material.color = Color.red;

        yield return new WaitForSeconds(0.35f);

        GetComponent<Renderer>().material.color = Color.white;
    }
}
