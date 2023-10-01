using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using static UnityEngine.UI.Button;
using UnityEngine.Serialization;
using static UnityEngine.Rendering.DebugUI;

public class TriggerZone : InteractObject
{
    Coroutine unlockRoutine;


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player"
            && active
            && !hasActivated
            && unlockRoutine == null)
        {
            m_OnTrigger.Invoke();
            SetHasActivated();
        }
    }
}
