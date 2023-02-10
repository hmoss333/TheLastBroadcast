using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LorePickup : InteractObject
{
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
        hasActivated = true;
        SaveDataController.instance.SaveLoreData(id);
        SaveDataController.instance.SaveObjectData(SceneManager.GetActiveScene().name);
        SaveDataController.instance.SaveFile();
        UIController.instance.ToggleLoreUI(loreText);
        gameObject.SetActive(false);
    }
}
