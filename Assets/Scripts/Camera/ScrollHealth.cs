using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

public class ScrollHealth : MonoBehaviour
{
    public static ScrollHealth instance;

    Image image;
    [SerializeField] float scrollSpeed;

    [NaughtyAttributes.HorizontalLine]

    [SerializeField] GameObject healthPanel;
    [SerializeField] Image healthUI;
    [SerializeField] Sprite deadHealth;
    [SerializeField] Vector2 inactivePos, activePos;
    [SerializeField] float slideSpeed;
    public bool isActive, toggled;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    private void Start()
    {
        image = this.GetComponent<Image>();
        image.color = new Color(0, 255f, 0, 255f);
        image.material.mainTextureOffset = new Vector2(0, 0);
        isActive = false;
    }

    private void Update()
    {
        if (PlayerController.instance.inputMaster.Player.Health.triggered)
        {
            toggled = !toggled;
        }

        if (PlayerController.instance.state == PlayerController.States.wakeUp
            || PlayerController.instance.state == PlayerController.States.listening
            || PlayerController.instance.state == PlayerController.States.interacting)
        {
            toggled = false;
            isActive = false;
        }
        else if (PlayerController.instance.IsSeen()
            || PlayerController.instance.state == PlayerController.States.hurt
            //|| PlayerController.instance.state == PlayerController.States.consuming
            || toggled)
        {
            isActive = true;
        }

        int currentHealth = PlayerController.instance.GetComponent<Health>().currentHealth;
        int maxHealth = SaveDataController.instance.saveData.maxHealth;
        float healthRatio = (float)maxHealth / (float)currentHealth;

        //Healthy
        if (currentHealth == maxHealth)
        {
            image.color = new Color(0f, 255f, 0f, 255f); //Green
        }
        //Almost-dead
        else if (currentHealth <= 1) 
        {
            image.color = new Color(255f, 0f, 0f, 255f); //Red
        }
        //Hurt
        else
        {
            image.color = new Color(255f, 255f, 0f, 255f); //Yellow
        }

        if (PlayerController.instance.dead)
        {
            healthUI.sprite = deadHealth;
        }

        float tempSpeed = healthRatio * scrollSpeed;
        image.material.mainTextureOffset = image.material.mainTextureOffset + new Vector2(Time.deltaTime * (-tempSpeed / 10f), 0f);


        //Move health UI panel into position based on active state
        healthPanel.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(healthPanel.GetComponent<RectTransform>().anchoredPosition,
            isActive ? activePos : inactivePos, slideSpeed * Time.deltaTime);
    }

    //bool CheckIsActive()
    //{
    //    if (PlayerController.instance.inputMaster.Player.Health.triggered)
    //    {
    //        toggled = !toggled;
    //    }

    //    if (PlayerController.instance.state == PlayerController.States.wakeUp
    //        || PlayerController.instance.state == PlayerController.States.listening
    //        || PlayerController.instance.state == PlayerController.States.interacting)
    //    {
    //        toggled = false;
    //        return false;
    //    }
    //    else if (PlayerController.instance.IsSeen()
    //        || PlayerController.instance.state == PlayerController.States.hurt
    //        //|| PlayerController.instance.state == PlayerController.States.consuming
    //        || toggled)
    //    {
    //        return true;
    //    }
    //    else
    //    {
    //        return false;
    //    }
    //}
}
