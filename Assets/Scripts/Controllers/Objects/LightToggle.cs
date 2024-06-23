using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightToggle : MonoBehaviour
{
    Light lightObj;
    [SerializeField] Color colorOn, colorOff;
    bool lightToggle = true;

    // Start is called before the first frame update
    void Start()
    {
        lightObj = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        lightObj.color = lightToggle ? colorOn : colorOff;
    }

    public void ToggleLight()
    {
        lightToggle = !lightToggle;
    }

    public void ToggleLight(bool value)
    {
        lightToggle = value;
    }
}
