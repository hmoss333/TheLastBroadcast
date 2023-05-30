using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightController : MonoBehaviour
{
    public static FlashlightController instance;

    public bool isOn;// { get; private set; }
    [SerializeField] Light lightSource;
    [SerializeField] GameObject flashlightObj;
    [SerializeField] private LayerMask layer;
    [SerializeField] float checkDist;
    [Range(0, 10f)]
    [SerializeField] float flashlightTime;
    [SerializeField] float flickerVal, offVal;
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
            isOn = PlayerController.instance.inputMaster.Player.Flashlight.ReadValue<float>() > 0 && flashlightTime > offVal
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

                flashlightTime -= Time.deltaTime;
                if (flashlightTime <= flickerVal)
                {
                    //Start randomly flickering flashlight
                    if (flickerRoutine == null)
                        flickerRoutine = StartCoroutine(FlickerLight());
                }
                if (flashlightTime <= offVal)
                {
                    isOn = false;
                }
            }
            else
            {
                flashlightTime += Time.deltaTime / 2f;
                if (flashlightTime > 10f)
                    flashlightTime = 10f; //need to rewrite this to not use hard-coded values
            }
        }

        lightSource.enabled = isOn;
        flashlightObj.SetActive(isOn);
        PlayerController.instance.animator.SetBool("flashlight", isOn);
    }

    IEnumerator FlickerLight()
    {
        print("Start flickering");

        while (isOn)
        {
            print($"Light is {lightSource.gameObject.activeSelf}");
            lightSource.enabled = !lightSource.enabled;
            float randVal = Random.Range(0, 0.5f);

            yield return new WaitForSeconds(randVal);
        }

        flickerRoutine = null;
        print("End flickering");
    }
}
