using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : InteractObject
{
    private bool collected = false;

    public override void Interact()
    {
        if (!hasActivated)
            base.Interact();
    }

    public override void StartInteract()
    {
        if (InventoryController.instance.GetInventorySize() < 6)
        {
            string collectText = $"Found a {InventoryController.instance.itemDict[inventoryItemID].itemName}";
            UIController.instance.SetDialogueText(collectText, false);
            UIController.instance.ToggleDialogueUI(true);
            InventoryController.instance.AddItem(inventoryItemID);
            collected = true;
        }
        else
        {
            string collectText = $"Inventory is full";
            UIController.instance.SetDialogueText(collectText, false);
            UIController.instance.ToggleDialogueUI(true);
        }
    }

    public override void EndInteract()
    {
        base.EndInteract();
        UIController.instance.ToggleDialogueUI(false);
        if (collected) { SetHasActivated(); m_OnTrigger.Invoke(); }
    }
}
