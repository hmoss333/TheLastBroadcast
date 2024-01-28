using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

//[RequireComponent(typeof(ParticleSystem))]
public class Health : MonoBehaviour
{
    public int currentHealth;// { get; private set; }
    [SerializeField] bool shockEffect;
    public bool isHit { get; private set; }
    [SerializeField] float cooldownTime = 1f;
    CharacterController character;
    [SerializeField] Transform bloodTransform;
    [SerializeField] GameObject bloodPrefab;
    AudioSource audioSource;
    [SerializeField] AudioClip hitClip, deathClip;


    [FormerlySerializedAs("onTrigger")]
    [SerializeField]
    private UnityEvent m_OnTrigger = new UnityEvent();

    private void Start()
    {
        character = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();
    }

    public void Hurt(int value, bool stagger)
    {
        if (!isHit)
        {
            isHit = true;
            currentHealth -= value;
            m_OnTrigger.Invoke();

            if (hitClip != null) { audioSource.Stop(); audioSource.clip = hitClip; audioSource.Play(); }
            if (shockEffect) { CamEffectController.instance.ShockEffect(0.25f); }

            if (currentHealth <= 0)
            {
                if (character != null)
                    character.dead = true;

                AudioController.instance.LoopClip(false);
                AudioController.instance.PlayClip(deathClip);
            }
            else
            {
                if (character != null && stagger == true)
                {
                    character.hurt = true;
                    if (bloodPrefab != null) { GameObject bloodInstance = Instantiate(bloodPrefab, bloodTransform.position, gameObject.transform.rotation); }
                }

                if (character != null && character.tag == "Player")
                    ScrollHealth.instance.toggled = true;

                StartCoroutine(HitCooldown(cooldownTime));
            }
        }
    }

    public void ModifyHealth(int value)
    {
        currentHealth += value;
        print($"{gameObject.name} health = {currentHealth}");

        if (shockEffect)
        {
            CamEffectController.instance.ShockEffect(0.25f);
        }

        if (character != null && currentHealth <= 0)
        {
            character.dead = true;
        }
    }

    IEnumerator HitCooldown(float timer)
    {
        yield return new WaitForSeconds(timer);

        isHit = false;
    }

    public void SetHealth(int value)
    {
        currentHealth = value;
    }
}
