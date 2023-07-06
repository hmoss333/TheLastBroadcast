using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip backgroundAudioClip;


    private void OnEnable()
    {
        audioSource = SaveDataController.instance.gameObject.GetComponent<AudioSource>();

        if (audioSource
            && backgroundAudioClip
            && audioSource.clip != backgroundAudioClip)
        {
            audioSource.Stop();
            audioSource.clip = backgroundAudioClip;
            audioSource.Play();
        }
    }
}
