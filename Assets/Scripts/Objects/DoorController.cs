using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : InteractObject
{
    public Transform exitPoint;

    //public enum Direction { left, right, top, bottom }
    //public Direction direction;




    public override void Interact()
    {
        base.Interact();

        StartCoroutine(DoorTrigger());
    }

    IEnumerator DoorTrigger()
    {
        yield return new WaitForSeconds(0.5f);

        FadeController.instance.StartFade(1.0f, 1f);
        //load next room

        while (FadeController.instance.isFading)
            yield return null;

        //NavigationController.instance.SetActiveRoom(exitPoint.parent.parent.GetComponent<RoomController>()); //targetDoor.transform.parent.GetComponent<RoomController>());
        //NavigationController.instance.UpdateRooms();
        exitPoint.parent.parent.gameObject.SetActive(true);

        PlayerController.instance.gameObject.transform.position = exitPoint.position; //targetDoor.exitPoint.position;

        transform.parent.gameObject.SetActive(false);

        FadeController.instance.StartFade(0.0f, 1f);

        PlayerController.instance.interacting = false;
    }
}
