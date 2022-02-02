using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : InteractObject
{
    public Transform exitPoint;


    public override void Interact()
    {
        if (activated)
        {
            base.Interact();

            StartCoroutine(DoorTrigger());
        }
    }

    IEnumerator DoorTrigger()
    {
        yield return new WaitForSeconds(0.5f);

        FadeController.instance.StartFade(1.0f, 1f);

        while (FadeController.instance.isFading)
            yield return null;

        exitPoint.parent.parent.gameObject.SetActive(true); //gross...

        PlayerController.instance.gameObject.transform.position = exitPoint.position;

        transform.parent.gameObject.SetActive(false);

        FadeController.instance.StartFade(0.0f, 1f);

        PlayerController.instance.InteractToggle(false);
    }
}
