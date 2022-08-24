using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TVController : SavePointController
{
    [SerializeField] Material tvMat;
    MeshRenderer renderer;


    private void Awake()
    {
        renderer = GetComponent<MeshRenderer>();

        if (active && renderer != null)
            AddMaterial();
    }

    void AddMaterial()
    {
        var materials = renderer.sharedMaterials.ToList();
        foreach (Material mat in materials)
        {
            if (mat == tvMat)
                return;
        }

        materials.Add(tvMat);

        renderer.materials = materials.ToArray();
    }

    public override void Activate()
    {
        base.Activate();

        if (renderer != null)
            AddMaterial();
    }
}
