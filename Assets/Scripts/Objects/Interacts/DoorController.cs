using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorController : InteractObject
{
    [SerializeField] private int securityLevel;
    [SerializeField] private string lockedText = "Current security level is too low";
    [SerializeField] Transform exitPoint;
    RoomController exitRoom;


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

            if (SaveDataController.instance.GetSecurityCardLevel() >= securityLevel)
            {
                StartCoroutine(DoorTrigger());
            }
            else
            {
                UIController.instance.SetDialogueText(lockedText);
                UIController.instance.ToggleDialogueUI(interacting);
            }
        }
    }

    IEnumerator DoorTrigger()
    {
        yield return new WaitForSeconds(0.5f);

        FadeController.instance.StartFade(1.0f, 1f);

        while (FadeController.instance.isFading)
            yield return null;

        PlayerController.instance.transform.position = exitPoint.position;
        PlayerController.instance.SetLastDir(exitPoint.transform.forward);
        CameraController.instance.transform.position = exitPoint.position;
        transform.GetComponentInParent<RoomController>().gameObject.SetActive(false);

        if (exitRoom)
            exitRoom.gameObject.SetActive(true);

        FadeController.instance.StartFade(0.0f, 1f);

        PlayerController.instance.SetState(PlayerController.States.idle);
    }
}
