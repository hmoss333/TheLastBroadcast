using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TuneAbility : RadioAbilityController
{
    public static TuneAbility instance;
    public static float chargeCost { get; private set; }

    [HideInInspector] public bool isUsing; //used to toggle camera after effect for special abilities


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);

        chargeCost = abilityData.chargeCost;
    }

    private void Update()
    {
        if (isUsing)
            RadioController.instance.UsingAbility();
    }

    public void SetTuning(bool isOn)
    {
        if (abilityData.isActive)
            isUsing = isOn;
    }
}
