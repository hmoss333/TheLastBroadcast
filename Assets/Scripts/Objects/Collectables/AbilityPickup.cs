using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AbilityPickup : InteractObject
{
    public enum CollectType
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
        SaveDataController.instance.SaveFile();
        print("Collected " + gameObject.name);
        PlayerController.instance.inputMaster.Player.Radio.bindings.ToString();
    }

    public override void EndInteract()
    {
        SetHasActivated();
        UIController.instance.ToggleAbilityUI(abilityText, abilityIcon);
        gameObject.SetActive(false);
    }
}
