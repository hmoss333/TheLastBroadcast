using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadioAbilityController : MonoBehaviour
{
    public enum RadioAbilities { Tune, Invisibility, Rats }
    public RadioAbilities ability;

    public RadioAbility abilityData;

    public virtual void Start()
    {
        abilityData = new RadioAbility();
        abilityData = SaveDataController.instance.GetRadioAbility(ability.ToString());
    }
}
