using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RadioController : MonoBehaviour
{
    public static RadioController instance;


    [Header("Radio Control Variables")]
    [SerializeField] private float speed;
    [SerializeField] private float yOffSet;
    [SerializeField] private float maxFrequency;
    [Range(0.0f, 10.0f)]
    public float currentFrequency;
    [HideInInspector] public bool isActive, abilityMode;


    [Header("UI Elements")]
    [SerializeField] private GameObject overlayPanel;
    [SerializeField] private GameObject radioPrefab;
    [SerializeField] private Slider radioSlider;
    [SerializeField] private Image stationBackground;
    private float xInput;
    [SerializeField] private Color onColor, offColor, abilityColor;
    [SerializeField] private Vector2 inactivePos, activePos;
    [SerializeField] private float slideSpeed;


    [Header("Audio Elements")]
    [SerializeField] private AudioSource staticSource;
    [SerializeField] private AudioSource stationSource;



    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
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
        }

        staticSource.mute = !isActive;
        overlayPanel.SetActive(SaveDataController.instance.saveData.abilities.radio && !PlayerController.instance.interacting);
        radioPrefab.SetActive(isActive);

        if (!abilityMode)
            stationBackground.color = isActive ? onColor : offColor;
        else
            stationBackground.color = abilityColor;

        //Move radio panel into position based on active state
        overlayPanel.transform.localPosition = Vector2.Lerp(overlayPanel.transform.localPosition, isActive ? activePos : inactivePos, slideSpeed * Time.deltaTime);
    }

    public void ToggleOn()
    {
        currentFrequency = 0.0f;
        isActive = !isActive;
    }
}
