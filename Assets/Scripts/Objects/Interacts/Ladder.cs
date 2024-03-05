using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class Ladder : InteractObject
{
    float vertical;
    [SerializeField] float climbSpeed;
    bool onLadder;
    Rigidbody playerRb;


    private void Start()
    {
        playerRb = PlayerController.instance.GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (interacting && onLadder)
        {
            vertical = InputController.instance.inputMaster.Player.Move.ReadValue<Vector2>().y;

            Vector3 tempMove = new Vector3(0f, vertical, 0f);
            playerRb.velocity = new Vector3(0f, tempMove.y * climbSpeed, 0f);

            if (InputController.instance.inputMaster.Player.Interact.IsPressed() && !active)
                Interact();
        }

        //PlayerController.instance.onLadder = onLadder;
    }

    public override void Interact()
    {
        InputController.instance.inputMaster.Disable();
        interacting = !interacting;
        onLadder = interacting;
        PlayerController.instance.SetState(onLadder ? PlayerController.States.interacting : PlayerController.States.moving); //This may not work properly. Needs review
        PlayerController.instance.transform.position = new Vector3(transform.position.x, PlayerController.instance.transform.position.y, transform.position.z);
        PlayerController.instance.transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles);
        playerRb.useGravity = !interacting;
        active = !active;
        InputController.instance.inputMaster.Enable();
    }

    private void OnTriggerExit(Collider other)
    {
        //Only trigger if the player is on the ladder and holding up
        if (other.tag == "Player" && onLadder && vertical > 0)
        {
            Interact();
        }
    }
}
