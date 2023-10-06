using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DialogueController))]
public class DialogueTriggerZone : MonoBehaviour
{
    private DialogueController dialogueObj;
    [SerializeField] bool setFocusPointRot = true; [Tooltip("Used with the DialogueController's focus point. Determines if the camera should match the focus point's rotation. Default to true.")]

    private void Start()
    {
        dialogueObj = GetComponent<DialogueController>();
    }

    private void FixedUpdate()
    {
        this.enabled = !dialogueObj.hasActivated;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !dialogueObj.hasActivated && !dialogueObj.interacting)
        {
            PlayerController.instance.SetState(PlayerController.States.listening);

            if (CameraController.instance.GetRotState() == true)
                CameraController.instance.SetLastTarget(CameraController.instance.GetTarget());

            if (dialogueObj.focusPoint != null)
            {
                CameraController.instance.SetTarget(dialogueObj.focusPoint); //Set the new camera target to the camPos
                CameraController.instance.SetRotation(setFocusPointRot); //Force rotation
            }
            dialogueObj.Interact();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" && CameraController.instance.GetLastTarget() == PlayerController.instance.lookTransform)
        {
            CameraController.instance.LoadLastTarget();
            CameraController.instance.SetRotation(false); //Disable forced rotation
        }
    }
}
