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
        targetValue = Random.Range(4.5f, 10f);
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
                    interacting = false;
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

    void TurnOn()
    {
        miniGameLight.color = Color.green;
        turnedOn = true;
        SetHasActivated();
        SaveDataController.instance.SaveObjectData();

        StartCoroutine(TurnOnRoutine());
    }

    IEnumerator TurnOnRoutine()
    {
        yield return new WaitForSeconds(1.5f);

        turnedOn = true;
        PlayerController.instance.ToggleAvatar();
        CameraController.instance.SetTarget(interacting ? focusPoint : PlayerController.instance.gameObject);
        CameraController.instance.FocusTarget();
        PlayerController.instance.SetState(PlayerController.States.idle);
    }
}
