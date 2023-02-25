using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AbilityPickup : InteractObject
{
    public new enum CollectType
    {
        radio,
        crowbar,
        gasmask,
        flashlight,
        book,
        mirror
    }
    public CollectType collectType;

    [SerializeField] string abilityText;
    [SerializeField] Sprite abilityIcon;


    private void Start()
    {
        gameObject.SetActive(!hasActivated);
    }

    public override void StartInteract()
    {
        UIController.instance.ToggleAbilityUI(abilityText, abilityIcon);
        SaveDataController.instance.GiveAbility(collectType.ToString());
        print("Collected " + gameObject.name);
    }

    public override void EndInteract()
    {
        //hasActivated = true;
        SetHasActivated();
        UIController.instance.ToggleAbilityUI(abilityText, abilityIcon);
        gameObject.SetActive(false);
    }
}
