using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamTriggerZone : MonoBehaviour
{
    [SerializeField] GameObject camPos;
    [SerializeField] bool isFocusing;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !isFocusing)
        {
            CameraController.instance.SetTarget(camPos);
            CameraController.instance.SetRotation();
            isFocusing = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" && isFocusing)
        {
            CameraController.instance.SetTarget(PlayerController.instance.gameObject);
            CameraController.instance.SetRotation();
            isFocusing = false;      
        }
    }
}
