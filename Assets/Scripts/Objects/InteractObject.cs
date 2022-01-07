using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractObject : MonoBehaviour
{
    public bool interacting, singleUse, activated;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    public virtual void Update()
    {

    }

    public virtual void Interact()
    {
        interacting = !interacting;

        if (interacting)
        {
            Debug.Log("Started interacting with " + name);
        }
        else
        {
            Debug.Log("Finished interacting with " + name);
        }
    }

    public virtual void Activate()
    {
        activated = true;
    } 
}
