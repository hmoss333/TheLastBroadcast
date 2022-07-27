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

    //TODO
    //Update this with new save file formating
    public void CreateNewSaveFile()
    {
        saveData = new SaveData();

        saveData.currentScene = "BroadcastRoom"; //Set BroadcastRoom as default for new save files
        saveData.savePointID = 0; //Set default spawn position

        //Set all station values here
        List<Station> tempStations = new List<Station>();

        Station station0 = new Station();
        station0.frequency = 2.02f;
        station0.sceneToLoad = "Facility";
        station0.isActive = true;
        tempStations.Add(station0);

        Station station1 = new Station();
        station0.frequency = 8.2f;
        station0.sceneToLoad = "Apartment";
        station0.isActive = false;
        tempStations.Add(station1);

        saveData.stations = tempStations;


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

    public void EnableStation(string stationName)
    {
        for (int i = 0; i < saveData.stations.Count; i++)
        {
            if (saveData.stations[i].sceneToLoad == stationName)
            {
                saveData.stations[i].isActive = true;
                break;
            }
        }
    }

    //public ScenarioObjective GetScenario()
    //{
    //    ScenarioObjective returnScenario = new ScenarioObjective();
    //    for (int i = 0; i < saveData.scenarios.Count; i++)
    //    {
    //        if (saveData.scenarios[i].sceneName == saveData.currentScene)
    //        {
    //            returnScenario = saveData.scenarios[i];
    //        }
    //    }

    //    return returnScenario;
    //}

    //public ScenarioObjective GetScenario(string sceneName)
    //{
    //    ScenarioObjective returnScenario = new ScenarioObjective();
    //    for (int i = 0; i < saveData.scenarios.Count; i++)
    //    {
    //        if (saveData.scenarios[i].sceneName == sceneName)
    //        {
    //            returnScenario = saveData.scenarios[i];
    //        }
    //    }

    //    return returnScenario;
    //}

    //public void UpdateScenario(string sceneName, string valueName)
    //{
    //    ScenarioObjective tempScenario = SaveDataController.instance.GetScenario(sceneName);
    //    for (int i = 0; i < tempScenario.objectives.Count; i++)
    //    {
    //        if (tempScenario.objectives[i].name == valueName)
    //        {
    //            float randNum = Random.Range(0f, 10f);
    //            randNum = (float)System.Math.Round(randNum, 2);
    //            tempScenario.objectives[i].value = randNum;
    //            break;
    //        }
    //    }
    //}

    //public void UpdateScenario(string sceneName, float stationVal)
    //{
    //    ScenarioObjective tempScenario = GetScenario(sceneName);
    //    tempScenario.station = stationVal;
    //}

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
    public int savePointID;
    public List<Station> stations = new List<Station>();
    //public List<ScenarioObjective> scenarios = new List<ScenarioObjective>();
    public Abilities abilities;
}

//[System.Serializable]
//public class ScenarioObjective
//{
//    public string sceneName;
//    public int savePointID;
//    //public int progressVal;
//    public float station; //testing with adding station value here instead of unsorted list
//    public List<ObjectiveObj> objectives = new List<ObjectiveObj>();
//    public List<SceneInteractObj> objectStates = new List<SceneInteractObj>();

//    public ObjectiveObj GetObjective(string objName)
//    {
//        ObjectiveObj returnObj = new ObjectiveObj();
//        for (int i = 0; i < objectives.Count; i++)
//        {
//            if (objectives[i].name == objName)
//            {
//                returnObj = objectives[i];
//                break;
//            }
//        }

//        return returnObj;
//    }

//    public void GenerateStation()
//    {
//        Debug.Log("Generating radio station");      
//        station = GetObjective("frequency").value;
//    }
//}


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
public class Station
{
    public float frequency;
    //public AudioClip audioClip;
    public string sceneToLoad;
    public bool isActive;
}

[System.Serializable]
public class SceneInteractObj
{
    public int ID;
    public string name;
    public bool active;
    public bool hasActivated; 
}

//[System.Serializable]
//public class ObjectiveObj
//{
//    public string name;
//    public float value;
//    public bool hasSet;
//}
