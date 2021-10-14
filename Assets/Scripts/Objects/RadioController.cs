using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadioController : InteractObject
{
    public static RadioController instance;

    [SerializeField] GameObject radioPrefab;

    public float powerLevel;
    public float antennaLevel;
    public float station;
    public int channel;


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
        //TODO put radio controls here
    }

    public override void Interact()
    {
        if (triggered)
        {
            base.Interact();

            radioPrefab.SetActive(interacting);
        }
    }

    public override void Trigger()
    {
        base.Trigger();

        gameObject.GetComponent<Renderer>().material = triggerMaterial; 
    }
}
