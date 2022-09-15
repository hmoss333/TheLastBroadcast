using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorController : InteractObject
{
    [SerializeField] private float speed, moveDelay;
    private float tempMoveDelay;
    [SerializeField] private bool moving, movingDown;
    [SerializeField] private Transform bottomPoint, topPoint;
    [SerializeField] Animator[] anims;

    private void Start()
    {
        tempMoveDelay = moveDelay;
    }

    private void Update()
    {
        if (moving)
        {
            //anim.SetBool("closeDoor", true);
            tempMoveDelay -= Time.deltaTime;
            if (tempMoveDelay < 0)
            {
                tempMoveDelay = 0f;
                if (movingDown && transform.position != bottomPoint.position)
                {
                    transform.position = Vector3.MoveTowards(transform.position, bottomPoint.position, speed * Time.deltaTime);
                }
                else if (!movingDown && transform.position != topPoint.position)
                {
                    transform.position = Vector3.MoveTowards(transform.position, topPoint.position, speed * Time.deltaTime);
                }
            }
        }
        else
            tempMoveDelay = moveDelay;

        if ((transform.position == topPoint.position && !movingDown)
            || (transform.position == bottomPoint.position && movingDown))
        {
            moving = false;
            //anim.SetBool("closeDoor", false);
        }

        //anim.SetBool("closeDoor", moving);
        foreach (Animator anim in anims)
        {
            anim.SetBool("closeDoor", moving);
        }
    }

    public void CallElevator(bool moveDir)
    {
        movingDown = moveDir;
        moving = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && active)
        {
            other.transform.parent = this.transform;

            if (transform.position == bottomPoint.position)
            {
                movingDown = false;
            }
            else if (transform.position == topPoint.position)
            {
                movingDown = true;
            }

            moving = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" && active)
        {
            other.transform.parent = null;
        }
    }
}
