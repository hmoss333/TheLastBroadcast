using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractIcon_Controller : MonoBehaviour
{
    public static InteractIcon_Controller instance;

    [SerializeField] bool isActive;
    [SerializeField] float yOffSet;
    bool interacting;
    GameObject targetObj;
    Image icon;
    [SerializeField] float fadeTime;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }

    private void Start()
    {
        icon = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = new Vector3(PlayerController.instance.transform.position.x, PlayerController.instance.transform.position.y + yOffSet);

        icon.color = new Color(icon.color.r, icon.color.g, icon.color.b, Mathf.PingPong(Time.time, fadeTime)/fadeTime);
    }

    public void UpdateIcon(bool isInteracting, GameObject interactObject)
    {
        interacting = isInteracting;
        targetObj = interactObject;

        if (targetObj != null && !interacting)
            isActive = true;
        else
            isActive = false;

        icon.enabled = isActive;
    }
}
