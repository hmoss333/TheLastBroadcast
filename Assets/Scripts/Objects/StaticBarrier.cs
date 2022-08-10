using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StaticBarrier : InteractObject
{
    private BoxCollider collider;
    private Renderer renderer;
    [SerializeField] Material dissolveMat;
    Material tempMat;
    float dissolveVal;
    bool dissolving;


    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<BoxCollider>();
        renderer = GetComponent<Renderer>();
        tempMat = new Material(dissolveMat);
        tempMat.SetFloat("_DissolveAmount", 0f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        collider.isTrigger = PlayerController.instance.invisible;
            
        if (dissolving)
        {
            tempMat.SetFloat("_DissolveAmount", dissolveVal += Time.deltaTime);

            if (dissolveVal >= 1f)
            {
                hasActivated = true;
                dissolving = false;
            }
        }

        this.gameObject.SetActive(!hasActivated);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !dissolving)
        {
            var materials = renderer.sharedMaterials.ToList();
            materials.Add(tempMat);
            renderer.materials = materials.ToArray();

            dissolving = true;
        }
    }
}
