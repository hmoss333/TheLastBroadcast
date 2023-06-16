using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractObject : SaveObject
{
    public bool interacting, hideOnLoad = false;
    public string inactiveText;
    public GameObject focusPoint;

    private void OnEnable()
    {
        if (hasActivated && hideOnLoad)
        {
            gameObject.SetActive(false);
        }
    }

    public virtual void Interact()
    {
        interacting = !interacting;

        if (active)
        {
            if (interacting)
            {
                StartInteract();
            }
            else
            {
                EndInteract();
            }
        }
        else
        {
            UIController.instance.SetDialogueText(inactiveText);
            UIController.instance.ToggleDialogueUI(interacting);
        }
    }

    public virtual void StartInteract()
    {
        //Used for logic at start of interaction
    }

    public virtual void EndInteract()
    {
        //Used for logic at end of interaction
    }
}
