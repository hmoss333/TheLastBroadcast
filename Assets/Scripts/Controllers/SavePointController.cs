using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SavePointController : InteractObject
{
    public int ID;
    public Transform initPoint;
    [SerializeField] GameObject focusPoint;
    //[SerializeField] TextMeshPro saveText;
    [SerializeField] int sceneVal;
    [SerializeField] string sceneToLoad;

    //TODO add logic here to allow the player to select if they would like to return to the Facility
    private void Start()
    {
        sceneToLoad = SaveDataController.instance.saveData.currentScene;
    }

    public override void Interact()
    {
        base.Interact();

        PlayerController.instance.ToggleAvatar();
        CameraController.instance.SetTarget(interacting ? focusPoint : PlayerController.instance.gameObject);
        CameraController.instance.FocusTarget();

        if (interacting)
        {
            Debug.Log("Saving game");
            SceneInitController.instance.SaveInteractObjs();
            SaveDataController.instance.SetSavePoint(SceneManager.GetActiveScene().name, ID);
            SaveDataController.instance.SaveFile();
        }
    }
}
