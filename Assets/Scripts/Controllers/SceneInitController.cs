using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneInitController : MonoBehaviour
{
    public static SceneInitController instance;

    [SerializeField] string currentScene;
    SaveData saveData;
    [SerializeField] SavePointController[] savePoints;
    [SerializeField] RoomController[] rooms;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        saveData = SaveDataController.instance.GetSaveData();

        InitializeGame();
    }

    private void Update()
    {
        if (currentScene != SceneManager.GetActiveScene().name)
            InitializeGame();
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

        //Whenever a new scene is loaded, update save file to reflect the current level
        if (currentScene != saveData.currentScene && currentScene != "Title")
        {
            SaveDataController.instance.SetSavePoint(currentScene, 0);
            SaveDataController.instance.SaveFile();
        }


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

        if (CameraController.instance)
            CameraController.instance.SetTarget(PlayerController.instance.gameObject);
        FadeController.instance.StartFade(0.0f, 1f);
    }
}
