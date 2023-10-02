using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
//using static UnityEditor.Experimental.GraphView.GraphView;
//using static UnityEditor.Searcher.SearcherWindow.Alignment;

[RequireComponent(typeof(SaveObject))]
public class ZombieController : CharacterController
{
    //[Header("Zombie Interact Variables")]
    [SerializeField] private float seeDist, attackDist, loseDist, focusTime;
    private float tempFocusTime, dist;
    private bool attacking;
    [SerializeField] private LayerMask layer;
    [SerializeField] private MeleeController melee;
    [SerializeField] private int damage;

    SaveObject saveObj;
    NavMeshAgent agent;

    // Start is called before the first frame update
    override public void Start()
    {
        melee.damage = damage;
        //storedSpeed = speed;
        tempFocusTime = focusTime;
        saveObj = GetComponent<SaveObject>();

        //Add NavMeshAgent at runtime
        NavMeshAgent tempAgent = this.gameObject.AddComponent<NavMeshAgent>();
        tempAgent.baseOffset = 1f;
        tempAgent.speed = speed;
        tempAgent.angularSpeed = 0f;
        tempAgent.acceleration = 100f;
        agent = tempAgent;

        base.Start();
    }

    public void FixedUpdate()
    {
        dist = Vector3.Distance(transform.position, PlayerController.instance.transform.position);

        if (PlayerController.instance.abilityState == PlayerController.AbilityStates.invisible
            || stunned
            || !saveObj.active
            || isPlaying("Sleep") || isPlaying("WakeUp"))
        {
            seePlayer = false;
        }
        else if (!hurt && !dead)
        {
            RaycastHit[] hits;
            Vector3 forwardDir = PlayerController.instance.transform.position - transform.position;
            float distanceToPlayer = Vector3.Distance(transform.position, PlayerController.instance.transform.position);
            //Debug.DrawRay(transform.position, forwardDir, Color.red);
            hits = Physics.RaycastAll(transform.position, forwardDir, distanceToPlayer, layer);

            if (dist <= seeDist || (seePlayer && dist <= loseDist))
            {
                if (hits[0].collider.gameObject.CompareTag("Player"))
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
                    if (!isPlaying("Melee"))
                    {
                        transform.LookAt(playerPos);
                        agent.SetDestination(playerPos);
                    }
                }

                attacking = dist <= attackDist && !attacking ? true : false; //&& !isPlaying("Melee")
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
                tempStunTime = stunTime;
            }
        }


        col.enabled = !dead && saveObj.active && !saveObj.hasActivated;
        rb.useGravity = !dead && saveObj.active && !saveObj.hasActivated;

        if (!isPlaying("Sleep") && !isPlaying("WakeUp"))
        {
            agent.speed = !isPlaying("Move") || dist <= attackDist ? 0f : speed; //storedSpeed
            //rb.velocity = transform.forward * storedSpeed;
        }

        if (!dead && seePlayer)
            PlayerController.instance.SeePlayer();
        animator.SetBool("seePlayer", seePlayer);
        animator.SetBool("isStunning", stunned);
        animator.SetBool("isAttacking", attacking);
        animator.SetBool("isAsleep", !saveObj.active && !saveObj.hasActivated);
        melee.gameObject.SetActive(isPlaying("Melee"));
    }
}
