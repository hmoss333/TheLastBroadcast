using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : InteractObject
{
    PlayerController player;
    [SerializeField] GameObject[] objectsToActivate;
    [SerializeField] string triggerText;


    void Start()
    {
        player = FindObjectOfType<PlayerController>();

        gameObject.SetActive(!hasActivated);
    }

    public override void StartInteract()
    {
        UIController.instance.SetDialogueText(triggerText);
        UIController.instance.ToggleDialogueUI(true);
        StartCoroutine(ActivateObjects());
    }

    public override void EndInteract()
    {
        UIController.instance.ToggleDialogueUI(false);
        CameraController.instance.SetTarget(player.gameObject);
    }

    IEnumerator ActivateObjects()
    {
        yield return new WaitForSeconds(0.65f); //brief pause for cinematic effect

        //Activate all interact objects in list
        for (int i = 0; i < objectsToActivate.Length; i++)
        {
            if (objectsToActivate[i].GetComponent<InteractObject>())
            {
                if (!objectsToActivate[i].GetComponent<InteractObject>().active)
                {
                    if (objectsToActivate[i].transform.parent.gameObject.activeSelf)
                    {
                        CameraController.instance.SetTarget(objectsToActivate[i].gameObject);

                        yield return new WaitForSeconds(1f);
                    }

                    objectsToActivate[i].GetComponent<InteractObject>().Activate();

                    yield return new WaitForSeconds(0.65f);
                }
            }
            else
            {
                if (objectsToActivate[i].transform.parent.gameObject.activeSelf)
                {
                    CameraController.instance.SetTarget(objectsToActivate[i].gameObject);

                    yield return new WaitForSeconds(1f);
                }

                objectsToActivate[i].SetActive(!objectsToActivate[i].activeSelf);

                yield return new WaitForSeconds(0.65f);
            }
        }

        hasActivated = true;
    }
}
