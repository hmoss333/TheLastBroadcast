using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadioLockController : InteractObject
{
    [SerializeField] bool isActive;
    [SerializeField] float checkDist;
    [SerializeField] float stationVal;
    [SerializeField] float stationOffset;

    PlayerController player;
    [SerializeField] Renderer activeMat;

    [SerializeField] InteractObject[] objectsToActivate;


    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        bool playerInRange = PlayerInRange();
        float currentFrequency = RadioOverlay_Controller.instance.currentFrequency;

        if (activated && playerInRange && !isActive && RadioOverlay_Controller.instance.isActive)
        {
            StartScan(currentFrequency);
        }

        if (!activated)
            activeMat.material.color = Color.black;
        else
            activeMat.material.color = isActive ? Color.green : Color.red;
    }

    bool PlayerInRange()
    {
        float playerDist = Vector3.Distance(transform.position, player.gameObject.transform.position);
        if (playerDist <= checkDist)
        {
            return true;
        }

        return false;
    }

    void StartScan(float currentFrequency)
    {
        if (currentFrequency >= stationVal - stationOffset && currentFrequency <= stationVal + stationOffset)
        {
            isActive = true;
            PlayerController.instance.RadioToggle();
            StartCoroutine(ActivateObjects());
        }
        else
        {
            isActive = false;
        }
    }

    IEnumerator ActivateObjects()
    {
        //Pause user input for brief moment
        PlayerController.instance.interacting = true;
        yield return new WaitForSeconds(0.65f);
        PlayerController.instance.interacting = false;


        //Activate all interact objects in list
        for (int i = 0; i < objectsToActivate.Length; i++)
        {
            CameraController.instance.SetTarget(objectsToActivate[i].gameObject);

            yield return new WaitForSeconds(1f);

            objectsToActivate[i].Activate();
            

            yield return new WaitForSeconds(1f);
        }

        //Reset camera to player
        CameraController.instance.SetTarget(player.gameObject);
    }
}
