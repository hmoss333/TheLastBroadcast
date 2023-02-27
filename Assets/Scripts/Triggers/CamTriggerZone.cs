using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamTriggerZone : MonoBehaviour
{
    [SerializeField] GameObject camPos;


    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player" && PlayerController.instance.state != PlayerController.States.radio)
        {
            CameraController.instance.SetLastTarget(camPos); //Set the lat target to the camPos in case of reset
            CameraController.instance.SetTarget(camPos); //Set the new camera target to the camPos
            CameraController.instance.SetRotation(true); //Force rotation
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            CameraController.instance.SetLastTarget(PlayerController.instance.gameObject); //Once out of the trigger, set the player to the last camera target
            CameraController.instance.SetTarget(PlayerController.instance.gameObject); //Set the camera to focus on the player
            CameraController.instance.SetRotation(false); //Disable forced rotation
        }
    }
}
