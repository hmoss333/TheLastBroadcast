using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatAbility : RadioAbilityController
{
    public static RatAbility instance;

    [SerializeField] GameObject ratPrefab;
    private GameObject ratObj;
    private RoomController playerRoom;

    [SerializeField] private bool isRat;
    private float checkFrequency, chargeCost;
    private float checkOffset = 0.5f;
    [SerializeField] float checkTime;
    private float tempCheckTime = 0f;
    public bool isUsing { get; private set; }


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    override public void Start()
    {
        base.Start();

        checkFrequency = abilityData.frequency;
        chargeCost = abilityData.chargeCost;
    }

    public void SetTuning(bool isOn)
    {
        isUsing = isOn;
    }

    private void Update()
    {
        if (RadioController.instance.abilityMode
            && abilityData.isActive
            && !isRat
            && RadioController.instance.currentCharge > chargeCost)
        {
            if ((RadioController.instance.currentFrequency < checkFrequency + checkOffset && RadioController.instance.currentFrequency > checkFrequency - checkOffset)
                && SaveDataController.instance.saveData.abilities.radio == true //does the player have the radio object; useful if the player loses the radio at some point)                                                      
                && RadioController.instance.isActive) //is the radio active (shouldn't be broadcasting if it is not turned on))
            {
                isUsing = true;
                tempCheckTime += Time.deltaTime;
                if (tempCheckTime >= checkTime)
                {
                    RadioController.instance.ModifyCharge(-chargeCost);
                    playerRoom = FindObjectOfType<RoomController>(); //should grab only active room controller                   
                    isRat = true;
                    isUsing = false;
                    Vector3 newPos = PlayerController.instance.transform.position;//new Vector3(PlayerController.instance.transform.position.x, PlayerController.instance.transform.position.y + 0.1f, PlayerController.instance.transform.position.z);
                    ratObj = Instantiate(ratPrefab, newPos, Quaternion.identity);
                    CameraController.instance.SetTarget(ratObj.transform);
                    CameraController.instance.SetLastTarget(ratObj.transform);
                    tempCheckTime = 0f;
                }
            }
            else
            {
                isUsing = false;
                tempCheckTime = 0f;
            }
        }
        else if (!RadioController.instance.abilityMode && !isRat)
        {
            //If the player turns off the radio before the ability has triggered
            isUsing = false;
        }

        if (PlayerController.instance.inputMaster.Player.RadioSpecial.triggered && isRat)
        {
            EndAbility();
        }

        if (isRat)
            PlayerController.instance.SetAbilityState(PlayerController.AbilityStates.isRat);


        //Enable static effect
        if (isUsing)
        {
            RadioController.instance.UsingAbility();
            PlayerController.instance.GetComponent<Rigidbody>().useGravity = !isRat; //keep the player from falling if they are off-screen
        }
    }

    public void EndAbility()
    {
        isRat = false;
        RoomController[] rooms = FindObjectsOfType<RoomController>();
        foreach (RoomController room in rooms)
        {
            room.gameObject.SetActive(false);
        }
        playerRoom.gameObject.SetActive(true);

        Destroy(ratObj); //expensive; maybe add the rat as a child object of the player that gets enabled/disabled with it's position reset

        CameraController.instance.SetTarget(PlayerController.instance.lookTransform);
        CameraController.instance.SetLastTarget(PlayerController.instance.lookTransform);
        CameraController.instance.SetFocus(false);
        CameraController.instance.SetRotation(false);
        CameraController.instance.SetTriggerState(false);

        PlayerController.instance.SetAbilityState(PlayerController.AbilityStates.none);
        PlayerController.instance.SetState(PlayerController.States.idle);
    }
}
