using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    public static PauseMenuController instance;

    [SerializeField] GameObject pauseMenu, settingMenu, controlMenu;
    [SerializeField] public bool isPaused;


    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Start()
    {
        isPaused = false;
        PauseGame(isPaused);
    }

    public void Update()
    {
        if (PlayerController.instance.inputMaster.Player.Pause.triggered && PlayerController.instance.state != PlayerController.States.interacting)
        {
            isPaused = !isPaused;

            PauseGame(isPaused);
        }
    }

    public void PauseGame(bool paused)
    {
        pauseMenu.SetActive(paused);
        Time.timeScale = paused ? 0f : 1f;
    }

    public void SettingMenu()
    {
        print("Load setting menu here");
        settingMenu.SetActive(!settingMenu.activeSelf);
    }

    public void ControlMenu()
    {
        print("Load control menu here");
        controlMenu.SetActive(!controlMenu.activeSelf);
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }    

    //TODO design pause menu
    /// <summary>
    /// Toggle through menus
    ///// Settings/Control Scheme/Quit (done)
    ///// Inventory/Abilities/Stations
    ///// Lore
    /// </summary>
}
