using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadioController : InteractObject
{
    public static RadioController instance;

    [SerializeField] GameObject radioPrefab;

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
        base.Interact();

        radioPrefab.SetActive(interacting);
    }
}
