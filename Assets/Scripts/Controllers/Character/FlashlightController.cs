using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class FlashlightController : MonoBehaviour
{
    public static FlashlightController instance;

    public bool isOn { get; private set; }
    [SerializeField] Light lightSource;
    [SerializeField] GameObject flashlightObj;
    [SerializeField] float checkDist;
    [SerializeField] private LayerMask layer;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        //else
        //    Destroy(this.gameObject);            
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
        if (SaveDataController.instance.saveData.abilities.flashlight && PlayerController.instance.state != PlayerController.States.radio)
        {
            isOn = PlayerController.instance.inputMaster.Player.Flashlight.ReadValue<float>() > 0
                ? true
                : false;

            if (isOn)
            {
                Vector3 forwardDir = transform.forward;
                Ray ray = new Ray(transform.position, forwardDir);
                //RaycastHit hit;
                RaycastHit[] hits;
                Debug.DrawLine(transform.position, forwardDir * checkDist, Color.blue);


                hits = Physics.RaycastAll(transform.position, transform.forward, checkDist, layer);
                hits = Physics.RaycastAll(transform.position, transform.forward, checkDist, layer);
                for (int i = 0; i < hits.Length; i++)
                {
                    RaycastHit hit = hits[i];
                    CharacterController tempEnemy = hit.transform.gameObject.GetComponent<CharacterController>();
                    tempEnemy.StunCharacter();
                }
            }
        }

        lightSource.enabled = isOn;
        flashlightObj.SetActive(isOn);
        PlayerController.instance.animator.SetBool("flashlight", isOn);
    }
}
