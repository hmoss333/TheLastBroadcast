using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorController : DoorController
{
    [SerializeField] private float doorDelay;
    //[SerializeField] private bool moving, movingDown;
    //[SerializeField] private Transform bottomPoint, topPoint;
    //[SerializeField] Transform exitPoint;
    //RoomController exitRoom;
    [SerializeField] Light elevatorLight;
    [SerializeField] Animator[] anims;


    private void Awake()
    {
        if (exitPoint)
            exitRoom = exitPoint.GetComponentInParent<RoomController>();
    }

    private void OnEnable()
    {
        if (active)
        {
            foreach (Animator anim in anims)
            {
                anim.SetTrigger("activate");//.SetBool("closeDoor", doorState);
            }
        }
    }

    public override void Activate()
    {
        base.Activate();

        elevatorLight.enabled = active;
        if (active)
        {
            foreach (Animator anim in anims)
            {
                anim.SetTrigger("activate");
            }
        }
    }

    //private void Update()
    //{
    //    if (moving)
    //    {
    //        //anim.SetBool("closeDoor", true);
    //        tempMoveDelay -= Time.deltaTime;
    //        if (tempMoveDelay < 0)
    //        {
    //            tempMoveDelay = 0f;
    //            if (movingDown && transform.position != bottomPoint.position)
    //            {
    //                transform.position = Vector3.MoveTowards(transform.position, bottomPoint.position, speed * Time.deltaTime);
    //            }
    //            else if (!movingDown && transform.position != topPoint.position)
    //            {
    //                transform.position = Vector3.MoveTowards(transform.position, topPoint.position, speed * Time.deltaTime);
    //            }
    //        }
    //    }
    //    else
    //        tempMoveDelay = moveDelay;

    //    if ((transform.position == topPoint.position && !movingDown)
    //        || (transform.position == bottomPoint.position && movingDown)
    //        && moving != false)
    //    {
    //        moving = false;
    //        StartCoroutine(CloseDoors(false, doorDelay));
    //    }

    //    if (doorCollider)
    //        doorCollider.SetActive(moving);
    //}

    //public void CallElevator(bool moveDir)
    //{
    //    movingDown = moveDir;
    //    moving = true;
    //}

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && active)
        {
            //other.transform.parent = this.transform;
            //other.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            StartCoroutine(CloseDoors(true, doorDelay));
        }
    }

    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.tag == "Player")// && active)
    //    {
    //        other.transform.parent = null;
    //    }
    //}

    IEnumerator CloseDoors(bool doorState, float delayTime)
    {
        foreach (Animator anim in anims)
        {
            anim.SetBool("closeDoor", doorState);
        }

        yield return new WaitForSeconds(delayTime);

        FadeController.instance.StartFade(1.0f, 1f);

        while (FadeController.instance.isFading)
            yield return null;

        PlayerController.instance.transform.position = exitPoint.position;
        PlayerController.instance.SetLastDir(exitPoint.transform.forward);
        CameraController.instance.transform.position = exitPoint.position;
        CameraController.instance.SetRotation(false);
        transform.GetComponentInParent<RoomController>().gameObject.SetActive(false);

        if (exitRoom)
            exitRoom.gameObject.SetActive(true);

        foreach (Animator anim in anims)
        {
            anim.SetTrigger("activate");
        }

        FadeController.instance.StartFade(0.0f, 1f);
        CameraController.instance.SetTarget(PlayerController.instance.lookTransform);
        CameraController.instance.SetLastTarget(PlayerController.instance.lookTransform);
        PlayerController.instance.SetState(PlayerController.States.idle);
    }
}
