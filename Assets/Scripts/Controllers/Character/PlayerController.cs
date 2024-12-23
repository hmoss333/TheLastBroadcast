﻿
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.UI;

public class PlayerController : CharacterController
{
    public static PlayerController instance;

    [Header("Player Movement Variables")]
    [NaughtyAttributes.HorizontalLine]
    [SerializeField] float runSpeed;
    [SerializeField] float rotSpeed;
    private float horizontal, vertical;
    private Vector3 lastDir, lastDir1, lastDir2;
    public Transform lookTransform;
    [SerializeField] private float lookOffset;


    [Header("Player State Variables")]
    [NaughtyAttributes.HorizontalLine]
    private bool isSeen;// { get; private set; }
    public enum States { wakeUp, idle, interacting, moving, attacking, listening, radio, movingObj, stunned, hurt, dead } //consuming,
    public States state;// { get; private set; }
    public enum AbilityStates { none, invisible, isRat }
    public AbilityStates abilityState { get; private set; }
    [SerializeField] private Health health;
    public int maxHealth { get; private set; }
    public float stamina { get; private set; }
    public bool running { get; private set; }

    [Header("Player Interact Variables")]
    [NaughtyAttributes.HorizontalLine]
    [SerializeField] private InteractIcon_Controller interactIcon;
    [SerializeField] private LayerMask layer;
    [SerializeField] private float checkDist;
    private InteractObject interactObj;

