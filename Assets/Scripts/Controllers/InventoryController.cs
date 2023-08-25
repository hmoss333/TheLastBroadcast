using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using System.IO;
using TMPro;
using UnityEngine.UI.Extensions;
using static UnityEngine.GraphicsBuffer;
using static UnityEditor.Progress;

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

    public InventoryItem selectedItem;// { get; private set; }
    [SerializeField] private int itemPosVal;
    [SerializeField] private List<ItemInstance> items = new List<ItemInstance>();
    public Dictionary<int, ItemInstance> itemDict { get; private set; }//= new Dictionary<int, ItemInstance>();
    private List<InventoryItem> inventoryItems = new List<InventoryItem>();


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
                    ShowItemData(inventoryItems[itemPosVal].itemData);
                }
                catch { }
            }

            if (PlayerController.instance.inputMaster.Player.Interact.triggered)
            {
                print("Pressed interact");
                if (selectedItem != inventoryItems[itemPosVal])
                    SelectItem(inventoryItems[itemPosVal]);
                else
                {
                    selectedItem = null;
                    PlayerItemUI.instance.UpdateCurrentItem(selectedItem);
                }
            }
        }
    }


    public void CreateNewItemFile()
    {
        Directory.CreateDirectory(System.IO.Path.Combine(Application.persistentDataPath, "Items"));
        string resourcesPath = System.IO.Path.Combine(Application.streamingAssetsPath, "items.json");
        string jsonData = File.ReadAllText(resourcesPath);

        InventoryWrapper i_Items = JsonUtility.FromJson<InventoryWrapper>("{\"itemInstances\":" + jsonData + "}");

        for (int i = 0; i < i_Items.itemInstances.Count; i++)
        {
            itemDict.Add(i_Items.itemInstances[i].id, i_Items.itemInstances[i]);
            items.Add(i_Items.itemInstances[i]);
        }
    }

    public void LoadItemFile()
    {
        if (File.Exists(itemDestination))
        {
            print("Loading item data from file");
            string jsonData = File.ReadAllText(itemDestination);
            InventoryWrapper i_Items = JsonUtility.FromJson<InventoryWrapper>(jsonData);

            for (int i = 0; i < i_Items.itemInstances.Count; i++)
            {
                itemDict.Add(i_Items.itemInstances[i].id, i_Items.itemInstances[i]);
                items.Add(i_Items.itemInstances[i]);
            }
        }
        else
        {
            print("Creating new item file");
            CreateNewItemFile();
            SaveItemData();
        }
    }

    public void SaveItemData()
    {
        InventoryWrapper tempItems = new InventoryWrapper();
        tempItems.itemInstances = items;

        string jsonData = JsonUtility.ToJson(tempItems);
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
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].hasItem)
            {
                InventoryItem itemPrefab = Instantiate(inventoryItemPrefab, inventoryContent);
                itemPrefab.ID = i;
                itemPrefab.itemData = items[i];
                itemPrefab.SetIcon(items[i].itemType.itemName);
                inventoryItems.Add(itemPrefab);

                if (firstItem)
                {
                    ShowItemData(itemPrefab.itemData);
                    firstItem = false;
                }
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
        //If inventory is empty, hide icon
        else
        {        
            PlayerItemUI.instance.SetIcon(null);
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
        ShowItemData(item.itemData);
        PlayerItemUI.instance.UpdateCurrentItem(item);
    }

    public void ShowItemData(ItemInstance item)
    {
        inventoryTitle.text = item.itemType.itemName;
        inventoryDesc.text = item.itemType.description;
    }


    public void AddItem(int itemID)
    {
        itemDict[itemID].hasItem = true;
        itemDict[itemID].count++;
        itemPosVal = 0;
        RefreshInventory();
    }

    public void RemoveItem(int itemID)
    {
        print("Removing item");
        itemDict[itemID].count--;
        if (itemDict[itemID].count <= 0)
        {
            itemDict[itemID].hasItem = false;
            itemPosVal = 0;
            selectedItem = null;
        }
        RefreshInventory();
    }

    public bool HasItem(int itemID)
    {
        ItemInstance itemInstance = itemDict[itemID];
        return itemInstance.hasItem;
    }

    public ItemInstance GetItem(int itemID)
    {
        return itemDict[itemID];
    }
}




[System.Serializable]
public class InventoryWrapper
{
    public List<ItemInstance> itemInstances = new List<ItemInstance>();
}


[System.Serializable]
public class ItemData
{   
    public string itemName;
    //public Sprite icon;
    [TextArea]
    public string description;
}

[System.Serializable]
public class ItemInstance
{
    public int id;
    public bool hasItem;
    public int count;
    public ItemData itemType;

    //public ItemInstance(ItemData itemData)
    //{
    //    itemType = itemData;        
    //}
}
