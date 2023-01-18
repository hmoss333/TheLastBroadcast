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
        PlayerController.instance.InteractToggle(interacting);

        if (interacting)
        {
            Debug.Log($"Started interacting with {name}");
        }
        else
        {
            Debug.Log($"Finished interacting with {name}");
        }
    }

    public virtual void Activate()
    {
        active = true;
    }
}
