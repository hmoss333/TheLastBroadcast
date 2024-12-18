using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RadioController : MonoBehaviour
{
    public static RadioController instance;


    [Header("Radio Control Variables")]
    [NaughtyAttributes.HorizontalLine]
    [SerializeField] private float speed;
    [SerializeField] private float maxVolume;
    [SerializeField] private float volRate;
    private float maxFrequency = 10f;
    public float maxCharge { get; private set; }
    public float currentCharge { get; private set; }
    public float currentFrequency { get; private set; }
    public bool isActive { get; private set; }
    public bool listening { get; private set; }
    public bool abilityMode { get; private set; }

    [Header("Radio UI Elements")]
    [NaughtyAttributes.HorizontalLine]
    [SerializeField] private GameObject overlayPanel;
    [SerializeField] private GameObject abilityPanel;
    [SerializeField] private GameObject tuneAbility, staticAbility, ratAbility;
    [SerializeField] private Slider radioSlider;
    [SerializeField] private Image stationBackground;
    private float xInput;
    [SerializeField] private Color onColor, offColor, abilityColor;
    [SerializeField] private Vector2 inactivePos, activePos, abilityInactivePos, abilityActivePos;
    [SerializeField] private float slideSpeed;
    [SerializeField] private Slider chargeSlider;

    [Header("Radio Audio Elements")]
    [NaughtyAttributes.HorizontalLine]
    [SerializeField] public AudioSource staticSource;



    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    private void Start()
    {
        maxCharge = SaveDataController.instance.saveData.maxCharge;
        currentCharge = maxCharge;
        chargeSlider.maxValue = maxCharge;       
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
            xInput = InputController.instance.inputMaster.Player.TuneRadio.ReadValue<Vector2>().x;
            currentFrequency += (float)(xInput * speed * Time.deltaTime);


            //Offset station slider element
            radioSlider.value = currentFrequency / 10f;


            if (SaveDataController.instance.saveData.abilities.radio_special)
                abilityMode = InputController.instance.inputMaster.Player.RadioSpecial.ReadValue<float>() > 0 ? true : false;

            if (staticSource.volume <= maxVolume)
            {
                staticSource.volume += Time.deltaTime * volRate;
            }
        }
        else
        {
            abilityMode = false;
            currentFrequency = 0f;
            staticSource.volume = 0;
        }

        staticSource.mute = !isActive;
        overlayPanel.SetActive(SaveDataController.instance.saveData.abilities.radio);

        if (!abilityMode)
            stationBackground.color = isActive ? onColor : offColor;
        else
            stationBackground.color = abilityColor;

        //Modify slider to match charge level
        chargeSlider.value = currentCharge;
        chargeSlider.gameObject.SetActive(SaveDataController.instance.saveData.abilities.radio_special);

        //Move radio panel into position based on active state
        overlayPanel.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(overlayPanel.GetComponent<RectTransform>().anchoredPosition, isActive || listening ? activePos : inactivePos, slideSpeed * Time.deltaTime);
        abilityPanel.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(abilityPanel.GetComponent<RectTransform>().anchoredPosition, abilityMode ? abilityActivePos : abilityInactivePos, slideSpeed * Time.deltaTime);

        //Toggle ability sprites
        tuneAbility.SetActive(SaveDataController.instance.GetRadioAbility("Tune").isActive);
        staticAbility.SetActive(SaveDataController.instance.GetRadioAbility("Invisibility").isActive);
        ratAbility.SetActive(SaveDataController.instance.GetRadioAbility("Rats").isActive);
    }

    public void SetActive(bool activeVal)
    {
        isActive = activeVal;
    }

    public void Listening(bool activeVal)
    {
        listening = activeVal;
        isActive = listening;
    }

    public void ModifyCharge(float chargeVal)
    {
        currentCharge += chargeVal;
        if (currentCharge >= maxCharge)
            currentCharge = maxCharge;
        else if (currentCharge < 0f)
            currentCharge = 0f;
    }

    public void UsingAbility()
    {
        CamEffectController.instance.SetEffectState(true);
    }
}
