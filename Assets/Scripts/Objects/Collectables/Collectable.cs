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
        bool inventoryNotFull = InventoryController.instance.inventoryItems.Count < 6;
        string collectText = inventoryNotFull
            ? $"Found a {InventoryController.instance.itemDict[inventoryItemID].itemName}"
            : $"Inventory is full";
        UIController.instance.SetDialogueText(collectText, false);
        UIController.instance.ToggleDialogueUI(true);

        if (inventoryNotFull)
        {
            InventoryController.instance.AddItem(inventoryItemID);
            collected = true;
        }
    }

    public override void EndInteract()
    {
        base.EndInteract();
        UIController.instance.ToggleDialogueUI(false);
        if (collected) { SetHasActivated(); m_OnTrigger.Invoke(); }
    }
}
