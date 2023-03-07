using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AbilityPickup;

public class CardPickup : InteractObject
{
    [SerializeField] int cardLevel;
    [SerializeField] string cardText;
    [SerializeField] Sprite cardIcon;

    private void Start()
    {
        gameObject.SetActive(!hasActivated);
    }

    public override void StartInteract()
    {
        UIController.instance.ToggleAbilityUI(cardText, cardIcon);
        SaveDataController.instance.SetSecurityCardLevel(cardLevel);
        SaveDataController.instance.SaveFile();
        print($"Collected Security Card Level {cardLevel}");
    }

    public override void EndInteract()
    {
        SetHasActivated();
        UIController.instance.ToggleAbilityUI(cardText, cardIcon);
        gameObject.SetActive(false);
    }
}
