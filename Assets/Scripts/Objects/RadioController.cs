using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadioController : InteractObject
{
    public static RadioController instance;

    [SerializeField] GameObject radioPrefab;

    public float powerLevel;
    public float antennaLevel;
    public int stationSetting;
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
    public override void Update()
    {
        //TODO put radio controls here
        base.Update();
    }

    public override void Interact()
    {
        base.Interact();

        radioPrefab.SetActive(interacting);
    }
}
