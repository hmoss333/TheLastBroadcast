using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class FlashlightController : MonoBehaviour
{
    public static FlashlightController instance;

    public bool isOn, isCharging;// { get; private set; }
    Light lightSource;
    [SerializeField] GameObject flashlightObj, flashlightTrigger;
    [SerializeField] private LayerMask layer;
    [SerializeField] float checkDist;
    [Range(0, 10f)]
    [SerializeField] float flashlightTime;
    [SerializeField] float flickerVal, rechargeRate;
    Coroutine flickerRoutine;


    private void Awake()
    {
        if (instance == null)
            instance = this;         
    }

    private void Start()
    {
        isOn = false;
        lightSource = GetComponent<Light>();
        lightSource.enabled = false;
        flashlightObj.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (SaveDataController.instance.saveData.abilities.flashlight
            && (PlayerController.instance.state == PlayerController.States.idle || PlayerController.instance.state == PlayerController.States.moving)
            && PlayerController.instance.abilityState == PlayerController.AbilityStates.none)
        {
            isOn = PlayerController.instance.inputMaster.Player.Flashlight.ReadValue<float>() > 0 && flashlightTime > 0f && !isCharging
                ? true
                : false;

            if (isOn)
            {
                flashlightTime -= 0.65f * Time.deltaTime;
                if (flashlightTime <= flickerVal)
                {
                    //Start randomly flickering flashlight
                    if (flickerRoutine == null)
                        flickerRoutine = StartCoroutine(FlickerLight());
                }

                if (flashlightTime <= 0f)
                {
                    isCharging = true;
                    isOn = false;
                }
            }
            else
            {
                flashlightTime += rechargeRate * Time.deltaTime;

                if (flashlightTime > 10f)
                {
                    flashlightTime = 10f;
                    isCharging = false;
                }
            }
        }

        lightSource.enabled = isOn && flashlightTime >= flickerVal;
        flashlightObj.SetActive(isOn);
        flashlightTrigger.SetActive(isOn);
        PlayerController.instance.animator.SetBool("flashlight", isOn);
    }

    IEnumerator FlickerLight()
    {
        while (flashlightTime < flickerVal && isOn)
        {
            lightSource.enabled = !lightSource.enabled;
            float randVal = Random.Range(0.01f, 0.05f);
            yield return new WaitForSeconds(randVal);
        }

        flickerRoutine = null;
    }
}
