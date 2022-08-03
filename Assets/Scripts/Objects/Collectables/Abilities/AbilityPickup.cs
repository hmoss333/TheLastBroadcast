using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityPickup : Collectable
{
    public new enum CollectType
    {
        radio,
        crowbar,
        book,
        hand,
        mirror
    }
    public CollectType collectType;


    public override void Interact()
    {
        SaveDataController.instance.GiveAbility(collectType.ToString());
        base.Interact();
    }
}
