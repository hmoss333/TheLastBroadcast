using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : InteractObject
{
    [SerializeField] SaveObject[] objectsToActivate;
    [SerializeField] string triggerText;


    public override void Interact()
    {
        if (active && !hasActivated)
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
        //hasActivated = true;
    }

    IEnumerator ActivateObjects()
    {
        //Activate all interact objects in list
        for (int i = 0; i < objectsToActivate.Length; i++)
        {
            objectsToActivate[i].Activate();

            //SaveObject tempObject = objectsToActivate[i].GetComponent<SaveObject>();
            //if (tempObject != null)
            //    tempObject.Activate();
            //else
            //    objectsToActivate[i].SetActive(!objectsToActivate[i].activeSelf);

            yield return new WaitForSeconds(1.5f);
        }

        if (CameraController.instance.GetLastTarget() != null)
            CameraController.instance.LoadLastTarget();
        else
            CameraController.instance.SetTarget(PlayerController.instance.gameObject);
    }
}
