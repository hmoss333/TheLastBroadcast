using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class ViewInteractObject : InteractObject
{
    [NaughtyAttributes.HorizontalLine]

    [SerializeField] Camera viewCam;
    [SerializeField] Camera mainCam;
    bool viewing;

    RoomController viewRoom;
    RoomController currentRoom;

    Coroutine vrRoutine;

    private void Awake()
    {
        //mainCam = CameraController.instance.GetComponent<Camera>();
        viewRoom = viewCam.gameObject.GetComponentInParent<RoomController>();
        currentRoom = GetComponentInParent<RoomController>();

        viewCam.enabled = false;
        vrRoutine = null;
    }


    public override void Interact()
    {
        if (active && vrRoutine == null)
        {
            interacting = !interacting;

            if (!viewing)
                StartInteract();
            else
                EndInteract();
        }
    }


    public override void StartInteract()
    {
        base.StartInteract();
        PlayerController.instance.animator.SetBool("isInspecting", true);

        //UIController.instance.SetDialogueText(string.Empty, false);
        //UIController.instance.ToggleDialogueUI(true);

        if (vrRoutine == null)
            vrRoutine = StartCoroutine(ViewRoom());
    }

    public override void EndInteract()
    {
        base.EndInteract();
        PlayerController.instance.animator.SetBool("isInspecting", false);
        m_OnTrigger.Invoke();

        //UIController.instance.ToggleDialogueUI(false);

        if (vrRoutine == null)
            vrRoutine = StartCoroutine(ViewRoom());
    }

    IEnumerator ViewRoom()
    {
        FadeController.instance.StartFade(1f, viewing ? 0.75f : 2f);

        while (FadeController.instance.isFading)
            yield return null;

        mainCam.enabled = !mainCam.enabled;
        viewCam.enabled = !viewCam.enabled;

        if (!viewRoom.gameObject.activeSelf)
            viewRoom.gameObject.SetActive(true);
        else
        {
            if (viewRoom != currentRoom)
                viewRoom.gameObject.SetActive(false);
        }

        FadeController.instance.StartFade(0f, 0.25f);

        while (FadeController.instance.isFading)
            yield return null;

        viewing = !viewing;
        vrRoutine = null;
    }
}
