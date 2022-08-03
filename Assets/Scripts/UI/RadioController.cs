using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RadioController : MonoBehaviour
{
    public static RadioController instance;

    [SerializeField] float speed;
    [SerializeField] float yOffSet;
    [SerializeField] float maxFrequency;
    [Range(0.0f, 10.0f)]
    public float currentFrequency;
    public bool isActive, abilityMode;


    //UI Elements
    [SerializeField] GameObject overlayPanel;
    [SerializeField] Slider radioSlider;
    float xInput;
    [SerializeField] Image stationBackground;
    [SerializeField] Color onColor, offColor, abilityColor;


    //Audio Elements
    [SerializeField] AudioSource staticSource;
    [SerializeField] AudioSource stationSource;





    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }

    private void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //Lock station slider to only scroll within the min/max range
        float tempSpeed = speed;
        if (currentFrequency < 0.0f)
        {
            tempSpeed = 0.0f;
            currentFrequency = 0.0f;
        }
        else if (currentFrequency > maxFrequency)
        {
            tempSpeed = 0.0f;
            currentFrequency = maxFrequency;
        }
        else
        {
            tempSpeed = speed;
        }


        if (isActive)
        {
            //Get input values
            xInput = Input.GetAxis("Horizontal");
            currentFrequency += (float)(xInput * speed * Time.deltaTime);


            //Offset station slider element
            radioSlider.value = currentFrequency / 10f;


            if (SaveDataController.instance.saveData.abilities.radio_special)
                abilityMode = Input.GetButton("RadioSpecial");

            //TODO add radio ability controls here
        }

        staticSource.mute = !isActive;
        overlayPanel.SetActive(SaveDataController.instance.saveData.abilities.radio && !PlayerController.instance.interacting);
        if (!abilityMode)
            stationBackground.color = isActive ? onColor : offColor;
        else
            stationBackground.color = abilityColor;
    }

    public void ToggleOn()
    {
        isActive = !isActive;
    }
}
