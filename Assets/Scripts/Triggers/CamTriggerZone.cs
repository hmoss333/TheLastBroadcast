using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamTriggerZone : MonoBehaviour
{
    [SerializeField] GameObject camPos;
    [SerializeField] bool snapCamPos = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (snapCamPos)
            {
                //Force cam position/rotation to match camPos
                CameraController.instance.transform.position = camPos.transform.position;
                CameraController.instance.transform.rotation = camPos.transform.rotation;
            }

            CameraController.instance.SetLastTarget(camPos.transform); //Set the lat target to the camPos in case of reset
            CameraController.instance.SetTarget(camPos.transform); //Set the new camera target to the camPos
            CameraController.instance.SetRotation(true); //Force rotation
            CameraController.instance.SetTriggerState(true); //Set camera to acknowledge that player is in a trigger volume
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" && CameraController.instance.GetLastTarget() == camPos.transform)
        {
            CameraController.instance.SetLastTarget(PlayerController.instance.lookTransform); //Once out of the trigger, set the player to the last camera target
            CameraController.instance.SetTarget(PlayerController.instance.lookTransform); //Set the camera to focus on the player
            CameraController.instance.SetRotation(false); //Disable forced rotation
            CameraController.instance.SetTriggerState(false);
        }
    }
}
