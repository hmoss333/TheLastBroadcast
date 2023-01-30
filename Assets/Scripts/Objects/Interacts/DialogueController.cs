using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class DialogueController : InteractObject
{
    [SerializeField] private string[] lines;
    //[SerializeField] private TMP_Text textObj;
    [SerializeField] private int index;

    private void Update()
    {
        if (interacting)
        {
            if (!PlayerController.instance.inputMaster.Player.Interact.IsPressed())
            {
                interacting = false;
            }
        }
    }

    public override void Interact()
    {
        if (!interacting)
        {
            if (index < lines.Length)
            {
                interacting = true;
                Debug.Log(lines[index]);
                //textObj.text = lines[index];
            }
            else
            {
                hasActivated = true;
                interacting = false;
                SaveDataController.instance.SaveObjectData(SceneManager.GetActiveScene().name);
                SaveDataController.instance.SaveFile();
                PlayerController.instance.state = PlayerController.States.idle;
            }

            index++;
        }
    }

    public override void Activate()
    {
        active = true;
        index = 0;
    }
}
