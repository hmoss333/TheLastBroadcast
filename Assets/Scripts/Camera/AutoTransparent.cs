using UnityEngine;
using System.Collections;

public class AutoTransparent : MonoBehaviour
{
    private Shader m_OldShader;
    private Color m_OldColor;
    private double m_Transparency = 0.3f;
    private float m_TargetTransparancy = 0.5f;
    private float m_FallOff = 0.5f; // returns to 100% in 0.5 sec
    private bool shouldBeTransparent = true;

    public void BeTransparent()
    {
        shouldBeTransparent = true;
    }

    void Start()
    {
        if (GetComponent<Renderer>().material)
        {
            // Save the current shader
            m_OldShader = GetComponent<Renderer>().material.shader;
            GetComponent<Renderer>().material.shader = Shader.Find("Transparent/Diffuse");
            if (GetComponent<Renderer>().material.HasProperty("_Color"))
            {
                m_OldColor = GetComponent<Renderer>().material.color;
                m_Transparency = m_OldColor.a;
            }
            else
            {
                m_OldColor = Color.white;
                m_Transparency = 1.0f;
            }
        }
        else
        {
            m_Transparency = 1.0f;
        }
    }

    void OnDestroy()
    {
        if (!m_OldShader) return;
        // Reset the shader
        GetComponent<Renderer>().material.shader = m_OldShader;
        GetComponent<Renderer>().material.color = m_OldColor;
    }

    void Update()
    {
        //Are we fading in our out?
        if (shouldBeTransparent)
        {
            //Fading out
            if (m_Transparency >= m_TargetTransparancy)
                 m_Transparency -= ((1.0 - m_TargetTransparancy) * Time.deltaTime) / m_FallOff;
        }
        else
        {
            //Fading in
            m_Transparency += ((1.0 - m_TargetTransparancy) * Time.deltaTime) / m_FallOff;

            if (m_Transparency >= m_OldColor.a)
                Destroy(this);
        }

        Color newColor = new Color(GetComponent<Renderer>().material.color.r, GetComponent<Renderer>().material.color.g, GetComponent<Renderer>().material.color.b, (float)m_Transparency);
        GetComponent<Renderer>().material.color = newColor;
        //The object will start to become visible again if BeTransparent() is not called
        shouldBeTransparent = false;
    }


}
