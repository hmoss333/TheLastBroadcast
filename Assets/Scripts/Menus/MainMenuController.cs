using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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


    private void Start()
    {
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
        //SceneManager.LoadScene("Title");//SaveDataController.instance.saveData.currentScene);
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
        int saveID = SaveDataController.instance.saveData.savePointID;
        loadGameText.text = $"{SaveDataController.instance.saveData.currentScene}: \nSaveID: {saveID}";
    }

    public void StartSavedGame()
    {
        //SceneManager.LoadScene(SaveDataController.instance.saveData.currentScene);
        FadeController.instance.StartFade(1, 1f);
        sceneToLoad = SaveDataController.instance.saveData.currentScene;
        loadingScene = true;
    }

    public void QuitGameButton()
    {
        Application.Quit();
    }
}
