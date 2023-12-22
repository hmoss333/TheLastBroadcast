using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class Mirror : MonoBehaviour
{
    Transform playerTarget;
    [SerializeField] Transform mirror;

    // Start is called before the first frame update
    void Start()
    {
        playerTarget = PlayerController.instance.transform;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 localPlayer = mirror.InverseTransformPoint(playerTarget.position);
        Vector3 lookatmirror = mirror.TransformPoint(new Vector3(-localPlayer.x, 0, localPlayer.z));
        transform.LookAt(lookatmirror);
    }
}
