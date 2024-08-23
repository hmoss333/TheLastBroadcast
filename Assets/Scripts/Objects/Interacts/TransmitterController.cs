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
    [Header("Equipment References")]
    [SerializeField] GeneratorController generator;
    [SerializeField] AntennaController antenna;

    [Header("Transmitter Frequency Values")]
    [SerializeField][Range(0f, 10f)] float currentFrequency;
    [SerializeField] float targetFrequency;
    [SerializeField] float minTarget, maxTarget, rotSpeed, offSet;
    private float xInput;
    private bool startCountdown = false;
    [SerializeField] float countdownTime = 3f, triggerDelay = 1.5f;
    private bool triggered;

    [Header("UI Values")]
    [SerializeField] TextMeshPro frequencyText;
    [SerializeField] MeshRenderer lightMesh;
    [SerializeField] GameObject dialObj;
    [SerializeField] SpriteRenderer arrowLeft, arrowRight;
    [SerializeField] Color arrowDefault, arrowActive;
    [SerializeField] Color stationColor;
    [SerializeField] Color presetColor;

    [Header("Audio Values")]
    [SerializeField] AudioSource staticSource;



    private void Start()
    {
        targetFrequency = Random.Range(minTarget, maxTarget);
        frequencyText.gameObject.SetActive(false);
    }

    private void Update()
    {
        lightMesh.material.color = active ? Color.green : Color.red;
        staticSource.volume = Mathf.Lerp(staticSource.volume, interacting ? 0.5f : 0.125f, 10f);
        arrowLeft.gameObject.SetActive(interacting);
        arrowRight.gameObject.SetActive(interacting);

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
            xInput = InputController.instance.inputMaster.Player.Move.ReadValue<Vector2>().x;
            dialObj.transform.Rotate(0.0f, xInput * tempSpeed, 0.0f);
            currentFrequency += (float)(xInput * Time.deltaTime);
            arrowLeft.color = xInput < 0 ? arrowActive : arrowDefault;
            arrowRight.color = xInput > 0 ? arrowActive : arrowDefault;


            if (currentFrequency >= targetFrequency - offSet && currentFrequency <= targetFrequency + offSet
                && generator.hasActivated
                && antenna.hasActivated)
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
                CamEffectController.instance.SetEffectState(false);
                interacting = false;
                SetHasActivated();
                StartCoroutine(TurnOnRoutine());
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

            if (active && interacting)
            {
                currentFrequency = 0.0f;
                CameraController.instance.SetTarget(focusPoint);
                CameraController.instance.SetRotation(true);
                CameraController.instance.transform.position = focusPoint.position;
            }
            else
            {
                CameraController.instance.LoadLastTarget();
                CameraController.instance.SetRotation(CameraController.instance.GetTriggerState() ? true : false);
                PlayerController.instance.SetState(PlayerController.States.idle);
            }
        }
    }

    IEnumerator TurnOnRoutine()
    {
        yield return new WaitForSeconds(triggerDelay);

        CameraController.instance.LoadLastTarget();
        CameraController.instance.SetRotation(CameraController.instance.GetTriggerState() ? true : false);
        PlayerController.instance.SetState(PlayerController.States.idle);

        m_OnTrigger.Invoke();
    }
}
