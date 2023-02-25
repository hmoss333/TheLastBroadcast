using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractObject : SaveObject
{
    public GameObject focusPoint;
    public bool interacting;

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
        //Debug.Log($"Started interacting with {name}");
    }

    public virtual void EndInteract()
    {
        //Debug.Log($"Finished interacting with {name}");
    }
}
