using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadioOverlay_Controller : MonoBehaviour
{
    public static RadioOverlay_Controller instance;

    [SerializeField] float yOffSet;
    public bool isActive;

    [SerializeField] GameObject overlayPanel;
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

            float xPos = stationScroll.uvRect.x;
            stationScroll.uvRect = new Rect(xPos += xInput * Time.deltaTime, 0, 1, 1);
        }

        overlayPanel.SetActive(isActive);
    }

    public void ToggleOn()
    {
        isActive = !isActive;
    }
}
