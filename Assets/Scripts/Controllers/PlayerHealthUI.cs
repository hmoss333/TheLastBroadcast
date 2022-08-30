using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] Health health;
    [SerializeField] int maxHealth;
    [SerializeField] RawImage healthImage;
    [SerializeField] float movementSpeed = 1;

    [SerializeField] Color high;
    [SerializeField] Color medium;
    [SerializeField] Color low;



    private void Update()
    {
        int checkHealth = health.CurrentHealth();
        float tempSpeed = (float)maxHealth / (float)checkHealth;
        movementSpeed = tempSpeed / 4f;

        Color healthColor = new Color();
        if (checkHealth >= 4)
        {
            healthColor = high;
        }
        else if (checkHealth >= 2)
        {
            healthColor = medium;
        }
        else
        {
            healthColor = low;
        }

        healthImage.color = healthColor;

        Rect rect = healthImage.uvRect;
        rect.x += Time.deltaTime * -movementSpeed;
        healthImage.uvRect = rect;
    }
}