    [Header("Player Object References")]
    [NaughtyAttributes.HorizontalLine]
    [SerializeField] private GameObject playerAvatar;
    [SerializeField] private GameObject bagObj;
    [SerializeField] private GameObject radioObj;
    [SerializeField] private GameObject gasMaskObj;
    [SerializeField] private GameObject gasMaskOverlay;
    [SerializeField] private MeleeController melee;
    [SerializeField] private int damage;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }

    override public void Start()
    {
        storedSpeed = speed;
        melee.damage = damage;
        stamina = 5f;
        gasMaskObj.SetActive(false);
        health.SetHealth(SaveDataController.instance.saveData.maxHealth);

        if (state == States.wakeUp)
            animator.SetTrigger("wakeUp");

        base.Start();
    }

    override public void Update()
    {
        maxHealth = SaveDataController.instance.saveData.maxHealth; //get most current maxHealth value

        Vector3 rayDir = lastDir.normalized;
        Ray ray = new Ray(transform.position, rayDir);
        Ray ray1 = new Ray(transform.position, lastDir1);
        Ray ray2 = new Ray(transform.position, lastDir2);
        RaycastHit hit, hit1, hit2;

        if (state != States.interacting)
            //Retain interactObj reference during interact state in case user has angled themselves so that the raycasts do not see the selected object
            //This would previously cause the game to hang so the system would not 'see' the selected object to exit the interact state
        {
            if (state == States.radio || state == States.attacking || state == States.listening || state == States.wakeUp || state == States.stunned
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
        }

        interactIcon.UpdateIcon(state == States.interacting, interactObj);


        //Hit wall check
        //Used to stop the camera from poking outside of bounds
        if (Physics.Raycast(ray, out hit, checkDist)
            //&& !CameraController.instance.GetRotState()
            && !CameraController.instance.GetTriggerState())
        {
            //reset state in case there is an update before a hit can be identified
            CameraController.instance.HittingWall(false);

            //If the raycastHit identifies an object on the Wall layer, set HittingWall = true
            RaycastHit[] hits = Physics.RaycastAll(ray, checkDist, 1 << 10);
            foreach (RaycastHit r_hit in hits)
            {
                if (r_hit.transform.gameObject.layer == 10)
                {
                    CameraController.instance.HittingWall(true);
                    break;
                }
            }
        }
        else
        {
            //Set player to not be hitting a wall
            CameraController.instance.HittingWall(false);
        }


        //Player hurt/death triggers
        if (dead)
            SetState(States.dead);


        //Manage Player Inputs
        if ((state == States.idle || state == States.moving)
            && abilityState == AbilityStates.none
            && !PauseMenuController.instance.isPaused && !FlashlightController.instance.isOn
            && state != States.hurt)
        {
            if (SaveDataController.instance.saveData.abilities.crowbar == true
                && InputController.instance.inputMaster.Player.Melee.triggered)
            {
                animator.SetTrigger("isMelee");
                SetState(States.attacking);
            }

            if (SaveDataController.instance.saveData.abilities.radio == true
                && InputController.instance.inputMaster.Player.Radio.ReadValue<float>() > 0)
            {
                SetState(States.radio);
            }
        }


        //Store player move values
        //Used in FixedUpdate for correct timing with animation flags
        Vector2 move = InputController.instance.inputMaster.Player.Move.ReadValue<Vector2>().normalized;     
        switch (state)
        {
            case States.wakeUp:
                if (!isPlaying("Wake Up"))
                {
                    SetState(States.idle);
                }
                break;
            case States.idle:
                if (move.x != 0f || move.y != 0f)
                {
                    SetState(States.moving);
                }
                break;
            case States.interacting:
                //interactIcon.SetActive(false); //hide interact icon while interacting
                break;
            case States.moving:
                if (!PauseMenuController.instance.isPaused)
                {
                    if (InputController.instance.inputMaster.Player.Run.ReadValue<float>() > 0 && stamina > 0)
                    {
                        running = true;
                        stamina -= (isSeen ? 2f : 1f) * Time.deltaTime;
                    }
                    else { running = false; }

                    if (!running)
                    {
                        if (speed < storedSpeed)
                        {
                            speed += Time.deltaTime * 5f;
                            animator.speed = speed / storedSpeed;
                        }
                        else
                        {
                            speed = storedSpeed;
                            animator.speed = 1f;
                        }
                    }
                    else
                    {
                        speed = runSpeed;
                        animator.speed = 1f;
                    }
                    //speed = running ? runSpeed : storedSpeed;

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
                        running = false;
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
                if (InputController.instance.inputMaster.Player.Radio.ReadValue<float>() <= 0                
                    && abilityState != AbilityStates.isRat || abilityState == AbilityStates.invisible)
                {
                    if (!CameraController.instance.GetRotState() || !CameraController.instance.GetTriggerState()) { CameraController.instance.LoadLastTarget(); }
                    SetState(States.idle);
                }
                break;
            case States.movingObj:
                if (!isPlaying("MoveObj"))
                {
                    SetState(States.idle);
                }
                break;
            case States.stunned:
                if (!isPlaying("Stunned"))
                {
                    print("End stun animation");
                    SetState(States.idle);
                }
                break;
            case States.hurt:
                RadioController.instance.SetActive(false);
                if (!isPlaying("Hurt"))
                {
                    print("End hurt animation");
                    SetState(States.idle);
                }
                break;
            case States.dead:
                CamEffectController.instance.ForceEffect();
                if (health.currentHealth >= 0)
                    health.SetHealth(0);
                break;
            default:
                break;
        }


        //Stamina System
        //If out of stamina, stop running and force recharge
        if (stamina <= 0)
        {
            stamina = 0;
            running = false;
            animator.SetTrigger("isStunned");
            SetState(States.stunned);
        }
        //Recharge stamina if less than max and not running
        if (stamina < 5f && !running)
        {
            stamina += Time.deltaTime * 2f;
        }


        //Handle player interaction inputs
        if (interactObj != null
            && !PauseMenuController.instance.isPaused
            && InputController.instance.inputMaster.Player.Interact.triggered)
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
        if (state != States.moving)
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


        //Injured
        animator.SetBool("injured", health.currentHealth < maxHealth);
        //Moving
        animator.SetBool("isMoving", state == States.moving);
        animator.SetBool("isRunning", state == States.moving && running);
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
        //Gasmask
        gasMaskObj.SetActive(InventoryController.instance.selectedItem != null && InventoryController.instance.selectedItem.itemInstance.itemName.ToLower() == "gasmask"); //toggle gasmask model if the item is equiped
        gasMaskOverlay.SetActive(gasMaskObj.activeSelf);
        //Listening
        //animator.SetBool("isListening", state == States.listening); //toggle listening animation based on bool value
        //Moving Object
        if (isPlaying("MoveObj")) { state = States.movingObj; }

        isSeen = false; //reset seen state if no enemies are currently seeing the player (this is ues for the dynamic camera)
    }


    //Toggle Functions
    public void ToggleAvatar()
    {
        playerAvatar.SetActive(!playerAvatar.activeSelf);
    }


    //Used for the door controller to set exit direction
    public void SetLastDir(Vector3 newDir)
    {
        lastDir = newDir;
    }


    public void SetState(States stateToSet)
    {
        animator.speed = 1f;
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
}
