using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorController : MonoBehaviour
{
    [SerializeField] private float speed, moveDelay, doorDelay;
    private float tempMoveDelay;
    [SerializeField] private bool moving, movingDown;
    [SerializeField] private Transform bottomPoint, topPoint;
    [SerializeField] GameObject doorCollider;
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
            || (transform.position == bottomPoint.position && movingDown)
            && moving != false)
        {
            moving = false;
            StartCoroutine(CloseDoors(false, doorDelay));
        }

        if (doorCollider)
            doorCollider.SetActive(moving);
    }

    public void CallElevator(bool moveDir)
    {
        movingDown = moveDir;
        moving = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.transform.parent = this.transform;
            other.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            StartCoroutine(CloseDoors(true, doorDelay));

            if (transform.position == bottomPoint.position)
            {
                movingDown = false;
            }
            else if (transform.position == topPoint.position)
            {
                movingDown = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")// && active)
        {
            other.transform.parent = null;
        }
    }

    IEnumerator CloseDoors(bool doorState, float delayTime)
    {
        foreach (Animator anim in anims)
        {
            anim.SetBool("closeDoor", doorState);
        }

        yield return new WaitForSeconds(delayTime);

        moving = doorState;
    }
}
