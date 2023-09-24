using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamEffectController : MonoBehaviour
{
    public static CamEffectController instance;

    [SerializeField] AnalogGlitch glitchEffect;
    public bool effectOn;// { get; private set;} //Manually turn effect on/off
    //private bool forceEffect = false;

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
        if (shockEffect != null)
            glitchEffect.enabled = true;
        else
            glitchEffect.enabled = effectOn;

        effectOn = false;
    }


    public void SetEffectState(bool effectState)
    {
        effectOn = effectState;
    }

    public void ShockEffect(float duration)
    {
        if (shockEffect == null)
            shockEffect = StartCoroutine(ShockRoutine(duration));
    }

    IEnumerator ShockRoutine(float duration)
    {
        yield return new WaitForSeconds(duration);

        shockEffect = null;
    }
}
