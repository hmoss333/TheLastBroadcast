using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryItem : MonoBehaviour
{
    public ItemInstance itemData;
    public Sprite icon;
    [SerializeField] Image image;


    public void DisplayItemData()
    {
        InventoryController.instance.ShowItemData(itemData);
    }
}
