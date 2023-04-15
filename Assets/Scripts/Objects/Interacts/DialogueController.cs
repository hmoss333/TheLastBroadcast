using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class DialogueController : InteractObject
{
    //[SerializeField] float camResetTime;
    [SerializeField] bool talkOnce;
    [SerializeField] private string[] lines;
    [SerializeField] private int index;
    private bool canInteract;



    private void Update()
    {
        if (interacting)
        {
            //On button up, reset interact state
            if (!PlayerController.instance.inputMaster.Player.Interact.IsPressed())
            {             
                canInteract = false;
            }
            else if (PlayerController.instance.inputMaster.Player.Interact.IsPressed() && !canInteract)
            {
                Interact();
            }
        }
    }

    public override void Interact()
    {
        if (!canInteract)
        {
            if (index < lines.Length)
            {
                interacting = true; //Still in the dialogue tree
                canInteract = true; //Ready for next input
                UIController.instance.SetDialogueText(lines[index]);
            }
            else
            {
                interacting = false; //Exited the dialogue tree
                if (talkOnce)
                    SetHasActivated(); //Dialogue event has completed
                else
                    index = -1; //Reset dialogue
                CameraController.instance.SetLastTarget(PlayerController.instance.gameObject); //Once out of the trigger, set the player to the last camera target
                CameraController.instance.SetTarget(PlayerController.instance.gameObject); //Set the camera to focus on the player
                CameraController.instance.SetRotation(false);//.DelayResetRotState(camResetTime);
                PlayerController.instance.SetState(PlayerController.States.idle);
            }

            UIController.instance.ToggleDialogueUI(interacting);
            index++;
        }
    }

    public override void Activate()
    {
        active = true;
        index = 0;
    }
}
