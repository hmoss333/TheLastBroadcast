using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;

    public Transform target;
    public float smoothTime = 0.3F;
    public float camYOffset;
    public float camZOffset;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = target.transform.position;
        pos.z += camZOffset;
        pos.y += camYOffset;
        transform.position = pos;
    }

    public void SetTarget(GameObject newTargetObj)
    {
        target = newTargetObj.transform;
    } 
}
