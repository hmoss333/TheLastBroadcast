using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class OnDeathTrigger : MonoBehaviour
{
    [SerializeField] private bool triggered;
    [SerializeField] private float activateTime;
    [SerializeField] private SaveObject[] objectsToActivate;
    Health objHealth;
    Coroutine triggerObjs;

    private void Start()
    {
        objHealth = GetComponent<Health>();
    }

    private void Update()
    {
        if (objHealth.currentHealth <= 0 && !triggered)
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
        for (int i = 0; i < objectsToActivate.Length; i++)
        {
            if (objectsToActivate[i] != null)
                objectsToActivate[i].Activate();

            yield return new WaitForSeconds(activateTime);
        }

        triggerObjs = null;
        this.enabled = false;
    }
}
