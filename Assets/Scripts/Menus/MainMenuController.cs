using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] GameObject mainMenuCanvas;
    [SerializeField] GameObject loadGameCanvas;
    [SerializeField] TextMeshProUGUI loadGameText;
    [SerializeField] SpriteRenderer radioLight;
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
    }

    private void Update()
    {
        radioLight.color = Color.Lerp(defaultColor, fadeColor, Mathf.PingPong(Time.time, 1));

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

        SaveDataController.instance.CreateNewSaveFile();
        FadeController.instance.StartFade(1, 1f);
        sceneToLoad = "Title";
        loadingScene = true;
    }

    public void LoadGameButton()
    {
        SaveDataController.instance.LoadFile();
        SaveDataController.instance.LoadObjectData(SaveDataController.instance.saveData.currentScene);
        mainMenuCanvas.SetActive(false);
        loadGameCanvas.SetActive(true);

        eventSystem.SetSelectedGameObject(loadButton);

        int saveID = SaveDataController.instance.sceneObjectContainer.savePointID;
        loadGameText.text = $"{SaveDataController.instance.saveData.currentScene}: \nSaveID: {saveID}";
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
}
