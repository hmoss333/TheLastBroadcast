using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccessTunelMazeExitPoint : MonoBehaviour
{
    AudioSource audioSource;
    [SerializeField] bool isExit;
    [SerializeField] Transform initPoint;
    [SerializeField] AccessTunelMazeController mazeController;
    [SerializeField] Light correctLight, correctLight2;
    public GameObject entrancePipe, exitPipe;


    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        Initialize();
    }

    public void Initialize()
    {
        isExit = true;
        audioSource.mute = false;
        correctLight.enabled = false;
        correctLight2.enabled = false;
        //exitPipe.SetActive(true);
        entrancePipe.SetActive(true);
    }

    public void SetFalse()
    {
        isExit = false;
        audioSource.mute = true;
        correctLight.enabled = true;
        correctLight2.enabled = true;
        //exitPipe.SetActive(false);
        entrancePipe.SetActive(false);
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
