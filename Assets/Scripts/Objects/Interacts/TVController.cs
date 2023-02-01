using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TVController : SavePointController
{
    [SerializeField] Material tvMat;
    MeshRenderer renderer;
    GameObject light;


    private void Awake()
    {
        renderer = GetComponent<MeshRenderer>();
        light = GetComponentInChildren<Light>().gameObject;
    }

    private void Start()
    {
        if (!active)
        {
            var materials = renderer.sharedMaterials.ToList();

            if (materials.Contains(tvMat))
                materials.Remove(tvMat);

            renderer.materials = materials.ToArray();
        }

        light.SetActive(active);
    }

    void AddMaterial()
    {
        var materials = renderer.sharedMaterials.ToList();

        if (!materials.Contains(tvMat))
            materials.Add(tvMat);

        renderer.materials = materials.ToArray();
    }

    public override void Activate()
    {
        base.Activate();

        if (renderer != null)
            AddMaterial();

        light.SetActive(true);
    }

    public override void Interact()
    {
        if (active)
        {
            interacting = !interacting;
            //layerController.instance.InteractToggle(interacting);
            PlayerController.instance.GetComponent<Health>().SetHealth(5); //set health back to max

            if (interacting)
            {
                //Debug.Log("Saving game");
                UIController.instance.DialogueUI("Saving game", 1.5f);
                SaveDataController.instance.SetSavePoint(SceneManager.GetActiveScene().name, ID);
                SaveDataController.instance.SaveObjectData(SceneManager.GetActiveScene().name);
                SaveDataController.instance.SaveFile();
            }
        }
    }
}
