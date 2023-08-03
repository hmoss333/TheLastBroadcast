using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StaticManAnimEventController : MonoBehaviour
{
    Coroutine killPlayer;

    //Attack Animation Logic
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


    //Death Animation logic
    public void HideAvatar()
    {
        GetComponentInParent<SaveObject>().SetHasActivated();
        CamEffectController.instance.ForceEffect(false);
    }

    public void StartDisolve()
    {
        print("TODO: add disolve shader here");
    }
}
