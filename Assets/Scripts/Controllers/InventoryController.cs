using System.Collections;
using System.Collections.Generic;
//using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using System.IO;
using TMPro;
using UnityEngine.UI.Extensions;
using static UnityEngine.GraphicsBuffer;
//using static UnityEditor.Progress;

public class InventoryController : MonoBehaviour
{
    public static InventoryController instance;

    private string itemDestination;
    [Header("UI Elements")]
    [SerializeField] RectTransform inventoryContent;
    [SerializeField] ScrollRect scrollRect;
    [SerializeField] InventoryItem inventoryItemPrefab;
    [SerializeField] TMP_Text inventoryTitle, inventoryDesc;

    //[NaughtyAttributes.HorizontalLine]

    public InventoryItem selectedItem { get; private set; }
    [SerializeField] private int itemPosVal;
    public Dictionary<int, ItemInstance> itemDict { get; private set; } //Reference for all possible items
    private List<InventoryItem> inventoryItems = new List<InventoryItem>(); //Item prefabs
    [SerializeField] private List<ItemInstance> currentInventory = new List<ItemInstance>(); //Current Inventory


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        itemDict = new Dictionary<int, ItemInstance>();
        itemDestination = System.IO.Path.Combine(Application.persistentDataPath, "Items", "items.json");

        LoadItemFile();
        RefreshInventory();
    }

    private void Update()
    {
        if (PauseMenuController.instance.isPaused) //&& currentPanel == inventoryPanel           
        {
            //Use directional input to navigate inventory menu
            if (PlayerController.instance.inputMaster.Player.Move.triggered)
            {
                Vector2 move = PlayerController.instance.inputMaster.Player.Move.ReadValue<Vector2>();
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
                else if (move.x < 0)
                {
                    itemPosVal--;
                    if (itemPosVal % 3 != 0)
                    {
                        inventoryContent.anchoredPosition = new Vector2(0, inventoryContent.anchoredPosition.y
                            - inventoryContent.GetComponent<GridLayoutGroup>().cellSize.y
                            - inventoryContent.GetComponent<GridLayoutGroup>().spacing.y);
                    }
                }
                else if (move.y > 0)
                {
                    itemPosVal -= 3;
                    inventoryContent.anchoredPosition = new Vector2(0, inventoryContent.anchoredPosition.y
                        - inventoryContent.GetComponent<GridLayoutGroup>().cellSize.y
                        - inventoryContent.GetComponent<GridLayoutGroup>().spacing.y);
                }
                else if (move.y < 0)
                {
                    itemPosVal += 3;
                    inventoryContent.anchoredPosition = new Vector2(0, inventoryContent.anchoredPosition.y
                        + inventoryContent.GetComponent<GridLayoutGroup>().cellSize.y
                        + inventoryContent.GetComponent<GridLayoutGroup>().spacing.y);
                }

                //Keep value inside of the inventoryItems array
                try
                {
                    if (itemPosVal <= 0)
                    {
                        itemPosVal = 0;
                    }
                    if (itemPosVal >= inventoryItems.Count - 1)
                    {
                        itemPosVal = inventoryItems.Count - 1;
                    }

                    //Refresh highlights to only show last highlighted item
                    for (int i = 0; i < inventoryItems.Count; i++)
                    {
                        inventoryItems[i].ToggleHighlight(i == itemPosVal);
                    }

                    //Display the current selected item data
                    ShowItemData(inventoryItems[itemPosVal].itemInstance);
                }
                catch { }
            }


            //Select current item
            if (PlayerController.instance.inputMaster.Player.Interact.triggered)
            {
                if (selectedItem != inventoryItems[itemPosVal])
                {
                    SelectItem(inventoryItems[itemPosVal]);
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
        if (File.Exists(itemDestination))
        {
            print("Loading item data");
            string jsonData = File.ReadAllText(itemDestination);
            InventoryWrapper i_Items = JsonUtility.FromJson<InventoryWrapper>(jsonData);
            currentInventory = i_Items.items;
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
        InventoryWrapper inventoryData = new InventoryWrapper();
        inventoryData.items = currentInventory;

        string jsonData = JsonUtility.ToJson(inventoryData);
        print("Saving Item Data:" + jsonData);
        File.WriteAllText(itemDestination, jsonData);
    }


    public void RefreshInventory()
    {
        //Remove all existing inventory item prefabs
        foreach (InventoryItem item in inventoryItems)
        {
            Destroy(item.gameObject);
        }
        inventoryTitle.text = "";
        inventoryDesc.text = "";
        itemPosVal = 0;

        //Repopulate items from fresh list and create new prefabs
        bool firstItem = true;
        inventoryItems.Clear();
        for (int i = 0; i < currentInventory.Count; i++)
        {
            InventoryItem itemPrefab = Instantiate(inventoryItemPrefab, inventoryContent);
            itemPrefab.ID = i;
            itemPrefab.itemInstance = currentInventory[i];
            itemPrefab.SetIcon(currentInventory[i].itemData.itemName);
            inventoryItems.Add(itemPrefab);

            if (firstItem)
            {
                ShowItemData(itemPrefab.itemInstance);
                firstItem = false;
            }
        }

        //Refresh reference to selected item after updateding list
        if (selectedItem != null)
        {
            foreach (InventoryItem item in inventoryItems)
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

    public void ShowItemData(ItemInstance item)
    {
        inventoryTitle.text = item.itemData.itemName;
        inventoryDesc.text = item.itemData.description;
    }


    public void AddItem(int itemID)
    {
        ItemInstance result = currentInventory.Find(x => x.id == itemID);
        if (result != null)
        {
            result.count++;
        }
        else
        {
            result = itemDict[itemID];
            result.count++;
            currentInventory.Add(result);
        }

        itemPosVal = 0;
        RefreshInventory();
    }

    public void RemoveItem(int itemID)
    {
        print("Removing item");
        ItemInstance result = currentInventory.Find(x => x.id == itemID);
        if (result != null)
        {
            result.count--;
            if (result.count <= 0)
                currentInventory.Remove(result);
        }

        RefreshInventory();
    }
}


[System.Serializable]
public class InventoryWrapper
{
    public List<ItemInstance> items = new List<ItemInstance>();
}

[System.Serializable]
public class ItemData
{   
    public string itemName;
    [TextArea]
    public string description;
}

[System.Serializable]
public class ItemInstance
{
    public int id;
    public int count;
    public bool consumable;
    public ItemData itemData;
}
