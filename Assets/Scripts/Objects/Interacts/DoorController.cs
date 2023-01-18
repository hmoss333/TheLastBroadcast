using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorController : InteractObject
{
    //public bool locked;
    public Transform exitPoint;
    [SerializeField] RoomController exitRoom;


    private void Awake()
    {
        if (exitPoint)
            exitRoom = exitPoint.GetComponentInParent<RoomController>();
    }

    public override void Interact()
    {
        if (active)
        {
            base.Interact();

            StartCoroutine(DoorTrigger());
        }
        else
        {
            print("Door is locked");
        }
    }

    IEnumerator DoorTrigger()
    {
        yield return new WaitForSeconds(0.5f);

        FadeController.instance.StartFade(1.0f, 1f);

        while (FadeController.instance.isFading)
            yield return null;

        if (exitRoom)
            exitRoom.gameObject.SetActive(true);

        PlayerController.instance.transform.position = exitPoint.position;
        PlayerController.instance.SetLastDir(exitPoint.transform.forward);

        transform.GetComponentInParent<RoomController>().gameObject.SetActive(false);

        FadeController.instance.StartFade(0.0f, 1f);

        PlayerController.instance.InteractToggle(false);
    }
}
