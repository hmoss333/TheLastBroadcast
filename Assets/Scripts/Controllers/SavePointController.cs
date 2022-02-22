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
                sceneToLoad = NextSceneName(-1);
            }
            else if (Input.GetKeyDown("right"))
            {
                sceneToLoad = NextSceneName(1);
            }
            //else if (Input.GetKeyDown("space"))
            //{
            //    if (sceneToLoad == SaveDataController.instance.saveData.currentScene)
            //        SaveGame();
            //    else
            //        SceneManager.LoadScene(sceneToLoad);
            //}
        }
    }


    string NextSceneName(int incrementNum)
    {
        int levelNum = 0;
        string returnLevelName = "";
        for (int i = 0; i < SaveDataController.instance.saveData.scenarios.Count; i++)
        {
            if (SaveDataController.instance.saveData.scenarios[i].sceneName == SaveDataController.instance.saveData.currentScene)
            {
                levelNum = i + incrementNum;
                returnLevelName = SaveDataController.instance.saveData.scenarios[levelNum].sceneName;
                break;
            }
        }

        return returnLevelName;
    }

    void SaveGame()
    {
        Debug.Log("Saving game");
        SceneInitController.instance.SaveInteractObjs();
        SaveDataController.instance.SetSavePoint(SceneManager.GetActiveScene().name, ID);
        SaveDataController.instance.SaveFile();
    }

    public override void Interact()
    {
        base.Interact();

        PlayerController.instance.ToggleAvatar();
        CameraController.instance.SetTarget(interacting ? focusPoint : PlayerController.instance.gameObject);
        CameraController.instance.FocusTarget();

        //if (interacting)
        //{
        //Debug.Log("Saving game");
        //SceneInitController.instance.SaveInteractObjs();
        //SaveDataController.instance.SetSavePoint(SceneManager.GetActiveScene().name, ID);
        //SaveDataController.instance.SaveFile();
        //{

        if (Input.GetKeyDown("space"))
        {
            if (sceneToLoad == SaveDataController.instance.saveData.currentScene)
                SaveGame();
            else
            {
                SceneManager.LoadScene(sceneToLoad);
                PlayerController.instance.ToggleAvatar();
            }
        }
    }
}
