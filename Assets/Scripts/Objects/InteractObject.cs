using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractObject : MonoBehaviour
{
    //public int objID;
    public bool interacting, active = true, hasActivated;

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

    public virtual void Activate()
    {
        active = true;
    }
}
