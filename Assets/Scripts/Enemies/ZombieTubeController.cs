using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;



public class ZombieTubeController : MonoBehaviour
{
    [SerializeField] private int triggerNum;

    [Header("Avatar References")]
    [NaughtyAttributes.HorizontalLine]
    [SerializeField] Animator avatarBody;

    [Header("Audio Variables")]
    [SerializeField] AudioClip hurtClip;
    [SerializeField] AudioClip deathClip;
    [SerializeField] AudioClip electricClip;
    AudioSource audioSource;
    [SerializeField] AudioSource electricSource;


    [Header("On Death Triggers")]
    public UnityEvent m_OnTrigger = new UnityEvent();

    // Start is called before the first frame update
    void Start()
    {
        //character = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();
    }

    public void Hurt()
    {
        triggerNum--;
        CamEffectController.instance.ShockEffect(1f);
        if (electricClip != null) { electricSource.Stop(); electricSource.clip = electricClip; electricSource.Play(); }

        if (triggerNum > 0)
        {
            avatarBody.SetTrigger("isHurt");
            if (hurtClip != null) { audioSource.Stop(); audioSource.clip = hurtClip; audioSource.Play(); }
        }
        else
        {
            triggerNum = 0;
            avatarBody.SetTrigger("isDead");
            if (deathClip != null) { audioSource.Stop(); audioSource.clip = deathClip; audioSource.Play(); }

            m_OnTrigger.Invoke();
        }
    }
}
