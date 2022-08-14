using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatAbility : RadioAbilityController
{
    public static RatAbility instance;

    [SerializeField] GameObject ratPrefab;
    private GameObject ratObj;

    [SerializeField] bool isRat;
    private float checkFrequency;
    private float checkOffset = 0.5f;
    [SerializeField] float checkTime;
    private float tempCheckTime;
    [HideInInspector] public bool isUsing;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }

    override public void Start()
    {
        base.Start();

        tempCheckTime = checkTime;
        checkFrequency = abilityData.frequency;
    }

    public void SetTuning(bool isOn)
    {
        isUsing = isOn;
    }

    private void Update()
    {
        if (RadioController.instance.abilityMode && !isRat)
        {
            if ((RadioController.instance.currentFrequency < checkFrequency + checkOffset && RadioController.instance.currentFrequency > checkFrequency - checkOffset)
                && SaveDataController.instance.saveData.abilities.radio == true //does the player have the radio object; useful if the player loses the radio at some point)                                                      
                && RadioController.instance.isActive) //is the radio active (shouldn't be broadcasting if it is not turned on))
            {
                isUsing = true;
                checkTime -= Time.deltaTime;
                if (checkTime < 0)
                {
                    isRat = true;
                    Vector3 newPos = new Vector3(PlayerController.instance.transform.position.x, 0.5f, PlayerController.instance.transform.position.z);
                    ratObj = Instantiate(ratPrefab, newPos, Quaternion.identity);
                    CameraController.instance.SetTarget(ratObj);
                    checkTime = tempCheckTime;
                }
            }
            else
            {
                isUsing = false;
                checkTime = tempCheckTime;
            }
        }
        else if (!RadioController.instance.abilityMode && !isRat)
        {
            //If the player turns off the radio before the ability has triggered
            isUsing = false;
        }

        if (Input.GetButtonDown("RadioSpecial") && isRat)
        {
            print("End rat ability");
            CameraController.instance.SetTarget(PlayerController.instance.gameObject);
            Destroy(ratObj);
            PlayerController.instance.usingRadio = false;
            isRat = false;
        }

        //Toggle player interaction state based on invisibility status
        PlayerController.instance.interacting = isRat;
    }
}
