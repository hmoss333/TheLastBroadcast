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
        if (other.tag == "Player")
        {
            PlayerController.instance.isListening = true;
            PlayerController.instance.state = PlayerController.States.interacting;
            dialogueObj.Interact();
        }
    }
}
