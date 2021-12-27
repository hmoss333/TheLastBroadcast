using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;

    public Transform target;
    public float smoothTime = 0.3F;
    [SerializeField] float camXOffset;
    [SerializeField] float camYOffset;
    [SerializeField] float camZOffset;
    Quaternion baseRot;

    [SerializeField] bool focus;
    [SerializeField] float camFocusSize;
    [SerializeField] float camDefaultSize;
    [SerializeField] float focusRate;


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
            Vector3 newRot = Vector3.RotateTowards(transform.position, target.position, smoothTime * Time.deltaTime, 0.0f);
            transform.rotation = target.localRotation;

            if (Camera.main.orthographicSize > camFocusSize)
                Camera.main.orthographicSize -= focusRate * Time.deltaTime; //= Mathf.Lerp(camDefaultSize, camFocusSize, Time.deltaTime);//0.5f;
        }
        else
        {
            transform.rotation = baseRot;

            if (Camera.main.orthographicSize < camDefaultSize)
                Camera.main.orthographicSize += focusRate * Time.deltaTime; //= Mathf.Lerp(camFocusSize, camDefaultSize, Time.deltaTime); //Camera.main.orthographicSize = 4f;
        }


    }

    public void SetTarget(GameObject newTargetObj)
    {
        target = newTargetObj.transform;
    }
}
