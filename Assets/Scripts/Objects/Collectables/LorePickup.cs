using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LorePickup : InteractObject
{
    //public enum Type { personalLogs, researchNotes, secretHistory }
    //public Type loreType;
    [SerializeField] string loreText, loreTitle; //this should be loaded from csv file
    [SerializeField] int id;


    private void Start()
    {
        gameObject.SetActive(!hasActivated);


    }

    public int GetID()
    {
        return id;
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
        SaveDataController.instance.SaveLoreData(id);
        UIController.instance.ToggleLoreUI(loreText, loreTitle);
        SetHasActivated();
        gameObject.SetActive(false);
    }
}
