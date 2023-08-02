using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticManAnimEventController : MonoBehaviour
{
    Coroutine killPlayer;


    public void KillPlayer()
    {
        if (killPlayer == null)
            killPlayer = StartCoroutine(KillPlayerRoutine());
    }

    IEnumerator KillPlayerRoutine()
    {
        CameraController.instance.SetTarget(this.gameObject);

        yield return new WaitForSeconds(0.25f);

        PlayerController.instance.dead = true;
        killPlayer = null;
    }


    public void StartDisolve()
    {
        print("TODO: add disolve shader here");
    }
}
