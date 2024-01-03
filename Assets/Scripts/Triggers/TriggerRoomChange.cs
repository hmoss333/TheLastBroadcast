using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TriggerRoomChange : DoorController
{
    [SerializeField] string animationTrigger;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (animationTrigger != string.Empty)
            {
                other.GetComponentInChildren<Animator>().SetTrigger(animationTrigger);
                PlayerController.instance.SetState(PlayerController.States.interacting);
            }

            Interact();
        }
    }
}
