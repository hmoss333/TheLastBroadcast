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
    [SerializeField] bool talkOnce;
    [SerializeField] private string[] lines;
    [SerializeField] private int index;
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
                print("Button pressed");
                if (TextWriter.instace.isTyping)
                {
                    print("Stop typing");
                    TextWriter.instace.StopTyping();
                }
                else
                {
                    print("Progress dialogue");
                    Interact();
                }
            }
        }
    }

    public override void Interact()
    {
        if (!canInteract && !TextWriter.instace.isTyping)
        {
            if (index < lines.Length)
            {
                interacting = true; //Still in the dialogue tree
                canInteract = true; //Ready for next input
                UIController.instance.SetDialogueText(lines[index], true);
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

                m_OnTrigger.Invoke();
            }

            UIController.instance.ToggleDialogueUI(interacting);
            index++;
        }
    }
}
