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


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    private void Start()
    {
        PopulateInventory();
    }

    public void PopulateInventory()
    {
        //Populate inventory from json file
        string itemFilePath = System.IO.Path.Combine(Application.streamingAssetsPath, "items.json");
        string jsonData = File.ReadAllText(itemFilePath);
        InventoryItems i_Items = JsonUtility.FromJson<InventoryItems>("{\"itemInstances\":" + jsonData + "}");

        for (int i = 0; i < i_Items.itemInstances.Count; i++)
        {
            //ItemInstance itemInstance = i_Items.itemInstances[i];
            itemDict.Add(i_Items.itemInstances[i].id, i_Items.itemInstances[i]);
            items.Add(i_Items.itemInstances[i]);
        }
    }

    public void UpdateInventory()
    {
        //Refresh inventory screen to reflect changes after calling AddItem or RemoveItem
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
}


public class UseItem
{
    public int id;
    public InteractObject obj;

    public UseItem(int u_id, InteractObject u_obj)
    {
        id = u_id;
        obj = u_obj;
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
