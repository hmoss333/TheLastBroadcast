using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using static UnityEditor.Experimental.GraphView.GraphView;
//using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class ZombieController : CharacterController
{
    [Header("Zombie Interact Variables")]
    private bool seePlayer, attacking, colliding;//, stunned;
    [SerializeField] private float seeDist, attackDist, loseDist, focusTime;
    private float tempFocusTime, dist;
    [SerializeField] private LayerMask layer;
    [SerializeField] private MeleeController melee;
    [SerializeField] private int damage;

    //[Header("Stun Values")]
    //[SerializeField] private float tuneTime = 1.5f;
    //private float tuneDist, tuneFrequency, tuneOffset = 0.5f;
    //[SerializeField] private float stunTime;
    //float tempTuneTime, tempStunTime;


    // Start is called before the first frame update
    override public void Start()
    {
        melee.damage = damage;
        storedSpeed = speed;
        tempFocusTime = focusTime;
        //tempTuneTime = tuneTime;
        //tempStunTime = stunTime;
        //tuneDist = seeDist * 1.25f;
        //tuneFrequency = Random.Range(1.5f, 7.5f);

        base.Start();
    }

    //override public void Update()
    //{
    //    if (!stunned)
    //    {
    //        if (dist <= tuneDist
    //            && (RadioController.instance.currentFrequency < tuneFrequency + tuneOffset && RadioController.instance.currentFrequency > tuneFrequency - tuneOffset)
    //            && !RadioController.instance.abilityMode //ability mode is not active                                                       
    //            && RadioController.instance.isActive) //is the radio active (shouldn't be broadcasting if it is not turned on))
    //        {
    //            //TODO add animator toggle here to play "stunning" animation
    //            animator.SetBool("isStunning", true);
    //            RadioController.instance.StunEnemy(true);
    //            tempTuneTime -= Time.deltaTime;
    //            if (tempTuneTime <= 0)
    //            {
    //                stunned = true;
    //                tempTuneTime = tuneTime;
    //            }
    //        }
    //        else
    //        {
    //            //TODO add animator toggle here to stop "stunning" animation
    //            animator.SetBool("isStunning", false);
    //            RadioController.instance.StunEnemy(false);
    //            tempTuneTime = tuneTime;
    //        }
    //    }

    //    if (stunned)
    //    {
    //        tempStunTime -= Time.deltaTime;
    //        if (tempStunTime <= 0)
    //        {
    //            stunned = false;
    //            tempStunTime = stunTime;
    //        }
    //    }

    //    base.Update();
    //}

    // Update is called once per frame
    void FixedUpdate()
    {
        dist = Vector3.Distance(transform.position, PlayerController.instance.transform.position);

        if (PlayerController.instance.invisible || dead)// || stunned)
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

        storedSpeed = !isPlaying("Move") || dist <= attackDist ? 0f : speed;
        rb.velocity = transform.forward * storedSpeed;

        PlayerController.instance.isSeen = seePlayer;
        animator.SetBool("seePlayer", seePlayer);
        animator.SetBool("isAttacking", attacking);
        //animator.SetBool("isStunned", stunned);
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
