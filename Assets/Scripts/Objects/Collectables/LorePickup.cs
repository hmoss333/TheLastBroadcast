using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;


//TODO
//Update class to inherit from Dialogue Controller to handle dialogue progression
//Add to saved lore list if collected
//Still use a JSON for storing/loading text (?)
public class LorePickup : InteractObject
{
    [SerializeField] string loreTitle; //this should be loaded from csv file
    [SerializeField] int loreId;
    [SerializeField] List<string> lines = new List<string>();
    private int index;
    private bool canInteract;
    private float iconTimer = 4f;


    public int GetID()
    {
        return loreId;
    }

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
            if (!InputController.instance.inputMaster.Player.Interact.IsPressed())
            {
                canInteract = false;
            }
            else if (!canInteract && InputController.instance.inputMaster.Player.Interact.triggered)
            {
                Interact();
            }
        }
    }

    public override void Interact()
    {
        if (!canInteract && !TextWriter.instace.isTyping)
        {
            if (index < lines.Count)
            {
                interacting = true; //Still in the dialogue tree
                canInteract = true; //Ready for next input
                UIController.instance.SetLoreText(lines[index], loreTitle);
                UIController.instance.ToggleLoreUI(true);
            }
            else
            {
                interacting = false; //Exited the dialogue tree
                index = -1; //Reset dialogue index

                CameraController.instance.LoadLastTarget();
                if (CameraController.instance.GetLastTarget() == PlayerController.instance.lookTransform)
                    CameraController.instance.SetRotation(false); //Disable forced rotation

                PlayerController.instance.SetState(PlayerController.States.idle); //Release player from interact state
                UIController.instance.ToggleDialogueInputIcon(false); //Force disable Input Icon

                m_OnTrigger.Invoke();
            }

            UIController.instance.ToggleLoreUI(interacting);
            index++;
        }
    }

    public void SetValue(List<string> textStrings, string title)
    {
        lines = textStrings;
        loreTitle = title;
    }

    //public override void StartInteract()
    //{
    //    UIController.instance.SetLoreText(loreText, loreTitle);
    //    UIController.instance.ToggleLoreUI(true);
    //    print("Collected " + gameObject.name);
    //}

    //public override void EndInteract()
    //{
    //    //SaveDataController.instance.SaveLoreData(loreId);
    //    UIController.instance.ToggleLoreUI(false);
    //    //SetHasActivated();
    //}
}
