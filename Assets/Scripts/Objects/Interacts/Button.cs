using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : InteractObject
{
    [SerializeField] GameObject[] objectsToActivate;
    [SerializeField] bool focusCam;

    private void OnEnable()
    {
        //If radioLock has already been triggered previously, interact with all objectsToActivate
        if (hasActivated)
        {
            StartCoroutine(UnlockObjects(false));
        }
    }

    public override void Interact()
    {
        if (active && !hasActivated)
        {
            hasActivated = true;
            StartCoroutine(UnlockObjects(focusCam));
        }
    }

    IEnumerator UnlockObjects(bool focusCamera)
    {
        print("Unlocking objects");
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

        if (CameraController.instance.GetLastTarget() != null)
            CameraController.instance.LoadLastTarget();
        else
            CameraController.instance.SetTarget(PlayerController.instance.gameObject);
    }
}
