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
            genAbilityText = GenerateAbilityText(abilityText);
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

    public static string getBetween(string strSource, string strStart, string strEnd)
    {
        if (strSource.Contains(strStart) && strSource.Contains(strEnd))
        {
            int Start, End;
            Start = strSource.IndexOf(strStart, 0) + strStart.Length;
            End = strSource.IndexOf(strEnd, Start);
            return strSource.Substring(Start, End - Start);
        }

        return "";
    }

    public string GetButtonName(string inputName)
    {
        InputAction inputAction = PlayerController.instance.inputMaster.FindAction(inputName);
        PlayerInput input = FindObjectOfType<PlayerInput>();
        string currentDevice = input.currentControlScheme;
        int bindingVal;

        //Set binding based on current device
        //Fill this in with more devices as needed
        if (currentDevice == "Keyboard")
            bindingVal = 1;
        else
            bindingVal = 0;

        string tempString = inputAction.bindings[bindingVal].ToDisplayString();
        return tempString;
    }

    string GenerateAbilityText(string originalText)
    {
        //Break up the abilityText to convert the input tag to the platform-specific value
        string abilityTextBegin = getBetween(abilityText, "", "{");
        string buttonString = GetButtonName(getBetween(abilityText, "{", "}"));
        string abilityTextEnd = getBetween(abilityText, "}", ".");
        string tempAbilityText = $"{abilityTextBegin}<color=#E5E727>{buttonString}</color>{abilityTextEnd}";

        return tempAbilityText;
    }
}
