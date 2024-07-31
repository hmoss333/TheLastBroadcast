using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Light))]
public class FlashlightController : MonoBehaviour
{
    public static FlashlightController instance;

    public bool isOn;//{ get; private set; }
    Light lightSource;
    [SerializeField] GameObject flashlightObj, flashlightTrigger;
    [SerializeField] Image flashlightOverlay, flashlightIcon;
    [SerializeField] private LayerMask layer;
    [SerializeField] float checkDist;
    [Range(0, 15f)]
    public float flashlightCharge, maxCharge;// { get; private set; }
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
        maxCharge = SaveDataController.instance.saveData.maxCharge;
        flashlightCharge = maxCharge;
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
                    && flashlightCharge > 0f
                        ? true
                        : false;

                if (isOn)
                {
                    flashlightCharge -= 0.65f * Time.deltaTime;                   
                    if (flashlightCharge <= flickerVal)
                    {
                        //Start randomly flickering flashlight
                        if (flickerRoutine == null)
                            flickerRoutine = StartCoroutine(FlickerLight());
                    }

                    if (flashlightCharge <= 0f) { isOn = false; }
                }
            }

            lightSource.enabled = isOn && flashlightCharge >= flickerVal;
            flashlightObj.SetActive(isOn);
            flashlightTrigger.SetActive(isOn);
            PlayerController.instance.animator.SetBool("flashlight", isOn);
            flashlightOverlay.gameObject.SetActive(SaveDataController.instance.saveData.abilities.flashlight);
            flashlightIcon.fillAmount = flashlightCharge / 15f;
        }
    }

    IEnumerator FlickerLight()
    {
        while (flashlightCharge < flickerVal && isOn)
        {
            lightSource.enabled = !lightSource.enabled;
            float randVal = Random.Range(0.01f, 0.05f);
            yield return new WaitForSeconds(randVal);
        }

        flickerRoutine = null;
    }

    public void ModifyCharge(float chargeVal)
    {
        flashlightCharge += chargeVal;
        if (flashlightCharge >= maxCharge)
            flashlightCharge = maxCharge;
        else if (flashlightCharge < 0f)
            flashlightCharge = 0f;
    }
}
