using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : InteractObject
{
    //[SerializeField] bool isActivated;
    PlayerController player;
    [SerializeField] InteractObject[] objectsToActivate;
    [SerializeField] string triggerText;
    float delayTime;


    void Start()
    {
        player = FindObjectOfType<PlayerController>();

        gameObject.SetActive(!hasActivated);
    }

    private void Update()
    {
        if (interacting)
        {
            delayTime -= Time.deltaTime;
            if (delayTime <= 0f)
            {
                print("Delay complete");
                interacting = false;
            }
        }
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
        //Set player state and display UI overlay
        PlayerController.instance.state = PlayerController.States.interacting;
        UIController.instance.SetDialogueText(triggerText);
        UIController.instance.ToggleDialogueUI(true);

        yield return new WaitForSeconds(0.65f); //brief pause for cinematic effect

        delayTime = PlayerController.instance.GetClipLength("Interact");
        interacting = true;

        //Activate all interact objects in list
        for (int i = 0; i < objectsToActivate.Length; i++)
        {
            if (!objectsToActivate[i].active)
            {
                if (objectsToActivate[i].transform.parent.gameObject.activeSelf)
                {
                    CameraController.instance.SetTarget(objectsToActivate[i].gameObject);

                    yield return new WaitForSeconds(1f);
                }

                objectsToActivate[i].Activate();

                yield return new WaitForSeconds(0.65f);
            }
        }

        //Reset camera to player
        CameraController.instance.SetTarget(player.gameObject);

        while (interacting)
            yield return null;

        PlayerController.instance.state = PlayerController.States.idle;
        UIController.instance.ToggleDialogueUI(false);
    }
}
