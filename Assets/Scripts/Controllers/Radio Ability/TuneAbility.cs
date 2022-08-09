using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TuneAbility : RadioAbilityController
{
    public static TuneAbility instance;


    public bool isUsing; //used to toggle camera after effect for special abilities


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }

    public void SetTuning(bool isOn)
    {
        isUsing = isOn;
    }
}
