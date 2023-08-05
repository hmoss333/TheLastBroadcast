using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCheck : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Wall")
            && !CameraController.instance.GetRotState()
            && PlayerController.instance.lastDir.x != 0
            && PlayerController.instance.lastDir.z == 0)
        {
            CameraController.instance.HittingWall(true);
        }
    }

    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.gameObject.layer == LayerMask.NameToLayer("Wall"))
    //    {
    //        CameraController.instance.HittingWall(false);
    //    }
    //}
}
