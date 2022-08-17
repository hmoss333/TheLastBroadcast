using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeController : MonoBehaviour
{
    [SerializeField] List<string> tags;
    [SerializeField] int damage;
    Collider col;


    private void Start()
    {
        col = GetComponent<Collider>();
    }

    private void OnEnable()
    {
        col.enabled = true;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (tags.Contains(collision.tag))
        {
            Health targetHealth = collision.GetComponent<Health>();
            if (targetHealth != null)
            {
                col.enabled = false;
                targetHealth.Hurt(damage);
            }
        }
    }
}
