using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryItem : MonoBehaviour
{
    public ItemInstance itemData;
    [SerializeField] Sprite icon;
    [SerializeField] Image image;
    [SerializeField] GameObject highlight;


    public void DisplayItemData()
    {
        InventoryController.instance.ShowItemData(itemData);
    }

    public void ToggleHighlight(bool value)
    {
        highlight.SetActive(value);
    }
}
