
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : CharacterController
{
    public static PlayerController instance;

    [SerializeField] float rotSpeed;
    private float horizontal, vertical;
    private Vector3 lastDir, lastDir1, lastDir2;


    [NaughtyAttributes.HorizontalLine]
    [Header("Player State Variables")]
    public bool isSeen;
    public enum States { wakeUp, idle, interacting, moving, attacking, listening, radio, hurt, dead }
    public States state { get; private set; }
    public enum AbilityStates { none, invisible, isRat }
    public AbilityStates abilityState { get; private set; }

    [NaughtyAttributes.HorizontalLine]
    [Header("Interact Variables")]
    [SerializeField] private LayerMask layer;
    [SerializeField] private float checkDist;
    [HideInInspector] public InteractObject interactObj { get; private set; }

    [NaughtyAttributes.HorizontalLine]
    [Header("Player Avatar Variables")]
    [SerializeField] private GameObject playerAvatar;
    [SerializeField] private GameObject bagObj;
    [SerializeField] private GameObject radioObj;
    [SerializeField] private GameObject gasMaskObj;
    [SerializeField] private MeleeController melee;
    [SerializeField] private int damage;
    public InputMaster inputMaster { get; private set; }
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
        gasMaskObj.SetActive(false);
        animator.SetTrigger("wakeUp");

        base.Start();
    }

    override public void Update()
    {
        Vector3 rayDir = lastDir.normalized;
        Ray ray = new Ray(transform.position, rayDir);
        Ray ray1 = new Ray(transform.position, lastDir1);
        Ray ray2 = new Ray(transform.position, lastDir2);
        RaycastHit hit, hit1, hit2;

        if (state == States.radio || state == States.attacking || state == States.listening
            || abilityState == AbilityStates.invisible || abilityState == AbilityStates.isRat)
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

        if (interactObj != null
            && interactObj.active
            && !interactObj.hasActivated
            && !interactObj.GetComponent<Outline>())
        {
            interactObj.gameObject.AddComponent<Outline>();
        }


        //Manage Player Inputs
        if ((state == States.idle || state == States.moving)
            && abilityState == AbilityStates.none /*!invisible*/
            && !PauseMenuController.instance.isPaused && !FlashlightController.instance.isOn)
        {
            if (SaveDataController.instance.saveData.abilities.crowbar == true
                && PlayerController.instance.inputMaster.Player.Melee.triggered)
            {
                animator.SetTrigger("isMelee");
                state = States.attacking;
            }

            if (SaveDataController.instance.saveData.abilities.radio == true
                && PlayerController.instance.inputMaster.Player.Radio.ReadValue<float>() > 0)
            {
                state = States.radio;
                CameraController.instance.SetLastTarget(CameraController.instance.GetTarget().gameObject);
                CameraController.instance.SetRotation(false);
                CameraController.instance.SetTarget(radioObj);
            }
        }

        //Handle player interaction inputs
        if (interactObj != null
            && interactObj.active
            && !PauseMenuController.instance.isPaused
            && PlayerController.instance.inputMaster.Player.Interact.triggered)
        {
            interactObj.Interact();
            if (interactObj.active && !interactObj.hasActivated && interactObj.interacting)
                state = States.interacting;
            else
                state = States.idle;
        }


        //Player hurt/death triggers
        if (hurt)
            state = States.hurt;
        if (dead)
            state = States.dead;



        //Standing Up
        if (isPlaying("Wake Up")) { state = States.wakeUp; }
        //Interacting
        animator.SetBool("isInteracting", state == States.interacting);
        //Radio
        animator.SetBool("isRadio", state == States.radio); //play radio animation while button is held
        RadioController.instance.SetActive(state == States.radio && abilityState != AbilityStates.isRat); //toggle radio controller active state if player is pressing the corresponding input; hide radio UI if isRat
        radioObj.SetActive(state == States.radio); //toggle radioObj based on usingRadio state
        bagObj.SetActive(SaveDataController.instance.saveData.abilities.radio); //only show the bag obj if the player has collected the radio
        //Melee
        melee.gameObject.SetActive(state == States.attacking); //toggle melee weapon visibility based on attacking state
        //Listening
        animator.SetBool("isListening", state == States.listening); //toggle listening animation based on bool value


        base.Update();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (state != States.moving)
        {
            speed = 0; //stop all player movement
        }

        // Determine which direction to rotate towards
        Vector3 targetDirection = lastDir;

        // The step size is equal to speed times frame time.
        float singleStep = rotSpeed * Time.deltaTime;

        // Rotate the forward vector towards the target direction by one step
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);
        lastDir1 = (2 * transform.forward - transform.right).normalized; //Left ray
        lastDir2 = (2 * transform.forward + transform.right).normalized; //Right ray

        // Calculate a rotation a step closer to the target and applies rotation to this object
        transform.rotation = Quaternion.LookRotation(newDirection);


        //Store player move values
        //Used in FixedUpdate for correct timing with animation flags
        Vector2 move = PlayerController.instance.inputMaster.Player.Move.ReadValue<Vector2>();
        switch (state)
        {
            case States.wakeUp:
                if (!isPlaying("Wake Up"))
                {
                    state = States.idle;
                }
                break;
            case States.idle:
                if (move.x != 0f || move.y != 0f)
                {
                    state = States.moving;
                }
                break;
            case States.moving:
                speed = storedSpeed;

                horizontal = Mathf.Round(move.x * 10f) * 0.1f;
                vertical = Mathf.Round(move.y * 10f) * 0.1f;

                //Save last input vector for interact raycast
                if (horizontal != 0f || vertical != 0f)
                {
                    lastDir.x = horizontal;
                    lastDir.z = vertical;
                }

                Vector3 tempMove = new Vector3(horizontal, 0f, vertical);
                if (tempMove.magnitude > 1)
                    tempMove = tempMove.normalized;
                rb.velocity = new Vector3(tempMove.x * speed, rb.velocity.y, tempMove.z * speed);

                if (move.x == 0f && move.y == 0f)
                {
                    state = States.idle;
                }
                break;
            case States.attacking:
                if (!isPlaying("Melee") || hurt)
                {
                    state = States.idle;
                }
                break;
            case States.radio:
                if (PlayerController.instance.inputMaster.Player.Radio.ReadValue<float>() <= 0
                    && !CameraController.instance.GetCamLockState()
                    && abilityState != AbilityStates.isRat || abilityState == AbilityStates.invisible)
                {
                    CameraController.instance.LoadLastTarget();
                    state = States.idle;
                }
                break;
            case States.hurt:
                if (!hurt && !isPlaying("Hurt"))
                    state = States.idle;
                break;
            default:
                break;
        }

        animator.SetBool("isMoving", state == States.moving);
        animator.SetBool("isFalling", rb.velocity.y < -1f ? true : false);
    }


    //Toggle Functions
    public void ToggleAvatar()
    {
        playerAvatar.SetActive(!playerAvatar.activeSelf);
    }

    public void ToggleGasMask(bool maskState)
    {
        gasMaskObj.SetActive(maskState);
    }

    //Used for the door controller to set exit direction
    public void SetLastDir(Vector3 newDir)
    {
        lastDir = newDir;
    }


    public void SetState(States stateToSet)
    {
        state = stateToSet;
    }

    public void SetAbilityState(AbilityStates abilityStateToSet)
    {
        abilityState = abilityStateToSet;
    }
}
