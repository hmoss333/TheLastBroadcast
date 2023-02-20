using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadioLockController : InteractObject
{
    [SerializeField] private bool unlocked = false; //has controller been triggered
    [SerializeField] private float checkRadius = 4.0f; //how far away the player needs to be in order for the door control to recognize the radio signal
    [SerializeField] private float checkTime = 2f; //time the radio must stay within the frequency range to activate
    [SerializeField] private float checkFrequency; //frequency that must be matched on field radio
    [SerializeField] private float checkOffset = 0.5f; //offset amount for matching with the current field radio frequency
    //[SerializeField] private GameObject focusPoint;
    [SerializeField] private MeshRenderer mesh;
    [SerializeField] private GameObject[] objectsToActivate;//InteractObject[] objectsToActivate;
    float tempTime = 0f;

    void Start()
    {
        mesh.material.color = Color.red;
        checkFrequency = Random.Range(1f, 7.5f);
        hasActivated = false;
    }

    //private void OnEnable()
    //{
    //    //If radioLock has already been triggered previously, interact with all objectsToActivate
    //    if (hasActivated)
    //    {
    //        StartCoroutine(UnlockObjects(false));
    //        unlocked = true;
    //    }
    //}

    void Update()
    {
        if (!unlocked && active)
        {
            float dist = Vector3.Distance(transform.position, PlayerController.instance.transform.position);

            if (dist <= checkRadius
                && (RadioController.instance.currentFrequency < checkFrequency + checkOffset && RadioController.instance.currentFrequency > checkFrequency - checkOffset)
                && SaveDataController.instance.saveData.abilities.radio == true //does the player have the radio object; useful if the player loses the radio at some point)
                && !RadioController.instance.abilityMode //ability mode is not active                                                       
                && RadioController.instance.isActive) //is the radio active (shouldn't be broadcasting if it is not turned on))
            {
                interacting = true;
                mesh.material.color = Color.yellow;
                tempTime += Time.deltaTime;
                if (tempTime >= checkTime)
                {
                    hasActivated = true;
                    //SetHasActivated(); //Not using this so the player must reactivate lock when returning to areas
                }
            }
            else if (interacting)
            {
                interacting = false;
                mesh.material.color = Color.red;
                tempTime = 0f;
            }
        }

        if (!unlocked && hasActivated)
        {
            StartCoroutine(UnlockObjects(true));
            unlocked = true;
        }

        mesh.material.color = unlocked ? Color.green : mesh.material.color;
    }

    IEnumerator UnlockObjects(bool focusCamera)
    {
        print("unlocking objects");
        for (int i = 0; i < objectsToActivate.Length; i++)
        {
            if (focusCamera)
                CameraController.instance.SetTarget(objectsToActivate[i].gameObject);

            InteractObject tempInteract = objectsToActivate[i].GetComponent<InteractObject>();
            if (tempInteract != null)
                tempInteract.Activate();
            else
                objectsToActivate[i].SetActive(!objectsToActivate[i].activeSelf);

            yield return new WaitForSeconds(1.25f);
        }

        if (CameraController.instance.GetLastTarget() != null)
            CameraController.instance.LoadLastTarget();
        else
            CameraController.instance.SetTarget(PlayerController.instance.gameObject);
    }
}
