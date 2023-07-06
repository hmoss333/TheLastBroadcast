using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightPulse : MonoBehaviour
{
    Light lightObj;
    [SerializeField] float pulseTime, maxIntensity;

    // Start is called before the first frame update
    void Start()
    {
        lightObj = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        lightObj.intensity = Mathf.PingPong(Time.time * pulseTime, maxIntensity);
    }
}
