using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    AudioSource audioSource;
    [SerializeField] AudioClip backgroundAudioClip;

    private void Start()
    {
        audioSource = SaveDataController.instance.gameObject.GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        if (audioSource == null)
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
