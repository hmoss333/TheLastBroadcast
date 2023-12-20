using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//TODO
//Once FMOD has been setup and configured all of these functions will need to be updated to work with the new framework
//If need be, these refrences can also be swapped out for direct FMOD library functions where applicable
public class AudioController : MonoBehaviour
{
    public static AudioController instance;

    [SerializeField] AudioSource audioSource;

    private void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    public void SetAudioVolume(float volume)
    {
        audioSource.volume = volume;
    }

    public void LoopClip(bool loop)
    {
        audioSource.loop = loop;
    }

    public void PlayClip(AudioClip clip)
    {
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();
    }

    public void StopClip()
    {
        audioSource.Stop();
    }
}
