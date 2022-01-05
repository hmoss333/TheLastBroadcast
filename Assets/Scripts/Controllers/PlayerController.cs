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
    [SerializeField] float rotSpeed;
    float storedSpeed;
    Vector3 lastDir, lastDir1, lastDir2;

    public bool interacting;
    [SerializeField] LayerMask layer;
    [SerializeField] float checkDist;

    [SerializeField] GameObject interactObj;
    [SerializeField] GameObject playerAvatar;
    [SerializeField] Animator animator;


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
        storedSpeed = speed;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 rayDir = lastDir.normalized;
        Ray ray = new Ray(transform.position, rayDir);
        Ray ray1 = new Ray(transform.position, lastDir1);
        Ray ray2 = new Ray(transform.position, lastDir2);
        RaycastHit hit, hit1, hit2;

        if (Physics.Raycast(ray, out hit, checkDist, layer))
        {
            interactObj = hit.transform.gameObject;
        }
        else if (Physics.Raycast(ray1, out hit1, checkDist, layer))
        {
            interactObj = hit1.transform.gameObject;
        }
        else if (Physics.Raycast(ray2, out hit2, checkDist, layer))
        {
            interactObj = hit2.transform.gameObject;
        }
        else
        {
            interactObj = null;
        }

        InteractIcon_Controller.instance.UpdateIcon(interacting, interactObj);
        animator.SetBool("isInteracting", interacting);


        if (Input.GetButtonDown("Interact") && interactObj != null)
        {
            interacting = !interacting;
            interactObj.GetComponent<InteractObject>().Interact();
        }

        if (Input.GetButtonDown("Radio"))
        {
            if (!interacting || (interacting && RadioOverlay_Controller.instance.isActive))
            {
                interacting = !interacting;
                RadioOverlay_Controller.instance.ToggleOn();
                animator.SetBool("isRadio", interacting);
            }
        }
    }

    private void FixedUpdate()
    {
        //Movement Controller
        if (interacting)
        {
            speed = 0; //stop all player movement
        }
        else
        {
            horizontal = Input.GetAxisRaw("Horizontal");
            vertical = Input.GetAxisRaw("Vertical");

            speed = storedSpeed; //restore default player movement

            //Save last input vector for interact raycast
            if (horizontal != 0 || vertical != 0)
            {
                lastDir.x = horizontal;
                lastDir.z = vertical;
                animator.SetBool("isMoving", true);
            }
            else
            {
                animator.SetBool("isMoving", false);
            }

            //Move player in FixedUpdate for consistent performance
            rb.velocity = new Vector3(horizontal * speed, 0, vertical * speed);

            // Determine which direction to rotate towards
            Vector3 targetDirection = lastDir;

            // The step size is equal to speed times frame time.
            float singleStep = rotSpeed * Time.deltaTime;

            // Rotate the forward vector towards the target direction by one step
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);
            lastDir1 = (transform.forward - transform.right).normalized;
            lastDir2 = (transform.forward + transform.right).normalized;

            // Draw rays pointing in all interact directions
            Debug.DrawRay(transform.position, newDirection, Color.green);
            Debug.DrawRay(transform.position, lastDir1, Color.green);
            Debug.DrawRay(transform.position, lastDir2, Color.green);

            // Calculate a rotation a step closer to the target and applies rotation to this object
            transform.rotation = Quaternion.LookRotation(newDirection);
        }
    }

    public void ToggleAvatar()
    {
        playerAvatar.SetActive(!playerAvatar.activeSelf);
    }
}
