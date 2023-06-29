using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    public static PauseMenuController instance;

    [SerializeField] GameObject pauseMenu, settingMenu, controlMenu;
    [SerializeField] public bool isPaused;

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
    }

    public void Update()
    {
        if (PlayerController.instance.inputMaster.Player.Pause.triggered
            && PlayerController.instance.state != PlayerController.States.interacting
            && PlayerController.instance.state != PlayerController.States.listening)
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
        SceneManager.LoadSceneAsync("MainMenu");
    }

    public void ReturnToOffice()
    {
        if (ls == null)
            ls = StartCoroutine(LoadScene("Office"));
    }

    IEnumerator LoadScene(string sceneToLoad)
    {
        print($"Loading scene {sceneToLoad}");
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        FadeController.instance.StartFade(1f, 1f);

        while (FadeController.instance.isFading)
            yield return null;

        SceneManager.LoadSceneAsync(sceneToLoad);

        ls = null;
    }

    //TODO design pause menu
    /// <summary>
    /// Toggle through menus
    ///// Settings/Control Scheme/Quit (done)
    ///// Inventory/Abilities/Stations
    ///// Lore
    /// </summary>
}
