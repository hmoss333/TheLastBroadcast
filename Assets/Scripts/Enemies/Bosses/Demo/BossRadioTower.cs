using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;


[RequireComponent(typeof(Health))]
[RequireComponent(typeof(OnDeathTrigger))]
public class BossRadioTower : MonoBehaviour
{
    //[SerializeField]
    private bool active, interacting, triggered, hit;
    [SerializeField] private float checkRadius = 5.0f; //how far away the player needs to be in order for the door control to recognize the radio signal
    [SerializeField] private float checkTime = 1.5f; //time the radio must stay within the frequency range to activate
    [SerializeField] private float checkFrequency; //frequency that must be matched on field radio
    [SerializeField] private float checkOffset = 0.5f; //offset amount for matching with the current field radio frequency
    [SerializeField] private MeshRenderer mesh;
    public Transform focusPoint;
    [SerializeField] private GameObject barrier;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip barrierOnClip, barrierOffClip;
    private Collider col;
    private Health health;
    private OnDeathTrigger onDeathTrigger;
    float tempTime = 0f;
    [SerializeField] BossZombieController bossController;


    Coroutine hitRoutine;


    private void Start()
    {
        bossController = GameObject.FindObjectOfType<BossZombieController>();
        col = GetComponent<Collider>();
        health = GetComponent<Health>();
        onDeathTrigger = GetComponent<OnDeathTrigger>();
        //audioSource.GetComponent<AudioSource>();
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
                //CamEffectController.instance.SetEffectState(true);
                tempTime += Time.deltaTime;
                if (tempTime >= checkTime)
                {
                    triggered = true;
                    //bossController.SetTower();
                    barrier.SetActive(false);
                    PlayClip(barrierOffClip);
                }
            }
            else if (interacting)
            {
                interacting = false;
                mesh.material.color = Color.red;
                tempTime = 0f;
            }
        }

        mesh.material.color = triggered ? hit ? Color.black
                                            : Color.green
                                        : active ? interacting
                                                ? Color.yellow
                                                : Color.red
                                        : Color.black;

        if (health.currentHealth <= 0) { mesh.material.color = Color.black; }
        col.enabled = !barrier.activeSelf;
    }

    public bool GetActiveState()
    {
        return active;
    }

    public void Activate()
    {
        active = true;
        barrier.SetActive(true);
        PlayClip(barrierOnClip);
        health.SetHealth(2);
        health.isHit = false;
        onDeathTrigger.enabled = true;
        onDeathTrigger.triggered = false;
        checkFrequency = Random.Range(1.5f, 9f);
    }

    public void DeActivate()
    {
        active = false;
        barrier.SetActive(false);
        triggered = false;
    }

    public void OnHit(float time)
    {
        if (hitRoutine == null)
            hitRoutine = StartCoroutine(OnHitRoutine(time));
    }

    IEnumerator OnHitRoutine(float time)
    {
        hit = true;
        yield return new WaitForSeconds(time);
        hit = false;

        hitRoutine = null;
    }

    void PlayClip(AudioClip clip)
    {
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();
    }
}
