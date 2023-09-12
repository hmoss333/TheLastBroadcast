using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using static UnityEditor.Experimental.GraphView.GraphView;

public class RatController : MonoBehaviour
{
    public static RatController instance;


    [Header("Character Movement Variables")]
    [SerializeField] float speed;
    [SerializeField] float rotSpeed;
    private float storedSpeed;
    private Rigidbody rb;
    private float horizontal, vertical;
    private Vector3 lastDir, lastDir1, lastDir2;

    //TODO change some of thse into a state machine
    [Header("Player State Variables")]
    private bool isMoving, attacking, hurt;
    [HideInInspector] public bool interacting { get; private set; }

    [Header("Interact Variables")]
    [SerializeField] private LayerMask layer;
    [SerializeField] private float checkDist;
    [HideInInspector] public InteractObject interactObj { get; private set; }

    [Header("Player Avatar Variables")]
    [SerializeField] private GameObject melee;
    [SerializeField] private Animator animator;


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

        if (attacking)
        {
            interactObj = null;
        }
        else if (Physics.Raycast(ray, out hit, checkDist, layer))
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


        //Player input controls
        if (Input.GetButtonDown("Interact") && interactObj != null && interactObj.active)
        {
            interactObj.Interact();
        }

        if (Input.GetButtonDown("Melee") && !interacting)
        {
            attacking = true;
            animator.SetTrigger("isMelee");
        }

        melee.SetActive(attacking); //toggle melee weapon visibility based on attacking state
    }

    private void FixedUpdate()
    {
        //Movement Controller
        if (interacting || attacking)
        {
            speed = 0; //stop all player movement

            //If attacking, pause all inputs until animation has completed
            if (attacking && !isPlaying("Melee"))
            {
                attacking = false;
            }
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
                isMoving = true;
            }
            else
            {
                isMoving = false;
            }

            animator.SetBool("isMoving", isMoving);
            //animator.SetBool("isFalling", rb.velocity.y < -1f ? true : false); //toggle falling animation


            //Move player in FixedUpdate for consistent performance
            Vector3 tempMove = new Vector3(horizontal, 0f, vertical);
            if (tempMove.magnitude > 1)
                tempMove = tempMove.normalized;
            rb.velocity = new Vector3(tempMove.x * speed, rb.velocity.y, tempMove.z * speed);


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

    public void InteractToggle(bool interactState)
    {
        interacting = interactState;
        animator.SetBool("isInteracting", interacting);
    }

    bool isPlaying(string stateName)
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName(stateName) &&
                animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
            return true;
        else
            return false;
    }

    public void SetLastDir(Vector3 newDir)
    {
        lastDir = newDir;
    }
}
