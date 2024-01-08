using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class DialogueController : InteractObject
{
    [SerializeField] bool talkOnce, quoteMarks;
    [SerializeField] private string speakerName;
    [SerializeField] private string[] lines;
    private int index;
    private bool canInteract, pauseTyping;
    private float iconTimer = 4f;


    private void Update()
    {
        if (interacting)
        {
            //Only on the first interaction
            //If the player waits more than 4 seconds then display the interact icon prompt
            //Else always hide the icon
            if (index <= 1)
            {
                if (iconTimer > 0)
                    iconTimer -= Time.deltaTime;
                else
                    UIController.instance.ToggleDialogueInputIcon(true);
            }
            else
            {
                UIController.instance.ToggleDialogueInputIcon(false);
            }


            //On button up, reset interact state
            if (!PlayerController.instance.inputMaster.Player.Interact.IsPressed())
            {
                canInteract = false;
            }
            else if (!canInteract && PlayerController.instance.inputMaster.Player.Interact.triggered)
            {
                if (TextWriter.instace.isTyping)
                {
                    TextWriter.instace.StopTyping();
                }
                else
                {
                    Interact();
                }
            }
        }
    }

    public override void Interact()
    {
        if (!canInteract && !TextWriter.instace.isTyping)
        {
            /////Temp
            if ((index < 1 || index > lines.Length - 1)
                && focusPoint != null)
            {
                print("Focus camera");
                PlayerController.instance.ToggleAvatar();
                CameraController.instance.SetTarget(focusPoint);// : CameraController.instance.GetLastTarget());
                CameraController.instance.FocusTarget();
                if (CameraController.instance.GetTriggerState())
                    CameraController.instance.SetRotation(true);
            }
            /////
            


            if (index < lines.Length)
            {
                interacting = true; //Still in the dialogue tree
                canInteract = true; //Ready for next input
                UIController.instance.SetDialogueText(lines[index], true);
                if (quoteMarks)
                    UIController.instance.ToggleQuoteMarks(speakerName, true);
            }
            else
            {
                interacting = false; //Exited the dialogue tree
                if (talkOnce)
                    SetHasActivated(); //Dialogue event has completed
                else
                    index = -1; //Reset dialogue index

                CameraController.instance.LoadLastTarget();
                if (CameraController.instance.GetLastTarget() == PlayerController.instance.lookTransform)
                    CameraController.instance.SetRotation(false); //Disable forced rotation

                PlayerController.instance.SetState(PlayerController.States.idle); //Release player from interact state
                UIController.instance.ToggleDialogueInputIcon(false); //Force disable Input Icon
                UIController.instance.ToggleQuoteMarks(speakerName, false); //Disable quotation marks

                m_OnTrigger.Invoke();
            }

            UIController.instance.ToggleDialogueUI(interacting);
            index++;
        }
    }
}
