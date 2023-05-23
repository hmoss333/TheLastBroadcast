using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class OnDeathTrigger : MonoBehaviour
{
    [SerializeField] private bool triggered, focusOnActivate;
    [SerializeField] private GameObject[] objectsToActivate;
    Health objHealth;
    Coroutine triggerObjs;

    private void Start()
    {
        objHealth = GetComponent<Health>();
    }

    private void Update()
    {
        if (objHealth.CurrentHealth() <= 0 && !triggered)
        {
            triggered = true;
            TriggerObjects();
        }
    }

    void TriggerObjects()
    {
        if (triggerObjs == null)
            StartCoroutine(TriggerObjectsRoutine());
    }

    IEnumerator TriggerObjectsRoutine()
    {
        CameraController.instance.SetCamLock(true);

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

            yield return new WaitForSeconds(2f);
        }

        CameraController.instance.SetCamLock(false);

        if (CameraController.instance.GetTarget() != PlayerController.instance.transform)
        {
            CameraController.instance.LoadLastTarget();
            CameraController.instance.transform.position = CameraController.instance.GetLastTarget().position;
        }

        triggerObjs = null;
        this.enabled = false;
        //this.gameObject.SetActive(false);
    }
}
