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

    [SerializeField] float currentFrequency;
    [SerializeField] float maxFrequency;
    [SerializeField] TextMeshPro frequencyText;

    [SerializeField] GameObject dialObj;
    [SerializeField] float rotSpeed;
    private float xInput;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
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
        else if (currentFrequency > maxFrequency)
        {
            tempSpeed = 0.0f;
            currentFrequency = maxFrequency;
        }
        else
        {
            tempSpeed = rotSpeed;
        }


        if (interacting)
        {
            xInput = Input.GetAxis("Horizontal");
            dialObj.transform.Rotate(0.0f, xInput * tempSpeed, 0.0f);
            currentFrequency += (float)(xInput * Time.deltaTime);

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
