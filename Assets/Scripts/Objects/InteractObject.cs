using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractObject : SaveObject
{
    public bool interacting, hideOnLoad = false;
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

        if (interacting)
        {
            StartInteract();
        }
        else
        {
            EndInteract();
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
