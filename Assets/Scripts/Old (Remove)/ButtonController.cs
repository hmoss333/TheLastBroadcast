using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : InteractObject
{
    //[SerializeField] bool focus;
    [SerializeField] bool activating;
    [SerializeField] GameObject[] objectsToActivate;
    [SerializeField] string triggerText;



    public override void Interact()
    {
        if (active && !hasActivated && !activating)
            base.Interact();
    }

    public override void StartInteract()
    {
        UIController.instance.SetDialogueText(triggerText);
        UIController.instance.ToggleDialogueUI(true);

        StartCoroutine(ActivateObjects());
    }

    public override void EndInteract()
    {
        UIController.instance.ToggleDialogueUI(false);
        CameraController.instance.SetTarget(PlayerController.instance.gameObject);
        SetHasActivated();
    }

    IEnumerator ActivateObjects()
    {
        activating = true;

        //Activate all interact objects in list
        for (int i = 0; i < objectsToActivate.Length; i++)
        {
            InteractObject tempInteract = objectsToActivate[i].GetComponent<InteractObject>();
            CameraController.instance.SetTarget(tempInteract != null && tempInteract.focusPoint != null ? tempInteract.focusPoint : objectsToActivate[i].gameObject);

            if (tempInteract != null)
                tempInteract.Activate();
            else
                objectsToActivate[i].SetActive(!objectsToActivate[i].activeSelf);

            yield return new WaitForSeconds(1.5f);
        }

        if (CameraController.instance.GetLastTarget() != null)
            CameraController.instance.LoadLastTarget();
        else
            CameraController.instance.SetTarget(PlayerController.instance.gameObject);

        activating = false;
    }
}
