using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;
using TMPro;

public class SaveDataController : MonoBehaviour
{
    public static SaveDataController instance;

    public SaveData saveData; //requires public for serialization
    private string saveDestination, levelDestination, loreDestination;

    public LoreSaveData loreSaveData;
    [SerializeField] LorePickup[] lorePickups;
    public SceneObjectsContainer sceneObjectContainer;
    char separator = Path.DirectorySeparatorChar;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);

        saveDestination = $"{Application.persistentDataPath}{separator}Save{separator}save.json";
        levelDestination = $"{Application.persistentDataPath}{separator}LevelData";
        loreDestination = $"{Application.persistentDataPath}{separator}Lore{separator}loreData.json";
        saveData = new SaveData();

        LoadFile();
        LoadLoreData();
        //LoadObjectData(saveData.currentScene);
    }


    //File Save/Load functions
    public void LoadFile()
    {
        if (File.Exists(saveDestination))
        {
            print("Loading save data");
            string jsonData = "";
            jsonData = File.ReadAllText(saveDestination);
            saveData = JsonUtility.FromJson<SaveData>(jsonData);
        }
        else
        {
            print("Creating new save file");
            CreateNewSaveFile();
            SaveFile();
        }
    }

    public void SaveFile()
    {
        string jsonData = JsonUtility.ToJson(saveData);
        print("Saving Data:" + jsonData);
        File.WriteAllText(saveDestination, jsonData);
    }

    public SaveData GetSaveData()
    {
        return saveData;
    }

    public void LoadObjectData(string sceneName)
    {
        string tempDest = $"{levelDestination}{separator}{sceneName}.json";

        if (File.Exists(tempDest))
        {
            print($"Loading object data for {sceneName}");
            SceneObjectsContainer tempContainer = new SceneObjectsContainer();
            string jsonData = File.ReadAllText(tempDest);
            tempContainer = JsonUtility.FromJson<SceneObjectsContainer>(jsonData);
            sceneObjectContainer = tempContainer;

            SaveObject[] sceneObjects = (SaveObject[])FindObjectsOfType(typeof(SaveObject), true);
            for (int i = 0; i < sceneObjects.Length; i++)
            {
                foreach (SceneInteractObj obj in sceneObjectContainer.sceneObjects)
                {
                    if (sceneObjects[i].id == obj.id)
                    {
                        //sceneObjects[i].name = obj.name;
                        sceneObjects[i].active = obj.active;
                        sceneObjects[i].hasActivated = obj.hasActivated;
                        break;
                    }
                }
            }
        }
        else
        {
            print("Creating new file");
            Directory.CreateDirectory($"{Application.persistentDataPath}{separator}LevelData{separator}");
            SaveObjectData();
        }
    }

    public void SaveObjectData()
    {
        SceneObjectsContainer tempContainer = new SceneObjectsContainer();
        tempContainer.sceneName = SceneManager.GetActiveScene().name;//SaveDataController.instance.saveData.currentScene;
        tempContainer.savePointID = sceneObjectContainer.savePointID;

        ///TODO as scene sizes get larger this sort will take more time to complete
        ///May be worth it to change to a per-object system
        SaveObject[] sceneObjects = (SaveObject[])FindObjectsOfType(typeof(SaveObject), true);
        for (int i = 0; i < sceneObjects.Length; i++)
        {
            SceneInteractObj tempObj = new SceneInteractObj();
            tempObj.id = sceneObjects[i].id;
            //tempObj.name = sceneObjects[i].name;
            tempObj.active = sceneObjects[i].active;
            tempObj.hasActivated = sceneObjects[i].hasActivated;

            tempContainer.sceneObjects.Add(tempObj);
        }

        string tempPath = $"{levelDestination}{separator}{tempContainer.sceneName}.json";
        string jsonData = JsonUtility.ToJson(tempContainer);
        print("Saving Object Data:" + jsonData);
        File.WriteAllText(tempPath, jsonData);
    }

    public void LoadLoreData()
    {
        if (File.Exists(loreDestination))
        {
            print("Loading lore data");
            LoreSaveData tempContainer = new LoreSaveData();
            string jsonData = File.ReadAllText(loreDestination);
            tempContainer = JsonUtility.FromJson<LoreSaveData>(jsonData);
            loreSaveData = tempContainer;

            lorePickups = GameObject.FindObjectsOfType<LorePickup>();
            for (int i = 0; i < lorePickups.Length; i++)
            {
                foreach (LoreData lore in loreSaveData.loreData)
                {
                    if (lorePickups[i].GetID() == lore.id)
                        lorePickups[i].SetValue(lore.text, lore.title);
                }
            }
        }
        else
        {
            print("Creating new lore file from resources");
            Directory.CreateDirectory($"{Application.persistentDataPath}{separator}Lore{separator}");
            string tempDest = $"Assets{separator}Resources{separator}Lore{separator}loreData.json";
            LoreSaveData tempContainer = new LoreSaveData();
            string jsonData = File.ReadAllText(tempDest);
            tempContainer = JsonUtility.FromJson<LoreSaveData>(jsonData);

            loreSaveData = tempContainer;
            SaveLoreData(-1);
        }
    }

    public void SaveLoreData(int id)
    {
        for (int i = 0; i < loreSaveData.loreData.Count; i++)
        {
            if (loreSaveData.loreData[i].id == id)
            {
                loreSaveData.loreData[i].collected = true;
                break;
            }
        }

        //TODO save lore data for specified ID
        string jsonData = JsonUtility.ToJson(loreSaveData);
        print("Saving Lore Data:" + jsonData);
        File.WriteAllText(loreDestination, jsonData);
    }


    //Initialize save file with correct formatting/values
    public void CreateNewSaveFile()
    {
        Directory.CreateDirectory($"{Application.persistentDataPath}{separator}Save{separator}");
        string resourcesPath = $"Assets{separator}Resources{separator}Save{separator}save.json";
        string jsonData = File.ReadAllText(resourcesPath);
        saveData = new SaveData();
        saveData = JsonUtility.FromJson<SaveData>(jsonData);
    }


    //Update Save Point Data
    public void SetSavePoint(string sceneName)
    {
        saveData.currentScene = sceneName;

        //Update currentScene value in save file
        string saveJson = JsonUtility.ToJson(saveData);
        print("Updating current scene");
        File.WriteAllText(saveDestination, saveJson);
    }

    public void SetSavePoint(string sceneName, int ID)
    {
        saveData.currentScene = sceneName;
        sceneObjectContainer.savePointID = ID;

        //Update savePoint ID in level file
        string tempPath = $"{levelDestination}{separator}{sceneName}.json";
        string jsonData = JsonUtility.ToJson(sceneObjectContainer);
        print("Updating Save Point:" + jsonData);
        File.WriteAllText(tempPath, jsonData);

        //Update currentScene value in save file
        string saveJson = JsonUtility.ToJson(saveData);
        print("Updating current scene");
        File.WriteAllText(saveDestination, saveJson);
    }


    //Ability Setters
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
            case "gasmask":
                saveData.abilities.gasmask = true;
                break;
            case "flashlight":
                saveData.abilities.flashlight = true;
                break;
            case "mirror":
                saveData.abilities.mirror = true;
                break;
            case "book":
                saveData.abilities.book = true;
                break;
            case "hand":
                saveData.abilities.hand = true;
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

    public void SetSecurityCardLevel(int levelVal)
    {
        saveData.cardLevel = levelVal;
    }

    public int GetSecurityCardLevel()
    {
        return saveData.cardLevel;
    }
}



[System.Serializable]
public class SaveData
{
    public string currentScene;
    public int maxHealth, maxCharge, cardLevel, healthParts, chargeParts;
    public Abilities abilities;
    public List<RadioAbility> radioAbilities;
}

[System.Serializable]
public class Abilities
{
    public bool radio;
    public bool radio_special;
    public bool crowbar;
    public bool gasmask;
    public bool flashlight;
    public bool mirror;
    public bool book;
    public bool hand;
}

[System.Serializable]
public class RadioAbility
{
    public string name;
    public float frequency;
    public bool isActive;
}

[System.Serializable]
public class SceneObjectsContainer
{
    public string sceneName;
    public int savePointID;
    public List<SceneInteractObj> sceneObjects = new List<SceneInteractObj>();
}

[System.Serializable]
public class SceneInteractObj
{
    public string id;
    //public string name;
    public bool active;
    public bool hasActivated;
}

[System.Serializable]
public class LoreSaveData
{
    public List<LoreData> loreData = new List<LoreData>();
}

[System.Serializable]
public class LoreData
{
    public int id;
    public bool collected;
    public string title;
    public string text;
}