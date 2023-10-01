using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using NaughtyAttributes;

[RequireComponent(typeof(IDGenerator))]
public class SaveObject : MonoBehaviour
{
    public string id;
    public bool active = true, hasActivated, focusOnActivate = false, hideOnLoad = false, needItem = false;
    public int inventoryItemID;

    [NaughtyAttributes.HorizontalLine]

    [Header("Event Triggers")]
    [FormerlySerializedAs("onTrigger")]
    public UnityEvent m_OnTrigger = new UnityEvent();


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

        if (focusOnActivate)
            CameraController.instance.AddObjToFocus(this);
    }

    public virtual void SetHasActivated()
    {
        hasActivated = true;

        if (hideOnLoad)
            gameObject.SetActive(false);
    }
}
