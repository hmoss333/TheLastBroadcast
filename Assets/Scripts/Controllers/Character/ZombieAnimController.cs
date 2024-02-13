using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAnimController : MonoBehaviour
{
    [SerializeField] AudioClip recognizeClip;
    [SerializeField] AudioSource audioSource;


    public void Recognize()
    {
        audioSource.Stop();
        audioSource.clip = recognizeClip;
        audioSource.loop = false;
        audioSource.Play();
    }
}
