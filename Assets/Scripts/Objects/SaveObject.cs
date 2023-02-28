using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveObject : MonoBehaviour
{
    public string id;
    public bool active = true, hasActivated;

    //private void Awake()
    //{
    //    id = Mathf.Abs(GetHashCode()).ToString().PadLeft(6, '0');
    //}

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
