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
            pos.x += 
                PlayerController.instance.state == PlayerController.States.radio
                || PlayerController.instance.IsSeen()
                || PlayerController.instance.state == PlayerController.States.interacting
                ? camXOffset : camXOffset + xOff;
            pos.y +=
                PlayerController.instance.state == PlayerController.States.radio
                || PlayerController.instance.IsSeen()
                || PlayerController.instance.state == PlayerController.States.interacting
                ? camYOffset : camYOffset + yOff;
            pos.z +=
                PlayerController.instance.state == PlayerController.States.radio
                || PlayerController.instance.IsSeen()
                || PlayerController.instance.state == PlayerController.States.interacting
                ? camZOffset : camZOffset + zOff;
        }

        smoothTime = focus ? focusSmoothTime : normalSmoothTime;

        //Force camera to be centered behind target uniformly
        Vector3 dir = target.position - transform.position;
        Quaternion rot = Quaternion.LookRotation(dir);
        Vector3 eulerRot = rot.eulerAngles; //modify the euler values for the camera rotation directly
        print(eulerRot);
        eulerRot = new Vector3(Mathf.Clamp(eulerRot.x, -20f, 20f), xOff < 0 ? -4 : 4, 0); //clamp rotation values
        rot = Quaternion.Euler(eulerRot);

        transform.rotation = Quaternion.Slerp(transform.rotation, setRot ? target.rotation : rot, setRot ? focusRotRate : rotRate * Time.deltaTime); //Update camera rotation
        transform.position = Vector3.Lerp(transform.position, pos, smoothTime * Time.deltaTime); //Update camera position

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
    //Used to determine if the camera should be focused on the current target
    //Will adjust camera view angle for better visuals
    public void FocusTarget()
    {
        focus = !focus;
        SetRotation(focus);
    }

    public bool isFocused()
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
