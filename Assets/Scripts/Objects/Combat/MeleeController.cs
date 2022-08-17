using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeController : MonoBehaviour
{
    [SerializeField] LayerMask interactLayers;
    [SerializeField] int damage;
    bool isHitting;


    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Destructable") && !isHitting)//interactLayers)
        {
            Health targetHealth = collision.GetComponent<Health>();
            if (targetHealth != null)
            {
                isHitting = true;
                targetHealth.Hurt(damage);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Destructable") && isHitting)//interactLayers)
        {
            isHitting = false;
        }
    }
}
