using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedDestroy : MonoBehaviour
{
    [SerializeField] float time;

    private void Update()
    {
        time -= Time.deltaTime;
        if (time <= 0f)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }
}
