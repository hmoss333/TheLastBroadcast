using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractIcon_Controller : MonoBehaviour
{
    bool isActive;
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
        bool canDisplay = false;
        if (interactObject != null)
            canDisplay = !interactObject.hasActivated;

        if (!isInteracting && canDisplay)
            isActive = true;
        else
            isActive = false;

        icon.enabled = isActive;
    }
}
