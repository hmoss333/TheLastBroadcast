using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using static UnityEditor.Experimental.GraphView.GraphView;
//using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class ZombieController : CharacterController
{
    [Header("Zombie Interact Variables")]
    [SerializeField] private bool seePlayer, attacking, colliding, stunned;
    [SerializeField] private float seeDist, attackDist, loseDist, focusTime;
    private float tempFocusTime, dist;
    [SerializeField] private LayerMask layer;
    [SerializeField] private MeleeController melee;
    [SerializeField] private int damage;

    [Header("Stun Values")]
    [SerializeField] private float stunTime;
    float tempStunTime;


    // Start is called before the first frame update
    override public void Start()
    {
        melee.damage = damage;
        storedSpeed = speed;
        tempFocusTime = focusTime;
        tempStunTime = stunTime;

        base.Start();
    }

    override public void Update()
    {
        dist = Vector3.Distance(transform.position, PlayerController.instance.transform.position);

        if (PlayerController.instance.invisible || dead || stunned)
        {
            seePlayer = false;
        }
        else if (!hurt && !dead)
        {
            RaycastHit[] hits;
            Vector3 forwardDir = PlayerController.instance.transform.position - transform.position;
            float distanceToPlayer = Vector3.Distance(transform.position, PlayerController.instance.transform.position);
            Debug.DrawRay(transform.position, forwardDir, Color.red);
            hits = Physics.RaycastAll(transform.position, forwardDir, distanceToPlayer, layer);

            if (dist <= seeDist || (seePlayer && dist <= loseDist))
            {
                if (hits[0].collider.gameObject.CompareTag("Player"))
                {
                    if (!seePlayer)
                    {
                        tempFocusTime += Time.deltaTime;
                        if (tempFocusTime >= focusTime)
                        {
                            seePlayer = true;
                            tempFocusTime = 0f;
                        }
                    }
                }
                else
                {
                    if (seePlayer)
                    {
                        tempFocusTime += Time.deltaTime;
                        if (tempFocusTime >= focusTime)
                        {
                            seePlayer = false;
                            colliding = false;
                            tempFocusTime = 0f;
                        }
                    }
                }

                if (seePlayer)
                {
                    Vector3 playerPos = new Vector3(PlayerController.instance.transform.position.x, transform.position.y, PlayerController.instance.transform.position.z);
                    if (!isPlaying("Melee"))
                        transform.LookAt(playerPos);
                }

                attacking = dist <= attackDist && !attacking && !isPlaying("Melee") ? true : false;
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
            animator.SetBool("isStunning", true);
            tempStunTime += Time.deltaTime;
            if (tempStunTime >= stunTime)
            {
                stunned = false;
                tempStunTime = 0;
            }
        }
        else
        {
            animator.SetBool("isStunning", false);
        }

        base.Update();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        storedSpeed = !isPlaying("Move") || dist <= attackDist ? 0f : speed;
        rb.velocity = transform.forward * storedSpeed;

        PlayerController.instance.isSeen = seePlayer;
        animator.SetBool("seePlayer", seePlayer);
        animator.SetBool("isAttacking", attacking);
        melee.gameObject.SetActive(isPlaying("Melee"));

        if (colliding)
            animator.SetBool("isAttacking", true);
    }

    public bool SeePlayer()
    {
        return seePlayer;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer != LayerMask.NameToLayer("Floor") && seePlayer)
        {
            colliding = true;
        }
    }

    public void Stun()
    {
        stunned = true;
    }
}
