using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorController : MonoBehaviour
{
    //[SerializeField] int elevatorID;
    [SerializeField] float speed;
    [SerializeField] bool moving, movingDown;
    [SerializeField] Transform bottomPoint, topPoint;


    private void FixedUpdate()
    {
        if (moving)
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

        if (transform.position == topPoint.position && !movingDown)
        {
            moving = false;
        }
        else if (transform.position == bottomPoint.position && movingDown)
        {
            moving = false;
        }
    }

    public void CallElevator(bool moveDir)
    {
        if (!moving)
        {
            movingDown = moveDir;
            moving = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
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
