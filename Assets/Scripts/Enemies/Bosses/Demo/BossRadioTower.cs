using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using NaughtyAttributes;


[RequireComponent(typeof(Health))]
[RequireComponent(typeof(OnDeathTrigger))]
public class BossRadioTower : MonoBehaviour
{
    [Header("Radio Values")]
    [NaughtyAttributes.HorizontalLine]
    [SerializeField] private float checkRadius = 5.0f; //how far away the player needs to be in order for the door control to recognize the radio signal
    [SerializeField] private float checkTime = 1.5f; //time the radio must stay within the frequency range to activate
    [SerializeField] private float checkFrequency; //frequency that must be matched on field radio
    [SerializeField] private float checkOffset = 0.5f; //offset amount for matching with the current field radio frequency
    //[SerializeField]
    private bool active, interacting, triggered;

    [Header("Object References")]
    [NaughtyAttributes.HorizontalLine]
    [SerializeField] private MeshRenderer mesh;
    private Collider collider;
    [SerializeField] private Health health;
    private OnDeathTrigger onDeathTrigger;
    [SerializeField] private GameObject barrier;
    [SerializeField] private ObjectFlicker flickerController;
    public Transform focusPoint;
    [SerializeField] ParticleSystem electricParticles, sparkParticles;

    [Header("Audio Values")]
    [NaughtyAttributes.HorizontalLine]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip barrierOnClip, barrierOffClip;

    float tempTime = 0f;

    [Header("Boss Controller")]
    [NaughtyAttributes.HorizontalLine]
    [SerializeField] BossZombieController bossController;


    Coroutine hitRoutine;


    private void Start()
    {
        bossController = FindObjectOfType<BossZombieController>();
        mesh.material.color = Color.black;
        health = GetComponent<Health>();
        onDeathTrigger = GetComponent<OnDeathTrigger>();
        collider = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (active && !triggered)
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
                    triggered = true;
                    tempTime = 0f;

                    PlayClip(barrierOffClip);
                    electricParticles.Play();
                    flickerController.StartFlicker();
                }
            }
            else if (interacting)
            {
                interacting = false;
                mesh.material.color = Color.red;
                tempTime = 0f;
            }
        }

        collider.enabled = !barrier.activeSelf;
        if (hitRoutine == null)
        {
            mesh.material.color = triggered
                                    ? Color.green
                                    : active
                                        ? interacting
                                            ? Color.yellow
                                            : Color.red
                                        : Color.black;

            if (health.currentHealth == 0)
                mesh.material.color = Color.black;
        }
    }

    public bool GetActiveState()
    {
        return active;
    }

    public void Activate()
    {
        active = true;
        barrier.SetActive(true);
        health.SetHealth(2);
        onDeathTrigger.enabled = true;
        onDeathTrigger.triggered = false;
        health.isHit = false;
        PlayClip(barrierOnClip);
        checkFrequency = Random.Range(1.5f, 9f);
    }

    public void DeActivate()
    {
        active = false;
        barrier.SetActive(true);
        collider.enabled = false;
        triggered = false;
    }

    public void TowerHit()
    {
        sparkParticles.Play();
        bossController.SetTower();
    }

    void PlayClip(AudioClip clip)
    {
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();
    }

    public void FlashHit(float waitTime)
    {
        if (hitRoutine == null)
            hitRoutine = StartCoroutine(FlashHitRoutine(waitTime));
    }

    IEnumerator FlashHitRoutine(float waitTime)
    {
        mesh.material.color = Color.red;

        yield return new WaitForSeconds(waitTime);

        mesh.material.color = Color.green;

        hitRoutine = null;
    }
}
