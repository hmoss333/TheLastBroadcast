using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
