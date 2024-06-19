using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System.Linq;

public class StorageController : InteractObject
{
    [NaughtyAttributes.HorizontalLine]
    [Header("Item Storage Values")]
    [SerializeField] GameObject storageMenu;
    [SerializeField] RectTransform inventoryContent, storageContent;
    [SerializeField] InventoryItem inventoryItemPrefab;
    private int itemPosVal;
    private bool moved, inStorageMenu, canInteract = false;
    private float inputDelay = 0;

    private List<InventoryItem> inventoryObjs = new List<InventoryItem>(); //Item prefabs, used for populating the Unity scrollview system
    private List<InventoryItem> storageObjs = new List<InventoryItem>(); //Stored item prefabs, used for populating the Unity scrollview system


    private void Update()
    {
        if (interacting)
        {
            //Wait for interact button to no longer be pressed before allowing user to select items
            //Needed due to the input system registering immediately following starting the interact,
            //putting the top item in the player inventory into storage
            if (!canInteract && !InputController.instance.inputMaster.Player.Interact.IsPressed())
            {
                canInteract = true;
            }

            //Clear all highlights, only highlight currently selected item
            foreach (InventoryItem item in inventoryObjs) { item.ToggleHighlight(false); }
            foreach (InventoryItem item in storageObjs) { item.ToggleHighlight(false); }
            try
            {
                if (inStorageMenu) { storageObjs[itemPosVal].ToggleHighlight(true); }
                else { inventoryObjs[itemPosVal].ToggleHighlight(true); }
            }
            catch { }

            //Use directional input to navigate inventory menu
            if (InputController.instance.inputMaster.Player.Move.triggered && !moved)
            {
                moved = true;
                //AudioController.instance.LoopClip(false);
                //AudioController.instance.PlayClip(moveClip);

                Vector2 inputVal = InputController.instance.inputMaster.Player.Move.ReadValue<Vector2>();
                if (inputVal.x > 0) { itemPosVal++; }
                else if (inputVal.x < -0) { itemPosVal--; }
                //TODO
                //Figure out best way to read directional inputs from control stick
                //Currently gives incorrect behaviors/reads multiple direction inputs at once


                //Keep value inside of the inventoryItems array
                try
                {
                    if (itemPosVal <= 0) { itemPosVal = 0; }
                    if (inStorageMenu && itemPosVal >= storageObjs.Count - 1)
                    {
                        itemPosVal = storageObjs.Count - 1;
                    }
                    else if (!inStorageMenu && itemPosVal >= inventoryObjs.Count - 1)
                    {
                        itemPosVal = inventoryObjs.Count - 1;
                    }
                }
                catch { }
            }

            //Add input delay
            if (moved)
            {
                inputDelay += Time.unscaledDeltaTime;
                if (inputDelay >= 0.25f)
                {
                    inputDelay = 0;
                    moved = false;
                }
            }

            //On input button press
            if (InputController.instance.inputMaster.Player.Interact.triggered && canInteract)
            {
                if (inStorageMenu)
                {
                    TakeItem(storageObjs[itemPosVal].itemInstance);
                    if (storageObjs.Count <= 0)
                        inStorageMenu = false;
                }
                else
                {
                    StoreItem(inventoryObjs[itemPosVal].itemInstance);
                    if (inventoryObjs.Count <= 0)
                        inStorageMenu = true;
                }

                PopulateInventoryElements();
            }

            //Change focus to storage
            if (InputController.instance.inputMaster.Player.MenuRight.triggered)
            {
                itemPosVal = 0;
                inStorageMenu = true;
            }
            //Change focus to inventory
            if (InputController.instance.inputMaster.Player.MenuLeft.triggered)
            {
                itemPosVal = 0;
                inStorageMenu = false;
            }

            //Exit storage interact menu
            if (InputController.instance.inputMaster.Player.Melee.triggered)
            {
                interacting = false;
                ToggleStorageUI(false);
                PlayerController.instance.SetState(PlayerController.States.idle);
            }
        }
    }

    public override void Interact()
    {
        if (!interacting)
        {
            base.Interact();
            if (interacting) { PopulateInventoryElements(); }
            ToggleStorageUI(interacting);

            canInteract = false;
            itemPosVal = 0;
            inStorageMenu = storageObjs.Count > 0
                ? true
                : false;
        }
    }


    //Item UI Functions
    //TODO REFACTOR THIS, PLEASE!!!
    public void PopulateInventoryElements()
    {
        foreach (InventoryItem item in inventoryObjs)
        {
            Destroy(item.gameObject);
        }
        inventoryObjs.Clear();
        foreach (InventoryItem item in storageObjs)
        {
            Destroy(item.gameObject);
        }
        storageObjs.Clear();

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
        storageMenu.SetActive(state);
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
