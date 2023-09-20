using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractIcon_Controller : MonoBehaviour
{
    bool isActive;
    bool interacting;
    InteractObject targetObj;
    Image icon;
    [SerializeField] float fadeTime;


    private void Start()
    {
        icon = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        icon.color = new Color(icon.color.r, icon.color.g, icon.color.b, Mathf.PingPong(Time.time, fadeTime)/fadeTime);
        transform.LookAt(Camera.main.transform);
    }

    public void UpdateIcon(bool isInteracting, InteractObject interactObject)
    {
        interacting = isInteracting;
        targetObj = interactObject;

        bool canDisplay = false;
        if (interactObject != null)
            canDisplay = interactObject.active;

        if (targetObj != null && !interacting && canDisplay)
            isActive = true;
        else
            isActive = false;

        icon.enabled = isActive;
    }
}
