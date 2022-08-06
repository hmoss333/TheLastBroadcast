using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : InteractObject
{
    public enum CollectType { }
    CollectType collectType;


    private void Update()
    {
        gameObject.SetActive(hasActivated ? false : true);
    }

    public override void Interact()
    {
        Debug.Log("Collected " + gameObject.name);
        hasActivated = true;
        //SaveDataController.instance.GiveAbility(collectType.ToString());
        SaveDataController.instance.SaveObjectData(SaveDataController.instance.GetSaveData().currentScene);
        SaveDataController.instance.SaveFile();
    }
}
