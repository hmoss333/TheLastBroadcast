using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieController : MonoBehaviour
{
    [Header("Zombie Interact Variables")]
    public bool seePlayer, attacking, hit;
    [SerializeField] private MeleeController melee;
    [SerializeField] private float seeDist, attackDist, loseDist;
    [SerializeField] private int damage;
    private Rigidbody rb;
    [SerializeField] private Animator animator;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        melee.damage = damage;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!PlayerController.instance.invisible)
        {
            float dist = Vector3.Distance(transform.position, PlayerController.instance.transform.position);
            if (dist <= seeDist || (seePlayer && dist <= loseDist))
            {
                seePlayer = true;
                Vector3 playerPos = new Vector3(PlayerController.instance.transform.position.x, 0f, PlayerController.instance.transform.position.z);
                transform.LookAt(playerPos);

                if (dist <= attackDist && !attacking)
                {
                    attacking = true;
                    animator.SetTrigger("attacking");
                }
                else if (attacking && !isPlaying("Melee"))
                {
                    attacking = false;
                }
            }
            else
            {
                seePlayer = false;
            }
        }
        else
        {
            seePlayer = false;
        }

        animator.SetBool("seePlayer", seePlayer);
        melee.gameObject.SetActive(isPlaying("Melee"));
    }

    bool isPlaying(string stateName)
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName(stateName) &&
                animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
            return true;
        else
            return false;
    }

    private void OnDisable()
    {
        
    }
}
