using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneratorController : InteractObject
{
    [NaughtyAttributes.HorizontalLine]
    [Header("Equipment References")]
    [SerializeField] private SaveObject[] objectsToActivate;

    [NaughtyAttributes.HorizontalLine]
    [Header("Interact Variables")]
    [SerializeField] private Light activeLight;
    [SerializeField] private GameObject miniGameUI;
    [SerializeField] private Image miniGameRotObj, buttonIcon;
    private int hitCount;
    [SerializeField] private float turnSpeed, angle = 0, radius = 7.5f;
    private Coroutine resetColor;



    private void Update()
    {
        if (interacting)
        {
            turnSpeed = (2f * Mathf.PI) * (2f * hitCount + radius);
            angle -= turnSpeed * Time.deltaTime;
            miniGameRotObj.transform.rotation = Quaternion.Euler(0, 0, angle * radius);
        }

        activeLight.color = hasActivated ? Color.green : Color.red;
        miniGameUI.SetActive(interacting);
    }

    public override void Interact()
    {
        if (active && !hasActivated && !interacting)
        {
            base.Interact();
        }
    }

    public override void StartInteract()
    {
        PlayerController.instance.ToggleAvatar();
        CameraController.instance.SetTarget(interacting ? focusPoint : CameraController.instance.GetLastTarget());
        CameraController.instance.FocusTarget();
        if (CameraController.instance.GetTriggerState())
            CameraController.instance.SetRotation(true);
    }

    public void Hit()
    {
        buttonIcon.color = Color.green;
        hitCount++;

        if (hitCount >= 3)
        {
            interacting = false;
            TurnOn();
        }

        ResetColor();
    }

    public void Miss()
    {
        buttonIcon.color = Color.red;
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

        buttonIcon.color = Color.white;
    }

    void TurnOn()
    {
        for (int i = 0; i < objectsToActivate.Length; i++)
        {
            objectsToActivate[i].Activate();
        }

        SetHasActivated();

        PlayerController.instance.ToggleAvatar();
        CameraController.instance.SetTarget(CameraController.instance.GetLastTarget());
        CameraController.instance.FocusTarget();
        if (CameraController.instance.GetTriggerState())
            CameraController.instance.SetRotation(true);
        PlayerController.instance.SetState(PlayerController.States.idle);
    }
}
