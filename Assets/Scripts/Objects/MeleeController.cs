using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeController : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Destructable")
        {
            //DestructObject target = collision.GetComponent<DestructObject>();
            //target.Hit(damage);
        }
    }
}
