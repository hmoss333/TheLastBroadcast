using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using Newtonsoft.Json;

public class SaveDataController : MonoBehaviour
{
    public static SaveDataController instance;

    public SaveData saveData;
    string destination;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        DontDestroyOnLoad(this.gameObject);

        destination = Application.persistentDataPath + "/save.json";
        saveData = new SaveData();
        saveData.currentScene = SceneManager.GetActiveScene().name;

        LoadFile();
    }

    void LoadFile()
    {
        if (File.Exists(destination))
        {
            //Load data from file
            //saveData = JsonUtility.FromJson<SaveData>(destination);

            string jsonData = "";
            jsonData = File.ReadAllText(Application.persistentDataPath + "/save.json");
            saveData = JsonConvert.DeserializeObject<SaveData>(jsonData);
        }
        else
        {
            //Create a new file
            saveData = new SaveData();
            saveData.currentScene = "RadioRoom"; //Set RadioRoom as default for new save files
            SaveFile();
        }
    }

    public void SaveFile()
    {
        string jsonData = JsonUtility.ToJson(saveData);
        print("Saving Data:" + jsonData);
        File.WriteAllText(destination, jsonData);
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

    public SaveData ReturnSaveData()
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
