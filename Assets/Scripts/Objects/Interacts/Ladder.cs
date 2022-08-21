using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

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

    private void Update()
    {
        if (!interacting && !onLadder)
        {
            active = true;
        }
    }

    private void FixedUpdate()
    {
        if (interacting && onLadder)
        {
            vertical = Input.GetAxisRaw("Vertical");

            Vector3 tempMove = new Vector3(0f, vertical, 0f);
            playerRb.velocity = new Vector3(0f, tempMove.y * climbSpeed, 0f);

            if (Input.GetButtonDown("Interact"))
            {
                Interact();
                active = false;
            }
        }

        PlayerController.instance.onLadder = onLadder;
    }

    public override void Interact()
    {
        interacting = !interacting;
        onLadder = interacting;
        PlayerController.instance.interacting = onLadder;
        PlayerController.instance.transform.position = new Vector3(transform.position.x, PlayerController.instance.transform.position.y, transform.position.z);
        PlayerController.instance.transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles);
        playerRb.useGravity = !interacting;
        active = !active;
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
