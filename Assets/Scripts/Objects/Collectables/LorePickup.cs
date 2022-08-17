using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LorePickup : InteractObject
{
    [SerializeField] string loreText; //this should be loaded from csv file
    [SerializeField] int id;


    private void Update()
    {
        gameObject.SetActive(!hasActivated);
    }

    public override void Interact()
    {
        print ("Collected " + gameObject.name);
        base.Interact();

        if (interacting)
        {
            UIController.instance.ToggleLoreUI(loreText);
        }
        else
        {
            hasActivated = true;
            SaveDataController.instance.SaveLoreData(id);
            SaveDataController.instance.SaveObjectData(SceneManager.GetActiveScene().name);
            SaveDataController.instance.SaveFile();
            UIController.instance.ToggleLoreUI(loreText);
            gameObject.SetActive(false);
        }
    }
}
