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

    public void LoadFile()
    {
        if (File.Exists(destination))
        {
            print("Loading data...");
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

    public void CreateNewSaveFile()
    {
        saveData = new SaveData();

        saveData.currentScene = "Facility"; //Set Facility as default for new save files
        //saveData.savePointID = 0;


        //Setup Scenario 0 objectives
        ScenarioObjective scenario0 = new ScenarioObjective();
        scenario0.sceneName = "Facility";
        scenario0.savePointID = 0;
        scenario0.progressVal = 0;
        scenario0.station = 0.0f;
        //scenario0.frequency = false;
        //scenario0.powerLevel = false;
        //scenario0.antenna = false;
        ObjectiveObj frequency0 = new ObjectiveObj();
        frequency0.name = "frequency";
        frequency0.value = 0;
        scenario0.objectives.Add(frequency0);
        ObjectiveObj antenna0 = new ObjectiveObj();
        antenna0.name = "antenna";
        antenna0.value = 0;
        scenario0.objectives.Add(antenna0);
        ObjectiveObj powerLevel0 = new ObjectiveObj();
        powerLevel0.name = "powerLevel";
        powerLevel0.value = 0;
        scenario0.objectives.Add(powerLevel0);
        saveData.scenarios.Add(scenario0);

        //Setup Scenario 1 objectives
        ScenarioObjective scenario1 = new ScenarioObjective();
        scenario1.sceneName = "Apartment";
        scenario1.savePointID = 0;
        scenario1.progressVal = 0;
        scenario1.station = 0.0f;
        //scenario1.frequency = false;
        //scenario1.powerLevel = false;
        //scenario1.antenna = false;
        ObjectiveObj frequency1 = new ObjectiveObj();
        frequency1.name = "frequency";
        frequency1.value = 0;
        scenario1.objectives.Add(frequency1);
        ObjectiveObj antenna1 = new ObjectiveObj();
        antenna1.name = "antenna";
        antenna1.value = 0;
        scenario1.objectives.Add(antenna1);
        ObjectiveObj powerLevel1 = new ObjectiveObj();
        powerLevel1.name = "powerLevel";
        powerLevel1.value = 0;
        scenario1.objectives.Add(powerLevel1);
        saveData.scenarios.Add(scenario1);

        //Setup Scenario 2 objectives
        ScenarioObjective scenario2 = new ScenarioObjective();
        scenario2.sceneName = "House";
        scenario2.savePointID = 0;
        scenario2.progressVal = 0;
        scenario2.station = 0.0f;
        //scenario2.frequency = false;
        //scenario2.powerLevel = false;
        //scenario2.antenna = false;
        ObjectiveObj frequency2 = new ObjectiveObj();
        frequency2.name = "frequency";
        frequency2.value = 0;
        scenario2.objectives.Add(frequency2);
        ObjectiveObj antenna2 = new ObjectiveObj();
        antenna2.name = "antenna";
        antenna2.value = 0;
        scenario2.objectives.Add(antenna2);
        ObjectiveObj powerLevel2 = new ObjectiveObj();
        powerLevel2.name = "powerLevel";
        powerLevel2.value = 0;
        scenario2.objectives.Add(powerLevel2);
        saveData.scenarios.Add(scenario2);

        //Setup Scenario 3 objectives
        ScenarioObjective scenario3 = new ScenarioObjective();
        scenario3.sceneName = "Library";
        scenario3.savePointID = 0;
        scenario3.progressVal = 0;
        scenario3.station = 0.0f;
        //scenario3.frequency = false;
        //scenario3.powerLevel = false;
        //scenario3.antenna = false;
        ObjectiveObj frequency3 = new ObjectiveObj();
        frequency3.name = "frequency";
        frequency3.value = 0;
        scenario3.objectives.Add(frequency3);
        ObjectiveObj antenna3 = new ObjectiveObj();
        antenna3.name = "antenna";
        antenna3.value = 0;
        scenario3.objectives.Add(antenna3);
        ObjectiveObj powerLevel3 = new ObjectiveObj();
        powerLevel3.name = "powerLevel";
        powerLevel3.value = 0;
        scenario3.objectives.Add(powerLevel3);
        saveData.scenarios.Add(scenario3);


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
        foreach (ScenarioObjective scenario in saveData.scenarios)
        {
            if (scenario.sceneName == sceneName)
            {
                scenario.savePointID = ID;
            }
        }
    }

    public ScenarioObjective GetScenario()
    {
        ScenarioObjective returnScenario = new ScenarioObjective();
        for (int i = 0; i < saveData.scenarios.Count; i++)
        {
            if (saveData.scenarios[i].sceneName == saveData.currentScene)
            {
                returnScenario = saveData.scenarios[i];
            }
        }

        return returnScenario;
    }

    public ScenarioObjective GetScenario(string sceneName)
    {
        ScenarioObjective returnScenario = new ScenarioObjective();
        for (int i = 0; i < saveData.scenarios.Count; i++)
        {
            if (saveData.scenarios[i].sceneName == sceneName)
            {
                returnScenario = saveData.scenarios[i];
            }
        }

        return returnScenario;
    }

    public void UpdateScenario(string sceneName, string valueName)
    {
        ScenarioObjective tempScenario = SaveDataController.instance.GetScenario(sceneName);
        //switch (valueName)
        //{
        //    case "frequency":
        //        tempScenario.frequency = true;
        //        break;
        //    case "powerLevel":
        //        tempScenario.powerLevel = true;
        //        break;
        //    case "antenna":
        //        tempScenario.antenna = true;
        //        break;
        //    default:
        //        Debug.Log($"{valueName} not found in saveData");
        //        break;
        //}


        //if (tempScenario.frequency && tempScenario.powerLevel && tempScenario.antenna)
        //{
        //    Debug.Log("Generating radio station");
        //    float randNum = Random.Range(0f, 10f);
        //    randNum = (float)System.Math.Round(randNum, 2);
        //    UpdateScenario(sceneName, randNum);
        //}

        for (int i = 0; i < tempScenario.objectives.Count; i++)
        {
            if (tempScenario.objectives[i].name == valueName)
            {
                float randNum = Random.Range(0f, 10f);
                randNum = (float)System.Math.Round(randNum, 2);
                tempScenario.objectives[i].value = randNum;
                break;
            }
        }
    }

    public void UpdateScenario(string sceneName, float stationVal)
    {
        ScenarioObjective tempScenario = GetScenario(sceneName);
        tempScenario.station = stationVal;
    }

    public void GiveAbility(string abilityName)
    {
        abilityName = abilityName.ToLower();
        switch (abilityName)
        {
            case "radio":
                saveData.abilities.radio = true;
                break;
            case "crowbar":
                saveData.abilities.crowbar = true;
                break;
            case "book":
                saveData.abilities.book = true;
                break;
            case "hand":
                saveData.abilities.hand = true;
                break;
            case "mirror":
                saveData.abilities.mirror = true;
                break;
            default:
                Debug.Log($"Ability not found: {abilityName}");
                break;
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
    public List<ScenarioObjective> scenarios = new List<ScenarioObjective>();
    public Abilities abilities;
}

[System.Serializable]
public class ScenarioObjective
{
    public string sceneName;
    public int savePointID;
    public int progressVal;
    public float station; //testing with adding station value here instead of unsorted list
    public List<ObjectiveObj> objectives = new List<ObjectiveObj>();
    public List<SceneInteractObj> objectStates = new List<SceneInteractObj>();
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

[System.Serializable]
public class SceneInteractObj
{
    public int ID;
    public string name;
    public bool active;
    public bool hasActivated; 
}

[System.Serializable]
public class ObjectiveObj
{
    public string name;
    public float value;
}
