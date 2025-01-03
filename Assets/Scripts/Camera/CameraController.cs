﻿using System.Collections;
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

    [SerializeField] //useful for debugging
    private bool focus, setRot, inTrigger, hitWall;
    public bool isFocusing { get; private set; }
    [SerializeField] float camFocusSize;
    [SerializeField] float camDefaultSize;
    [SerializeField] float focusRate;
    [SerializeField] float rotRate;
    [SerializeField] float focusRotRate;

    //private bool focusingOnObjs;
    [SerializeField] private List<SaveObject> objsToFocus;

    //Coroutine resetRot;
    Coroutine focusObjsRoutine;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        //Run in awake for consistent behavior 
        Vector3 cameraPos = transform.position - target.position;
        transform.position = cameraPos;
        camOffset = cameraPos;

        baseRot = transform.rotation;
        xTemp = offset.x;
    }

    private void Update()
    {
        //Manualy shift xOff with button press
        //if (InputController.instance.inputMaster.Player.ShiftCamera.triggered)
        //{
        //    offset.x = offset.x * -1f;
        //}

        //Shift zOff based on forward/backwards movement
        if (PlayerController.instance.state == PlayerController.States.moving)
        {
            if (InputController.instance.inputMaster.Player.Move.ReadValue<Vector2>().y < 0)
                offset.z = zOffMin;
            else if (InputController.instance.inputMaster.Player.Move.ReadValue<Vector2>().y > 0)
                offset.z = zOffMax;
        }
    }

    //Fixed Update keeps the camera on the same update step as the player's movement position calculation
    void FixedUpdate()
    {
        smoothTime = focus ? focusSmoothTime : normalSmoothTime;

        Vector3 pos = target.position;
        if (!focus && !setRot)
        {
            pos.x +=
                PlayerController.instance.IsSeen()
                || PlayerController.instance.state == PlayerController.States.radio
                    ? camOffset.x - offset.x :
                PlayerController.instance.state == PlayerController.States.interacting
                    ? camOffset.x
                    : camOffset.x + offset.x;
            pos.y +=
                camOffset.y + offset.y;
            pos.z +=
                PlayerController.instance.IsSeen()
                || PlayerController.instance.state == PlayerController.States.radio
                    ? camOffset.z + (1.5f * offset.z) : camOffset.z + offset.z;
            //PlayerController.instance.state == PlayerController.States.interacting
            //    ? camOffset.z
            //    : camOffset.z + offset.z;
        }

        //if (hitWall && focusObjsRoutine == null)
        //{
        //    pos = new Vector3(transform.position.x, pos.y, pos.z);
        //}


        //Force camera to be centered behind target, uniformly
        Vector3 dir = target.position - transform.position;
        Quaternion rot = Quaternion.LookRotation(dir);
        Vector3 eulerRot = rot.eulerAngles; //modify the euler values for the camera rotation directly
        eulerRot = new Vector3(Mathf.Clamp(eulerRot.x, -15f, 15f), offset.x < 0 ? -4 : 0, 0); //clamp rotation values //  eulerRot.x, -15f, 3f)
        rot = Quaternion.Euler(eulerRot);

        transform.rotation = Quaternion.Slerp(transform.rotation, setRot ? target.rotation : rot, rotRate * Time.deltaTime); //Update camera rotation
        transform.position = Vector3.Lerp(transform.position, pos, smoothTime * Time.deltaTime); //Update camera position

        try
        {
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, focus ? 60f : 20f, focusRate * Time.deltaTime); //update camera field of view based on focus state
        }
        catch { }
    }

    private void LateUpdate()
    {
        if (objsToFocus.Count > 0 && focusObjsRoutine == null)
        {
            StartFocusingObjs();
        }
    }


    //Target Get/Sets
    public void SetTarget(Transform newTargetObj)
    {
        print($"Set Cam Target: {newTargetObj.name}");
        target = newTargetObj;
    }

    public Transform GetTarget()
    {
        return target;
    }

    public void SetLastTarget(Transform newLastTarget)
    {
        print($"Set Last Cam Target: {newLastTarget.name}");
        lastTarget = newLastTarget;
    }

    public Transform GetLastTarget()
    {
        return lastTarget;
    }

    public void LoadLastTarget()
    {
        print($"Loading Last Cam Target");
        target = lastTarget;
    }

    public void HittingWall(bool value)
    {
        hitWall = value;
    }


    //Focus Get/Set
    //Used to determine if the camera should be focused on the current target
    //Will adjust camera view angle for better visuals
    public void FocusTarget()
    {
        focus = !focus;
        isFocusing = focus;
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



    //Focus object list functions
    //Used to cycle through a list of objects to focus/activate
    //Allows for viewing/activating multiple objects at once
    //without sending multiple calls to the CameraController class
    public void AddObjToFocus(SaveObject newObj)
    {
        List<SaveObject> tempList = objsToFocus;
        tempList.Add(newObj);
        objsToFocus = tempList;
    }

    void StartFocusingObjs()
    {
        if (focusObjsRoutine == null)
            focusObjsRoutine = StartCoroutine(StartFocusObjsRoutine());
    }

    IEnumerator StartFocusObjsRoutine()
    {
        bool tempRotSet = setRot;
        bool tempTriggerState = inTrigger;
        bool tempHitWall = hitWall;
        setRot = false;
        inTrigger = false;
        hitWall = false;
        isFocusing = true;

        PlayerController.instance.SetState(PlayerController.States.listening);

        for (int i = 0; i < objsToFocus.Count; i++)
        {
            SetTarget(objsToFocus[i].transform);

            yield return new WaitForSeconds(2f);
        }

        setRot = tempRotSet;
        inTrigger = tempTriggerState;
        hitWall = tempHitWall;
        LoadLastTarget();
        PlayerController.instance.SetState(PlayerController.States.idle);

        objsToFocus.Clear();
        isFocusing = false;
        focusObjsRoutine = null;
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


    //In Trigger Get/Set
    //Used to determine if an object is inside of a trigger volume that needs to force a camera angle
    //Used for instances where the player interacts with an object that will force perspective while inside of a cameratriggervolume
    public void SetTriggerState(bool triggerState)
    {
        inTrigger = triggerState;
    }

    public bool GetTriggerState()
    {
        return inTrigger;
    }
}
