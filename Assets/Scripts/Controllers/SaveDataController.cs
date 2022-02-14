using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using Newtonsoft.Json;

public class SaveDataController : MonoBehaviour
{
    public static SaveDataController instance;

    public SaveData saveData; //requires public for serialization
    private string destination;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        DontDestroyOnLoad(this.gameObject);

        destination = Application.persistentDataPath + "/save.json";
        saveData = new SaveData();

        LoadFile();
    }

    void LoadFile()
    {
        if (File.Exists(destination))
        {
            //Load data from file
            string jsonData = "";
            jsonData = File.ReadAllText(destination);
            saveData = JsonConvert.DeserializeObject<SaveData>(jsonData);
        }
        else
        {
            //Create a new file
            CreateNewSaveFile();
            SaveFile();
        }
    }

    public void SaveFile()
    {
        string jsonData = JsonUtility.ToJson(saveData);
        print("Saving Data:" + jsonData);
        File.WriteAllText(destination, jsonData);
    }

    void CreateNewSaveFile()
    {
        saveData = new SaveData();

        saveData.currentScene = "Facility"; //Set Facility as default for new save files
        saveData.savePointID = 0;


        //Setup Scenario 1 objectives
        ScenarioObjective scenario1 = new ScenarioObjective();
        scenario1.sceneName = "Apartment";
        scenario1.presetVal = 0.0f;
        scenario1.frequency = false;
        scenario1.powerLevel = false;
        scenario1.antenna = false;
        saveData.scenarios.Add(scenario1);

        //Setup Scenario 2 objectives
        ScenarioObjective scenario2 = new ScenarioObjective();
        scenario2.sceneName = "House";
        scenario2.presetVal = 0.0f;
        scenario2.frequency = false;
        scenario2.powerLevel = false;
        scenario2.antenna = false;
        saveData.scenarios.Add(scenario2);

        //Setup Scenario 3 objectives
        ScenarioObjective scenario3 = new ScenarioObjective();
        scenario3.sceneName = "Library";
        scenario3.presetVal = 0.0f;
        scenario3.frequency = false;
        scenario3.powerLevel = false;
        scenario3.antenna = false;
        saveData.scenarios.Add(scenario3);


        //Randomize stations
        //TODO have each station be generated and added once scenario objectives have been completed
        for (int i = 0; i < saveData.scenarios.Count; i++)
        {
            float randNum = Random.Range(0f, 10f);
            randNum = (float)System.Math.Round(randNum, 2);
            UpdateScenario(saveData.scenarios[i].sceneName, randNum);
        }

        //Set Ability defaults
        Abilities tempAbilities = new Abilities();
        tempAbilities.radio = false;
        tempAbilities.crowbar = false;
        tempAbilities.book = false;
        tempAbilities.hand = false;
        tempAbilities.mirror = false;
        saveData.abilities = tempAbilities;
    }

    public void SetSavePoint(string sceneName, int ID)
    {
        saveData.currentScene = sceneName;
        saveData.savePointID = ID;
    }

    public void UpdateScenario(string sceneName, string valueName, bool value)
    {
        foreach (ScenarioObjective scenario in saveData.scenarios)
        {
            if (scenario.sceneName == sceneName)
            {
                switch (valueName)
                {
                    case "frequency":
                        scenario.frequency = value;
                        break;
                    case "powerLevel":
                        scenario.powerLevel = value;
                        break;
                    case "antenna":
                        scenario.antenna = value;
                        break;
                    default:
                        Debug.Log($"{valueName} not found in saveData");
                        break;
                }

                break;
            }
        }
    }

    public void UpdateScenario(string sceneName, float stationVal)
    {
        foreach (ScenarioObjective scenario in saveData.scenarios)
        {
            if (scenario.sceneName == sceneName)
            {
                scenario.presetVal = stationVal;
                break;
            }
        }
    }

    public SaveData GetSaveData()
    {
        return saveData;
    }
}



[System.Serializable]
public class SaveData
{
    public string currentScene;
    public int savePointID;
    public List<ScenarioObjective> scenarios = new List<ScenarioObjective>();
    //public List<float> radioStations = new List<float>(); //Need to configure this as a Dictionary<string, float> to make sure that stations are only added once per area
    public Abilities abilities;
    //public List<int> collectableIDs = new List<int>();
}

[System.Serializable]
public class ScenarioObjective
{
    public string sceneName;
    public float presetVal; //testing with adding station value here instead of unsorted list
    public bool frequency;
    public bool powerLevel;
    public bool antenna;
}

[System.Serializable]
public class Abilities
{
    public bool radio;
    public bool crowbar;
    public bool book;
    public bool hand;
    public bool mirror;
}
