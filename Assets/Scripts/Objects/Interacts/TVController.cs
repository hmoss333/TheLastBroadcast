using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TVController : SavePointController
{
    [SerializeField] Material tvMat;
    MeshRenderer meshRenderer;
    GameObject tvLight;


    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        tvLight = GetComponentInChildren<Light>().gameObject;
    }

    private void Start()
    {
        if (!active)
        {
            var materials = meshRenderer.sharedMaterials.ToList();

            if (materials.Contains(tvMat))
                materials.Remove(tvMat);

            meshRenderer.materials = materials.ToArray();
        }

        tvLight.SetActive(active);
    }

    void AddMaterial()
    {
        var materials = meshRenderer.sharedMaterials.ToList();

        if (!materials.Contains(tvMat))
            materials.Add(tvMat);

        meshRenderer.materials = materials.ToArray();
    }

    public override void Activate()
    {
        base.Activate();

        if (meshRenderer != null)
            AddMaterial();

        tvLight.SetActive(true);
    }

    public override void Interact()
    {
        if (active)
        {
            interacting = !interacting;
            PlayerController.instance.GetComponent<Health>().SetHealth(5); //set health back to max

            if (interacting)
            {
                Debug.Log("Saving game");
                SaveDataController.instance.SetSavePoint(SceneManager.GetActiveScene().name, ID);
            }
        }
    }
}
