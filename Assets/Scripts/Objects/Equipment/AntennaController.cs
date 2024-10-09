using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class AntennaController : InteractObject
{
    [SerializeField] private float speed, currentValue, targetValue, offset;
    [SerializeField] float waitTime, checkTime;
    [SerializeField] GameObject miniGameUI, miniGameLight;
    [SerializeField] Slider miniGameSlider;
    AudioSource audioSource;
    [SerializeField] AudioClip activateClip;


    private void Start()
    {
        targetValue = Random.Range(4.5f, 10f);
        miniGameSlider.maxValue = 10f;
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = activateClip;
    }

    private void Update()
    {
        if (interacting && !hasActivated)
        {
            float antennaValue = InputController.instance.inputMaster.Player.Move.ReadValue<Vector2>().y;
            currentValue += antennaValue * speed * Time.deltaTime;
            miniGameSlider.value = currentValue;
            if (currentValue <= targetValue + offset && currentValue >= targetValue - offset)
            {
                miniGameLight.GetComponent<Renderer>().material.color = Color.yellow;
                checkTime += Time.deltaTime;
                if (checkTime >= waitTime)
                {
                    interacting = false;
                    TurnOn();
                }
            }
            else
            {
                miniGameLight.GetComponent<Renderer>().material.color = Color.red;
                checkTime = 0f;
            }
        }
        else
        {
            if (!active)
                miniGameLight.GetComponent<Renderer>().material.color = Color.black;
            else if (hasActivated)
                miniGameLight.GetComponent<Renderer>().material.color = Color.green;
            else
                miniGameLight.GetComponent<Renderer>().material.color = Color.red;
        }

        miniGameUI.SetActive(active && interacting && !hasActivated);
    }

    public override void Interact()
    {
        base.Interact();

        if (active && !hasActivated)
        {
            PlayerController.instance.ToggleAvatar();
            CameraController.instance.SetTarget(interacting ? focusPoint : PlayerController.instance.lookTransform);
            CameraController.instance.FocusTarget();
        }
    }

    public override void StartInteract()
    {
        currentValue = 0f;
        checkTime = 0f;
    }

    void TurnOn()
    {
        miniGameLight.GetComponent<Renderer>().material.color = Color.green;
        audioSource.Play();
        SetHasActivated();
        //SaveDataController.instance.SaveObjectData();

        StartCoroutine(TurnOnRoutine());
    }

    IEnumerator TurnOnRoutine()
    {
        yield return new WaitForSeconds(1.5f);

        PlayerController.instance.ToggleAvatar();
        CameraController.instance.SetTarget(interacting ? focusPoint : CameraController.instance.GetLastTarget());
        CameraController.instance.FocusTarget();
        if (CameraController.instance.GetTriggerState())
            CameraController.instance.SetRotation(true);
        PlayerController.instance.SetState(PlayerController.States.idle);

        m_OnTrigger.Invoke();
    }
}
