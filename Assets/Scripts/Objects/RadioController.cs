using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class RadioController : InteractObject
{
    public static RadioController instance;

    //[SerializeField] GameObject radioPrefab;
    [SerializeField] GameObject activeModel;
    [SerializeField] GameObject deactivatedModel;
    [SerializeField] GameObject focusPoint;
    [SerializeField] AudioSource staticSource;
    [SerializeField] GameObject instructionText;

    [SerializeField][Range(0f, 10f)] float currentFrequency;
    [SerializeField] TextMeshPro frequencyText;

    [SerializeField] GameObject dialObj;
    [SerializeField] float rotSpeed;
    private float xInput;
    [SerializeField] List<float> stationNums;
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
        stationNums = new List<float>();
        stationNums = SaveDataController.instance.saveData.radioStations;
    }

    private void Update()
    {
        activeModel.SetActive(activated);
        deactivatedModel.SetActive(!activated);
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
            if (Input.GetKey("right shift") && stationNums.Count > 0)
            {
                frequencyText.color = presetColor;
                if (Input.GetKeyDown("left"))
                {
                    stationIndex--;
                    if (stationIndex < 0)
                        stationIndex = stationNums.Count - 1;
                }
                if (Input.GetKeyDown("right"))
                {
                    stationIndex++;
                    if (stationIndex > stationNums.Count - 1)
                        stationIndex = 0;
                }

                currentFrequency = stationNums[stationIndex];
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
        if (stationVal == stationNums[0] && !loadScene)
        {
            loadScene = true;
            StartCoroutine(LoadStation("Apartment"));
        }
        else if (stationVal == stationNums[1] && !loadScene)
        {
            loadScene = true;
            StartCoroutine(LoadStation("House"));
        }
        else if (stationVal == stationNums[2] && !loadScene)
        {
            loadScene = true;
            StartCoroutine(LoadStation("Facility"));
        }
        else
        {
            Debug.Log("Station not found in presets");
        }
    }

    IEnumerator LoadStation(string sceneToLoad)
    {
        FadeController.instance.StartFade(1.0f, 1f);

        while (FadeController.instance.isFading)
            yield return null;

        PlayerController.instance.ToggleAvatar();
        SceneManager.LoadSceneAsync(sceneToLoad);
    }

    public override void Interact()
    {
        if (activated)
        {
            base.Interact();

            PlayerController.instance.ToggleAvatar();
            CameraController.instance.SetTarget(interacting ? focusPoint : PlayerController.instance.gameObject);
            CameraController.instance.FocusTarget();
        }

        staticSource.mute = !interacting;
    }
}
