using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class RadioController : InteractObject
{
    [SerializeField] AudioSource staticSource;

    [SerializeField] TextMeshPro displayTextObj;
    [SerializeField] string[] displayText;
    [SerializeField] int textVal;


    void DisplayText()
    {
        displayTextObj.text = displayText[textVal];
        textVal++;

        if (textVal > displayText.Length)
            textVal = 0;
    }

    public override void Interact()
    {
        if (active && textVal < displayText.Length)
        {
            interacting = true;
            DisplayText();
        }
        else if (textVal >= displayText.Length)
        {
            textVal = 0;
            interacting = false;
        }

        CameraController.instance.SetTarget(interacting ? this.gameObject : PlayerController.instance.gameObject);
        PlayerController.instance.InteractToggle(interacting);
        displayTextObj.gameObject.SetActive(interacting);
        staticSource.mute = !interacting;
    }
}
