using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering;


//TODO
//Once FMOD has been setup and configured all of these functions will need to be updated to work with the new framework
//If need be, these refrences can also be swapped out for direct FMOD library functions where applicable
public class AudioController : MonoBehaviour
{
    public static AudioController instance;

    [SerializeField] AudioSource audioSource;
    [SerializeField] private List<AudioLayer> audioLayers;


    private void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }


    private void OnValidate()
    {
        AudioSource[] sources = gameObject.GetComponentsInChildren<AudioSource>();
        List<AudioSource> validAudio = new List<AudioSource>();
        for (int i = 0; i < audioLayers.Count; i++)
        {
            for (int j = 0; j < sources.Length; j++)
            {
                if (audioLayers[i].audioSource == sources[j])
                {
                    validAudio.Add(sources[j]);
                    break;
                }
            }
        }

        foreach (AudioSource source in sources)
        {
            if (!validAudio.Contains(source))
                Destroy(source);
        }
    }

    private void Update()
    {
        foreach (AudioLayer layer in audioLayers)
        {
            if (layer.audioSource == null)
            {
                AudioSource newSource = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
                newSource.spatialBlend = 0;
                layer.audioSource = newSource;
            }

            layer.audioSource.clip = layer.audioClip;
            layer.audioSource.volume = layer.volume;
            layer.audioSource.mute = layer.mute;
            layer.audioSource.loop = layer.loop;

            //Start audio playback if layer has been added to the loop
            if (!layer.audioSource.isPlaying)
                layer.audioSource.Play(); 
        }


        CleanAudioSources();
    }


    ///Individual Audio Clip Controls
    ///TODO phase these out in favor of the layer system
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


    ///Audio Layer System Controls
    public void AddLayer(AudioClip clip)
    {
        AudioLayer newLayer = new AudioLayer();
        newLayer.audioClip = clip;
        newLayer.volume = 1f;
        newLayer.mute = false;
        newLayer.loop = true;

        audioLayers.Add(newLayer);
    }

    public void AddLayer(AudioClip clip, float volume, bool mute, bool loop)
    {
        AudioLayer newLayer = new AudioLayer();
        newLayer.audioClip = clip;
        newLayer.volume = volume;
        newLayer.mute = mute;
        newLayer.loop = loop;

        audioLayers.Add(newLayer);
    }

    //TODO improve this system. Dictionary may work better
    public void RemoveLayer(int layerID)
    {
        Destroy(audioLayers[layerID].audioSource);
        audioLayers.Remove(audioLayers[layerID]);
    }

    private void CleanAudioSources()
    {
        AudioSource[] sources = gameObject.GetComponentsInChildren<AudioSource>();
        List<AudioSource> validAudio = new List<AudioSource>();
        for (int i = 0; i < audioLayers.Count; i++)
        {
            for (int j = 0; j < sources.Length; j++)
            {
                if (audioLayers[i].audioSource == sources[j])
                {
                    validAudio.Add(sources[j]);
                    break;
                }
            }
        }

        foreach (AudioSource source in sources)
        {
            if (!validAudio.Contains(source))
                Destroy(source);
        }
    }
}


[System.Serializable]
public class AudioLayer
{
    public AudioSource audioSource;
    public AudioClip audioClip;
    [Range(0, 1)]
    public float volume;
    public bool mute, loop;
}