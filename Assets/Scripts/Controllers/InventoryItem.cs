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
    public Sprite icon { get; private set; }
    [SerializeField] Image image;
    [SerializeField] TMP_Text countText;
    [SerializeField] GameObject highlight;


    private void Start()
    {
        SetIcon(itemData.itemType.itemName);
    }

    public void SetIcon(string itemName)
    {
        Sprite tempSprite = atlas.GetSprite(itemName.ToLower());
        if (tempSprite != null)
        {
            icon = tempSprite;
            image.sprite = icon;
        }
        countText.text = itemData.count.ToString();
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
