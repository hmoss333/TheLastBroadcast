using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorController : InteractObject
{
    //[SerializeField] private float doorDelay;
    [SerializeField] Light elevatorLight;
    [SerializeField] Animator[] anims;



    private void OnEnable()
    {
        if (active)
        {
            foreach (Animator anim in anims)
            {
                anim.SetTrigger("activate");
            }
        }
    }

    public override void Activate()
    {
        base.Activate();

        if (active)
        {
            foreach (Animator anim in anims)
            {
                anim.SetTrigger("activate");
            }
        }
    }

    private void Update()
    {
        elevatorLight.enabled = active;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && active)
        {
            //other.transform.parent = this.transform;
            //other.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            //StartCoroutine(CloseDoors(true, doorDelay));
            foreach (Animator anim in anims)
            {
                anim.SetBool("closeDoor", true);
            }

            m_OnTrigger.Invoke();
        }
    }

    //IEnumerator CloseDoors(bool doorState, float delayTime)
    //{
    //    foreach (Animator anim in anims)
    //    {
    //        anim.SetBool("closeDoor", doorState);
    //    }

        //yield return new WaitForSeconds(delayTime);

        //FadeController.instance.StartFade(1.0f, 1f);

        //while (FadeController.instance.isFading)
        //    yield return null;

        //if (exitPoint)
        //{
        //    PlayerController.instance.transform.position = exitPoint.position;
        //    PlayerController.instance.SetLastDir(exitPoint.transform.forward);
        //    CameraController.instance.transform.position = exitPoint.position;
        //    CameraController.instance.SetRotation(false);
        //    transform.GetComponentInParent<RoomController>().gameObject.SetActive(false);

        //    if (exitRoom)
        //        exitRoom.gameObject.SetActive(true);

        //    foreach (Animator anim in anims)
        //    {
        //        anim.SetTrigger("activate");
        //    }

        //    FadeController.instance.StartFade(0.0f, 1f);
        //    CameraController.instance.SetTarget(PlayerController.instance.lookTransform);
        //    CameraController.instance.SetLastTarget(PlayerController.instance.lookTransform);
        //    PlayerController.instance.SetState(PlayerController.States.idle);
        //}
    //}
}
