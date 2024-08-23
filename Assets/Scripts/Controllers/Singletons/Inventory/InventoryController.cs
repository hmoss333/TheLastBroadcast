using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using System.IO;
using TMPro;
using UnityEngine.UI.Extensions;


public class InventoryController : MonoBehaviour
{
    public static InventoryController instance;

    //Input delay variables
    private bool moved = false;
    private float inputDelay = 0f;

    [Header("UI Elements")]
    [SerializeField] RectTransform inventoryContent;
    [SerializeField] InventoryItem inventoryItemPrefab;
    [SerializeField] TMP_Text inventoryTitle, inventoryDesc;
    [SerializeField] Image itemImage;
    [SerializeField] AudioClip equipClip, moveClip;

    private int itemPosVal; //Current selected item position in inventory

    //To be refactored
    public InventoryItem selectedItem { get; private set; } //Currently selected item; consider removing this and just checking if itemID is in the inventory list
    public List<InventoryItem> inventoryObjs = new List<InventoryItem>(); //Item prefabs, used for populating the Unity scrollview system


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    private void Start()
    {
        //Populate Inventory UI Elements
        inventoryObjs.Clear();
        for (int i = 0; i < SaveDataController.instance.saveData.inventory.Count; i++)
        {
            InventoryItem itemPrefab = Instantiate(inventoryItemPrefab, inventoryContent);
            itemPrefab.ID = i;
            itemPrefab.itemInstance = SaveDataController.instance.saveData.inventory[i];
            itemPrefab.SetIcon(SaveDataController.instance.saveData.inventory[i].itemName);
            inventoryObjs.Add(itemPrefab);

            if (i == 0)
            {
                ShowItemData(itemPrefab.itemInstance);
            }
        }
    }

    private void Update()
    {
        if (PauseMenuController.instance.isPaused
            && PauseMenuController.instance.CurrentPanel().name.ToLower() == "inventory")           
        {
            //Update UI for all inventory items
            foreach (InventoryItem item in inventoryObjs)
            {
                item.SetIcon(item.itemInstance.itemName);
            }

            //Set item highlight
            if (inventoryObjs.Count > 0) { inventoryObjs[itemPosVal].ToggleHighlight(true); ShowItemData(inventoryObjs[itemPosVal].itemInstance); }
            else { inventoryTitle.text = ""; inventoryDesc.text = ""; }

            //Toggle itemImage if inventory is not empty
            itemImage.gameObject.SetActive(inventoryObjs.Count > 0 ? true : false);

            //Use directional input to navigate inventory menu
            if (InputController.instance.inputMaster.Player.Move.triggered && !moved)
            {
                moved = true;
                AudioController.instance.LoopClip(false);
                AudioController.instance.PlayClip(moveClip);

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
                    if (itemPosVal >= inventoryObjs.Count - 1) { itemPosVal = inventoryObjs.Count - 1; }

                    //Refresh highlights to only show last highlighted item
                    for (int i = 0; i < inventoryObjs.Count; i++)
                    {
                        inventoryObjs[i].ToggleHighlight(i == itemPosVal);
                    }

                    //Display the current selected item data
                    ShowItemData(inventoryObjs[itemPosVal].itemInstance);
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

            //Select current item
            if (InputController.instance.inputMaster.Player.Interact.triggered
                && SaveDataController.instance.saveData.inventory.Count > 0)
            {
                AudioController.instance.LoopClip(false);
                AudioController.instance.PlayClip(equipClip);

                if (selectedItem != inventoryObjs[itemPosVal])
                {
                    SelectItem(inventoryObjs[itemPosVal]);
                }
                else
                {
                    selectedItem = null;
                    PlayerItemUI.instance.UpdateCurrentItem(selectedItem);
                }
            }
        }
    }



    //Item UI Functions
    public void SelectItem(InventoryItem item)
    {
        //Hide all item highlights
        InventoryItem[] tempItems = inventoryContent.GetComponentsInChildren<InventoryItem>();
        foreach (InventoryItem i_Item in tempItems)
        {
            i_Item.ToggleHighlight(false);
        }

        //Set item prefab as the current selected
        selectedItem = item;
        selectedItem.ToggleHighlight(true);

        //Display item name/description
        ShowItemData(item.itemInstance);
        PlayerItemUI.instance.UpdateCurrentItem(item);
    }

    public void ShowItemData(ItemData item)
    {
        inventoryTitle.text = item.itemName;
        inventoryDesc.text = item.description;
        itemImage.sprite = inventoryObjs[itemPosVal].icon;
    }



    //Item Inventory Functions
    public void AddItem(int itemID)
    {
        SaveDataController.instance.AddItem(itemID);

        ItemData result = SaveDataController.instance.saveData.inventory.Find(x => x.id == itemID);
        if (result != null)
        {
            InventoryItem checkItem = inventoryObjs.Find(x => x.itemInstance.id == itemID);

            //If the inventoryObj does already NOT exist
            if (checkItem == null)
            {
                InventoryItem tempItem = Instantiate(inventoryItemPrefab, inventoryContent);
                tempItem.ID = result.id;
                tempItem.itemInstance = result;
                tempItem.SetIcon(result.itemName);
                inventoryObjs.Add(tempItem);
            }
        }

        itemPosVal = 0;
    }

    public void RemoveItem(int itemID)
    {
        SaveDataController.instance.RemoveItem(itemID);

        ItemData result = SaveDataController.instance.saveData.inventory.Find(x => x.id == itemID);
        //If the item has been removed from the inventory
        if (result == null)
        {
            InventoryItem tempItem = inventoryObjs.Find(x => x.itemInstance.id == itemID);
            Destroy(inventoryObjs.Find(x => x.itemInstance.id == itemID).gameObject);
            inventoryObjs.Remove(tempItem);
            itemPosVal = 0;
        }
    }
}


[System.Serializable]
public class InventoryWrapper
{
    public List<ItemData> items = new List<ItemData>();
}

[System.Serializable]
public class ItemData
{
    public int id;
    public int count;
    public bool consumable;
    public string itemName;
    [TextArea]
    public string description;
}
