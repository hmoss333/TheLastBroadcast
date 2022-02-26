using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntennaController : InteractObject
{
    public static AntennaController instance;



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
