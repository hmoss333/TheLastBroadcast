using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class StorageController : InteractObject
{
    [NaughtyAttributes.HorizontalLine]
    [Header("Item Storage Values")]
    [SerializeField] GameObject storageMenu;
    [SerializeField] RectTransform inventoryContent, storageContent;
    InventoryItem inventoryItemPrefab;
    [SerializeField] public List<ItemData> storedItems = new List<ItemData>();
    [SerializeField] InventoryItem selectedItem;

    private List<InventoryItem> inventoryObjs = new List<InventoryItem>(); //Item prefabs, used for populating the Unity scrollview system
    private List<InventoryItem> storageObjs = new List<InventoryItem>(); //Stored item prefabs, used for populating the Unity scrollview system


    private void Update()
    {
        if (interacting && storedItems != InventoryController.instance.storedItems)
        {
            print($"Updating local storedItem data for {this.gameObject.name}");
            storedItems.Clear();
            storedItems = InventoryController.instance.storedItems;
        }
    }

    public override void Interact()
    {
        base.Interact();
        PopulateInventoryElements();
        ToggleStorageUI(interacting);

        //TODO
        //Bring up inventory UI
        //Should display current player inventory in one set and stored items in another set
        //Player can switch between item sets by pressing a button
        //If selected item is in inventory, StoreItem
        //If selected item is in storage, TakeItem
        //Exit menu by pressing melee/back button
    }


    //Item UI Functions
    public void PopulateInventoryElements()
    {
        //Populate inventory rectTransform with InventoryItem objects
        for (int i = 0; i < InventoryController.instance.inventoryItems.Count; i++)
        {
            InventoryItem itemPrefab = Instantiate(inventoryItemPrefab, inventoryContent);
            itemPrefab.ID = i;
            itemPrefab.itemInstance = InventoryController.instance.inventoryItems[i];
            itemPrefab.SetIcon(InventoryController.instance.inventoryItems[i].itemName);
            inventoryObjs.Add(itemPrefab);
        }

        //Populate storage rectTransform with InventoryItem objects
        for (int s = 0; s < InventoryController.instance.storedItems.Count; s++)
        {
            InventoryItem itemPrefab = Instantiate(inventoryItemPrefab, storageContent);
            itemPrefab.ID = s;
            itemPrefab.itemInstance = InventoryController.instance.storedItems[s];
            itemPrefab.SetIcon(InventoryController.instance.storedItems[s].itemName);
            storageObjs.Add(itemPrefab);
        }
    }

    public void ToggleStorageUI(bool state)
    {
        //Used to toggle storage menu UI on/off
    }


    //Item Storage Functions
    public void StoreItem(ItemData itemData)
    {
        if (!InventoryController.instance.storedItems.Contains(itemData))
        {
            InventoryController.instance.storedItems.Add(itemData);
        }
        else
        {
            ItemData item = InventoryController.instance.storedItems.Find(x => x == itemData);
            item.count++;
        }

        InventoryController.instance.RemoveItem(itemData.id);
    }

    public void TakeItem(ItemData itemData)
    {
        if (InventoryController.instance.inventoryItems.Count < 6)
        {
            ItemData item = InventoryController.instance.storedItems.Find(x => x == itemData);
            if (item.count > 1) { item.count--; }
            else { InventoryController.instance.storedItems.Remove(item); }

            InventoryController.instance.AddItem(itemData.id);
        }
    }
}
