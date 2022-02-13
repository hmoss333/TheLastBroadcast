using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SavePointController : InteractObject
{
    public int ID;
    public Transform initPoint;

    public override void Interact()
    {
        SaveDataController.instance.SetSavePoint(SceneManager.GetActiveScene().name, ID);
        SaveDataController.instance.SaveFile();
    }
}
