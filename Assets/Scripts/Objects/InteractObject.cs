using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractObject : MonoBehaviour
{
    public bool interacting, singleUse, activated;
    public Material defaultMat, activatedMat;
    public GameObject activatedLight;
    public Renderer alRenderer;

    public enum Station { preset1, preset2, preset3 };
    public Station station;
    string currentRadioStation;


    // Start is called before the first frame update
    void Start()
    {
        if (!activatedLight)
            activatedLight = this.gameObject;
    }

    // Update is called once per frame
    public virtual void Update()
    {
        currentRadioStation = RadioController.instance.station.ToString();

        activated = currentRadioStation == station.ToString() ? true : false;

        if (!alRenderer)
        {
            alRenderer = activatedLight.GetComponent<Renderer>();
        }
        else
        {
            alRenderer.material = activated ? activatedMat : defaultMat;
        }
    }

    public virtual void Interact()
    {
        interacting = !interacting;
        PlayerController.instance.interacting = interacting;

        if (interacting)
        {
            Debug.Log("Started interacting with " + name);
        }
        else
        {
            Debug.Log("Finished interacting with " + name);
        }
    }
}
