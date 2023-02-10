using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class DialogueController : InteractObject
{
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
                hasActivated = true; //Dialogue event has completed
                PlayerController.instance.isListening = false;
                PlayerController.instance.state = PlayerController.States.idle;
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
