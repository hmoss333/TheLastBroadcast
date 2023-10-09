using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;


//TODO
//Update class to inherit from Dialogue Controller to handle dialogue progression
//Add to saved lore list if collected
//Still use a JSON for storing/loading text (?)
public class LorePickup : InteractObject
{
    [SerializeField] string loreText, loreTitle; //this should be loaded from csv file
    [SerializeField] int loreId;
    [SerializeField] List<string> dialogueItems;


    public int GetID()
    {
        return loreId;
    }

    public void SetValue(string text, string title)
    {
        loreText = text;
        loreTitle = title;
    }

    public override void StartInteract()
    {
        UIController.instance.ToggleLoreUI(loreText, loreTitle);
        print("Collected " + gameObject.name);
    }

    public override void EndInteract()
    {
        SaveDataController.instance.SaveLoreData(loreId);
        UIController.instance.ToggleLoreUI(loreText, loreTitle);
        SetHasActivated();
    }
}
