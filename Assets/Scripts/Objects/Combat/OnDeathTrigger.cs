using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

[RequireComponent(typeof(Health))]
public class OnDeathTrigger : MonoBehaviour
{
    [SerializeField] public bool triggered;
    [SerializeField] private float activateTime;
    Health objHealth;

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
            m_OnTrigger.Invoke();
            this.enabled = false;
        }
    }
}
