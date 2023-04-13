using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTriggerZone : MonoBehaviour
{
    [SerializeField] DialogueController dialogueObj;

    private void Start()
    {
        dialogueObj = GetComponent<DialogueController>();
    }

    private void Update()
    {
        this.enabled = !dialogueObj.hasActivated;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !dialogueObj.hasActivated)
        {
            PlayerController.instance.SetState(PlayerController.States.listening);
            if (dialogueObj.focusPoint != null)
            {
                CameraController.instance.SetLastTarget(dialogueObj.focusPoint); //Set the lat target to the camPos in case of reset
                CameraController.instance.SetTarget(dialogueObj.focusPoint); //Set the new camera target to the camPos
                CameraController.instance.SetRotation(true); //Force rotation
            }
            dialogueObj.Interact();
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
