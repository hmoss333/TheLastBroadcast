using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieController : CharacterController
{
    [Header("Zombie Interact Variables")]
    [HideInInspector] public bool seePlayer;
    private bool attacking;
    [SerializeField] private float seeDist, attackDist, loseDist;
    [SerializeField] private MeleeController melee;
    [SerializeField] private int damage;


    // Start is called before the first frame update
    override public void Start()
    {
        melee.damage = damage;
        base.Start();
    }

    override public void Update()
    {
        base.Update();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (PlayerController.instance.invisible)
        {
            seePlayer = false;
        }
        else if (!hurt && !dead)
        {
            float dist = Vector3.Distance(transform.position, PlayerController.instance.transform.position);
            if (dist <= seeDist || (seePlayer && dist <= loseDist))
            {
                seePlayer = true;
                Vector3 playerPos = new Vector3(PlayerController.instance.transform.position.x, transform.position.y, PlayerController.instance.transform.position.z);
                transform.LookAt(playerPos);

                attacking = dist <= attackDist && !attacking && !isPlaying("Melee") ? true : false;
            }
            else
            {
                seePlayer = false;
            }
        }

        animator.SetBool("seePlayer", seePlayer);
        animator.SetBool("isAttacking", attacking);
        melee.gameObject.SetActive(isPlaying("Melee"));
    }
}
