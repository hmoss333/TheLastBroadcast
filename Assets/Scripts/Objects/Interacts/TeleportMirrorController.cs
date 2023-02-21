using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportMirrorController : InteractObject
{
    [SerializeField] Transform exitPoint;
    private RoomController exitRoom;

    private void Awake()
    {
        if (exitPoint)
            exitRoom = exitPoint.GetComponentInParent<RoomController>();
    }

    public override void Interact()
    {
        if (SaveDataController.instance.saveData.abilities.mirror)
        {
            base.Interact();

            StartCoroutine(TeleportTrigger());
        }
        else
        {
            print("It's just a mirror...");
        }
    }

    IEnumerator TeleportTrigger()
    {
        yield return new WaitForSeconds(0.5f);

        FadeController.instance.StartFade(1.0f, 1f);

        while (FadeController.instance.isFading)
            yield return null;

        //if (exitRoom)
        //    exitRoom.gameObject.SetActive(true);

        PlayerController.instance.transform.position = exitPoint.position;
        PlayerController.instance.SetLastDir(exitPoint.transform.forward);
        transform.GetComponentInParent<RoomController>().gameObject.SetActive(false);

        if (exitRoom)
            exitRoom.gameObject.SetActive(true);

        FadeController.instance.StartFade(0.0f, 1f);

        PlayerController.instance.state = PlayerController.States.idle;
    }
}
