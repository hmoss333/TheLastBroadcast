using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerPoisonFog : SaveObject
{
    [SerializeField] private int damage;
    [SerializeField] private bool gasmask;

    private void Update()
    {
        gameObject.SetActive(active);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            gasmask = SaveDataController.instance.saveData.abilities.gasmask;
            other.GetComponent<PlayerController>().ToggleGasMask(gasmask);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player" && !gasmask)
        {
            other.GetComponent<Health>().Hurt(damage, false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<PlayerController>().ToggleGasMask(false);
        }
    }
}
