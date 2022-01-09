using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;

    [SerializeField] Transform target;
    [SerializeField] float smoothTime;
    [SerializeField] float focusSmoothTime;
    [SerializeField] float normalSmoothTime;
    private float camXOffset;
    private float camYOffset;
    private float camZOffset;
    Quaternion baseRot;

    [SerializeField] bool focus;
    [SerializeField] float camFocusSize;
    [SerializeField] float camDefaultSize;
    [SerializeField] float focusRate;
    [SerializeField] float focusRotRate;


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
        Vector3 cameraPos = Camera.main.transform.position;
        camXOffset = cameraPos.x;
        camYOffset = cameraPos.y;
        camZOffset = cameraPos.z;

        baseRot = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = target.transform.position;
        if (!focus)
        {
            pos.x += camXOffset;
            pos.y += camYOffset;
            pos.z += camZOffset;
        }

        Vector3 newPos = Vector3.Lerp(transform.position, pos, smoothTime);
        transform.position = newPos;

        if (focus)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, target.rotation, Time.deltaTime * focusRotRate);


            if (Camera.main.orthographicSize > camFocusSize)
                Camera.main.orthographicSize -= focusRate * Time.deltaTime;
            else
                Camera.main.orthographic = false; //Set camera to perspective when in focus mode
        }
        else
        {
            transform.rotation = baseRot;

            if (Camera.main.orthographicSize < camDefaultSize)
                Camera.main.orthographicSize += focusRate * Time.deltaTime;
        }

        smoothTime = focus ? focusSmoothTime : normalSmoothTime;
    }

    public void SetTarget(GameObject newTargetObj)
    {
        target = newTargetObj.transform;
    }

    public void FocusTarget()
    {
        focus = !focus;

        if (!focus)
            Camera.main.orthographic = true; //Set camera to ortho when exiting focus mode
    }
}
