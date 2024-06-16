using System.Collections;
using System.Collections.Generic;
//using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using System.IO;
using TMPro;
using UnityEngine.UI.Extensions;


//TODO
//Create ItemBox json object (should use similar structure to inventory)
//Add logic to transfer items between local and box inventories
//Update local inventory to be a limited amount of items (6 total)
//If local inventory is full, do not pick up any new items UNLESS the same ID already exists, then add to stack
//Remove auto scroll to inventory view (is no longer necessary with the limited local inventory)
//Review implementing the "select item" popup when interacting with an object that requires an item to activate


public class InventoryController : MonoBehaviour
{
    public static InventoryController instance;

    private string inventoryDest, storageDest;
    private bool moved = false;
    private float inputDelay = 0f;
    [Header("UI Elements")]
    [SerializeField] RectTransform inventoryContent;
    [SerializeField] ScrollRect scrollRect;
    [SerializeField] InventoryItem inventoryItemPrefab;
    [SerializeField] TMP_Text inventoryTitle, inventoryDesc;
    [SerializeField] Image itemImage;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip equipClip, moveClip;

    [NaughtyAttributes.HorizontalLine]
    [Header("Inventory Data")]
    [SerializeField] private List<ItemData> inventoryItems = new List<ItemData>(); //Current Inventory
    public Dictionary<int, ItemData> itemDict { get; private set; } //Holds references from streaming item file for all possible items; can probably refactor this to just use a direct check to the file when adding a new item
    [SerializeField] private int itemPosVal;
    public InventoryItem selectedItem { get; private set; }

    private List<InventoryItem> inventoryObjs = new List<InventoryItem>(); //Item prefabs; redundant, can eventually be consolidated into this class


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        itemDict = new Dictionary<int, ItemData>();
        inventoryDest = System.IO.Path.Combine(Application.persistentDataPath, "Items", "items.json");
        storageDest = System.IO.Path.Combine(Application.persistentDataPath, "Items", "itembox.json");

        LoadItemFile();
        RefreshInventory();
    }

    private void Update()
    {
        if (PauseMenuController.instance.isPaused
            && PauseMenuController.instance.CurrentPanel().name.ToLower() == "inventory")           
        {
            if (inventoryObjs.Count > 0)
                inventoryObjs[itemPosVal].ToggleHighlight(true); //highlight the currently displayed item position

            //Use directional input to navigate inventory menu
            if (InputController.instance.inputMaster.Player.Move.triggered && !moved)
            {
                moved = true;
                PlayClip(moveClip);

                Vector2 move = InputController.instance.inputMaster.Player.Move.ReadValue<Vector2>();
                if (move.x > 0)
                {
                    itemPosVal++;
                    if (itemPosVal % 3 == 0)
                    {
                        inventoryContent.anchoredPosition = new Vector2(0, inventoryContent.anchoredPosition.y
                            + inventoryContent.GetComponent<GridLayoutGroup>().cellSize.y
                            + inventoryContent.GetComponent<GridLayoutGroup>().spacing.y);
                    }
                }
                else
                if (move.x < 0)
                {
                    itemPosVal--;
                    if (itemPosVal % 3 == 2) //columnCount - 1
                    {
                        inventoryContent.anchoredPosition = new Vector2(0, inventoryContent.anchoredPosition.y
                            - inventoryContent.GetComponent<GridLayoutGroup>().cellSize.y
                            - inventoryContent.GetComponent<GridLayoutGroup>().spacing.y);
                    }
                }
                
                //if (move.y > 0)
                //{
                //    itemPosVal -= 3;
                //    inventoryContent.anchoredPosition = new Vector2(0, inventoryContent.anchoredPosition.y
                //        - inventoryContent.GetComponent<GridLayoutGroup>().cellSize.y
                //        - inventoryContent.GetComponent<GridLayoutGroup>().spacing.y);
                //}
                //else
                //if (move.y < 0)
                //{
                //    itemPosVal += 3;
                //    inventoryContent.anchoredPosition = new Vector2(0, inventoryContent.anchoredPosition.y
                //        + inventoryContent.GetComponent<GridLayoutGroup>().cellSize.y
                //        + inventoryContent.GetComponent<GridLayoutGroup>().spacing.y);
                //}

                //Keep value inside of the inventoryItems array
                try
                {
                    if (itemPosVal <= 0)
                    {
                        itemPosVal = 0;
                    }
                    if (itemPosVal >= inventoryObjs.Count - 1)
                    {
                        itemPosVal = inventoryObjs.Count - 1;
                    }

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
                PlayClip(equipClip);

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
    }

    public void SaveItemData()
    {
        //Serialize Inventory Data
        InventoryWrapper inventoryData = new InventoryWrapper();
        inventoryData.items = inventoryItems;

        string jsonData = JsonUtility.ToJson(inventoryData);
        print("Saving Inventory Data:" + jsonData);
        File.WriteAllText(inventoryDest, jsonData);  
    }


    public void RefreshInventory()
    {
        //Remove all existing inventory item prefabs
        foreach (InventoryItem item in inventoryObjs)
        {
            Destroy(item.gameObject);
        }
        inventoryTitle.text = "";
        inventoryDesc.text = "";
        itemPosVal = 0;

        //Repopulate items from fresh list and create new prefabs
        bool firstItem = true;
        inventoryObjs.Clear();
        for (int i = 0; i < inventoryItems.Count; i++)
        {
            InventoryItem itemPrefab = Instantiate(inventoryItemPrefab, inventoryContent);
            itemPrefab.ID = i;
            itemPrefab.itemInstance = inventoryItems[i];
            itemPrefab.SetIcon(inventoryItems[i].itemName);
            inventoryObjs.Add(itemPrefab);

            if (firstItem)
            {
                ShowItemData(itemPrefab.itemInstance);
                firstItem = false;
            }
        }

        //Refresh reference to selected item after updateding list
        if (selectedItem != null)
        {
            foreach (InventoryItem item in inventoryObjs)
            {
                if (item.ID == selectedItem.ID)
                {
                    SelectItem(item);
                    break;
                }
            }
        }
    }

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
        }

        itemPosVal = 0;
        RefreshInventory();
    }

    public void RemoveItem(int itemID)
    {
        print("Removing item");
        ItemData result = inventoryItems.Find(x => x.id == itemID);
        if (result != null)
        {
            result.count--;
            if (result.count <= 0)
                inventoryItems.Remove(result);
        }

        RefreshInventory();
    }


    public void PlayClip(AudioClip clip)
    {
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();
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
