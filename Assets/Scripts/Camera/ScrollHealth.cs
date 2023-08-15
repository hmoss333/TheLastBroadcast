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

        float tempSpeed;
        if (currentHealth >= 4)
        {
            image.color = new Color(0f, 255f, 0f, 255f);
            tempSpeed = scrollSpeed;
        }
        else if (currentHealth >= 2)
        {
            image.color = new Color(255f, 255f, 0f, 255f);
            tempSpeed = 2f * scrollSpeed;
        }
        else
        {
            image.color = new Color(255f, 0f, 0f, 255f);
            tempSpeed = 3f * scrollSpeed;
        }

        image.material.mainTextureOffset = image.material.mainTextureOffset + new Vector2(Time.deltaTime * (-tempSpeed / 10f), 0f);
    }
}
