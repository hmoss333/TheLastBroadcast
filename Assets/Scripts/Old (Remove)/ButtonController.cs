using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : InteractObject
{
    [SerializeField] SaveObject[] objectsToActivate;
    [SerializeField] string triggerText;
    [SerializeField] float activateDelay = 0.5f;


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
        SetHasActivated();
    }

    IEnumerator ActivateObjects()
    {
        //Activate all interact objects in list
        for (int i = 0; i < objectsToActivate.Length; i++)
        {
            objectsToActivate[i].Activate();

            yield return new WaitForSeconds(activateDelay);
        }
    }
}
