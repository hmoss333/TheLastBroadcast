using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(IDGenerator))]
public class SaveObject : MonoBehaviour
{
    public string id;
    public bool active = true, hasActivated;


    private void OnValidate()
    {
        id = GetComponent<IDGenerator>().ID;
    }

    public virtual void Activate()
    {
        active = !active;
        print($"Set active state of {gameObject.name} to {active}");
        //SaveDataController.instance.SaveObjectData();
    }

    public virtual void SetHasActivated()
    {
        hasActivated = true;
        print($"Set hasActivated state of {gameObject.name} to {hasActivated}");
        //SaveDataController.instance.SaveObjectData();
    }
}
