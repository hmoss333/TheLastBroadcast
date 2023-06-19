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
    [SerializeField] AudioSource audioSource;


    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        tvLight = GetComponentInChildren<Light>().gameObject;
        audioSource = GetComponentInChildren<AudioSource>();
    }

    private void Start()
    {
        if (!active)
        {
            RemoveMaterial();
        }

        tvLight.SetActive(active);
        audioSource.mute = !active;
    }

    void AddMaterial()
    {
        var materials = meshRenderer.sharedMaterials.ToList();

        if (!materials.Contains(tvMat))
            materials.Add(tvMat);

        meshRenderer.materials = materials.ToArray();
    }

    void RemoveMaterial()
    {
        var materials = meshRenderer.sharedMaterials.ToList();

        if (materials.Contains(tvMat))
            materials.Remove(tvMat);

        meshRenderer.materials = materials.ToArray();
    }

    public override void Activate()
    {
        base.Activate();

        if (active)
            AddMaterial();
        else
            RemoveMaterial();

        tvLight.SetActive(active);
        audioSource.mute = !active;
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
