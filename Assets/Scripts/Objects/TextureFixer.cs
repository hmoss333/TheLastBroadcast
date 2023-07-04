using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureFixer : MonoBehaviour
{
    Mesh mesh;
    private Material mat;

    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponent<Renderer>().material;
        mesh = GetComponent<MeshFilter>().mesh;

        mat.mainTextureScale = new Vector2((mesh.bounds.size.x * transform.localScale.x), (mesh.bounds.size.y * transform.localScale.y));
    }
}
