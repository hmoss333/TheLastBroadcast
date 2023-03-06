using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AntennaController : InteractObject
{
    [SerializeField] private bool turnedOn;
    [SerializeField] private float speed, currentValue, targetValue, offset;
    [SerializeField] float waitTime, checkTime;
    [SerializeField] GameObject miniGameUI;
    [SerializeField] Image miniGameLight;
    [SerializeField] Slider miniGameSlider;


    private void Start()
    {
        targetValue = Random.Range(0f, 10f);
        miniGameSlider.maxValue = 10f;
    }

    private void Update()
    {
        if (interacting && !turnedOn)
        {
            float antennaValue = PlayerController.instance.inputMaster.Player.Move.ReadValue<Vector2>().y;
            currentValue += antennaValue * speed * Time.deltaTime;
            miniGameSlider.value = currentValue;
            if (currentValue <= targetValue + offset && currentValue >= targetValue - offset)
            {
                miniGameLight.color = Color.yellow;
                checkTime += Time.deltaTime;
                if (checkTime >= waitTime)
                {
                    TurnOn();
                }
            }
            else
            {
                miniGameLight.color = Color.red;
                checkTime = 0f;
            }
        }

        miniGameUI.SetActive(interacting && !turnedOn);
    }

    public override void Interact()
    {
        if (active && !hasActivated)
        {
            base.Interact();

            PlayerController.instance.ToggleAvatar();
            CameraController.instance.SetTarget(interacting ? focusPoint : PlayerController.instance.gameObject);
            CameraController.instance.FocusTarget();
        }
    }

    public override void StartInteract()
    {
        currentValue = 0f;
        checkTime = 0f;
    }

    public override void EndInteract()
    {
        if (turnedOn)
        {
            hasActivated = true;
            SaveDataController.instance.SaveObjectData(SaveDataController.instance.saveData.currentScene);
            UIController.instance.ToggleDialogueUI(false);
        }
    }

    void TurnOn()
    {
        miniGameLight.color = Color.green;
        UIController.instance.SetDialogueText("Antenna configured");
        UIController.instance.ToggleDialogueUI(true);
        turnedOn = true;
    }
}
