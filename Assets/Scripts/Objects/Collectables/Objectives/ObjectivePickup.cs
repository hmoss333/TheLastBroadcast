using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectivePickup : Collectable
{
    [SerializeField] string scenarioName;
    public new enum CollectType
    {
        frequency,
        powerLevel,
        antenna
    }
    public new CollectType collectType;


    public override void Interact()
    {
        SaveDataController.instance.UpdateScenario(scenarioName, collectType.ToString());
        base.Interact();
    }
}
