using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerZone : InteractObject
{
    [SerializeField] GameObject[] objectsToActivate;
    [SerializeField] bool focusCam;


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
            hasActivated = true;
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

        CameraController.instance.LoadLastTarget();//.SetTarget(PlayerController.instance.gameObject);
    }
}
