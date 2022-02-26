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

    public override void Interact()
    {
        if (active)
            base.Interact();
    }
}
