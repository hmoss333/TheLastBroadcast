using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomController : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip backgroundAudioClip;

    private void Start()
    {
        try
        {
            audioSource = SaveDataController.instance.gameObject.GetComponent<AudioSource>();
        }
        catch { }
    }

    private void OnEnable()
    {
        try
        {
            if (audioSource == null)
                audioSource = SaveDataController.instance.gameObject.GetComponent<AudioSource>();
        }
        catch { Debug.Log("Unable to locate AudioSource"); }

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
