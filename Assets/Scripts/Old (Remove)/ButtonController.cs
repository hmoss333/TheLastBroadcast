using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class ButtonController : InteractObject
{
    [SerializeField] SaveObject[] objectsToActivate;
    [SerializeField] string triggerText;
    [SerializeField] float activateDelay = 0.5f;
    Coroutine triggerRoutine;

    [FormerlySerializedAs("onTrigger")]
    [SerializeField]
    private UnityEvent m_OnTrigger = new UnityEvent();


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

        if (triggerRoutine == null)
            triggerRoutine = StartCoroutine(ActivateObjects());
    }

    public override void EndInteract()
    {
        if (focusPoint)
        {
            PlayerController.instance.ToggleAvatar();
            CameraController.instance.LoadLastTarget();
            CameraController.instance.FocusTarget();
        }

        UIController.instance.ToggleDialogueUI(false);
        SetHasActivated();
    }

    IEnumerator ActivateObjects()
    {
        //Activate all interact objects in list
        for (int i = 0; i < objectsToActivate.Length; i++)
        {
            objectsToActivate[i].Activate();

            yield return new WaitForSeconds(activateDelay);
        }

        m_OnTrigger.Invoke();

        triggerRoutine = null;
    }
}
