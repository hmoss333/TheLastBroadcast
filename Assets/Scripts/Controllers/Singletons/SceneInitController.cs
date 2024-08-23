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
    public RoomController currentRoom { get; private set; }
    [SerializeField] private float fadeInTime = 1.5f;
    [SerializeField] private List<string> scenesToIgnore;
    [SerializeField] private SceneObjectsContainer currentScenario;



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
        InitializeGame();
    }

    private void Update()
    {
        //Reload scene on Player death
        if (PlayerController.instance.dead)
        {
            if (!FadeController.instance.isFading)
                FadeController.instance.StartFade(1.0f, fadeTime - 0.25f);

            fadeTime -= Time.deltaTime;
            if (fadeTime < 0)
            {
                SceneManager.LoadScene(currentScene);
            }
        }
    }

    void GetAllSavePoints()
    {
        SavePointController[] tempArray = FindObjectsOfType<SavePointController>();
        savePoints = tempArray;
    }

    public List<string> GetScenesToIgnore()
    {
        return scenesToIgnore;
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

    public void SetCurrentRoom(RoomController room)
    {
        currentRoom = room;
        Debug.Log($"Current room: {currentRoom.gameObject.name}");
    }

    public void InitializeGame()
    {
        currentScene = SceneManager.GetActiveScene().name;
        if (!scenesToIgnore.Contains(currentScene)) { SaveDataController.instance.SetSavePoint(currentScene); }
        SaveDataController.instance.LoadObjectData(currentScene);
        currentScenario = SaveDataController.instance.sceneObjectContainer;

        Resources.UnloadUnusedAssets();

        GetAllSavePoints();
        HideAllRooms();

        foreach (SavePointController point in savePoints)
        {
            if (point.ID == currentScenario.savePointID)
            {
                if (!point.transform.parent.gameObject.activeSelf)
                {
                    RoomController initRoom = point.transform.parent.GetComponent<RoomController>();
                    initRoom.gameObject.SetActive(true);
                    SetCurrentRoom(initRoom);
                }
                PlayerController.instance.transform.position = point.initPoint.position;
                PlayerController.instance.transform.rotation = point.initPoint.rotation;
                break;
            }
        }

        if (CameraController.instance != null && CameraController.instance.GetTarget() == null)
        {
            CameraController.instance.SetTarget(PlayerController.instance.lookTransform);
            if (CameraController.instance.GetLastTarget() == null)
                CameraController.instance.SetLastTarget(PlayerController.instance.lookTransform);
        }

        StartCoroutine(LateStart());
    }

    IEnumerator LateStart()
    {
        yield return new WaitForSeconds(0.01f);

        CameraController.instance.transform.position = PlayerController.instance.gameObject.transform.position;

        FadeController.instance.StartFade(0.0f, fadeInTime);
    }
}
