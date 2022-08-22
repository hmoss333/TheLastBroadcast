using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamTriggerZone : MonoBehaviour
{
    [SerializeField] GameObject camPos;
    [SerializeField] bool isFocusing;



    private void Update()
    {
        if (isFocusing && CameraController.instance.GetTarget().gameObject == camPos)
        {
            CameraController.instance.SetRotation(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !isFocusing)
        {
            isFocusing = true;
            CameraController.instance.SetTarget(camPos);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" && isFocusing)
        {
            isFocusing = false;
            CameraController.instance.SetTarget(PlayerController.instance.gameObject);
            CameraController.instance.SetRotation(false);   //Manually disable setRot one trigger exit
        }
    }
}
