using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class SettingsMenuController : MonoBehaviour
{
    public bool updatingSettings;// { get; private set; }

    [Header("UI Elements")]
    [SerializeField] TextMeshProUGUI setScreenText;
    [SerializeField] Slider brightnessSlider, masterVolSlider, musicVolSlider;

    [Header("Values")]
    private int screenVal;
    [Range(0.0f, 1.0f)] private float brightnessVal;
    [Range(0.0f, 1.0f)] private float masterVol;
    [Range(0.0f, 1.0f)] private float musicVol;

    //MainMenuController mainMenuController;
    EventSystem eventSystem;


    private void Start()
    {
        //mainMenuController = GetComponent<MainMenuController>();
        eventSystem = FindObjectOfType<EventSystem>();
        updatingSettings = false;

        GetSettings();
        ApplySettings();
    }


    private void Update()
    {
        try
        {
            if (eventSystem.currentSelectedGameObject.GetComponent<Slider>())
            {
                updatingSettings = true;
            }
            else
            {
                updatingSettings = false;
            }
        }
        catch { }
    }

    public void GetSettings()
    {
        screenVal = PlayerPrefs.GetInt("screenVal", 0);
        brightnessVal = PlayerPrefs.GetFloat("brightnessVal", 0.5f);
        masterVol = PlayerPrefs.GetFloat("masterVol", 1.0f);
        musicVol = PlayerPrefs.GetFloat("musicVol", 1.0f);

        SetScreenName();
        brightnessSlider.value = brightnessVal;
        masterVolSlider.value = masterVol;
        musicVolSlider.value = musicVol;
    }

    public void ApplySettings()
    {
        //Screen Settings
        PlayerPrefs.SetInt("screenVal", screenVal);
        FullScreenMode fullScreenMode;
        switch (screenVal)
        {
            case 0:
                fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                break;
            case 1:
                fullScreenMode = FullScreenMode.FullScreenWindow;
                break;
            case 2:
                fullScreenMode = FullScreenMode.MaximizedWindow;
                break;
            case 3:
                fullScreenMode = FullScreenMode.Windowed;
                break;
            default:
                fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                break;
        }
        Screen.fullScreenMode = fullScreenMode;

        //Brightness Setting
        PlayerPrefs.SetFloat("brightnessVal", brightnessVal);

        //Music Settings
        //PlayerPrefs.SetFloat("masterVol", masterVol);
        //PlayerPrefs.SetFloat("musicVol", musicVol);

        //AudioListener.volume = masterVol;
    }


    //Fullscreen Mode
    public void SetScreen()
    {
        screenVal++;
        if (screenVal > 3)
            screenVal = 0;

        SetScreenName();
    }

    void SetScreenName()
    {
        string name = string.Empty;
        switch (screenVal)
        {
            case 0:
                name = "Full Screen";
                break;
            case 1:
                name = "Full Screen, Windowed";
                break;
            case 2:
                name = "Maximized Window";
                break;
            case 3:
                name = "Windowed";
                break;
            default:
                name = $"Error: {screenVal}";
                break;
        }

        setScreenText.text = $"Screen: {name}";
    }


    //Brightness
    public void SetBrightnessVal()
    {
        brightnessVal = brightnessSlider.value;
        Screen.brightness = brightnessVal;
    }


    //Audio
    public void SetMaxVolume()
    {
        masterVol = masterVolSlider.value;
        PlayerPrefs.SetFloat("masterVol", masterVol);
        AudioListener.volume = masterVol;
    }

    public void SetMusicVolume()
    {
        musicVol = musicVolSlider.value;
        PlayerPrefs.SetFloat("musicVol", musicVol);
    }
}
