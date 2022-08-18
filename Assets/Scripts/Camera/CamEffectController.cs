using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamEffectController : MonoBehaviour
{
    public static CamEffectController instance;

    [SerializeField] AnalogGlitch glitchEffect;
    private bool effectOn; //Manually turn effect on/off


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



    public void SetEffectValues(bool effectState)
    {
        effectOn = effectState;
    }



    public void ShockEffect(float duration)
    {
        StartCoroutine(ShockRoutine(duration));
    }

    IEnumerator ShockRoutine(float duration)
    {
        effectOn = true;

        yield return new WaitForSeconds(duration);

        effectOn = false;
    }
}
