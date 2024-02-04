using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTriggerZone : MonoBehaviour
{
    [SerializeField] List<AudioClip> layers;
    [SerializeField] bool clearLayers;


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            List<AudioClip> clips = AudioController.instance.GetLayerClips();

            //If clips are already found in the current audio layers, do not make any changes
            foreach (AudioClip clip in layers)
            {
                if (clips.Contains(clip))
                    return;
            }


            //Otherwise, check clearLayer flag and add new layers
            if (clearLayers)
                AudioController.instance.PopAllLayers();

            for (int i = 0; i < layers.Count; i++)
            {
                AudioController.instance.AddLayer(layers[i]);
            }
        }
    }
}
