using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (PlayerController.instance.abilityState != PlayerController.AbilityStates.invisible)
        {
            Vector3 playerPos = new Vector3(PlayerController.instance.transform.position.x, PlayerController.instance.transform.localPosition.y - 1f, PlayerController.instance.transform.position.z);
            transform.LookAt(playerPos);
        }
    }
}
