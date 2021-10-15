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

    // Update is called once per frame
    public override void Update()
    {
        //TODO put radio controls here
        base.Update();
    }

    public override void Interact()
    {
        if (activated)
            base.Interact();
    }
}
