using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : InteractObject
{
    [SerializeField] string abilityName;


    private void Update()
    {
        gameObject.SetActive(hasActivated ? false : true);
    }

    public override void Interact()
    {
        Debug.Log("Collected " + gameObject.name);
        hasActivated = true;
        SaveDataController.instance.GiveAbility(abilityName);
        SceneInitController.instance.SaveInteractObjs();
        SaveDataController.instance.SaveFile();
    }
}
