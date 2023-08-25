using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerItemUI : MonoBehaviour
{
    public static PlayerItemUI instance;

    [SerializeField] Image itemIcon;
    [SerializeField] TMP_Text countText;
    public InventoryItem currentItem { get; private set; }
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
            itemIcon.gameObject.SetActive(true);
            itemIcon.sprite = currentItem.icon;

            int count = currentItem.itemData.count;
            countText.gameObject.SetActive(count > 1);
            countText.text = count.ToString();
        }
        catch
        {
            itemIcon.gameObject.SetActive(false);
            countText.text = string.Empty;
        }
    }

    public void SetIcon(Sprite sprite)
    {
        itemIcon.sprite = sprite;
    }
}
