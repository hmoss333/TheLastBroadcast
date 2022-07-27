using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;


    //Player Movement Controls
    Rigidbody rb;
    float horizontal, vertical;
    [SerializeField] float speed, rotSpeed;
    float storedSpeed;
    Vector3 lastDir, lastDir1, lastDir2;

    public bool interacting;
    [SerializeField] LayerMask layer;
    [SerializeField] float checkDist;

    public InteractObject interactObj;
    [SerializeField] GameObject playerAvatar;
    public Animator animator;




    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }

    void Start()
    {
        storedSpeed = speed;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Vector3 rayDir = lastDir.normalized;
        Ray ray = new Ray(transform.position, rayDir);
        Ray ray1 = new Ray(transform.position, lastDir1);
        Ray ray2 = new Ray(transform.position, lastDir2);
        RaycastHit hit, hit1, hit2;

        if (Physics.Raycast(ray, out hit, checkDist, layer))
        {
            interactObj = hit.transform.gameObject.GetComponent<InteractObject>();
        }
        else if (Physics.Raycast(ray1, out hit1, checkDist, layer))
        {
            interactObj = hit1.transform.gameObject.GetComponent<InteractObject>();
        }
        else if (Physics.Raycast(ray2, out hit2, checkDist, layer))
        {
            interactObj = hit2.transform.gameObject.GetComponent<InteractObject>();
        }
        else
        {
            interactObj = null;
        }

        if (interactObj != null && interactObj.active && !interactObj.hasActivated)
            interactObj.gameObject.GetComponent<Outline>().enabled = true;


        //Change these statements into a state machine
        if (Input.GetButtonDown("Interact") && interactObj != null && interactObj.active)// && !RadioOverlay_Controller.instance.isActive && !attacking && !dashing)
        {
            interactObj.Interact();
        }

        if (Input.GetButtonDown("Radio") && SaveDataController.instance.saveData.abilities.radio == true)// && !attacking && !dashing)
        {
            RadioToggle();
        }
    }

    private void FixedUpdate()
    {
        //Movement Controller
        if (interacting)
        {
            speed = 0; //stop all player movement
        }


        if (!interacting)
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
            rb.velocity = new Vector3(horizontal * speed, rb.velocity.y, vertical * speed);

            // Determine which direction to rotate towards
            Vector3 targetDirection = lastDir;

            // The step size is equal to speed times frame time.
            float singleStep = rotSpeed * Time.deltaTime;

            // Rotate the forward vector towards the target direction by one step
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);
            lastDir1 = (2 * transform.forward - transform.right).normalized; //Left ray
            lastDir2 = (2 * transform.forward + transform.right).normalized; //Right ray

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

    public void InteractToggle(bool interactState)
    {
        interacting = interactState;
        animator.SetBool("isInteracting", interacting);
    }

    public void RadioToggle()
    {
        if (!interacting || (interacting && RadioOverlay_Controller.instance.isActive))
        {
            interacting = !interacting;
            RadioOverlay_Controller.instance.ToggleOn();
            animator.SetBool("isRadio", interacting);
        }
    }
}
