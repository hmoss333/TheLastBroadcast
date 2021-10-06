using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;


    //Player Movement Controls
    Rigidbody rb;
    float horizontal, vertical;
    [SerializeField] float speed;
    float storedSpeed;
    [SerializeField] Vector3 lastDir = new Vector3();


    public bool interacting;
    [SerializeField] LayerMask layer;
    [SerializeField] float checkDist;


    // Start is called before the first frame update
    void Start()
    {
        storedSpeed = speed;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Interact"))
        {
            Interact();
        }
    }

    private void FixedUpdate()
    {
        //Convert input to local value
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        //Movement Controller
        if (interacting)
        {
            speed = 0; //stop all player movement
        }
        else
        {
            speed = storedSpeed; //restore default player movement

            //Save last input vector for interact raycast
            if (horizontal != 0 || vertical != 0)
            {
                lastDir.x = horizontal;
                lastDir.z = vertical;
            }
        }

        //Move player in FixedUpdate for consistent performance
        rb.velocity = new Vector3(horizontal * speed, 0, vertical * speed);
    }

    void Interact()
    {
        GameObject interactObj;
        Vector3 rayDir = lastDir.normalized;
        Ray ray = new Ray(transform.position, rayDir);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, layer))
        {
            interactObj = hit.transform.gameObject;
            float dist = Vector3.Distance(transform.position, interactObj.transform.position);
            if (dist <= checkDist)
            {
                interacting = !interacting;

                try
                {
                    interactObj.GetComponent<InteractObject>().Interact();
                }
                catch
                {
                    Debug.Log("Object is missing interactable script: " + interactObj.name);
                }
            }
        }
    }
}
