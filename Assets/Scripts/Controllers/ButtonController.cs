using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : InteractObject
{
    //[SerializeField] bool isActivated;
    PlayerController player;
    [SerializeField] InteractObject[] objectsToActivate;


    void Start()
    {
        player = FindObjectOfType<PlayerController>();
    }

    public override void Interact()
    {
        if (!hasActivated)
        {
            hasActivated = true;         

            StartCoroutine(ActivateObjects());
        }
    }

    IEnumerator ActivateObjects()
    {
        //Pause user input for brief moment
        PlayerController.instance.interacting = true;
        yield return new WaitForSeconds(0.65f);
        PlayerController.instance.interacting = false;


        //Activate all interact objects in list
        for (int i = 0; i < objectsToActivate.Length; i++)
        {
            if (!objectsToActivate[i].active)
            {
                CameraController.instance.SetTarget(objectsToActivate[i].gameObject);

                yield return new WaitForSeconds(1f);

                objectsToActivate[i].Activate();


                yield return new WaitForSeconds(1f);
            }
        }

        //Reset camera to player
        CameraController.instance.SetTarget(player.gameObject);
    }
}
