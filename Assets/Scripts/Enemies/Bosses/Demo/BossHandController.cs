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
    [SerializeField] GameObject shadowObj;
    [SerializeField] Transform target;

    float tempDelay = 0f;


    private void OnEnable()
    {
        SetState(State.idle);
        shadowObj.transform.localScale = new Vector3(0f, 0.01f, 0f); //set default scale
    }

    private void FixedUpdate()
    {
        switch (bossState)
        {
            case State.idle:
                handPos = new Vector3(target.position.x, yOff, target.position.z);
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
                shadowObj.transform.localScale = Vector3.Lerp(shadowObj.transform.localScale, new Vector3(1f, 0.01f, 1f), (attackSpeed / 2f) * Time.deltaTime);

                if (transform.position.y <= 0.05f)
                {
                    //partnerHand.SetActive(true);
                    SetState(State.reset);
                }
                break;
            case State.reset:
                handPos = new Vector3(handPos.x, handPos.y += resetSpeed * Time.deltaTime, handPos.z);
                transform.position = handPos;
                shadowObj.transform.localScale = Vector3.Lerp(shadowObj.transform.localScale, new Vector3(0f, 0.01f, 0f), (resetSpeed / 2f) * Time.deltaTime);

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

        shadowObj.transform.position = new Vector3(handPos.x, 0, handPos.z);
    }

    void SetState(State stateToSet)
    {
        bossState = stateToSet;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<Health>().Hurt(damage, true);
        }
    }
}
