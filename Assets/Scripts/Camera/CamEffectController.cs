using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamEffectController : MonoBehaviour
{
    [SerializeField] VHSPostProcessEffect vhsEffect;

    private void Start()
    {
        vhsEffect.enabled = false;
    }

    private void Update()
    {
        if (TuneAbility.instance.isUsing || InvisAbility.instance.isUsing || RatAbility.instance.isUsing)
        {
            vhsEffect.enabled = true;
        }
        else
        {
            vhsEffect.enabled = false;
        }
    }
}
