using UnityEngine;
using System.Collections;

public class ClearSight : MonoBehaviour
{
    public float DistanceToPlayer = 5.0f;
    [SerializeField] LayerMask layerToFade;


    void Update()
    {
        RaycastHit[] hits;
        Vector3 forwardDir = PlayerController.instance.transform.position - transform.position;
        hits = Physics.RaycastAll(transform.position, forwardDir, DistanceToPlayer, layerToFade);
        Debug.DrawRay(transform.position, forwardDir);


        foreach (RaycastHit hit in hits)
        {
            Renderer R = hit.collider.GetComponent<Renderer>();

            if (R == null || CameraController.instance.isFocused())
                continue; // no renderer attached? go to next hit
                          // TODO: maybe implement here a check for GOs that should not be affected like the player

            AutoTransparent AT = R.GetComponent<AutoTransparent>();
            if (AT == null) // if no script is attached, attach one
            {
                AT = R.gameObject.AddComponent<AutoTransparent>();
            }
            AT.BeTransparent(); // get called every frame to reset the falloff
        }
    }
}
