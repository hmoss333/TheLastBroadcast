using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberStationTriggerZone : MonoBehaviour
{
    [SerializeField] AudioClip stationAudio;
    AudioClip staticAudio;

    private void Start()
    {
        staticAudio = RadioController.instance.staticSource.clip;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            RadioController.instance.staticSource.Stop();
            RadioController.instance.staticSource.clip = stationAudio;
            RadioController.instance.staticSource.Play();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            RadioController.instance.staticSource.Stop();
            RadioController.instance.staticSource.clip = staticAudio;
            RadioController.instance.staticSource.Play();
        }
    }
}
