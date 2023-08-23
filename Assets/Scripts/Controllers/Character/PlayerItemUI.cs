using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerItemUI : MonoBehaviour
{
    public static PlayerItemUI instance;

    [SerializeField] Image itemIcon;
    [SerializeField] InventoryItem currentItem;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    public void UpdateCurrentItem(InventoryItem itemData)
    {
        currentItem = itemData;
        itemIcon.sprite = currentItem.icon;
    }

    public void SetIcon(Sprite sprite)
    {
        itemIcon.sprite = sprite;
    }
}
