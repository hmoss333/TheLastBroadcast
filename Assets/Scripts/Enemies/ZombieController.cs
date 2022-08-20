using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class ZombieController : CharacterController
{
    [Header("Zombie Interact Variables")]
    private bool seePlayer, attacking, colliding;
    [SerializeField] private float seeDist, attackDist, loseDist, focusTime;
    [SerializeField] private float tempFocusTime;
    [SerializeField] private LayerMask layer;
    [SerializeField] private MeleeController melee;
    [SerializeField] private int damage;


    // Start is called before the first frame update
    override public void Start()
    {
        melee.damage = damage;
        storedSpeed = speed;
        tempFocusTime = focusTime;
        base.Start();
    }

    override public void Update()
    {
        base.Update();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float dist = Vector3.Distance(transform.position, PlayerController.instance.transform.position);

        if (PlayerController.instance.invisible || dead)
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
                //seePlayer = hits[0].collider.gameObject.tag == "Player" ? true : false;
                if (hits[0].collider.gameObject.tag == "Player")
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
                            colliding = false;
                            tempFocusTime = focusTime;
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

        storedSpeed = !isPlaying("Move") || dist <= attackDist ? 0f : speed;//? speed : 0f;
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
        if (collision.gameObject.layer != layer && seePlayer)
        {
            colliding = true;
        }
    }
}
