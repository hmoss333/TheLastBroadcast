using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class PanCamera : MonoBehaviour
{
    [SerializeField] Vector3 startPos, endPos;
    [SerializeField] float speed, offsetVal;
    public bool isPanning { get; private set; }


    // Update is called once per frame
    void Update()
    {
        if (isPanning)
        {
            Vector3 interpolatedPosition = Vector3.Lerp(transform.position, endPos, speed * Time.deltaTime);
            transform.position = interpolatedPosition;

            float distance = Vector3.Distance(transform.position, endPos);
            if (distance < offsetVal)
                isPanning = false;
        }
    }

    public void TogglePanning(bool panningState)
    {
        isPanning = panningState;
    }
}
