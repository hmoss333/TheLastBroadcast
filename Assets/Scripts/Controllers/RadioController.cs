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
    [SerializeField] private float maxFrequency;
    [Range(0.0f, 10.0f)]
    public float currentFrequency;
    [HideInInspector] public bool isActive, abilityMode;//, stunEnemy;


    [Header("UI Elements")]
    [SerializeField] private GameObject overlayPanel;
    [SerializeField] private Slider radioSlider;
    [SerializeField] private Image stationBackground;
    private float xInput;
    [SerializeField] private Color onColor, offColor, abilityColor, enemyColor;
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
            xInput = PlayerController.instance.inputMaster.Player.TuneRadio.ReadValue<Vector2>().x;
            currentFrequency += (float)(xInput * speed * Time.deltaTime);


            //Offset station slider element
            radioSlider.value = currentFrequency / 10f;


            if (SaveDataController.instance.saveData.abilities.radio_special)
                abilityMode = PlayerController.instance.inputMaster.Player.RadioSpecial.ReadValue<float>() > 0 ? true : false;
        }

        staticSource.mute = !isActive;
        overlayPanel.SetActive(SaveDataController.instance.saveData.abilities.radio && !PlayerController.instance.interacting);

        //if (stunEnemy)
        //    stationBackground.color = enemyColor;
        //else
        if (!abilityMode)
            stationBackground.color = isActive ? onColor : offColor;
        else
            stationBackground.color = abilityColor; //usingAbility ? abilityOnColor : abilityOffColor;

        //Move radio panel into position based on active state
        overlayPanel.transform.localPosition = Vector2.Lerp(overlayPanel.transform.localPosition, isActive ? activePos : inactivePos, slideSpeed * Time.deltaTime);
    }

    //public void StunEnemy(bool isStunning)
    //{
    //    stunEnemy = isStunning;
    //}
}
