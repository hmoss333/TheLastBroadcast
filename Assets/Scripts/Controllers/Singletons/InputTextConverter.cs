using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputTextConverter : MonoBehaviour
{
    public static InputTextConverter instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }


    string getBetween(string strSource, string strStart, string strEnd)
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

    string GetButtonName(string inputName)
    {
        InputAction inputAction = InputController.instance.inputMaster.FindAction(inputName);
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

    public string GenerateText(string originalText)
    {
        //Break up the originalText to convert the input tag to the platform-specific value
        string textBegin = getBetween(originalText, "", "{");
        string buttonString = GetButtonName(getBetween(originalText, "{", "}"));
        string abilityTextEnd = getBetween(originalText, "}", ".");
        string returnText = $"{textBegin}{buttonString}{abilityTextEnd}";

        return returnText;
    }

    public string GenerateText(string originalText, Color color)
    {
        string colorText = ColorUtility.ToHtmlStringRGB(color);

        //Break up the originalText to convert the input tag to the platform-specific value
        string textBegin = getBetween(originalText, "", "{");
        string buttonString = GetButtonName(getBetween(originalText, "{", "}"));
        string abilityTextEnd = getBetween(originalText, "}", ".");
        string returnText = $"{textBegin}<color=#{colorText}>{buttonString}</color>{abilityTextEnd}";

        return returnText;
    }

    public string GenerateText(string originalText, string colorText)
    {
        //Break up the originalText to convert the input tag to the platform-specific value
        string textBegin = getBetween(originalText, "", "{");
        string buttonString = GetButtonName(getBetween(originalText, "{", "}"));
        string abilityTextEnd = getBetween(originalText, "}", ".");
        string returnText = $"{textBegin}<color=#{colorText}>{buttonString}</color>{abilityTextEnd}"; //<color=#E5E727>

        return returnText;
    }
}
