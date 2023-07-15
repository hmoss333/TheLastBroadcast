using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamEffectController : MonoBehaviour
{
    public static CamEffectController instance;

    [SerializeField] AnalogGlitch glitchEffect;
    public bool effectOn { get; private set;} //Manually turn effect on/off
    private bool forceEffect = false;

    Coroutine shockEffect;


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
        effectOn = false;
    }

    private void Update()
    {
        glitchEffect.enabled = effectOn;

        if (forceEffect)
            glitchEffect.enabled = true;

        effectOn = false;
    }


    public void SetEffectValues(bool effectState)
    {
        effectOn = effectState;
    }

    public void ForceEffect(bool forceEffectState)
    {
        forceEffect = forceEffectState;
    }

    public void ShockEffect(float duration)
    {
        if (shockEffect == null)
            shockEffect = StartCoroutine(ShockRoutine(duration));
    }

    IEnumerator ShockRoutine(float duration)
    {
        forceEffect = true;
        yield return new WaitForSeconds(duration);
        forceEffect = false;

        shockEffect = null;
    }
}
