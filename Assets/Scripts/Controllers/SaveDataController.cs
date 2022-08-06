using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;

public class SaveDataController : MonoBehaviour
{
    public static SaveDataController instance;

    public SaveData saveData; //requires public for serialization
    private string destination;

    public SceneObjectsContainer sceneObjectContainer;



    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);

        DontDestroyOnLoad(this.gameObject);

        destination = Application.persistentDataPath + "/save.json";
        saveData = new SaveData();

        LoadFile();
        LoadObjectData(saveData.currentScene);
    }


    public void LoadFile()
    {
        if (File.Exists(destination))
        {
            print("Loading data...");
            //Load data from file
            string jsonData = "";
            jsonData = File.ReadAllText(destination);
            saveData = JsonUtility.FromJson<SaveData>(jsonData);
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

    public SaveData GetSaveData()
    {
        return saveData;
    }

    public void LoadObjectData(string sceneName)
    {
        string tempDest = Application.persistentDataPath + "/" + sceneName + ".json";

        if (File.Exists(tempDest))
        {
            print("Loading data...");
            //Load data from file
            SceneObjectsContainer tempContainer = new SceneObjectsContainer();
            string jsonData = File.ReadAllText(tempDest);
            tempContainer = JsonUtility.FromJson<SceneObjectsContainer>(jsonData);

            sceneObjectContainer = tempContainer;
        }
        else
        {
            //Create a new file
            print("Creating new file");
            SaveObjectData(sceneName);
        }
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
        station1.frequency = 8.20f;
        station1.sceneToLoad = "Apartment";
        station1.isActive = false;
        tempStations.Add(station1);

        Station station2 = new Station();
        station2.frequency = 3.330f;
        station2.sceneToLoad = "Backrooms";
        station2.isActive = false;
        tempStations.Add(station2);

        Station station3 = new Station();
        station3.frequency = 9.30f;
        station3.sceneToLoad = "Library";
        station3.isActive = false;
        tempStations.Add(station3);

        saveData.stations = tempStations;


        //Set radio ability values here
        List<RadioAbility> tempRadioAbilities = new List<RadioAbility>();

        RadioAbility rb0 = new RadioAbility();
        rb0.name = "Tune";
        rb0.frequency = 2.5f;
        rb0.isActive = false;
        tempRadioAbilities.Add(rb0);

        RadioAbility rb1 = new RadioAbility();
        rb1.name = "Invisibility";
        rb1.frequency = 5.0f;
        rb1.isActive = false;
        tempRadioAbilities.Add(rb1);

        RadioAbility rb2 = new RadioAbility();
        rb2.name = "Rats";
        rb2.frequency = 7.5f;
        rb2.isActive = false;
        tempRadioAbilities.Add(rb2);

        saveData.radioAbilities = tempRadioAbilities;


        //Set Ability defaults
        Abilities tempAbilities = new Abilities();
        tempAbilities.radio = false;
        tempAbilities.crowbar = false;
        tempAbilities.book = false;
        tempAbilities.hand = false;
        tempAbilities.mirror = false;
        saveData.abilities = tempAbilities;
    }


    //Update Save Point Data
    public void SetSavePoint(string sceneName, int ID)
    {
        saveData.currentScene = sceneName;
        saveData.savePointID = ID;

        SaveFile();
    }

    public void SaveObjectData(string sceneName)
    {
        SceneObjectsContainer tempContainer = new SceneObjectsContainer();
        tempContainer.sceneName = sceneName;

        InteractObject[] sceneObjects = (InteractObject[])FindObjectsOfType(typeof(InteractObject), true);
        for (int i = 0; i < sceneObjects.Length; i++)
        {
            SceneInteractObj tempObj = new SceneInteractObj();
            tempObj.name = sceneObjects[i].name;
            tempObj.active = sceneObjects[i].active;
            tempObj.hasActivated = sceneObjects[i].hasActivated;

            tempContainer.sceneObjects.Add(tempObj);
        }

        string tempPath = Application.persistentDataPath + "/" + sceneName + ".json";
        string jsonData = JsonUtility.ToJson(tempContainer);
        print("Saving Object Data:" + jsonData);
        File.WriteAllText(tempPath, jsonData);
    }

    //Ability Setters
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

    public void GiveRadioAbility(string abilityName)
    {
        for (int i = 0; i < saveData.radioAbilities.Count; i++)
        {
            if (saveData.radioAbilities[i].name == abilityName)
            {
                saveData.radioAbilities[i].isActive = true;
                break;
            }
        }
    }

    public void GiveAbility(string abilityName)
    {
        abilityName = abilityName.ToLower();
        switch (abilityName)
        {
            case "radio":
                saveData.abilities.radio = true;
                break;
            case "radio_special":
                saveData.abilities.radio_special = true;
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

    public RadioAbility GetRadioAbility(string abilityName)
    {
        string tempName = abilityName.ToLower();
        SaveData tempData = GetSaveData();

        for (int i = 0; i < tempData.radioAbilities.Count; i++)
        {
            if (tempData.radioAbilities[i].name.ToLower() == tempName)
                return tempData.radioAbilities[i];
        }

        print("No matching ability found for " + abilityName);
        return null;
    }
}



[System.Serializable]
public class SaveData
{
    public string currentScene;
    public int savePointID;
    public List<Station> stations = new List<Station>();
    public Abilities abilities;
    public List<RadioAbility> radioAbilities;
    //    public List<SceneInteractObj> objectStates = new List<SceneInteractObj>();
}

[System.Serializable]
public class Abilities
{
    public bool radio;
    public bool radio_special;
    public bool crowbar;
    public bool book;
    public bool hand;
    public bool mirror;
}

[System.Serializable]
public class RadioAbility
{
    public string name;
    public float frequency;
    public bool isActive;
}

[System.Serializable]
public class Station
{
    //TODO initialize these values from a csv file and audio clip folder
    public float frequency;
    //public AudioClip audioClip;
    public string sceneToLoad;
    public bool isActive;
}

[System.Serializable]
public class SceneObjectsContainer
{
    public string sceneName;
    public List<SceneInteractObj> sceneObjects = new List<SceneInteractObj>();
}

[System.Serializable]
public class SceneInteractObj
{
    //public int ID;
    public string name;
    public bool active;
    public bool hasActivated; 
}