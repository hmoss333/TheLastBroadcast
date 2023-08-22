using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;
using TMPro;

public class InventoryItem : MonoBehaviour
{
    public int ID;
    public ItemInstance itemData;
    [SerializeField] SpriteAtlas atlas;
    [SerializeField] Sprite icon;
    [SerializeField] Image image;
    [SerializeField] GameObject highlight;


    private void Awake()
    {
        Sprite tempSprite = atlas.GetSprite(itemData.itemType.itemName);
        if (tempSprite != null)
        {
            icon = tempSprite;
            image.sprite = icon;
        }
    }

    public void DisplayItemData()
    {
        InventoryController.instance.SelectItem(this);
    }

    public void ToggleHighlight(bool value)
    {
        highlight.SetActive(value);
    }
}
