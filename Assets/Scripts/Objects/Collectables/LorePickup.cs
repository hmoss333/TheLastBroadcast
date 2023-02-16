using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LorePickup : InteractObject
{
    public enum Type { personalLogs, researchNotes, secretHistory }
    public Type loreType;
    [SerializeField] string loreText; //this should be loaded from csv file
    [SerializeField] int id;


    private void Start()
    {
        gameObject.SetActive(!hasActivated);
    }

    public override void StartInteract()
    {
        UIController.instance.ToggleLoreUI(loreText);
        print("Collected " + gameObject.name);
    }

    public override void EndInteract()
    {
        //hasActivated = true;
        SaveDataController.instance.SaveLoreData(id);
        UIController.instance.ToggleLoreUI(loreText);
        SetHasActivated();
        gameObject.SetActive(false);
    }
}
