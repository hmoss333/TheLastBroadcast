using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SavePointController : InteractObject
{
    public int ID;
    public Transform initPoint;
    [SerializeField] GameObject focusPoint;


    public override void Interact()
    {
        base.Interact();

        PlayerController.instance.ToggleAvatar();
        CameraController.instance.SetTarget(interacting ? focusPoint : PlayerController.instance.gameObject);
        CameraController.instance.FocusTarget();

        if (interacting)
        {
            Debug.Log("Saving game");
            SaveDataController.instance.SetSavePoint(SceneManager.GetActiveScene().name, ID);
            SaveDataController.instance.SaveFile();
        }
    }
}
