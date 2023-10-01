using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerPoisonFog : SaveObject
{
    [SerializeField] private int damage;
    [SerializeField] private bool gasmask;

    private void Update()
    {
        gameObject.SetActive(active);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            gasmask = InventoryController.instance.selectedItem != null
                && InventoryController.instance.selectedItem.itemInstance.itemData.itemName.ToLower() == ("gasmask");

            if (!gasmask)
                other.GetComponent<Health>().Hurt(damage, false);
        }
    }
}
