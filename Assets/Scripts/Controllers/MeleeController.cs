using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeController : MonoBehaviour
{
    bool hit;

    private void OnEnable()
    {
        hit = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Destructable" && !hit)
        {
            hit = true;
            other.GetComponent<DestructableObject>().Hit();
        }
    }
}
