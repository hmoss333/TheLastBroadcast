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
    [SerializeField] ZombieController enemy;


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
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, checkDist, layer))
                {
                    enemy = hit.transform.gameObject.GetComponent<ZombieController>();
                }
                else
                {
                    enemy = null;
                }

                if (enemy != null && enemy.SeePlayer())
                {
                    enemy.Stun();
                }

                Debug.DrawRay(transform.position, forwardDir, Color.yellow);
            }
        }

        lightSource.enabled = isOn;
        flashlightObj.SetActive(isOn);
        PlayerController.instance.animator.SetBool("flashlight", isOn);
    }
}
