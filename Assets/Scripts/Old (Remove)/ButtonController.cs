using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class ButtonController : InteractObject
{
    [SerializeField] string triggerText;
    [SerializeField] float activateDelay = 0.5f;
    Coroutine triggerRoutine;


    public override void Interact()
    {
        if (!hasActivated)
            base.Interact();
    }

    public override void StartInteract()
    {
        if (focusPoint)
        {
            PlayerController.instance.ToggleAvatar();
            CameraController.instance.SetTarget(focusPoint);
            CameraController.instance.FocusTarget();
        }

        if (triggerText != string.Empty)
        {
            UIController.instance.SetDialogueText(triggerText, false);
            UIController.instance.ToggleDialogueUI(true);
        }
    }

    public override void EndInteract()
    {
        if (focusPoint)
        {
            PlayerController.instance.ToggleAvatar();
            CameraController.instance.LoadLastTarget();
            CameraController.instance.FocusTarget();
        }

        m_OnTrigger.Invoke();
        UIController.instance.ToggleDialogueUI(false);
        SetHasActivated();
    }
}
