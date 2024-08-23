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

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            CameraController.instance.SetTriggerState(true); //So long as player is in trigger volume, set trigger state to true

            //Covers for fringe cases for if the player walks into an adjacent cam triggerZone and then immediately exits back into the first zone
            //Previously would cause the camera to revert back to focusing on the player even though they would technically be in an active triggerZone
            if (CameraController.instance.GetTarget() != camPos.transform
                && PlayerController.instance.state != PlayerController.States.interacting)
            {
                OnTriggerEnter(other);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" && CameraController.instance.GetLastTarget() == camPos.transform)
        {
            Transform target;
            if (other.GetComponent<PlayerController>())
            {
                target = other.GetComponent<PlayerController>().lookTransform;
            }
            else
            {
                target = other.gameObject.transform;
            }


            CameraController.instance.SetLastTarget(target);//PlayerController.instance.lookTransform); //Once out of the trigger, set the player to the last camera target
            CameraController.instance.SetTarget(target); //PlayerController.instance.lookTransform); //Set the camera to focus on the player
            CameraController.instance.SetRotation(false); //Disable forced rotation
            CameraController.instance.SetTriggerState(false);
        }
    }
}
