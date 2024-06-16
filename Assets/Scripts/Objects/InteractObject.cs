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

        if (needItem && InventoryController.instance.itemDict.ContainsKey(inventoryItemID))
            //&& InventoryController.instance.selectedItem != null && InventoryController.instance.selectedItem.ID == inventoryItemID)
        {
            active = true;
            needItem = false;
            UIController.instance.SetDialogueText($"Used {InventoryController.instance.itemDict.GetValueOrDefault(inventoryItemID).itemName}", false);
            UIController.instance.ToggleDialogueUI(interacting);
            InventoryController.instance.RemoveItem(inventoryItemID);

            //if (selectItemRoutine == null)
            //    selectItemRoutine = StartCoroutine(SelectItem());
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

    //IEnumerator SelectItem()
    //{
    //    //Open inventory sceen

    //    while (true)
    //    {
    //        yield return null;

    //        if (InputController.instance.inputMaster.Player.Interact.triggered)
    //        {
    //            if (InventoryController.instance.selectedItem != null
    //                && InventoryController.instance.selectedItem.itemInstance.id == inventoryItemID)
    //            {
    //                active = true;
    //                needItem = false;
    //                UIController.instance.SetDialogueText($"Used {InventoryController.instance.selectedItem.itemInstance.itemData.itemName}", false);
    //                UIController.instance.ToggleDialogueUI(interacting);
    //                break;
    //            }
    //            else
    //            {
    //                interacting = !interacting;
    //            }
    //        }
    //    }

    //    selectItemRoutine = null;
    //}
}
