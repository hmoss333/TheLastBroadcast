
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
    public Transform lookTransform;
    [SerializeField] private float lookOffset;


    [NaughtyAttributes.HorizontalLine]
    [Header("Player State Variables")]
    private bool isSeen;// { get; private set; }
    public enum States { wakeUp, idle, interacting, moving, attacking, listening, radio, healing, hurt, dead }
    public States state;// { get; private set; }
    public enum AbilityStates { none, invisible, isRat }
    public AbilityStates abilityState { get; private set; }
    [SerializeField] private Health health;

    [NaughtyAttributes.HorizontalLine]
    [Header("Interact Variables")]
    [SerializeField] private GameObject interactIcon;
    [SerializeField] private LayerMask layer;
    [SerializeField] private float checkDist, useItemTime = 3f;
    private InteractObject interactObj;

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
        health.SetHealth(SaveDataController.instance.saveData.maxHealth);

        if (state == States.wakeUp)
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
            && !interactObj.hasActivated)
        {
            interactIcon.SetActive(true);
        }
        else
        {
            interactIcon.SetActive(false);
        }


        //Hit wall check
        //Used to stop the camera from poking outside of bounds
        if (Physics.Raycast(ray, out hit, checkDist))
        {
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Wall")
                && !CameraController.instance.GetRotState())
            {
                CameraController.instance.HittingWall(true);
            }
        }
        else
        {
            CameraController.instance.HittingWall(false);
        }


        //Player hurt/death triggers
        if (hurt)
        {
            RadioController.instance.SetActive(false);
            CameraController.instance.LoadLastTarget();
            SetState(States.hurt);
        }
        if (dead)
            SetState(States.dead);


        //Manage Player Inputs
        if ((state == States.idle || state == States.moving)
            && abilityState == AbilityStates.none
            && !PauseMenuController.instance.isPaused && !FlashlightController.instance.isOn
            && !hurt)
        {
            if (SaveDataController.instance.saveData.abilities.crowbar == true
                && PlayerController.instance.inputMaster.Player.Melee.triggered)
            {
                animator.SetTrigger("isMelee");
                SetState(States.attacking);
            }

            if (SaveDataController.instance.saveData.abilities.radio == true
                && PlayerController.instance.inputMaster.Player.Radio.ReadValue<float>() > 0)
            {
                SetState(States.radio);
            }
        }


        //Store player move values
        //Used in FixedUpdate for correct timing with animation flags
        Vector2 move = PlayerController.instance.inputMaster.Player.Move.ReadValue<Vector2>();
        switch (state)
        {
            case States.wakeUp:
                if (!isPlaying("Wake Up"))
                {
                    SetState(States.idle);
                }
                break;
            case States.idle:
                if (interactObj == null //if no object is currently highlighted for interaction
                    && InventoryController.instance.selectedItem != null
                    && InventoryController.instance.selectedItem.ID == 0 //if the medkit is currently selected
                    && health.CurrentHealth() < SaveDataController.instance.saveData.maxHealth //if not at full health
                    && !PauseMenuController.instance.isPaused //if the game is not paused
                    && inputMaster.Player.Interact.IsPressed())
                {
                    SetState(States.healing);
                }

                if (move.x != 0f || move.y != 0f)
                {
                    SetState(States.moving);
                }
                break;
            case States.interacting:
                interactIcon.SetActive(false); //hide interact icon while interacting
                break;
            case States.moving:
                if (!PauseMenuController.instance.isPaused)
                {
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
                        SetState(States.idle);
                    }
                }
                break;
            case States.attacking:
                if (!isPlaying("Melee"))
                {
                    SetState(States.idle);
                }
                break;
            case States.radio:
                if (inputMaster.Player.Radio.ReadValue<float>() <= 0
                    //&& !CameraController.instance.GetCamLockState()
                    && abilityState != AbilityStates.isRat || abilityState == AbilityStates.invisible)
                {
                    CameraController.instance.LoadLastTarget();
                    SetState(States.idle);
                }
                break;
            case States.healing:
                if (inputMaster.Player.Interact.IsPressed())
                {
                    useItemTime -= Time.deltaTime;
                    if (useItemTime <= 0)
                    {
                        InventoryController.instance.RemoveItem(0);
                        health.ModifyHealth(2); //increment player health; placeholder value for now, should be dependant on the medkit size/value
                        useItemTime = 3f;
                    }
                }
                else
                {
                    useItemTime = 3f;
                    SetState(States.idle);
                }
                break;
            case States.hurt:
                RadioController.instance.SetActive(false);
                CameraController.instance.SetTarget(this.transform);

                if (!hurt && !isPlaying("Hurt"))
                {
                    SetState(States.idle);
                }
                break;
            case States.dead:
                if (health.CurrentHealth() >= 0)
                    health.SetHealth(0);
                break;
            default:
                break;
        }


        //Handle player interaction inputs
        if (interactObj != null
            && !PauseMenuController.instance.isPaused
            && PlayerController.instance.inputMaster.Player.Interact.triggered)
        {
            interactObj.Interact();
            if (!interactObj.hasActivated && interactObj.interacting)
                SetState(States.interacting);
            else
                SetState(States.idle);
        }


        base.Update();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (state != States.moving || isPlaying("Hurt"))
        {
            speed = 0;
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

        //Update lookTransform position
        lookTransform.localPosition = Vector3.forward * lookOffset;


        if (isPlaying("Melee"))
        {
            SetState(States.attacking);
        }
        if (isPlaying("Hurt"))
        {
            SetState(States.hurt);
        }


        //Moving
        animator.SetBool("isMoving", state == States.moving);
        //Falling
        animator.SetBool("isFalling", rb.velocity.y < -1f ? true : false);
        //Waking Up
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
        //Healing
        animator.SetBool("isRadio", state == States.healing); //toggle radio animation if healing
        isSeen = false; //reset seen state if no enemies are currently seeing the player (this is ues for the dynamic camera)
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


    //Handle isSeen bool
    public bool IsSeen()
    {
        return isSeen;
    }

    public void SeePlayer()
    {
        isSeen = true;
    }
}
