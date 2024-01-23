using System.Collections;
using System.Collections.Generic;
using System.Runtime;
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


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);

        saveDestination = System.IO.Path.Combine(Application.persistentDataPath, "Save", "save.json");
        levelDestination = System.IO.Path.Combine(Application.persistentDataPath, "LevelData");
        loreDestination = System.IO.Path.Combine(Application.streamingAssetsPath, "loreData.json");
        saveData = new SaveData();

        LoadFile();
        LoadLoreData();
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
        //StartCoroutine(SaveFileRoutine());
    }

    //IEnumerator SaveFileRoutine()
    //{
    //    string jsonData = JsonUtility.ToJson(saveData);
    //    print("Saving Data:" + jsonData);
    //    File.WriteAllText(saveDestination, jsonData);
    //    yield return new WaitForSeconds(0.5f); //trying this
    //}

    public SaveData GetSaveData()
    {
        return saveData;
    }

    public void LoadObjectData(string sceneName)
    {
        string tempDest = System.IO.Path.Combine(levelDestination, $"{sceneName}.json");

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
                        sceneObjects[i].needItem = obj.needItem;
                        sceneObjects[i].inventoryItemID = obj.inventoryItemID;
                        sceneObjects[i].InitializeObject();
                        break;
                    }
                }
            }
        }
        else
        {
            print("Creating new file");
            Directory.CreateDirectory(System.IO.Path.Combine(Application.persistentDataPath, "LevelData"));
            SaveObjectData();
            LoadObjectData(sceneName);
        }
    }

    public void SaveObjectData()
    {
        SceneObjectsContainer tempContainer = new SceneObjectsContainer();
        tempContainer.sceneName = SceneManager.GetActiveScene().name;
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
            tempObj.needItem = sceneObjects[i].needItem;
            tempObj.inventoryItemID = sceneObjects[i].inventoryItemID;

            tempContainer.sceneObjects.Add(tempObj);
        }

        string tempPath = System.IO.Path.Combine(levelDestination, $"{tempContainer.sceneName}.json");
        string jsonData = JsonUtility.ToJson(tempContainer);
        print("Saving Object Data:" + jsonData);
        File.WriteAllText(tempPath, jsonData);
        //StartCoroutine(SaveObjectDataRoutine());
    }

    IEnumerator SaveObjectDataRoutine()
    {
        SceneObjectsContainer tempContainer = new SceneObjectsContainer();
        tempContainer.sceneName = SceneManager.GetActiveScene().name;
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
            tempObj.needItem = sceneObjects[i].needItem;
            tempObj.inventoryItemID = sceneObjects[i].inventoryItemID;

            tempContainer.sceneObjects.Add(tempObj);
        }

        string tempPath = System.IO.Path.Combine(levelDestination, $"{tempContainer.sceneName}.json");
        string jsonData = JsonUtility.ToJson(tempContainer);
        print("Saving Object Data:" + jsonData);
        File.WriteAllText(tempPath, jsonData);
        yield return new WaitForSeconds(0.5f);
    }

    public void LoadLoreData()
    {
        //Load lore data from streamingassets location
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

    //Pulling for now as we are not supporting re-reading lore from the menu at the moment
    //public void SaveLoreData(int id)
    //{
    //    for (int i = 0; i < loreSaveData.loreData.Count; i++)
    //    {
    //        if (loreSaveData.loreData[i].id == id)
    //        {
    //            loreSaveData.loreData[i].collected = true;
    //            break;
    //        }
    //    }

    //    //TODO save lore data for specified ID
    //    string jsonData = JsonUtility.ToJson(loreSaveData);
    //    print("Saving Lore Data:" + jsonData);
    //    File.WriteAllText(loreDestination, jsonData);
    //}


    //Initialize save file with correct formatting/values
    public void CreateNewSaveFile()
    {
        Directory.CreateDirectory(System.IO.Path.Combine(Application.persistentDataPath, "Save"));
        string resourcesPath = System.IO.Path.Combine(Application.streamingAssetsPath, "save.json");
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
        print($"Setting Save Scene: {sceneName}");
        saveData.currentScene = sceneName;
        sceneObjectContainer.savePointID = ID;

        //Update savePoint ID in level file
        string tempPath = System.IO.Path.Combine(levelDestination, $"{sceneName}.json");
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
            //case "gasmask":
            //    saveData.abilities.gasmask = true;
            //    break;
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

    public void TakeAbility(string abilityName)
    {
        abilityName = abilityName.ToLower();
        switch (abilityName)
        {
            case "radio":
                saveData.abilities.radio = false;
                break;
            case "radio_special":
                saveData.abilities.radio_special = false;
                break;
            case "crowbar":
                saveData.abilities.crowbar = false;
                break;
            //case "gasmask":
            //    saveData.abilities.gasmask = true;
            //    break;
            case "flashlight":
                saveData.abilities.flashlight = false;
                break;
            case "mirror":
                saveData.abilities.mirror = false;
                break;
            case "book":
                saveData.abilities.book = false;
                break;
            case "hand":
                saveData.abilities.hand = false;
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

    public void CollectHealthPart()
    {
        saveData.healthParts++;
        int currentParts = saveData.healthParts;

        if (currentParts >= 3)
        {
            saveData.maxHealth++;
            saveData.healthParts = 0;
        }

        SaveFile();
    }

    public void CollectChargePart()
    {
        saveData.chargeParts++;
        int currentParts = saveData.chargeParts;

        if (currentParts >= 3)
        {
            saveData.maxCharge += 10f;
            saveData.chargeParts = 0;
        }

        SaveFile();
    }
}



[System.Serializable]
public class SaveData
{
    public string currentScene;
    public int maxHealth;
    public float maxCharge;
    public int cardLevel;
    public int healthParts, chargeParts;
    public Abilities abilities;
    public List<RadioAbility> radioAbilities;
}

[System.Serializable]
public class Abilities
{
    public bool radio;
    public bool radio_special;
    public bool crowbar;
    //public bool gasmask;
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
    public float chargeCost;
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
    public bool needItem;
    public int inventoryItemID;
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
    public List<string> text;
}