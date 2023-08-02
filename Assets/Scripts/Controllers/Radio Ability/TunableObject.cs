using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TunableObject : MonoBehaviour
{
    private static HashSet<Mesh> registeredMeshes = new HashSet<Mesh>();

    [Serializable]
    private class ListVector3
    {
        public List<Vector3> data;
    }

    [SerializeField, HideInInspector]
    private List<Mesh> bakeKeys = new List<Mesh>();

    [SerializeField, HideInInspector]
    private List<ListVector3> bakeValues = new List<ListVector3>();

    private Renderer[] renderers;
    [SerializeField] private Material staticMaterial;
    [SerializeField] bool showStatic;

    private bool needsUpdate;
    SaveObject baseObject;


    private float checkRadius = 4.0f; //how far away the player needs to be in order for the door control to recognize the radio signal
    [SerializeField] private float checkTime = 3f; //time the radio must stay within the frequency range to activate
    private float tempTime = 0f;
    private float checkFrequency, chargeCost; //frequency that must be matched on field radio
    private float checkOffset = 0.5f; //offset amount for matching with the current field radio frequency


    void Awake()
    {
        baseObject = gameObject.GetComponent<SaveObject>();

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

    void OnEnable()
    {
        foreach (var renderer in renderers)
        {
            // Append outline shaders
            var materials = renderer.sharedMaterials.ToList();

            materials.Add(staticMaterial);

            renderer.materials = materials.ToArray();
        }

        showStatic = true;

        //Disable script if base object has already been activated
        if (baseObject && (baseObject.active || baseObject.hasActivated))
            enabled = false;
    }

    void Update()
    {
        checkFrequency = TuneAbility.instance.abilityData.frequency;
        chargeCost = SaveDataController.instance.GetRadioAbility("Tune").chargeCost;

        if (needsUpdate)
        {
            needsUpdate = false;
            UpdateMaterialProperties();
        }

        if (RadioController.instance.abilityMode)
        {
            float dist = Vector3.Distance(transform.position, PlayerController.instance.transform.position);

            if (dist <= checkRadius)
            {
                if ((RadioController.instance.currentFrequency < checkFrequency + checkOffset && RadioController.instance.currentFrequency > checkFrequency - checkOffset)
                    && SaveDataController.instance.saveData.abilities.radio == true //does the player have the radio object; useful if the player loses the radio at some point)                                                      
                    && RadioController.instance.isActive) //is the radio active (shouldn't be broadcasting if it is not turned on))
                {
                    CameraController.instance.SetTarget(this.transform); //If the radio is set to the correct station, focus on tunable object
                    TuneAbility.instance.isUsing = true;

                    tempTime += Time.deltaTime;
                    if (tempTime > checkTime)
                    {
                        RadioController.instance.ModifyCharge(-chargeCost);
                        StartCoroutine(ActivateObject());
                    }
                }
                else
                {
                    CameraController.instance.SetTarget(PlayerController.instance.lookTransform);//.gameObject); //If the radio is not set to the correct station, re-focus the camera on the player
                    tempTime = 0f;
                    TuneAbility.instance.isUsing = false;
                }
            }
        }
        else
        {
            //If the player disables the ability radio during tuning, reset camera to player
            if (CameraController.instance.GetTarget() == this.gameObject.transform)
                CameraController.instance.SetTarget(PlayerController.instance.lookTransform);

            TuneAbility.instance.isUsing = false;
        }

        if (baseObject)
            enabled = !baseObject.active; //tunable script is disabled when base object is activated

        if (showStatic)
        {
            foreach (var renderer in renderers)
            {
                var mats = renderer.sharedMaterials.ToList();

                foreach (Material mat in mats)
                {
                    mat.SetFloat("_DissolveAmount", Mathf.Sin(Time.time) / 2.5f + 0.6f);
                }
            }
        }
    }

    void OnDisable()
    {
        foreach (var renderer in renderers)
        {
            // Remove outline shaders
            var materials = renderer.sharedMaterials.ToList();

            materials.Remove(staticMaterial);

            renderer.materials = materials.ToArray();
        }

        showStatic = false;
    }

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
        staticMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.LessEqual);
        staticMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Greater);
        staticMaterial.SetFloat("_OutlineWidth", 0);
    }

    IEnumerator ActivateObject()
    {
        Debug.Log($"Activating {baseObject.name}");
        baseObject.Activate();

        yield return new WaitForSeconds(1f);

        TuneAbility.instance.isUsing = false;

        yield return new WaitForSeconds(1.25f);

        CameraController.instance.SetTarget(PlayerController.instance.lookTransform);
    }
}
