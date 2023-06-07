using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InvisAbility : RadioAbilityController
{
    public static InvisAbility instance;


    [SerializeField] private bool isInvis;
    private int oldLayer;
    //private int voidLayer;
    [SerializeField] Material invisMat;
    private float checkFrequency;
    private float checkOffset = 0.5f;
    [SerializeField] float checkTime, invisTime;
    private float tempCheckTime = 0f, tempInvisTime = 0f;
    [HideInInspector] public bool isUsing; //used to toggle camera after effect for special abilities


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


    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    override public void Start()
    {
        base.Start();

        oldLayer = LayerMask.NameToLayer("Player");
        checkFrequency = abilityData.frequency;

        // Cache renderers
        renderers = GetComponentsInChildren<Renderer>();

        // Retrieve or generate smooth normals
        LoadSmoothNormals();

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

    private void Update()
    {
        if (RadioController.instance.abilityMode
            && abilityData.isActive
            && !isInvis)
        {
            if ((RadioController.instance.currentFrequency < checkFrequency + checkOffset && RadioController.instance.currentFrequency > checkFrequency - checkOffset)
                && SaveDataController.instance.saveData.abilities.radio == true //does the player have the radio object; useful if the player loses the radio at some point)                                                      
                && RadioController.instance.isActive) //is the radio active (shouldn't be broadcasting if it is not turned on))
            {
                isUsing = true;
                tempCheckTime += Time.deltaTime;
                if (tempCheckTime >= checkTime)
                {
                    AddMaterials();
                    isInvis = true;
                    tempCheckTime = 0;
                }
            }
            else
            {
                isUsing = false;
                tempCheckTime = 0;
            }
        }
        else if (!RadioController.instance.abilityMode && !isInvis)
        {
            //If the player turns off the radio before the ability has triggered
            isUsing = false;
        }

        //If the player is currently using the ability
        if (isInvis)
        {
            CameraController.instance.SetTarget(PlayerController.instance.gameObject);

            tempInvisTime += Time.deltaTime;
            if (tempInvisTime > invisTime)
            {
                PlayerController.instance.SetAbilityState(PlayerController.AbilityStates.none);
                RemoveMaterials();
                tempInvisTime = 0;
                isInvis = false;
                isUsing = false;
                CameraController.instance.LoadLastTarget();
            }
        }

        //Toggle player interaction state based on invisibility status
        if (isInvis)
            PlayerController.instance.SetAbilityState(PlayerController.AbilityStates.invisible);
    }

    void AddMaterials()
    {
        foreach (var renderer in renderers)
        {
            var materials = renderer.sharedMaterials.ToList();
            materials.Add(invisMat);
            renderer.materials = materials.ToArray();
        }
    }

    void RemoveMaterials()
    {
        foreach (var renderer in renderers)
        {
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
}
