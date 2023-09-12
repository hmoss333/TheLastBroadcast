using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatholeController : InteractObject
{
    public Transform exitPoint;
    [HideInInspector] public RoomController exitRoom;
    [SerializeField] float triggerTime = 0.5f;
    Coroutine holeRoutine = null;

    private void Awake()
    {
        if (exitPoint)
            exitRoom = exitPoint.GetComponentInParent<RoomController>();
    }

    public override void Interact()
    {
        if (holeRoutine == null)
            base.Interact();
    }

    public override void StartInteract()
    {
        holeRoutine = StartCoroutine(DoorTrigger());
    }

    IEnumerator DoorTrigger()
    {
        if (focusPoint != null)
        {
            CameraController.instance.SetTarget(focusPoint);
            CameraController.instance.SetRotation(true);
        }

        //GetComponent<AudioSource>().Play();

        yield return new WaitForSeconds(triggerTime);

        FadeController.instance.StartFade(1.0f, 1f);

        while (FadeController.instance.isFading)
            yield return null;

        UIController.instance.ToggleDialogueUI(false);

        RatController.instance.transform.position = exitPoint.position;
        RatController.instance.SetLastDir(exitPoint.transform.forward);

        CameraController.instance.transform.position = exitPoint.position;
        CameraController.instance.SetRotation(false);
        transform.GetComponentInParent<RoomController>().gameObject.SetActive(false);

        if (exitRoom)
            exitRoom.gameObject.SetActive(true);

        interacting = false;
        FadeController.instance.StartFade(0.0f, 1f);
        CameraController.instance.SetTarget(RatController.instance.transform);//PlayerController.instance.lookTransform);
        //CameraController.instance.SetLastTarget(PlayerController.instance.lookTransform);
        //PlayerController.instance.SetState(PlayerController.States.idle);

        holeRoutine = null;
    }
}
