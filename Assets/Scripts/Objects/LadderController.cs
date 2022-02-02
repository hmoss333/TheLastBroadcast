using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderController : InteractObject
{
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
        PlayerController.instance.OnLadder(transform.position, this);
    }
}
