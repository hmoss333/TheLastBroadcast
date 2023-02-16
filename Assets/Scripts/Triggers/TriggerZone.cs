using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerZone : InteractObject
{
    [SerializeField] bool focusCam;
    [SerializeField] GameObject[] objectsToActivate;


    private void OnEnable()
    {
        if (hasActivated)
        {
            StartCoroutine(UnlockObjects(false));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && active && !hasActivated)
        {
            //hasActivated = true;
            SetHasActivated();
            StartCoroutine(UnlockObjects(focusCam));
        }
    }

    IEnumerator UnlockObjects(bool focusCamera)
    {
        for (int i = 0; i < objectsToActivate.Length; i++)
        {
            if (focusCamera)
                CameraController.instance.SetTarget(objectsToActivate[i].gameObject);

            InteractObject tempInteract = objectsToActivate[i].GetComponent<InteractObject>();
            if (tempInteract != null)
                tempInteract.Activate();
            else
                objectsToActivate[i].SetActive(!objectsToActivate[i].activeSelf);

            yield return new WaitForSeconds(1.25f);
        }

        print($"Cam Last target: {CameraController.instance.GetLastTarget().name}");
        if (CameraController.instance.GetLastTarget() != null)
            CameraController.instance.LoadLastTarget();
        else
            CameraController.instance.SetTarget(PlayerController.instance.gameObject);
    }
}
