using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTriggerZone : MonoBehaviour
{
    [SerializeField] List<AudioClip> layers;
    [SerializeField] List<AudioLayer> aLayers;
    [SerializeField] bool clearLayers;


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            List<AudioLayer> audioLayers = AudioController.instance.GetLayers();

            //If clips are already found in the current audio layers, do not make any changes
            foreach (AudioLayer al in aLayers)
            {
                AudioLayer checkLayer = audioLayers.Find(x => x.audioClip == al.audioClip);

                if (checkLayer != null)
                    return;
            }

            //Otherwise, check clearLayer flag and add new layers
            if (clearLayers)
                AudioController.instance.PopAllLayers();

            for (int i = 0; i < aLayers.Count; i++)
            {
                AudioController.instance.AddLayer(
                    aLayers[i].audioClip,
                    aLayers[i].volume,
                    aLayers[i].mute,
                    aLayers[i].loop);
            }
        }
    }

    private void OnValidate()
    {
        if (aLayers.Count == 0 && layers.Count > 0)
        {
            for (int i = 0; i < layers.Count; i++)
            {
                AudioLayer tempLayer = new AudioLayer();
                tempLayer.ID = i;
                tempLayer.audioClip = layers[i];
                tempLayer.volume = 1f; //default volume to max
                tempLayer.mute = false; //default to false
                tempLayer.loop = false; //default to false

                aLayers.Add(tempLayer);
            }
        }
    }
}
