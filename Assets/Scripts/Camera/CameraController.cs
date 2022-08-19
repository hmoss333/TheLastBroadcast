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
    [SerializeField] float xOff;
    [SerializeField] float yOff;
    [SerializeField] float zOff;
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

    private void Update()
    {
        if (Input.GetButtonDown("CamXOffset"))
            xOff = -xOff;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 pos = target.transform.position;
        if (!focus)
        {
            pos.x += target != PlayerController.instance.transform || PlayerController.instance.usingRadio || PlayerController.instance.isSeen
                ? camXOffset : camXOffset + xOff;
            pos.y += target != PlayerController.instance.transform || PlayerController.instance.usingRadio || PlayerController.instance.isSeen
                ? camYOffset : camYOffset + yOff;
            pos.z += target != PlayerController.instance.transform || PlayerController.instance.usingRadio || PlayerController.instance.isSeen
                ? camZOffset : camZOffset + zOff;
        }

        smoothTime = focus ? focusSmoothTime : normalSmoothTime;

        transform.position = Vector3.Lerp(transform.position, pos, smoothTime * Time.deltaTime); //update camera position
        transform.rotation = Quaternion.Lerp(transform.rotation, focus ? target.rotation : baseRot, focusRotRate * Time.deltaTime); //update camera rotation based on focus state
        Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, focus ? 60f : 20f, focusRate * Time.deltaTime); //update camera field of view based on focus state
    }

    public void SetTarget(GameObject newTargetObj)
    {
        target = newTargetObj.transform;
    }

    public Transform GetTarget()
    {
        return target;
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
