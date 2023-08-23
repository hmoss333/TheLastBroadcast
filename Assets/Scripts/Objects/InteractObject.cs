using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractObject : SaveObject
{
    public bool interacting;
    public string inactiveText;
    public Transform focusPoint;


    private void OnEnable()
    {
        if (needItem)
        {
            inventoryItem = InventoryController.instance.GetItem(inventoryItemID);
        }
    }

    public virtual void Interact()
    {
        interacting = !interacting;

        if (needItem && InventoryController.instance.selectedItem.ID == inventoryItemID)//inventoryItem.hasItem)
        {
            active = true;
            needItem = false;
            InventoryController.instance.RemoveItem(inventoryItemID);
            UIController.instance.SetDialogueText($"Used {inventoryItem.itemType.itemName}", false);
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
