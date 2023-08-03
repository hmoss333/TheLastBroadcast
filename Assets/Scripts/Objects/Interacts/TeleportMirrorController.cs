using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class TeleportMirrorController : InteractObject
{
    [SerializeField] Transform exitPoint;
    private RoomController exitRoom;
    //[SerializeField] float dissolveVal = 1f;


    [SerializeField] Material dissolveMat;
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


    private void Awake()
    {
        if (exitPoint)
            exitRoom = exitPoint.GetComponentInParent<RoomController>();
    }

    private void Start()
    {
        // Cache renderers
        renderers = PlayerController.instance.gameObject.GetComponentsInChildren<Renderer>();

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
        //Object cannot be interacted with unless the player has collected the mirror ability
        active = SaveDataController.instance.saveData.abilities.mirror;

        //if (interacting && dissolveVal > 0)
        //{
        //    dissolveVal -= Time.deltaTime;
        //    dissolveMat.SetFloat("_DissolveAmount", dissolveVal);
        //}
        //else if (!interacting && dissolveVal <= 1f)
        //{
        //    dissolveVal += Time.deltaTime;
        //    dissolveMat.SetFloat("_DissolveAmount", dissolveVal);
        //    if (dissolveVal >= 1)
        //        RemoveMaterials();
        //}
    }

    public override void StartInteract()
    {
        StartCoroutine(TeleportTrigger());
    }

    IEnumerator TeleportTrigger()
    {
        //Add dissolve material to Player
        AddMaterials();
        float dissolveVal = 1f;

        yield return new WaitForSeconds(0.5f);

        while (dissolveVal > 0)
        {
            dissolveVal -= Time.deltaTime;
            dissolveMat.SetFloat("_DissolveAmount", dissolveVal);

            yield return null;
        }

        print(dissolveVal);

        FadeController.instance.StartFade(1.0f, 0.5f);

        while (FadeController.instance.isFading)
            yield return null;

        PlayerController.instance.transform.position = exitPoint.position;       
        PlayerController.instance.SetLastDir(exitPoint.transform.forward);
        CameraController.instance.transform.position = exitPoint.position;
        transform.GetComponentInParent<RoomController>().gameObject.SetActive(false);

        if (exitRoom)
            exitRoom.gameObject.SetActive(true);

        FadeController.instance.StartFade(0.0f, 0.5f);

        while (dissolveVal < 1f)
        {
            print(dissolveVal);
            dissolveVal += Time.deltaTime;
            dissolveMat.SetFloat("_DissolveAmount", dissolveVal);

            yield return null;
        }

        RemoveMaterials();

        interacting = false;
        PlayerController.instance.SetState(PlayerController.States.idle);
    }

    void AddMaterials()
    {
        foreach (var renderer in renderers)
        {
            // Append outline shaders
            var materials = renderer.sharedMaterials.ToList();
            materials.Add(dissolveMat);
            renderer.materials = materials.ToArray();
        }
    }

    void RemoveMaterials()
    {
        foreach (var renderer in renderers)
        {
            // Remove outline shaders
            var materials = renderer.sharedMaterials.ToList();
            materials.Remove(dissolveMat);
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
