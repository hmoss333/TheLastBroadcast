using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    public static PauseMenuController instance;

    [SerializeField] GameObject pauseMenu;//, inventoryMenu, abilityMenu, settingMenu;
    [SerializeField] public bool isPaused;
    [SerializeField] List<GameObject> menuPanels;
    int menuIndex = 0;

    Coroutine ls;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Start()
    {
        isPaused = false;
        PauseGame(isPaused);
        ToggleMenu(0);
    }

    public void Update()
    {
        if (PlayerController.instance.inputMaster.Player.Pause.triggered
            && PlayerController.instance.state != PlayerController.States.interacting
            && PlayerController.instance.state != PlayerController.States.listening
            && PlayerController.instance.state != PlayerController.States.wakeUp)
        {
            //If a SceneToIgnore is loaded, disable pause
            List<Scene> activeScenes = SceneManager.GetAllScenes().ToList();
            for (int i = 0; i < activeScenes.Count; i++)
            {
                if (SceneInitController.instance.GetScenesToIgnore().Contains(activeScenes[i].name))
                {
                    return;
                }
            }

            isPaused = !isPaused;

            PauseGame(isPaused);
        }

        if (isPaused)
        {           
            if (PlayerController.instance.inputMaster.Player.MenuLeft.triggered)
            {
                menuIndex--;
                if (menuIndex < 0)
                    menuIndex = menuPanels.Count - 1;

                ToggleMenu(menuIndex);
            }
            if (PlayerController.instance.inputMaster.Player.MenuRight.triggered)
            {
                menuIndex++;
                if (menuIndex > menuPanels.Count - 1)
                    menuIndex = 0;

                ToggleMenu(menuIndex);
            }
        }
    }

    public void PauseGame(bool paused)
    {
        pauseMenu.SetActive(paused);
        Time.timeScale = paused ? 0f : 1f;
    }

    void ToggleMenu(int index)
    {
        for (int i = 0; i < menuPanels.Count; i++)
        {
            menuPanels[i].SetActive(i == index);
        }
    }

    public GameObject CurrentPanel()
    {
        return menuPanels[menuIndex];
    }

    public void SettingMenu()
    {
        print("Load setting menu here");
        //settingMenu.SetActive(!settingMenu.activeSelf);
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadSceneAsync("MainMenu");
    }
}
