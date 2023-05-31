using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightController : MonoBehaviour
{
    public static FlashlightController instance;

    public bool isOn, isCharging;// { get; private set; }
    [SerializeField] Light lightSource;
    [SerializeField] GameObject flashlightObj;
    [SerializeField] private LayerMask layer;
    [SerializeField] float checkDist;
    [Range(0, 10f)]
    [SerializeField] float flashlightTime;
    float flashLightTimeMax;
    [SerializeField] float flickerVal, rechargeRate;//, offVal;
    Coroutine flickerRoutine;


    private void Awake()
    {
        if (instance == null)
            instance = this;         
    }

    private void Start()
    {
        isOn = false;
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
                Vector3 forwardDir = transform.forward;
                Ray ray = new Ray(transform.position, forwardDir);
                RaycastHit[] hits;
                Debug.DrawRay(transform.position, forwardDir * checkDist, Color.blue);

                hits = Physics.RaycastAll(transform.position, transform.forward, checkDist, layer);
                for (int i = 0; i < hits.Length; i++)
                {
                    RaycastHit hit = hits[i];
                    CharacterController tempEnemy = hit.transform.gameObject.GetComponent<CharacterController>();
                    tempEnemy.StunCharacter();
                }

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
        PlayerController.instance.animator.SetBool("flashlight", isOn);
    }

    IEnumerator FlickerLight()
    {
        while (flashlightTime < flickerVal && isOn)
        {
            lightSource.enabled = !lightSource.enabled;
            float randVal = Random.Range(0f, 0.4f);
            yield return new WaitForSeconds(randVal);
        }

        flickerRoutine = null;
    }
}
