using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.Events;
using UnityEngine.Serialization;


public class InteractObject : SaveObject
{
    [HideInInspector] public bool interacting;
    public string inactiveText;
    public bool focusOnInteract;
    public Transform focusPoint;


    public virtual void Interact()
    {
        interacting = !interacting;

        if (InventoryController.instance.selectedItem != null
            && InventoryController.instance.selectedItem.itemInstance.id == inventoryItemID
            && needItem)
        {
            active = true;
            needItem = false;
            InventoryController.instance.RemoveItem(inventoryItemID);
            UIController.instance.SetDialogueText($"Used {InventoryController.instance.selectedItem.itemInstance.itemData.itemName}", false);
            UIController.instance.ToggleDialogueUI(interacting);
        }

        if (active && !needItem)
        {
            if (interacting)
            {
                StartInteract();            
            }
            else
            {
                EndInteract();
            }         
        }
        else
        {
            UIController.instance.SetDialogueText(inactiveText, false);
            UIController.instance.ToggleDialogueUI(interacting);
        }

        if (focusOnInteract && focusPoint != null)
        {
            PlayerController.instance.ToggleAvatar();
            CameraController.instance.SetTarget(interacting ? focusPoint : CameraController.instance.GetLastTarget());
            CameraController.instance.FocusTarget();
            if (CameraController.instance.GetTriggerState())
                CameraController.instance.SetRotation(true);
        }
    }

    public virtual void StartInteract()
    {
        //Used for logic at start of interaction
    }

    public virtual void EndInteract()
    {
        //Used for logic at end of interaction
    }
}
