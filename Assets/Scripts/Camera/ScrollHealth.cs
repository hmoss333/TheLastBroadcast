using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI.Table;

public class ScrollHealth : MonoBehaviour
{
    Image image;
    [SerializeField] float scrollSpeed;


    private void Start()
    {
        image = this.GetComponent<Image>();
        image.color = new Color(0, 255f, 0, 255f);
        image.material.mainTextureOffset = new Vector2(0, 0);
    }

    private void Update()
    {
        int currentHealth = PlayerController.instance.GetComponent<Health>().CurrentHealth();
        int maxHealth = SaveDataController.instance.saveData.maxHealth;
        float healthRatio = (float)maxHealth / (float)currentHealth;

        //Healthy
        if (currentHealth >= (maxHealth * 2f)/3f) 
        {
            image.color = new Color(0f, 255f, 0f, 255f); //Green
        }
        //Almost-dead
        else if (currentHealth == 1) 
        {
            image.color = new Color(255f, 0f, 0f, 255f); //Red
        }
        //Hurt
        else if (currentHealth >= (maxHealth) / 5f) 
        {
            image.color = new Color(255f, 255f, 0f, 255f); //Yellow
        }

        float tempSpeed = healthRatio * scrollSpeed;
        image.material.mainTextureOffset = image.material.mainTextureOffset + new Vector2(Time.deltaTime * (-tempSpeed / 10f), 0f);
    }
}
