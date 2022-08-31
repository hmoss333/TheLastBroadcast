using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class SceneInitController : MonoBehaviour
{
    public static SceneInitController instance;


    public string currentScene { get; private set; }
    private SaveData saveData;
    [SerializeField] private SavePointController[] savePoints;
    [SerializeField] private RoomController[] rooms;

    [SerializeField] private SceneObjectsContainer currentScenario;
    [SerializeField] private List<SceneInteractObj> currentInteractObjects;


    float fadeTime = 3f;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        saveData = SaveDataController.instance.GetSaveData();
        currentInteractObjects = new List<SceneInteractObj>();
        currentScene = SceneManager.GetActiveScene().name;
        InitializeGame();
    }

    private void Update()
    {
        //Initialize game on scene change
        if (currentScene != SceneManager.GetActiveScene().name)
        {
            InitializeGame();
        }

        //Reload scene on Player death
        if (PlayerController.instance.dead)
        {
            FadeController.instance.StartFade(100f, 250f);

            fadeTime -= Time.deltaTime;
            if (fadeTime < 0)
            {
                SceneManager.LoadScene(currentScene);
            }
        }
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
        SaveDataController.instance.LoadObjectData(currentScene);
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
        if (CameraController.instance.GetLastTarget() == null)
            CameraController.instance.SetLastTarget(PlayerController.instance.gameObject);
        FadeController.instance.StartFade(0.0f, 5f);
    }
}
