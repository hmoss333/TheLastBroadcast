using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticManAnimEventController : MonoBehaviour
{
    public void KillPlayer()
    {
        PlayerController.instance.dead = true;
    }

    public void StartDisolve()
    {
        print("TODO: add disolve shader here");
    }
}
