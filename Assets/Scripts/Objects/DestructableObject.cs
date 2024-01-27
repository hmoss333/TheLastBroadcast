using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class DestructableObject : SaveObject
{
    private Health health;
    AudioSource audioSource;
    [SerializeField] AudioClip deathClip;
    

    private void Start()
    {
        health = GetComponent<Health>();
        audioSource = GetComponent<AudioSource>();

        gameObject.SetActive(!hasActivated);
    }

    private void Update()
    {
        if (health.currentHealth <= 0 && !hasActivated)
        {
            audioSource.Stop();
            audioSource.clip = deathClip;
            audioSource.Play();

            SetHasActivated();
            m_OnTrigger.Invoke();
        }
    }
}
