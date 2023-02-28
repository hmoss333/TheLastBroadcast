using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveObject : MonoBehaviour
{
    public bool active = true, hasActivated;

    public virtual void Activate()
    {
        active = !active;
        print($"Set active state of {gameObject.name} to {active}");
        SaveDataController.instance.SaveObjectData(SceneManager.GetActiveScene().name);
    }

    public virtual void SetHasActivated()
    {
        hasActivated = true;
        SaveDataController.instance.SaveObjectData(SceneManager.GetActiveScene().name);
    }
}
