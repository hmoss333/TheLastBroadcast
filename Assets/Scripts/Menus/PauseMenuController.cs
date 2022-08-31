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
        ResumeGame();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !PlayerController.instance.interacting)
        {
            isPaused = !isPaused;

            if (isPaused)
                PauseGame();
            else
                ResumeGame();
        }
    }

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }


    //TODO design pause menu
    /// <summary>
    /// Toggle through menus
    /// Settings/Control Scheme/Quit
    /// Inventory/Abilities/Stations
    /// Lore
    /// </summary>
}
