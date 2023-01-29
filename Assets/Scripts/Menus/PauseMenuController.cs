using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        if (Input.GetKeyDown(KeyCode.Space) && PlayerController.instance.state != PlayerController.States.interacting)//.interacting)
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


    //TODO design pause menu
    /// <summary>
    /// Toggle through menus
    /// Settings/Control Scheme/Quit
    /// Inventory/Abilities/Stations
    /// Lore
    /// </summary>
}
