using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerItemUI : MonoBehaviour
{
    public static PlayerItemUI instance;

    [SerializeField] Image itemIcon;
    [SerializeField] InventoryItem currentItem;
    [SerializeField] RectTransform itemPanel;
    [SerializeField] Vector2 inactivePos, activePos;
    [SerializeField] float slideSpeed;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    private void Update()
    {
        //Move Item panel into position based on selected item state
        itemPanel.anchoredPosition = Vector2.Lerp(itemPanel.anchoredPosition,
            currentItem != null ? activePos : inactivePos, slideSpeed * Time.deltaTime);
    }

    public void UpdateCurrentItem(InventoryItem itemData)
    {
        currentItem = itemData;

        try
        {
            itemIcon.sprite = currentItem.icon;
        }
        catch
        {
            itemIcon.sprite = null;
        }
    }

    public void SetIcon(Sprite sprite)
    {
        itemIcon.sprite = sprite;
    }
}
