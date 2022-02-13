using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;


    //Player Movement Controls
    Rigidbody rb;
    float horizontal, vertical;
    [SerializeField] float speed, rotSpeed;//, attackTimer;
    float storedSpeed;
    Vector3 lastDir, lastDir1, lastDir2;

    public bool interacting;//, dashing, attacking;
    //[SerializeField] float dashSpeed;
    //[SerializeField] float dashDist;
    [SerializeField] LayerMask layer;
    [SerializeField] float checkDist;
    //[SerializeField] ParticleSystem dashEffect;

    private InteractIcon_Controller interactIconController;
    [SerializeField] InteractObject interactObj;
    [SerializeField] GameObject playerAvatar;
    [SerializeField] Animator animator;

    //[SerializeField] GameObject meleeObject;




    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);

        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        storedSpeed = speed;
        rb = GetComponent<Rigidbody>();
        interactIconController = GetComponentInChildren<InteractIcon_Controller>();
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

        interactIconController.UpdateIcon(interacting, interactObj);


        //Change these statements into a state machine
        if (Input.GetButtonDown("Interact") && interactObj != null && interactObj.activated)// && !RadioOverlay_Controller.instance.isActive && !attacking && !dashing)
        {
            interactObj.Interact();
        }

        //if (Input.GetButtonDown("Radio") && !attacking && !dashing)
        //{
        //    RadioToggle();
        //}

        //if (Input.GetButtonDown("Melee") && !interacting && !attacking)
        //{
        //    attacking = true;
        //    animator.SetTrigger("isMelee");
        //}

        //if (Input.GetButtonDown("Dash") && !interacting && !dashing)
        //{
        //    dashing = true;
        //}
    }

    private void FixedUpdate()
    {
        //Movement Controller
        if (interacting)
        {
            speed = 0; //stop all player movement
        }

        //meleeObject.SetActive(attacking);


        //if (dashing && !interacting)
        //{
        //    animator.SetTrigger("isDashing");
        //    StartCoroutine(PlayDashParticles());

        //    for (float i = 0; i < dashDist; i += dashSpeed * Time.deltaTime)
        //    {
        //        rb.velocity += transform.forward * i;

        //        Ray ray = new Ray(transform.position, transform.forward);
        //        RaycastHit hit;
        //        if (Physics.Raycast(ray, out hit, dashDist))
        //        {
        //            rb.velocity = Vector3.zero;
        //            transform.position = new Vector3(hit.transform.position.x - transform.position.normalized.x, transform.position.y, hit.transform.position.z - transform.position.normalized.z);
        //            break;
        //        }
        //    }

        //    dashing = false;
        //}
        //else if (attacking)
        //{
        //    if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > attackTimer)// && !animator.IsInTransition(0))
        //        attacking = false;
        //}
        //else
        if (!interacting)// && !attacking)
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

    //IEnumerator PlayDashParticles()
    //{
    //    dashEffect.Play();

    //    yield return new WaitForSeconds(0.1f);

    //    dashEffect.Stop();
    //}

    public void InteractToggle(bool interactState)
    {
        interacting = interactState; //!interacting;
        animator.SetBool("isInteracting", interacting);
    }

    //public void RadioToggle()
    //{
    //    if (!interacting || (interacting && RadioOverlay_Controller.instance.isActive))
    //    {
    //        interacting = !interacting;
    //        RadioOverlay_Controller.instance.ToggleOn();
    //        animator.SetBool("isRadio", interacting);
    //    }
    //}

    //void Melee()
    //{
    //    animator.SetTrigger("isMelee");
    //}
}
