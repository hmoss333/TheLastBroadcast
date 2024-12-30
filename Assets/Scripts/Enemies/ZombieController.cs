using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(Health))]
public class ZombieController : CharacterController
{
    [SerializeField] List<GameObject> hitObjects;

    //[Header("Zombie Interact Variables")]
    public enum ZombieState { asleep, sitting, idle, following, attacking, stunned, hurt, dead }
    [SerializeField] ZombieState zombieState;

    [Header("Health Values")]
    private int maxHealth;
    Health health;

    [Header("Visibility Values")]
    [SerializeField] private float seeDist;
    [SerializeField] private float attackDist;
    [SerializeField] private float loseDist;
    [SerializeField] private float focusTime;
    private float tempFocusTime, dist;
    [SerializeField] private LayerMask layer;

    [Header("Melee Values")]
    [SerializeField] private MeleeController melee;
    [SerializeField] private int damage;
    private bool attacking;

    [Header("Respawn Values")]
    [SerializeField] private float respawnTime = 240f;
    private float tempTime = 0f;
    public bool wakeUp { get; private set; } //while false, zombie will stay in sleep state


    private void OnEnable()
    {
        if (dead) { animator.Play("Dead", 0, 1f); } //if returning to a scene where the zombie is already dead, start them at the end of the death animation
    }

    // Start is called before the first frame update
    override public void Start()
    {
        //Default values
        health = GetComponent<Health>();
        maxHealth = health.currentHealth;
        melee.damage = damage;
        storedSpeed = speed;
        tempFocusTime = focusTime;

        base.Start();
    }

    public void FixedUpdate()
    {
        col.enabled = zombieState != ZombieState.asleep && zombieState != ZombieState.sitting && zombieState != ZombieState.dead;
        rb.useGravity = zombieState != ZombieState.asleep && zombieState != ZombieState.sitting && zombieState != ZombieState.dead;
        rb.isKinematic = zombieState == ZombieState.asleep || zombieState == ZombieState.sitting || zombieState == ZombieState.dead;
        dist = Vector3.Distance(transform.position, PlayerController.instance.transform.position);

        switch (zombieState)
        {
            case ZombieState.asleep:
                if (wakeUp)
                {
                    animator.SetBool("wakeUp", true);
                    if (!isPlaying("Wake Up"))
                    {
                        SetState(ZombieState.idle);
                    }
                }
                break;
            case ZombieState.sitting:
                if (wakeUp)
                {
                    //animator.SetBool("isSitting", false);
                    if (!isPlaying("Sitting"))
                    {
                        SetState(ZombieState.idle);
                    }
                }
                break;
            case ZombieState.idle:
                if (dist <= seeDist || (seePlayer && dist <= loseDist))
                {
                    SetState(ZombieState.following);
                }
                break;
            case ZombieState.following:
                storedSpeed = !isPlaying("Move") || dist <= attackDist ? 0f : speed;
                rb.velocity = transform.forward * storedSpeed;

                //Player is within melee range
                if (dist <= attackDist)
                {
                    SetState(ZombieState.attacking);
                }
                //Player moved out of range
                if (dist >= loseDist)
                {
                    SetState(ZombieState.idle);
                }
                break;
            case ZombieState.attacking:
                if (!isPlaying("Melee"))
                {
                    animator.SetBool("isAttacking", true);
                
                    //Player is within range
                    if (dist <= seeDist)
                    {
                        SetState(ZombieState.following);
                    }
                    //Player moved out of range
                    if (!seePlayer || dist >= loseDist)
                    {
                        SetState(ZombieState.idle);
                    }
                }             
                break;
            case ZombieState.stunned:
                if (!isPlaying("Stunned"))
                {
                    animator.SetBool("isStunning", true);
                }
                break;
            case ZombieState.hurt:
                break;
            case ZombieState.dead:
                tempTime += Time.deltaTime;
                if (tempTime >= respawnTime)
                {
                    tempTime = 0f;
                    InitializeZombie();
                }
                break;
            default:
                break;
        }

        if (zombieState == ZombieState.asleep || zombieState == ZombieState.sitting || isPlaying("WakeUp") || stunned) //|| PlayerController.instance.abilityState == PlayerController.AbilityStates.invisible
        {
            seePlayer = false;
        }
        else if (!hurt && !dead)
        {
            RaycastHit[] hits;
            Vector3 forwardDir = PlayerController.instance.transform.position - transform.position;
            float distanceToPlayer = Vector3.Distance(transform.position, PlayerController.instance.transform.position);
            hits = Physics.RaycastAll(transform.position, forwardDir, seeDist, layer);

            if (dist <= seeDist || (seePlayer && dist <= loseDist))
            {
                if (hits.Length > 0 && hits[hits.Length - 1].collider.gameObject.CompareTag("Player"))
                {
                    if (!seePlayer)
                    {
                        tempFocusTime -= Time.deltaTime;
                        if (tempFocusTime <= 0f)
                        {
                            seePlayer = true;
                            tempFocusTime = focusTime;
                        }
                    }
                }
                else
                {
                    if (seePlayer)
                    {
                        tempFocusTime -= Time.deltaTime;
                        if (tempFocusTime <= 0f)
                        {
                            seePlayer = false;
                            tempFocusTime = focusTime;
                        }
                    }
                }

                if (seePlayer)
                {
                    Vector3 playerPos = new Vector3(PlayerController.instance.transform.position.x, transform.position.y, PlayerController.instance.transform.position.z);
                    if (zombieState != ZombieState.attacking)
                    {
                        transform.LookAt(playerPos);
                    }
                }

                attacking = dist <= attackDist && !attacking ? true : false;
            }
            else
            {
                seePlayer = false;
            }
        }

        //Stun Logic
        if (stunned)
        {
            seePlayer = false;
            tempStunTime -= Time.deltaTime;
            if (tempStunTime <= 0)
            {
                stunned = false;
                SetState(ZombieState.idle);
                tempStunTime = stunTime;
            }

            if (hurt || dead)
            {
                stunned = false;
                SetState(ZombieState.stunned);
            }
        }
        //Hurt state
        if (hurt)
            SetState(ZombieState.hurt);
        //Dead state
        if (dead)
            SetState(ZombieState.dead);


        if (!dead && seePlayer)
            PlayerController.instance.SeePlayer(); //is this necessary?
        animator.SetBool("seePlayer", seePlayer);
        animator.SetBool("isStunning", stunned);
        animator.SetBool("isAttacking", attacking);
        animator.SetBool("isAsleep", zombieState == ZombieState.asleep && !wakeUp);
        animator.SetBool("isSitting", zombieState == ZombieState.sitting && !wakeUp);
        melee.gameObject.SetActive(isPlaying("Melee"));
    }

    public void SetState(ZombieState state)
    {
        Debug.Log($"Set Zombie state: {state.ToString()}");
        zombieState = state;
    }

    public void InitializeZombie()
    {
        health.SetHealth(maxHealth);
        health.isHit = false;
        hurt = false;
        dead = false;
        seePlayer = false;
        stunned = false;
        attacking = false;
        wakeUp = false;
        animator.Play("WakeUp");
        SetState(ZombieState.idle);
    }

    public void WakeUp()
    {
        SetState(ZombieState.idle);
    }
}
