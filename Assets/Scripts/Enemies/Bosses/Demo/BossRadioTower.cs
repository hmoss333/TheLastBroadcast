using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using NaughtyAttributes;


public class BossRadioTower : MonoBehaviour
{
    [Header("Radio Values")]
    [NaughtyAttributes.HorizontalLine]
    [SerializeField] private float checkRadius = 5.0f; //how far away the player needs to be in order for the door control to recognize the radio signal
    [SerializeField] private float checkTime = 1.5f; //time the radio must stay within the frequency range to activate
    [SerializeField] private float checkFrequency; //frequency that must be matched on field radio
    [SerializeField] private float checkOffset = 0.5f; //offset amount for matching with the current field radio frequency
    //[SerializeField]
    private bool active, interacting, triggered, hit;
    [SerializeField] int activateCount = 3;

    [Header("Object References")]
    [NaughtyAttributes.HorizontalLine]
    [SerializeField] private MeshRenderer mesh;
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
        bossController = GameObject.FindObjectOfType<BossZombieController>();
        mesh.material.color = Color.black;
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
                    activateCount--;
                    tempTime = 0f;

                    PlayClip(barrierOffClip);
                    electricParticles.Play();

                    if (activateCount > 0)
                    {
                        checkFrequency = Random.Range(1.5f, 9f);
                        triggered = false;
                    }
                    else
                    {
                        flickerController.StartFlicker();
                        sparkParticles.Play();
                        bossController.SetTower();
                    }
                }
            }
            else if (interacting)
            {
                interacting = false;
                mesh.material.color = Color.red;
                tempTime = 0f;
            }
        }

        mesh.material.color = triggered ? Color.green
                                        : active ? interacting
                                                ? Color.yellow
                                                : Color.red
                                        : Color.black;
    }

    public bool GetActiveState()
    {
        return active;
    }

    public void Activate()
    {
        active = true;
        activateCount = 3;
        barrier.SetActive(true);
        PlayClip(barrierOnClip);
        checkFrequency = Random.Range(1.5f, 9f);
    }

    public void DeActivate()
    {
        active = false;
        barrier.SetActive(true);
        triggered = false;
    }

    void PlayClip(AudioClip clip)
    {
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();
    }
}
