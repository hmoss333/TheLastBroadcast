using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RadioOverlay_Controller : MonoBehaviour
{
    public static RadioOverlay_Controller instance;

    [SerializeField] float speed;
    [SerializeField] float yOffSet;
    [SerializeField] float maxFrequency;
    [Range(0.0f, 10.0f)]
    public float currentFrequency;
    public bool isActive;
    [SerializeField] float stationOffset;


    //UI Elements
    [SerializeField] GameObject overlayPanel;
    [SerializeField] RawImage stationScroll;
    [SerializeField] TextMeshProUGUI frequencyText;
    float xInput;

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
            //Display the current frequency
            float displayFrequency = currentFrequency * 10f;
            frequencyText.text = displayFrequency.ToString("F1");


            //Get input values
            xInput = Input.GetAxis("Horizontal");
            currentFrequency += (float)(xInput * Time.deltaTime);


            //Offset station strip element
            float xPos = stationScroll.uvRect.x;
            stationScroll.uvRect = new Rect(xPos += tempSpeed * xInput * Time.deltaTime, 0, 1, 1);
        }

        staticSource.mute = !isActive;
        overlayPanel.SetActive(isActive);
    }

    public void ToggleOn()
    {
        isActive = !isActive;
    }
}
