using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

[RequireComponent(typeof(AudioSource))]
public class RadioLockController : SaveObject
{
    private bool interacting;
    [SerializeField] private float checkRadius = 4.0f; //how far away the player needs to be in order for the door control to recognize the radio signal
    [SerializeField] private float checkTime = 2f; //time the radio must stay within the frequency range to activate
    [SerializeField] private float checkFrequency; //frequency that must be matched on field radio
    [SerializeField] private float checkOffset = 0.5f; //offset amount for matching with the current field radio frequency
    [SerializeField] private float unlockTime = 0.5f;
    [SerializeField] private MeshRenderer mesh;
    public AudioClip interactClip;
    AudioSource interactSource;

    float tempTime = 0f;
    Coroutine unlockRoutine;

    void Start()
    {
        mesh.material.color = Color.red;
        checkFrequency = Random.Range(1f, 7.5f);
        interactSource = GetComponent<AudioSource>();
        interactSource.clip = interactClip;
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
                if (!CameraController.instance.GetRotState())
                    CameraController.instance.SetTarget(this.transform); //If the radio is set to the correct station, focus on tunable object

                interacting = true;
                mesh.material.color = Color.yellow;
                tempTime += Time.deltaTime;
                if (tempTime >= checkTime)
                {
                    SetHasActivated();
                    Unlock();                   
                }
            }
            else if (interacting)
            {
                if (!CameraController.instance.GetRotState() || !CameraController.instance.GetTriggerState())
                    CameraController.instance.LoadLastTarget(); //If the radio is set to the correct station, focus on tunable object

                interacting = false;
                mesh.material.color = Color.red;
                tempTime = 0f;
            }
        }

        mesh.material.color = hasActivated ? Color.green : mesh.material.color;
    }

    public void Lock()
    {
        active = false;
        Activate();
        hasActivated = false;
    }

    public void Unlock()
    {
        if (unlockRoutine == null)
            unlockRoutine = StartCoroutine(UnlockObjects());
    }

    IEnumerator UnlockObjects()
    {
        SetHasActivated();

        interactSource.Play();

        yield return new WaitForSeconds(unlockTime);

        m_OnTrigger.Invoke();
       
        if (!CameraController.instance.GetRotState() || !CameraController.instance.GetTriggerState())
            CameraController.instance.LoadLastTarget();

        unlockRoutine = null;
    }
}
