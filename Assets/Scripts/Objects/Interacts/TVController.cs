using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
}
