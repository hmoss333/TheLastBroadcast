using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneInitController : MonoBehaviour
{
    public static SceneInitController instance;

    public string currentScene;
    SaveData saveData;
    [SerializeField] SavePointController[] savePoints;
    [SerializeField] RoomController[] rooms;

    ScenarioObjective currentScenario;
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

        InitializeGame();
    }

    private void Update()
    {
        if (currentScene != SceneManager.GetActiveScene().name)
            InitializeGame();
    }

    //void GetCurrentScenario()
    //{
    //    //Get reference to current Scenario
    //    foreach (ScenarioObjective scenarioObjective in saveData.scenarios)
    //    {
    //        if (scenarioObjective.sceneName == currentScene)
    //        {
    //            currentScenario = scenarioObjective;
    //            break;
    //        }
    //    }
    //}

    void GetInteractObjs()
    {      
        InteractObject[] tempObjArray = FindObjectsOfType<InteractObject>();
        foreach (InteractObject tempObj in tempObjArray)
        {
            foreach (SceneInteractObj tempSceneObj in currentScenario.objectStates)
            {
                if (tempObj.objID == tempSceneObj.ID)
                {
                    tempObj.activated = tempSceneObj.activated;
                }
            }
        }

        //Create list of all interactObjects in scene
        currentInteractObjects = new List<SceneInteractObj>();
        foreach (InteractObject tempObj in tempObjArray)
        {
            SceneInteractObj newObj = new SceneInteractObj();
            newObj.ID = tempObj.objID;
            newObj.activated = tempObj.activated;
            //TODO add more values to track here

            currentInteractObjects.Add(newObj);
        }     
    }

    public void SaveInteractObjs() //call whenever changing FROM scene
    {
        InteractObject[] tempObjArray = FindObjectsOfType<InteractObject>();
        foreach (SceneInteractObj tempSceneObj in currentInteractObjects)
        {
            foreach (InteractObject tempObj in tempObjArray)
            {
                if (tempSceneObj.ID == tempObj.objID)
                {
                    tempSceneObj.activated = tempObj.activated;
                }
            }
        }

        //Update save file with current changes
        currentScenario.objectStates = currentInteractObjects;
        SaveDataController.instance.SaveFile();
    }

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
        currentScenario = SaveDataController.instance.GetScenario(currentScene);

        GetInteractObjs();
        if (currentScenario.objectStates.Count <= 0) { SaveInteractObjs(); }
        GetAllSavePoints();
        HideAllRooms();

        foreach (SavePointController point in savePoints)
        {
            if (point.ID == currentScenario.savePointID)
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
