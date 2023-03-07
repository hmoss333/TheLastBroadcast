using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AbilityPickup;

public class CardPickup : InteractObject
{
    [SerializeField] int cardLevel;
    [SerializeField] Sprite cardIcon;

    private void Start()
    {
        gameObject.SetActive(!hasActivated);      
    }

    public override void StartInteract()
    {
        UIController.instance.ToggleAbilityUI($"Collected Security Card Level {cardLevel}", cardIcon);
        if (SaveDataController.instance.GetSecurityCardLevel() < cardLevel)
        {
            SaveDataController.instance.SetSecurityCardLevel(cardLevel);
            SaveDataController.instance.SaveFile();
        }
        print($"Collected Security Card Level {cardLevel}");
    }

    public override void EndInteract()
    {
        SetHasActivated();
        UIController.instance.ToggleAbilityUI("", cardIcon);
        gameObject.SetActive(false);
    }
}
