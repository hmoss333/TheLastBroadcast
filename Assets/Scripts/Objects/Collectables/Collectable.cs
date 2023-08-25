using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : InteractObject
{
    string collectText = "";

    private void Start()
    {       
        collectText = $"Found a {InventoryController.instance.itemDict[inventoryItemID].itemData.itemName}";
    }

    public override void StartInteract()
    {
        UIController.instance.SetDialogueText(collectText, false);
        UIController.instance.ToggleDialogueUI(true);
        InventoryController.instance.AddItem(inventoryItemID);
    }

    public override void EndInteract()
    {
        base.EndInteract();
        UIController.instance.ToggleDialogueUI(false);
        gameObject.SetActive(false);
    }
}
