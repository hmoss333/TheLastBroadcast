using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RadioController : InteractObject
{
    public static RadioController instance;

    //[SerializeField] GameObject radioPrefab;
    [SerializeField] GameObject activeModel;
    [SerializeField] GameObject deactivatedModel;
    [SerializeField] GameObject focusPoint;
    [SerializeField] AudioSource staticSource;

    [SerializeField][Range(0f, 10f)] float currentFrequency;
    [SerializeField] TextMeshPro frequencyText;

    [SerializeField] GameObject dialObj;
    [SerializeField] float rotSpeed;
    private float xInput;
    [SerializeField] float[] stationNums;
    //[SerializeField] float activeStation;
    int stationIndex;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);


        stationIndex = 0;
    }

    private void Update()
    {
        activeModel.SetActive(activated);
        deactivatedModel.SetActive(!activated);

        float tempSpeed = rotSpeed;
        if (currentFrequency < 0.0f)
        {
            tempSpeed = 0.0f;
            currentFrequency = 0.0f;
        }
        else if (currentFrequency > 10f)
        {
            tempSpeed = 0.0f;
            currentFrequency = 10f;
        }
        else
        {
            tempSpeed = rotSpeed;
        }


        if (interacting)
        {
            if (Input.GetKey("right shift"))
            {
                if (Input.GetKeyDown("left"))
                {
                    stationIndex--;
                    if (stationIndex < 0)
                        stationIndex = stationNums.Length - 1;

                    dialObj.transform.Rotate(0.0f, dialObj.transform.localEulerAngles.y - 25f, 0.0f);
                }
                if (Input.GetKeyDown("right"))
                {
                    stationIndex++;
                    if (stationIndex > stationNums.Length - 1)
                        stationIndex = 0;

                    dialObj.transform.Rotate(0.0f, dialObj.transform.localEulerAngles.y + 25f, 0.0f);
                }

                currentFrequency = stationNums[stationIndex];
            }
            else
            {
                xInput = Input.GetAxis("Horizontal");
                dialObj.transform.Rotate(0.0f, xInput * tempSpeed, 0.0f);
                currentFrequency += (float)(xInput * Time.deltaTime);
            }

            float displayFrequency = currentFrequency * 10f;
            frequencyText.text = displayFrequency.ToString("F1");
        }
    }

    public override void Interact()
    {
        if (activated)
        {
            base.Interact();

            PlayerController.instance.ToggleAvatar();
            CameraController.instance.SetTarget(interacting ? focusPoint : PlayerController.instance.gameObject);
            CameraController.instance.FocusTarget();
        }

        staticSource.mute = !interacting;
    }
}
