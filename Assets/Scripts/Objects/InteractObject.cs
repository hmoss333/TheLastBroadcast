using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.Events;
using UnityEngine.Serialization;
using System.Linq;


public class InteractObject : SaveObject
{
    [HideInInspector] public bool interacting;
    public string inactiveText;

    [Header("Focus Variables")]
    public bool focusOnInteract;
    public Transform focusPoint;


    Coroutine selectItemRoutine;


    public virtual void Interact()
    {
        interacting = !interacting;
     
        if (needItem && InventoryController.instance.ItemInInventory(inventoryItemID))
        {
            active = true;
            needItem = false;
            UIController.instance.SetDialogueText($"Used {InventoryController.instance.itemDict.GetValueOrDefault(inventoryItemID).itemName}", false);
            UIController.instance.ToggleDialogueUI(interacting);
            InventoryController.instance.RemoveItem(inventoryItemID);
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
            UIController.instance.SetDialogueText(needItem ? $"Needs a {InventoryController.instance.itemDict.GetValueOrDefault(inventoryItemID).itemName}" : inactiveText, false);
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
