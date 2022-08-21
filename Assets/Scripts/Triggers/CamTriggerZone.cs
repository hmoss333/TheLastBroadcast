using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamTriggerZone : InteractObject
{
    [SerializeField] GameObject camPos;
    [SerializeField] float timeToFocus;
    float tempTime;
    bool isFocusing;


    private void Start()
    {
        tempTime = timeToFocus;
    }

    private void Update()
    {
        if (isFocusing)
        {
            tempTime -= Time.deltaTime;
            if (tempTime <= 0)
            {
                CameraController.instance.SetTarget(PlayerController.instance.gameObject);
                isFocusing = false;
                hasActivated = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hasActivated)
        {
            CameraController.instance.SetTarget(camPos);
            isFocusing = true;
        }
    }
}
