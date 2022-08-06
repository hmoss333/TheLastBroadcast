using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TuneAbility : RadioAbilityController
{
    public static TuneAbility instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }
}
