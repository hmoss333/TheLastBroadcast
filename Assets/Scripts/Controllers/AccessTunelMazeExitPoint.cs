using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccessTunelMazeExitPoint : MonoBehaviour
{
    AudioSource audioSource;
    [SerializeField] bool isExit;
    [SerializeField] Transform initPoint;


    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        Initialize();
    }

    public void Initialize()
    {
        isExit = true;
        audioSource.mute = true;
    }

    public void SetFalse()
    {
        isExit = false;
        audioSource.mute = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (isExit)
            {
                AccessTunelMazeController.instance.Correct(initPoint);
            }
            else
            {
                AccessTunelMazeController.instance.Incorrect(initPoint);
            }
        }
    }
}
