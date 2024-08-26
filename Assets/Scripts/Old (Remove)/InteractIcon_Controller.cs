using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractIcon_Controller : MonoBehaviour
{
    bool isActive;
    Image icon;
    [SerializeField] float fadeTime;
    [SerializeField] Sprite interactIcon;
    [SerializeField] Sprite inactiveIcon;
    [SerializeField] Sprite lockedIcon;
    [SerializeField] Sprite unlockedIcon;


    private void Start()
    {
        icon = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        try
        {
            icon.color = new Color(icon.color.r, icon.color.g, icon.color.b, Mathf.PingPong(Time.time, fadeTime) / fadeTime);
            //transform.LookAt(Camera.main.transform, PlayerController.instance.transform.up); //displaying icons reversed
            transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
        }
        catch { }
    }

    public void UpdateIcon(bool isInteracting, InteractObject interactObject)
    {
        print("Updating Interact Icon");
        bool canDisplay = false;
        if (interactObject != null)
        {
            canDisplay = !interactObject.hasActivated;

            //Update icon based on interactObject
            if (!interactObject.active)
            {
                if (interactObject.needItem)
                {
                    if (SaveDataController.instance.saveData.inventory.Exists(x => x.id == interactObject.inventoryItemID))
                        icon.sprite = unlockedIcon;
                    else
                        icon.sprite = lockedIcon;
                }
                else
                {
                    icon.sprite = inactiveIcon;
                }
            }
            else
                icon.sprite = interactIcon;
        }

        if (!isInteracting && canDisplay)
            isActive = true;
        else
            isActive = false;


        icon.enabled = isActive;
    }
}
