using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(IDGenerator))]
public class SaveObject : MonoBehaviour
{
    public string id;
    public bool active = true, hasActivated, hideOnLoad = false, needItem = false;
    public int inventoryItemID;
    [HideInInspector]
    public ItemInstance inventoryItem;


    private void Start()
    {
        if ((hasActivated || !active) && hideOnLoad)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        if ((hasActivated || !active) && hideOnLoad)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnValidate()
    {
        id = GetComponent<IDGenerator>().ID;
    }

    public virtual void Activate()
    {
        active = !active;

        if (active && !gameObject.activeSelf)
            gameObject.SetActive(true);

        if (hideOnLoad && !active)
            gameObject.SetActive(false);
    }

    public virtual void SetHasActivated()
    {
        hasActivated = true;

        if (hideOnLoad)
            gameObject.SetActive(false);
    }
}
