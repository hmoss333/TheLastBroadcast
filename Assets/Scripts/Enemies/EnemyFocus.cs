using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFocus : MonoBehaviour
{
    bool hasFocused;
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
            timeToFocus -= Time.deltaTime;
            if (timeToFocus <= 0)
            {
                CameraController.instance.LoadLastTarget();//SetTarget(PlayerController.instance.lookTransform);
                hasFocused = true;
            }
        }
    }
}
