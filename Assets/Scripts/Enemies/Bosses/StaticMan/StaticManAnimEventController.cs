using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class StaticManAnimEventController : MonoBehaviour
{
    Coroutine killPlayer;

    [Header("Event Triggers")]
    [FormerlySerializedAs("onTrigger")]
    public UnityEvent m_OnTrigger = new UnityEvent();


    //Spawn Animation Logic
    public void FocusAvatar()
    {
        if (!CameraController.instance.GetRotState())
        {
            CameraController.instance.SetTarget(transform);
            PlayerController.instance.SetState(PlayerController.States.listening);
        }
    }

    public void EndFocusAvatar()
    {
        if (!CameraController.instance.GetRotState())
        {
            CameraController.instance.LoadLastTarget();
            PlayerController.instance.SetState(PlayerController.States.idle);
        }
    }


    //Attack Animation Logic
    public void KillPlayer()
    {
        if (killPlayer == null)
            killPlayer = StartCoroutine(KillPlayerRoutine());
    }

    IEnumerator KillPlayerRoutine()
    {
        CameraController.instance.SetTarget(transform);

        yield return new WaitForSeconds(0.5f);

        PlayerController.instance.dead = true;
        killPlayer = null;
    }


    //Death Animation logic
    public void HideAvatar()
    {
        GetComponentInParent<SaveObject>().SetHasActivated();
        CamEffectController.instance.SetEffectState(false);//.ForceEffect(false);
    }


    public void TriggerEvents()
    {
        m_OnTrigger.Invoke();
    }
}
