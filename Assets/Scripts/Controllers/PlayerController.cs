
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : CharacterController
{
    public static PlayerController instance;


    [Header("Player Movement Variables")]
    [SerializeField] float rotSpeed;
    private float horizontal, vertical;
    private Vector3 lastDir, lastDir1, lastDir2;

    //TODO change some of thse into a state machine
    [Header("Player State Variables")]
    [HideInInspector] public bool interacting, usingRadio, invisible, onLadder, isSeen;
    private bool isMoving, attacking;//, colliding;

    [Header("Interact Variables")]
    [SerializeField] private LayerMask layer;
    [SerializeField] private float checkDist;
    [HideInInspector] public InteractObject interactObj;

    [Header("Player Avatar Variables")]
    [SerializeField] private MeleeController melee;
    [SerializeField] private int damage;
    [SerializeField] private GameObject playerAvatar, bagObj, radioObj;


    public InputMaster inputMaster;
    private InputControlScheme controlScheme;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);

        inputMaster = new InputMaster();
        inputMaster.Enable();
    }

    override public void Start()
    {
        storedSpeed = speed;
        melee.damage = damage;

        base.Start();
    }

    //Update Functions
    override public void Update()
    {
        Vector3 rayDir = lastDir.normalized;
        Ray ray = new Ray(transform.position, rayDir);
        Ray ray1 = new Ray(transform.position, lastDir1);
        Ray ray2 = new Ray(transform.position, lastDir2);
        RaycastHit hit, hit1, hit2;

        if (usingRadio || attacking || invisible)
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
            interactObj.gameObject.GetComponent<Outline>().enabled = interacting ? false : true;


        //Player input controls
        if (!isPlaying("Hurt") && !dead)
        {
            if (inputMaster.Player.Interact.triggered
                && interactObj != null
                && interactObj.active)
            {
                interactObj.Interact();
            }

            if (SaveDataController.instance.saveData.abilities.radio == true
                && !interacting
                && !attacking
                && !invisible)
            {
                //If player holds down Radio button, interact with radio
                if (inputMaster.Player.Radio.triggered) //only register the intial button press event
                {
                    usingRadio = true;
                    RadioController.instance.currentFrequency = 0.0f;
                    CameraController.instance.SetLastTarget(CameraController.instance.GetTarget().gameObject);
                    CameraController.instance.SetRotation(false);
                    CameraController.instance.SetTarget(radioObj);
                }
                //If player releases Radio button, stop interacting with radio
                else if (inputMaster.Player.Radio.ReadValue<float>() <= 0) //if no longer holding
                {
                    usingRadio = false;
                    RadioController.instance.abilityMode = false;
                    CameraController.instance.LoadLastTarget();
                }
            }

            if (SaveDataController.instance.saveData.abilities.crowbar == true              
                && inputMaster.Player.Melee.triggered
                && !interacting
                && !usingRadio
                && !attacking
                && !invisible)  
            {
                animator.SetTrigger("isMelee");
            }
        }

        //Pause ladder climbing animation if there is no input
        if (onLadder)
            animator.enabled = inputMaster.Player.Move.ReadValue<Vector2>().y != 0 ? true : false;
        else
            animator.enabled = true;


        //Melee
        attacking = isPlaying("Melee"); //set attacking value for the duration of the melee animation
        melee.gameObject.SetActive(attacking); //toggle melee weapon visibility based on attacking state
        //Radio
        animator.SetBool("isRadio", usingRadio); //play radio animation while button is held
        RadioController.instance.isActive = usingRadio; //toggle radio controller active state if player is pressing the corresponding input
        radioObj.SetActive(usingRadio); //toggle radioObj based on usingRadio state
        bagObj.SetActive(SaveDataController.instance.saveData.abilities.radio); //only show the bag obj if the player has collected the radio
        //Ladder
        animator.SetBool("ladderMove", onLadder); //play ladder climbing animation while onLadder


        base.Update(); //Handles hurt and dead controller state overrides
    }

    private void FixedUpdate()
    {
        //Movement Controller
        if (interacting || usingRadio || attacking || onLadder || dead || isPlaying("Hurt"))
        {         
            speed = 0; //stop all player movement
            //colliding = false;
        }
        else
        {
            Vector2 move = inputMaster.Player.Move.ReadValue<Vector2>();
            horizontal = move.x;
            vertical = move.y;

            speed = storedSpeed; //restore default player movement

            //Save last input vector for interact raycast
            if (horizontal != 0 || vertical != 0)// && !colliding)
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
            animator.SetBool("isFalling", rb.velocity.y < -1f ? true : false); //toggle falling animation


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


    //Toggle Functions
    public void ToggleAvatar()
    {
        print($"Toggle avatar {!playerAvatar.activeSelf}");
        playerAvatar.SetActive(!playerAvatar.activeSelf);
    }

    public void InteractToggle(bool interactState)
    {
        interacting = interactState;
        animator.SetBool("isInteracting", interacting);
    }

    //Used for the door controller to set exit direction
    public void SetLastDir(Vector3 newDir)
    {
        lastDir = newDir;
    }


    //Pause player movement animation when they walk into an object
    //private void OnCollisionEnter(Collision collision)
    //{
    //    int floorLayer = LayerMask.NameToLayer("Floor");
    //    if (collision.gameObject.layer != floorLayer)
    //    {
    //        isMoving = false;
    //        colliding = true;
    //    }
    //}

    //private void OnCollisionExit(Collision collision)
    //{
    //    colliding = false;
    //}
}
