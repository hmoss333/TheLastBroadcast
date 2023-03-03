using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorController : InteractObject
{
    [Header("Equipment References")]
    [SerializeField] TranscieverController transciever;
    [SerializeField] AntennaController antenna;

    [Header("Interact Variables")]
    [SerializeField] GameObject miniGameUI;
    [SerializeField] bool playing, activated;
    [SerializeField] int hitCount;
    [SerializeField] [Range(0f, 10f)] float turnVal;
    [SerializeField] float turnSpeed, targetVal, offset;


    private void Start()
    {
        targetVal = Random.Range(0f, 10f);
    }

    private void Update()
    {
        if (playing)
        {
            turnVal += turnSpeed * Time.deltaTime;
            if (turnVal >= 10f)
            {
                turnVal = 0f;
            }

            if (turnVal >= targetVal - offset && turnVal <= targetVal + offset
                && PlayerController.instance.inputMaster.Player.Interact.triggered)
            {
                hitCount--;
                if (hitCount <= 0)
                {
                    TurnOn();
                    playing = false;
                }
            }
            else if (PlayerController.instance.inputMaster.Player.Interact.triggered)
            {
                hitCount = 3;
            }
        }

        //miniGameUI.SetActive(playing);
    }

    public override void Interact()
    {
        if (active && !hasActivated && !playing)
        {
            base.Interact();
        }
    }

    public override void StartInteract()
    {
        PlayerController.instance.ToggleAvatar();
        CameraController.instance.SetTarget(interacting ? focusPoint : PlayerController.instance.gameObject);
        CameraController.instance.FocusTarget();

        playing = true;
    }

    public override void EndInteract()
    {
        if (activated)
        {
            PlayerController.instance.ToggleAvatar();
            CameraController.instance.SetTarget(interacting ? focusPoint : PlayerController.instance.gameObject);
            CameraController.instance.FocusTarget();

            hasActivated = true;
            SaveDataController.instance.SaveObjectData(SaveDataController.instance.saveData.currentScene);
            UIController.instance.ToggleDialogueUI(false);
        }
    }

    void TurnOn()
    {
        transciever.Activate();
        antenna.Activate();
        UIController.instance.SetDialogueText("Power has been restored");
        UIController.instance.ToggleDialogueUI(true);
        activated = true;
    }
}
