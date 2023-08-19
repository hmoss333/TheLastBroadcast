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
            PlayerController.instance.GetComponent<Health>().SetHealth(SaveDataController.instance.saveData.maxHealth); //set health back to max
            RadioController.instance.ModifyCharge(SaveDataController.instance.saveData.maxCharge); //set charge back to max

            //Focus on TV screen
            PlayerController.instance.ToggleAvatar();
            CameraController.instance.SetTarget(interacting ? focusPoint : PlayerController.instance.lookTransform);
            CameraController.instance.FocusTarget();

            //Display 'Saving...' text
            UIController.instance.SetDialogueText("Saving...", true);
            UIController.instance.ToggleDialogueUI(interacting);

            if (interacting)
            {
                Debug.Log("Saving game");
                SaveDataController.instance.SetSavePoint(SceneManager.GetActiveScene().name, ID);
                SaveDataController.instance.SaveFile();
                SaveDataController.instance.SaveObjectData();
            }
        }
    }
}
