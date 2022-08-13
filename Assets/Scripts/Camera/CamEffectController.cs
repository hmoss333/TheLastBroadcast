using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamEffectController : MonoBehaviour
{
    public static CamEffectController instance;

    [SerializeField] AnalogGlitch glitchEffect;
    public bool effectOn; //Manually turn effect on/off


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    private void Start()
    {
        glitchEffect.enabled = false;
    }

    private void Update()
    {
        if (effectOn || TuneAbility.instance.isUsing || InvisAbility.instance.isUsing || RatAbility.instance.isUsing)
        {
            glitchEffect.enabled = true;
        }
        else
        {
            glitchEffect.enabled = false;
        }
    }

    void SetEffectValues()
    {

    }
}
