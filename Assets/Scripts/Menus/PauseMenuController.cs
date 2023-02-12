using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    [SerializeField] bool isPaused;


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

    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    //TODO design pause menu
    /// <summary>
    /// Toggle through menus
    ///// Settings/Control Scheme/Quit/Load last save
    ///// Inventory/Abilities/Stations
    ///// Lore
    /// </summary>
}
