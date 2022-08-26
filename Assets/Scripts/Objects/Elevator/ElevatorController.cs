using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorController : InteractObject
{
    [SerializeField] private float speed, moveDelay;
    private float tempMoveDelay;
    [SerializeField] private bool moving, movingDown;
    [SerializeField] private Transform bottomPoint, topPoint;

    private void Start()
    {
        tempMoveDelay = moveDelay;
    }

    private void Update()
    {
        if (moving)
        {
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
}
