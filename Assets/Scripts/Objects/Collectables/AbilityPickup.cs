using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.Windows;

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
        string genAbilityText;
        try
        {
            genAbilityText = InputTextConverter.instance.GenerateText(abilityText, "E5E727");
        }
        catch
        {
            genAbilityText = abilityText;
        }

        string abilityName = collectType.ToString();
        abilityName = abilityName[0].ToString().ToUpper() + abilityName.Substring(1);
        UIController.instance.ToggleAbilityUI(abilityName, genAbilityText, abilityIcon);
        SaveDataController.instance.GiveAbility(collectType.ToString());
        print("Collected " + gameObject.name);
    }

    public override void EndInteract()
    {
        SetHasActivated();
        string abilityName = collectType.ToString();
        abilityName = abilityName[0].ToString().ToUpper() + abilityName.Substring(1);
        UIController.instance.ToggleAbilityUI(abilityName, abilityText, abilityIcon);

        m_OnTrigger.Invoke();

        gameObject.SetActive(false);
    }
}
