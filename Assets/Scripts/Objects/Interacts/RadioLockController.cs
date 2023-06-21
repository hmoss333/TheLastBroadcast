using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public class RadioLockController : SaveObject
{
    [SerializeField] private bool interacting;
    [SerializeField] private bool focusOnActivate;
    [SerializeField] private float checkRadius = 4.0f; //how far away the player needs to be in order for the door control to recognize the radio signal
    [SerializeField] private float checkTime = 2f; //time the radio must stay within the frequency range to activate
    [SerializeField] private float checkFrequency; //frequency that must be matched on field radio
    [SerializeField] private float checkOffset = 0.5f; //offset amount for matching with the current field radio frequency
    [SerializeField] private MeshRenderer mesh;
    [SerializeField] private GameObject[] objectsToActivate;

    float tempTime = 0f;
    Coroutine unlockRoutine;

    void Start()
    {
        mesh.material.color = Color.red;
        checkFrequency = Random.Range(1f, 7.5f);
        
        //This is gross; need to refactor the activation logic
        if (hasActivated)
        {
            for (int i = 0; i < objectsToActivate.Length; i++)
            {
                SaveObject tempSaveObj = objectsToActivate[i].GetComponent<InteractObject>();
                if (tempSaveObj == null)
                    objectsToActivate[i].SetActive(!objectsToActivate[i].activeSelf);
            }
        }
    }

    void Update()
    {
        if (!active)
            mesh.material.color = Color.black;
        else if (active && !hasActivated && !interacting)
            mesh.material.color = Color.red;

        if (!hasActivated && active)
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
                    SetHasActivated();
                    if (unlockRoutine == null)
                        StartCoroutine(UnlockObjects());
                }
            }
            else if (interacting)
            {
                interacting = false;
                mesh.material.color = Color.red;
                tempTime = 0f;
            }
        }

        mesh.material.color = hasActivated ? Color.green : mesh.material.color;
    }

    IEnumerator UnlockObjects()
    {
        CameraController.instance.SetCamLock(true);

        if (focusOnActivate)
            yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < objectsToActivate.Length; i++)
        {
            InteractObject tempInteract = objectsToActivate[i].GetComponent<InteractObject>();
            if (focusOnActivate && objectsToActivate[i].transform.parent.gameObject.activeSelf)
            {
                CameraController.instance.SetTarget(tempInteract != null && tempInteract.focusPoint != null
                    ? tempInteract.focusPoint : objectsToActivate[i].gameObject);
                CameraController.instance.transform.position = tempInteract != null && tempInteract.focusPoint != null
                    ? tempInteract.focusPoint.transform.position : objectsToActivate[i].gameObject.transform.position;
            }

            if (tempInteract != null)
                tempInteract.Activate();
            else
                objectsToActivate[i].SetActive(!objectsToActivate[i].activeSelf);

            if (focusOnActivate)
                yield return new WaitForSeconds(1f);
        }

        CameraController.instance.SetCamLock(false);
        CameraController.instance.SetTarget(PlayerController.instance.gameObject);

        unlockRoutine = null;
    }
}
