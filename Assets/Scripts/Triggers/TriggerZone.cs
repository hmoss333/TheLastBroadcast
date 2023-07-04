using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerZone : InteractObject
{
    [SerializeField] SaveObject[] objectsToActivate;
    Coroutine unlockRoutine;


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && active && !hasActivated)
        {
            //SetHasActivated();
            StartCoroutine(UnlockObjects());
        }
    }

    IEnumerator UnlockObjects()
    {
        for (int i = 0; i < objectsToActivate.Length; i++)
        {
            print($"Activating object: {objectsToActivate[i].name}");

            objectsToActivate[i].Activate();

            yield return new WaitForSeconds(1.25f);
        }

        //try
        //{
        //    print($"Cam Last target: {CameraController.instance.GetLastTarget().name}");
        //    if (CameraController.instance.GetLastTarget() != null)
        //        CameraController.instance.LoadLastTarget();
        //    else
        //        CameraController.instance.SetTarget(PlayerController.instance.gameObject);
        //}
        //catch { }

        SetHasActivated();
    }
}
