using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;
using TMPro;

public class InventoryItem : MonoBehaviour
{
    public int ID;
    public ItemInstance itemInstance;
    [SerializeField] SpriteAtlas atlas;
    [SerializeField] public Sprite icon; //{ get; private set; }
    [SerializeField] Image image;
    [SerializeField] TMP_Text countText;
    [SerializeField] GameObject highlight;


    private void Start()
    {
        icon = GetComponent<Image>().sprite;
        SetIcon(itemInstance.itemData.itemName);
    }

    public void SetIcon(string itemName)
    {
        Sprite tempSprite = atlas.GetSprite(itemName.ToLower());
        if (tempSprite != null)
        {
            icon = tempSprite;
            image.sprite = icon;
        }
        countText.gameObject.SetActive(itemInstance.count > 1);
        countText.text = itemInstance.count.ToString();
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
