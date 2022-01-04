using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadioOverlay_Controller : MonoBehaviour
{
    public static RadioOverlay_Controller instance;

    [SerializeField] float yOffSet;
    [SerializeField] bool isActive;

    [SerializeField] Image overlayPanel;
    [SerializeField] RawImage stationScroll;
    float xInput;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = new Vector3(PlayerController.instance.transform.position.x, PlayerController.instance.transform.position.y + yOffSet);

        if (isActive)
        {
            xInput = Input.GetAxis("Horizontal");

            //Use horizontal input to offset stationScroll UI element
        }
    }

    public void ToggleOn()
    {
        isActive = !isActive;

        overlayPanel.color = new Color(overlayPanel.color.r, overlayPanel.color.g, overlayPanel.color.b, isActive ? 1f : 0f);
    }
}
