using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeController : MonoBehaviour
{
    [SerializeField] List<string> tags;
    public int damage;
    Collider meleeCollider;


    private void Start()
    {
        meleeCollider = GetComponent<Collider>();
    }

    private void OnEnable()
    {
        meleeCollider.enabled = true;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (tags.Contains(collision.tag))
        {
            Health targetHealth = collision.GetComponent<Health>();
            if (targetHealth != null)
            {
                targetHealth.Hurt(damage);
                meleeCollider.enabled = false;
            }
        }
    }
}
