using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanCamera : MonoBehaviour
{
    [SerializeField] Vector3 startPos, endPos;
    [SerializeField] float speed;


    // Update is called once per frame
    void Update()
    {
        Vector3 interpolatedPosition = Vector3.Lerp(transform.position, endPos, speed * Time.deltaTime);
        transform.position = interpolatedPosition;
    }
}
