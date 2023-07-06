using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;
using System.IO;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] GameObject mainMenuCanvas;
    [SerializeField] GameObject loadGameCanvas;
    [SerializeField] TextMeshProUGUI loadGameText, debugText;
    [SerializeField] SpriteRenderer radioLight;
    [SerializeField] AudioSource backgroundAudio;
    [SerializeField] float maxAudioVolume;
    Color defaultColor, fadeColor;
    bool loadingScene;
    string sceneToLoad;

    EventSystem eventSystem;
    [SerializeField] GameObject loadButton;

    private void Start()
    {
        eventSystem = FindObjectOfType<EventSystem>();

        mainMenuCanvas.SetActive(true);
        loadGameCanvas.SetActive(false);
        defaultColor = radioLight.color;
        fadeColor = new Color(defaultColor.r, defaultColor.g, defaultColor.b, 0);

        //StartCoroutine(FadeAudio(backgroundAudio, maxAudioVolume, 10f));
    }

    private void Update()
    {
        radioLight.color = Color.Lerp(defaultColor, fadeColor, Mathf.PingPong(Time.time, 1));
        backgroundAudio.volume = Mathf.Lerp(0f, maxAudioVolume, Time.time / 10f);    

        if (loadingScene)
        {
            mainMenuCanvas.SetActive(false);
            loadGameCanvas.SetActive(false);

            if (!FadeController.instance.isFading)
            {
                print("Change scene");
                SceneManager.LoadScene(sceneToLoad);
            }
        }
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

        SaveDataController.instance.CreateNewSaveFile();
        mainMenuCanvas.SetActive(false);
        FadeController.instance.StartFade(1, 1f);
        sceneToLoad = "RadioRoom"; 
        loadingScene = true;
    }

    public void LoadGameButton()
    {
        SaveDataController.instance.LoadFile();

        if (SaveDataController.instance.saveData.currentScene != string.Empty)
        {
            loadGameText.text = $"Last save point: {SaveDataController.instance.saveData.currentScene}";
            mainMenuCanvas.SetActive(false);
            loadGameCanvas.SetActive(true);

            eventSystem.SetSelectedGameObject(loadButton);
        }
    }

    public void StartSavedGame()
    {
        FadeController.instance.StartFade(1, 1f);
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
