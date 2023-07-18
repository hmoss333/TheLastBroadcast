using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepController : MonoBehaviour
{
    [SerializeField] AudioClip clip;
    [SerializeField] AudioSource audioSource;


    public void PlayStepClip()
    {
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();
    }
}
