using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Light))]
public class FlashlightController : MonoBehaviour
{
    public static FlashlightController instance;

    public bool isOn, isCharging;// { get; private set; }
    Light lightSource;
    [SerializeField] GameObject flashlightObj, flashlightTrigger;
    [SerializeField] Image flashlightOverlay, flashlightIcon;
    [SerializeField] private LayerMask layer;
    [SerializeField] float checkDist;
    [Range(0, 15f)]
    [SerializeField] float flashlightTime;
    [SerializeField] float flickerVal, rechargeRate;
    Coroutine flickerRoutine;
    AudioSource audioSource;


    private void Awake()
    {
        if (instance == null)
            instance = this;         
    }

    private void Start()
    {
        isOn = false;
        audioSource = GetComponent<AudioSource>();
        lightSource = GetComponent<Light>();
        lightSource.enabled = false;
        flashlightObj.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!PauseMenuController.instance.isPaused)
        {
            if (SaveDataController.instance.saveData.abilities.flashlight
                && InputController.instance.inputMaster.Player.Flashlight.triggered)
            {
                audioSource.Stop();
                audioSource.Play();
            }

            if (SaveDataController.instance.saveData.abilities.flashlight
                && (PlayerController.instance.state == PlayerController.States.idle || PlayerController.instance.state == PlayerController.States.moving)
                && PlayerController.instance.abilityState == PlayerController.AbilityStates.none)
            {
                isOn = InputController.instance.inputMaster.Player.Flashlight.ReadValue<float>() > 0
                    && !PlayerController.instance.running
                    && flashlightTime > 0f
                    && !isCharging
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

                    if (flashlightTime > 15f)
                    {
                        flashlightTime = 15f;
                        isCharging = false;
                    }
                }
            }

            lightSource.enabled = isOn && flashlightTime >= flickerVal;
            flashlightObj.SetActive(isOn);
            flashlightTrigger.SetActive(isOn);
            PlayerController.instance.animator.SetBool("flashlight", isOn);
            flashlightOverlay.gameObject.SetActive(SaveDataController.instance.saveData.abilities.flashlight);
            flashlightIcon.fillAmount = flashlightTime / 15f;
        }
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
