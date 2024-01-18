using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHandController : MonoBehaviour
{
    enum State { idle, attacking, reset }
    [SerializeField] State bossState;
    [SerializeField] float attackDelay, attackSpeed, resetSpeed;
    [SerializeField] float yOff;
    [SerializeField] int damage;
    private Vector3 handPos;
    //[SerializeField] GameObject partnerHand;

    float tempDelay = 0f;


    private void OnEnable()
    {
        SetState(State.idle);
    }

    private void FixedUpdate()
    {
        switch (bossState)
        {
            case State.idle:
                handPos = new Vector3(PlayerController.instance.transform.position.x, yOff, PlayerController.instance.transform.position.z);
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

                if (transform.position.y <= 0.05f)
                {
                    //partnerHand.SetActive(true);
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
                    gameObject.SetActive(false);
                    //SetState(State.idle);
                }           
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
            other.GetComponent<Health>().Hurt(damage, true);
        }
    }
}
