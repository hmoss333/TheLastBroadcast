using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorButtonController : InteractObject
{
    [SerializeField] ElevatorController elevatorController;
    //[SerializeField] int ID;
    [SerializeField] bool moveDown;

    public override void Interact()
    {
        //print("Interacting with elevator button");
        elevatorController.CallElevator(moveDown);
        //PlayerController.instance.InteractToggle(false);
    }
}
