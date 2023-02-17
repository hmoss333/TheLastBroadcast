using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightController : MonoBehaviour
{
    public static FlashlightController instance;

    public bool isOn { get; private set; }
    [SerializeField] Light lightSource;
    [SerializeField] GameObject flashlightObj;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        //else
        //    Destroy(this.gameObject);
            
    }

    private void Start()
    {
        isOn = false;
        lightSource.enabled = false;
        flashlightObj.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (SaveDataController.instance.saveData.abilities.flashlight && PlayerController.instance.state != PlayerController.States.radio)
        {
            isOn = PlayerController.instance.inputMaster.Player.Flashlight.ReadValue<float>() > 0
                ? true
                : false;
        }

        lightSource.enabled = isOn;
        flashlightObj.SetActive(isOn);
        PlayerController.instance.animator.SetBool("flashlight", isOn);
    }
}
