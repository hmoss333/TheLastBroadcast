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
                if (movingDown)
                {
                    if (transform.position != bottomPoint.position)
                    {
                        transform.position = Vector3.MoveTowards(transform.position, bottomPoint.position, speed * Time.deltaTime);
                    }
                }
                else
                {
                    if (transform.position != topPoint.position)
                    {
                        transform.position = Vector3.MoveTowards(transform.position, topPoint.position, speed * Time.deltaTime);
                    }
                }
            }
        }

        if (transform.position == topPoint.position && !movingDown)
        {
            moving = false;
            tempMoveDelay = moveDelay;
        }
        else if (transform.position == bottomPoint.position && movingDown)
        {
            tempMoveDelay = moveDelay;
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
            //other.gameObject.transform.SetParent(this.transform);

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
    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.tag == "Player")
    //    {
    //        other.gameObject.transform.SetParent(null);
    //    }
    //}
}
