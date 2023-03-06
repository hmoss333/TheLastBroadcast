using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneratorController : InteractObject
{
    [Header("Equipment References")]
    [SerializeField] TranscieverController transciever;
    [SerializeField] AntennaController antenna;

    [Header("Interact Variables")]
    [SerializeField] GameObject miniGameUI;
    [SerializeField] Image miniGameRotObj;
    [SerializeField] bool playing, turnedOn;
    int hitCount;
    float turnSpeed, angle = 0, radius = 5;

    Coroutine resetColor;


    private void Update()
    {
        if (playing)
        {
            turnSpeed = (2f * Mathf.PI) * (hitCount + radius);
            angle -= turnSpeed * Time.deltaTime;
            miniGameRotObj.transform.rotation = Quaternion.Euler(0, 0, angle * radius);
        }

        miniGameUI.SetActive(playing);
    }

    public override void Interact()
    {
        if (active && !hasActivated && !playing
            || turnedOn)
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
        if (turnedOn)
        {
            PlayerController.instance.ToggleAvatar();
            CameraController.instance.SetTarget(interacting ? focusPoint : PlayerController.instance.gameObject);
            CameraController.instance.FocusTarget();

            hasActivated = true;
            SaveDataController.instance.SaveObjectData(SaveDataController.instance.saveData.currentScene);
            UIController.instance.ToggleDialogueUI(false);
        }
    }

    public void Hit()
    {
        print("Hit");
        miniGameRotObj.color = Color.green;
        hitCount++;

        if (hitCount >= 3)
        {
            TurnOn();
            playing = false;
        }

        ResetColor();
    }

    public void Miss()
    {
        print("Miss");
        miniGameRotObj.color = Color.red;
        hitCount = 0;

        ResetColor();
    }

    public void ResetColor()
    {
        if (resetColor == null)
            resetColor = StartCoroutine(ResetColorCoroutine());
        else
        {
            StopCoroutine(resetColor);
            resetColor = StartCoroutine(ResetColorCoroutine());
        }
    }

    IEnumerator ResetColorCoroutine()
    {
        yield return new WaitForSeconds(0.5f);

        miniGameRotObj.color = Color.white;
    }

    void TurnOn()
    {
        transciever.Activate();
        antenna.Activate();
        UIController.instance.SetDialogueText("Power has been restored");
        UIController.instance.ToggleDialogueUI(true);
        turnedOn = true;
    }
}
