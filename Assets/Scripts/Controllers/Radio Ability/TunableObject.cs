using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TunableObject : RadioLockController
{
    [SerializeField] Material baseMat;
    [SerializeField] Material staticMat;

    private void Start()
    {
        baseMat = mr.material;
        //checkFrequency = TuneAbility.instance.abilityData.frequency;
    }

    private void Update()
    {
        if (!unlocked && active)
        {         
            checkFrequency = TuneAbility.instance.abilityData.frequency;
            float dist = Vector3.Distance(transform.position, PlayerController.instance.transform.position);

            if (dist <= checkRadius
                && (RadioController.instance.currentFrequency < checkFrequency + checkOffset && RadioController.instance.currentFrequency > checkFrequency - checkOffset)
                && RadioController.instance.abilityMode //ability mode is not active                                                       
                && RadioController.instance.isActive) //is the radio active (shouldn't be broadcasting if it is not turned on))
            {
                interacting = true;
                checkTime -= Time.deltaTime;
                if (checkTime < 0)
                {
                    unlocked = true;
                    hasActivated = true;
                }
            }
            else
            {
                interacting = false;             
                checkTime = 2f;
            }
        }

        if (hasActivated && !unlocked)
            unlocked = true;

        GetComponentInChildren<InteractObject>().active = unlocked ? true : false;
        mr.material = unlocked ? baseMat : staticMat;
    }
}
