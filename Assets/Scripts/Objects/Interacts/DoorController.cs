using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


[RequireComponent(typeof(AudioSource))]
public class DoorController : InteractObject
{
    public Transform exitPoint;
    [HideInInspector] public RoomController exitRoom;
    [SerializeField] float triggerDelay = 0.5f, triggerTime = 1f;
    [SerializeField] private AudioClip useClip, unlockClip;
    private AudioSource audioSource;
    Coroutine doorRoutine;


    private void Awake()
    {
        if (exitPoint) { exitRoom = exitPoint.GetComponentInParent<RoomController>(); }
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public override void Activate()
    {
        base.Activate();

        if (active)
        {
            audioSource.Stop();
            audioSource.clip = unlockClip;
            audioSource.Play();
        }
    }

    public override void Interact()
    {
        if (doorRoutine == null)
            base.Interact();
    }

    public override void StartInteract()
    {
        doorRoutine = StartCoroutine(DoorTrigger());
    }

    IEnumerator DoorTrigger()
    {
        if (focusPoint != null)
        {
            CameraController.instance.SetTarget(focusPoint);
            CameraController.instance.SetRotation(true);
        }

        audioSource.Stop();
        audioSource.clip = useClip;
        audioSource.Play();

        yield return new WaitForSeconds(triggerDelay);

        FadeController.instance.StartFade(1.0f, triggerTime);

        while (FadeController.instance.isFading)
            yield return null;

        m_OnTrigger.Invoke();

        UIController.instance.ToggleDialogueUI(false);
        PlayerController.instance.transform.position = exitPoint.position;
        PlayerController.instance.SetLastDir(exitPoint.transform.forward);
        CameraController.instance.transform.position = exitPoint.position;
        CameraController.instance.SetRotation(false);
        CameraController.instance.SetTriggerState(false);
        transform.GetComponentInParent<RoomController>().gameObject.SetActive(false);

        if (exitRoom)
        {
            exitRoom.gameObject.SetActive(true);
            SceneInitController.instance.SetCurrentRoom(exitRoom);
        }

        interacting = false;
        FadeController.instance.StartFade(0.0f, triggerTime);
        CameraController.instance.SetTarget(PlayerController.instance.lookTransform);
        CameraController.instance.SetLastTarget(PlayerController.instance.lookTransform);
        PlayerController.instance.SetState(PlayerController.States.idle);

        doorRoutine = null;
    }
}
