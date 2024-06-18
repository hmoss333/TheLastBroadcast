using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using System.IO;
using TMPro;
using UnityEngine.UI.Extensions;
using UnityEngine.U2D;
using UnityEditorInternal.VersionControl;


//TODO
//Simplify inventoryObjs to just instantiate a placeholder object using inventoryItems data
//Possibly remove the selectedItem system and just trigger item usage from the inventory menu


public class InventoryController : MonoBehaviour
{
    public static InventoryController instance;

    //Save file paths
    private string inventoryDest, storageDest; //save file destinations

    //Input delay variables
    private bool moved = false;
    private float inputDelay = 0f;

    [Header("UI Elements")]
    [SerializeField] RectTransform inventoryContent;
    [SerializeField] InventoryItem inventoryItemPrefab;
    [SerializeField] TMP_Text inventoryTitle, inventoryDesc;
    [SerializeField] Image itemImage;
    [SerializeField] AudioClip equipClip, moveClip;

    //[NaughtyAttributes.HorizontalLine]
    //[Header("Inventory Data")]
    [SerializeField] public List<ItemData> inventoryItems { get; private set; } //Player Inventory
    [SerializeField] public List<ItemData> storedItems { get; private set; } //Storage inventory
    public Dictionary<int, ItemData> itemDict { get; private set; } //Holds references from streaming item file for all possible items; used for reference when picking up new items
    private int itemPosVal; //Current selected item position in inventory

    //To be refactored
    public InventoryItem selectedItem { get; private set; } //Currently selected item; consider removing this and just checking if itemID is in the inventory list
    private List<InventoryItem> inventoryObjs = new List<InventoryItem>(); //Item prefabs, used for populating the Unity scrollview system
    private List<InventoryItem> storageObjs = new List<InventoryItem>(); //Stored item prefabs, used for populating the Unity scrollview system


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        inventoryItems = new List<ItemData>();
        storedItems = new List<ItemData>();
        itemDict = new Dictionary<int, ItemData>();
        inventoryDest = System.IO.Path.Combine(Application.persistentDataPath, "Items", "items.json");
        storageDest = System.IO.Path.Combine(Application.persistentDataPath, "Items", "itembox.json");

        LoadItemFile();

        //Populate Inventory UI Elements
        inventoryObjs.Clear();
        for (int i = 0; i < inventoryItems.Count; i++)
        {
            InventoryItem itemPrefab = Instantiate(inventoryItemPrefab, inventoryContent);
            itemPrefab.ID = i;
            itemPrefab.itemInstance = inventoryItems[i];
            itemPrefab.SetIcon(inventoryItems[i].itemName);
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
            if (inventoryObjs.Count > 0) { inventoryObjs[itemPosVal].ToggleHighlight(true); ShowItemData(inventoryObjs[itemPosVal].itemInstance); }
            else { inventoryTitle.text = ""; inventoryDesc.text = ""; }

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
                && inventoryItems.Count > 0)
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


    //Save/Load inventory data
    public void LoadItemFile()
    {
        //Get base item data and load into dictionary
        string resourcesPath = System.IO.Path.Combine(Application.streamingAssetsPath, "items.json");
        string r_jsonData = File.ReadAllText(resourcesPath);

        InventoryWrapper r_Items = JsonUtility.FromJson<InventoryWrapper>("{\"items\":" + r_jsonData + "}");
        for (int i = 0; i < r_Items.items.Count; i++)
        {
            itemDict.Add(r_Items.items[i].id, r_Items.items[i]);
        }


        //Check if saved inventory file exists
        if (File.Exists(inventoryDest))
        {
            print("Loading Inventory Data");
            string jsonData = File.ReadAllText(inventoryDest);
            InventoryWrapper i_Items = JsonUtility.FromJson<InventoryWrapper>(jsonData);
            inventoryItems = i_Items.items;
        }
        //If not, create new inventory save file
        else
        {
            print("Creating new item file");
            Directory.CreateDirectory(System.IO.Path.Combine(Application.persistentDataPath, "Items"));
            SaveItemData();
        }


        //Check if saved storage file exists
        if (File.Exists(storageDest))
        {
            print("Loading Inventory Data");
            string jsonData = File.ReadAllText(storageDest);
            InventoryWrapper i_Items = JsonUtility.FromJson<InventoryWrapper>(jsonData);
            storedItems = i_Items.items;
        }
        //If not, create new inventory save file
        else
        {
            print("Creating new item file");
            Directory.CreateDirectory(System.IO.Path.Combine(Application.persistentDataPath, "Items"));
            SaveItemData();
        }
    }

    public void SaveItemData()
    {
        //Serialize Item Data
        InventoryWrapper inventoryData = new InventoryWrapper();
        InventoryWrapper storageData = new InventoryWrapper();
        inventoryData.items = inventoryItems;
        storageData.items = storedItems;

        //Write Inventory Data to JSON file
        string jsonData = JsonUtility.ToJson(inventoryData);
        print("Saving Inventory Data:" + jsonData);
        File.WriteAllText(inventoryDest, jsonData);

        //Write Storage Data to JSON File
        string jsonStorageData = JsonUtility.ToJson(storageData);
        print("Saving Storage Data:" + jsonStorageData);
        File.WriteAllText(storageDest, jsonStorageData);
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
        ItemData result = inventoryItems.Find(x => x.id == itemID);
        if (result != null)
        {
            result.count++;
        }
        else
        {
            result = itemDict[itemID];
            result.count++;
            inventoryItems.Add(result);

            InventoryItem tempItem = Instantiate(inventoryItemPrefab, inventoryContent);
            tempItem.ID = result.id;
            tempItem.itemInstance = result;
            tempItem.SetIcon(result.itemName);
            inventoryObjs.Add(tempItem);
        }

        itemPosVal = 0;
    }

    public void RemoveItem(int itemID)
    {
        print($"Removed {itemDict[itemID].itemName}");
        ItemData result = inventoryItems.Find(x => x.id == itemID);
        if (result != null)
        {
            result.count--;
            if (result.count <= 0)
            {
                inventoryItems.Remove(result);
                InventoryItem tempItem = inventoryObjs.Find(x => x.ID == itemID);
                Destroy(inventoryObjs.Find(x => x.ID == itemID).gameObject);
                inventoryObjs.Remove(tempItem);
                itemPosVal = 0;
            }
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
