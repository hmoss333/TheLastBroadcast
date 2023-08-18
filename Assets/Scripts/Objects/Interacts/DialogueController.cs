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
    private int index;
    private bool canInteract;
    private float iconTimer = 4f;


    [FormerlySerializedAs("onTrigger")]
    [SerializeField]
    private UnityEvent m_OnTrigger = new UnityEvent();


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
                UIController.instance.SetDialogueText(lines[index], true);
            }
            else
            {
                interacting = false; //Exited the dialogue tree
                if (talkOnce)
                    SetHasActivated(); //Dialogue event has completed
                else
                    index = -1; //Reset dialogue

                if (CameraController.instance.GetLastTarget() == PlayerController.instance.lookTransform)
                {
                    CameraController.instance.LoadLastTarget();
                    CameraController.instance.SetRotation(false); //Disable forced rotation
                }

                PlayerController.instance.SetState(PlayerController.States.idle); //Allow player to move freely

                m_OnTrigger.Invoke();
            }

            UIController.instance.ToggleDialogueUI(interacting);
            index++;           
        }
    }

    public override void Activate()
    {
        base.Activate();
    }
}
