using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RadioOverlay_Controller : MonoBehaviour
{
    public static RadioOverlay_Controller instance;

    [SerializeField] float speed;
    [SerializeField] float yOffSet;
    [SerializeField] float maxFrequency;
    [Range(0.0f, 10.0f)]
    public float currentFrequency;
    public bool isActive;
    [SerializeField] float stationOffset;


    //UI Elements
    [SerializeField] GameObject overlayPanel;
    [SerializeField] RawImage stationScroll;
    [SerializeField] TextMeshProUGUI frequencyText;
    float xInput;

    //Audio Elements
    [SerializeField] AudioSource staticSource;
    [SerializeField] AudioSource stationSource;
    [SerializeField] List<float> stationNums;
    [SerializeField] Dictionary<string, AudioClip> stations;



    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }

    private void Start()
    {
        stations = new Dictionary<string, AudioClip>();
        stationNums = new List<float>();

        stations = GetRadioStations();
        stationNums = GetRadioStationNumbers();
    }

    // Update is called once per frame
    void Update()
    {
        //Track player position
        transform.localPosition = new Vector3(PlayerController.instance.transform.position.x, PlayerController.instance.transform.position.y + yOffSet);

        //Lock station slider to only scroll within the min/max range
        float tempSpeed = speed;
        if (currentFrequency < 0.0f)
        {
            tempSpeed = 0.0f;
            currentFrequency = 0.0f;
        }
        else if (currentFrequency > maxFrequency)
        {
            tempSpeed = 0.0f;
            currentFrequency = maxFrequency;
        }
        else
        {
            tempSpeed = speed;
        }


        if (isActive)
        {
            if (stationSource.clip)
            {
                if (!stationSource.isPlaying)
                {
                    //Keep radio station playback updated in real time
                    if (Time.realtimeSinceStartup > stationSource.clip.length)
                        stationSource.time = 0f;
                    else
                        stationSource.time = Time.realtimeSinceStartup;
                    stationSource.Play();
                }
                if (!staticSource.isPlaying)
                    staticSource.Play();
            }

            //Display the current frequency
            float displayFrequency = currentFrequency * 10f;
            frequencyText.text = displayFrequency.ToString("F1");


            //Get input values
            xInput = Input.GetAxis("Horizontal");
            currentFrequency += (float)(xInput * Time.deltaTime);


            //Offset station strip element
            float xPos = stationScroll.uvRect.x;
            stationScroll.uvRect = new Rect(xPos += tempSpeed * xInput * Time.deltaTime, 0, 1, 1);


            //Play station as radio approaches set station
            for (int i = 0; i < stationNums.Count; i++)
            {
                if (currentFrequency >= (stationNums[i] - stationOffset) && currentFrequency <= (stationNums[i] + stationOffset))
                {
                    //Check for all available stations
                    foreach (string stationVal in stations.Keys)
                    {
                        //If the current station is the one being checked
                        if (stationVal == stationNums[i].ToString())
                        {
                            //Set clip and play station audio
                            if (stationSource.clip != stations[stationVal])
                                stationSource.clip = stations[stationVal];
                            staticSource.mute = true; //mute static audio
                            stationSource.mute = false; //unmute station audio
                            //stationSource.volume = Mathf.Abs(currentFrequency) / stationNums[i]; //experimenting with this one
                            break;
                        }
                    }
                    break;
                }
                else
                {
                    //If no station is selected, play static
                    staticSource.mute = false; //unmute static audio
                    stationSource.mute = true; //mute station audio
                }
            }
        }
        else
        {
            staticSource.mute = true;
        }

        overlayPanel.SetActive(isActive);
    }

    public void ToggleOn()
    {
        isActive = !isActive;
    }

    Dictionary<string, AudioClip> GetRadioStations()
    {
        Dictionary<string, AudioClip> tempDict = new Dictionary<string, AudioClip>();
        List<AudioClip> stationAudio = new List<AudioClip>();

        stationAudio.AddRange(Resources.LoadAll<AudioClip>("Stations/"));
        float stationVal;

        for (int i = 0; i < stationAudio.Count; i++)
        {
            if (float.TryParse(stationAudio[i].name, out stationVal))
            {
                tempDict.Add(stationAudio[i].name, stationAudio[i]);
            }
            else
            {
                Debug.Log(stationAudio[i].name + " name is improperly formatted. Should be a float value.");
            }
        }

        return tempDict;
    }

    List<float> GetRadioStationNumbers()
    {
        List<float> tempList = new List<float>();
        foreach (KeyValuePair<string, AudioClip> pair in stations)
        {
            try
            {
                float stationVal = float.Parse(pair.Key);

                tempList.Add(stationVal);
            }
            catch
            {
                Debug.Log("Station name formatted improperly: " + pair.Key);
            }
        }

        return tempList;
    }
}
