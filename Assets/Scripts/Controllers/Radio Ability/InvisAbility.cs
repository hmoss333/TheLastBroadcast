using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InvisAbility : RadioAbilityController
{
    [SerializeField] private bool isInvis;

    private int oldLayer;
    private int voidLayer;
    [SerializeField] Material invisMat;

    private float checkFrequency;
    private float checkOffset = 0.5f;
    [SerializeField] float checkTime;
    [SerializeField] float invisTime;
    private float tempTime;



    ////Invis Material Values////
    private static HashSet<Mesh> registeredMeshes = new HashSet<Mesh>();

    private class ListVector3
    {
        public List<Vector3> data;
    }

    [SerializeField, HideInInspector]
    private List<Mesh> bakeKeys = new List<Mesh>();

    [SerializeField, HideInInspector]
    private List<ListVector3> bakeValues = new List<ListVector3>();

    private Renderer[] renderers;

    private bool needsUpdate;


    void Awake()
    {
        tempTime = checkTime;

        // Cache renderers
        renderers = GetComponentsInChildren<Renderer>();

        // Retrieve or generate smooth normals
        LoadSmoothNormals();

        // Apply material properties immediately
        needsUpdate = true;

        foreach (var skinnedMeshRenderer in GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            if (skinnedMeshRenderer.sharedMesh.subMeshCount > 1)
            {
                skinnedMeshRenderer.sharedMesh.subMeshCount = skinnedMeshRenderer.sharedMesh.subMeshCount + 1;
                skinnedMeshRenderer.sharedMesh.SetTriangles(skinnedMeshRenderer.sharedMesh.triangles, skinnedMeshRenderer.sharedMesh.subMeshCount - 1);
            }

        }

        foreach (var meshFilter in GetComponentsInChildren<MeshFilter>())
        {
            if (meshFilter.mesh.subMeshCount > 1)
            {
                //meshFilter.sharedMesh.subMeshCount = meshFilter.sharedMesh.subMeshCount + 1;
                meshFilter.mesh.subMeshCount = meshFilter.mesh.subMeshCount + 1;
                meshFilter.mesh.SetTriangles(meshFilter.mesh.triangles, meshFilter.mesh.subMeshCount - 1);
            }
        }
    }

    void Start()
    {
        base.Start();

        oldLayer = LayerMask.NameToLayer("Player");
        voidLayer = LayerMask.NameToLayer("Void");
        tempTime = invisTime;
        checkFrequency = abilityData.frequency;
    }

    private void Update()
    {
        if (RadioController.instance.abilityMode && !isInvis)
        {
            if ((RadioController.instance.currentFrequency < checkFrequency + checkOffset && RadioController.instance.currentFrequency > checkFrequency - checkOffset)
                && SaveDataController.instance.saveData.abilities.radio == true //does the player have the radio object; useful if the player loses the radio at some point)                                                      
                && RadioController.instance.isActive) //is the radio active (shouldn't be broadcasting if it is not turned on))
            {
                checkTime -= Time.deltaTime;
                if (checkTime < 0)
                {
                    DisableCollider();
                    isInvis = true;
                    checkTime = 3f;
                }
            }
            else
            {
                checkTime = 3f;
            }
        }

        if (isInvis)
        {
            PlayerController.instance.usingRadio = false;
            RadioController.instance.isActive = false;

            invisTime -= Time.deltaTime;
            if (invisTime < 0)
            {
                EnableCollider();
                isInvis = false;
                invisTime = tempTime;
            }
        }

        PlayerController.instance.invisible = isInvis;
    }

    public void DisableCollider()
    {
        gameObject.layer = voidLayer;

        foreach (var renderer in renderers)
        {
            // Append outline shaders
            var materials = renderer.sharedMaterials.ToList();

            materials.Add(invisMat);

            renderer.materials = materials.ToArray();
        }
    }

    public void EnableCollider()
    {
        gameObject.layer = oldLayer;

        foreach (var renderer in renderers)
        {
            // Remove outline shaders
            var materials = renderer.sharedMaterials.ToList();

            materials.Remove(invisMat);

            renderer.materials = materials.ToArray();
        }
    }



    ////Invis Material Functions////
    void LoadSmoothNormals()
    {
        // Retrieve or generate smooth normals
        foreach (var meshFilter in GetComponentsInChildren<MeshFilter>())
        {

            // Skip if smooth normals have already been adopted
            if (!registeredMeshes.Add(meshFilter.sharedMesh))
            {
                continue;
            }

            // Retrieve or generate smooth normals
            var index = bakeKeys.IndexOf(meshFilter.mesh);
            var smoothNormals = (index >= 0) ? bakeValues[index].data : SmoothNormals(meshFilter.mesh);

            // Store smooth normals in UV3
            meshFilter.sharedMesh.SetUVs(3, smoothNormals);
        }

        // Clear UV3 on skinned mesh renderers
        foreach (var skinnedMeshRenderer in GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            if (registeredMeshes.Add(skinnedMeshRenderer.sharedMesh))
            {
                skinnedMeshRenderer.sharedMesh.uv4 = new Vector2[skinnedMeshRenderer.sharedMesh.vertexCount];
            }
        }
    }

    List<Vector3> SmoothNormals(Mesh mesh)
    {
        // Group vertices by location
        var groups = mesh.vertices.Select((vertex, index) => new KeyValuePair<Vector3, int>(vertex, index)).GroupBy(pair => pair.Key);

        // Copy normals to a new list
        var smoothNormals = new List<Vector3>(mesh.normals);

        // Average normals for grouped vertices
        foreach (var group in groups)
        {

            // Skip single vertices
            if (group.Count() == 1)
            {
                continue;
            }

            // Calculate the average normal
            var smoothNormal = Vector3.zero;

            foreach (var pair in group)
            {
                smoothNormal += mesh.normals[pair.Value];
            }

            smoothNormal.Normalize();

            // Assign smooth normal to each vertex
            foreach (var pair in group)
            {
                smoothNormals[pair.Value] = smoothNormal;
            }
        }

        return smoothNormals;
    }

    void UpdateMaterialProperties()
    {
        invisMat.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.LessEqual);
        invisMat.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Greater);
        invisMat.SetFloat("_OutlineWidth", 0);
    }
}
