using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFocus : MonoBehaviour
{
    bool hasFocused, toggleAvatar;
    ZombieController enemyToFocus;
    float timeToFocus = 2.5f;


    private void Start()
    {
        enemyToFocus = GetComponent<ZombieController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasFocused && enemyToFocus.SeePlayer())
        {
            CameraController.instance.SetTarget(transform);
            if (!toggleAvatar)
            {
                PlayerController.instance.ToggleAvatar();
                toggleAvatar = true;
            }
            timeToFocus -= Time.deltaTime;
            if (timeToFocus <= 0)
            {
                PlayerController.instance.ToggleAvatar();
                CameraController.instance.SetTarget(PlayerController.instance.lookTransform);
                hasFocused = true;
            }
        }
    }
}
