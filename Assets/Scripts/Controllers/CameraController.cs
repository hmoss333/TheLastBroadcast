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
    void FixedUpdate()
    {
        Vector3 pos = target.transform.position;
        if (!focus)
        {
            pos.x += camXOffset;
            pos.y += camYOffset;
            pos.z += camZOffset;
        }

        Vector3 newPos = Vector3.Lerp(transform.position, pos, smoothTime * Time.deltaTime);
        transform.position = newPos;

        if (focus)
            transform.rotation = Quaternion.Slerp(transform.rotation, target.rotation, focusRotRate * Time.deltaTime);          
        else
            transform.rotation = Quaternion.Slerp(transform.rotation, baseRot, focusRotRate * 5f * Time.deltaTime); //baseRot;

        smoothTime = focus ? focusSmoothTime : normalSmoothTime;
        Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, focus ? 60f : 20f, focusRate * Time.deltaTime);
    }

    public void SetTarget(GameObject newTargetObj)
    {
        target = newTargetObj.transform;
    }

    public void FocusTarget()
    {
        focus = !focus;
    }

    public bool isFocused()
    {
        return focus;
    }
}
