using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorController : InteractObject
{
    [SerializeField] private bool changeScene;
    [SerializeField] private string sceneToLoad;


    [SerializeField] private int securityLevel;
    [SerializeField] private string lockedText = "Current security level is too low";
    [SerializeField] private MeshRenderer lightMesh;
    [SerializeField] Transform exitPoint;
    RoomController exitRoom;


    private void Awake()
    {
        if (exitPoint)
            exitRoom = exitPoint.GetComponentInParent<RoomController>();

        SetLightColor(active ? Color.green : Color.black);
    }

    public override void Activate()
    {
        SetLightColor(Color.green);

        base.Activate();
    }

    public override void Interact()
    {
        if (active)
        {
            base.Interact();

            if (SaveDataController.instance.GetSecurityCardLevel() >= securityLevel)
            {
                interacting = true; //force interacting state so player cannot exit animation prematurely
                SetLightColor(Color.green); //visually show that door can be opened
                StartCoroutine(DoorTrigger()); //Start door opening coroutine
            }
            else
            {
                SetLightColor(Color.red);
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

        if (!changeScene)
        {
            PlayerController.instance.transform.position = exitPoint.position;
            PlayerController.instance.SetLastDir(exitPoint.transform.forward);
            CameraController.instance.transform.position = exitPoint.position;
            transform.GetComponentInParent<RoomController>().gameObject.SetActive(false);

            if (exitRoom)
                exitRoom.gameObject.SetActive(true);

            interacting = false;
            FadeController.instance.StartFade(0.0f, 1f);
            PlayerController.instance.SetState(PlayerController.States.idle);
        }
        else
        {
            SaveDataController.instance.SetSavePoint(sceneToLoad, 0);
            SceneManager.LoadSceneAsync(sceneToLoad);
        }
    }

    public void SetLightColor(Color colorToSet)
    {
        if (lightMesh)
            lightMesh.material.color = colorToSet;
    }
}
