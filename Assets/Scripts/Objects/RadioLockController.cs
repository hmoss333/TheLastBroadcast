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
    [SerializeField] private MeshRenderer mr;
    [SerializeField] private InteractObject[] objectsToActivate;

    void Start()
    {
        mr.material.color = Color.red;
        checkFrequency = Random.Range(1f, 7.5f);
    }

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
                mr.material.color = Color.yellow;
                checkTime -= Time.deltaTime;
                if (checkTime < 0)
                {
                    UnlockDoor();
                }
            }
            else
            {
                interacting = false;
                mr.material.color = Color.red;
                checkTime = 2f;
            }
        }

        if (hasActivated && !unlocked)
            unlocked = true;

        mr.material.color = unlocked ? Color.green : mr.material.color;
    }

    void UnlockDoor()
    {
        unlocked = true;
        hasActivated = true;
        for (int i = 0; i < objectsToActivate.Length; i++)
        {
            objectsToActivate[i].Activate();
        }
    }
}
