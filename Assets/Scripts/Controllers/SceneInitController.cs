using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneInitController : MonoBehaviour
{
    SaveData saveData;

    [SerializeField] SavePointController[] savePoints;

    // Start is called before the first frame update
    void Start()
    {
        saveData = SaveDataController.instance.ReturnSaveData();

        InitializeGame();
    }

    void InitializeGame()
    {
        if (saveData.savePointID != 0)
        {
            SavePointController[] tempArray = FindObjectsOfType<SavePointController>();
            savePoints = tempArray;
            foreach (SavePointController point in savePoints)
            {
                if (point.ID == saveData.savePointID)
                {
                    PlayerController.instance.transform.position = point.initPoint.position;
                    PlayerController.instance.transform.rotation = point.initPoint.rotation;
                    break;
                }
            }
        }
    }
}
