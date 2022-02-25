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


    private void Start()
    {
        mainMenuCanvas.SetActive(true);
        loadGameCanvas.SetActive(false);
    }

    public void NewGameButton()
    {        
        SaveDataController.instance.CreateNewSaveFile();
        SceneManager.LoadScene(SaveDataController.instance.saveData.currentScene);
    }

    public void LoadGameButton()
    {
        SaveDataController.instance.LoadFile();
        mainMenuCanvas.SetActive(false);
        loadGameCanvas.SetActive(true);
        int saveID = SaveDataController.instance.GetScenario().savePointID;
        loadGameText.text = $"{SaveDataController.instance.saveData.currentScene}: \nSaveID: {saveID}";
    }

    public void StartSavedGame()
    {
        SceneManager.LoadScene(SaveDataController.instance.saveData.currentScene);
    }

    public void QuitGameButton()
    {
        Application.Quit();
    }
}
