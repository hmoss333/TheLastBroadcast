using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AntennaController : InteractObject
{
    [SerializeField] private bool turnedOn;
    [SerializeField] private float speed, currentValue, targetValue, offset;
    [SerializeField] float waitTime, checkTime;
    [SerializeField] GameObject miniGameUI, miniGameLight;
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

        miniGameUI.SetActive(active && interacting && !turnedOn);
        if (!active)
            miniGameLight.GetComponent<Renderer>().material.color = Color.black;
        if (hasActivated)
            miniGameLight.GetComponent<Renderer>().material.color = Color.green;
    }

    public override void Interact()
    {
        base.Interact();

        if (active && !hasActivated)
        {
            PlayerController.instance.ToggleAvatar();
            CameraController.instance.SetTarget(interacting ? focusPoint : PlayerController.instance.lookTransform);//.gameObject);
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
        turnedOn = true;
        SetHasActivated();
        //SaveDataController.instance.SaveObjectData();

        StartCoroutine(TurnOnRoutine());
    }

    IEnumerator TurnOnRoutine()
    {
        yield return new WaitForSeconds(1.5f);

        turnedOn = true;
        PlayerController.instance.ToggleAvatar();
        CameraController.instance.SetTarget(interacting ? focusPoint : PlayerController.instance.lookTransform);
        CameraController.instance.FocusTarget();
        PlayerController.instance.SetState(PlayerController.States.idle);
    }
}
