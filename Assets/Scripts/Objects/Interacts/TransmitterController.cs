using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Windows;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class TransmitterController : InteractObject
{
    [Header("Transmitter Frequency Values")]
    [SerializeField] float targetFrequency;
    [Range(0f, 10f)] float currentFrequency;
    [SerializeField] float minTarget, maxTarget, rotSpeed, offSet;
    private float xInput;
    private bool startCountdown = false;
    [SerializeField] float countdownTime = 3f;
    private bool triggered;
    Coroutine triggerRoutine;

    [Header("UI Values")]
    [SerializeField] TextMeshPro frequencyText;
    [SerializeField] MeshRenderer lightMesh;
    [SerializeField] GameObject dialObj;
    [SerializeField] Color stationColor;
    [SerializeField] Color presetColor;
    [SerializeField] string activationText;

    [Header("Audio Values")]
    [SerializeField] AudioSource staticSource;


    [FormerlySerializedAs("onTrigger")]
    [SerializeField]
    private UnityEvent m_OnTrigger = new UnityEvent();



    private void Start()
    {
        targetFrequency = Random.Range(minTarget, maxTarget);
        frequencyText.gameObject.SetActive(false);
    }

    private void Update()
    {
        lightMesh.material.color = active ? Color.green : Color.red;
        staticSource.mute = !active;

        //Lock rotation once the player reaches either end of frequency spectrum
        float tempSpeed = rotSpeed;
        if (currentFrequency <= 0.0f || currentFrequency >= 10f)
        {
            tempSpeed = 0.0f;
            currentFrequency = currentFrequency <= 0.0f ? 0.0f : 10f;
        }
        else
        {
            tempSpeed = rotSpeed;
        }



        //Interact Logic
        if (interacting && !hasActivated)
        {
            frequencyText.color = stationColor;
            xInput = PlayerController.instance.inputMaster.Player.Move.ReadValue<Vector2>().x;
            dialObj.transform.Rotate(0.0f, xInput * tempSpeed, 0.0f);
            currentFrequency += (float)(xInput * Time.deltaTime);

            if (currentFrequency >= targetFrequency - offSet && currentFrequency <= targetFrequency + offSet)
            {
                frequencyText.color = presetColor;
                lightMesh.material.color = presetColor;
                if (currentFrequency >= targetFrequency - 0.15f && currentFrequency <= targetFrequency + 0.15f)
                {
                    startCountdown = true;
                }
                else
                {
                    startCountdown = false;
                }
            }
            else
            {
                startCountdown = false;
            }

            float displayFrequency = currentFrequency * 10f;
            frequencyText.text = displayFrequency.ToString("F1");
            CamEffectController.instance.SetEffectState(startCountdown);
        }


        //Countdown Timer
        if (startCountdown)
        {
            countdownTime -= Time.deltaTime;
            if (countdownTime < 0)
            {
                startCountdown = false;
                triggered = true;
                UIController.instance.SetDialogueText(activationText, false);
                UIController.instance.ToggleDialogueUI(true);
            }
        }
        else
        {
            countdownTime = 3f;
        }

        frequencyText.gameObject.SetActive(interacting);
    }

    public override void Interact()
    {
        if (!startCountdown && !hasActivated)
        {
            base.Interact();

            if (active)
            {
                currentFrequency = 0.0f;
                PlayerController.instance.ToggleAvatar();
                CameraController.instance.SetTarget(interacting ? focusPoint : CameraController.instance.GetLastTarget());
                CameraController.instance.FocusTarget();
                if (CameraController.instance.GetTriggerState())
                    CameraController.instance.SetRotation(true);
            }
        }

        staticSource.mute = !interacting;
    }

    public override void EndInteract()
    {
        base.EndInteract();

        CamEffectController.instance.SetEffectState(false);
        UIController.instance.ToggleDialogueUI(false);
        if (triggered)
        {
            SetHasActivated();
            m_OnTrigger.Invoke();
        }
    }
}
