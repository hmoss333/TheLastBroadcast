using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PSController : InteractObject
{
    public static PSController instance;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Interact()
    {
        if (triggered)
            base.Interact();
    }

    public override void Trigger()
    {
        base.Trigger();

        gameObject.GetComponent<Renderer>().material = triggerMaterial;
    }
}
