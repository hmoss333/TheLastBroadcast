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

    private void Update()
    {
        if (interacting)
        {
            if (Input.GetKeyDown("left"))
            {
                sceneVal--;
                CycleSceneName();
            }
            else if (Input.GetKeyDown("right"))
            {
                sceneVal++;
                CycleSceneName();
            }
            else if (Input.GetKeyDown("up") && sceneToLoad != SaveDataController.instance.saveData.currentScene)
            {
                PlayerController.instance.ToggleAvatar();
                SceneManager.LoadScene(sceneToLoad);
            }
        }
    }


    void CycleSceneName()
    {
        List<string> sceneNames = new List<string>();
        for (int i = 0; i < SaveDataController.instance.saveData.scenarios.Count; i++)
        {
            sceneNames.Add(SaveDataController.instance.saveData.scenarios[i].sceneName);
        }

        if (sceneVal > sceneNames.Count - 1)
            sceneVal = 0;
        else if (sceneVal < 0)
            sceneVal = sceneNames.Count - 1;
        sceneToLoad = sceneNames[sceneVal];
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
