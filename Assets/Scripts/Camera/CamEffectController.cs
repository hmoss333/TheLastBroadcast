using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamEffectController : MonoBehaviour
{
    public static CamEffectController instance;

    [SerializeField] AnalogGlitch glitchEffect;
    [SerializeField] private bool effectOn = false; //Manually turn effect on/off


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
        glitchEffect.enabled = effectOn;
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
