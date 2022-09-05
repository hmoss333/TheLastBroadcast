using System.Collections;
using System.Collections.Generic;
using UnityEngine;


class ConstantBufferVariable {
    public int     FrameIndex;
    public Vector2 FrameOffset;
    public float   GrainStrange;
    public float   VignetteStrange;
    public static void Apply(ComputeShader shader, ConstantBufferVariable buffer) {
        shader.SetInt("FrameIndex", buffer.FrameIndex);
        shader.SetVector("FrameOffset", buffer.FrameOffset);
        shader.SetFloat("GrainStrange", buffer.GrainStrange);
        shader.SetFloat("VignetteStrange", buffer.VignetteStrange);

    }
}




[RequireComponent(typeof(Camera)), ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class OldCinemaEffect : MonoBehaviour {

    [Range(0.0f, 1.0f)]   public float GrainStrange    = 0.2f;
    [Range(1.0f, 100.0f)] public float VignetteStrange = 1.0f;
    [Range(0.0f, 0.01f)]  public float JitterStrange = 0.0f;
 
    private ComputeShader          m_OldCinemaComputer;
    private Camera                 m_Camera;
    private RenderTexture          m_TextureOldCinema;
    private ConstantBufferVariable m_ConstantBuffer = new ConstantBufferVariable();
    private int                    m_FrameIndex = 0;
    private Vector2                m_FrameOffset = Vector2.zero;

    private void InitializeRenderTexture(int width, int height) {
        if (m_TextureOldCinema == null) {

            if (m_TextureOldCinema != null)
                m_TextureOldCinema.Release();

            m_TextureOldCinema = new RenderTexture(width, height, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            m_TextureOldCinema.enableRandomWrite = true;
            m_TextureOldCinema.Create();

        }
    }


    void Start() {
        m_Camera = GetComponent<Camera>();
        m_OldCinemaComputer = Resources.Load<ComputeShader>("Shaders/ComputerOldCinema");       
    }
    // Update is called once per frame
    void Update() {
        m_ConstantBuffer.FrameIndex = m_FrameIndex;
        m_ConstantBuffer.GrainStrange = GrainStrange;
        m_ConstantBuffer.VignetteStrange = 100 - VignetteStrange;
        m_ConstantBuffer.FrameOffset = m_FrameOffset;

        if (Input.GetKeyDown(KeyCode.F))
            ScreenCapture.CaptureScreenshot("Frame.png");

        m_FrameIndex++;
        m_FrameOffset = new Vector2(JitterStrange * Random.value, JitterStrange * Random.value);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination) {
        InitializeRenderTexture(m_Camera.pixelWidth, m_Camera.pixelHeight);
        ConstantBufferVariable.Apply(m_OldCinemaComputer, m_ConstantBuffer);

        var kernelOldCinema = m_OldCinemaComputer.FindKernel("OldCinema");
        m_OldCinemaComputer.SetTexture(kernelOldCinema, "TextureColorSRV", source);
        m_OldCinemaComputer.SetTexture(kernelOldCinema, "TextureColorUAV", m_TextureOldCinema);
        m_OldCinemaComputer.Dispatch(kernelOldCinema, Mathf.CeilToInt(m_Camera.pixelWidth / 8.0f), Mathf.CeilToInt(m_Camera.pixelHeight / 8.0f), 1);
        Graphics.Blit(m_TextureOldCinema, destination);

     
    }


    private void OnDestroy() {
        if (m_TextureOldCinema != null)
            m_TextureOldCinema.Release();
    }

    private void OnValidate()
    {
        
    }

}
