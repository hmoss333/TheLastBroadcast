using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHandController : MonoBehaviour
{
    enum State { idle, attacking, reset, hurt, dead }
    [SerializeField] State bossState;
    [SerializeField] float attackDelay, attackSpeed, resetSpeed, transperancy;
    [SerializeField] float yOff;
    [SerializeField] int damage;
    private Vector3 handPos;

    float tempDelay = 0f;


    private void FixedUpdate()
    {
        switch (bossState)
        {
            case State.idle:
                handPos = PlayerController.instance.transform.position;
                handPos = new Vector3(handPos.x, yOff, handPos.z);
                transform.position = handPos;

                tempDelay += attackSpeed * Time.deltaTime;
                if (tempDelay >= attackDelay)
                {
                    tempDelay = 0;
                    SetState(State.attacking);
                }
                break;
            case State.attacking:
                handPos = new Vector3(handPos.x, handPos.y -= attackSpeed * Time.deltaTime, handPos.z);
                transform.position = handPos;

                if (transform.position.y <= 0f)
                {
                    SetState(State.reset);
                }
                break;
            case State.reset:
                handPos = new Vector3(handPos.x, handPos.y += resetSpeed * Time.deltaTime, handPos.z);
                transform.position = handPos;

                if (transform.position.y >= yOff)
                {
                    handPos.y = yOff;
                    transform.localPosition = handPos;
                    SetState(State.idle);
                }           
                break;
            case State.hurt:
                break;
            case State.dead:
                break;
            default:
                break;
        }
    }

    void SetState(State stateToSet)
    {
        bossState = stateToSet;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<Health>().Hurt(damage, false);
        }
    }
}
