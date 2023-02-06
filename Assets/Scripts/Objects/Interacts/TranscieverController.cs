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

    [SerializeField] AudioSource staticSource;

    [SerializeField] [Range(0f, 10f)] float currentFrequency;
    [SerializeField] TextMeshPro frequencyText;
    [SerializeField] MeshRenderer lightMesh;

    [SerializeField] GameObject dialObj;
    [SerializeField] float rotSpeed;
    private float xInput;
    Dictionary<float, Station> presetVals;
    [SerializeField] bool startCountdown = false;
    [SerializeField] bool loadScene = false;
    [SerializeField] float countdownTime = 2f;
    [SerializeField] string sceneToLoad;


    [SerializeField] Color stationColor;
    [SerializeField] Color presetColor;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }

    private void Start()
    {
        presetVals = new Dictionary<float, Station>();
        UpdatedStationList();
    }

    private void Update()
    {
        lightMesh.material.color = active ? Color.green : Color.red;

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
            frequencyText.color = stationColor;
            xInput = Input.GetAxis("Horizontal");
            dialObj.transform.Rotate(0.0f, xInput * tempSpeed, 0.0f);
            currentFrequency += (float)(xInput * Time.deltaTime);

            //Compare current frequency to available stations
            ///Very gross, but works for now
            foreach (float frequency in presetVals.Keys)
            {
                if (presetVals[frequency].sceneToLoad != SceneManager.GetActiveScene().name &&
                    presetVals[frequency].isActive &&
                    currentFrequency >= frequency - 0.015f && currentFrequency <= frequency + 0.015f)
                {
                    frequencyText.color = presetColor;
                    lightMesh.material.color = presetColor;
                    sceneToLoad = presetVals[frequency].sceneToLoad;
                    startCountdown = true;
                    break;
                }
                else
                {
                    startCountdown = false;
                }
            }

            float displayFrequency = currentFrequency * 10f;
            frequencyText.text = displayFrequency.ToString("F1");
        }


        //Countdown Timer
        if (startCountdown)
        {
            countdownTime -= Time.deltaTime;
            if (countdownTime < 0 && !loadScene)
            {
                loadScene = true;
                interacting = false;
                StartCoroutine(LoadStation(sceneToLoad));
            }
        }
        else
        {
            countdownTime = 2f;
        }
    }

    IEnumerator LoadStation(string sceneToLoad)
    {
        FadeController.instance.StartFade(1.0f, 1f);

        while (FadeController.instance.isFading)
            yield return null;

        SaveDataController.instance.SetSavePoint(sceneToLoad, 0);
        PlayerController.instance.ToggleAvatar();
        //SceneInitController.instance.SaveInteractObjs();
        SceneManager.LoadSceneAsync(sceneToLoad);
    }

    public override void Interact()
    {
        if (active)
        {
            base.Interact();
        }
        //else
        //{
        //    //Debug.Log($"The {name} doesn't appear to have any power");
        //    //UIController.instance.SetDialogueText($"The {name} doesn't appear to have any power");
        //    //UIController.instance.FadeUI(3f);
        //}

        //UIController.instance.ToggleDialogueUI(interacting);
        staticSource.mute = !interacting;
        UpdatedStationList();
    }

    void UpdatedStationList()
    {
        foreach (Station station in SaveDataController.instance.saveData.stations)
        {
            if (!presetVals.ContainsKey(station.frequency))
                presetVals.Add(station.frequency, station);
        }
    }
}
