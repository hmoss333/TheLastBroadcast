using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightTrigger : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            if (other.GetComponent<CharacterController>().SeePlayer())
            {
                other.GetComponent<CharacterController>().StunCharacter();
            }
        }
    }
}
