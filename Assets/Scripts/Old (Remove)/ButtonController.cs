using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : InteractObject
{
    [SerializeField] SaveObject[] objectsToActivate;
    [SerializeField] GameObject[] normalObjectsToActivate;
    [SerializeField] string triggerText;

    private void Start()
    {
        if (hasActivated)
        {
            for (int j = 0; j < normalObjectsToActivate.Length; j++)
            {
                normalObjectsToActivate[j].SetActive(!normalObjectsToActivate[j].activeSelf);
            }
        }
    }

    public override void Interact()
    {
        if (active && !hasActivated)
            base.Interact();
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
        CameraController.instance.SetTarget(PlayerController.instance.gameObject);
        SetHasActivated();
    }

    IEnumerator ActivateObjects()
    {
        //Activate all interact objects in list
        for (int i = 0; i < objectsToActivate.Length; i++)
        {
            objectsToActivate[i].Activate();

            yield return new WaitForSeconds(0.5f);
        }

        for (int j = 0; j < normalObjectsToActivate.Length; j++)
        {
            normalObjectsToActivate[j].SetActive(!normalObjectsToActivate[j].activeSelf);
        }

        //if (CameraController.instance.GetLastTarget() != null)
        //    CameraController.instance.LoadLastTarget();
        //else
            CameraController.instance.SetTarget(PlayerController.instance.gameObject);
    }
}
