using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccessTunelMazeExitPoint : MonoBehaviour
{
    AudioSource audioSource;
    [SerializeField] bool isExit;
    [SerializeField] Transform initPoint;
    [SerializeField] AccessTunelMazeController mazeController;


    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        Initialize();
    }

    public void Initialize()
    {
        isExit = true;
        audioSource.mute = false;
    }

    public void SetFalse()
    {
        isExit = false;
        audioSource.mute = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (isExit)
            {
                mazeController.Correct(initPoint);
            }
            else
            {
                mazeController.Incorrect(initPoint);
            }
        }
    }
}
