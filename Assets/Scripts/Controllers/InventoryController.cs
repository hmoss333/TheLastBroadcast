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
    private Dictionary<int, ItemInstance> itemDict = new Dictionary<int, ItemInstance>();
    private List<InventoryItem> inventoryItems = new List<InventoryItem>();//InventoryItem[] inventoryItems;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        itemDestination = System.IO.Path.Combine(Application.persistentDataPath, "Items", "items.json");

        LoadItemFile();
        RefreshInventory();
    }

    private void Update()
    {
        if (PauseMenuController.instance.isPaused && PlayerController.instance.inputMaster.Player.Move.triggered) //&& currentPanel == inventoryPanel
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

                //Display the current selected item data
                SelectItem(inventoryItems[itemPosVal]);
            }
            catch { }
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
        selectedItem = null;
        InventoryItem[] currentItems = inventoryContent.GetComponentsInChildren<InventoryItem>();
        foreach(InventoryItem item in currentItems)
        {
            Destroy(item.gameObject);
        }
        inventoryTitle.text = "";
        inventoryDesc.text = "";

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
                    SelectItem(itemPrefab);
                    firstItem = false;
                }
            }
        }

        if (inventoryItems.Count <= 0) { PlayerItemUI.instance.SetIcon(null); }
        itemPosVal = 0;
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
        itemPosVal = 0;
        RefreshInventory();
    }

    public void RemoveItem(int itemID)
    {
        itemDict[itemID].hasItem = false;
        itemPosVal = 0;
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
    public ItemData itemType;

    //public ItemInstance(ItemData itemData)
    //{
    //    itemType = itemData;        
    //}
}
