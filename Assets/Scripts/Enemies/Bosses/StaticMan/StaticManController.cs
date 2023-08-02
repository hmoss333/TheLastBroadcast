using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using static UnityEditor.Experimental.GraphView.GraphView;

public class StaticManController : CharacterController
{
    [SerializeField] float hurtSpeed;
    [SerializeField] float distance;
    [SerializeField] float staticTriggerRadius, killRadius;
    [SerializeField] private LayerMask layer, camLayer;
    [SerializeField] int hurtCount;


    override public void Start()
    {
        storedSpeed = speed;
        base.Start();
    }

    public override void Update()
    {
        distance = Vector3.Distance(transform.position, PlayerController.instance.transform.position);

        if (!hurt && !dead)
        {
            RaycastHit[] hits;
            Vector3 forwardDir = PlayerController.instance.transform.position - transform.position;
            float distanceToPlayer = Vector3.Distance(transform.position, PlayerController.instance.transform.position);
            Debug.DrawRay(transform.position, forwardDir, Color.red);
            hits = Physics.RaycastAll(transform.position, forwardDir, distanceToPlayer, layer);

            try
            {
                if (hits[0].collider.gameObject.CompareTag("Player"))
                {
                    PlayerController.instance.SeePlayer();
                }
            }
            catch { }

            if (distance < killRadius)
            {
                storedSpeed = 0f;
                animator.SetTrigger("isAttacking");
            }
        }

        if (distance <= staticTriggerRadius && !dead)
        {
            CamEffectController.instance.ForceEffect(true);
        }
        else
        {
            CamEffectController.instance.ForceEffect(false);
        }

        Vector3 playerPos = new Vector3(PlayerController.instance.transform.position.x, transform.position.y, PlayerController.instance.transform.position.z);
        if (!dead)
        {
            PlayerController.instance.SeePlayer();
            transform.LookAt(playerPos);
        }

        base.Update();
    }

    private void FixedUpdate()
    {
        storedSpeed = PlayerController.instance.state == PlayerController.States.interacting
            || PlayerController.instance.state == PlayerController.States.wakeUp
            || PlayerController.instance.state == PlayerController.States.listening
            && distance <= killRadius
                ? 0f : isPlaying("Hurt")
                ? hurtSpeed : speed;

        if (isPlaying("Attack")) { storedSpeed = 0f; }
        rb.velocity = transform.forward * storedSpeed;

        animator.SetBool("isMoving", storedSpeed != 0 ? true : false);
        if (hurtCount <= 0 && !isPlaying("Hurt")) { dead = true; }
    }

    public void Hurt()
    {
        hurt = true;
        hurtCount--;
    }
}
