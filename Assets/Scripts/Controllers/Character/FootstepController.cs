using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepController : MonoBehaviour
{
    [Header("Footstep Types")]
    [SerializeField] AudioClip concreteClip;
    [SerializeField] AudioClip glassClip;
    [SerializeField] AudioClip metalClip;
    [SerializeField] AudioClip grassClip;

    [Header("Sources")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] AudioClip clip;
    [SerializeField] AudioSource audioSource;


    private void Update()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;
        Debug.DrawRay(transform.position, Vector3.down, Color.red);

        if (Physics.Raycast(ray, out hit, 1.5f, groundLayer))
        {
            LayerMask tempLayer = hit.transform.gameObject.layer;
            switch (tempLayer.value)
            {
                case 16:
                    clip = concreteClip;
                    break;
                case 17:
                    clip = glassClip;
                    break;
                case 18:
                    clip = grassClip;
                    break;
                case 19:
                    clip = metalClip;
                    break;
                default:
                    clip = concreteClip;
                    break;
            }
        }
    }


    public void PlayStepClip()
    {
        print($"Playing footstep clip: {clip.name}");
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();
    }
}
