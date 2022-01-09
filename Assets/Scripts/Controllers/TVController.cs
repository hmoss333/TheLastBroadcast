using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TVController : InteractObject
{
    [SerializeField] GameObject activeContainer;
    //[SerializeField] GameObject focusPoint;
    [SerializeField] AudioSource staticSource;
    [SerializeField] float audioDist;

    private void Update()
    {
        Vector3 playerPos = PlayerController.instance.gameObject.transform.position; //Get current player position
        staticSource.volume = (audioDist - Vector3.Distance(transform.position, playerPos))/audioDist; //scale volume based on how close the player is to the TV
        if (Vector3.Distance(transform.position, playerPos) < audioDist)
            staticSource.mute = !activated; //if TV is activated, unmute audio when in range
        else
            staticSource.mute = true; //If TV is not active, keep audio muted

        activeContainer.SetActive(activated); //Toggle container based on activation state
    }

    public override void Interact()
    {
        if (activated)
        {
            base.Interact();

            if (interacting)
                SaveGame();
        }
    }

    public override void Activate()
    {
        base.Activate();       
    }

    void SaveGame()
    {
        Debug.Log("Add save logic here");
    }
}
