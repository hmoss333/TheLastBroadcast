using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : InteractObject
{
    private bool collected = false;
    [SerializeField] AudioClip collectClip;

    public override void Interact()
    {
        if (!hasActivated)
            base.Interact();
    }

    public override void StartInteract()
    {
        bool inventoryNotFull = SaveDataController.instance.saveData.inventory.Count < 6;//InventoryController.instance.inventoryItems.Count < 6;
        string collectText = inventoryNotFull
            ? $"Found a {SaveDataController.instance.itemDict[inventoryItemID].itemName}"
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
        if (collected) {
            SetHasActivated();
            m_OnTrigger.Invoke();
            try { AudioController.instance.LoopClip(false); AudioController.instance.PlayClip(collectClip); }
            catch { Debug.Log("No AudioController in current scene"); }
        }
    }
}
