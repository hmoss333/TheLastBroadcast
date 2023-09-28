using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TriggerRoomChange : DoorController
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Interact();
        }
    }
}
