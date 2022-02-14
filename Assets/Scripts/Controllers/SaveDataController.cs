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

        saveData.currentScene = "RadioRoom"; //Set RadioRoom as default for new save files
        saveData.savePointID = 0;

        ScenarioObjective scenario1 = new ScenarioObjective();
        scenario1.sceneName = "Apartment";
        scenario1.frequency = false;
        scenario1.powerLevel = false;
        scenario1.antenna = false;
        saveData.scenarios.Add(scenario1);

        ScenarioObjective scenario2 = new ScenarioObjective();
        scenario2.sceneName = "House";
        scenario2.frequency = false;
        scenario2.powerLevel = false;
        scenario2.antenna = false;
        saveData.scenarios.Add(scenario2);

        ScenarioObjective scenario3 = new ScenarioObjective();
        scenario3.sceneName = "Facility";
        scenario3.frequency = false;
        scenario3.powerLevel = false;
        scenario3.antenna = false;
        saveData.scenarios.Add(scenario3);

        //Randomize stations
        for (int i = 0; i < 3; i++)
        {
            float randNum = Random.Range(0f, 10f);
            saveData.radioStations.Add(randNum);
        }
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
    public List<float> radioStations = new List<float>();
}

[System.Serializable]
public class ScenarioObjective
{
    public string sceneName;
    public bool frequency;
    public bool powerLevel;
    public bool antenna;
    public List<int> collectableIDs = new List<int>();
}
