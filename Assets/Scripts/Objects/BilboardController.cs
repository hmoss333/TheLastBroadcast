using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BilboardController : InteractObject
{
    public static BilboardController instance;

    [SerializeField] GameObject bilboardPrefab;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }

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

        bilboardPrefab.SetActive(interacting);
    }
}
