using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;

    [SerializeField] private Transform target, lastTarget;
    [SerializeField] float smoothTime;
    [SerializeField] float focusSmoothTime;
    [SerializeField] float normalSmoothTime;
    Vector3 camOffset;
    [SerializeField] Vector3 offset;
    [SerializeField] float zOffMin, zOffMax;
    private float xTemp;
    Quaternion baseRot;

    [SerializeField] bool focus, lockCam, setRot;
    [SerializeField] float camFocusSize;
    [SerializeField] float camDefaultSize;
    [SerializeField] float focusRate;
    [SerializeField] float rotRate;
    [SerializeField] float focusRotRate;

    Coroutine resetRot;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        //Run in awake for consistent behavior 
        Vector3 cameraPos = transform.position - target.position; //Camera.main.transform.position;
        transform.position = cameraPos;
        camOffset = cameraPos;

        baseRot = transform.rotation;
        xTemp = offset.x;
    }

    private void Update()
    {
        //Manualy shift xOff with button press
        if (PlayerController.instance.inputMaster.Player.ShiftCamera.triggered)
        {
            offset.x = offset.x * -1f;
        }

        //Shift zOff based on forward/backwards movement
        if (PlayerController.instance.inputMaster.Player.Move.ReadValue<Vector2>().y < 0)
            offset.z = zOffMin;
        else if (PlayerController.instance.inputMaster.Player.Move.ReadValue<Vector2>().y > 0)
            offset.z = zOffMax;
    }

    //Late Update should always be used for camera follow logic
    //This calculates after all other update logic to ensure that it uses the most accurate position values
    void LateUpdate()
    {
        smoothTime = focus ? focusSmoothTime : normalSmoothTime;

        Vector3 pos = target.position;
        if (!focus && !setRot)
        {
            pos.x +=
                PlayerController.instance.state == PlayerController.States.radio
                //|| PlayerController.instance.IsSeen()
                || PlayerController.instance.state == PlayerController.States.interacting
                ? camOffset.x : camOffset.x + offset.x;
            pos.y += 
                PlayerController.instance.state == PlayerController.States.radio
                //|| PlayerController.instance.IsSeen()
                || PlayerController.instance.state == PlayerController.States.interacting
                ? camOffset.y : camOffset.y + offset.y;
            pos.z += 
                PlayerController.instance.state == PlayerController.States.radio
                //|| PlayerController.instance.IsSeen()
                || PlayerController.instance.state == PlayerController.States.interacting
                ? camOffset.z : camOffset.z + offset.z;
        }

        //Force camera to be centered behind target, uniformly
        Vector3 dir = target.position - transform.position;
        Quaternion rot = Quaternion.LookRotation(dir);
        Vector3 eulerRot = rot.eulerAngles; //modify the euler values for the camera rotation directly
        eulerRot = new Vector3(Mathf.Clamp(eulerRot.x, -15f, 15f), offset.x < 0 ? -4 : 0, 0); //clamp rotation values
        rot = Quaternion.Euler(eulerRot);

        transform.rotation = Quaternion.Slerp(transform.rotation, setRot ? target.rotation : rot, rotRate * Time.deltaTime); //Update camera rotation
        transform.position = Vector3.Lerp(transform.position, pos, smoothTime * Time.deltaTime); //Update camera position

        try
        {
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, focus ? 60f : 20f, focusRate * Time.deltaTime); //update camera field of view based on focus state
        }
        catch { }
    }


    //Target Get/Sets
    public void SetTarget(GameObject newTargetObj)
    {
        target = newTargetObj.transform;
    }

    public Transform GetTarget()
    {
        return target;
    }

    public void SetLastTarget(GameObject newLastTarget)
    {
        lastTarget = newLastTarget.transform;
    }

    public Transform GetLastTarget()
    {
        return lastTarget;
    }

    public void LoadLastTarget()
    {
        target = lastTarget;
    }


    //Focus Get/Set
    //Used to determine if the camera should be focused on the current target
    //Will adjust camera view angle for better visuals
    public void FocusTarget()
    {
        focus = !focus;
        SetRotation(focus);
    }

    public void SetFocus(bool focusState)
    {
        focus = focusState;
    }

    public bool GetFocusState()
    {
        return focus;
    }


    //Object Rot Get/Set
    //Used to force target-specific rotations
    //without forcing the camera into a focus-zoom
    public void SetRotation(bool rotState)
    {
        setRot = rotState;
    }

    public bool GetRotState()
    {
        return setRot;
    }


    //Cam Lock Get/Set
    //Used for locking the camera in place
    //specifically when enabling/disabling objects
    public void SetCamLock(bool lockState)
    {
        lockCam = lockState;
    }

    public bool GetCamLockState()
    {
        return lockCam;
    }
}
