using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticManAnimEventController : MonoBehaviour
{
    public void KillPlayer()
    {
        PlayerController.instance.dead = true;
    }
}
