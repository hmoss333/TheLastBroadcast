using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatAbility : RadioAbilityController
{
    public static RatAbility instance;

    [SerializeField] GameObject ratObj;
    [SerializeField] private Vector3 defaultRatPos;
    private RoomController playerRoom;

    [SerializeField] private bool isRat;
    private float checkFrequency;
    private float checkOffset = 0.5f;
    [SerializeField] float checkTime;
    private float tempCheckTime = 0f;


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
        defaultRatPos = ratObj.transform.localPosition;
    }

    private void Update()
    {
        if (RadioController.instance.abilityMode
            && abilityData.isActive
            && !isRat)
        {
            if ((RadioController.instance.currentFrequency < checkFrequency + checkOffset && RadioController.instance.currentFrequency > checkFrequency - checkOffset)
                && SaveDataController.instance.saveData.abilities.radio == true //does the player have the radio object; useful if the player loses the radio at some point)                                                      
                && RadioController.instance.isActive) //is the radio active (shouldn't be broadcasting if it is not turned on))
            {
                RadioController.instance.UsingAbility();
                tempCheckTime += Time.deltaTime;
                if (tempCheckTime >= checkTime)
                {
                    playerRoom = FindObjectOfType<RoomController>(); //Grab the current player room; should only see current room controller                   
                    isRat = true;
                    CameraController.instance.SetTarget(ratObj.transform);
                    CameraController.instance.SetLastTarget(ratObj.transform);
                    tempCheckTime = 0f;
                }
            }
            else
            {
                tempCheckTime = 0f;
            }
        }

        if (isRat)
        {
            PlayerController.instance.SetAbilityState(PlayerController.AbilityStates.isRat);
            PlayerController.instance.GetComponent<Rigidbody>().useGravity = false;

            if (InputController.instance.inputMaster.Player.RadioSpecial.triggered) { EndAbility(); }
        }

        ratObj.SetActive(isRat); //Toggle rat prefab based on isRat state
    }

    public void EndAbility()
    {
        isRat = false;
        if (playerRoom != SceneInitController.instance.currentRoom)
        {
            RoomController tempRoom = SceneInitController.instance.currentRoom;
            tempRoom.gameObject.SetActive(false);
            SceneInitController.instance.SetCurrentRoom(playerRoom);
            playerRoom.gameObject.SetActive(true);
        }

        PlayerController.instance.GetComponent<Rigidbody>().useGravity = true;
        ratObj.transform.localPosition = defaultRatPos; //Reset rat position back to default when ending rat ability

        CameraController.instance.SetTarget(PlayerController.instance.lookTransform);
        CameraController.instance.SetLastTarget(PlayerController.instance.lookTransform);
        CameraController.instance.SetFocus(false);
        CameraController.instance.SetRotation(false);
        CameraController.instance.SetTriggerState(false);

        PlayerController.instance.SetAbilityState(PlayerController.AbilityStates.none);
        PlayerController.instance.SetState(PlayerController.States.idle);
    }
}
