using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq;

public class TranscieverController : SavePointController
{
    public static TranscieverController instance;

    [SerializeField] GameObject activeModel;
    [SerializeField] GameObject deactivatedModel;
    //[SerializeField] GameObject focusPoint;
    [SerializeField] AudioSource staticSource;
    //[SerializeField] GameObject instructionText;

    [SerializeField] [Range(0f, 10f)] float currentFrequency;
    [SerializeField] TextMeshPro frequencyText;

    [SerializeField] GameObject dialObj;
    [SerializeField] float rotSpeed;
    private float xInput;
    Dictionary<string, float> presetVals;
    int stationIndex;
    bool loadScene = false;


    [SerializeField] Color stationColor;
    [SerializeField] Color presetColor;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);


        stationIndex = 0;
    }

    private void Start()
    {
        presetVals = new Dictionary<string, float>();
        for (int i = 0; i < SaveDataController.instance.saveData.scenarios.Count; i++)
        {
            if (SaveDataController.instance.saveData.scenarios[i].station > 0.0f) //ignore if presetVal hasn't been set yet
                presetVals.Add(SaveDataController.instance.saveData.scenarios[i].sceneName, SaveDataController.instance.saveData.scenarios[i].station);
        }
    }

    private void Update()
    {
        activeModel.SetActive(active);
        deactivatedModel.SetActive(!active);
        //instructionText.SetActive(interacting);


        //Lock rotation once the player reaches either end of frequency spectrum
        float tempSpeed = rotSpeed;
        if (currentFrequency < 0.0f)
        {
            tempSpeed = 0.0f;
            currentFrequency = 0.0f;
        }
        else if (currentFrequency > 10f)
        {
            tempSpeed = 0.0f;
            currentFrequency = 10f;
        }
        else
        {
            tempSpeed = rotSpeed;
        }



        //Interact Logic
        if (interacting)
        {
            //Get latest updated list of Preset Values
            for (int i = 0; i < SaveDataController.instance.saveData.scenarios.Count; i++)
            {
                if (SaveDataController.instance.saveData.scenarios[i].station > 0.0f && !presetVals.ContainsKey(SaveDataController.instance.saveData.scenarios[i].sceneName)) //ignore if presetVal hasn't been set yet
                    presetVals.Add(SaveDataController.instance.saveData.scenarios[i].sceneName, SaveDataController.instance.saveData.scenarios[i].station);
            }


            if (Input.GetKey("right shift") && presetVals.Count > 0)
            {
                //Update station number text color
                frequencyText.color = presetColor;

                //Cycle through presets based on button input
                if (Input.GetKeyDown("left"))
                {
                    stationIndex--;
                }
                if (Input.GetKeyDown("right"))
                {
                    stationIndex++;
                }

                //Confirm that stationIndex will always referance an existing value inside of the presetVals Dictionary
                if (stationIndex < 0)
                    stationIndex = presetVals.Count - 1;
                else if (stationIndex > presetVals.Count - 1)
                    stationIndex = 0;

                //Set the current frequency to the cooresponding preset value
                if (presetVals.Keys.ElementAt(stationIndex) != SceneInitController.instance.currentScene)
                    currentFrequency = presetVals.Values.ElementAt(stationIndex);
            }
            else
            {
                frequencyText.color = stationColor;
                xInput = Input.GetAxis("Horizontal");
                dialObj.transform.Rotate(0.0f, xInput * tempSpeed, 0.0f);
                currentFrequency += (float)(xInput * Time.deltaTime);
            }

            if (Input.GetKeyDown("x"))
            {
                currentFrequency = (float)System.Math.Round(currentFrequency, 2);
                SelectStation(currentFrequency);
            }

            float displayFrequency = currentFrequency * 10f;
            frequencyText.text = displayFrequency.ToString("F1");
        }
    }

    void SelectStation(float stationVal)
    {
        foreach (KeyValuePair<string, float> preset in presetVals)
        {
            if (stationVal == preset.Value && !loadScene)
            {
                loadScene = true;
                StartCoroutine(LoadStation(preset.Key));
            }
        }
    }

    IEnumerator LoadStation(string sceneToLoad)
    {
        FadeController.instance.StartFade(1.0f, 1f);

        while (FadeController.instance.isFading)
            yield return null;

        PlayerController.instance.ToggleAvatar();
        SceneInitController.instance.SaveInteractObjs();
        SceneManager.LoadSceneAsync(sceneToLoad);
    }

    public override void Interact()
    {
        if (active)
        {
            base.Interact();
        }

        staticSource.mute = !interacting;
    }
}
