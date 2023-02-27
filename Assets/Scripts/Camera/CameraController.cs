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
    private float camXOffset;
    private float camYOffset;
    private float camZOffset;
    [SerializeField] float xOff;
    [SerializeField] float yOff;
    [SerializeField] float zOff;
    [SerializeField] float zOffMin, zOffMax;
    float xTemp;
    //[SerializeField] Vector3 camRotMod;
    Quaternion baseRot;

    [SerializeField] bool focus, setRot, lockCam;
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
        xTemp = xOff;
    }

    private void Update()
    {
        //Shift xOff based on left/right movement
        if (PlayerController.instance.inputMaster.Player.Move.ReadValue<Vector2>().x < 0)
            xOff = -xTemp;
        else if (PlayerController.instance.inputMaster.Player.Move.ReadValue<Vector2>().x > 0)
            xOff = xTemp;

        //Manualy shift xOff with button press
        if (PlayerController.instance.inputMaster.Player.ShiftCamera.triggered)
        {
            xOff = xOff * -1f;
        }

        //Shift zOff based on forward/backwards movement
        if (PlayerController.instance.inputMaster.Player.Move.ReadValue<Vector2>().y < 0)
            zOff = zOffMin;
        else if (PlayerController.instance.inputMaster.Player.Move.ReadValue<Vector2>().y > 0)
            zOff = zOffMax;
    }

    //Late Update should always be used for camera follow logic
    //This calculates after all other update logic to ensure that it uses the most accurate position values
    void LateUpdate()
    {
        Vector3 pos = target.transform.position;
        if (!focus)
        {
            pos.x += target != PlayerController.instance.transform
                || PlayerController.instance.state == PlayerController.States.radio
                || PlayerController.instance.isSeen
                || PlayerController.instance.state == PlayerController.States.interacting
                ? camXOffset : camXOffset + xOff;
            pos.y += target != PlayerController.instance.transform
                || PlayerController.instance.state == PlayerController.States.radio
                || PlayerController.instance.isSeen
                || PlayerController.instance.state == PlayerController.States.interacting
                ? camYOffset : camYOffset + yOff;
            pos.z += target != PlayerController.instance.transform
                || PlayerController.instance.state == PlayerController.States.radio
                || PlayerController.instance.isSeen
                || PlayerController.instance.state == PlayerController.States.interacting
                ? camZOffset : camZOffset + zOff;
        }

        smoothTime = focus ? focusSmoothTime : normalSmoothTime;

        transform.position = Vector3.Lerp(transform.position, pos, smoothTime * Time.deltaTime); //update camera position
        transform.rotation = Quaternion.Lerp(transform.rotation, focus || setRot ? target.rotation : baseRot, focusRotRate * Time.deltaTime); //update camera rotation based on focus state
        //transform.rotation *= Quaternion.Euler(camRotMod.x, camRotMod.y, camRotMod.z); //Apply additional rotation modifyers; default 0
        if (!focus && !setRot)
            transform.LookAt(target);

        Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, focus ? 60f : 20f, focusRate * Time.deltaTime); //update camera field of view based on focus state
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
    public void FocusTarget()
    {
        focus = !focus;
    }

    public bool isFocused()
    {
        return focus;
    }


    //Object Rot Get/Set
    public void SetRotation(bool rotState)
    {
        setRot = rotState;
    }

    public bool GetRotState()
    {
        return setRot;
    }


    //Cam Lock Get/Set
    public void SetCamLock(bool lockState)
    {
        lockCam = lockState;
    }

    public bool GetCamLockState()
    {
        return lockCam;
    }
}
