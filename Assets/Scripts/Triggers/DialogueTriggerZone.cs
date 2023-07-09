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

            if (CameraController.instance.GetRotState() == true)
                CameraController.instance.SetLastTarget(CameraController.instance.GetTarget().gameObject);

            if (dialogueObj.focusPoint != null)
            {
                CameraController.instance.SetTarget(dialogueObj.focusPoint); //Set the new camera target to the camPos
                CameraController.instance.SetRotation(true); //Force rotation
            }
            dialogueObj.Interact();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" && CameraController.instance.GetLastTarget() == PlayerController.instance.transform)
        {
            CameraController.instance.LoadLastTarget();
            CameraController.instance.SetRotation(false); //Disable forced rotation
        }
    }
}
