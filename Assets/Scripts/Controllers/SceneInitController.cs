using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class SceneInitController : MonoBehaviour
{
    public static SceneInitController instance;

    public string currentScene;
    SaveData saveData;
    [SerializeField] SavePointController[] savePoints;
    [SerializeField] RoomController[] rooms;

    [SerializeField] SceneObjectsContainer currentScenario;
    [SerializeField] List<SceneInteractObj> currentInteractObjects;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        saveData = SaveDataController.instance.GetSaveData();
        currentInteractObjects = new List<SceneInteractObj>();
        //SaveDataController.instance.LoadObjectData(currentScene);
        InitializeGame();
    }

    private void Update()
    {
        if (currentScene != SceneManager.GetActiveScene().name)
            InitializeGame();
    }

    void GetInteractObjs()
    {      
        InteractObject[] tempObjArray = (InteractObject[])FindObjectsOfType(typeof(InteractObject), true);
        foreach (InteractObject tempObj in tempObjArray)
        {
            foreach (SceneInteractObj tempSceneObj in currentScenario.sceneObjects)
            {
                if (tempObj.name == tempSceneObj.name)
                {
                    tempObj.active = tempSceneObj.active;
                    tempObj.hasActivated = tempSceneObj.hasActivated;
                }
            }
        }

        //Create list of all interactObjects in scene
        currentInteractObjects = new List<SceneInteractObj>();
        foreach (InteractObject tempObj in tempObjArray)
        {
            SceneInteractObj newObj = new SceneInteractObj();
            newObj.name = tempObj.gameObject.name;
            //newObj.ID = tempObj.objID;
            newObj.active = tempObj.active;
            newObj.hasActivated = tempObj.hasActivated;
            //TODO add more values to track here

            currentInteractObjects.Add(newObj);
        }

        //currentInteractObjects = currentInteractObjects.OrderBy(x => x.ID).ToList(); //sort objects by ID value
    }

    //public void SaveInteractObjs() //call whenever changing FROM scene
    //{
    //    InteractObject[] tempObjArray = FindObjectsOfType<InteractObject>();
    //    foreach (SceneInteractObj tempSceneObj in currentInteractObjects)
    //    {
    //        foreach (InteractObject tempObj in tempObjArray)
    //        {
    //            if (tempSceneObj.ID == tempObj.objID)
    //            {
    //                tempSceneObj.name = tempObj.gameObject.name;
    //                tempSceneObj.active = tempObj.active;
    //                tempSceneObj.hasActivated = tempObj.hasActivated;
    //            }
    //        }
    //    }

    //    //Update save file with current changes
    //    //currentScenario.objectStates = currentInteractObjects;
    //    SaveDataController.instance.SaveFile();
    //}

    void GetAllSavePoints()
    {
        SavePointController[] tempArray = FindObjectsOfType<SavePointController>();
        savePoints = tempArray;
    }

    void HideAllRooms()
    {
        RoomController[] tempRooms = FindObjectsOfType<RoomController>();
        rooms = tempRooms;

        foreach (RoomController room in rooms)
        {
            room.gameObject.SetActive(false);
        }
    }

    public void InitializeGame()
    {
        currentScene = SceneManager.GetActiveScene().name;
        currentScenario = SaveDataController.instance.sceneObjectContainer;

        GetInteractObjs();
        GetAllSavePoints();
        HideAllRooms();

        foreach (SavePointController point in savePoints)
        {
            if (point.ID == saveData.savePointID)
            {
                if (!point.transform.parent.gameObject.activeSelf)
                    point.transform.parent.gameObject.SetActive(true);
                PlayerController.instance.transform.position = point.initPoint.position;
                PlayerController.instance.transform.rotation = point.initPoint.rotation;
                PlayerController.instance.interacting = false;
                break;
            }
        }

        CameraController.instance.SetTarget(PlayerController.instance.gameObject);
        FadeController.instance.StartFade(0.0f, 1f);
    }
}
