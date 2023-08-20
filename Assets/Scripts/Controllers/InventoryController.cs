using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;

public class InventoryController : MonoBehaviour
{
    public static InventoryController instance;

    public List<ItemInstance> items = new();
    Dictionary<int, ItemInstance> itemDict = new Dictionary<int, ItemInstance>();
    private string itemDestination;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        itemDestination = System.IO.Path.Combine(Application.persistentDataPath, "Items", "items.json");

        LoadItemFile();
    }

    public void CreateNewItemFile()
    {
        Directory.CreateDirectory(System.IO.Path.Combine(Application.persistentDataPath, "Items"));
        string resourcesPath = System.IO.Path.Combine(Application.streamingAssetsPath, "items.json");
        string jsonData = File.ReadAllText(resourcesPath);

        InventoryItems i_Items = JsonUtility.FromJson<InventoryItems>("{\"itemInstances\":" + jsonData + "}");

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
            InventoryItems i_Items = JsonUtility.FromJson<InventoryItems>(jsonData);

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
        InventoryItems tempItems = new InventoryItems();
        tempItems.itemInstances = items;

        string jsonData = JsonUtility.ToJson(tempItems);
        print("Saving Item Data:" + jsonData);
        File.WriteAllText(itemDestination, jsonData);
    }

    public void AddItem(int itemID)
    {
        itemDict[itemID].hasItem = true;
    }

    public void RemoveItem(int itemID)
    {
        itemDict[itemID].hasItem = false;
    }

    public bool HasItem(int itemID)
    {
        ItemInstance itemInstance = itemDict[itemID];
        return itemInstance.hasItem;
    }

    public void UseItem(int itemID)
    {
        if (HasItem(itemID))
        {
            print($"Used item: {itemDict[itemID].itemType.itemName}");
            RemoveItem(itemID);
        }
    }

    public ItemInstance GetItem(int itemID)
    {
        return itemDict[itemID];
    }
}




[System.Serializable]
public class InventoryItems
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
