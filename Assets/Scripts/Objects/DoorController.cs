using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorController : InteractObject
{
    public Transform exitPoint;

    public override void Interact()
    {
        if (active)
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

        PlayerController.instance.transform.position = exitPoint.position;
        //PlayerController.instance.SetlastDir(exitPoint.position);

        transform.parent.gameObject.SetActive(false);

        FadeController.instance.StartFade(0.0f, 1f);

        PlayerController.instance.InteractToggle(false);
    }
}
