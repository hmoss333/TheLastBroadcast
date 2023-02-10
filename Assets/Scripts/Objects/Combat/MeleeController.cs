using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeController : MonoBehaviour
{
    [SerializeField] List<string> tags;
    public int damage;

    private void OnTriggerEnter(Collider collision)
    {
        if (tags.Contains(collision.tag))
        {
            Health targetHealth = collision.GetComponent<Health>();
            if (targetHealth != null)
            {
                targetHealth.Hurt(damage, true);
            }
        }
    }
}
