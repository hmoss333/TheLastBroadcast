using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.IO;
using static UnityEditor.PlayerSettings;
using static UnityEngine.GraphicsBuffer;
//using UnityEngine.UIElements;


public class MainMenuController : MonoBehaviour
{
    [SerializeField] MainMenuElement[] menuElements;
    [SerializeField] MainMenuElement currentElement;
    [SerializeField] float camSpeed;
    private int index;
    InputMaster inputMaster;
    [SerializeField] GameObject loadGameCanvas;
    [SerializeField] Button loadGameButton;
    [SerializeField] TextMeshProUGUI loadGameText, versionText;
    [SerializeField] SpriteRenderer radioLight;
    [SerializeField] AudioSource backgroundAudio;
    [SerializeField] float maxAudioVolume;
    Color defaultColor, fadeColor;
    bool loadingScene;
    string sceneToLoad;

    EventSystem eventSystem;


    private void Start()
    {
        eventSystem = FindObjectOfType<EventSystem>();
        inputMaster = new InputMaster();
        inputMaster.Enable();

        loadGameCanvas.SetActive(false);
        defaultColor = radioLight.color;
        fadeColor = new Color(defaultColor.r, defaultColor.g, defaultColor.b, 0);
        index = 0;
        currentElement = menuElements[index];
        //menuButtons[index].Select();

        versionText.text = $"Version: {Application.version}";

        //StartCoroutine(FadeAudio(backgroundAudio, maxAudioVolume, 10f));
    }

    private void Update()
    {
        radioLight.color = Color.Lerp(defaultColor, fadeColor, Mathf.PingPong(Time.time, 1));
        backgroundAudio.volume = Mathf.Lerp(0f, maxAudioVolume, Time.time / 10f);

        //Loading saved game
        if (loadingScene)
        {
            loadGameCanvas.SetActive(false);

            if (!FadeController.instance.isFading)
            {
                print("Change scene");
                SceneManager.LoadScene(sceneToLoad);
            }
        }
        //Menu controls
        else
        {
            if (loadGameCanvas.activeSelf)
            {
                if (inputMaster.Player.Interact.triggered)
                {
                    loadGameButton.onClick.Invoke();
                }
                else if (inputMaster.Player.Melee.triggered)
                {
                    loadGameCanvas.SetActive(false);
                }
            }
            else
            {
                float inputVal = inputMaster.Player.Move.ReadValue<Vector2>().y;
                bool inputCheck = inputMaster.Player.Move.triggered;
                if (inputVal > 0 && inputCheck)
                {
                    index--;
                    if (index < 0)
                        index = menuElements.Length - 1;

                    print(menuElements[index].button.name);
                }
                else if (inputVal < 0 && inputCheck)
                {
                    index++;
                    if (index > menuElements.Length - 1)
                        index = 0;

                    print(menuElements[index].button.name);
                }

                if (inputMaster.Player.Interact.triggered)
                {
                    currentElement.onClick();
                }
            }
        }
    }

    private void FixedUpdate()
    {
        currentElement = menuElements[index];

        Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, currentElement.pos.rotation, camSpeed * Time.deltaTime);
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, currentElement.pos.position, camSpeed * Time.deltaTime);
    }

    public void NewGameButton()
    {
        //TODO
        //Implement commented-out logic to offer option to overwrite save file
        //SaveDataController.instance.LoadFile();

        //if (SaveDataController.instance.saveData.currentScene != "")
        //{
        //    //If no previous save file exists, create new
        //    SaveDataController.instance.CreateNewSaveFile();
        //    mainMenuCanvas.SetActive(false);
        //    FadeController.instance.StartFade(1, 1f);
        //    sceneToLoad = "RadioRoom";
        //    loadingScene = true;
        //}
        //else
        //{
        //    //If a save file is found, display save file confirmation panel
        //}

        if (Directory.Exists(System.IO.Path.Combine(Application.persistentDataPath, "Save")))
        {
            print("Deleting old save file");
            Directory.Delete(System.IO.Path.Combine(Application.persistentDataPath, "Save"), true);
        }

        if (Directory.Exists(System.IO.Path.Combine(Application.persistentDataPath, "LevelData")))
        {
            print("Deleting old level data");
            Directory.Delete(System.IO.Path.Combine(Application.persistentDataPath, "LevelData"), true);
        }

        if (Directory.Exists(System.IO.Path.Combine(Application.persistentDataPath, "Items")))
        {
            print("Deleting old item data");
            Directory.Delete(System.IO.Path.Combine(Application.persistentDataPath, "Items"), true);
        }

        SaveDataController.instance.CreateNewSaveFile();
        FadeController.instance.StartFade(1, 1.5f);
        sceneToLoad = "Intro"; 
        loadingScene = true;
    }

    public void LoadGameButton()
    {
        SaveDataController.instance.LoadFile();

        if (SaveDataController.instance.saveData.currentScene != string.Empty)
        {
            loadGameText.text = $"Last save point: {SaveDataController.instance.saveData.currentScene}";
            loadGameCanvas.SetActive(true);

            //eventSystem.SetSelectedGameObject(loadButton);
        }
    }

    public void StartSavedGame()
    {
        FadeController.instance.StartFade(1, 1.5f);
        sceneToLoad = SaveDataController.instance.saveData.currentScene;
        loadingScene = true;
    }

    public void QuitGameButton()
    {
        Application.Quit();
    }

    IEnumerator FadeAudio(AudioSource audioSource, float aValue, float aTime)
    {
        float delta = audioSource.volume;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            delta = Mathf.Lerp(delta, aValue, t);
            audioSource.volume = delta;
            yield return null;
        }
    }
}

[System.Serializable]
public class MainMenuElement
{
    public Transform pos;
    public Button button;

    public void onClick()
    {
        button.onClick.Invoke();
    }
}