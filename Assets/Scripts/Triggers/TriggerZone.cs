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
    [SerializeField] SaveObject[] objectsToActivate;
    Coroutine unlockRoutine;

    [FormerlySerializedAs("onTrigger")]
    [SerializeField]
    private UnityEvent m_OnTrigger = new UnityEvent();


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player"
            && active
            && !hasActivated
            && unlockRoutine == null)
        {
            unlockRoutine = StartCoroutine(UnlockObjects());
        }
    }

    IEnumerator UnlockObjects()
    {
        m_OnTrigger.Invoke();

        for (int i = 0; i < objectsToActivate.Length; i++)
        {
            print($"Activating object: {objectsToActivate[i].name}");

            objectsToActivate[i].Activate();

            yield return new WaitForSeconds(1.25f);
        }

        //SetHasActivated();
        active = false;
        unlockRoutine = null;
    }
}
