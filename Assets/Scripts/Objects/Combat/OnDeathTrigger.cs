using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

[RequireComponent(typeof(Health))]
public class OnDeathTrigger : MonoBehaviour
{
    [SerializeField] private bool triggered;
    [SerializeField] private float activateTime;
    [SerializeField] private SaveObject[] objectsToActivate;
    Health objHealth;
    Coroutine triggerObjs;

    [FormerlySerializedAs("onTrigger")]
    [SerializeField]
    private UnityEvent m_OnTrigger = new UnityEvent();

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

    public void InstantiateObj(GameObject obj)
    {
        GameObject tempObj = Instantiate(obj, transform.position, Quaternion.identity);
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

        m_OnTrigger.Invoke();

        triggerObjs = null;
        this.enabled = false;
    }
}
