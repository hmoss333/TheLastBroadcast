using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using System.IO;


public class MainMenuController : MonoBehaviour
{
    public static MainMenuController instance;

    [SerializeField] MainMenuElement[] menuElements;
    public MainMenuElement currentElement { get; private set; }
    [SerializeField] Transform loadCamPos;
    [SerializeField] float camSpeed;
    private int index;
    public CanvasGroup settingsCanvas, loadGameCanvas;
    [SerializeField] Button loadGameButton;
    [SerializeField] TextMeshProUGUI loadGameText, versionText;
    [SerializeField] AudioSource audioSource;
    [SerializeField] float maxAudioVolume;
    [SerializeField] bool loadingScene;
    string sceneToLoad;
    SettingsMenuController settingsMenu;
    [SerializeField] LoadingScene loadingScreen;

    Coroutine newGameRoutine;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    private void Start()
    {
        settingsMenu = GetComponent<SettingsMenuController>();

        loadGameCanvas.gameObject.SetActive(false);
        index = 0;
        currentElement = menuElements[index];

        versionText.text = $"Version: {Application.version}";

        FadeController.instance.StartFade(0f, 1f);
    }

    private void Update()
    {
        //Loading saved game
        if (loadingScene)
        {
            loadGameCanvas.gameObject.SetActive(false);

            if (!FadeController.instance.isFading)
            {
                if (sceneToLoad == "Intro")
                    SceneManager.LoadScene(sceneToLoad);
                else
                    loadingScreen.LoadScene(sceneToLoad);
            }
        }
        //Menu controls
        else
        {
            if (loadGameCanvas.gameObject.activeSelf)
            {
                if (InputController.instance.inputMaster.Player.Interact.triggered)
                {                   
                    loadGameButton.onClick.Invoke();
                }
                else if (InputController.instance.inputMaster.Player.Melee.triggered)
                {
                    loadGameCanvas.gameObject.SetActive(false);
                }
            }
            else if (!settingsMenu.updatingSettings)
            {
                Vector2 inputVal = InputController.instance.inputMaster.Player.Move.ReadValue<Vector2>();
                bool inputCheck = InputController.instance.inputMaster.Player.Move.WasPressedThisFrame();
                if (Mathf.RoundToInt(inputVal.x) < 0 && inputCheck)
                {
                    index--;
                    if (index < 0)
                        index = menuElements.Length - 1;

                    settingsMenu.GetSettings();
                }
                else if (Mathf.RoundToInt(inputVal.x) > 0 && inputCheck)
                {
                    index++;
                    if (index > menuElements.Length - 1)
                        index = 0;

                    settingsMenu.GetSettings();
                }

                if (InputController.instance.inputMaster.Player.Interact.triggered)
                {
                    audioSource.Stop();
                    audioSource.clip = currentElement.clip;
                    audioSource.Play();
                    currentElement.onClick();
                }
            }
        }

        settingsCanvas.interactable = currentElement.menuOption == MainMenuElement.MenuOption.settings;       
    }

    private void FixedUpdate()
    {
        currentElement = menuElements[index];

        Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, loadGameCanvas.gameObject.activeSelf ? loadCamPos.rotation : currentElement.pos.rotation, camSpeed * Time.deltaTime);
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, loadGameCanvas.gameObject.activeSelf ? loadCamPos.position : currentElement.pos.position, camSpeed * Time.deltaTime);
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

        if (Directory.Exists(System.IO.Path.Combine(Application.persistentDataPath, "Save")) && SaveDataController.instance.saveData.currentScene != "")
        {
            print("Deleting old save file");
            Directory.Delete(System.IO.Path.Combine(Application.persistentDataPath, "Save"), true);
        }

        if (Directory.Exists(System.IO.Path.Combine(Application.persistentDataPath, "LevelData")))
        {
            print("Deleting old level data");
            Directory.Delete(System.IO.Path.Combine(Application.persistentDataPath, "LevelData"), true);
        }

        //Depreciated
        //if (Directory.Exists(System.IO.Path.Combine(Application.persistentDataPath, "Items")))
        //{
        //    print("Deleting old item data");
        //    Directory.Delete(System.IO.Path.Combine(Application.persistentDataPath, "Items"), true);
        //}

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
            loadGameCanvas.gameObject.SetActive(true);
        }
    }

    public void StartSavedGame()
    {
        FadeController.instance.StartFade(1.0f, 1.5f);
        sceneToLoad = SaveDataController.instance.saveData.currentScene;
        loadingScene = true;
    }

    public void QuitGameButton()
    {
        StartCoroutine(QuitGame());
    }

    IEnumerator QuitGame()
    {
        FadeController.instance.StartFade(1.0f, 1f);

        while (FadeController.instance.isFading)
            yield return null;

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
    public enum MenuOption { title, continueGame, newGame, settings, exit }
    public MenuOption menuOption;
    public Transform pos;
    public AudioClip clip;
    public UnityEvent trigger;

    public void onClick()
    {
        trigger.Invoke();
    }
}