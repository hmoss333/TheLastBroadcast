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

public class InventoryController : MonoBehaviour
{
    public static InventoryController instance;

    private string itemDestination;
    [Header("UI Elements")]
    [SerializeField] RectTransform inventoryContent;
    [SerializeField] ScrollRect scrollRect;
    [SerializeField] InventoryItem inventoryItemPrefab;
    [SerializeField] TMP_Text inventoryTitle, inventoryDesc;

    [NaughtyAttributes.HorizontalLine]

    [Header("Inventory Values")]
    [SerializeField] InventoryItem selectedItem;
    private int itemPosVal;
    [SerializeField] private List<ItemInstance> items = new List<ItemInstance>();
    private Dictionary<int, ItemInstance> itemDict = new Dictionary<int, ItemInstance>();
    private InventoryItem[] inventoryItems;


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
            Canvas.ForceUpdateCanvases();
            float scrollValue = scrollRect.verticalScrollbar.value;
            Vector2 move = PlayerController.instance.inputMaster.Player.Move.ReadValue<Vector2>();
            if (move.x > 0)
            {
                itemPosVal++;
                //scrollValue -= scrollRect.content.GetComponent<GridLayoutGroup>().cellSize.y + scrollRect.content.GetComponent<GridLayoutGroup>().spacing.y * 2f;
            }
            if (move.x < 0)
            {
                itemPosVal--;
                //scrollValue += scrollRect.content.GetComponent<GridLayoutGroup>().cellSize.y + scrollRect.content.GetComponent<GridLayoutGroup>().spacing.y * 2f;
            }
            if (move.y > 0)
            {
                itemPosVal -= 3;
                //scrollValue += scrollRect.content.GetComponent<GridLayoutGroup>().cellSize.y + scrollRect.content.GetComponent<GridLayoutGroup>().spacing.y * 2f;
            }
            if (move.y < 0)
            {
                itemPosVal += 3;
                //scrollValue -= scrollRect.content.GetComponent<GridLayoutGroup>().cellSize.y + scrollRect.content.GetComponent<GridLayoutGroup>().spacing.y * 2f;
            }

            if (inventoryItems.Length > 6)
            {
                if (itemPosVal > 6 && itemPosVal % 6 != 0)
                    scrollValue -= scrollRect.content.GetComponent<GridLayoutGroup>().cellSize.y + scrollRect.content.GetComponent<GridLayoutGroup>().spacing.y * 2f;
                //(scrollRect.content.GetComponent<GridLayoutGroup>().cellSize.y * 2f + scrollRect.content.GetComponent<GridLayoutGroup>().spacing.y * 2f) / scrollRect.content.rect.height;
                else
                    scrollValue += scrollRect.content.GetComponent<GridLayoutGroup>().cellSize.y + scrollRect.content.GetComponent<GridLayoutGroup>().spacing.y * 2f;
                //(scrollRect.content.GetComponent<GridLayoutGroup>().cellSize.y * 2f + scrollRect.content.GetComponent<GridLayoutGroup>().spacing.y * 2f) / scrollRect.content.rect.height;
            }


            scrollRect.verticalScrollbar.value = scrollValue;

            //Keep value inside of the inventoryItems array
            if (itemPosVal <= 0)
            {
                itemPosVal = 0;
            }
            if (itemPosVal >= inventoryItems.Length - 1)
            {
                itemPosVal = inventoryItems.Length - 1;
            }

            foreach (InventoryItem item in inventoryItems)
            {
                item.ToggleHighlight(false);
            }

            //Display the current selected item data
            SelectItem(inventoryItems[itemPosVal]);
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
            print(itemDestination);
            string jsonData = File.ReadAllText(itemDestination);
            print(jsonData);
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
        InventoryItem[] currentItems = inventoryContent.GetComponentsInChildren<InventoryItem>();
        foreach(InventoryItem item in currentItems)
        {
            Destroy(item.gameObject);
        }
        inventoryTitle.text = "";
        inventoryDesc.text = "";

        for(int i = 0; i < items.Count; i++)
        {
            if (items[i].hasItem)
            {
                InventoryItem itemPrefab = Instantiate(inventoryItemPrefab, inventoryContent);
                itemPrefab.itemData = items[i];
                //Set icon value
                //Apply icon

                if (i == 0)
                {
                    SelectItem(itemPrefab);
                    itemPosVal = 0;
                }
            }
        }

        inventoryItems = inventoryContent.GetComponentsInChildren<InventoryItem>();
    }

    public void SelectItem(InventoryItem item)
    {
        selectedItem = item;
        selectedItem.ToggleHighlight(true);
        ShowItemData(item.itemData);
    }

    public void ShowItemData(ItemInstance item)
    {
        inventoryTitle.text = item.itemType.itemName;
        inventoryDesc.text = item.itemType.description;
    }


    public void AddItem(int itemID)
    {
        itemDict[itemID].hasItem = true;
        RefreshInventory();
    }

    public void RemoveItem(int itemID)
    {
        itemDict[itemID].hasItem = false;
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
