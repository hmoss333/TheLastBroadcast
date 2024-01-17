using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public float speed, stunTime;
    [HideInInspector] public float storedSpeed, tempStunTime;
    [HideInInspector] public bool hurt, dead, stunned, seePlayer;
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public Collider col;
    [HideInInspector] public Animator animator;


    virtual public void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        animator = GetComponentInChildren<Animator>();
        tempStunTime = stunTime;
    }

    virtual public void Update()
    {
        if (dead && !isPlaying("Dead"))
        {
            animator.SetTrigger("isDead");
            try
            {
                rb.useGravity = false;
                rb.isKinematic = true;
                col.enabled = false;
            }
            catch { }
        }
        else if (hurt && !isPlaying("Hurt"))
        {
            print($"{gameObject.name} is hurt");
            hurt = false;
            animator.SetTrigger("isHurt");
        }
    }

    public bool isPlaying(string stateName)
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName(stateName))
            return true;
        else
            return false;
    }

    public float GetClipLength(string stateName)
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName(stateName))
            return animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
        else
        {
            print($"Clip for {stateName} not found");
            return 0;
        }
    }

    public void StunCharacter()
    {
        stunned = true;
    }

    public bool SeePlayer()
    {
        return seePlayer;
    }

    //WakeUp animation delay
    public void PauseAnimation(float delayTime)
    {
        StartCoroutine(PauseAnimationRoutine(delayTime));
    }

    IEnumerator PauseAnimationRoutine(float delayTime)
    {
        animator.speed = 0f;

        yield return new WaitForSeconds(delayTime);

        animator.speed = 1f;
    }
}
