using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StaticBarrier : MonoBehaviour
{
    private BoxCollider col;
    private Renderer rend;
    [SerializeField] Material dissolveMat;
    [SerializeField] AudioSource collideAudio;
    Material tempMat;
    float dissolveVal;
    bool dissolving, activated;


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
        col.isTrigger = PlayerController.instance.invisible;
            
        if (dissolving)
        {
            tempMat.SetFloat("_DissolveAmount", dissolveVal += Time.deltaTime);

            if (dissolveVal >= 1f)
            {
                activated = true;
                dissolving = false;
            }
        }

        this.gameObject.SetActive(!activated);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !dissolving)
        {
            var materials = rend.sharedMaterials.ToList();
            materials.Add(tempMat);
            rend.materials = materials.ToArray();

            dissolving = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            StartCoroutine(CollisionEffect());
        }
    }

    IEnumerator CollisionEffect()
    {
        CamEffectController.instance.effectOn = true;
        collideAudio.mute = false;

        yield return new WaitForSeconds(0.15f);

        CamEffectController.instance.effectOn = false;
        collideAudio.mute = true;
    }
}
