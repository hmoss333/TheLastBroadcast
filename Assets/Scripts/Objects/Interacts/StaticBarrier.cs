using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class StaticBarrier : SaveObject
{
    private BoxCollider col;
    private Renderer rend;
    [SerializeField] Material dissolveMat;
    [SerializeField] AudioSource collideAudio;
    Material tempMat;
    float dissolveVal;
    bool dissolving;


    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<BoxCollider>();
        rend = GetComponent<Renderer>();
        tempMat = new Material(dissolveMat);
        tempMat.SetFloat("_DissolveAmount", 0f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        col.isTrigger = PlayerController.instance.abilityState == PlayerController.AbilityStates.invisible;
            
        if (dissolving)
        {
            tempMat.SetFloat("_DissolveAmount", dissolveVal += Time.deltaTime);

            if (dissolveVal >= 1f)
            {
                dissolving = false;
                SetHasActivated();
            }
        }

        this.gameObject.SetActive(!hasActivated);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !dissolving)
        {
            var materials = rend.sharedMaterials.ToList();
            materials.Clear();
            materials.Add(tempMat);
            rend.materials = materials.ToArray();

            dissolving = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            CamEffectController.instance.ShockEffect(0.25f);
        }
    }
}
