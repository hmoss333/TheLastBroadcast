using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostEffectController : MonoBehaviour
{
    [SerializeField] Shader postShader;
    Material postEffectMaterial;
    [SerializeField] float radius, feather;
    [SerializeField] Color tintColor;


    private void Awake()
    {
        postEffectMaterial = new Material(postShader);
    }

    private void Update()
    {
        radius = PlayerController.instance.stamina / 5f;
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        int width = src.width;
        int height = src.height;

        RenderTexture startRenderTexture = RenderTexture.GetTemporary(width, height);

        postEffectMaterial.SetFloat("_Radius", radius);
        postEffectMaterial.SetFloat("_Feather", feather);
        postEffectMaterial.SetColor("_TintColor", tintColor);
        Graphics.Blit(src, startRenderTexture, postEffectMaterial, 0);
        Graphics.Blit(startRenderTexture, dest);
        RenderTexture.ReleaseTemporary(startRenderTexture);
    }
}
