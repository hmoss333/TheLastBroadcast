using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : InteractObject
{
    public override void StartInteract()
    {
        string collectText = $"Found a {InventoryController.instance.itemDict[inventoryItemID].itemData.itemName}";
        UIController.instance.SetDialogueText(collectText, false);
        UIController.instance.ToggleDialogueUI(true);
        InventoryController.instance.AddItem(inventoryItemID);
    }

    public override void EndInteract()
    {
        base.EndInteract();
        UIController.instance.ToggleDialogueUI(false);
        SetHasActivated();
    }
}
